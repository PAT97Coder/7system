using BusinessLayer;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._17_ExamStatistics
{
    public static class Exam317TemplateExporter
    {
        public const int ProfessionalForm = 1;
        public const int ChineseForm = 2;
        public const int ChineseRetakeForm = 3;
        public const int InterviewForm = 4;
        public const int SummaryForm = 5;

        private static readonly Dictionary<int, string> TemplateNames = new Dictionary<int, string>
        {
            { ProfessionalForm, "TE-701-T001-01 技術工程師評鑑學科成績明細表.xlsx" },
            { ChineseForm, "TE-701-T001-02 技術工程師評鑑中文考試成績明細表.xlsx" },
            { ChineseRetakeForm, "TE-701-T001-03 冶金部技術工程師中文補考成績表.xlsx" },
            { InterviewForm, "TE-701-T001-04 冶金部技術工程師口試報告成績表.xlsx" },
            { SummaryForm, "TE-701-T001-05 冶金部技術工程師評鑑結果彙總表.xlsx" }
        };

        public static string ExportForm(int formNumber, Exam317ExportData data, string outputFolder)
        {
            if (!TemplateNames.TryGetValue(formNumber, out string templateName))
                throw new ArgumentOutOfRangeException(nameof(formNumber));

            string templatePath = Path.Combine(TPConfigs.ResourcesPath, "317", "Templates", templateName);
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Không tìm thấy file mẫu xuất biểu 317.", templatePath);

            Directory.CreateDirectory(outputFolder);
            string outputPath = Path.Combine(outputFolder, templateName);
            File.Copy(templatePath, outputPath, true);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(outputPath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                switch (formNumber)
                {
                    case ProfessionalForm:
                        FillProfessionalForm(worksheet, data);
                        break;
                    case ChineseForm:
                        FillChineseForm(worksheet, data);
                        break;
                    case ChineseRetakeForm:
                        FillChineseRetakeForm(worksheet, data);
                        break;
                    case InterviewForm:
                        FillInterviewForm(worksheet, data);
                        break;
                    case SummaryForm:
                        FillSummaryForm(worksheet, data);
                        break;
                }

                package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                package.Save();
            }

            return outputPath;
        }

        public static List<string> ExportAll(Exam317ExportData data, string outputFolder)
        {
            return TemplateNames.Keys
                .OrderBy(item => item)
                .Select(item => ExportForm(item, data, outputFolder))
                .ToList();
        }

        private static void FillProfessionalForm(ExcelWorksheet worksheet, Exam317ExportData data)
        {
            List<Exam317ExportPerson> rows = data.People
                .Where(item => item.IsProfessionalCandidate)
                .OrderBy(item => item.DepartmentCode)
                .ThenBy(item => item.UserId)
                .ToList();
            SetYearAndDate(worksheet, "A4", "D4", data);
            int lastDataRow = EnsureDataRows(worksheet, 6, 15, 16, rows.Count, 5, 15);

            for (int index = 0; index < rows.Count; index++)
            {
                Exam317ExportPerson item = rows[index];
                int row = 6 + index;
                worksheet.Cells[row, 1].Value = item.DepartmentName;
                worksheet.Cells[row, 2].Value = item.DepartmentCode;
                worksheet.Cells[row, 3].Value = item.UserId;
                worksheet.Cells[row, 4].Value = item.UserName;
                worksheet.Cells[row, 5].Value = item.ProfessionalScore;
            }

            FormatTable(worksheet, 6, lastDataRow, 5,
                new[] { 30d, 14d, 18d, 24d, 17d },
                new[] { 1, 4 }, new[] { 5 }, 24d, false);
            worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, lastDataRow + 2, 5];
        }

        private static void FillChineseForm(ExcelWorksheet worksheet, Exam317ExportData data)
        {
            List<Exam317ExportPerson> rows = data.People
                .Where(item => item.IsChineseCandidate)
                .OrderBy(item => item.DepartmentCode)
                .ThenBy(item => item.UserId)
                .ToList();
            SetYearAndDate(worksheet, "A4", "F4", data);
            int lastDataRow = EnsureDataRows(worksheet, 6, 15, 16, rows.Count, 8, 15);

            for (int index = 0; index < rows.Count; index++)
            {
                Exam317ExportPerson item = rows[index];
                int row = 6 + index;
                worksheet.Cells[row, 1].Value = item.DepartmentName;
                worksheet.Cells[row, 2].Value = item.DepartmentCode;
                worksheet.Cells[row, 3].Value = item.UserId;
                worksheet.Cells[row, 4].Value = item.UserName;
                worksheet.Cells[row, 5].Value = item.ChineseOfficialScore;
                worksheet.Cells[row, 6].Value = item.ChineseRetakeActualScore;
                worksheet.Cells[row, 7].Value = null;
                worksheet.Cells[row, 8].Value = PassText(item.ChinesePassed) ?? "N";
            }

            UpdatePassHeaderForRetakeRule(worksheet, "H5");
            FormatTable(worksheet, 6, lastDataRow, 8,
                new[] { 30d, 14d, 18d, 24d, 19d, 19d, 13d, 23d },
                new[] { 1, 4 }, new[] { 5, 6 }, 24d, true);
            worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, lastDataRow + 2, 8];
        }

        private static void FillChineseRetakeForm(ExcelWorksheet worksheet, Exam317ExportData data)
        {
            List<Exam317ExportPerson> rows = data.People
                .Where(item => item.IsChineseRetakeFormCandidate)
                .OrderBy(item => item.DepartmentCode)
                .ThenBy(item => item.UserId)
                .ToList();
            SetYearAndDate(worksheet, "A4", "G4", data);
            int lastDataRow = EnsureDataRows(worksheet, 7, 14, 15, rows.Count, 9, 14);

            for (int index = 0; index < rows.Count; index++)
            {
                Exam317ExportPerson item = rows[index];
                int row = 7 + index;
                worksheet.Cells[row, 1].Value = item.DepartmentName;
                worksheet.Cells[row, 2].Value = item.DepartmentCode;
                worksheet.Cells[row, 3].Value = item.UserId;
                worksheet.Cells[row, 4].Value = item.UserName;
                worksheet.Cells[row, 5].Value = item.ChineseOfficialScore;
                worksheet.Cells[row, 6].Value = item.ChineseRetakeActualScore;
                worksheet.Cells[row, 7].Value = GetRetakeRecognizedScore(item);
                worksheet.Cells[row, 8].Value = PassText(item.ChinesePassed) ?? "N";
                worksheet.Cells[row, 9].Value = item.ChineseRetakePassed == true ? "*" : null;
            }

            UpdatePassHeaderForRetakeRule(worksheet, "H5");
            FormatTable(worksheet, 7, lastDataRow, 9,
                new[] { 30d, 14d, 18d, 24d, 19d, 19d, 19d, 23d, 12d },
                new[] { 1, 4, 9 }, new[] { 5, 6, 7 }, 24d, true);
            worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, lastDataRow + 3, 9];
        }

        private static void FillInterviewForm(ExcelWorksheet worksheet, Exam317ExportData data)
        {
            List<Exam317InterviewExportCandidate> candidates = data.InterviewCandidates
                .OrderBy(item => item.UserId)
                .ToList();
            SetYearAndDate(worksheet, "A4", "L4", data);

            worksheet.Cells["A7:A9"].Merge = false;
            worksheet.Cells["B7:B9"].Merge = false;
            worksheet.Cells["C7:C9"].Merge = false;
            worksheet.Cells["D7:D9"].Merge = false;
            worksheet.Cells["O7:O9"].Merge = false;

            int requiredRows = candidates.Any()
                ? candidates.Sum(item => Math.Max(3, item.Scores.Count))
                : 3;
            int extraRows = Math.Max(0, requiredRows - 3);
            if (extraRows > 0)
                worksheet.InsertRow(10, extraRows, 9);

            for (int index = 0; index < requiredRows; index++)
            {
                int targetRow = 7 + index;
                int templateRow = 7 + Math.Min(index % 3, 2);
                if (targetRow > 9)
                    worksheet.Cells[templateRow, 1, templateRow, 15]
                        .Copy(worksheet.Cells[targetRow, 1, targetRow, 15]);
                worksheet.Row(targetRow).Height = worksheet.Row(templateRow).Height;
            }
            worksheet.Cells[7, 1, 6 + requiredRows, 15].Value = null;

            int currentRow = 7;
            if (!candidates.Any())
            {
                MergeInterviewCandidateCells(worksheet, currentRow, currentRow + 2);
            }
            else
            {
                Dictionary<string, Exam317ExportPerson> people = data.People
                    .ToDictionary(item => item.UserId);
                foreach (Exam317InterviewExportCandidate candidate in candidates)
                {
                    people.TryGetValue(candidate.UserId, out Exam317ExportPerson person);
                    int groupRows = Math.Max(3, candidate.Scores.Count);
                    int groupEndRow = currentRow + groupRows - 1;
                    MergeInterviewCandidateCells(worksheet, currentRow, groupEndRow);
                    worksheet.Cells[currentRow, 1].Value = person?.DepartmentName;
                    worksheet.Cells[currentRow, 2].Value = person?.DepartmentCode;
                    worksheet.Cells[currentRow, 3].Value = candidate.UserId;
                    worksheet.Cells[currentRow, 4].Value = person?.UserName;

                    for (int index = 0; index < candidate.Scores.Count; index++)
                    {
                        Exam317InterviewExportScore score = candidate.Scores[index];
                        int row = currentRow + index;
                        worksheet.Cells[row, 5].Value = score.InterviewerName;
                        worksheet.Cells[row, 6].Value = score.ProfessionalSkill;
                        worksheet.Cells[row, 7].Value = score.ProfessionalSkillNote;
                        worksheet.Cells[row, 8].Value = score.Responsiveness;
                        worksheet.Cells[row, 9].Value = score.ResponsivenessNote;
                        worksheet.Cells[row, 10].Value = score.Communication;
                        worksheet.Cells[row, 11].Value = score.CommunicationNote;
                        worksheet.Cells[row, 12].Value = score.ReportQuality;
                        worksheet.Cells[row, 13].Value = score.ReportQualityNote;
                        if (score.Total.HasValue)
                            worksheet.Cells[row, 14].Formula = string.Format(
                                "F{0}*0.4+H{0}*0.3+J{0}*0.2+L{0}*0.1", row);
                    }

                    if (candidate.Scores.Any(item => item.Total.HasValue))
                        worksheet.Cells[currentRow, 15].Formula = string.Format(
                            "AVERAGE(N{0}:N{1})", currentRow, groupEndRow);
                    currentRow += groupRows;
                }
            }

            int footerRow = 12 + extraRows;
            int lastDataRow = 6 + requiredRows;
            FormatTable(worksheet, 7, lastDataRow, 15,
                new[] { 28d, 14d, 18d, 23d, 18d, 10d, 22d, 10d, 22d,
                    10d, 22d, 10d, 22d, 14d, 16d },
                new[] { 1, 4, 5, 7, 9, 11, 13 },
                new[] { 6, 8, 10, 12, 14, 15 }, 38d, true);
            worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, footerRow, 15];
        }

        private static void FillSummaryForm(ExcelWorksheet worksheet, Exam317ExportData data)
        {
            List<Exam317ExportPerson> rows = data.People
                .OrderBy(item => item.Rank)
                .ThenBy(item => item.UserId)
                .ToList();
            SetYearAndDate(worksheet, "A4", "I4", data);
            worksheet.Cells["G8:H8"].Merge = false;
            int lastDataRow = EnsureDataRows(worksheet, 6, 18, 19, rows.Count, 11, 18);
            worksheet.DeleteRow(lastDataRow + 1, 1);

            for (int index = 0; index < rows.Count; index++)
            {
                Exam317ExportPerson item = rows[index];
                int row = 6 + index;
                worksheet.Cells[row, 1].Value = item.DepartmentName;
                worksheet.Cells[row, 2].Value = item.DepartmentCode;
                worksheet.Cells[row, 3].Value = item.UserId;
                worksheet.Cells[row, 4].Value = item.UserName;
                worksheet.Cells[row, 5].Value = item.JobName;
                worksheet.Cells[row, 6].Value = item.JobCode;
                worksheet.Cells[row, 7].Value = item.ProfessionalScore;
                worksheet.Cells[row, 8].Value = item.ChineseRecognizedScore;
                worksheet.Cells[row, 9].Value = item.InterviewScore;
                worksheet.Cells[row, 10].Formula = string.Format(
                    "IF(G{0}=\"\",0,G{0})*0.2+IF(H{0}=\"\",0,H{0})*0.3+IF(I{0}=\"\",0,I{0})*0.5", row);
                worksheet.Cells[row, 11].Value = item.TotalScore >= 80m ? "Y" : "N";
            }

            FormatTable(worksheet, 6, lastDataRow, 11,
                new[] { 30d, 14d, 18d, 24d, 24d, 15d, 17d, 17d, 17d, 15d, 20d },
                new[] { 1, 4, 5 }, new[] { 7, 8, 9, 10 }, 24d, true);
            worksheet.PrinterSettings.PrintArea = worksheet.Cells[1, 1, lastDataRow + 1, 11];
        }

        private static int EnsureDataRows(ExcelWorksheet worksheet, int dataStartRow,
            int originalDataEndRow, int insertBeforeRow, int dataCount, int columnCount,
            int styleSourceRow)
        {
            int capacity = originalDataEndRow - dataStartRow + 1;
            int extraRows = Math.Max(0, dataCount - capacity);
            if (extraRows > 0)
            {
                worksheet.InsertRow(insertBeforeRow, extraRows, styleSourceRow);
                for (int index = 0; index < extraRows; index++)
                    worksheet.Row(insertBeforeRow + index).Height = worksheet.Row(styleSourceRow).Height;
            }

            int lastDataRow = originalDataEndRow + extraRows;
            worksheet.Cells[dataStartRow, 1, lastDataRow, columnCount].Value = null;
            return lastDataRow;
        }

        private static void SetYearAndDate(ExcelWorksheet worksheet, string yearCell,
            string dateCell, Exam317ExportData data)
        {
            worksheet.Cells[yearCell].Value = "Năm/年度：" + data.Year;
            worksheet.Cells[dateCell].Value = "Ngày xuất biểu/制定日期："
                                              + data.ExportedAt.ToString("yyyy/MM/dd");
        }

        private static void MergeInterviewCandidateCells(ExcelWorksheet worksheet,
            int startRow, int endRow)
        {
            foreach (int column in new[] { 1, 2, 3, 4, 15 })
                worksheet.Cells[startRow, column, endRow, column].Merge = true;
        }

        private static decimal? GetRetakeRecognizedScore(Exam317ExportPerson item)
        {
            if (!item.ChineseRetakeActualScore.HasValue)
                return null;
            return item.ChineseRetakePassed == true ? 78m : item.ChineseRetakeActualScore;
        }

        private static string PassText(bool? passed)
        {
            return passed.HasValue ? (passed.Value ? "Y" : "N") : null;
        }

        private static void FormatTable(ExcelWorksheet worksheet, int dataStartRow,
            int dataEndRow, int columnCount, double[] columnWidths,
            int[] leftAlignedColumns, int[] numericColumns, double rowHeight,
            bool landscape)
        {
            for (int column = 1; column <= columnWidths.Length; column++)
                worksheet.Column(column).Width = columnWidths[column - 1];

            ExcelRange dataRange = worksheet.Cells[dataStartRow, 1, dataEndRow, columnCount];
            dataRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            dataRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            dataRange.Style.WrapText = true;
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            foreach (int column in leftAlignedColumns)
                worksheet.Cells[dataStartRow, column, dataEndRow, column]
                    .Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            foreach (int column in numericColumns)
                worksheet.Cells[dataStartRow, column, dataEndRow, column]
                    .Style.Numberformat.Format = "0.##";
            for (int row = dataStartRow; row <= dataEndRow; row++)
                worksheet.Row(row).Height = Math.Max(worksheet.Row(row).Height, rowHeight);

            worksheet.PrinterSettings.Orientation = landscape
                ? eOrientation.Landscape
                : eOrientation.Portrait;
            worksheet.PrinterSettings.FitToPage = true;
            worksheet.PrinterSettings.FitToWidth = 1;
            worksheet.PrinterSettings.FitToHeight = 0;
            worksheet.PrinterSettings.HorizontalCentered = true;
        }

        private static void UpdatePassHeaderForRetakeRule(ExcelWorksheet worksheet, string cellAddress)
        {
            string header = Convert.ToString(worksheet.Cells[cellAddress].Value);
            if (!string.IsNullOrWhiteSpace(header))
                worksheet.Cells[cellAddress].Value = header.Replace("(成績>=80分)", "(依考試及格標準)");
        }
    }
}
