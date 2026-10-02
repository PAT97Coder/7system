using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;

namespace KnowledgeSystem.Helpers
{
    public sealed class OxpsSubjectDescriptionResult
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// Đọc trực tiếp lớp chữ Unicode và tọa độ Glyphs trong tài liệu OXPS.
    /// Đây không phải OCR: tài liệu phải còn chứa UnicodeString trong các trang FixedPage.
    /// </summary>
    public static class OxpsParser
    {
        private const long MaxOxpsFileSize = 50L * 1024 * 1024;
        private const long MaxFixedPageSize = 10L * 1024 * 1024;
        private const double SameLineTolerance = 0.8d;
        private const double LabelColumnTolerance = 20d;

        public static OxpsSubjectDescriptionResult ReadSubjectAndDescription(string filePath)
        {
            ValidateFile(filePath);

            using (var archive = ZipFile.OpenRead(filePath))
            {
                var glyphs = ReadGlyphs(archive);
                if (glyphs.Count == 0)
                    throw new InvalidDataException("OXPS 檔案中沒有可讀取的文字資料。");

                var subjectLabel = FindLabel(glyphs, "主旨", null);
                if (subjectLabel == null)
                    throw new InvalidDataException("找不到「主旨」欄位。");

                var descriptionLabel = FindLabel(glyphs, "說明", subjectLabel);
                if (descriptionLabel == null)
                    throw new InvalidDataException("找不到「說明」欄位。");

                var dueDateLabel = FindLabel(glyphs, "待覆日期", null);
                DateTime? dueDate = ReadDateValueOnSameLine(glyphs, dueDateLabel);

                var attachmentLabel = glyphs
                    .Where(item => IsAfter(item, descriptionLabel)
                        && Normalize(item.Text) == "附件"
                        && item.X <= descriptionLabel.X + LabelColumnTolerance)
                    .OrderBy(item => item.PageIndex)
                    .ThenBy(item => item.Y)
                    .ThenBy(item => item.X)
                    .FirstOrDefault();

                string subject = BuildText(glyphs.Where(item =>
                    IsAfter(item, subjectLabel)
                    && IsBefore(item, descriptionLabel)
                    && item.X > subjectLabel.X + LabelColumnTolerance));

                string description = BuildText(glyphs.Where(item =>
                    IsAfter(item, descriptionLabel)
                    && (attachmentLabel == null || IsBefore(item, attachmentLabel))
                    && item.X > descriptionLabel.X + LabelColumnTolerance));

                if (string.IsNullOrWhiteSpace(subject))
                    throw new InvalidDataException("「主旨」欄位沒有可讀取的內容。");
                if (string.IsNullOrWhiteSpace(description))
                    throw new InvalidDataException("「說明」欄位沒有可讀取的內容。");

                return new OxpsSubjectDescriptionResult
                {
                    Subject = subject,
                    Description = description,
                    DueDate = dueDate
                };
            }
        }

        private static DateTime? ReadDateValueOnSameLine(
            IEnumerable<OxpsGlyph> glyphs,
            OxpsGlyph label)
        {
            if (label == null) return null;

            string value = glyphs
                .Where(item => item.PageIndex == label.PageIndex
                    && Math.Abs(item.Y - label.Y) <= SameLineTolerance
                    && item.X > label.X + LabelColumnTolerance)
                .OrderBy(item => item.X)
                .Select(item => item.Text?.Trim())
                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

            if (string.IsNullOrWhiteSpace(value)) return null;

            string[] formats =
            {
                "yyyy/M/d", "yyyy/MM/dd", "yyyy-M-d", "yyyy-MM-dd",
                "yyyy.M.d", "yyyy.MM.dd"
            };
            if (DateTime.TryParseExact(
                value,
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime result))
                return result.Date;

            return null;
        }

        private static void ValidateFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("尚未選擇 OXPS 檔案。", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("找不到 OXPS 檔案。", filePath);
            string extension = Path.GetExtension(filePath);
            if (!string.Equals(extension, ".oxps", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".xps", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("只支援 XPS/OXPS 檔案。");
            if (new FileInfo(filePath).Length > MaxOxpsFileSize)
                throw new InvalidDataException("OXPS 檔案不可超過 50 MB。");
        }

        private static List<OxpsGlyph> ReadGlyphs(ZipArchive archive)
        {
            var pageEntries = archive.Entries
                .Where(entry => entry.FullName.EndsWith(".fpage", StringComparison.OrdinalIgnoreCase))
                .OrderBy(entry => entry.FullName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var result = new List<OxpsGlyph>();
            for (int pageIndex = 0; pageIndex < pageEntries.Count; pageIndex++)
            {
                var entry = pageEntries[pageIndex];
                if (entry.Length > MaxFixedPageSize)
                    throw new InvalidDataException("OXPS 頁面資料過大，無法安全讀取。");

                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                    MaxCharactersInDocument = MaxFixedPageSize
                };

                using (var stream = entry.Open())
                using (var reader = XmlReader.Create(stream, settings))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element || reader.LocalName != "Glyphs")
                            continue;

                        string text = reader.GetAttribute("UnicodeString");
                        if (string.IsNullOrEmpty(text)) continue;

                        if (!double.TryParse(reader.GetAttribute("OriginX"),
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double x))
                            continue;
                        if (!double.TryParse(reader.GetAttribute("OriginY"),
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double y))
                            continue;

                        result.Add(new OxpsGlyph
                        {
                            PageIndex = pageIndex,
                            X = x,
                            Y = y,
                            Text = text
                        });
                    }
                }
            }

            return result
                .OrderBy(item => item.PageIndex)
                .ThenBy(item => item.Y)
                .ThenBy(item => item.X)
                .ToList();
        }

        private static OxpsGlyph FindLabel(
            IEnumerable<OxpsGlyph> glyphs,
            string label,
            OxpsGlyph after)
        {
            return glyphs
                .Where(item => Normalize(item.Text) == label
                    && (after == null || IsAfter(item, after)))
                .OrderBy(item => item.PageIndex)
                .ThenBy(item => item.Y)
                .ThenBy(item => item.X)
                .FirstOrDefault();
        }

        private static string BuildText(IEnumerable<OxpsGlyph> source)
        {
            var ordered = source
                .Where(item => !string.IsNullOrWhiteSpace(item.Text))
                .OrderBy(item => item.PageIndex)
                .ThenBy(item => item.Y)
                .ThenBy(item => item.X)
                .ToList();

            var lines = new List<List<OxpsGlyph>>();
            foreach (var glyph in ordered)
            {
                var line = lines.LastOrDefault();
                if (line == null
                    || line[0].PageIndex != glyph.PageIndex
                    || Math.Abs(line[0].Y - glyph.Y) > SameLineTolerance)
                {
                    line = new List<OxpsGlyph>();
                    lines.Add(line);
                }
                line.Add(glyph);
            }

            return string.Join(Environment.NewLine, lines
                .Select(line => string.Concat(line.OrderBy(item => item.X).Select(item => item.Text)).Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line)));
        }

        private static bool IsAfter(OxpsGlyph item, OxpsGlyph marker)
        {
            if (item.PageIndex != marker.PageIndex)
                return item.PageIndex > marker.PageIndex;
            if (Math.Abs(item.Y - marker.Y) > SameLineTolerance)
                return item.Y > marker.Y;
            return item.X > marker.X;
        }

        private static bool IsBefore(OxpsGlyph item, OxpsGlyph marker)
        {
            if (item.PageIndex != marker.PageIndex)
                return item.PageIndex < marker.PageIndex;
            if (Math.Abs(item.Y - marker.Y) > SameLineTolerance)
                return item.Y < marker.Y;
            return item.X < marker.X;
        }

        private static string Normalize(string text)
        {
            return (text ?? string.Empty).Trim().Replace("：", string.Empty).Replace(":", string.Empty);
        }

        private sealed class OxpsGlyph
        {
            public int PageIndex { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public string Text { get; set; }
        }
    }
}
