using BusinessLayer;
using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public static class Lab208MonthlyExporter
    {
        // Keep the supplied controlled form as the source of layout, borders and print settings.
        public static int Export(string path, DateTime month, string department, string location, string sn, DataTable readings)
        {
            var start = new DateTime(month.Year, month.Month, 1);
            var end = start.AddMonths(1);
            var rows = readings.AsEnumerable().Where(r => r["SN"].ToString() == sn
                && r["IdDept"].ToString() == department && r["Location"].ToString() == location
                && !r.IsNull("MeasuredAt") && (DateTime)r["MeasuredAt"] >= start && (DateTime)r["MeasuredAt"] < end
                && !r.IsNull("TempMin") && !r.IsNull("TempMax") && !r.IsNull("HumidityMin") && !r.IsNull("HumidityMax")).ToList();
            using (var workbook = new Workbook())
            using (var template = typeof(Lab208MonthlyExporter).Assembly.GetManifestResourceStream("KnowledgeSystem.Lab208.MonthlyInspection.xls"))
            {
                if (template == null) throw new InvalidOperationException("找不到每月檢查紀錄表範本。");
                workbook.LoadDocument(template, DevExpress.Spreadsheet.DocumentFormat.Xls);
                // The other sheet in the source file is an unrelated historical example.
                while (workbook.Worksheets.Count > 1) workbook.Worksheets.RemoveAt(1);
                var sheet = workbook.Worksheets[0];
                sheet.Name = "溫濕度檢查紀錄表";
                sheet.Cells["A1"].Value = $"BẢNG THEO DÕI NHIỆT ĐỘ, ĐỘ ẨM THÁNG {start.Month:00} NĂM {start.Year}\n{start.Year}年{start.Month:00}月溫濕度檢查紀錄表";
                sheet.Cells["A2"].Value = "Phòng thí nghiệm/實驗室: " + department + "／" + location;
                sheet.Cells["A3"].Value = "Mã nhiệt ẩm kế/溫濕度計編號: " + sn;
                sheet.Cells["A4"].Value = "Nhiệt độ kiểm soát/溫度管制值: " + RangeText(rows, "TempMin", "TempMax", "°C");
                sheet.Cells["F4"].Value = "Độ ẩm kiểm soát/濕度管制值: " + RangeText(rows, "HumidityMin", "HumidityMax", "%RH");
                // Replace the original mixed-language rich text runs as well as their text.
                // Otherwise old run offsets/font charsets can hide characters in print previews.
                foreach (string address in new[] { "A1", "A2", "A3", "A4", "F4" })
                    SetHeaderText(sheet.Cells[address]);

                var details = workbook.Worksheets.Add("量測明細");
                details.Cells["A1"].Value = "每日08:00及16:00，取前後30分鐘內最近量測；距離相同取較早一筆。時間：UTC+07。";
                details.Range["A1:P1"].Merge();
                string[] headers = { "預定時間", "實際量測時間", "實驗室", "設備序號", "溫度 °C", "濕度 %RH", "溫度下限", "溫度上限", "濕度下限", "濕度上限", "狀態", "原因", "處理措施", "處理結果", "處理人", "處理時間" };
                for (int col = 0; col < headers.Length; col++) details.Cells[1, col].Value = headers[col];
                details.Range["A2:P2"].Font.Bold = true;
                int detailRow = 2, count = 0;
                for (int day = 1; day <= 31; day++)
                {
                    int rowIndex = day + 6; // Template: two header rows (6–7), daily rows 8–38.
                    for (int col = 0; col < 13; col++) sheet.Cells[rowIndex, col].Value = CellValue.Empty;
                    if (day > DateTime.DaysInMonth(start.Year, start.Month)) continue;
                    DateTime date = start.AddDays(day - 1);
                    sheet.Cells[rowIndex, 0].Value = sheet.Cells[rowIndex, 6].Value = day;
                    sheet.Cells[rowIndex, 1].Value = "1";
                    sheet.Cells[rowIndex, 7].Value = "2";
                    var notes = new List<string>();
                    foreach (int hour in new[] { 8, 16 })
                    {
                        DateTime target = date.AddHours(hour);
                        var reading = rows.Where(r => Math.Abs(((DateTime)r["MeasuredAt"] - target).TotalMinutes) <= 30)
                            .OrderBy(r => Math.Abs(((DateTime)r["MeasuredAt"] - target).Ticks))
                            .ThenBy(r => (DateTime)r["MeasuredAt"]).ThenBy(r => Convert.ToInt64(r["Id"])).FirstOrDefault();
                        details.Cells[detailRow, 0].Value = target;
                        details.Cells[detailRow, 2].Value = department + "／" + location;
                        details.Cells[detailRow, 3].Value = sn;
                        if (reading != null)
                        {
                            count++;
                            int timeColumn = hour == 8 ? 2 : 8;
                            sheet.Cells[rowIndex, timeColumn].Value = ((DateTime)reading["MeasuredAt"]).ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                            sheet.Cells[rowIndex, timeColumn + 1].Value = Convert.ToDouble(reading["Temperature"]);
                            sheet.Cells[rowIndex, timeColumn + 2].Value = Convert.ToDouble(reading["Humidity"]);
                            string status = dt208_EnvironmentRules.MeasurementStatus(reading);
                            if (status.Contains("超限"))
                            {
                                if (dt208_EnvironmentRules.IsOutside((decimal)reading["Temperature"], (decimal)reading["TempMin"], (decimal)reading["TempMax"]))
                                    sheet.Cells[rowIndex, timeColumn + 1].Font.Color = Color.Firebrick;
                                if (dt208_EnvironmentRules.IsOutside((decimal)reading["Humidity"], (decimal)reading["HumidityMin"], (decimal)reading["HumidityMax"]))
                                    sheet.Cells[rowIndex, timeColumn + 2].Font.Color = Color.Firebrick;
                                notes.Add((hour == 8 ? "1：" : "2：") + "超限");
                            }
                            details.Cells[detailRow, 1].Value = (DateTime)reading["MeasuredAt"];
                            string[] numbers = { "Temperature", "Humidity", "TempMin", "TempMax", "HumidityMin", "HumidityMax" };
                            for (int col = 0; col < numbers.Length; col++) details.Cells[detailRow, col + 4].Value = Convert.ToDouble(reading[numbers[col]]);
                            details.Cells[detailRow, 10].Value = status;
                            details.Cells[detailRow, 11].Value = Text(reading, "Cause");
                            details.Cells[detailRow, 12].Value = Text(reading, "Action");
                            details.Cells[detailRow, 13].Value = Text(reading, "Result");
                            details.Cells[detailRow, 14].Value = Text(reading, "CreatedBy");
                            if (reading.Table.Columns.Contains("ActionAt") && !reading.IsNull("ActionAt")) details.Cells[detailRow, 15].Value = (DateTime)reading["ActionAt"];
                        }
                        else details.Cells[detailRow, 10].Value = "缺測";
                        detailRow++;
                    }
                    sheet.Cells[rowIndex, 12].Value = string.Join("\n", notes);
                    // Automatic collection does not constitute a human inspection/signature: F/L remain blank.
                }
                details.GetUsedRange().Font.Name = "Microsoft JhengHei UI";
                details.GetUsedRange().Font.Size = 11;
                details.Columns["A:B"].NumberFormat = "yyyy/mm/dd hh:mm:ss";
                details.Columns["P"].NumberFormat = "yyyy/mm/dd hh:mm:ss";
                details.Columns["A:D"].WidthInCharacters = 24;
                details.Columns["E:K"].WidthInCharacters = 14;
                details.Columns["L:P"].WidthInCharacters = 30;
                details.Range[$"L3:N{detailRow}"].Alignment.WrapText = true;
                sheet.Range["A8:B38"].NumberFormat = sheet.Range["G8:H38"].NumberFormat = "0";
                sheet.Range["A8:M38"].Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                sheet.Range["C8:C38"].Font.Size = sheet.Range["I8:I38"].Font.Size = 10;
                sheet.Range["D8:E38"].NumberFormat = sheet.Range["J8:K38"].NumberFormat = "0.00";
                sheet.Range["M8:M38"].Font.Size = 9;
                sheet.Range["M8:M38"].Alignment.WrapText = true;
                sheet.Rows[38].Height = 720;
                sheet.SetPrintRange(sheet.Range["A1:M41"]);
                sheet.PrintOptions.FitToPage = true;
                sheet.PrintOptions.FitToWidth = sheet.PrintOptions.FitToHeight = 1;
                workbook.Worksheets.ActiveWorksheet = sheet;
                workbook.SaveDocument(path, DevExpress.Spreadsheet.DocumentFormat.Xls);
                return count;
            }
        }

        private static string Text(DataRow row, string column) => row.Table.Columns.Contains(column) && !row.IsNull(column) ? row[column].ToString() : "";
        private static void SetHeaderText(Cell cell)
        {
            var text = new RichTextString();
            foreach (Match run in Regex.Matches(cell.Value.TextValue, @"[\u2E80-\uFFFF]+|[^\u2E80-\uFFFF]+"))
            {
                var font = new RichTextRunFont(cell.Font) { Name = run.Value[0] >= '\u2E80' ? "DFKai-SB" : "Times New Roman" };
                text.AddTextRun(run.Value, font);
            }
            cell.SetRichText(text);
        }
        private static string RangeText(List<DataRow> rows, string min, string max, string unit)
        {
            var ranges = rows.Select(r => dt208_EnvironmentRules.Range(Convert.ToDecimal(r[min]), Convert.ToDecimal(r[max]), unit)).Distinct().ToList();
            return ranges.Count == 1 ? ranges[0] : ranges.Count == 0 ? "—" : "依量測明細";
        }
    }
}
