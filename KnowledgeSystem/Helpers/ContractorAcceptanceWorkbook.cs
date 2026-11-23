using BusinessLayer;
using DataAccessLayer;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KnowledgeSystem.Helpers
{
    // Mẫu R1: nhận diện nhãn và cấu trúc, không phụ thuộc màu chữ hoặc tên sheet.
    public static class ContractorAcceptanceWorkbook
    {
        public static List<ContractorAcceptance> Read(string path)
        {
            if (!string.Equals(Path.GetExtension(path), ".xlsx", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("僅支援 .xlsx 檔案。");
            if (!File.Exists(path)) throw new FileNotFoundException("找不到來源檔案：" + Path.GetFileName(path));
            var result = new List<ContractorAcceptance>();
            if (new FileInfo(path).Length > ContractorAcceptanceService.MaximumFileBytes)
                throw new InvalidDataException("來源檔案不可超過 10 MB。");
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0 || bytes.Length > ContractorAcceptanceService.MaximumFileBytes)
                throw new InvalidDataException("來源檔案無效或超過 10 MB。");
            using (var stream = new MemoryStream(bytes))
            using (var package = new ExcelPackage(stream))
            {
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    if (!Text(sheet, 5, 1).Contains("外包工作驗收") || !Text(sheet, 6, 3).Contains("本單編號")) continue;
                    var acceptance = ReadSheet(sheet, Path.GetFullPath(path));
                    acceptance.SourceContent = bytes;
                    result.Add(acceptance);
                }
            }
            if (result.Count == 0) throw new InvalidDataException("找不到外包工作驗收單 R1，請確認檔案格式。");
            return result;
        }

        private static ContractorAcceptance ReadSheet(ExcelWorksheet sheet, string path)
        {
            var errors = new List<string>();
            var period = Regex.Match(Text(sheet, 5, 1), @"(?<year>\d{4})年\s*(?<month>\d{1,2})月");
            int month;
            if (!period.Success || !int.TryParse(period.Groups["month"].Value, out month) || month < 1 || month > 12)
                throw new InvalidDataException(sheet.Name + "：驗收年月格式錯誤。");
            var data = new ContractorAcceptance
            {
                SourcePath = path, SourceFileName = Path.GetFileName(path), SheetName = sheet.Name,
                Period = period.Groups["year"].Value + "-" + month.ToString("00"),
                ReportDate = Date(sheet.Cells[7, 1].Value, sheet.Name + "!A7"),
                DocumentNumber = ContractorAcceptanceService.NormalizeNumber(Text(sheet, 7, 3)), CostCenter = Text(sheet, 7, 4),
                ContractNumber = Text(sheet, 7, 5), Contractor = Text(sheet, 7, 6)
            };
            if (string.IsNullOrWhiteSpace(data.DocumentNumber) || string.IsNullOrWhiteSpace(data.CostCenter)
                || string.IsNullOrWhiteSpace(data.ContractNumber) || string.IsNullOrWhiteSpace(data.Contractor))
                errors.Add("本單編號、成本單位、合約編號及外包商名稱不可空白");
            int adjustmentRow = 0;
            for (int row = 9; row <= Math.Min(sheet.Dimension.End.Row, 10000); row++)
            {
                if (Text(sheet, row, 1).Contains("調整金額")) { adjustmentRow = row; break; }
                // 忽略只有序號、金額公式和預設幣別的空白列。
                if (Enumerable.Range(2, 7).All(column => string.IsNullOrWhiteSpace(Text(sheet, row, column)))) continue;
                var line = new ContractorAcceptanceLine
                {
                    ExcelRow = row, Description = Text(sheet, row, 2), WorkCode = Text(sheet, row, 3),
                    Location = Text(sheet, row, 4), WorkPeriod = Text(sheet, row, 5), Unit = Text(sheet, row, 6),
                    Quantity = Number(sheet, row, 7), UnitPrice = Number(sheet, row, 8), Currency = Text(sheet, row, 10)
                };
                if (string.IsNullOrWhiteSpace(line.Description) || string.IsNullOrWhiteSpace(line.Unit)
                    || string.IsNullOrWhiteSpace(line.Currency) || line.Quantity <= 0 || line.UnitPrice < 0)
                    errors.Add("第 " + row + " 列：請檢查工作項目、單位、幣別、數量及單價");
                CheckLiteral(sheet, row, 9, line.Amount, errors);
                data.Lines.Add(line);
            }
            if (adjustmentRow == 0 || !Text(sheet, adjustmentRow + 1, 1).Contains("不含稅額")
                || !Text(sheet, adjustmentRow + 3, 1).Contains("含稅額"))
                throw new InvalidDataException(sheet.Name + "：金額彙總區格式錯誤。");
            if (data.Lines.Count == 0) errors.Add("沒有工作明細");
            if (data.Lines.Select(line => line.Currency).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1)
                errors.Add("同一張驗收單不可混用幣別");
            data.Adjustment = Number(sheet, adjustmentRow, 9);
            data.BeforeTax = data.Lines.Sum(line => line.Amount) - data.Adjustment;
            // Chỉ hỗ trợ công thức thuế theo mẫu: I<row>*<rate>%.
            var taxCell = sheet.Cells[adjustmentRow + 2, 9];
            if (!string.IsNullOrWhiteSpace(taxCell.Formula))
            {
                string pattern = @"^\s*\$?I\$?" + (adjustmentRow + 1) + @"\s*\*\s*(?<rate>\d+(?:\.\d+)?)\s*%\s*$";
                var taxFormula = Regex.Match(taxCell.Formula, pattern, RegexOptions.IgnoreCase);
                if (!taxFormula.Success) throw new InvalidDataException(sheet.Name + "：不支援的稅額公式。");
                decimal rate = decimal.Parse(taxFormula.Groups["rate"].Value, CultureInfo.InvariantCulture);
                if (rate > 100) throw new InvalidDataException(sheet.Name + "：稅率超出範圍。");
                data.Tax = data.BeforeTax * rate / 100m;
            }
            else data.Tax = Number(sheet, adjustmentRow + 2, 9);
            data.Total = data.BeforeTax + data.Tax;
            if (data.BeforeTax < 0 || data.Tax < 0) errors.Add("未稅金額或稅額不可為負數");
            CheckLiteral(sheet, adjustmentRow + 1, 9, data.BeforeTax, errors);
            CheckLiteral(sheet, adjustmentRow + 3, 9, data.Total, errors);
            data.Validation = errors.Count == 0 ? "檢查通過" : string.Join("；", errors);
            return data;
        }

        private static string Text(ExcelWorksheet sheet, int row, int column)
        {
            return Convert.ToString(sheet.Cells[row, column].Value, CultureInfo.InvariantCulture).Trim();
        }

        private static DateTime Date(object value, string address)
        {
            if (value is DateTime) return ((DateTime)value).Date;
            if (value is double)
            {
                try { return DateTime.FromOADate((double)value).Date; }
                catch (ArgumentException) { }
            }
            DateTime date;
            if (DateTime.TryParseExact(Convert.ToString(value, CultureInfo.InvariantCulture),
                new[] { "yyyy/MM/dd", "yyyy-MM-dd", "yyyyMMdd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return date;
            throw new InvalidDataException(address + "：填報日期無效。");
        }

        private static decimal Number(ExcelWorksheet sheet, int row, int column)
        {
            var cell = sheet.Cells[row, column];
            if (!string.IsNullOrEmpty(cell.Formula)) throw new InvalidDataException(cell.Address + "：輸入欄位不可使用公式。");
            decimal number;
            var value = Convert.ToString(cell.Value, CultureInfo.InvariantCulture).Trim();
            if (cell.Value is string && !Regex.IsMatch(value, @"^[+-]?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$"))
                throw new InvalidDataException(sheet.Name + "!" + cell.Address + "：數值格式錯誤。");
            if (decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out number))
                return number;
            throw new InvalidDataException(sheet.Name + "!" + cell.Address + "：數值無效或空白。");
        }

        private static void CheckLiteral(ExcelWorksheet sheet, int row, int column, decimal expected, List<string> errors)
        {
            var cell = sheet.Cells[row, column];
            if (string.IsNullOrEmpty(cell.Formula) && Number(sheet, row, column) != expected)
                errors.Add(cell.Address + "：金額與重新計算結果不符");
        }

        public static void Export(string path, IList<ContractorAcceptance> records)
        {
            if (records == null || records.Count == 0) throw new InvalidOperationException("沒有可匯出的資料。");
            if (records.Any(record => string.Equals(Path.GetFullPath(path), record.SourcePath, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("不可覆寫來源檔案，請選擇其他檔名。");
            using (var package = new ExcelPackage())
            {
                var header = package.Workbook.Worksheets.Add("驗收資料");
                var detail = package.Workbook.Worksheets.Add("工作明細");
                string[] headings = { "驗收年月", "填報日期", "本單編號", "成本單位", "合約編號", "外包商名稱", "調整金額", "未稅金額", "稅額", "含稅金額", "檢查結果", "來源檔案", "工作表" };
                string[] lines = { "本單編號", "來源列", "工作項目", "工作代號", "位置", "作業時間", "單位", "數量", "單價", "金額", "幣別" };
                for (int i = 0; i < headings.Length; i++) header.Cells[1, i + 1].Value = headings[i];
                for (int i = 0; i < lines.Length; i++) detail.Cells[1, i + 1].Value = lines[i];
                int r = 2, d = 2;
                foreach (var item in records)
                {
                    object[] values = { item.Period, item.ReportDate, item.DocumentNumber, item.CostCenter, item.ContractNumber,
                        item.Contractor, item.Adjustment, item.BeforeTax, item.Tax, item.Total, item.Validation, item.SourceFileName, item.SheetName };
                    for (int i = 0; i < values.Length; i++) header.Cells[r, i + 1].Value = values[i];
                    header.Cells[r++, 2].Style.Numberformat.Format = "yyyy/mm/dd";
                    foreach (var line in item.Lines)
                    {
                        object[] data = { item.DocumentNumber, line.ExcelRow, line.Description, line.WorkCode, line.Location,
                            line.WorkPeriod, line.Unit, line.Quantity, line.UnitPrice, line.Amount, line.Currency };
                        for (int i = 0; i < data.Length; i++) detail.Cells[d, i + 1].Value = data[i];
                        d++;
                    }
                }
                foreach (var sheet in new[] { header, detail })
                {
                    sheet.Cells.Style.Font.Name = "Microsoft JhengHei UI";
                    sheet.Cells.Style.Font.Size = 12;
                    sheet.Cells[1, 1, 1, sheet.Dimension.End.Column].Style.Font.Bold = true;
                    sheet.Cells[sheet.Dimension.Address].AutoFilter = true;
                    sheet.View.FreezePanes(2, 1);
                    for (int col = 1; col <= sheet.Dimension.End.Column; col++) sheet.Column(col).Width = 20;
                }
                header.Column(6).Width = 36;
                header.Column(11).Width = 55;
                header.Column(12).Width = 55;
                detail.Column(3).Width = 36;
                header.Cells[2, 7, r - 1, 10].Style.Numberformat.Format = "#,##0.###";
                if (d > 2) detail.Cells[2, 8, d - 1, 10].Style.Numberformat.Format = "#,##0.###";
                package.SaveAs(new FileInfo(path));
            }
        }
    }
}
