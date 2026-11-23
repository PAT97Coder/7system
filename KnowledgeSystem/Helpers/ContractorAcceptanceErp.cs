using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement.f311_AutoERP;

namespace KnowledgeSystem.Helpers
{
    public static class ContractorAcceptanceErp
    {
        public static List<ErpAction> Build(ContractorAcceptance record)
        {
            if (record == null || record.Validation != "檢查通過" || record.Lines == null || record.Lines.Count == 0)
                throw new InvalidOperationException("請選取檢查通過且具有工作明細的驗收單。");
            var costCenter = Regex.Match((record.CostCenter ?? "").Trim(), @"^(?<company>[A-Za-z]{2})(?<department>\d+)$");
            if (!costCenter.Success) throw new InvalidOperationException("成本單位格式錯誤，應為公司代號及部門代號，例如 LG7730。");
            if (string.IsNullOrWhiteSpace(record.ContractNumber)) throw new InvalidOperationException("合約編號不可空白。");
            var lines = record.Lines.OrderBy(line => line.ExcelRow).ToList();
            var months = lines.Select(line => WorkMonth(line.WorkPeriod)).Distinct().ToList();
            if (months.Count != 1) throw new InvalidOperationException("工作明細的發生月份不同，無法使用同一個 ERP 日期及本單編號。");
            string date = months[0].ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            // ERP tự chuyển ô khi đủ ký tự ở phần đầu phiếu; không gửi Tab.
            string header = string.Concat(date, costCenter.Groups["company"].Value.ToUpperInvariant(),
                costCenter.Groups["department"].Value, Literal(record.ContractNumber.Trim()), date);
            var rows = new List<string>();
            for (int index = 0; index < lines.Count; index++)
            {
                var line = lines[index];
                if (string.IsNullOrWhiteSpace(line.WorkCode) || line.Quantity <= 0)
                    throw new InvalidOperationException("第 " + line.ExcelRow + " 列：工作代號不可空白，工作量必須大於零。");
                // STT → Tab → mã (tự chuyển ô) → số lượng → mũi tên xuống về STT dòng sau.
                rows.Add((index + 1).ToString(CultureInfo.InvariantCulture) + "{TAB}"
                    + Literal(line.WorkCode.Trim())
                    + line.Quantity.ToString("0.############################", CultureInfo.InvariantCulture) + "{DOWN}");
            }
            return new List<ErpAction> { new ErpAction
            {
                Text = header + string.Concat(rows)
            } };
        }

        private static DateTime WorkMonth(string value)
        {
            var match = Regex.Match((value ?? "").Trim(), @"^(?<date>\d{4}[/\-]\d{1,2}[/\-]\d{1,2})(?!\d)");
            DateTime start;
            if (!match.Success || !DateTime.TryParseExact(match.Groups["date"].Value,
                new[] { "yyyy/M/d", "yyyy-M-d" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
                throw new InvalidOperationException("作業時間必須以有效日期開頭，例如 2026/11/01-11/15。");
            return new DateTime(start.Year, start.Month, 1);
        }

        // Dữ liệu Excel là văn bản, không được diễn giải thành lệnh SendKeys.
        private static string Literal(string value)
        {
            if (value.Any(char.IsControl)) throw new InvalidOperationException("ERP 欄位不可包含換行或控制字元。");
            return string.Concat(value.Select(character => "+^%~(){}[]".Contains(character)
                ? "{" + character + "}" : character.ToString()));
        }
    }
}
