using System;
using System.Data;
using System.IO;
using DevExpress.Spreadsheet;
using KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment;

internal static class Lab208MonthlyReportTests
{
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static DataTable Readings()
    {
        var data = new DataTable();
        data.Columns.Add("Id", typeof(long)); data.Columns.Add("MeasuredAt", typeof(DateTime));
        foreach (string column in new[] { "SN", "IdDept", "Location", "Cause", "Action", "Result", "CreatedBy" }) data.Columns.Add(column);
        data.Columns.Add("ActionAt", typeof(DateTime));
        foreach (string column in new[] { "Temperature", "Humidity", "TempMin", "TempMax", "HumidityMin", "HumidityMax" }) data.Columns.Add(column, typeof(decimal));
        return data;
    }
    private static DataRow Add(DataTable data, DateTime time, decimal temperature, string department = "D1", string location = "實驗室 A", string sn = "LAB-001")
    {
        var row = data.NewRow(); row["Id"] = data.Rows.Count + 1; row["MeasuredAt"] = time; row["SN"] = sn; row["IdDept"] = department; row["Location"] = location;
        row["Temperature"] = temperature; row["Humidity"] = 60m;
        row["TempMin"] = 15m; row["TempMax"] = 25m; row["HumidityMin"] = 50m; row["HumidityMax"] = 70m;
        data.Rows.Add(row); return row;
    }
    public static void Run()
    {
        string output = Path.Combine(Path.GetTempPath(), "Lab208-MonthlyReport-Test.xls");
        var data = Readings();
        DateTime month = new DateTime(2024, 2, 1);
        Add(data, month.AddHours(7).AddMinutes(50), 21m);
        Add(data, month.AddHours(8).AddMinutes(10), 22m); // Equal distance: choose earlier.
        Add(data, month.AddHours(8).AddMinutes(20), 23m);
        var abnormal = Add(data, month.AddHours(16), 26m);
        abnormal["Cause"] = "空調故障"; abnormal["Action"] = "維修空調"; abnormal["Result"] = "複測正常";
        abnormal["CreatedBy"] = "handler1"; abnormal["ActionAt"] = month.AddHours(17);
        Add(data, month.AddDays(1).AddHours(7).AddMinutes(30), 24m); // Inclusive lower boundary.
        Add(data, month.AddDays(1).AddHours(16).AddMinutes(30), 25m); // Inclusive upper boundary.
        Add(data, month.AddDays(2).AddHours(8).AddMinutes(30).AddMilliseconds(1), 90m); // Outside window.
        Add(data, month.AddDays(2).AddHours(8), 91m, "D2");
        Add(data, month.AddDays(2).AddHours(8), 92m, "D1", "實驗室 B");
        Add(data, month.AddDays(2).AddHours(8), 93m, "D1", "實驗室 A", "OTHER-SN");
        Add(data, month.AddDays(2).AddHours(8), 94m)["TempMin"] = DBNull.Value;
        Add(data, month.AddMonths(1).AddHours(8), 95m);
        Add(data, month.AddDays(-1).AddHours(8), 96m);
        var leap = Add(data, new DateTime(2024, 2, 29, 8, 0, 0), 20m);
        leap["TempMin"] = 18m; leap["TempMax"] = 22m; // Historical thresholds differ.
        int count = Lab208MonthlyExporter.Export(output, month, "D1", "實驗室 A", "LAB-001", data);
        Check(count == 5, "Wrong number of selected monthly readings");
        using (var book = new Workbook())
        {
            book.LoadDocument(output);
            var sheet = book.Worksheets[0]; var details = book.Worksheets[1];
            Check(book.Worksheets.Count == 2 && sheet.Name == "溫濕度檢查紀錄表", "Unrelated sample sheet retained");
            Check(sheet.Cells["A1"].Value.TextValue.Contains("2024年02月"), "Report month missing");
            Check(sheet.Cells["A2"].Value.TextValue.Contains("D1／實驗室 A") && sheet.Cells["A3"].Value.TextValue.Contains("LAB-001"), "Report identity missing");
            Check(sheet.Cells["C8"].Value.TextValue == "07:50:00" && sheet.Cells["D8"].Value.NumericValue == 21, "Nearest/tie selection wrong or actual time hidden");
            Check(sheet.Cells["I8"].Value.TextValue == "16:00:00" && sheet.Cells["J8"].Value.NumericValue == 26, "Afternoon reading is on a different daily row");
            Check(sheet.Cells["D9"].Value.NumericValue == 24 && sheet.Cells["J9"].Value.NumericValue == 25, "30-minute endpoints excluded");
            Check(sheet.Cells["D10"].Value.IsEmpty && sheet.Cells["J10"].Value.IsEmpty, "Wrong room/SN/department/unconfigured/out-of-window reading leaked");
            Check(sheet.Cells["A36"].Value.NumericValue == 29 && sheet.Cells["D36"].Value.NumericValue == 20, "Leap day missing");
            Check(sheet.Cells["A37"].Value.IsEmpty && sheet.Cells["G38"].Value.IsEmpty, "Nonexistent February days populated");
            Check(sheet.Cells["F8"].Value.IsEmpty && sheet.Cells["L8"].Value.IsEmpty, "Human inspection falsely signed");
            Check(sheet.Cells["M8"].Value.TextValue.Contains("2：超限"), "Excursion not marked");
            Check(sheet.Cells["A8"].NumberFormat == "0", "Day inherited a date number format from template");
            Check(sheet.PrintOptions.FitToPage && sheet.PrintOptions.FitToWidth == 1 && sheet.PrintOptions.FitToHeight == 1, "Monthly form is split across printed pages");
            Check(sheet.Cells["J8"].Font.Color.ToArgb() == System.Drawing.Color.Firebrick.ToArgb()
                && sheet.Cells["K8"].Font.Color.ToArgb() != System.Drawing.Color.Firebrick.ToArgb(), "Normal humidity highlighted as an excursion");
            Check(sheet.Cells["A4"].Value.TextValue.Contains("依量測明細"), "Changed historical thresholds replaced with a single range");
            Check(details.Cells["L4"].Value.TextValue == "空調故障" && details.Cells["O4"].Value.TextValue == "handler1", "Cause/action audit missing from detail sheet");
            Check(details.Cells["B3"].Value.DateTimeValue == month.AddHours(7).AddMinutes(50), "Actual full timestamp missing from details");
            Check(sheet.GetUsedRange().GetMergedRanges().Count > 10, "Template merged layout lost");
            Check(sheet.Cells["A39"].Value.TextValue.Contains("實驗室"), "Template instructions lost");
        }
        // Month lengths and no data: preserve rows, leave measurement cells blank.
        foreach (DateTime date in new[] { new DateTime(2025, 2, 1), new DateTime(2026, 4, 1), new DateTime(2026, 10, 1) })
        {
            Check(Lab208MonthlyExporter.Export(output, date, "D1", "實驗室 A", "LAB-001", data.Clone()) == 0, "Empty month fabricated readings");
            using (var book = new Workbook())
            {
                book.LoadDocument(output); var sheet = book.Worksheets[0]; int days = DateTime.DaysInMonth(date.Year, date.Month);
                Check(sheet.Cells[days + 6, 0].Value.NumericValue == days, "Last valid day missing");
                if (days < 31) Check(sheet.Cells[days + 7, 0].Value.IsEmpty, "Invalid calendar day shown");
                Check(sheet.Cells["D8"].Value.IsEmpty && sheet.Cells["K38"].Value.IsEmpty, "Empty report contains measurements");
            }
        }
        // Keep a filled sample for visual review, without production data.
        var sample = Readings(); var sampleMonth = new DateTime(2026, 10, 1);
        for (int day = 0; day < 31; day++)
        {
            Add(sample, sampleMonth.AddDays(day).AddHours(8), 20m + day % 5);
            Add(sample, sampleMonth.AddDays(day).AddHours(16).AddMinutes(3), day == 5 ? 26m : 23m);
        }
        Lab208MonthlyExporter.Export(output, sampleMonth, "D1", "實驗室 A", "LAB-001", sample);
        Console.WriteLine("PASS: monthly template; 08:00/16:00 nearest within ±30 minutes; room/device/department scope; actual timestamps; leap/month-end days; excursion audit; blank readings/signatures.");
    }
}
