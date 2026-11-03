using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using H = Exam312Tests;

internal static class ExcelImportTests
{
    private const string Manager = "VNW0000001", Learner = "VNW0000002";
    private static List<dt312_QuestionImportItem> Parse(DataTable data, string folder)
    {
        try
        {
            return (List<dt312_QuestionImportItem>)typeof(f312_QuesMgmt).Assembly
                .GetType("KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval.Exam312QuestionExcel")
                .GetMethod("Parse", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { data, folder });
        }
        catch (TargetInvocationException ex) { throw ex.InnerException; }
    }
    private static DataTable Sheet(int count = 9)
    {
        var table = new DataTable();
        for (int i = 0; i < count; i++) table.Columns.Add("Column" + i);
        return table;
    }
    private static void Expect<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { H.Check(true, typeof(T).Name); return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
    internal static void Run()
    {
        string source = Path.Combine(Path.GetTempPath(), "7system-Excel312-" + Guid.NewGuid().ToString("N"));
        string images = Path.Combine(source, "Images"), destination = Path.Combine(source, "Stored312");
        Directory.CreateDirectory(images); Directory.CreateDirectory(destination);
        foreach (string file in new[] { "question.png", "answer.png", "essay.png", "no-extension" })
            using (var bitmap = new Bitmap(20, 20)) bitmap.Save(Path.Combine(images, file), ImageFormat.Png);
        string oldFolder = TPConfigs.Folder312;
        TPConfigs.Folder312 = destination;
        try
        {
            var old = Sheet(7);
            old.Rows.Add("10", "舊題目", "no-extension", "1", "正確", "", "1");
            old.Rows.Add("10", "舊題目", "no-extension", "2", "錯誤", "", "0");
            var legacy = Parse(old, images);
            H.Check(legacy.Count == 1 && !legacy[0].Question.IsEssay && legacy[0].Answers.Count == 2, "Legacy 7-column file no longer works");
            H.Check(File.Exists(legacy[0].Question.ImageName), "Legacy extensionless image no longer works");

            var sheet = Sheet();
            sheet.Rows.Add("10", "選擇題一", "question.png", "1", "正確一", "answer.png", "1", "選擇題", "");
            sheet.Rows.Add("", "", "", "2", "錯誤一", "", "0", "", "");
            sheet.Rows.Add("2", "申論題 <script>測試</script>", "essay.png", "", "", "", "", "申論題", "REF_SECRET Nguyễn 中文\n第二行");
            sheet.Rows.Add("40", "複選題", "", "1", "正確甲", "answer.png", "1", "", "");
            sheet.Rows.Add("40", "複選題", "", "2", "正確乙", "", "1", "", "");
            sheet.Rows.Add("80", "無參考答案申論題", "", "", "", "", "", "申論題", "");
            sheet.Rows.Add("", "", "", "", "", "", "", "", "");
            var mixed = Parse(sheet, images);
            H.Check(mixed.Count == 4 && mixed.Count(q => q.Question.IsEssay) == 2, "Mixed sheet question/type count incorrect");
            H.Check(mixed.Where(q => q.Question.IsEssay).All(q => q.Answers.Count == 0), "Essay import creates MC options");
            H.Check(mixed[0].Answers.Count == 2 && mixed[2].Question.IsmultiAns, "MC continuation/multichoice key lost");
            H.Check(mixed[1].Question.ReferenceAnswer.Contains("Nguyễn 中文") && mixed[1].Question.ReferenceAnswer.Contains("第二行"), "Unicode/multiline reference lost");
            string previewHtml = f312_ExamReview.BuildQuestionPreviewHtml(mixed[1].Question, mixed[1].Answers, 2);
            H.Check(previewHtml.Contains("data:image/png;base64,") && previewHtml.Contains("REF_SECRET") && previewHtml.Contains("&lt;script&gt;"), "Essay import preview omits image/reference or fails HTML encoding");
            H.Check(!f312_ExamReview.BuildQuestionHtml(mixed[1].Question, mixed[1].Answers, 2).Contains("REF_SECRET"), "Import change leaks reference during exam");

            var bad = sheet.Copy(); bad.Rows[2][7] = "未知";
            Expect<ArgumentException>(() => Parse(bad, images));
            bad = sheet.Copy(); bad.Rows[2][4] = "誤填選項";
            Expect<ArgumentException>(() => Parse(bad, images));
            bad = sheet.Copy(); bad.Rows[2][2] = "missing.png";
            Expect<ArgumentException>(() => Parse(bad, images));
            bad = sheet.Copy(); bad.Rows[2][2] = "../outside.png";
            Expect<ArgumentException>(() => Parse(bad, images));
            bad = sheet.Copy(); bad.Rows[1][1] = "不一致題目";
            Expect<ArgumentException>(() => Parse(bad, images));
            Expect<ArgumentException>(() => Parse(Sheet(6), images));
            Expect<ArgumentException>(() => Parse(Sheet(), images));

            int group;
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var g = new dt312_Groups { DisplayName = "匯入測試", CreateAt = DateTime.Now.Date, CreateBy = Manager };
                db.dt312_Groups.Add(g); db.SaveChanges(); group = g.Id;
            }
            Expect<UnauthorizedAccessException>(() => dt312_QuestionsBUS.Instance.ImportQuestions(Learner, group, mixed, destination));
            Expect<InvalidOperationException>(() => dt312_QuestionsBUS.Instance.ImportQuestions(Manager, -999, mixed, destination));
            using (var preview = new f312_ExamReview(mixed))
            {
                preview.ShowInTaskbar = false; preview.Show(); Application.DoEvents();
                H.Check(!((BarButtonItem)H.Get(preview, "btnConfirmImport")).Enabled, "Import confirmation enabled before last question");
                H.Call(preview, "ConfirmImport_Click", null, null);
                H.Check(preview.Visible, "Premature confirmation closes preview");
                H.Call(preview, "PreviewNext_Click", null, null);
                var browser = (WebBrowser)H.Get(preview, "browser");
                Exam312DbTests.Wait(() => browser.ReadyState == WebBrowserReadyState.Complete && browser.Document?.Body != null, "Import essay preview");
                H.Check(browser.Document.Body.InnerText.Contains("REF_SECRET") && browser.Document.Images.Count == 1, "Preview DOM omits essay image/reference");
                H.Screenshot(preview, "Excel312-ImportEssay.png");
                H.Call(preview, "PreviewNext_Click", null, null); H.Call(preview, "PreviewNext_Click", null, null);
                H.Check(((BarButtonItem)H.Get(preview, "btnConfirmImport")).Enabled, "Cannot confirm at last question");
                H.Call(preview, "CancelImport_Click", null, null);
                H.Check(preview.DialogResult == DialogResult.Cancel, "Cancel import ignored");
            }
            using (var db = new DBDocumentManagementSystemEntities())
                H.Check(!db.dt312_Questions.Any(q => q.GroupId == group) && Directory.GetFiles(destination).Length == 0, "Cancelled preview wrote data/files");
            var ids = dt312_QuestionsBUS.Instance.ImportQuestions(Manager, group, mixed, destination);
            H.Check(ids.Count == 4 && ids.Distinct().Count() == 4, "Imported IDs are not unique");
            using (var db = new DBDocumentManagementSystemEntities())
            {
                H.Check(db.dt312_Questions.Count(q => q.GroupId == group) == 4, "Mixed import question count incorrect");
                for (int i = 0; i < mixed.Count; i++)
                {
                    var saved = db.dt312_Questions.Find(ids[i]);
                    var answers = db.dt312_Answers.Where(a => a.QuesId == saved.Id).OrderBy(a => a.Id).ToList();
                    H.Check(saved.IsEssay == mixed[i].Question.IsEssay && answers.Count == mixed[i].Answers.Count, "Question/answer ID mapping crossed questions");
                    H.Check(answers.Select(a => a.DisplayName).SequenceEqual(mixed[i].Answers.Select(a => a.DisplayName)), "Options mapped to wrong question");
                    H.Check(answers.All(a => string.IsNullOrEmpty(a.ImageName) || File.Exists(Path.Combine(destination, a.ImageName))), "Answer image not stored in 312 folder");
                }
                var essay = db.dt312_Questions.Find(ids[1]);
                H.Check(essay.ReferenceAnswer.Contains("REF_SECRET") && File.Exists(Path.Combine(destination, essay.ImageName)), "Essay image/reference did not persist");
                H.Check(f312_ExamReview.BuildQuestionPreviewHtml(essay, new List<dt312_Answers>(), 1).Contains("data:image/png;base64,"), "Saved essay image cannot be viewed");
            }
            int[] excelNumbers = mixed.Select(q => q.ExcelNumber).ToArray();
            H.Check(excelNumbers.SequenceEqual(new[] { 10, 2, 40, 80 }), "Import mutated Excel identifiers");
            int[] beforeFiles = { Directory.GetFiles(destination).Length };
            using (var db = new DBDocumentManagementSystemEntities())
                db.Database.ExecuteSqlCommand(@"CREATE TRIGGER dbo.TR_Test312ImportFail ON dbo.dt312_Answers AFTER INSERT AS THROW 51000, 'Injected import failure', 1;");
            try { Expect<System.Data.Entity.Infrastructure.DbUpdateException>(() => dt312_QuestionsBUS.Instance.ImportQuestions(Manager, group, mixed, destination)); }
            finally { using (var db = new DBDocumentManagementSystemEntities()) db.Database.ExecuteSqlCommand("DROP TRIGGER dbo.TR_Test312ImportFail"); }
            using (var db = new DBDocumentManagementSystemEntities())
                H.Check(db.dt312_Questions.Count(q => q.GroupId == group) == 4 && Directory.GetFiles(destination).Length == beforeFiles[0], "Failed import left partial questions/images");
            var essays = mixed.Where(q => q.Question.IsEssay).ToList();
            var essayIds = dt312_QuestionsBUS.Instance.ImportQuestions(Manager, group, essays, destination);
            using (var db = new DBDocumentManagementSystemEntities())
                H.Check(essayIds.Count == 2 && !db.dt312_Answers.Any(a => essayIds.Contains(a.QuesId)), "All-essay import fails without MC options");
            string keptImage;
            using (var db = new DBDocumentManagementSystemEntities()) keptImage = db.dt312_Questions.Find(ids[1]).ImageName;
            dt312_QuestionsBUS.Instance.SaveEssay(Manager, group, ids[1], "手動編輯申論題", "更新參考答案");
            using (var db = new DBDocumentManagementSystemEntities())
                H.Check(db.dt312_Questions.Find(ids[1]).ImageName == keptImage, "Manual essay edit removed imported image");
            Console.WriteLine("PASS: 7/9-column Excel; mixed/all-essay import with shared images; sparse IDs; Unicode; preview/cancel; correct mappings; rollback; manual essay editing preserved.");
        }
        finally
        {
            TPConfigs.Folder312 = oldFolder;
            // Test fixtures stay in a uniquely named temp directory for inspection.
        }
    }
}
