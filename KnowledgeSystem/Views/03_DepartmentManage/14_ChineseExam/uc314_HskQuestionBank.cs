using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    public partial class uc314_HskQuestionBank : XtraUserControl
    {
        private sealed class QuestionBankRow
        {
            public int Id { get; set; }
            public string LevelCode { get; set; }
            public string SectionCode { get; set; }
            public string PartCode { get; set; }
            public string GroupType { get; set; }
            public string QuestionType { get; set; }
            public string DisplayText { get; set; }
            public bool HaveImg { get; set; }
            public bool IsMultiAns { get; set; }
            public bool IsActive { get; set; }
            public bool HasSharedPassage { get; set; }
            public bool HasSharedWordBank { get; set; }
            public string SharedPassagePreview { get; set; }
            public string SharedWordBankPreview { get; set; }
        }

        private readonly BindingSource sourceQues = new BindingSource();
        private readonly Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        public uc314_HskQuestionBank()
        {
            InitializeComponent();
            btnUpload.ImageOptions.SvgImage = TPSvgimages.UploadFile;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            var questions = dt314_HskQuestionsBUS.Instance.GetList();
            var displayMap = Hsk314ReadingBankRepository.GetQuestionDisplayMap(questions.Select(r => r.Id));

            var rows = questions.Select(q =>
            {
                Hsk314ReadingQuestionDisplay display = null;
                displayMap.TryGetValue(q.Id, out display);
                return new QuestionBankRow()
                {
                    Id = q.Id,
                    LevelCode = q.LevelCode,
                    SectionCode = q.SectionCode,
                    PartCode = display?.PartCode,
                    GroupType = display?.GroupType,
                    QuestionType = q.QuestionType,
                    DisplayText = q.DisplayText,
                    HaveImg = !string.IsNullOrWhiteSpace(q.ImageName),
                    IsMultiAns = q.IsMultiAns,
                    IsActive = q.IsActive,
                    HasSharedPassage = !string.IsNullOrWhiteSpace(display?.SharedPassage),
                    HasSharedWordBank = !string.IsNullOrWhiteSpace(display?.SharedOptionPool),
                    SharedPassagePreview = ShortText(display?.SharedPassage, 80),
                    SharedWordBankPreview = ShortText(display?.SharedOptionPool, 80)
                };
            }).ToList();

            sourceQues.DataSource = rows;
            gcData.DataSource = sourceQues;
            EnsureExtraColumns();
            gvQues.BestFitColumns();
        }

        private void uc314_HskQuestionBank_Load(object sender, EventArgs e)
        {
            gvQues.ReadOnlyGridView();
            gvAns.ReadOnlyGridView();
            gvQues.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvAns.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportReadingTemplateByType();
            return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel files (*.xlsx)|*.xlsx";
                dlg.FileName = $"HSK_Question_Template_{DateTime.Now:yyyyMMdd}.xlsx";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo(dlg.FileName)))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("EXCEL");
                    string[] headers =
                    {
                        "GroupNo",
                        "QuesNo",
                        "Level",
                        "Section",
                        "PartCode",
                        "GroupType",
                        "GroupTitle",
                        "InstructionText",
                        "SharedPassage",
                        "SharedOptionPool",
                        "QuestionType",
                        "Question",
                        "QuestionImage",
                        "Answer",
                        "AnswerImage",
                        "TrueAns",
                        "QuestionOrder",
                        "AnswerOrder",
                        "Remark"
                    };

                    ws.Cells[1, 1].Value = "314 HSK Reading Import Template";
                    ws.Cells[2, 1].Value = "1 file / 1 sheet. Fill sample rows below and keep header names unchanged for import.";
                    ws.Cells[3, 1].Value = "Supported sample blocks in this template: SharedWordBank, SentenceOrder, PassageCloze, SharedPassage.";
                    ws.Cells[4, 1].Value = "Notes: same GroupNo = one block, same QuesNo = one question, TrueAns uses 1/0, images go in Images folder next to the Excel file.";
                    ws.Cells[1, 1, 1, headers.Length].Merge = true;
                    ws.Cells[2, 1, 2, headers.Length].Merge = true;
                    ws.Cells[3, 1, 3, headers.Length].Merge = true;
                    ws.Cells[4, 1, 4, headers.Length].Merge = true;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ws.Cells[6, i + 1].Value = headers[i];
                    }

                    object[,] sample =
                    {
                        { "G001", "Q001", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 Word Bank 46-50", "第 46-50 题：选词填空。", "", "A 重  B 首先  C 观众  D 坚持  E 擦  F 地点", "SingleChoice", "爷爷，为什么橡皮能（ ）掉铅笔写的字？", "", "擦", "", 1, 46, 1, "Sample type 1 - SharedWordBank" },
                        { "G001", "Q001", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 Word Bank 46-50", "第 46-50 题：选词填空。", "", "A 重  B 首先  C 观众  D 坚持  E 擦  F 地点", "SingleChoice", "爷爷，为什么橡皮能（ ）掉铅笔写的字？", "", "观众", "", 0, 46, 2, "Sample type 1 - SharedWordBank" },
                        { "G001", "Q002", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 Word Bank 46-50", "第 46-50 题：选词填空。", "", "A 重  B 首先  C 观众  D 坚持  E 擦  F 地点", "SingleChoice", "这部电影非常感人，很多（ ）都被感动得哭了。", "", "观众", "", 1, 47, 1, "Sample type 1 - SharedWordBank" },
                        { "G001", "Q002", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 Word Bank 46-50", "第 46-50 题：选词填空。", "", "A 重  B 首先  C 观众  D 坚持  E 擦  F 地点", "SingleChoice", "这部电影非常感人，很多（ ）都被感动得哭了。", "", "地点", "", 0, 47, 2, "Sample type 1 - SharedWordBank" },
                        { "G002", "Q003", "HSK4", "Reading", "ReadingPart2", "SentenceOrder", "HSK4 Order 56", "第 56-65 题：排列顺序。", "", "", "SentenceOrder", "A 意思是希望朋友之间的友好关系  B 能够一直继续下去，越久越好  C 人们常说“友谊地久天长”", "", "C-A-B", "", 1, 56, 1, "Sample type 2 - SentenceOrder" },
                        { "G002", "Q003", "HSK4", "Reading", "ReadingPart2", "SentenceOrder", "HSK4 Order 56", "第 56-65 题：排列顺序。", "", "", "SentenceOrder", "A 意思是希望朋友之间的友好关系  B 能够一直继续下去，越久越好  C 人们常说“友谊地久天长”", "", "A-B-C", "", 0, 56, 2, "Sample type 2 - SentenceOrder" },
                        { "G003", "Q004", "HSK5", "Reading", "ReadingPart1", "PassageCloze", "HSK5 Cloze 46-48", "第 46-48 题：请选出正确答案。", "有一个年轻人在一家公司做得很出色，他为自己设计了一个美好的未来，对 46 充满信心。", "", "SingleChoice", "46．对（ ）充满信心。", "", "未来", "", 1, 46, 1, "Sample type 3 - PassageCloze" },
                        { "G003", "Q004", "HSK5", "Reading", "ReadingPart1", "PassageCloze", "HSK5 Cloze 46-48", "第 46-48 题：请选出正确答案。", "有一个年轻人在一家公司做得很出色，他为自己设计了一个美好的未来，对 46 充满信心。", "", "SingleChoice", "46．对（ ）充满信心。", "", "天气", "", 0, 46, 2, "Sample type 3 - PassageCloze" },
                        { "G004", "Q005", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 Passage 71-74", "第 71-74 题：请根据短文选出正确答案。", "一个冬天，一个人带着猎狗去打猎。那个人一枪击中了一只兔子的腿，受伤的兔子拼命地跑。", "", "SingleChoice", "兔子的腿怎么了？", "", "被枪打中了", "", 1, 71, 1, "Sample type 4 - SharedPassage" },
                        { "G004", "Q005", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 Passage 71-74", "第 71-74 题：请根据短文选出正确答案。", "一个冬天，一个人带着猎狗去打猎。那个人一枪击中了一只兔子的腿，受伤的兔子拼命地跑。", "", "SingleChoice", "兔子的腿怎么了？", "", "摔伤了", "", 0, 71, 2, "Sample type 4 - SharedPassage" },
                        { "G004", "Q006", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 Passage 71-74", "第 71-74 题：请根据短文选出正确答案。", "一个冬天，一个人带着猎狗去打猎。那个人一枪击中了一只兔子的腿，受伤的兔子拼命地跑。", "", "SingleChoice", "猎狗为什么没追上兔子？", "", "兔子拼命地跑", "", 1, 72, 1, "Sample type 4 - SharedPassage" },
                        { "G004", "Q006", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 Passage 71-74", "第 71-74 题：请根据短文选出正确答案。", "一个冬天，一个人带着猎狗去打猎。那个人一枪击中了一只兔子的腿，受伤的兔子拼命地跑。", "", "SingleChoice", "猎狗为什么没追上兔子？", "", "因为它睡着了", "", 0, 72, 2, "Sample type 4 - SharedPassage" }
                    };

                    for (int row = 0; row < sample.GetLength(0); row++)
                    {
                        for (int col = 0; col < sample.GetLength(1); col++)
                        {
                            ws.Cells[row + 7, col + 1].Value = sample[row, col];
                        }
                    }

                    using (ExcelRange title = ws.Cells[1, 1, 1, headers.Length])
                    {
                        title.Style.Font.Bold = true;
                        title.Style.Font.Size = 14;
                        title.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        title.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 78, 121));
                        title.Style.Font.Color.SetColor(Color.White);
                    }

                    using (ExcelRange info = ws.Cells[2, 1, 4, headers.Length])
                    {
                        info.Style.WrapText = true;
                        info.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        info.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
                    }

                    using (ExcelRange rng = ws.Cells[6, 1, 6, headers.Length])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    int lastRow = sample.GetLength(0) + 6;
                    using (ExcelRange all = ws.Cells[6, 1, lastRow, headers.Length])
                    {
                        all.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        all.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        all.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        all.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        all.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    PaintGroup(ws, 7, 10, headers.Length, Color.FromArgb(226, 239, 218));
                    PaintGroup(ws, 11, 12, headers.Length, Color.FromArgb(255, 242, 204));
                    PaintGroup(ws, 13, 14, headers.Length, Color.FromArgb(252, 228, 214));
                    PaintGroup(ws, 15, 18, headers.Length, Color.FromArgb(222, 234, 246));

                    ws.Row(1).Height = 24;
                    ws.Row(2).Height = 22;
                    ws.Row(3).Height = 22;
                    ws.Row(4).Height = 36;
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    ws.View.FreezePanes(7, 1);

                    pck.Save();
                }

                Process.Start(dlg.FileName);
            }
        }

        private static void PaintGroup(ExcelWorksheet ws, int fromRow, int toRow, int totalCols, Color color)
        {
            using (ExcelRange rng = ws.Cells[fromRow, 1, toRow, totalCols])
            {
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(color);
            }
        }

        private void ExportReadingTemplateByType()
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel files (*.xlsx)|*.xlsx";
                dlg.FileName = $"314_HSK_Reading_Template_{DateTime.Now:yyyyMMdd}.xlsx";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo(dlg.FileName)))
                {
                    string[] headers =
                    {
                        "GroupNo",
                        "QuesNo",
                        "Level",
                        "Section",
                        "PartCode",
                        "GroupType",
                        "GroupTitle",
                        "InstructionText",
                        "SharedPassage",
                        "SharedOptionPool",
                        "QuestionType",
                        "Question",
                        "QuestionImage",
                        "Answer",
                        "AnswerImage",
                        "TrueAns",
                        "QuestionOrder",
                        "AnswerOrder",
                        "Remark"
                    };

                    CreateTemplateSheet(
                        pck.Workbook.Worksheets.Add("共享詞語"),
                        headers,
                        "314 HSK 閱讀題庫匯入範本",
                        "題型：共享詞語（SharedWordBank）",
                        "適用：HSK4 ReadingPart1。相同 GroupNo 代表同一組詞庫；相同 QuesNo 代表同一題。",
                        new object[,]
                        {
                            { "G001", "Q001", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 詞語組 46-50", "第 46-50 題：選詞填空。", "", "A 重  B 首先  C 觀眾  D 堅持  E 擦  F 地點", "SingleChoice", "爺爺，為什麼橡皮能（ ）掉鉛筆寫的字？", "", "擦", "", 1, 46, 1, "共享詞語範例" },
                            { "G001", "Q001", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 詞語組 46-50", "第 46-50 題：選詞填空。", "", "A 重  B 首先  C 觀眾  D 堅持  E 擦  F 地點", "SingleChoice", "爺爺，為什麼橡皮能（ ）掉鉛筆寫的字？", "", "觀眾", "", 0, 46, 2, "共享詞語範例" },
                            { "G001", "Q002", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 詞語組 46-50", "第 46-50 題：選詞填空。", "", "A 重  B 首先  C 觀眾  D 堅持  E 擦  F 地點", "SingleChoice", "這部電影非常感人，很多（ ）都被感動得哭了。", "", "觀眾", "", 1, 47, 1, "共享詞語範例" },
                            { "G001", "Q002", "HSK4", "Reading", "ReadingPart1", "SharedWordBank", "HSK4 詞語組 46-50", "第 46-50 題：選詞填空。", "", "A 重  B 首先  C 觀眾  D 堅持  E 擦  F 地點", "SingleChoice", "這部電影非常感人，很多（ ）都被感動得哭了。", "", "地點", "", 0, 47, 2, "共享詞語範例" }
                        },
                        Color.FromArgb(226, 239, 218));

                    CreateTemplateSheet(
                        pck.Workbook.Worksheets.Add("語序排列"),
                        headers,
                        "314 HSK 閱讀題庫匯入範本",
                        "題型：語序排列（SentenceOrder）",
                        "適用：HSK4 ReadingPart2。Question 欄位可直接放 A/B/C 句段，Answer 欄位放正確順序。",
                        new object[,]
                        {
                            { "G002", "Q003", "HSK4", "Reading", "ReadingPart2", "SentenceOrder", "HSK4 語序 56", "第 56-65 題：排列順序。", "", "", "SentenceOrder", "A 意思是希望朋友之間的友好關係  B 能夠一直繼續下去，越久越好  C 人們常說「友誼地久天長」", "", "C-A-B", "", 1, 56, 1, "語序排列範例" },
                            { "G002", "Q003", "HSK4", "Reading", "ReadingPart2", "SentenceOrder", "HSK4 語序 56", "第 56-65 題：排列順序。", "", "", "SentenceOrder", "A 意思是希望朋友之間的友好關係  B 能夠一直繼續下去，越久越好  C 人們常說「友誼地久天長」", "", "A-B-C", "", 0, 56, 2, "語序排列範例" }
                        },
                        Color.FromArgb(255, 242, 204));

                    CreateTemplateSheet(
                        pck.Workbook.Worksheets.Add("完形填空"),
                        headers,
                        "314 HSK 閱讀題庫匯入範本",
                        "題型：完形填空（PassageCloze）",
                        "適用：HSK5 ReadingPart1。SharedPassage 放整段短文，Question 放每小題句子。",
                        new object[,]
                        {
                            { "G003", "Q004", "HSK5", "Reading", "ReadingPart1", "PassageCloze", "HSK5 完形 46-48", "第 46-48 題：請選出正確答案。", "有一個年輕人在一家公司做得很出色，他為自己設計了一個美好的未來，對 46 充滿信心。", "", "SingleChoice", "46．對（ ）充滿信心。", "", "未來", "", 1, 46, 1, "完形填空範例" },
                            { "G003", "Q004", "HSK5", "Reading", "ReadingPart1", "PassageCloze", "HSK5 完形 46-48", "第 46-48 題：請選出正確答案。", "有一個年輕人在一家公司做得很出色，他為自己設計了一個美好的未來，對 46 充滿信心。", "", "SingleChoice", "46．對（ ）充滿信心。", "", "天氣", "", 0, 46, 2, "完形填空範例" }
                        },
                        Color.FromArgb(252, 228, 214));

                    CreateTemplateSheet(
                        pck.Workbook.Worksheets.Add("短文閱讀"),
                        headers,
                        "314 HSK 閱讀題庫匯入範本",
                        "題型：短文閱讀（SharedPassage）",
                        "適用：HSK5 ReadingPart3。SharedPassage 放共用短文，同一 GroupNo 可對應多題。",
                        new object[,]
                        {
                            { "G004", "Q005", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 閱讀 71-74", "第 71-74 題：請根據短文選出正確答案。", "一個冬天，一個人帶著獵狗去打獵。那個人一槍擊中了一隻兔子的腿，受傷的兔子拼命地跑。", "", "SingleChoice", "兔子的腿怎麼了？", "", "被槍打中了", "", 1, 71, 1, "短文閱讀範例" },
                            { "G004", "Q005", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 閱讀 71-74", "第 71-74 題：請根據短文選出正確答案。", "一個冬天，一個人帶著獵狗去打獵。那個人一槍擊中了一隻兔子的腿，受傷的兔子拼命地跑。", "", "SingleChoice", "兔子的腿怎麼了？", "", "摔傷了", "", 0, 71, 2, "短文閱讀範例" },
                            { "G004", "Q006", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 閱讀 71-74", "第 71-74 題：請根據短文選出正確答案。", "一個冬天，一個人帶著獵狗去打獵。那個人一槍擊中了一隻兔子的腿，受傷的兔子拼命地跑。", "", "SingleChoice", "獵狗為什麼沒追上兔子？", "", "兔子拼命地跑", "", 1, 72, 1, "短文閱讀範例" },
                            { "G004", "Q006", "HSK5", "Reading", "ReadingPart3", "SharedPassage", "HSK5 閱讀 71-74", "第 71-74 題：請根據短文選出正確答案。", "一個冬天，一個人帶著獵狗去打獵。那個人一槍擊中了一隻兔子的腿，受傷的兔子拼命地跑。", "", "SingleChoice", "獵狗為什麼沒追上兔子？", "", "因為牠睡著了", "", 0, 72, 2, "短文閱讀範例" }
                        },
                        Color.FromArgb(222, 234, 246));

                    pck.Save();
                }

                Process.Start(dlg.FileName);
            }
        }

        private static void CreateTemplateSheet(ExcelWorksheet ws, string[] headers, string titleText, string typeText, string noteText, object[,] sample, Color sampleColor)
        {
            ws.Cells[1, 1].Value = titleText;
            ws.Cells[2, 1].Value = typeText;
            ws.Cells[3, 1].Value = noteText;
            ws.Cells[4, 1].Value = "欄位說明：TrueAns=1 代表正確答案；若有圖片，請將圖片放在 Excel 同層的 Images 資料夾。";
            ws.Cells[1, 1, 1, headers.Length].Merge = true;
            ws.Cells[2, 1, 2, headers.Length].Merge = true;
            ws.Cells[3, 1, 3, headers.Length].Merge = true;
            ws.Cells[4, 1, 4, headers.Length].Merge = true;

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[6, i + 1].Value = headers[i];
            }

            for (int row = 0; row < sample.GetLength(0); row++)
            {
                for (int col = 0; col < sample.GetLength(1); col++)
                {
                    ws.Cells[row + 7, col + 1].Value = sample[row, col];
                }
            }

            using (ExcelRange title = ws.Cells[1, 1, 1, headers.Length])
            {
                title.Style.Font.Bold = true;
                title.Style.Font.Size = 14;
                title.Style.Fill.PatternType = ExcelFillStyle.Solid;
                title.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 78, 121));
                title.Style.Font.Color.SetColor(Color.White);
            }

            using (ExcelRange info = ws.Cells[2, 1, 4, headers.Length])
            {
                info.Style.WrapText = true;
                info.Style.Fill.PatternType = ExcelFillStyle.Solid;
                info.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
            }

            using (ExcelRange header = ws.Cells[6, 1, 6, headers.Length])
            {
                header.Style.Font.Bold = true;
                header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
                header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                header.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            int lastRow = sample.GetLength(0) + 6;
            using (ExcelRange all = ws.Cells[6, 1, lastRow, headers.Length])
            {
                all.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                all.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                all.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                all.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                all.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            using (ExcelRange sampleRange = ws.Cells[7, 1, lastRow, headers.Length])
            {
                sampleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                sampleRange.Style.Fill.BackgroundColor.SetColor(sampleColor);
            }

            ws.Row(1).Height = 24;
            ws.Row(2).Height = 22;
            ws.Row(3).Height = 22;
            ws.Row(4).Height = 36;
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            ws.View.FreezePanes(7, 1);
        }

        private void btnUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                DataTable table = ReadExcel(dlg.FileName);
                string err = ValidateImport(table);
                if (!string.IsNullOrEmpty(err))
                {
                    MsgTP.MsgError(err);
                    return;
                }

                if (XtraMessageBox.Show($"Import {table.Rows.Count} answer rows?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                ImportQuestions(table, Path.Combine(Path.GetDirectoryName(dlg.FileName), "Images"));
                LoadData();
            }
        }

        private DataTable ReadExcel(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                });
                return ds.Tables["EXCEL"] ?? ds.Tables[0];
            }
        }

        private string ValidateImport(DataTable table)
        {
            if (Hsk314ReadingBankRepository.HasGroupedColumns(table))
            {
                return Hsk314ReadingBankRepository.ValidateGroupedImport(table);
            }

            string[] required = { "QuesNo", "Level", "Section", "QuestionType", "Question", "Answer", "TrueAns" };
            foreach (string col in required)
                if (!table.Columns.Contains(col)) return $"Missing column: {col}";

            var groups = table.Rows.Cast<DataRow>().GroupBy(r => Read(r, "QuesNo"));
            foreach (var group in groups)
            {
                string level = Read(group.First(), "Level");
                string section = Read(group.First(), "Section");
                string qType = Read(group.First(), "QuestionType");
                if (!Hsk314Constants.Levels.Contains(level)) return $"Invalid Level: {level}";
                if (!Hsk314Constants.Sections.Contains(section)) return $"Invalid Section: {section}";
                if (!Hsk314Constants.QuestionTypes.Contains(qType)) return $"Invalid QuestionType: {qType}";
                if (!group.Any(r => Read(r, "TrueAns") == "1")) return $"Question {group.Key} has no correct answer.";
            }
            return "";
        }

        private void ImportQuestions(DataTable table, string imageFolder)
        {
            if (Hsk314ReadingBankRepository.HasGroupedColumns(table))
            {
                Hsk314ReadingBankRepository.ImportGroupedQuestions(table, imageFolder, TPConfigs.LoginUser?.Id);
                return;
            }

            foreach (var group in table.Rows.Cast<DataRow>().GroupBy(r => Read(r, "QuesNo")))
            {
                DataRow first = group.First();
                var question = new dt314_HskQuestions()
                {
                    LevelCode = Read(first, "Level"),
                    SectionCode = Read(first, "Section"),
                    QuestionType = Read(first, "QuestionType"),
                    DisplayText = Clean(Read(first, "Question")),
                    ImageName = CopyImage(Read(first, "QuestionImage"), imageFolder),
                    IsMultiAns = group.Count(r => Read(r, "TrueAns") == "1") > 1 || Read(first, "QuestionType") == "MultiChoice",
                    IsActive = true,
                    CreatedBy = TPConfigs.LoginUser?.Id,
                    CreatedDate = DateTime.Now,
                    Remark = Read(first, "Remark")
                };

                int quesId = dt314_HskQuestionsBUS.Instance.Add(question);
                if (quesId <= 0) continue;

                int order = 1;
                var answers = group.Select(r => new dt314_HskAnswers()
                {
                    QuesId = quesId,
                    DisplayText = Clean(Read(r, "Answer")),
                    ImageName = CopyImage(Read(r, "AnswerImage"), imageFolder),
                    TrueAns = Read(r, "TrueAns") == "1",
                    DisplayOrder = order++,
                    IsActive = true
                }).ToList();

                dt314_HskAnswersBUS.Instance.AddRange(answers);
            }
        }

        private string CopyImage(string imageName, string imageFolder)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return "";
            string source = Path.Combine(imageFolder, imageName);
            if (!File.Exists(source)) return "";

            string destFolder = TPConfigs.Folder307;
            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
            string encrypted = $"{EncryptionHelper.EncryptionFileName(Path.GetFileNameWithoutExtension(imageName))}{Path.GetExtension(imageName)}";
            File.Copy(source, Path.Combine(destFolder, encrypted), true);
            return encrypted;
        }

        private static string Read(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) ? row[col]?.ToString().Trim() ?? "" : "";
        }

        private static string Clean(string input)
        {
            return Regex.Replace(input ?? "", @"[\t\n\r\s]+", m => m.Value.Contains("\n") ? "\r\n" : " ").Trim();
        }

        private void EnsureExtraColumns()
        {
            AddColumnIfMissing("PartCode", "Part", 95, 2);
            AddColumnIfMissing("GroupType", "GroupType", 130, 3);
            AddColumnIfMissing("HasSharedPassage", "HasPassage", 90, 6);
            AddColumnIfMissing("HasSharedWordBank", "HasWordBank", 95, 7);
            AddColumnIfMissing("SharedPassagePreview", "PassagePreview", 220, 8);
            AddColumnIfMissing("SharedWordBankPreview", "WordBankPreview", 220, 9);
        }

        private void AddColumnIfMissing(string fieldName, string caption, int width, int visibleIndex)
        {
            if (gvQues.Columns[fieldName] != null) return;

            var column = gvQues.Columns.AddVisible(fieldName, caption);
            column.Width = width;
            column.VisibleIndex = visibleIndex;
        }

        private static string ShortText(string value, int maxLen)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            string text = Regex.Replace(value, @"\s+", " ").Trim();
            return text.Length <= maxLen ? text : text.Substring(0, maxLen) + "...";
        }

        private void gvQues_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e) { e.IsEmpty = false; }
        private void gvQues_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) { e.RelationCount = 1; }
        private void gvQues_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) { e.RelationName = "Answers"; }
        private void gvQues_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            int idQues = Convert.ToInt32(gvQues.GetRowCellValue(e.RowHandle, gColId));
            e.ChildList = dt314_HskAnswersBUS.Instance.GetListByQues(idQues);
        }

        private void gvQues_DoubleClick(object sender, EventArgs e)
        {
            gvQues.ExpandMasterRow(gvQues.FocusedRowHandle, 0);
        }

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.Appearance.Font = fontUI14;
                view.IndicatorWidth = Math.Max(view.IndicatorWidth, 45);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "HaveImg" && e.IsGetData)
            {
                string imgName = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "ImageName")?.ToString();
                e.Value = !string.IsNullOrWhiteSpace(imgName);
            }
        }
    }
}
