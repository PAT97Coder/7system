using DataAccessLayer;
using KnowledgeSystem.Helpers;
using System;
using System.Globalization;
using System.Threading;

internal static class ContractorAcceptanceErpTests
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }

    private static void Reject(ContractorAcceptance record, string message)
    {
        try { ContractorAcceptanceErp.Build(record); }
        catch (InvalidOperationException) { Check(true, message); return; }
        throw new Exception(message);
    }

    private static void Main()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("vi-VN");
        var record = new ContractorAcceptance { CostCenter = "LG7730", ContractNumber = "XXXX",
            Period = "2026-12", ReportDate = new DateTime(2026, 11, 25), DocumentNumber = "7700-202611" };
        record.Lines.Add(new ContractorAcceptanceLine { ExcelRow = 10, WorkCode = "B", Quantity = 2.5m, WorkPeriod = "2026/11/20-11/30" });
        record.Lines.Add(new ContractorAcceptanceLine { ExcelRow = 9, WorkCode = "A", Quantity = 1, WorkPeriod = "2026/11/01-11/15" });
        var actions = ContractorAcceptanceErp.Build(record);
        Check(actions.Count == 1 && actions[0].Step == 1, "Header and all lines run in one action");
        const string header = "20261101LG7730XXXX20261101";
        Check(actions[0].Text.StartsWith(header + "1{TAB}"),
            "Header auto-advances without tabs; fields 1 and 5 use yyyyMMdd work-month start");
        Check(actions[0].Text == header + "1{TAB}A1{DOWN}2{TAB}B2.5{DOWN}",
            "Each row tabs after sequence and sends one Down key after quantity; decimals invariant");
        record.Lines.RemoveAt(0);
        record.ContractNumber = "X+{Y}%";
        Check(ContractorAcceptanceErp.Build(record)[0].Text.Contains("X{+}{{}Y{}}{%}"), "Excel values escaped for SendKeys");
        Check(ContractorAcceptanceErp.Build(record)[0].Text.EndsWith("202611011{TAB}A1{DOWN}"), "Single line ends with one Down key returning to sequence field");
        record.Lines[0].WorkPeriod = "2026-02-28";
        Check(ContractorAcceptanceErp.Build(record)[0].Text.StartsWith("20260201LG7730"), "Work month independent of title month");
        record.Lines[0].WorkPeriod = "2026/02/30-03/01";
        Reject(record, "Invalid work date rejected");
        record.Lines[0].WorkPeriod = "2026/11/01-11/15";
        record.Lines.Add(new ContractorAcceptanceLine { ExcelRow = 10, WorkCode = "B", Quantity = 1, WorkPeriod = "2026/12/01" });
        Reject(record, "Mixed work months rejected");
        record.Lines.RemoveAt(1);
        record.CostCenter = "LG";
        Reject(record, "Invalid cost center rejected");
        record.CostCenter = "LG7730";
        record.Lines[0].WorkCode = "";
        Reject(record, "Missing work code rejected");
        record.Lines[0].WorkCode = "A\nB";
        Reject(record, "Control characters cannot inject navigation keys");
        record.Validation = "error";
        Reject(record, "Invalid record rejected");
    }
}
