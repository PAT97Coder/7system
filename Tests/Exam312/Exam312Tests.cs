using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using Newtonsoft.Json;

internal static class Exam312Tests
{
    private const BindingFlags F = BindingFlags.Instance | BindingFlags.NonPublic;
    internal static int Checks;
    internal static string OutputDir = Path.Combine(Path.GetTempPath(), "7system-Exam312-TestArtifacts");
    internal static object Get(object o, string name) => o.GetType().GetField(name, F).GetValue(o);
    internal static void Set(object o, string name, object value) => o.GetType().GetField(name, F).SetValue(o, value);
    internal static object Call(object o, string name, params object[] args) => o.GetType().GetMethod(name, F).Invoke(o, args);
    internal static void Check(bool ok, string message) { Checks++; if (!ok) throw new Exception(message); }
    internal static void Screenshot(Form form, string name)
    {
        Directory.CreateDirectory(OutputDir);
        bool hasBrowser=ContainsBrowser(form);
        bool wasTopMost=form.TopMost;
        if(hasBrowser) form.TopMost=true;
        form.BringToFront(); form.Activate(); form.Update(); Application.DoEvents();
        System.Threading.Thread.Sleep(200); Application.DoEvents();
        using (var bitmap = new Bitmap(form.ClientSize.Width, form.ClientSize.Height))
        {
            if(hasBrowser)
            {
                using (var graphics = Graphics.FromImage(bitmap))
                    graphics.CopyFromScreen(form.PointToScreen(Point.Empty), Point.Empty, bitmap.Size);
            }
            else form.DrawToBitmap(bitmap,new Rectangle(Point.Empty,bitmap.Size));
            bitmap.Save(Path.Combine(OutputDir, name));
        }
        form.TopMost=wasTopMost;
    }
    private static bool ContainsBrowser(Control parent) => parent is WebBrowser || parent.Controls.Cast<Control>().Any(ContainsBrowser);
    [STAThread]
    public static int Main()
    {
        try
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Directory.CreateDirectory(OutputDir);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EXAM312_TEST_CONNECTION"))) Exam312DbTests.Run();
            Check(dt312_ExamScoring.IsCorrect("1,3", "3,1"), "Multichoice order affects grading");
            Check(!dt312_ExamScoring.IsCorrect("1,3", "1"), "Partial selection accepted");
            Check(!dt312_ExamScoring.IsCorrect("1", "1,2"), "Extra answer accepted");
            Check(!dt312_ExamScoring.IsCorrect("", "") && !dt312_ExamScoring.IsCorrect("1", null), "Blank/missing key counted as correct");
            Check(dt312_ExamScoring.Score(1, 2) == 50 && dt312_ExamScoring.Score(2, 3) == 67, "Score rounding incorrect");
            Check(dt312_ExamScoring.Score(0, 0) == null, "All-essay exam receives a grade");
            using(var allocator=new f312_DoExam(true))
            {
                var random=new Random(312);
                for(int run=0;run<100;run++)
                {
                    var groups=Enumerable.Range(1,random.Next(1,9)).Select(id=>new f312_DoExam.GroupDistribution { GroupId=id,MaxAvailable=random.Next(0,30) }).ToList();
                    int needed=random.Next(0,150), total=groups.Sum(g=>g.MaxAvailable);
                    allocator.DistributeQuestions(groups,needed);
                    Check(groups.Sum(g=>g.CountToTake)==Math.Min(needed,total),"Allocation lost requested questions");
                    Check(groups.All(g=>g.CountToTake>=0&&g.CountToTake<=g.MaxAvailable),"Allocation exceeded group capacity");
                }
            }
            var assembly = typeof(dt312_Questions).Assembly;
            var metadata = new MetadataWorkspace(new[] { "res://*/ModelDocManager.csdl", "res://*/ModelDocManager.ssdl", "res://*/ModelDocManager.msl" }, new[] { assembly });
            var entity = metadata.GetItems<EntityType>(DataSpace.CSpace).Single(e => e.Name == "dt312_Questions");
            Check(entity.Properties.Any(p => p.Name == "IsEssay") && entity.Properties.Any(p => p.Name == "ReferenceAnswer"), "Essay EF properties missing");
            metadata.GetItemCollection(DataSpace.CSSpace);

            var essay = new dt312_Questions { Id = 1, GroupId = 1, DisplayName = "說明操作步驟 <script>alert(1)</script>", IsEssay = true, ReferenceAnswer = "REFERENCE_SECRET\n先確認安全" };
            var choice = new dt312_Questions { Id = 2, GroupId = 1, DisplayName = "選擇安全措施", IsEssay = false };
            var second = new dt312_Questions { Id = 3, GroupId = 1, DisplayName = "選擇正確步驟", IsEssay = false };
            var answerList = new List<dt312_Answers> { new dt312_Answers { QuesId = 2, Id = 1, DisplayName = "確認安全", TrueAns = true }, new dt312_Answers { QuesId = 2, Id = 2, DisplayName = "忽略安全", TrueAns = false } };
            string questionHtml = f312_ExamReview.BuildQuestionHtml(essay, new List<dt312_Answers>(), 1);
            Check(!questionHtml.Contains("REFERENCE_SECRET"), "Reference answer leaked before submitting");
            Check(questionHtml.Contains("&lt;script&gt;") && !questionHtml.Contains("<script>"), "Question text executes HTML");
            var legacy=JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>("[{\"Questions\":{\"Id\":5,\"DisplayName\":\"舊題目\"},\"Answers\":[],\"QuestionIndex\":1,\"CorrectAnswer\":\"1\",\"UserAnswer\":\"2\",\"IsCorrect\":false}]");
            Check(!legacy[0].Questions.IsEssay && f312_ExamReview.BuildHtml(legacy,0,false).Contains("舊題目"),"Old JSON snapshot cannot be reviewed");
            Check(!f312_ExamReview.BuildQuestionHtml(choice,answerList,2).Contains("正確答案"),"MC key leaked before submit");
            var results = new List<f312_DoExam.ExamResult>
            {
                new f312_DoExam.ExamResult { Questions = essay, QuestionIndex = 1, Answers = new List<dt312_Answers>(), UserAnswer = "", CorrectAnswer = "" },
                new f312_DoExam.ExamResult { Questions = choice, QuestionIndex = 2, Answers = answerList, UserAnswer = "1", CorrectAnswer = "1" },
                new f312_DoExam.ExamResult { Questions = second, QuestionIndex = 3, Answers = new List<dt312_Answers>(), UserAnswer = "2", CorrectAnswer = "1" }
            };
            using (var dialog = new f312_EssayQuestion("test", 1, essay, true))
            {
                dialog.ShowInTaskbar = false; dialog.Show(); Application.DoEvents();
                var layout = (DevExpress.XtraLayout.LayoutControl)Get(dialog, "layout");
                foreach (BaseEdit editor in layout.Controls.OfType<BaseEdit>())
                    Check(editor.Width > 300 && editor.Height > 80 && editor.Bottom <= layout.Height, "Essay editor is clipped");
                Screenshot(dialog, "Exam312-EssayEditor.png"); dialog.Close();
            }
            using (var exam = new f312_DoExam(true))
            {
                exam.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), exam, exam.GetType().GetMethod("f312_DoExam_Load", F));
                Set(exam, "ques", new List<dt312_Questions> { essay, choice, second });
                Set(exam, "answers", answerList); Set(exam, "examResults", results); Set(exam, "numQues", 3);
                exam.idExamUser = 123456; // Practice must not query/write an assigned official exam even if an ID exists.
                exam.ShowInTaskbar = false; exam.Show(); Application.DoEvents();
                Call(exam, "InitializeWebView2", 0); Application.DoEvents();
                var memo = (MemoEdit)Get(exam, "essayAnswer"); memo.Text = "我的答案\r\n第二行";
                Call(exam, "NextQues"); Check(results[0].UserAnswer.Contains("第二行"), "Essay answer lost on next question");
                Call(exam, "PreviousQues"); Check(memo.Text.Contains("第二行"), "Essay answer not restored");
                Screenshot(exam, "Exam312-EssayExam.png");
                Check(memo.Visible && memo.Height >= 70, "Essay input not visible or too short: " + memo.Bounds);
                memo.Text = "最後一次修改";
                int reviews = 0;
                using (var closeReview = new Timer { Interval = 100 })
                {
                    closeReview.Tick += (s, e) =>
                    {
                        var review = Application.OpenForms.Cast<Form>().OfType<f312_ExamReview>().FirstOrDefault();
                        if (review == null) return;
                        var browser = (WebBrowser)Get(review, "browser");
                        if (browser.ReadyState != WebBrowserReadyState.Complete || browser.Document?.Body == null) return;
                        Check(browser.Width > 600 && browser.Height > 400, "Review browser is clipped: " + browser.Bounds);
                        string html = browser.DocumentText;
                        Check(html.Contains("50／100") && html.Contains("REFERENCE_SECRET"), "Practice does not show score and reference answer");
                        Check(browser.Document.Body.InnerText.Contains("50／100"), "Practice score is not rendered");
                        closeReview.Stop();
                        review.Update(); browser.Update();
                        Screenshot(review, "Exam312-Review.png");
                        reviews++; closeReview.Stop(); review.Close();
                    };
                    closeReview.Start(); Call(exam, "Submit_Exam");
                }
                Check(reviews == 1 && results[0].UserAnswer == "最後一次修改", "Last answer lost on submit");
                Check(!results[0].IsCorrect && results[1].IsCorrect && !results[2].IsCorrect, "Essay affected automatic grading");
            }
            string reviewHtml = f312_ExamReview.BuildHtml(results, 50, true);
            Check(reviewHtml.Contains("答錯") && reviewHtml.Contains("正確答案") && reviewHtml.Contains("您的答案"), "Practice review does not identify mistakes");
            var restored = JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>(JsonConvert.SerializeObject(results));
            Check(restored[0].Questions.IsEssay && restored[0].UserAnswer == "最後一次修改", "Essay answer lost from stored official snapshot");
            Console.WriteLine("PASS: EF model/mapping; multichoice grading; essays excluded; no answers before submit; input persistence/navigation/submit; practice score/review without official DB access; HTML encoding; JSON snapshots; editor layout.");
            File.WriteAllText(Path.Combine(OutputDir, "unit-results.txt"), "PASS: " + Checks + " assertions; EF metadata, UI, navigation, scoring, JSON and HTML encoding.\r\n");
            Console.WriteLine("Assertions: " + Checks + "; artifacts: " + OutputDir);
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
