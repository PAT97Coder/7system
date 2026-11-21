using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace KnowledgeSystem.Helpers
{
    internal sealed class FuelPhotoReportEntry
    {
        public string Department { get; set; }
        public string LicensePlate { get; set; }
        public DateTime FuelDate { get; set; }
        public int? OdometerReading { get; set; }
        public decimal Quantity { get; set; }
        public string PhotoPath { get; set; }
    }

    internal static class FuelPhotoReportExporter
    {
        // Mỗi xe có một danh sách liên tục, phần ký chỉ xuất hiện sau bản ghi cuối.
        internal static void Export(IList<FuelPhotoReportEntry> entries, string outputPath, DateTime reportDate)
        {
            if (entries == null || entries.Count == 0)
                throw new InvalidOperationException("沒有可匯出的加油資料。");

            var groups = entries.GroupBy(row => new { row.Department, row.LicensePlate })
                .Select(group => group.OrderBy(row => row.FuelDate).ToList())
                .ToList();

            using (var stream = new MemoryStream())
            {
                using (var template = typeof(FuelPhotoReportExporter).Assembly.GetManifestResourceStream("KnowledgeSystem.Templates.FuelPhotoReport.docx"))
                {
                    if (template == null) throw new FileNotFoundException("找不到加油照片報表範本。");
                    template.CopyTo(stream);
                }
                stream.Position = 0;
                using (var word = WordprocessingDocument.Open(stream, true))
                {
                    var body = word.MainDocumentPart.Document.Body;
                    var section = (W.SectionProperties)body.Elements<W.SectionProperties>().Last().CloneNode(true);
                    var contents = body.ChildElements.Where(element => !(element is W.SectionProperties))
                        .Select(element => element.CloneNode(true)).ToList();
                    // Tổng trang tính theo section của từng xe, thay vì toàn bộ báo cáo.
                    foreach (var footer in word.MainDocumentPart.FooterParts)
                    {
                        foreach (var field in footer.Footer.Descendants<W.FieldCode>())
                            field.Text = System.Text.RegularExpressions.Regex.Replace(field.Text,
                                @"\bNUMPAGES\b", "SECTIONPAGES", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        foreach (var field in footer.Footer.Descendants<W.SimpleField>())
                            if (field.Instruction != null)
                                field.Instruction.Value = System.Text.RegularExpressions.Regex.Replace(field.Instruction.Value,
                                    @"\bNUMPAGES\b", "SECTIONPAGES", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        footer.Footer.Save();
                    }
                    body.RemoveAllChildren();
                    for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
                    {
                        var pageElements = contents.Select(element => element.CloneNode(true)).ToList();
                        var tables = pageElements.OfType<W.Table>().ToList();
                        var rows = groups[groupIndex];
                        var first = rows[0];
                        SetCell(tables[0], 1, 1, first.Department);
                        string firstMonth = first.FuelDate.ToString("yyyy/MM", CultureInfo.InvariantCulture);
                        string lastMonth = rows.Last().FuelDate.ToString("yyyy/MM", CultureInfo.InvariantCulture);
                        SetCell(tables[0], 1, 3, firstMonth == lastMonth ? firstMonth : firstMonth + "–" + lastMonth);
                        SetCell(tables[0], 2, 1, first.LicensePlate);
                        SetCell(tables[0], 2, 3, reportDate.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
                        SetCell(tables[1], 1, 0, "序號");
                        var photoTable = tables[1];
                        var rowTemplate = (W.TableRow)photoTable.Elements<W.TableRow>().ElementAt(2).CloneNode(true);
                        foreach (var sampleRow in photoTable.Elements<W.TableRow>().Skip(2).ToList())
                            sampleRow.Remove();
                        // Lặp tiêu đề cột khi danh sách xe kéo dài qua nhiều trang.
                        foreach (var header in photoTable.Elements<W.TableRow>())
                        {
                            if (header.TableRowProperties == null) header.TableRowProperties = new W.TableRowProperties();
                            header.TableRowProperties.Append(new W.TableHeader());
                        }
                        for (int index = 0; index < rows.Count; index++)
                        {
                            var row = rows[index];
                            photoTable.Append(rowTemplate.CloneNode(true));
                            SetCell(photoTable, index + 2, 0, (index + 1).ToString());
                            SetCell(photoTable, index + 2, 1, row.FuelDate.ToString("MM/dd", CultureInfo.InvariantCulture));
                            SetCell(photoTable, index + 2, 2, row.OdometerReading?.ToString("N0", CultureInfo.InvariantCulture));
                            SetCell(photoTable, index + 2, 3, row.Quantity.ToString("0.###", CultureInfo.InvariantCulture));
                            SetCell(photoTable, index + 2, 4, "{{FUEL_PHOTO_" + groupIndex + "_" + index + "}}");
                        }
                        // Giữ bản ghi cuối và vùng ký cùng trang.
                        var signatureIndex = pageElements.IndexOf(tables[2]);
                        var photoTableIndex = pageElements.IndexOf(photoTable);
                        var keepWithSignature = photoTable.Elements<W.TableRow>().Last().Descendants<W.Paragraph>()
                            .Concat(pageElements.Skip(photoTableIndex + 1).Take(signatureIndex - photoTableIndex)
                                .SelectMany(element => element is W.Paragraph
                                    ? new[] { (W.Paragraph)element } : element.Descendants<W.Paragraph>()));
                        foreach (var paragraph in keepWithSignature)
                        {
                            if (paragraph.ParagraphProperties == null) paragraph.ParagraphProperties = new W.ParagraphProperties();
                            paragraph.ParagraphProperties.KeepNext = new W.KeepNext();
                        }
                        // Không để dữ liệu mẫu màu đỏ hoặc hàng ảnh bị tách qua trang.
                        foreach (var row in tables.SelectMany(table => table.Elements<W.TableRow>()))
                        {
                            if (row.TableRowProperties == null) row.TableRowProperties = new W.TableRowProperties();
                            row.TableRowProperties.Append(new W.CantSplit());
                        }
                        body.Append(pageElements);
                        var vehicleSection = (W.SectionProperties)section.CloneNode(true);
                        vehicleSection.RemoveAllChildren<W.SectionType>();
                        vehicleSection.RemoveAllChildren<W.PageNumberType>();
                        vehicleSection.InsertBefore(new W.SectionType { Val = W.SectionMarkValues.NextPage },
                            vehicleSection.GetFirstChild<W.PageSize>());
                        vehicleSection.InsertBefore(new W.PageNumberType { Start = 1 },
                            vehicleSection.GetFirstChild<W.Columns>());
                        if (groupIndex < groups.Count - 1)
                            body.Append(new W.Paragraph(new W.ParagraphProperties(vehicleSection)));
                        else
                            body.Append(vehicleSection);
                    }
                    word.MainDocumentPart.Document.Save();
                }
                stream.Position = 0;
                using (var server = new RichEditDocumentServer())
                {
                    server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    var document = server.Document;
                    for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
                        for (int index = 0; index < groups[groupIndex].Count; index++)
                        {
                            var range = document.FindAll("{{FUEL_PHOTO_" + groupIndex + "_" + index + "}}", SearchOptions.None).Single();
                            var position = range.Start;
                            document.Delete(range);
                            string path = groups[groupIndex][index].PhotoPath;
                            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                            {
                                document.InsertText(position, "未上傳加油照片");
                                continue;
                            }
                            using (var image = Image.FromFile(path))
                            {
                                var inserted = document.Images.Insert(position, DocumentImageSource.FromFile(path));
                                // Ô rộng 263pt, cao 170pt; chỉ chừa khoảng 2pt quanh ảnh.
                                // RichEdit dùng đơn vị 1/300 inch.
                                float scale = Math.Min(1080f / image.Width, 680f / image.Height);
                                inserted.Size = new SizeF(image.Width * scale, image.Height * scale);
                            }
                        }
                    document.Fields.Update();
                    server.ExportToPdf(outputPath);
                }
            }
        }

        private static void SetCell(W.Table table, int rowIndex, int columnIndex, string value)
        {
            var cell = table.Elements<W.TableRow>().ElementAt(rowIndex).Elements<W.TableCell>().ElementAt(columnIndex);
            var paragraph = cell.Elements<W.Paragraph>().FirstOrDefault();
            var properties = paragraph?.ParagraphProperties?.CloneNode(true);
            var templateRun = paragraph?.Elements<W.Run>().FirstOrDefault(run => run.RunProperties != null);
            var runProperties = templateRun == null
                ? new W.RunProperties()
                : (W.RunProperties)templateRun.RunProperties.CloneNode(true);
            if (templateRun == null && paragraph?.ParagraphProperties?.ParagraphMarkRunProperties != null)
                foreach (var property in paragraph.ParagraphProperties.ParagraphMarkRunProperties.ChildElements)
                    runProperties.Append(property.CloneNode(true));
            // Giữ font, cỡ chữ và kiểu chữ của ô Word; dữ liệu thực dùng màu đen.
            runProperties.Color = new W.Color { Val = "000000" };
            cell.RemoveAllChildren<W.Paragraph>();
            var replacement = new W.Paragraph();
            if (properties != null) replacement.Append(properties);
            if (value != null && value.StartsWith("{{FUEL_PHOTO_", StringComparison.Ordinal))
            {
                if (cell.TableCellProperties == null) cell.TableCellProperties = new W.TableCellProperties();
                cell.TableCellProperties.TableCellMargin = new W.TableCellMargin(
                    new W.TopMargin { Width = "0", Type = W.TableWidthUnitValues.Dxa },
                    new W.LeftMargin { Width = "20", Type = W.TableWidthUnitValues.Dxa },
                    new W.BottomMargin { Width = "0", Type = W.TableWidthUnitValues.Dxa },
                    new W.RightMargin { Width = "20", Type = W.TableWidthUnitValues.Dxa });
                if (replacement.ParagraphProperties == null) replacement.ParagraphProperties = new W.ParagraphProperties();
                replacement.ParagraphProperties.SpacingBetweenLines = new W.SpacingBetweenLines
                {
                    Before = "0", After = "0", Line = "240", LineRule = W.LineSpacingRuleValues.Auto
                };
                replacement.ParagraphProperties.SnapToGrid = new W.SnapToGrid { Val = false };
            }
            replacement.Append(new W.Run(runProperties,
                new W.Text(value ?? "") { Space = SpaceProcessingModeValues.Preserve }));
            cell.Append(replacement);
        }
    }
}
