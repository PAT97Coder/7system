using BusinessLayer;
using DataAccessLayer;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

internal static class WorkflowTests
{
    private const string A = "TEST7810", B = "TEST7820", R1 = "REVIEW7810", R2 = "REVIEW7820", Psm = "TESTPSM", Boss = "TESTBOSS", Other = "OUTSIDER";
    private static readonly Task318Participants People = new Task318Participants { User7810 = A, User7820 = B };
    private static readonly dt318_TaskWorkflowBUS Service = dt318_TaskWorkflowBUS.Instance;
    private static int assertions;

    private static void Assert(bool value, string name)
    {
        if (!value) throw new Exception("FAILED: " + name);
        assertions++; Console.WriteLine("PASS " + name);
    }

    private static void Reject(Action action, string name)
    {
        try { action(); }
        catch (InvalidOperationException) { Assert(true, name); return; }
        throw new Exception("FAILED (accepted invalid operation): " + name);
    }

    private static dt318_Tasks NewTask()
    {
        return new dt318_Tasks
        {
            ChuDe = "Workflow fixture", GiaiThich = "Test only", NgayDenHan = DateTime.Today.AddDays(5),
            TrangThai = "ChoXuLy", DuongDanThuMucChung = ConfigurationManager.AppSettings["Module318:CommonRoot"],
            DuongDanThuMucCongViec = "fixture", TrangThaiOCR = "HoanThanh", NgayTao = DateTime.Now,
            NguoiTao = A, MaNguon = Guid.NewGuid().ToString("N")
        };
    }

    private static int Import(string recipient)
    {
        int id = dt318_TasksBUS.Instance.Import(NewTask(), recipient, new List<Task318ImportAttachment>(), People);
        Assert(id > 0, "import creates case"); return id;
    }

    private static Task318TeamSnapshot Team(int id, string code, string actor = A)
        => Service.GetSnapshot(id, actor).Teams.Single(r => r.Team.MaTo == code);

    private static List<Task318StepDraft> Drafts(Task318TeamSnapshot team)
        => team.Steps.Select(r => new Task318StepDraft
        { Id = r.Id, TenBuoc = r.TenBuoc, VaiTro = r.VaiTro, IdNguoiXuLy = r.IdNguoiXuLy, IdNhomXuLy = r.IdNhomXuLy, TrangThai = r.TrangThai }).ToList();

    private static void Confirm(int id, string code, bool yes, string actor)
    {
        var team = Team(id, code, actor);
        Service.ConfirmTeam(id, code, yes, actor, team.Team.RowVersion);
    }

    private static void Save(int id, string code, List<Task318StepDraft> steps, string actor)
    {
        Service.SaveWorkflow(id, code, steps, actor, Team(id, code, actor).Team.RowVersion);
    }

    private static void Complete(int id, string code, string actor)
    {
        var team = Team(id, code, actor);
        var step = team.Steps.Single(r => r.TrangThai == "DangCho");
        Service.CompleteStep(id, code, step.Id, actor, team.Team.RowVersion, step.RowVersion, "Test confirmation");
    }

    private static List<Task318StepDraft> Route(string owner, bool full)
    {
        var result = new List<Task318StepDraft> { new Task318StepDraft { TenBuoc = "Work", VaiTro = "NguoiXuLy", IdNguoiXuLy = owner } };
        if (full) result.AddRange(new[]
        {
            new Task318StepDraft { TenBuoc = "Level 2", VaiTro = "Cap2", IdNhomXuLy = 1 },
            new Task318StepDraft { TenBuoc = "PSM", VaiTro = "PSM", IdNhomXuLy = 3 },
            new Task318StepDraft { TenBuoc = "Level 1", VaiTro = "Cap1", IdNguoiXuLy = Boss }
        });
        return result;
    }

    private static object Invoke(object target, string method, params object[] args)
    {
        try { return target.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, args); }
        catch (TargetInvocationException ex) when (ex.InnerException is InvalidOperationException) { throw ex.InnerException; }
    }

    private static DevExpress.XtraGrid.Views.Grid.GridView WorkflowView(f318_Workflow form)
    {
        var layout = form.Controls.OfType<DevExpress.XtraLayout.LayoutControl>().Single();
        var grid = layout.Controls.OfType<DevExpress.XtraGrid.GridControl>().Single();
        grid.ForceInitialize();
        return (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
    }

    private static T Field<T>(object target, string name)
        => (T)target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);

    private static f318_Settings SettingsForm(string file)
        => (f318_Settings)Activator.CreateInstance(typeof(f318_Settings), BindingFlags.Instance | BindingFlags.NonPublic,
            null, new object[] { file }, null);

    private static string ReadPath(string key, string file)
        => (string)typeof(f318_Settings).Assembly.GetType("KnowledgeSystem.Helpers.Task318WorkflowSettings")
            .GetMethod("Required").Invoke(null, new object[] { key, file });

    private static void CheckPathSettings(string directory)
    {
        string originalConfig = File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        string settingsFile = Path.Combine(directory, "Settings", "paths.config");
        string source7810 = Path.Combine(directory, "Download7810"), source7820 = Path.Combine(directory, "Download7820");
        string common = Path.Combine(directory, "Cases & Files");
        Directory.CreateDirectory(source7810); Directory.CreateDirectory(source7820);
        using (var form = SettingsForm(settingsFile))
        {
            Assert(Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbCommonRoot").Text == ConfigurationManager.AppSettings["Module318:CommonRoot"],
                "path dialog initially uses existing app configuration");
            var layout = form.Controls.OfType<DevExpress.XtraLayout.LayoutControl>().Single();
            Assert(layout.Controls.OfType<DevExpress.XtraEditors.ButtonEdit>().Count() == 3
                && layout.Controls.OfType<DevExpress.XtraEditors.ButtonEdit>().All(r => r.Properties.Buttons.Count == 1
                    && r.Properties.Buttons[0].Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
                && form.FormBorderStyle == FormBorderStyle.FixedToolWindow && form.Height < 250,
                "compact settings dialog has three editable paths with browse buttons");
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbSource7810").Text = source7810 + Path.DirectorySeparatorChar;
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbSource7820").Text = source7820;
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbCommonRoot").Text = common;
            form.Opacity = 0; form.ShowInTaskbar = false; form.Show(); Application.DoEvents();
            using (var bitmap = new System.Drawing.Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, form.Width, form.Height));
                bitmap.Save(Path.Combine(directory, "path-settings.png"));
            }
            Invoke(form, "SaveSettings");
            Assert(form.DialogResult == DialogResult.OK && File.Exists(settingsFile), "Confirm persists all path settings");
        }
        Assert(ReadPath("Module318:SourceTuan", settingsFile) == source7810 && ReadPath("Module318:SourceHung", settingsFile) == source7820
            && ReadPath("Module318:CommonRoot", settingsFile) == common,
            "runtime path reader uses saved overrides including escaped characters");
        string saved = File.ReadAllText(settingsFile);
        using (var form = SettingsForm(settingsFile))
        {
            Assert(Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbSource7810").Text == source7810
                && Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbCommonRoot").Text == common,
                "reopening settings restores saved paths");
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbSource7820").Text = source7810;
            Reject(() => Invoke(form, "SaveSettings"), "same source folder for both teams rejected");
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbSource7820").Text = source7820;
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbCommonRoot").Text = "relative-folder";
            Reject(() => Invoke(form, "SaveSettings"), "relative output folder rejected");
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbCommonRoot").Text = source7810;
            Reject(() => Invoke(form, "SaveSettings"), "output folder cannot be imported as a source");
        }
        Assert(File.ReadAllText(settingsFile) == saved, "invalid save and closing without Confirm preserve prior settings");
        using (var form = SettingsForm(settingsFile))
        {
            Field<DevExpress.XtraEditors.ButtonEdit>(form, "txbCommonRoot").Text = Path.Combine(directory, "NewCases");
            Invoke(form, "SaveSettings");
        }
        Assert(ReadPath("Module318:CommonRoot", settingsFile) == Path.Combine(directory, "NewCases")
            && File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) == originalConfig,
            "later saves replace local settings without editing executable configuration");
    }

    [STAThread]
    private static int Main()
    {
        try
        {
            using (var context = new DBDocumentManagementSystemEntities())
            {
                var sql = new SqlConnectionStringBuilder(context.Database.Connection.ConnectionString);
                if (sql.DataSource != @"(localdb)\Codex318WorkflowValidation" || sql.InitialCatalog != "Codex318WorkflowTests")
                    throw new Exception("Refusing to test against a non-test database.");
            }

            int single = Import(A), dual = Import(B);
            var a = Team(single, "7810"); var b = Team(single, "7820", B);
            Assert(a.Decision == true && b.Decision == null, "recipient Shi, other team unconfirmed");
            var dualTask = Service.GetSnapshot(dual, A).Task;
            Assert(dualTask.IdNguoiNhanAn == B && dualTask.NguoiTao == A && dualTask.MaToNhanAn == "7820", "receiver differs from creator");
            Reject(() => Service.ConfirmTeam(single, "7820", false, A, b.Team.RowVersion), "other team cannot confirm");
            Confirm(single, "7820", false, B);
            Reject(() => Service.ConfirmTeam(single, "7820", true, B, b.Team.RowVersion), "stale decision token rejected");
            Assert(Team(single, "7820", B).Decision == false, "stale update preserves Fou");
            int oldHistory = Service.GetSnapshot(single, A).History.Count;
            byte[] oldVersion = Team(single, "7820", B).Team.RowVersion;
            Service.PrepareExistingTasks(People, A);
            Service.PrepareExistingTasks(People, A);
            Assert(Service.GetSnapshot(single, A).History.Count == oldHistory && Team(single, "7820", B).Team.RowVersion.SequenceEqual(oldVersion), "refresh is idempotent");

            int legacy, ambiguous;
            using (var context = new DBDocumentManagementSystemEntities())
            {
                var task = NewTask(); task.CanXuLy7810 = false; context.dt318_Tasks.Add(task); context.SaveChanges(); legacy = task.Id;
                context.dt318_TaskUsers.Add(new dt318_TaskUsers { IdCongViec = legacy, IdNguoiDung = A, NguoiTao = B, NgayTao = DateTime.Now });
                task = NewTask(); context.dt318_Tasks.Add(task); context.SaveChanges(); ambiguous = task.Id;
                foreach (string user in new[] { A, B }) context.dt318_TaskUsers.Add(new dt318_TaskUsers { IdCongViec = ambiguous, IdNguoiDung = user, NguoiTao = A, NgayTao = DateTime.Now });
                context.SaveChanges();
            }
            Service.PrepareExistingTasks(People, A);
            Assert(Team(legacy, "7810").Decision == false, "backfill preserves legacy Fou");
            Assert(Service.GetSnapshot(legacy, A).Task.IdNguoiNhanAn == A, "legacy single recipient recovered");
            Assert(Service.GetSnapshot(ambiguous, A).Task.IdNguoiNhanAn == null, "ambiguous receiver never inferred from creator");

            Reject(() => Save(single, "7810", new List<Task318StepDraft> { new Task318StepDraft { TenBuoc = "No assignee", VaiTro = "NguoiXuLy" } }, A), "missing route assignee rejected");
            Assert(Team(single, "7810").Steps.Count == 0, "invalid save leaves no steps");
            Save(single, "7810", Route(A, true), A);
            a = Team(single, "7810");
            Reject(() => Service.SaveWorkflow(single, "7810", Drafts(a), B, a.Team.RowVersion), "other owner cannot edit route");
            var active = a.Steps.Single(r => r.TrangThai == "DangCho");
            Reject(() => Service.CompleteStep(single, "7810", active.Id, Other, a.Team.RowVersion, active.RowVersion, ""), "unassigned user cannot complete");
            Reject(() => Service.CompleteStep(single, "7810", a.Steps[1].Id, R1, a.Team.RowVersion, a.Steps[1].RowVersion, ""), "cannot skip steps");
            Complete(single, "7810", A);
            Reject(() => Service.CompleteStep(single, "7810", active.Id, A, a.Team.RowVersion, active.RowVersion, ""), "repeat completion rejected");
            a = Team(single, "7810", R1);
            Assert(a.CanComplete && a.Team.NguoiXuLyXong == A && a.Steps[1].TrangThai == "DangCho", "work completes and activates group level 2");
            var invalid = Drafts(a); invalid[0].TenBuoc = "Tamper completed work";
            Reject(() => Save(single, "7810", invalid, A), "completed prefix immutable");
            var edited = Drafts(a); var temp = edited[1]; edited[1] = edited[2]; edited[2] = temp;
            Save(single, "7810", edited, A);
            Assert(Team(single, "7810", Psm).CanComplete && !Team(single, "7810", R1).CanComplete, "pending route can be reordered");
            Reject(() => Service.SaveWorkflow(single, "7810", edited, A, a.Team.RowVersion), "stale route save rejected");
            Complete(single, "7810", Psm); Complete(single, "7810", R1); Complete(single, "7810", Boss);
            Assert(Service.GetSnapshot(single, A).Task.TrangThai == "HoanThanh", "single participating team closes case");
            Reject(() => Confirm(single, "7810", false, A), "cannot withdraw confirmed work");
            Reject(() => Save(single, "7810", Drafts(Team(single, "7810")), A), "finished route locked");

            Confirm(dual, "7810", true, A);
            Save(dual, "7810", Route(A, false), A); Save(dual, "7820", Route(B, false), B);
            Complete(dual, "7810", A);
            Assert(Team(dual, "7810").Team.TrangThaiXuLy == "HoanThanh" && Service.GetSnapshot(dual, A).Task.TrangThai != "HoanThanh", "two teams progress independently");
            Complete(dual, "7820", B);
            Assert(Service.GetSnapshot(dual, A).Task.TrangThai == "HoanThanh", "both participating teams must finish");

            int waiting = Import(A); Save(waiting, "7810", Route(A, false), A); Complete(waiting, "7810", A);
            Assert(Service.GetSnapshot(waiting, A).Task.TrangThai == "ChoXacNhanTo", "unconfirmed other team blocks case closure");
            Confirm(waiting, "7820", false, B);
            Assert(Service.GetSnapshot(waiting, A).Task.TrangThai == "HoanThanh", "other team Fou releases aggregate closure");

            int paused = Import(A); Save(paused, "7810", Route(A, true), A); Confirm(paused, "7810", false, A);
            Assert(Team(paused, "7810").Steps.All(r => r.TrangThai != "DangCho"), "Fou pauses unconfirmed route");
            Confirm(paused, "7810", true, A); Save(paused, "7810", Drafts(Team(paused, "7810")), A);
            Assert(Team(paused, "7810").Steps.Count(r => r.TrangThai == "DangCho") == 1, "resume creates one active step");

            int concurrent = Import(A); byte[] token = Team(concurrent, "7820", B).Team.RowVersion;
            int won = 0, rejected = 0; var start = new ManualResetEventSlim(false);
            var jobs = new[] { true, false }.Select(value => Task.Run(() =>
            {
                start.Wait();
                try { Service.ConfirmTeam(concurrent, "7820", value, B, token); Interlocked.Increment(ref won); }
                catch (InvalidOperationException) { Interlocked.Increment(ref rejected); }
            })).ToArray();
            start.Set(); Task.WaitAll(jobs);
            Assert(won == 1 && rejected == 1, "concurrent confirmations have one winner");
            Assert(Service.GetSnapshot(concurrent, A).History.Count(r => r.LoaiSuKien == "XacNhanTo") == 1, "concurrent operation logs once");

            string sharedKey = Guid.NewGuid().ToString("N");
            var importStart = new ManualResetEventSlim(false);
            var competingImports = new[] { 1, 2 }.Select(_ => Task.Run(() =>
            {
                var task = NewTask(); task.MaNguon = sharedKey;
                importStart.Wait();
                return dt318_TasksBUS.Instance.Import(task, A, new List<Task318ImportAttachment>(), People);
            })).ToArray();
            importStart.Set(); Task.WaitAll(competingImports);
            Assert(competingImports.Count(r => r.Result > 0) == 1 && competingImports.Count(r => r.Result == 0) == 1,
                "concurrent source import produces one case and one skip");
            using (var context = new DBDocumentManagementSystemEntities())
                Assert(context.dt318_Tasks.Count(r => r.MaNguon == sharedKey) == 1, "source and team initialization stay atomic");

            string directory = ConfigurationManager.AppSettings["TestFiles"];
            Directory.CreateDirectory(directory); string document = Path.Combine(directory, "source.xps"), attachment = Path.Combine(directory, "attachment.txt");
            File.WriteAllText(document, "Fixture: parser is tested separately"); File.WriteAllText(attachment, "Attachment");
            var manual = NewTask(); int manualId = new Task318FolderImportService().ImportSelectedFiles(manual, document, new[] { attachment }, A, People);
            Assert(manualId > 0 && File.Exists(Path.Combine(manual.DuongDanThuMucCongViec, "source.xps")), "manual import copies files and creates workflow data");
            using (var context = new DBDocumentManagementSystemEntities()) Assert(context.dt318_TaskAttach.Count(r => r.IdCongViec == manualId) == 2, "manual attachments persisted");
            Assert(new Task318FolderImportService().ImportSelectedFiles(NewTask(), document, new[] { attachment }, A, People) == 0, "manual source deduplication");
            string badDocument = Path.Combine(directory, "rollback.xps"); File.WriteAllText(badDocument, "Failure fixture");
            string common = ConfigurationManager.AppSettings["Module318:CommonRoot"];
            int foldersBefore = Directory.GetDirectories(common).Length;
            int tasksBefore; using (var context = new DBDocumentManagementSystemEntities()) tasksBefore = context.dt318_Tasks.Count();
            var missing = new Task318Participants { User7810 = A, User7820 = "MISSING" };
            Assert(new Task318FolderImportService().ImportSelectedFiles(NewTask(), badDocument, new string[0], A, missing) == -1, "bad configuration import fails");
            using (var context = new DBDocumentManagementSystemEntities()) Assert(context.dt318_Tasks.Count() == tasksBefore, "failed import rolls back database");
            Assert(Directory.GetDirectories(common).Length == foldersBefore, "failed import cleans only its reserved folder");

            var list = dt318_TasksBUS.Instance.GetListRows();
            Assert(list.Single(r => r.Id == single).Fou7820 && list.Single(r => r.Id == ambiguous).Assignee == "待確認收件人", "list exposes decisions and honest receiver label");
            using (var context = new DBDocumentManagementSystemEntities())
            {
                Assert(context.sys_NotesMail.Count() == 0, "Notes queue remains untouched");
                TPConfigs.LoginUser = context.dm_User.Single(r => r.Id == A);
            }
            Application.EnableVisualStyles();
            using (var main = new uc318_SharedTaskManagement())
            {
                var view = (DevExpress.XtraGrid.Views.BandedGrid.BandedGridView)typeof(uc318_SharedTaskManagement)
                    .GetField("gvData", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(main);
                Assert(view.Bands.Count == 3 && view.Columns["Shi7810"] != null && view.Columns["Fou7820"] != null, "two team bands expose Shi/Fou controls");
                var toolbar = Field<DevExpress.XtraBars.Bar>(main, "bar2");
                Field<DevExpress.XtraBars.BarManager>(main, "barManagerTP").ForceInitialize();
                Assert(toolbar.ItemLinks.Cast<DevExpress.XtraBars.BarItemLink>().Select(r => r.Item.Caption).SequenceEqual(new[] { "新增", "刷新", "設定" })
                    && Field<DevExpress.XtraBars.BarButtonItem>(main, "btnSetting").ImageOptions.SvgImage != null,
                    "Settings gear button follows Refresh on the toolbar");
            }
            using (var form = new f318_Workflow(paused, "7810"))
            {
                Assert(!form.Controls.OfType<DevExpress.XtraTab.XtraTabControl>().Any()
                    && !form.Controls.OfType<DevExpress.XtraLayout.LayoutControl>().Single().Root.Items.OfType<DevExpress.XtraLayout.TabbedControlGroup>().Any(),
                    "workflow form has one page without tabs");
                var groupRows = (BindingList<f318_Workflow.WorkflowRow>)WorkflowView(form).GridControl.DataSource;
                Assert(groupRows.Count(r => r.Draft.IdNhomXuLy.HasValue) == 2 && groupRows[1].JobName == "群組", "legacy group routes remain readable and assigned");
            }

            int simple = Import(A);
            using (var form = new f318_Workflow(simple, "7810"))
            {
                // Initialize real DevExpress controls, not just the form constructor.
                // Show with zero opacity so DevExpress performs its normal layout without a visible window.
                form.Opacity = 0; form.ShowInTaskbar = false; form.Show(); Application.DoEvents();
                var view = WorkflowView(form);
                Assert(view.Columns.Count == 5 && view.VisibleColumns.Select(c => c.Caption).SequenceEqual(new[] { "職務", "工號", "名稱", "權限", " " }),
                    "single grid has only the five signature-style columns after initialization");
                Assert(Field<DevExpress.XtraEditors.TextEdit>(form, "txbTeam").Text == "7810"
                    && Field<DevExpress.XtraEditors.TextEdit>(form, "txbTitle").Text == "Workflow fixture", "selected team and case shown above the grid");
                using (var sign = new KnowledgeSystem.Views._03_DepartmentManage._06_Signature.f306_NewSignDoc())
                {
                    var signView = Field<DevExpress.XtraGrid.Views.Grid.GridView>(sign, "gvProgress");
                    Assert(form.ClientSize == sign.ClientSize && form.FormBorderStyle == sign.FormBorderStyle
                        && view.Appearance.HeaderPanel.Font.Equals(signView.Appearance.HeaderPanel.Font)
                        && view.Appearance.Row.Font.Equals(signView.Appearance.Row.Font)
                        && view.Appearance.HeaderPanel.TextOptions.HAlignment == signView.Appearance.HeaderPanel.TextOptions.HAlignment,
                        "form size, border, grid fonts and header alignment match signature form");
                    Assert(Field<DevExpress.XtraBars.BarButtonItem>(form, "btnConfirm").ImageOptions.SvgImageSize
                        == Field<DevExpress.XtraBars.BarButtonItem>(sign, "btnConfirm").ImageOptions.SvgImageSize
                        && Field<DevExpress.XtraEditors.SimpleButton>(form, "btnDefaulProgress").Appearance.Font
                        .Equals(Field<DevExpress.XtraEditors.SimpleButton>(sign, "btnDefaulProgress").Appearance.Font),
                        "Confirm toolbar icon and fixed-flow button use signature styling");
                }
                var rows = (BindingList<f318_Workflow.WorkflowRow>)view.GridControl.DataSource;
                var lookup = (DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit)view.Columns["IdUser"].ColumnEdit;
                Assert(lookup.View.Columns.Count == 3 && lookup.View.Columns["Department"].Caption == "單位" && lookup.DisplayMember == "Id",
                    "employee lookup contains only department, employee ID and name");
                Assert(rows.Count == 1 && rows[0].IdUser == A && rows[0].JobName == "工程師", "initial route contains only owner with job auto-filled");
                view.AddNewRow();
                view.SetRowCellValue(DevExpress.XtraGrid.GridControl.NewItemRowHandle, "IdUser", R1);
                view.UpdateCurrentRow();
                Assert(rows.Count == 2 && rows[1].IdUser == R1 && rows[1].UserName == "REVIEW78" && rows[1].JobName == "工程師", "new-item row adds employee and fills name and job");
                Reject(() => Invoke(form, "ApplyFixedProgress", "7820", B), "fixed flow cannot edit other team");
                Reject(() => Invoke(form, "ApplyFixedProgress", "7810", A + ";MISSING"), "invalid fixed flow rejected");
                Assert(rows.Count == 2 && rows[1].IdUser == R1, "invalid template preserves unsaved employees");
                Invoke(form, "ApplyFixedProgress", "7810", A + "|1;" + R1 + "|2;" + Psm + "|4;" + Boss + "|1");
                Assert(rows.Select(r => r.IdUser).SequenceEqual(new[] { A, R1, Psm, Boss }) && rows.All(r => r.Permission == "確認"),
                    "shared fixed template supplies ordered employees with confirmation permission");
                using (var bitmap = new System.Drawing.Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, form.Width, form.Height));
                    bitmap.Save(Path.Combine(directory, "simple-workflow.png"));
                }
                Invoke(form, "SaveSelectedWorkflow");
            }
            using (var form = new f318_Workflow(simple, "7820"))
            {
                Assert(Field<DevExpress.XtraEditors.TextEdit>(form, "txbTeam").Text == "7820"
                    && WorkflowView(form).DataRowCount == 0 && !Field<DevExpress.XtraBars.BarButtonItem>(form, "btnConfirm").Enabled,
                    "opening other team never displays or edits first team's route");
            }
            using (var form = new f318_Workflow(simple, null))
                Assert(Field<DevExpress.XtraEditors.TextEdit>(form, "txbTeam").Text == "7810", "generic open selects actor's current or owned team");
            var simpleSteps = Team(simple, "7810").Steps;
            Assert(simpleSteps.Count == 4 && simpleSteps[0].VaiTro == "NguoiXuLy" && simpleSteps.Skip(1).All(r => r.VaiTro == "XacNhan"),
                "simple Confirm saves valid ordered workflow without technical role editing");
            Complete(simple, "7810", A);
            var completedStep = Team(simple, "7810").Steps[0];
            using (var form = new f318_Workflow(simple, "7810"))
            {
                var view = WorkflowView(form);
                var rows = (BindingList<f318_Workflow.WorkflowRow>)view.GridControl.DataSource;
                Reject(() => Invoke(form, "ApplyFixedProgress", "7810", R1 + ";" + Boss), "fixed template cannot replace completed prefix");
                Assert(rows.Count == 4 && rows[0].Draft.Id == completedStep.Id, "rejected replacement leaves completed and pending rows intact");
                Invoke(form, "ApplyFixedProgress", "7810", A + ";" + Psm + ";" + Boss);
                Invoke(form, "SaveSelectedWorkflow");
            }
            simpleSteps = Team(simple, "7810").Steps;
            Assert(simpleSteps.Count == 3 && simpleSteps[0].Id == completedStep.Id && simpleSteps[0].NgayXacNhan == completedStep.NgayXacNhan
                && simpleSteps[1].IdNguoiXuLy == Psm && Team(simple, "7810", Psm).CanComplete,
                "matching fixed template keeps completed result and replaces pending tail");
            using (var context = new DBDocumentManagementSystemEntities()) TPConfigs.LoginUser = context.dm_User.Single(r => r.Id == B);
            using (var form = new f318_Workflow(simple, null))
                Assert(Field<DevExpress.XtraEditors.TextEdit>(form, "txbTeam").Text == "7820", "generic open selects 7820 for its owner");
            using (var context = new DBDocumentManagementSystemEntities()) TPConfigs.LoginUser = context.dm_User.Single(r => r.Id == Psm);
            using (var form = new f318_Workflow(simple, null))
            {
                Assert(Field<DevExpress.XtraEditors.TextEdit>(form, "txbTeam").Text == "7810"
                    && !Field<DevExpress.XtraBars.BarButtonItem>(form, "btnConfirm").Enabled,
                    "reviewer opens current team without route editing permission");
                Invoke(form, "CompleteCurrentStep");
                Assert(Team(simple, "7810", Boss).CanComplete
                    && Field<DevExpress.XtraLayout.LayoutControlItem>(form, "lcComplete").Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never,
                    "single-page confirmation advances route and refreshes reviewer controls");
            }
            using (var context = new DBDocumentManagementSystemEntities())
                Assert(context.sys_NotesMail.Count() == 0, "simple workflow changes leave Notes queue untouched");
            CheckPathSettings(directory);
            Console.WriteLine("ALL PASSED: " + assertions + " assertions"); return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
