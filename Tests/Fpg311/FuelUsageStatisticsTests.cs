using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;

internal static class FuelUsageStatisticsTests
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }

    private static FuelUsageDetail Entry(string department, string type, string fuel, int month, decimal quantity, string invoice)
    {
        return new FuelUsageDetail { Department = department, VehicleType = type, FuelCategory = fuel,
            Date = new DateTime(2026, month, 15), Quantity = quantity, Invoice = invoice, Plate = type + "-TEST" };
    }

    private static void Main(string[] args)
    {
        string output = Path.Combine(args[1], "FuelUsageStatistics-Test.xlsx");
        Directory.CreateDirectory(args[1]);
        var rows = Enumerable.Range(1, 60).Select(index => Entry("7710", "摩托車", "汽油", index == 60 ? 12 : 1, 1.125m, "M-" + index)).ToList();
        rows.Add(Entry("7820", "汽車", "柴油", 2, 10.5m, "CAR-DIESEL"));
        rows.Add(Entry("7710", "汽車", "汽油", 1, 25m, "CAR-PETROL"));
        rows.Add(Entry("7710", "貨卡車", "柴油", 1, 100.125m, "TRUCK"));
        rows.Add(Entry("7710", "推高車", "柴油", 3, 50m, "FORKLIFT"));
        rows.Add(Entry("7710", "", "柴油", 4, 7.5m, "UNREGISTERED-VEHICLE"));
        FuelUsageStatisticsExporter.Export(args[0], output, 2026, "7710、7820", rows);
        using (var template = new ExcelPackage(new FileInfo(args[0])))
        using (var package = new ExcelPackage(new FileInfo(output)))
        {
            decimal detailSum = 0m;
            foreach (string name in new[] { "摩托車", "汽車", "貨卡車", "推高車", "未分類" })
            {
                var sheet = package.Workbook.Worksheets[name];
                var original = template.Workbook.Worksheets[name == "未分類" ? "摩托車" : name];
                int count = rows.Count(row => name == "未分類" ? string.IsNullOrEmpty(row.VehicleType) : row.VehicleType == name);
                for (int row = 3; row < count + 3; row++)
                {
                    decimal litres = sheet.Cells[row, 8].GetValue<decimal>();
                    Check(litres == sheet.Cells[row, 11].GetValue<decimal>(), name + " H/K litres agree at " + row);
                    detailSum += litres;
                    Check(sheet.Cells[row, 1].GetValue<string>() == rows.First(item => item.Invoice == sheet.Cells[row, 6].GetValue<string>()).Department,
                        "department comes from invoice");
                    Check(Enumerable.Range(1, 12).All(column => sheet.Cells[row, column].Style.Font.Name == original.Cells[3, column == 12 ? 9 : column].Style.Font.Name
                        && sheet.Cells[row, column].Style.Font.Size == original.Cells[3, column == 12 ? 9 : column].Style.Font.Size), "inserted row fonts inherit template cells");
                }
                Check(Enumerable.Range(1, 11).All(column => sheet.Column(column).Width == original.Column(column).Width
                    && sheet.Column(column).Hidden == original.Column(column).Hidden
                    && sheet.Cells[2, column].Text == original.Cells[2, column].Text
                    && sheet.Cells[2, column].Style.Font.Name == original.Cells[2, column].Style.Font.Name
                    && sheet.Cells[2, column].Style.Font.Size == original.Cells[2, column].Style.Font.Size), "template headers, widths, hidden columns and fonts preserved");
                Check(sheet.Row(1).Height == original.Row(1).Height && sheet.Row(2).Height >= original.Row(2).Height
                    && sheet.Cells[1, 1].Text.Contains("7710、7820"), "title layout retained and department updated");
            }
            Check(detailSum == rows.Sum(row => row.Quantity.Value), "all four detail sheets total correctly");
            var summary = package.Workbook.Worksheets["統計表"];
            decimal monthlySum = 0m, totalSum = 0m, decemberPetrol = 0m;
            string department = null;
            for (int row = 3; row <= summary.Dimension.End.Row; row++)
            {
                if (summary.Cells[row, 1].Value != null) department = summary.Cells[row, 1].Text;
                if (summary.Cells[row, 3].Value == null) continue;
                decimal litres = summary.Cells[row, 5].GetValue<decimal>();
                if (summary.Cells[row, 9].Value == null) totalSum += litres;
                else monthlySum += litres;
                if (department == "7710" && summary.Cells[row, 9].Text == "12" && summary.Cells[row, 3].Text == template.Workbook.Worksheets["統計表"].Cells[3, 3].Text)
                    decemberPetrol += litres;
                decimal density = summary.Cells[row, 6].GetValue<decimal>();
                Check(Math.Abs(summary.Cells[row, 7].GetValue<decimal>() - litres * density) < 0.000001m, "weight conversion calculated");
            }
            Check(monthlySum == detailSum && totalSum == detailSum, "summary monthly and annual totals reconcile to every invoice");
            Check(decemberPetrol == 1.125m, "record after template row 50 included in month 12");
            Check(summary.Cells[1, 1].Text.Contains("2026") && summary.Cells[1, 1].Text.Contains("7710、7820"), "summary header matches selected year and actual departments");
            var summaryTemplate = template.Workbook.Worksheets["統計表"];
            Check(Enumerable.Range(1, 9).All(column => summary.Column(column).Width == summaryTemplate.Column(column).Width
                && summary.Column(column).Hidden == summaryTemplate.Column(column).Hidden
                && summary.Cells[2, column].Text == summaryTemplate.Cells[2, column].Text), "summary template columns and header captions preserved");
            Check(summary.MergedCells.Contains("A1:H1") && summary.MergedCells.Any(address => address.StartsWith("B3:")), "summary title and monthly merged layout retained");
        }
        string emptyOutput = Path.Combine(args[1], "FuelUsageStatistics-EmptyTypes.xlsx");
        FuelUsageStatisticsExporter.Export(args[0], emptyOutput, 2026, "7710", rows.Take(1).ToList());
        using (var package = new ExcelPackage(new FileInfo(emptyOutput)))
            foreach (string name in new[] { "汽車", "貨卡車", "推高車" })
                Check(package.Workbook.Worksheets[name].Cells[3, 8].Value == null && package.Workbook.Worksheets[name].Cells[3, 11].Value == null,
                    name + " contains no stale template litres");
        var invalid = Entry("7710", "汽車", "汽油", 1, 1m, "MISSING-QTY");
        invalid.Quantity = null;
        bool rejected = false;
        try { FuelUsageStatisticsExporter.Export(args[0], Path.Combine(args[1], "Invalid.xlsx"), 2026, "7710", new[] { invalid }); }
        catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "missing quantity is rejected instead of silently counted as zero");
        Console.WriteLine("Preview: " + output);
    }
}
