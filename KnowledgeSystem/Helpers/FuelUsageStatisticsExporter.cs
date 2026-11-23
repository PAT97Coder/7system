using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KnowledgeSystem.Helpers
{
    internal sealed class FuelUsageDetail
    {
        public string Department { get; set; }
        public DateTime Date { get; set; }
        public string Invoice { get; set; }
        public string Plate { get; set; }
        public decimal? Quantity { get; set; }
        public string FuelCategory { get; set; }
        public string FuelName { get; set; }
        public string VehicleType { get; set; }
    }

    internal static class FuelUsageStatisticsExporter
    {
        private static readonly string[] VehicleSheets = { "摩托車", "汽車", "貨卡車", "推高車" };

        internal static void Export(string templatePath, string outputPath, int year, string departmentTitle, IList<FuelUsageDetail> rows)
        {
            if (rows == null || rows.Count == 0) throw new InvalidOperationException("所選年度沒有燃料資料。");
            var invalid = rows.FirstOrDefault(row => row.Date.Year != year || string.IsNullOrWhiteSpace(row.Department)
                || !row.Quantity.HasValue || row.Quantity < 0
                || (row.FuelCategory != "汽油" && row.FuelCategory != "柴油"));
            if (invalid != null)
                throw new InvalidOperationException($"發票 {invalid.Invoice} 的年度、部門、燃料分類或數量不完整，請先確認資料。");

            using (var package = new ExcelPackage(new FileInfo(templatePath)))
            {
                var missing = VehicleSheets.Concat(new[] { "統計表" }).Where(name => package.Workbook.Worksheets[name] == null).ToList();
                if (missing.Count > 0) throw new InvalidOperationException("報表範本缺少工作表：" + string.Join("、", missing));
                var detailSheets = VehicleSheets.ToList();
                if (rows.Any(row => !VehicleSheets.Contains(row.VehicleType)))
                {
                    detailSheets.Add("未分類");
                    if (package.Workbook.Worksheets["未分類"] == null)
                        package.Workbook.Worksheets.Add("未分類", package.Workbook.Worksheets[VehicleSheets[0]]);
                }
                var detailEnds = new Dictionary<string, int>();
                foreach (string name in detailSheets)
                {
                    var sheet = package.Workbook.Worksheets[name];
                    var rowStyle = CaptureRow(sheet, 3, 11);
                    ClearBody(sheet);
                    sheet.Cells[1, 1].Value = UpdateTitle(sheet.Cells[1, 1].Text, year, departmentTitle, name);
                    var details = rows.Where(row => SheetName(row.VehicleType) == name)
                        .OrderBy(row => row.Department).ThenBy(row => row.Date).ThenBy(row => row.Invoice).ToList();
                    for (int index = 0; index < details.Count; index++)
                    {
                        var item = details[index];
                        int row = index + 3;
                        ApplyRow(sheet, row, rowStyle);
                        sheet.Cells[row, 1].Value = DepartmentValue(item.Department);
                        sheet.Cells[row, 2].Value = index + 1;
                        sheet.Cells[row, 3].Value = item.Date.Month;
                        sheet.Cells[row, 4].Value = item.Date.Month;
                        sheet.Cells[row, 5].Value = item.Date;
                        sheet.Cells[row, 6].Value = item.Invoice;
                        sheet.Cells[row, 7].Value = item.Plate;
                        sheet.Cells[row, 8].Value = item.Quantity.Value;
                        sheet.Cells[row, 9].Value = string.IsNullOrWhiteSpace(item.VehicleType) ? "未設定車種" : item.VehicleType;
                        sheet.Cells[row, 10].Value = string.IsNullOrWhiteSpace(item.FuelName) ? item.FuelCategory : item.FuelName;
                        sheet.Cells[row, 11].Formula = "H" + row;
                        sheet.Cells[row, 12].Value = item.FuelCategory;
                        sheet.Cells[row, 12].StyleID = rowStyle.Styles[8];
                    }
                    int endRow = Math.Max(3, details.Count + 2);
                    detailEnds[name] = endRow;
                    sheet.Cells[3, 5, endRow, 5].Style.Numberformat.Format = "yyyy/mm/dd";
                    sheet.Cells[3, 8, endRow, 8].Style.Numberformat.Format = "#,##0.###";
                    sheet.Cells[3, 11, endRow, 11].Style.Numberformat.Format = "#,##0.###";
                    // Cột phụ chỉ phục vụ SUMIFS, không thay đổi các cột của mẫu đang hiển thị.
                    sheet.Column(12).Hidden = true;
                    ApplyBorders(sheet, details.Count + 2, 11);
                }

                var summary = package.Workbook.Worksheets["統計表"];
                var monthlyStyles = new[] { CaptureRow(summary, 3, 9), CaptureRow(summary, 4, 9), CaptureRow(summary, 5, 9) };
                var totalStyles = new[] { CaptureRow(summary, 39, 9), CaptureRow(summary, 40, 9), CaptureRow(summary, 41, 9) };
                string petrolCaption = summary.Cells[3, 3].Text;
                string dieselCaption = summary.Cells[4, 3].Text;
                var purposeCaptions = new[] { summary.Cells[3, 4].Text, summary.Cells[4, 4].Text, summary.Cells[5, 4].Text };
                string densityNote = summary.Cells[3, 8].Text;
                string totalCaption = summary.Cells[39, 2].Text;
                ClearBody(summary);
                summary.Cells[1, 1].Value = UpdateTitle(summary.Cells[1, 1].Text, year, departmentTitle, null);
                var purposes = new[] { "公務車（汽車、摩托車）", "貨卡車", "推高車" };
                int summaryRow = 3;
                foreach (string department in rows.Select(row => row.Department).Distinct().OrderBy(value => value))
                {
                    // 保留範本的三種用途，並支援同一用途實際使用不同燃料。
                    var categories = new[] { "汽油|" + purposes[0], "柴油|" + purposes[1], "柴油|" + purposes[2] }
                        .Concat(rows.Where(row => row.Department == department).Select(row => row.FuelCategory + "|" + Purpose(row.VehicleType)))
                        .Distinct().Select(value => value.Split('|')).ToList();
                    int departmentStart = summaryRow;
                    for (int month = 1; month <= 12; month++)
                    {
                        int monthStart = summaryRow;
                        foreach (var category in categories)
                        {
                            int styleIndex = category[1] == purposes[1] ? 1 : category[1] == purposes[2] ? 2 : 0;
                            ApplyRow(summary, summaryRow, monthlyStyles[styleIndex]);
                            WriteSummaryRow(summary, summaryRow, department, month, category[0], category[1]);
                            summary.Cells[summaryRow, 3].Value = category[0] == "汽油" ? petrolCaption : dieselCaption;
                            summary.Cells[summaryRow, 4].Value = category[1] == "未分類" ? "未分類" : purposeCaptions[styleIndex];
                            var sheets = category[1] == purposes[0] ? new[] { "摩托車", "汽車" } : new[] { category[1] };
                            summary.Cells[summaryRow, 5].Formula = string.Join("+", sheets.Select(name =>
                                $"SUMIFS('{name}'!$H$3:$H${detailEnds[name]},'{name}'!$A$3:$A${detailEnds[name]},$A${departmentStart},'{name}'!$C$3:$C${detailEnds[name]},$B${monthStart},'{name}'!$L$3:$L${detailEnds[name]},\"{category[0]}\")"));
                            summaryRow++;
                        }
                        MergeColumn(summary, monthStart, summaryRow - 1, 2);
                    }
                    int monthEnd = summaryRow - 1;
                    int totalStart = summaryRow;
                    foreach (var category in categories)
                    {
                        int styleIndex = category[1] == purposes[1] ? 1 : category[1] == purposes[2] ? 2 : 0;
                        ApplyRow(summary, summaryRow, totalStyles[styleIndex]);
                        WriteSummaryRow(summary, summaryRow, department, totalCaption, category[0], category[1]);
                        summary.Cells[summaryRow, 9].Value = null;
                        summary.Cells[summaryRow, 3].Value = category[0] == "汽油" ? petrolCaption : dieselCaption;
                        summary.Cells[summaryRow, 4].Value = category[1] == "未分類" ? "未分類" : purposeCaptions[styleIndex];
                        summary.Cells[summaryRow, 5].Formula = $"SUMIFS(E{departmentStart}:E{monthEnd},C{departmentStart}:C{monthEnd},C{summaryRow},D{departmentStart}:D{monthEnd},D{summaryRow})";
                        summaryRow++;
                    }
                    MergeColumn(summary, totalStart, summaryRow - 1, 2);
                    MergeColumn(summary, departmentStart, summaryRow - 1, 1);
                    summary.Cells[departmentStart, 8].Value = densityNote + (categories.Any(category => category[1] == "未分類") ? "\n未分類：請確認車種。" : "");
                    MergeColumn(summary, departmentStart, summaryRow - 1, 8);
                }
                summary.Cells[3, 5, summaryRow - 1, 7].Style.Numberformat.Format = "#,##0.###";
                ApplyBorders(summary, summaryRow - 1, 8);
                package.Workbook.Calculate();
                foreach (string name in detailSheets)
                {
                    var sheet = package.Workbook.Worksheets[name];
                    for (int row = 3; row <= detailEnds[name]; row++)
                        foreach (int column in new[] { 8, 11 }) SetQuantityFormat(sheet.Cells[row, column]);
                }
                for (int row = 3; row < summaryRow; row++)
                    foreach (int column in new[] { 5, 7 }) SetQuantityFormat(summary.Cells[row, column]);
                package.SaveAs(new FileInfo(outputPath));
            }
        }

        private static string Purpose(string vehicleType)
        {
            return vehicleType == "汽車" || vehicleType == "摩托車" ? "公務車（汽車、摩托車）" : SheetName(vehicleType);
        }

        private static string SheetName(string vehicleType)
        {
            return VehicleSheets.Contains(vehicleType) ? vehicleType : "未分類";
        }

        private static void WriteSummaryRow(ExcelWorksheet sheet, int row, string department, object month, string category, string purpose)
        {
            sheet.Cells[row, 1].Value = DepartmentValue(department);
            sheet.Cells[row, 2].Value = month;
            sheet.Cells[row, 9].Value = month is int ? month : null;
            sheet.Cells[row, 3].Value = category;
            sheet.Cells[row, 4].Value = purpose;
            // 沿用原範本既有換算係數。
            sheet.Cells[row, 6].Value = category == "汽油" ? 0.73m : 0.84m;
            sheet.Cells[row, 7].Formula = $"E{row}*F{row}";
            sheet.Cells[row, 8].Value = category == "汽油" ? "汽油密度：0.73 kg/L" : "柴油密度：0.84 kg/L";
            if (purpose == "未分類") sheet.Cells[row, 8].Value += "；車種待確認";
        }

        private static void ClearBody(ExcelWorksheet sheet)
        {
            foreach (string address in sheet.MergedCells.ToList())
                if (sheet.Cells[address].Start.Row >= 3) sheet.Cells[address].Merge = false;
            var body = sheet.Cells[3, 1, Math.Max(3, sheet.Dimension?.End.Row ?? 3), Math.Max(12, sheet.Dimension?.End.Column ?? 12)];
            body.Value = null;
            body.Formula = string.Empty;
        }

        private static object DepartmentValue(string value)
        {
            return int.TryParse(value, out int number) && !value.StartsWith("0", StringComparison.Ordinal)
                ? (object)number : value;
        }

        private static void SetQuantityFormat(ExcelRange cell)
        {
            if (cell.Value == null) return;
            decimal value = Convert.ToDecimal(cell.Value, CultureInfo.InvariantCulture);
            cell.Style.Numberformat.Format = value == decimal.Truncate(value) ? "#,##0" : "#,##0.###";
        }

        private static void ApplyBorders(ExcelWorksheet sheet, int endRow, int columns)
        {
            var table = sheet.Cells[2, 1, Math.Max(2, endRow), columns];
            foreach (var border in new[] { table.Style.Border.Top, table.Style.Border.Bottom, table.Style.Border.Left, table.Style.Border.Right })
            {
                border.Style = ExcelBorderStyle.Thin;
                border.Color.SetColor(Color.Black);
            }
        }

        private sealed class TemplateRow
        {
            public int[] Styles { get; set; }
            public double Height { get; set; }
            public bool CustomHeight { get; set; }
        }

        private static TemplateRow CaptureRow(ExcelWorksheet sheet, int row, int columns)
        {
            return new TemplateRow
            {
                Styles = Enumerable.Range(1, columns).Select(column => sheet.Cells[row, column].StyleID).ToArray(),
                Height = sheet.Row(row).Height,
                CustomHeight = sheet.Row(row).CustomHeight
            };
        }

        private static void ApplyRow(ExcelWorksheet sheet, int row, TemplateRow template)
        {
            for (int column = 1; column <= template.Styles.Length; column++)
                sheet.Cells[row, column].StyleID = template.Styles[column - 1];
            sheet.Row(row).Height = template.Height;
            sheet.Row(row).CustomHeight = template.CustomHeight;
        }

        private static void MergeColumn(ExcelWorksheet sheet, int start, int end, int column)
        {
            if (end <= start) return;
            sheet.Cells[start + 1, column, end, column].Value = null;
            sheet.Cells[start, column, end, column].Merge = true;
        }

        private static string UpdateTitle(string title, int year, string departments, string vehicleType)
        {
            string result = title ?? string.Empty;
            if (vehicleType == null)
                return result.Replace("\u5ee0\u8655\u4f7f\u7528\u71c3\u6599\u91cf\u7d71\u8a08\u8868", $"\u5ee0\u8655\u4f7f\u7528\u71c3\u6599\u91cf\u7d71\u8a08\u8868\uff08{year}\uff1b\u90e8\u9580\uff1a{departments}\uff09");
            result = result.Replace("\u71c3\u6599\u4f7f\u7528\u91cf\u7d71\u8a08\u8868", $"\u71c3\u6599\u4f7f\u7528\u91cf\u7d71\u8a08\u8868\uff08{year}\uff09");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(\u5ee0\u8655\s*[:\uff1a]\s*)\d+", match => match.Groups[1].Value + departments);
            string division = string.Join("\u3001", departments.Split('\u3001').Where(value => value.Length > 0).Select(value => value.Substring(0, 1)).Distinct());
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(\u4e8b\u696d\u90e8\s*[:\uff1a]\s*)\d+", match => match.Groups[1].Value + division);
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(\u8a2d\u5099\u985e\u5225\s*[:\uff1a]\s*)[^\r\n]+", match => match.Groups[1].Value + vehicleType);
            var vietnameseTypes = new Dictionary<string, string>
            {
                ["\u6469\u6258\u8eca"] = "XE M\u00c1Y", ["\u6c7d\u8eca"] = "\u00d4 T\u00d4", ["\u8ca8\u5361\u8eca"] = "XE T\u1ea2I",
                ["\u63a8\u9ad8\u8eca"] = "XE N\u00c2NG", ["\u672a\u5206\u985e"] = "CH\u01afA PH\u00c2N LO\u1ea0I"
            };
            result = System.Text.RegularExpressions.Regex.Replace(result, @"(LO\u1ea0I THI\u1ebeT B\u1eca\s*:\s*)[^\u3000\r\n]+",
                match => match.Groups[1].Value + vietnameseTypes[vehicleType]);
            return result;
        }
    }
}
