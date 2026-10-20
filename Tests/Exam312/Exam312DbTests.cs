using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval;
using Newtonsoft.Json;
using H = Exam312Tests;

internal static class Exam312DbTests
{
    private const string Manager = "VNW0000001", Learner = "VNW0000002", Outsider = "VNW0000003", SimilarRole = "VNW0000004";
    private static int essayId, multiId, activeGroup, removedGroup, assignedId;
    private static readonly List<string> Passed = new List<string>();
    private static void Pass(string value) { Passed.Add(value); Console.WriteLine("PASS: " + value); }
    private static void As(string id) { TPConfigs.LoginUser = new dm_User { Id = id, DisplayName = "測試人員", IdDepartment = "3120" }; }
    private static void Expect<T>(Action action, string message) where T : Exception
    {
        try { action(); }
        catch (TargetInvocationException ex) when (ex.InnerException is T) { H.Check(true, message); return; }
        catch (T) { H.Check(true, message); return; }
        throw new Exception("Expected " + typeof(T).Name + ": " + message);
    }
    internal static void Wait(Func<bool> condition, string message, int milliseconds = 8000)
    {
        var watch = Stopwatch.StartNew();
        while (!condition())
        {
            if (watch.ElapsedMilliseconds > milliseconds) throw new Exception("Timed out: " + message);
            Application.DoEvents(); Thread.Sleep(20);
        }
        Application.DoEvents();
    }
    private sealed class Dialogs : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr window,int message,IntPtr value,IntPtr data);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr window,int id);
        private static IEnumerable<Control> Descendants(Control parent)
        {
            foreach(Control control in parent.Controls)
            { yield return control; foreach(var child in Descendants(control)) yield return child; }
        }
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 150 };
        private readonly Stopwatch limit = Stopwatch.StartNew();
        internal int Reviews, Messages;
        internal Action<f312_ExamReview> InspectReview;
        internal DialogResult Answer = DialogResult.OK;
        internal Dialogs()
        {
            timer.Tick += (s, e) =>
            {
                if (limit.ElapsedMilliseconds > 30000)
                { Console.Error.WriteLine("Dialog watchdog expired: " + string.Join(",", Application.OpenForms.Cast<Form>().Select(f=>f.GetType().Name+":"+f.Text))); Environment.Exit(2); }
                foreach (var form in Application.OpenForms.Cast<Form>().ToArray())
                {
                    if (form is f312_ExamReview review)
                    {
                        var browser = (WebBrowser)H.Get(review, "browser");
                        if (browser.ReadyState != WebBrowserReadyState.Complete || browser.Document?.Body == null) continue;
                        timer.Stop();
                        // Native WebBrowser painting occurs after DOM readiness.
                        Application.DoEvents(); Thread.Sleep(200); Application.DoEvents();
                        InspectReview?.Invoke(review); Reviews++; review.Close(); timer.Start();
                    }
                    else if (form.GetType().Name.Contains("MessageBox"))
                    {
                        Messages++;
                        var managedButton=Descendants(form).FirstOrDefault(c=>Equals(c.GetType().GetProperty("DialogResult")?.GetValue(c,null),Answer));
                        if(managedButton!=null)
                        { managedButton.GetType().GetMethod("PerformClick",Type.EmptyTypes)?.Invoke(managedButton,null); continue; }
                        if(form.GetType().Name.Contains("NativeMessageBox"))
                        {
                            IntPtr buttonWindow=GetDlgItem(form.Handle,(int)Answer);
                            if(buttonWindow!=IntPtr.Zero) SendMessage(buttonWindow,0x00F5,IntPtr.Zero,IntPtr.Zero);
                            else { form.DialogResult=Answer; form.Close(); }
                            continue;
                        }
                        var button=Descendants(form).OfType<IButtonControl>().FirstOrDefault(b=>b.DialogResult==Answer);
                        if(button!=null) button.PerformClick();
                        else { form.DialogResult = Answer; form.Close(); }
                    }
                }
            };
            timer.Start();
        }
        public void Dispose() { timer.Stop(); timer.Dispose(); }
    }
    private static void ConnectToDisposableDatabase()
    {
        string value = Environment.GetEnvironmentVariable("EXAM312_TEST_CONNECTION");
        var entity = new EntityConnectionStringBuilder(value);
        var provider = new SqlConnectionStringBuilder(entity.ProviderConnectionString);
        H.Check(System.Text.RegularExpressions.Regex.IsMatch(provider.InitialCatalog, "^7system_Exam312_Test_[0-9a-f]{32}$"), "Integration connection is not a disposable test database");
        string ignored = SingleConnection.ConString; // Initialize holder, without opening the application connection.
        var holder = typeof(SingleConnection).GetField("_ConsString", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        typeof(SingleConnection).GetField("_String", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(holder, value);
    }
    private static void Seed()
    {
        using (var db = new DBDocumentManagementSystemEntities())
        {
            var legacy = db.dt312_Questions.Single();
            H.Check(!legacy.IsEssay && legacy.ReferenceAnswer == "既有參考資料", "Migration rerun changed existing questions/reference values");
            H.Check(db.dt312_Answers.Count() == 2, "Migration removed legacy answer options");
            db.dm_Function.AddRange(new[] {
                new dm_Function { Id=31201, IdParent=0, DisplayName="題庫管理", ControlName="uc312_GroupMgmt" },
                new dm_Function { Id=31202, IdParent=0, DisplayName="個人測驗", ControlName="uc312_ExamMain" },
                new dm_Function { Id=31203, IdParent=0, DisplayName="無關功能", ControlName="uc312_GroupMgmtBackup" }
            });
            db.dm_FunctionRole.AddRange(new[] {
                new dm_FunctionRole { IdFunction=31201, IdRole=11 }, new dm_FunctionRole { IdFunction=31202, IdRole=12 },
                new dm_FunctionRole { IdFunction=31203, IdRole=13 }
            });
            db.dm_UserRole.AddRange(new[] { new dm_UserRole { IdUser=Manager, IdRole=11 }, new dm_UserRole { IdUser=Learner, IdRole=12 }, new dm_UserRole { IdUser=SimilarRole, IdRole=13 } });
            foreach (string id in new[] { Manager, Learner, Outsider, SimilarRole })
                db.dm_User.Add(new dm_User { Id=id, DisplayName="測試人員", DateCreate=new DateTime(2026,10,20), IdDepartment="3120" });
            var group = new dt312_Groups { DisplayName="安全題組", CreateAt=new DateTime(2026,10,20), CreateBy=Manager };
            var removed = new dt312_Groups { DisplayName="停用題組", CreateAt=new DateTime(2026,10,20), CreateBy=Manager, RemoveAt=new DateTime(2026,10,20), RemoveBy=Manager };
            db.dt312_Groups.Add(group); db.dt312_Groups.Add(removed);
            db.dt312_Setting.Add(new dt312_Setting { id=1, CountEmp=10, PassingScore=60, QuesCount=3, TestDuration=20, CycleCreate=12 });
            db.SaveChanges(); activeGroup=group.Id; removedGroup=removed.Id;
            var multi = new dt312_Questions { GroupId=activeGroup, DisplayName="選出正確的安全措施（複選）", IsmultiAns=true };
            db.dt312_Questions.Add(multi);
            db.dt312_Questions.Add(new dt312_Questions { GroupId=removedGroup, DisplayName="停用題目不可抽到" });
            db.SaveChanges(); multiId=multi.Id;
            db.dt312_Answers.AddRange(new[] {
                new dt312_Answers { QuesId=multiId, DisplayName="確認設備狀態", TrueAns=true },
                new dt312_Answers { QuesId=multiId, DisplayName="忽略安全規範", TrueAns=false },
                new dt312_Answers { QuesId=multiId, DisplayName="依標準作業程序執行", TrueAns=true }
            });
            var cycle = new dt312_ExamMgmt { DisplayName="測試正式考試", CreateAt=new DateTime(2026,10,20), RoundNumber=1 };
            db.dt312_ExamMgmt.Add(cycle); db.SaveChanges();
            var assigned = new dt312_ExamUser { ExamId=cycle.Id, UserId=Learner };
            db.dt312_ExamUser.Add(assigned); db.SaveChanges(); assignedId=assigned.Id;
        }
        Pass("Migration twice: existing questions, answer options, and saved reference data preserved");
    }
    private static void PermissionsAndValidation()
    {
        var bus = dt312_QuestionsBUS.Instance;
        H.Check(bus.CanManageQuestions(Manager) && bus.CanPractice(Manager), "Manager cannot manage/practice");
        H.Check(bus.CanPractice(Learner) && !bus.CanManageQuestions(Learner), "Learner permission is incorrect");
        H.Check(!bus.CanPractice(Outsider) && !bus.CanManageQuestions(Outsider), "Outsider granted access");
        H.Check(!bus.CanManageQuestions(SimilarRole), "Similar control name incorrectly grants question management");
        H.Check(bus.CanManageQuestions(Manager.ToLowerInvariant()), "Account normalization failed");
        Expect<UnauthorizedAccessException>(() => bus.SaveEssay(Learner,1,0,"試題",""), "Learner can add essay");
        Expect<ArgumentException>(() => bus.SaveEssay(Manager,1,0,"  ",""), "Blank question accepted");
        Expect<ArgumentException>(() => bus.SaveEssay(Manager,1,0,new string('題',2049),""), "Overlong question accepted");
        Expect<ArgumentException>(() => bus.SaveEssay(Manager,1,0,"題目",new string('答',16001)), "Overlong reference accepted");
        Expect<InvalidOperationException>(() => bus.SaveEssay(Manager,removedGroup,0,"題目",""), "Removed group accepts essay");
        Expect<InvalidOperationException>(() => bus.SaveEssay(Manager,987654,0,"題目",""), "Unknown group accepts essay");
        Expect<InvalidOperationException>(() => bus.SaveEssay(Manager,1,1,"題目",""), "Legacy MC question converted into essay");
        int boundary = bus.SaveEssay(Manager,activeGroup,0,new string('題',2048),new string('答',16000));
        H.Check(bus.GetItemById(boundary).ReferenceAnswer.Length == 16000, "Boundary-length essay does not round trip");
        bus.RemoveById(boundary);
        Pass("Role grants/denials; similar-name isolation; account normalization; empty/oversize/boundary/group/type validation");
    }
    private static void EssayRoundTrip()
    {
        As(Manager);
        using (var form = new f312_EssayQuestion(Manager,1,null,true))
        {
            form.Show(); Application.DoEvents();
            ((MemoEdit)H.Get(form,"questionEdit")).Text="請說明設備啟動前的安全檢查步驟。";
            ((MemoEdit)H.Get(form,"referenceEdit")).Text="先確認電源及防護裝置。\r\n再依標準作業程序檢查。";
            H.Screenshot(form,"01-add-essay.png");
            H.Call(form,"Save_Click",null,null);
            H.Check(form.DialogResult==DialogResult.OK,"Essay form did not save successfully");
        }
        var saved = dt312_QuestionsBUS.Instance.GetListByIdGroup(1).Single(q=>q.IsEssay);
        essayId=saved.Id;
        H.Check(saved.ReferenceAnswer.Contains("標準作業") && !saved.IsmultiAns,"Essay reference/type not saved");
        dt312_QuestionsBUS.Instance.SaveEssay(Manager,1,essayId,"請說明啟動設備前的安全檢查步驟。","確認防護裝置完整，再依標準作業程序檢查。");
        saved=dt312_QuestionsBUS.Instance.GetItemById(essayId);
        using(var form=new f312_EssayQuestion(Manager,1,saved,true))
        { form.Show(); Application.DoEvents(); H.Screenshot(form,"02-edit-saved-essay.png"); form.Close(); }
        using(var form=new f312_EssayQuestion(Learner,1,saved,false))
        {
            H.Check(!((DevExpress.XtraBars.BarButtonItem)H.Get(form,"btnSave")).Enabled,"Viewer save is enabled");
            H.Check(((MemoEdit)H.Get(form,"questionEdit")).Properties.ReadOnly,"Viewer question is editable");
        }
        using(var db=new DBDocumentManagementSystemEntities()) H.Check(!db.dt312_Answers.Any(a=>a.QuesId==essayId),"Essay has MC answer options");
        Expect<InvalidOperationException>(()=>dt312_QuestionsBUS.Instance.SaveEssay(Manager,activeGroup,essayId,"錯誤題組",""),"Cross-group editing accepted");
        Pass("Actual add/edit dialog and EF round trip; reference text; read-only controls; no MC options; cross-group denial");
    }
    private static f312_DoExam Open(bool practice, int assigned = -1)
    {
        var form=new f312_DoExam(practice) { idExamUser=assigned, ShowInTaskbar=false };
        using(var dialogs=new Dialogs()) { form.Show(); Application.DoEvents(); }
        return form;
    }
    private static List<f312_DoExam.ExamResult> Results(f312_DoExam form) => (List<f312_DoExam.ExamResult>)H.Get(form,"examResults");
    private static void Fill(f312_DoExam form, bool oneWrong)
    {
        var results=Results(form); bool wrong=false;
        for(int i=0;i<results.Count;i++)
        {
            H.Set(form,"indexQues",i); H.Call(form,"InitializeWebView2",i); Application.DoEvents();
            if(results[i].Questions.IsEssay) ((MemoEdit)H.Get(form,"essayAnswer")).Text="先檢查安全防護，再依規範操作。";
            else
            {
                string selection=oneWrong&&!wrong?"2":results[i].CorrectAnswer; wrong=true;
                ((TokenEdit)H.Get(form,"txbUserAns")).EditValue=selection;
            }
            H.Call(form,"SaveCurrentAnswer");
        }
    }
    private static void Close(f312_DoExam form) { H.Set(form,"IsClose",true); form.Close(); }
    private static void PracticeAndTimeout()
    {
        As(Learner);
        string baseline;
        using(var db=new DBDocumentManagementSystemEntities()) baseline=JsonConvert.SerializeObject(db.dt312_ExamUser.OrderBy(r=>r.Id).ToList());
        using(var form=Open(true,assignedId))
        {
            var results=Results(form);
            H.Check(results.Count==3 && results.Any(r=>r.Questions.IsEssay),"Real practice load omitted essay or exact-size pool failed");
            H.Check(results.All(r=>r.Questions.GroupId!=removedGroup),"Practice selected a removed group");
            Fill(form,true);
            int essayIndex=results.FindIndex(r=>r.Questions.IsEssay);
            H.Set(form,"indexQues",essayIndex); H.Call(form,"InitializeWebView2",essayIndex); Application.DoEvents();
            var essayEditor=(MemoEdit)H.Get(form,"essayAnswer");
            var nextButton=(SimpleButton)H.Get(form,"btnNextQues");
            var questionBrowser=(WebBrowser)H.Get(form,"webViewQues");
            H.Check(essayEditor.Top>=questionBrowser.Bottom&&nextButton.Top>=essayEditor.Bottom,"Essay/question/navigation overlap");
            H.Screenshot(form,"03-practice-essay.png");
            int mc=results.FindIndex(r=>!r.Questions.IsEssay); H.Set(form,"indexQues",mc); H.Call(form,"InitializeWebView2",mc); Application.DoEvents();
            var choiceEditor=(TokenEdit)H.Get(form,"txbUserAns");
            H.Screenshot(form,"04-practice-choice.png");
            var choiceLayout=(DevExpress.XtraLayout.LayoutControlItem)H.Get(form,"layoutControlItem6");
            H.Check(choiceEditor.Width>500&&choiceEditor.Top>=questionBrowser.Bottom&&nextButton.Top>=choiceEditor.Bottom,"Choice answer area is narrow or overlaps navigation: "+choiceEditor.Bounds+" question "+questionBrowser.Bounds+" next "+nextButton.Bounds+" item "+choiceLayout.Size+" min "+choiceLayout.MinSize+" max "+choiceLayout.MaxSize+" col "+choiceLayout.OptionsTableLayoutItem.ColumnIndex+" alignment "+choiceLayout.ControlAlignment);
            using(var dialogs=new Dialogs())
            {
                dialogs.InspectReview=review=>
                {
                    var browser=(WebBrowser)H.Get(review,"browser"); string body=browser.Document.Body.InnerText;
                    H.Check(body.Contains("50／100")&&body.Contains("答錯")&&body.Contains("正確答案"),"Practice review score/mistakes missing");
                    H.Screenshot(review,"05-practice-score.png");
                    browser.Document.Window.ScrollTo(0,browser.Document.Body.ScrollRectangle.Height);
                    Application.DoEvents(); Thread.Sleep(150); Application.DoEvents(); H.Screenshot(review,"06-practice-answer-details.png");
                };
                H.Call(form,"Submit_Exam"); H.Check(dialogs.Reviews==1,"Practice did not open one review");
            }
        }
        using(var db=new DBDocumentManagementSystemEntities()) H.Check(JsonConvert.SerializeObject(db.dt312_ExamUser.OrderBy(r=>r.Id).ToList())==baseline,"Practice modified official exam rows");
        using(var form=Open(true))
        {
            Fill(form,false); H.Set(form,"timeRemaining",TimeSpan.FromSeconds(1));
            using(var dialogs=new Dialogs())
            { H.Call(form,"CountdownTimer_Tick",null,EventArgs.Empty); H.Check(dialogs.Reviews==1,"Timeout did not submit practice exactly once"); }
        }
        Pass("Actual practice load/start; active-group allocation; mixed inputs; 50/100 review; no official mutation; timeout autosubmit");
    }
    private static void EntryPointsAndNavigation()
    {
        As(Manager);
        using(var management=new f312_QuesMgmt(1))
        {
            management.Show(); Application.DoEvents();
            H.Check(((DevExpress.XtraBars.BarButtonItem)H.Get(management,"btnAddEssay")).Enabled,"Manager cannot add essay from question screen");
            H.Check(((DevExpress.XtraGrid.Views.Grid.GridView)H.Get(management,"gvQues")).DataRowCount==2,"Question screen omits saved essay/legacy question");
            H.Screenshot(management,"00-question-management.png"); management.Close();
        }
        As(Learner);
        using(var host=new Form { ClientSize=new Size(1250,800), ShowInTaskbar=false })
        using(var main=new uc312_ExamMain())
        {
            host.Controls.Add(main); main.Dock=DockStyle.Fill; host.Show(); Application.DoEvents();
            H.Check(((DevExpress.XtraBars.BarButtonItem)H.Get(main,"btnPractice")).Enabled,"Learner cannot see practice entry");
            H.Screenshot(host,"00-practice-entry.png"); host.Close();
        }
        using(var form=Open(true))
        {
            var results=Results(form); int essay=results.FindIndex(r=>r.Questions.IsEssay), other=results.FindIndex(r=>!r.Questions.IsEssay);
            H.Set(form,"indexQues",essay); H.Call(form,"InitializeWebView2",essay); Application.DoEvents();
            ((MemoEdit)H.Get(form,"essayAnswer")).Text="尚未切換前的作答";
            H.Call(form,"F312_DoExam_KeyDown",null,new KeyEventArgs(Keys.Right));
            H.Check((int)H.Get(form,"indexQues")==essay,"Arrow key inside essay navigated away");
            var view=(DevExpress.XtraGrid.Views.Grid.GridView)H.Get(form,"gvData");
            view.FocusedRowHandle=view.LocateByValue("QuestionIndex",other+1);
            H.Call(form,"gcData_DoubleClick",null,EventArgs.Empty);
            H.Check((int)H.Get(form,"indexQues")==other&&results[essay].UserAnswer.Contains("尚未"),"Direct row navigation loses essay or selects wrong question");
            ((TokenEdit)H.Get(form,"txbUserAns")).EditValue="2";
            H.Call(form,"SaveCurrentAnswer"); H.Check(results[other].UserAnswer=="2","Token selection not captured");
            using(var dialogs=new Dialogs { Answer=DialogResult.No }) H.Call(form,"btnSubmit_Click",null,EventArgs.Empty);
            H.Check(form.Visible&&!(bool)H.Get(form,"IsClose"),"Cancel submission closes exam");
            H.Check(((System.Windows.Forms.Timer)H.Get(form,"countdownTimer")).Enabled,"Cancel submission stopped countdown");
            Close(form);
        }
        Pass("Actual management/practice entry screens; keyboard/direct-row navigation; uncommitted essay; MC token value; cancel submit");
    }
    private static void SubmissionFailureAndLongClock()
    {
        As(Learner);
        int id;
        using(var db=new DBDocumentManagementSystemEntities())
        { var item=new dt312_ExamUser { ExamId=1,UserId=Learner }; db.dt312_ExamUser.Add(item); db.SaveChanges(); id=item.Id; }
        using(var form=Open(false,id))
        {
            Fill(form,false);
            // Invalidate the assignment after the exam opens: submission must fail without freezing the session.
            using(var db=new DBDocumentManagementSystemEntities()) { var item=db.dt312_ExamUser.Find(id); item.UserId=Outsider; db.SaveChanges(); }
            using(var dialogs=new Dialogs()) H.Call(form,"Submit_Exam");
            H.Check(form.Visible&&!(bool)H.Get(form,"IsClose"),"Failed submission closes exam");
            H.Check(((System.Windows.Forms.Timer)H.Get(form,"countdownTimer")).Enabled,"Failed submission leaves countdown paused");
            using(var db=new DBDocumentManagementSystemEntities()) H.Check(!db.dt312_ExamUser.Find(id).SubmitAt.HasValue,"Failed submission modified assignment");
            H.Set(form,"timeRemaining",TimeSpan.FromMinutes(120));
            H.Call(form,"CountdownTimer_Tick",null,EventArgs.Empty);
            string clock=((Label)H.Get(form,"lbTime")).Text;
            H.Check(clock.Contains("119:59")&&clock.StartsWith("剩餘時間"),"Long countdown wraps minutes or loses localized caption");
            Close(form);
        }
        Pass("Failed submission preserves answers/session and resumes clock; durations over one hour display correctly");
    }
    private static void Official()
    {
        As(Outsider);
        using(var form=new f312_DoExam(false) { idExamUser=assignedId })
            H.Check(((string)H.Call(form,"LoadData")).Length>0,"Other user can open assigned exam");
        As(Learner);
        using(var form=Open(false,assignedId))
        {
            Fill(form,false);
            using(var dialogs=new Dialogs()) H.Call(form,"Submit_Exam");
            H.Call(form,"Submit_Exam");
        }
        using(var db=new DBDocumentManagementSystemEntities())
        {
            var row=db.dt312_ExamUser.Single(r=>r.Id==assignedId);
            H.Check(row.Score==100&&row.IsPass==true&&row.SubmitAt.HasValue,"Official score/pass/submission not persisted");
            var results=JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>(row.ExamData);
            H.Check(results.Single(r=>r.Questions.IsEssay).UserAnswer.Contains("防護"),"Official essay text lost");
            using(var review=new f312_ExamReview(results,row.Score,false))
            { review.Show(); var browser=(WebBrowser)H.Get(review,"browser"); Wait(()=>browser.ReadyState==WebBrowserReadyState.Complete&&browser.Document?.Body!=null,"official review"); H.Screenshot(review,"07-official-result.png"); review.Close(); }
        }
        using(var form=new f312_DoExam(false) { idExamUser=assignedId }) H.Check(((string)H.Call(form,"LoadData")).Length>0,"Submitted exam can restart");
        Pass("Official ownership protection; real persistence of 100/pass/timestamp/essay snapshot; repeated submit/restart rejection");
    }
    private static void ConfigurationEdges()
    {
        As(Learner);
        using(var db=new DBDocumentManagementSystemEntities()) { db.dt312_Setting.Find(1).QuesCount=4; db.SaveChanges(); }
        using(var form=new f312_DoExam(true)) H.Check(((string)H.Call(form,"LoadData")).Contains("不夠"),"Insufficient active pool accepted");
        using(var db=new DBDocumentManagementSystemEntities()) { db.dt312_Setting.Find(1).QuesCount=0; db.SaveChanges(); }
        using(var form=new f312_DoExam(true)) H.Check(((string)H.Call(form,"LoadData")).Length>0,"Zero count accepted");
        using(var db=new DBDocumentManagementSystemEntities()) { var setting=db.dt312_Setting.Find(1); setting.QuesCount=3; setting.TestDuration=0; db.SaveChanges(); }
        using(var form=new f312_DoExam(true)) H.Check(((string)H.Call(form,"LoadData")).Length>0,"Zero duration accepted");
        using(var db=new DBDocumentManagementSystemEntities()) { var setting=db.dt312_Setting.Find(1); setting.TestDuration=20; setting.PassingScore=101; db.SaveChanges(); }
        using(var form=new f312_DoExam(true)) H.Check(((string)H.Call(form,"LoadData")).Length>0,"Invalid passing score accepted");
        using(var db=new DBDocumentManagementSystemEntities())
        { db.dt312_Setting.Find(1).PassingScore=60; foreach(var a in db.dt312_Answers.Where(a=>a.QuesId==multiId)) a.TrueAns=false; db.SaveChanges(); }
        using(var form=new f312_DoExam(true)) H.Check(((string)H.Call(form,"LoadData")).Contains("正確答案"),"Missing answer key accepted");
        using(var db=new DBDocumentManagementSystemEntities())
        { var first=db.dt312_Answers.Where(a=>a.QuesId==multiId).OrderBy(a=>a.Id).First(); first.TrueAns=true; db.SaveChanges(); }
        As(Outsider);
        using(var form=new f312_DoExam(true)) H.Check(((string)H.Call(form,"LoadData")).Contains("權限"),"Outsider can practice");
        Pass("Insufficient/zero settings, missing keys, and unauthorized practice are rejected");
    }
    private static void AllEssay()
    {
        using(var db=new DBDocumentManagementSystemEntities())
        { db.dt312_Setting.Find(1).QuesCount=1; foreach(var q in db.dt312_Questions.Where(q=>!q.IsEssay)) q.GroupId=removedGroup; db.SaveChanges(); }
        As(Learner);
        using(var form=Open(true))
        {
            H.Check(Results(form).Count==1&&Results(form)[0].Questions.IsEssay,"All-essay pool fails to load");
            Fill(form,false);
            using(var dialogs=new Dialogs())
            {
                dialogs.InspectReview=review=>
                { var browser=(WebBrowser)H.Get(review,"browser"); H.Check(browser.Document.Body.InnerText.Contains("全為申論題"),"All-essay review invents numeric grade"); H.Screenshot(review,"08-all-essay-result.png"); };
                H.Call(form,"Submit_Exam");
            }
        }
        int id;
        using(var db=new DBDocumentManagementSystemEntities())
        { var assigned=new dt312_ExamUser { ExamId=1,UserId=Learner }; db.dt312_ExamUser.Add(assigned); db.SaveChanges(); id=assigned.Id; }
        using(var form=Open(false,id)) { Fill(form,false); using(var dialogs=new Dialogs()) H.Call(form,"Submit_Exam"); }
        using(var db=new DBDocumentManagementSystemEntities())
        { var saved=db.dt312_ExamUser.Find(id); H.Check(saved.SubmitAt.HasValue&&!saved.Score.HasValue&&!saved.IsPass.HasValue,"All-essay official exam incorrectly graded/passed"); }
        Pass("All-essay practice and official submit: no numeric grade/pass, text persists");
    }
    internal static void Run()
    {
        ConnectToDisposableDatabase(); Seed(); PermissionsAndValidation(); EssayRoundTrip();
        EntryPointsAndNavigation(); PracticeAndTimeout(); Official(); SubmissionFailureAndLongClock(); ConfigurationEdges(); AllEssay();
        As(Manager);
        Directory.CreateDirectory(H.OutputDir);
        File.WriteAllLines(Path.Combine(H.OutputDir,"integration-results.txt"),Passed.Select(p=>"PASS: "+p));
    }
}
