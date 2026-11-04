using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using H = Exam312Tests;

internal static class ExamAnalysisTests
{
    private const string Manager = "VNW0000800", First = "VNW0000801", Second = "VNW0000802", MissingProfile = "VNW0000803";
    private const int Function = 31800, Role = 31800, UnknownGroup = 700000;
    private static readonly dt312_ExamAnalysisBUS Bus = dt312_ExamAnalysisBUS.Instance;
    private static int examA, examB, edgeExam, groupA, groupB;

    internal static void Run()
    {
        Control.CheckForIllegalCrossThreadCalls = true;
        H.Check(Environment.GetEnvironmentVariable("EXAM312_TEST_CONNECTION") != null, "Analysis fixtures require disposable DB");
        H.Check(!Bus.CanAnalyze(Manager) && !Bus.CanAnalyze("VNW0000002"), "Unregistered analysis inherits exam/practice permission");
        Denied(() => Bus.GetStatistics(Manager, null, null));
        Denied(() => Bus.GetOptions(Manager, null));
        Seed();
        H.Check(Bus.CanAnalyze(" vnw0000800 "), "Analysis actor normalization failed");
        Denied(() => Bus.GetStatistics(First, null, null));
        string before;
        using (var db = new DBDocumentManagementSystemEntities()) before = JsonConvert.SerializeObject(db.dt312_ExamUser.OrderBy(x => x.Id).ToList());
        CheckNumbers(); CheckLegacyAndOptions(); CheckUi();
        using (var db = new DBDocumentManagementSystemEntities())
            H.Check(before == JsonConvert.SerializeObject(db.dt312_ExamUser.OrderBy(x => x.Id).ToList()), "Analysis mutated official JSON/score/status");
        Console.WriteLine("PASS: analysis 4 scopes; weighted counts; immutable snapshots; legacy/malformed JSON; dedicated permissions; standalone UC; synchronized filtered/sorted chart; many groups/long names; async filter changes; no official mutation.");
    }

    private static void Denied(Action action)
    {
        try { action(); } catch (UnauthorizedAccessException) { H.Check(true, "Denied"); return; }
        throw new Exception("Analysis allowed an unauthorized request");
    }

    private static JObject Q(int group, bool? correct, string key = "1", string answer = "1", bool essay = false)
    {
        var row = new JObject { ["Questions"] = new JObject { ["Id"] = 987654, ["GroupId"] = group, ["IsEssay"] = essay },
            ["CorrectAnswer"] = key, ["UserAnswer"] = answer };
        if (correct.HasValue) row["IsCorrect"] = correct.Value;
        return row;
    }

    private static dt312_ExamUser Paper(int exam, string user, IEnumerable<JObject> rows, bool submitted = true)
    {
        return new dt312_ExamUser { ExamId = exam, UserId = user, SubmitAt = submitted ? (DateTime?)new DateTime(2020, 1, 1) : null,
            Score = null, IsPass = null, ExamData = new JArray(rows).ToString(Formatting.None) };
    }

    private static IEnumerable<JObject> Questions(int group, int count, int wrong)
    {
        return Enumerable.Range(0, count).Select(i => Q(group, i >= wrong));
    }

    private static void Seed()
    {
        using (var db = new DBDocumentManagementSystemEntities())
        {
            db.dm_Function.Add(new dm_Function { Id = Function, IdParent = 0, DisplayName = "錯題分析", ControlName = "uc312_ExamAnalysis", Prioritize = 5 });
            db.dm_UserRole.Add(new dm_UserRole { IdRole = Role, IdUser = Manager });
            db.dm_FunctionRole.Add(new dm_FunctionRole { IdRole = Role, IdFunction = Function });
            db.dm_User.AddRange(new[] {
                new dm_User { Id = Manager, DisplayName = "分析管理", DateCreate = DateTime.Today },
                new dm_User { Id = First, DisplayName = "第一人員", DisplayNameVN = "Tên Việt", DateCreate = DateTime.Today },
                new dm_User { Id = Second, DisplayName = "", DisplayNameVN = "Người thứ hai", DateCreate = DateTime.Today }
            });
            var a = new dt312_ExamMgmt { DisplayName = "分析測試甲", CreateAt = new DateTime(2019, 1, 1), RoundNumber = 1 };
            var b = new dt312_ExamMgmt { DisplayName = "分析測試乙", CreateAt = new DateTime(2021, 1, 1), RoundNumber = 1 };
            var c = new dt312_ExamMgmt { DisplayName = "分析相容測試", CreateAt = DateTime.Today, RoundNumber = 1 };
            db.dt312_ExamMgmt.AddRange(new[] { a, b, c });
            var ga = new dt312_Groups { DisplayName = "設備安全", CreateAt = DateTime.Today, CreateBy = Manager, RemoveAt = DateTime.Today };
            var gb = new dt312_Groups { DisplayName = "設備安全操作流程與異常處理之專業知識群組（長名稱顯示測試）", CreateAt = DateTime.Today, CreateBy = Manager };
            db.dt312_Groups.AddRange(new[] { ga, gb }); db.SaveChanges();
            examA = a.Id; examB = b.Id; edgeExam = c.Id; groupA = ga.Id; groupB = gb.Id;
            db.dt312_ExamUser.AddRange(new[] {
                Paper(examA, First, Questions(groupA,5,2).Concat(Questions(groupB,1,1)).Concat(new[] { Q(groupA, false, essay:true) })),
                Paper(examA, Second, Questions(groupA,5,4).Concat(Questions(groupB,2,0))),
                Paper(examB, First, Questions(groupA,10,3)),
                Paper(examB, First, Questions(groupA,5,1)),
                Paper(examA, First, Questions(groupA,100,100),false)
            });
            var missingGroup = Q(groupA, false); ((JObject)missingGroup["Questions"]).Remove("GroupId");
            var badFlag = Q(groupA, true); badFlag["IsCorrect"] = "false";
            var old = Q(UnknownGroup, null, "1,2", " 2 , 1 "); ((JObject)old["Questions"]).Remove("IsEssay");
            var legacyRows = new[] { old, Q(UnknownGroup, null, "1,2", "1"), Q(UnknownGroup, null, "1", ""),
                Q(UnknownGroup, false), missingGroup, badFlag, Q(groupA,false,essay:true) };
            db.dt312_ExamUser.Add(Paper(edgeExam, MissingProfile, legacyRows));
            db.dt312_ExamUser.Add(new dt312_ExamUser { ExamId = edgeExam, UserId = MissingProfile, SubmitAt = DateTime.Today, ExamData = "invalid-json" });
            db.dt312_ExamUser.Add(new dt312_ExamUser { ExamId = edgeExam, UserId = MissingProfile, SubmitAt = DateTime.Today, ExamData = "" });
            db.dt312_ExamUser.Add(Paper(edgeExam, First, new[] { Q(groupA,false,essay:true) }));
            db.dt312_ExamUser.Add(Paper(examB, First, Enumerable.Range(1,20).Select(i => Q(UnknownGroup+i, i % 2 == 0))));
            db.SaveChanges();
        }
    }

    private static Exam312AnalysisRow Group(Exam312AnalysisResult result, int id) => result.Rows.Single(x => x.GroupId == id);

    private static void CheckNumbers()
    {
        var one = Bus.GetStatistics(Manager, examA, First);
        H.Check(Group(one,groupA).QuestionCount == 5 && Group(one,groupA).WrongCount == 2 && Group(one,groupA).ErrorRate == 40,
            "Single person/single exam includes essays or unsubmitted paper");
        H.Check(Group(one,groupB).ErrorRate == 100 && one.Rows[0].GroupId == groupB, "Rate sorting incorrect");
        var exam = Bus.GetStatistics(Manager, examA, "");
        H.Check(Group(exam,groupA).ErrorRate == 60 && Group(exam,groupA).PeopleCount == 2 && Group(exam,groupA).PaperCount == 2,
            "All people/single exam incorrect");
        var person = Bus.GetStatistics(Manager,null," vnw0000801 ");
        H.Check(Group(person,groupA).QuestionCount == 20 && Group(person,groupA).WrongCount == 6 && Group(person,groupA).ErrorRate == 30
            && Group(person,groupA).PaperCount == 3, "History takes latest, averages rates, or counts essays");
        var all = Bus.GetStatistics(Manager,null,null);
        H.Check(Group(all,groupA).QuestionCount == 25 && Group(all,groupA).WrongCount == 10 && Group(all,groupA).ErrorRate == 40
            && Group(all,groupA).PeopleCount == 2 && Group(all,groupA).PaperCount == 4, "All people/history incorrect");
        H.Check(Group(all,groupA).GroupName == "設備安全", "Removed group loses name/history");
        H.Check(Bus.GetStatistics(Manager,examA,MissingProfile).Rows.Count == 0, "Person filter leaks other candidate");
        H.Check(Bus.GetStatistics(Manager,edgeExam,First).Rows.Count == 0, "All-essay paper produces numeric error rate");
        using (var db = new DBDocumentManagementSystemEntities())
        {
            var function = db.dm_Function.Find(Function); function.ControlName = "Views.uc312_ExamAnalysis"; db.SaveChanges();
        }
        H.Check(Bus.CanAnalyze(Manager), "Qualified ControlName not supported");
    }

    private static void CheckLegacyAndOptions()
    {
        var result = Bus.GetStatistics(Manager,edgeExam,null);
        var row = Group(result,UnknownGroup);
        H.Check(row.QuestionCount == 4 && row.WrongCount == 3 && row.ErrorRate == 75, "Legacy set grading/blank/stored false incorrect");
        H.Check(result.SkippedPapers == 2 && result.SkippedQuestions == 2 && result.SubmittedPapers == 4,
            "Malformed/empty JSON or invalid questions not counted accurately");
        H.Check(row.GroupName.Contains("未知群組") && !result.Rows.Any(x => x.GroupId == groupA), "Essay/unknown group mishandled");
        var options = Bus.GetOptions(Manager,edgeExam);
        H.Check(options.People.Any(x => x.UserId == MissingProfile && x.Name == MissingProfile)
            && !options.People.Any(x => x.UserId == Second), "Missing profile removed or exam people not filtered");
        options = Bus.GetOptions(Manager,null);
        H.Check(options.People.Single(x => x.UserId == First).Name == "第一人員"
            && options.People.Single(x => x.UserId == Second).Name == "Người thứ hai", "Person name fallback incorrect");
    }

    private static void Finished(uc312_ExamAnalysis uc)
    {
        Exam312DbTests.Wait(() => !(bool)H.Get(uc,"loading"), "Analysis async load",15000);
    }

    private static void CheckUi()
    {
        TPConfigs.LoginUser = new dm_User { Id = Manager };
        var type = typeof(uc312_ExamAnalysis).Assembly.GetTypes().Single(t => t.Name == "uc312_ExamAnalysis");
        H.Check(typeof(XtraUserControl).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null,
            "UC is not discoverable by existing function picker/frame");
        using (var host = new XtraForm { Width = 1250, Height = 850 })
        using (var uc = (uc312_ExamAnalysis)Activator.CreateInstance(type))
        {
            uc.Dock = DockStyle.Fill; host.Controls.Add(uc); host.ShowInTaskbar = false; host.Show(); Finished(uc);
            var scope = (BarEditItem)H.Get(uc,"barScope"); var exam = (BarEditItem)H.Get(uc,"barExam"); var person = (BarEditItem)H.Get(uc,"barPerson");
            var grid = (GridView)H.Get(uc,"resultsView"); var chart = (ChartControl)H.Get(uc,"chart");
            H.Check(scope.Enabled && !exam.Enabled && chart.Series[0].Points.Count == grid.DataRowCount && grid.DataRowCount > 20,
                "Default/history chart is truncated or filters invalid");
            H.Check(!((XYDiagram)chart.Diagram).Rotated && !((XYDiagram)chart.Diagram).AxisX.Reverse
                && Convert.ToDouble(((XYDiagram)chart.Diagram).AxisY.VisualRange.MaxValue) == 100, "Chart orientation/rate range wrong");
            H.Check(!((XYDiagram)chart.Diagram).AxisX.VisualRange.Auto, "Many groups lack scrolling window");
            AssertChartMatches(grid,chart);
            H.Screenshot(host,"Exam312-Analysis-History.png");
            grid.ActiveFilterString = "[GroupId] = " + groupA; Application.DoEvents();
            H.Check(grid.DataRowCount == 1 && chart.Series[0].Points.Count == 1
                && chart.Series[0].Points[0].Values[0] == 40, "Quick filter leaves stale chart or percentage inflated");
            grid.ActiveFilterString = ""; grid.Columns["GroupName"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending; Application.DoEvents();
            AssertChartMatches(grid,chart);
            grid.ActiveFilterString = "[GroupId] = -100"; Application.DoEvents();
            H.Check(chart.Series[0].Points.Count == 0, "Empty grid filter leaves bars visible");
            grid.ActiveFilterString = "";
            scope.EditValue = "單次考試"; exam.EditValue = examA; Finished(uc);
            person.EditValue = Second; Finished(uc);
            H.Check(((Exam312AnalysisRow)grid.GetRow(grid.LocateByValue("GroupId",groupA))).ErrorRate == 80,
                "UI single person/single exam incorrect");
            // Rapidly replace scopes while requests are outstanding; latest selection must win.
            exam.EditValue = edgeExam; scope.EditValue = "全部歷史"; person.EditValue = First; Finished(uc);
            H.Check(Convert.ToString(person.EditValue) == First && ((Exam312AnalysisRow)grid.GetRow(grid.LocateByValue("GroupId",groupA))).ErrorRate == 30,
                "Old async request overwrote newer filters/data");
            scope.EditValue = "單次考試"; exam.EditValue = edgeExam; person.EditValue = Second; Finished(uc);
            H.Check(Convert.ToString(person.EditValue) == ""
                && !((IEnumerable<Exam312AnalysisPersonOption>)((RepositoryItemSearchLookUpEdit)H.Get(uc,"personEditor")).DataSource).Any(x => x.UserId == Second),
                "Invalid candidate survives exam change");
            H.Screenshot(host,"Exam312-Analysis.png");
            host.Size = new System.Drawing.Size(920,650); Application.DoEvents();
            H.Check(chart.Height > 100 && chart.Width > 600, "Chart does not resize with UC");
            H.Check(chart.PointToScreen(new System.Drawing.Point(chart.Width,chart.Height)).Y
                <= host.PointToScreen(new System.Drawing.Point(host.ClientSize.Width,host.ClientSize.Height)).Y + 1,
                "Chart extends outside host client area");
            H.Screenshot(host,"Exam312-Analysis-Small.png");
            using (var db = new DBDocumentManagementSystemEntities())
            { db.dm_FunctionRole.RemoveRange(db.dm_FunctionRole.Where(x => x.IdRole == Role)); db.SaveChanges(); }
            H.Call(uc,"Refresh_ItemClick",null,null); Finished(uc);
            H.Check(!scope.Enabled && chart.Series[0].Points.Count == 0,"Revoked analysis permission retains results");
            host.Close();
        }
        TPConfigs.LoginUser = new dm_User { Id = First };
        using (var host = new XtraForm())
        using (var uc = new uc312_ExamAnalysis())
        {
            host.Controls.Add(uc); uc.Dock = DockStyle.Fill; host.ShowInTaskbar = false; host.Show(); Finished(uc);
            H.Check(!((BarEditItem)H.Get(uc,"barScope")).Enabled && ((ChartControl)H.Get(uc,"chart")).Series[0].Points.Count == 0,
                "Unauthorized UI exposes results/filters");
            host.Close();
        }
    }

    private static void AssertChartMatches(GridView grid, ChartControl chart)
    {
        H.Check(chart.Series[0].Points.Count == grid.DataRowCount,"Chart row count differs");
        for (int i=0;i<grid.DataRowCount;i++)
        {
            var row=(Exam312AnalysisRow)grid.GetRow(grid.GetVisibleRowHandle(i)); var point=chart.Series[0].Points[i];
            H.Check(point.Argument==row.ChartLabel && Math.Abs(point.Values[0]-(double)row.ErrorRate)<0.000001
                && point.ToolTipHint.Contains(row.WrongCount+"/"+row.QuestionCount),"Chart order/value/tooltip differs from table");
        }
    }
}
