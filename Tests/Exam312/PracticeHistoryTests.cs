using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using H = Exam312Tests;

internal static class PracticeHistoryTests
{
    private const string Learner = "VNW0000002", Outsider = "VNW0000003", NewMenuUser = "VNW0000004";
    private static readonly dt312_PracticeBUS Bus = dt312_PracticeBUS.Instance;
    private static void Expect<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { H.Check(true, typeof(T).Name); return; }
        throw new Exception("Expected " + typeof(T).Name);
    }

    internal static void Run()
    {
        string officialBefore;
        using (var db = new DBDocumentManagementSystemEntities())
        {
            H.Check(db.Database.SqlQuery<int>(@"SELECT COUNT(*) FROM sys.foreign_keys
                WHERE parent_object_id IN (OBJECT_ID('dbo.dt312_ExternalParticipant'), OBJECT_ID('dbo.dt312_PracticeAttempt'))").Single() == 0,
                "Practice migration created foreign keys");
            H.Check(db.Database.SqlQuery<int>(@"SELECT COUNT(*) FROM sys.columns
                WHERE object_id IN (OBJECT_ID('dbo.dt312_ExternalParticipant'), OBJECT_ID('dbo.dt312_PracticeAttempt'))
                AND name='Id' AND system_type_id=TYPE_ID('int') AND is_identity=1").Single() == 2,
                "Practice tables do not follow int IDENTITY keys");
            officialBefore = JsonConvert.SerializeObject(db.dt312_ExamUser.OrderBy(x => x.Id).ToList());
            db.dm_Function.Add(new dm_Function { Id = 31204, IdParent = 0, DisplayName = "Thi thu", ControlName = "uc312ExamTest" });
            db.dm_FunctionRole.Add(new dm_FunctionRole { IdFunction = 31204, IdRole = 13 });
            db.SaveChanges();
        }
        H.Check(dt312_QuestionsBUS.Instance.CanPractice(NewMenuUser), "New UC permission cannot open practice");
        Expect<UnauthorizedAccessException>(() => Bus.StartExternal(Outsider, "X", "名字", "部門"));
        Expect<ArgumentException>(() => Bus.StartExternal(Learner, " ", "名字", "部門"));
        Expect<ArgumentException>(() => Bus.StartExternal(Learner, "X", " ", "部門"));
        Expect<ArgumentException>(() => Bus.StartExternal(Learner, "X", "名字", " "));
        Expect<ArgumentException>(() => Bus.StartExternal(Learner, new string('X', 65), "名字", "部門"));
        // A real SQL error during start must roll back the participant as well as the attempt.
        using (var db = new DBDocumentManagementSystemEntities())
            db.Database.ExecuteSqlCommand(@"CREATE TRIGGER dbo.TR_Test312RejectStart ON dbo.dt312_PracticeAttempt
                AFTER INSERT AS THROW 51000, 'Injected start failure', 1;");
        try { Expect<System.Data.Entity.Infrastructure.DbUpdateException>(() => Bus.StartExternal(Learner, "ROLLBACK", "外來人員", "測試")); }
        finally
        {
            using (var db = new DBDocumentManagementSystemEntities())
                db.Database.ExecuteSqlCommand("DROP TRIGGER dbo.TR_Test312RejectStart");
        }
        using (var db = new DBDocumentManagementSystemEntities())
            H.Check(db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.dt312_ExternalParticipant").Single() == 0,
                "Failed SQL start left an orphan candidate");
        foreach (string department in new[] { "7710", "7410", "7820" })
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var user = db.dm_User.Find(Learner);
                user.IdDepartment = department;
                user.DisplayNameVN = "Nguyễn Văn 測試";
                db.SaveChanges();
            }
            int id = Bus.StartInternal(Learner);
            Bus.Submit(Learner, id, 67, ReviewSnapshot());
            H.Check(Bus.GetResults(Learner, "INTERNAL").Any(x => x.Id == id && x.Department == department && x.Score == 67),
                "Department prefix or internal identity incorrect: " + department);
        }
        H.Check(!dt312_PracticeBUS.IsInternalDepartment("3120") && !dt312_PracticeBUS.IsInternalDepartment(null), "Invalid department accepted");
        using (var db = new DBDocumentManagementSystemEntities())
        { db.dm_User.Find(Learner).IdDepartment = "3120"; db.SaveChanges(); }
        Expect<InvalidOperationException>(() => Bus.StartInternal(Learner));
        string employeeBefore;
        using (var db = new DBDocumentManagementSystemEntities()) employeeBefore = JsonConvert.SerializeObject(db.dm_User.Find(Learner));
        int external = Bus.StartExternal(Learner, " " + Learner.ToLowerInvariant() + " ", " Nguyễn Thị 華 ", " 外部 / Phòng khác ");
        using (var db = new DBDocumentManagementSystemEntities())
        {
            H.Check(db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.dt312_ExternalParticipant WHERE EmployeeName=N'Nguyễn Thị 華'").Single() == 1,
                "External candidate not persisted at start or Unicode lost");
            H.Check(db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.dt312_PracticeAttempt WHERE Id=@id AND SubmitAt IS NULL", new SqlParameter("@id", external)).Single() == 1,
                "Attempt not persisted before exam");
        }
        H.Check(!Bus.GetResults(Learner, "EXTERNAL").Any(), "Unsubmitted attempt displayed as result");
        H.Check(Bus.GetResultDetail(Learner, external, "EXTERNAL") == null, "Unsubmitted answers exposed in detail");
        Expect<UnauthorizedAccessException>(() => Bus.GetResultDetail(Outsider, external, "EXTERNAL"));
        Expect<InvalidOperationException>(() => Bus.Submit(NewMenuUser, external, 50, "[]"));
        Expect<ArgumentException>(() => Bus.Submit(Learner, external, 50, " "));
        Bus.Submit(Learner, external, 50, "[]");
        Bus.Submit(Learner, external, 50, "[]"); // Idempotent retry does not create another result.
        Expect<InvalidOperationException>(() => Bus.Submit(Learner, external, 50, "[] "));
        int repeat = Bus.StartExternal(Learner, Learner, "新名字", "新部門");
        Bus.Submit(Learner, repeat, null, "[]");
        var history = Bus.GetResults(Learner, "EXTERNAL");
        H.Check(history.Count == 2 && history.Single(x => x.Id == external).EmployeeName == "Nguyễn Thị 華"
            && history.Single(x => x.Id == repeat).Department == "新部門" && history.Single(x => x.Id == repeat).Score == null,
            "Repeat exam overwrites identity history or all-essay score");
        Expect<InvalidOperationException>(() => Bus.Submit(Learner, external, 100, "[]"));
        H.Check(Bus.GetResults(Learner, "INTERNAL").Count == 3, "External results mixed into internal history");
        using (var db = new DBDocumentManagementSystemEntities())
        {
            H.Check(db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.dt312_ExternalParticipant").Single() == 1, "Repeated external code created duplicates");
            H.Check(JsonConvert.SerializeObject(db.dm_User.Find(Learner)) == employeeBefore, "External collision altered internal employee");
        }
        int unknown = Bus.StartExternal(Learner, "NOT_IN_DM_USER", "外來人員", "External Dept");
        // Exercise the actual shared submit handler, including SQL failure/retry and review.
        TPConfigs.LoginUser = new dm_User { Id = Learner };
        using (var exam = new f312_DoExam(true))
        {
            var method = exam.GetType().GetMethod("f312_DoExam_Load", BindingFlags.NonPublic | BindingFlags.Instance);
            exam.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), exam, method);
            var question = new dt312_Questions { Id = 10, DisplayName = "測試選擇題" };
            var options = new List<dt312_Answers> { new dt312_Answers { Id = 1, QuesId = 10, DisplayName = "正確", TrueAns = true } };
            var results = new List<f312_DoExam.ExamResult> { new f312_DoExam.ExamResult {
                Questions = question, Answers = options, QuestionIndex = 1, CorrectAnswer = "1", UserAnswer = "1" } };
            H.Set(exam, "ques", new List<dt312_Questions> { question }); H.Set(exam, "answers", options); H.Set(exam, "examResults", results);
            H.Set(exam, "timeRemaining", TimeSpan.FromMinutes(10));
            H.Set(exam, "countdownTimer", new Timer { Interval = 1000 });
            int calls = 0;
            exam.SavePracticeResult = (score, data) => {
                calls++;
                Bus.Submit(Learner, unknown, score, data);
            };
            exam.ShowInTaskbar = false; exam.Show(); Application.DoEvents(); H.Call(exam, "InitializeWebView2", 0);
            using (var db = new DBDocumentManagementSystemEntities())
                db.Database.ExecuteSqlCommand(@"CREATE TRIGGER dbo.TR_Test312RejectSubmit ON dbo.dt312_PracticeAttempt
                    AFTER UPDATE AS THROW 51000, 'Injected submit failure', 1;");
            try { using (var dialogs = new Exam312DbTests.Dialogs()) H.Call(exam, "Submit_Exam"); }
            finally
            {
                using (var db = new DBDocumentManagementSystemEntities())
                    db.Database.ExecuteSqlCommand("DROP TRIGGER dbo.TR_Test312RejectSubmit");
            }
            H.Check(!exam.PracticeResultSaved && exam.Visible && ((Timer)H.Get(exam, "countdownTimer")).Enabled,
                "Failed practice save closed exam or stopped retry timer");
            using (var dialogs = new Exam312DbTests.Dialogs())
            {
                H.Call(exam, "Submit_Exam");
                H.Check(dialogs.Reviews == 1 && exam.PracticeResultSaved, "Persisted practice result did not show review");
            }
            H.Check(calls == 2 && Bus.GetResults(Learner, "EXTERNAL").Single(x => x.Id == unknown).Score == 100,
                "Retry lost answers or used different scoring");
        }
        using (var host = new Form { ClientSize = new Size(1250, 800), ShowInTaskbar = false })
        using (var uc = new uc312ExamTest())
        {
            uc.Dock = DockStyle.Fill; host.Controls.Add(uc); host.Show(); Application.DoEvents();
            var view = (GridView)H.Get(uc, "resultsView");
            H.Check(view.VisibleColumns.Count == 6, "Unified result grid must have six columns");
            H.Check(view.DataRowCount == 6 && Bus.GetResults(Learner).Count == 6, "Unified history loses or duplicates attempts");
            H.Check(string.IsNullOrEmpty(view.GetRowCellDisplayText(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, "PracticeKind"))
                && string.IsNullOrEmpty(view.GetRowCellDisplayText(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, "Score")), "Empty filter row displays an unselected type or score");
            H.Check(uc.Controls.OfType<DevExpress.XtraLayout.LayoutControl>().Count() == 1
                && view.Appearance.HeaderPanel.Font.Size == 14.25F && view.Appearance.Row.Font.Size == 12F,
                "UC does not use standard LayoutControl/grid fonts");
            H.Check(!((DevExpress.XtraBars.BarButtonItem)H.Get(uc, "btnInternal")).Enabled && ((DevExpress.XtraBars.BarButtonItem)H.Get(uc, "btnExternal")).Enabled,
                "Non-7 department can start internal practice or cannot start external");
            view.Columns["Score"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
            Application.DoEvents();
            int internalId = Bus.GetResults(Learner, "INTERNAL").First().Id;
            H.Check(view.GetRowCellDisplayText(view.LocateByValue("Id", internalId), "PracticeKind") == "內部"
                && view.GetRowCellDisplayText(view.LocateByValue("Id", unknown), "PracticeKind") == "外部", "Practice type captions are not localized");
            CheckRightClickReview(uc, view, view.LocateByValue("Id", internalId), "INTERNAL");
            CheckRightClickReview(uc, view, view.LocateByValue("Id", unknown), "EXTERNAL");
            view.FocusedRowHandle = view.LocateByValue("Id", unknown);
            H.Call(uc, "RefreshResults");
            H.Check(((dt312_PracticeResult)view.GetFocusedRow()).Id == unknown, "Refresh does not retain selected attempt");
            H.Check(Bus.GetResultDetail(Learner, unknown, "INTERNAL") == null, "Detail lookup mixed internal and external rows");
            using (var menu = new DevExpress.XtraGrid.Menu.GridViewMenu(view))
            {
                var empty = new PopupMenuShowingEventArgs(menu, new DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo());
                H.Call(uc, "Results_PopupMenuShowing", view, empty);
                H.Check(menu.Items.Count == 0, "Empty grid area offers result detail");
            }
            using (var dialogs = new Exam312DbTests.Dialogs())
            {
                H.Call(uc, "ShowResultDetail", external, "EXTERNAL"); // [] is invalid review data.
                H.Check(dialogs.Reviews == 0 && dialogs.Messages > 0, "Invalid stored data not handled safely");
            }
            H.Screenshot(host, "PracticeHistory-1250.png");
            host.ClientSize = new Size(900, 600); Application.DoEvents();
            var grid = (DevExpress.XtraGrid.GridControl)H.Get(uc, "resultsGrid");
            H.Check(grid.Width > 850 && grid.Height > 450, "Unified result grid does not fill the screen");
            var smallBar = (DevExpress.XtraBars.Bar)H.Get(uc, "toolbar");
            var smallDock = (DevExpress.XtraBars.StandaloneBarDockControl)H.Get(uc, "toolbarDock");
            H.Check(smallBar.ItemLinks.Cast<DevExpress.XtraBars.BarItemLink>().Sum(link => link.Width) <= smallDock.Width,
                "Toolbar buttons overflow after resizing");
            H.Screenshot(host, "PracticeHistory-900.png"); host.Close();
        }
        using (var dialog = new f312_ExternalCandidate())
        {
            dialog.ShowInTaskbar = false; dialog.Show(); Application.DoEvents();
            using (var dialogs = new Exam312DbTests.Dialogs()) H.Call(dialog, "Start_Click", null, EventArgs.Empty);
            H.Check(dialog.Visible && dialog.DialogResult != DialogResult.OK, "Blank external entry starts exam");
            ((TextEdit)H.Get(dialog, "codeEdit")).Text = " outside ";
            ((TextEdit)H.Get(dialog, "nameEdit")).Text = " Nguyễn Thị 華 ";
            ((TextEdit)H.Get(dialog, "departmentEdit")).Text = " 部門 / Phòng ban ";
            H.Screenshot(dialog, "PracticeHistory-ExternalCandidate.png");
            H.Call(dialog, "Start_Click", null, EventArgs.Empty);
            H.Check(dialog.DialogResult == DialogResult.OK && dialog.EmployeeCode == "OUTSIDE"
                && dialog.EmployeeName == "Nguyễn Thị 華" && dialog.Department == "部門 / Phòng ban", "External form does not validate/trim/retain Unicode");
        }
        using (var db = new DBDocumentManagementSystemEntities())
            H.Check(JsonConvert.SerializeObject(db.dt312_ExamUser.OrderBy(x => x.Id).ToList()) == officialBefore,
                "New practice history modified official results");
        CheckConcurrencyAndNames();
        Console.WriteLine("PASS: practice history migration twice; internal departments; new UC permission; external Unicode and collisions; persistent attempts/results; repeat history; submit retry; unified six-column grid; sorted row review and selection persistence; responsive toolbar; official data unchanged.");
    }

    private static string ReviewSnapshot()
    {
        return JsonConvert.SerializeObject(new List<f312_DoExam.ExamResult> {
            new f312_DoExam.ExamResult {
                Questions = new dt312_Questions { Id=99, DisplayName="歷史題目", IsEssay=true, ReferenceAnswer="參考解答" },
                Answers = new List<dt312_Answers>(), QuestionIndex=1, UserAnswer="Tiếng Việt 中文", CorrectAnswer=""
            }
        });
    }

    private static void CheckRightClickReview(uc312ExamTest uc, GridView view, int rowHandle, string kind)
    {
        view.FocusedRowHandle = rowHandle == 0 ? 1 : 0;
        var selected = (dt312_PracticeResult)view.GetRow(rowHandle);
        var cell = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo)view.GetViewInfo()).GetGridCellInfo(rowHandle, view.Columns["EmployeeCode"]);
        var hit = view.CalcHitInfo(new Point(cell.Bounds.X + 5, cell.Bounds.Y + 5));
        using (var menu = new DevExpress.XtraGrid.Menu.GridViewMenu(view))
        {
            var nativeItem = new DevExpress.Utils.Menu.DXMenuItem("預設選單");
            menu.Items.Add(nativeItem);
            H.Call(uc, "Results_PopupMenuShowing", view, new PopupMenuShowingEventArgs(menu, hit));
            H.Check(menu.Items.Count == 2 && ReferenceEquals(menu.Items[0], nativeItem) && menu.Items[1].Caption == "查看詳情"
                && menu.Items[1].BeginGroup && menu.Items[1].Appearance.Font.Size == 12F,
                "Native menu replaced or business group/font missing");
            view.FocusedRowHandle = rowHandle == 0 ? 1 : 0; // Menu must retain the originally clicked attempt.
            using (var dialogs = new Exam312DbTests.Dialogs())
            {
                dialogs.InspectReview = review => {
                    var browser = (WebBrowser)H.Get(review, "browser");
                    string body = browser.Document.Body.InnerText;
                    var print = (BarButtonItem)H.Get(review, "btnPrint");
                    H.Check(print.Visibility == BarItemVisibility.Always && print.Enabled,
                        "Loaded result does not offer printing");
                    H.Check(((Bar)H.Get(review, "previewBar")).Visible
                        && ((BarButtonItem)H.Get(review, "btnConfirmImport")).Visibility == BarItemVisibility.Never,
                        "Result toolbar exposes import actions");
                    H.Check(body.Contains(selected.EmployeeCode) && body.Contains(selected.EmployeeName)
                        && body.Contains(selected.Department) && body.Contains(selected.SubmitAt.ToString("yyyy/MM/dd HH:mm:ss")),
                        "Review identity/date does not match clicked row");
                    H.Check(body.Contains(kind == "INTERNAL" ? "模擬測驗內部" : "模擬測驗外部") && body.Contains("您的答案"),
                        "Review type or saved answers missing");
                    if (kind == "INTERNAL") H.Check(body.Contains("Tiếng Việt 中文") && body.Contains("參考解答"), "Essay history not shown");
                    H.Screenshot(review, "PracticeHistory-Detail-" + kind + ".png");
                };
                menu.Items[1].GenerateClickEvent();
                H.Check(dialogs.Reviews == 1, "Clicked row did not open existing review form");
            }
        }
        var detail = Bus.GetResultDetail(Learner, selected.Id, kind);
        detail.EmployeeName = "<script>alert(1)</script>";
        string html = f312_ExamReview.BuildHtml(JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>(detail.ExamData), detail.Score, true, detail);
        H.Check(html.Contains("&lt;script&gt;") && !html.Contains("<script>"), "Participant header allows HTML injection");
        H.Check(html.Contains("<title>") && html.Contains("@media print") && html.Contains("@page"),
            "Result print document lacks title or print layout");
    }

    private static void CheckConcurrencyAndNames()
    {
        var starts = new[] {
            System.Threading.Tasks.Task.Run(() => Bus.StartExternal(Learner, "CONCURRENT", "第一位", "部門甲")),
            System.Threading.Tasks.Task.Run(() => Bus.StartExternal(Learner, "CONCURRENT", "第二位", "部門乙"))
        };
        System.Threading.Tasks.Task.WaitAll(starts);
        using (var db = new DBDocumentManagementSystemEntities())
        {
            H.Check(starts[0].Result != starts[1].Result && db.dt312_ExternalParticipant.Count(p => p.EmployeeCode == "CONCURRENT") == 1,
                "Concurrent EF starts duplicate participant or attempt");
            H.Check(db.dt312_PracticeAttempt.Find(starts[0].Result).EmployeeName == "第一位"
                && db.dt312_PracticeAttempt.Find(starts[1].Result).EmployeeName == "第二位", "Concurrent starts mixed participant snapshots");
        }
        int attempt = starts[0].Result;
        var submissions = new[] { 40, 90 }.Select(score => System.Threading.Tasks.Task.Run(() => {
            try { Bus.Submit(Learner, attempt, score, "[]"); return true; }
            catch (InvalidOperationException) { return false; }
        })).ToArray();
        System.Threading.Tasks.Task.WaitAll(submissions);
        H.Check(submissions.Count(t => t.Result) == 1, "Concurrent EF submit overwrote a completed result");
        foreach (var names in new[] {
            new[] { "繁體姓名", "Tên Việt", "繁體姓名" },
            new[] { "", "Tên Việt", "Tên Việt" },
            new[] { "", "", Learner }
        })
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var user = db.dm_User.Find(Learner); user.IdDepartment = "7710";
                user.DisplayName = names[0]; user.DisplayNameVN = names[1]; db.SaveChanges();
            }
            int id = Bus.StartInternal(" " + Learner.ToLowerInvariant() + " ");
            using (var db = new DBDocumentManagementSystemEntities())
                H.Check(db.dt312_PracticeAttempt.Find(id).EmployeeName == names[2], "Traditional/Vietnamese/account name priority failed");
        }
        H.Check(DevExpress.XtraGrid.Localization.GridLocalizer.Active.GetLocalizedString(
            DevExpress.XtraGrid.Localization.GridStringId.MenuColumnSortAscending) == "遞增排序", "Native grid menu not localized");
    }

}
