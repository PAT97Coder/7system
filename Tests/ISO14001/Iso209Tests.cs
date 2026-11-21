using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._09_ISO14001Docs;

internal static class Iso209Tests
{
    private const string UserId = "VNW0000001";
    private static int checks;
    private static readonly string ArtifactFolder = Environment.GetEnvironmentVariable("ISO209_TEST_ARTIFACTS");

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        checks++;
        Console.WriteLine("PASS: " + message);
    }

    [STAThread]
    private static int Main()
    {
        try
        {
            string sqlConnection = Environment.GetEnvironmentVariable("ISO209_TEST_CONNECTION");
            var sql = new System.Data.SqlClient.SqlConnectionStringBuilder(sqlConnection);
            if (!sql.InitialCatalog.StartsWith("7system_Iso209_Test_", StringComparison.Ordinal))
                throw new Exception("Refusing to run against a non-test database.");
            var entity = new EntityConnectionStringBuilder
            {
                Metadata = "res://DataAccessLayer/ModelDocManager.csdl|res://DataAccessLayer/ModelDocManager.ssdl|res://DataAccessLayer/ModelDocManager.msl",
                Provider = "System.Data.SqlClient", ProviderConnectionString = sqlConnection
            };
            // Redirect the existing BUS singleton connection in this test process only.
            object singleton = Activator.CreateInstance(typeof(SingleConnection), true);
            typeof(SingleConnection).GetField("_String", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(singleton, entity.ConnectionString);
            typeof(SingleConnection).GetField("_ConsString", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, singleton);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TPConfigs.LoginUser = dm_UserBUS.Instance.GetItemById(UserId);
            TPConfigs.Folder209 = Path.Combine(ArtifactFolder, "209");
            TPConfigs.Folder201 = Path.Combine(ArtifactFolder, "201");
            TPConfigs.DocTypes209 = "一階;二階;三階";
            TPConfigs.DocTypes201 = TPConfigs.DocTypes209;
            TPConfigs.SoftNameTW = "ISO14001測試";
            Directory.CreateDirectory(TPConfigs.Folder209);

            Check(dt209_RoleBUS.Instance.GetList().Count == 3, "independent signing roles seeded once");
            using (var db = new DBDocumentManagementSystemEntities())
            {
                Check(db.dm_Function.Count(f => f.ControlName.StartsWith("uc209_") || f.ControlName.StartsWith("mnu209_")) == 6, "all six navigation entries are idempotent");
                Check(!db.dm_Function.Any(f => f.ControlName == "uc209_UpdateUsrReq"), "unused personnel menu removed");
                Check(typeof(DBDocumentManagementSystemEntities).GetProperty("dt209_UpdateUsrReq") == null && typeof(DBDocumentManagementSystemEntities).GetProperty("dt209_ReqUpdateDocs") == null, "personnel entities removed from EF6 context");
                Check(db.dm_Function.Single(f => f.ControlName == "mnu209_ISO14001Docs").IdParent == 73, "ISO14001 remains inside the ISO menu");
                Check(db.dm_FunctionRole.Count() == 0 && db.dm_GroupUser.Count() == 0, "installation grants no roles or memberships");
                Check(db.dt209_RecordCode.Count() == 0, "ISO17025-specific reference catalogues not silently adopted");
                Check(db.dt201_Base.Single().DocCode == "SOURCE-201", "source module retained");
                var sourceGroup = db.dm_Group.Single(g => g.DisplayName == "ISO組");
                db.dm_GroupUser.Add(new dm_GroupUser { IdGroup = sourceGroup.Id, IdUser = UserId });
                db.SaveChanges();
            }
            Check(!Permission("CanEditCurrentUser"), "201 membership does not grant 209 editing");
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var viewGroup = db.dm_Group.Single(g => g.DisplayName == "ISO14001組");
                var editGroup = db.dm_Group.Single(g => g.DisplayName == "ISO14001文件【僅編輯】");
                db.dm_GroupUser.Add(new dm_GroupUser { IdGroup = viewGroup.Id, IdUser = UserId });
                db.dm_GroupUser.Add(new dm_GroupUser { IdGroup = editGroup.Id, IdUser = UserId });
                db.SaveChanges();
            }
            Check(Permission("CanEditCurrentUser") && !Permission("CanDeleteCurrentUser"), "209 edit-only group separates edit and delete");

            Check(dt209_RecordCodeBUS.Instance.Add(new dt209_RecordCode { Id = 90, Code = "ENV-TEST", DisplayName = "Ho so moi truong\n測試環境文件", Articles = "TEST" }), "EF6 creates ISO14001 reference code");
            var parent = new dt209_Base { IdParent = -1, DocCode = "ENV-TEST", DisplayName = "環境管理文件", DisplayNameVN = "Ho so moi truong", IdDept = "7820", IsDel = false, IsFinalNode = false, IsDisable = false, IsPaperType = false, IdRecordCode = 90, DocType = "一階" };
            int parentId = dt209_BaseBUS.Instance.Add(parent);
            Check(parentId > 0, "EF6 creates independent document tree");
            var child = new dt209_Base { IdParent = parentId, DocCode = "ENV-TEST", DisplayName = "2026", DisplayNameVN = "2026", IdDept = "7820", IsDel = false, IsFinalNode = true, IsDisable = false, IsPaperType = false, IdRecordCode = 90 };
            int childId = dt209_BaseBUS.Instance.Add(child);
            Check(childId > 0 && dt209_BaseBUS.Instance.GetAllChildByParentId(parentId).Count == 2, "document hierarchy loaded through LINQ");
            parent.DocCode = "ENV-REVISED";
            Check(dt209_BaseBUS.Instance.AddOrUpdate(parent), "EF6 updates parent document");
            Check(dt209_BaseBUS.Instance.GetItemById(childId).DocCode == "ENV-REVISED", "copied trigger propagates document code to final node");

            var form = new dt209_Forms { IdBase = childId, DisplayName = "環境管理附件", Code = "ENV-REVISED", UploadUser = UserId, UploadTime = DateTime.Now, IsProcessing = true, IsDel = false, NextStepProg = UserId };
            int formId = dt209_FormsBUS.Instance.Add(form);
            Check(formId > 0 && dt209_FormsBUS.Instance.GetListProcessing().Any(f => f.Id == formId), "EF6 attachment and pending workflow queries");
            Check(dt209_ProgressBUS.Instance.Add(new dt209_Progress { IdForm = formId, IdRole = 1, IdUsr = UserId }) > 0, "EF6 stores independent signing step");
            Check(dt209_ProgInfoBUS.Instance.Add(new dt209_ProgInfo { IdForm = formId, IdUsr = UserId, Desc = "測試核簽", RespTime = DateTime.Now }) > 0, "EF6 stores signing history");
            Check(dt209_ProgressBUS.Instance.GetListByIdForm(formId).Count == 1 && dt209_ProgInfoBUS.Instance.GetListByIdForm(formId).Count == 1, "EF6 reads signing steps and history");

            TestConstructors();
            Check(dt209_RecordCodeBUS.Instance.Add(new dt209_RecordCode { Id = 91, Code = "SINGLE", DisplayName = "單行名稱", Articles = "TEST" }), "create one-line catalogue regression fixture");
            Check(dt209_RecordCodeBUS.Instance.Add(new dt209_RecordCode { Id = 92, Code = "EMPTY", DisplayName = "", Articles = "TEST" }), "create empty-name catalogue fixture");
            Check(dt209_RecordCodeBUS.Instance.Add(new dt209_RecordCode { Id = 93, Code = "VN-ONLY", DisplayName = "Ten Viet\r\n", Articles = "TEST" }), "create catalogue fixture without Chinese name");
            using (var dialog = new f209_AddNode())
            {
                InvokeLoad(dialog, "f209_AddNode_Load");
                var editor = (DevExpress.XtraEditors.GridLookUpEdit)GetField(dialog, "txbIdRecord");
                var rows = ((System.Collections.IEnumerable)editor.Properties.DataSource).Cast<object>().ToList();
                Check(NameValue(rows, "SINGLE", "DisplayNameTW") == "單行名稱" && NameValue(rows, "SINGLE", "DisplayNameVN") == "", "single-line catalogue name loads without an index error");
                Check(NameValue(rows, "EMPTY", "DisplayNameTW") == "EMPTY", "empty catalogue name falls back to document code");
                Check(NameValue(rows, "VN-ONLY", "DisplayNameTW") == "Ten Viet", "missing Chinese catalogue name falls back to Vietnamese");
                Check(NameValue(rows, "ENV-TEST", "DisplayNameTW") == "測試環境文件" && NameValue(rows, "ENV-TEST", "DisplayNameVN") == "Ho so moi truong", "bilingual catalogue retains both names");
            }
            foreach (int codeId in new[] { 91, 92, 93 }) Check(dt209_RecordCodeBUS.Instance.RemoveById(codeId), "remove catalogue regression fixture " + codeId);
            TestSigningWorkflow(childId);
            TestLargeRecordId();
            CaptureMain();
            using (var dialog = new f209_AddNode())
            {
                dialog.parentData = parent;
                dialog.formName = "ISO14001文件";
                Capture(dialog, "209-AddNode.png");
            }
            using (var dialog = new f209_AddAttachment())
            {
                dialog.parentData = parent;
                dialog.currentData = child;
                dialog.formName = "ISO14001附件";
                Capture(dialog, "209-AddAttachment.png");
            }
            using (var dialog = new f209_AddRangeAtts())
            {
                Capture(dialog, "209-AddRangeAtts.png");
            }

            bool removed = dt209_FormsBUS.Instance.Remove(formId);
            var deleted = dt209_FormsBUS.Instance.GetItemById(formId);
            Check(removed && deleted.IsDel == true && deleted.DelTime.HasValue, "soft delete returns success and persists deletion time");
            Check(!dt209_FormsBUS.Instance.GetListProcessing().Any(f => f.Id == formId), "deleted attachment is excluded from pending list");
            Check(!dt209_FormsBUS.Instance.Remove(formId) && dt209_FormsBUS.Instance.GetItemById(formId).DelTime == deleted.DelTime, "repeated deletion makes no further changes");
            Check(!dt209_FormsBUS.Instance.Remove(int.MaxValue), "deleting a missing attachment returns false");
            using (var db = new DBDocumentManagementSystemEntities())
            {
                Check(db.dt201_Base.Single().DocCode == "SOURCE-201" && db.dt201_Forms.Count() == 0 && db.dt201_UpdateUsrReq.Count() == 0, "209 changes leave source 201 data untouched");
            }
            Console.WriteLine("Completed " + checks + " checks.");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e.ToString()); return 1; }
    }

    private static bool Permission(string method)
    {
        var type = typeof(TPConfigs).Assembly.GetType("KnowledgeSystem.Helpers.Iso209Permission", true);
        return (bool)type.GetMethod(method, BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
    }

    private static object GetField(object target, string name)
    {
        return target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    }

    private static void InvokeLoad(object target, string name)
    {
        target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, new object[] { target, EventArgs.Empty });
    }

    private static string NameValue(List<object> rows, string code, string property)
    {
        var row = rows.Single(r => (string)r.GetType().GetProperty("Code").GetValue(r, null) == code);
        return (string)row.GetType().GetProperty(property).GetValue(row, null);
    }

    private static void CheckSigningState(int formId, bool allowed, string expectedStatus)
    {
        using (var dialog = new f209_SignDoc_Info { idBase = formId })
        {
            InvokeLoad(dialog, "f209_SignDoc_Info_Load");
            Check((bool)GetField(dialog, "canProcessWorkflow") == allowed &&
                (allowed || dialog.Text.Contains(expectedStatus)), "signing state: " + expectedStatus);
            foreach (string name in new[] { "btnApproval", "btnConfirm", "btnCancel" })
            {
                var button = GetField(dialog, name);
                if (!allowed)
                    Check(!(bool)button.GetType().GetProperty("Enabled").GetValue(button, null) &&
                        button.GetType().GetProperty("Visibility").GetValue(button, null).ToString() == "Never", name + " disabled and hidden");
            }
        }
    }

    private static void TestSigningWorkflow(int childId)
    {
        CheckSigningState(int.MaxValue, false, "不存在");
        var form = new dt209_Forms { IdBase = childId, Code = "WORKFLOW", DisplayName = "流程測試", UploadUser = UserId, UploadTime = DateTime.Now, IsProcessing = true, NextStepProg = UserId };
        int formId = dt209_FormsBUS.Instance.Add(form);
        Check(formId > 0, "create signing regression fixture");
        CheckSigningState(formId, false, "不完整");
        Check(dt209_ProgressBUS.Instance.Add(new dt209_Progress { IdForm = formId, IdUsr = UserId, IdRole = 1 }) > 0, "create signing step");
        CheckSigningState(formId, false, "附件");
        form.AttId = dm_AttachmentBUS.Instance.Add(new dm_Attachment { Thread = "209", EncryptionName = "regression.pdf", ActualName = "regression.pdf" });
        Check(form.AttId > 0 && dt209_FormsBUS.Instance.AddOrUpdate(form), "attach signing regression file metadata");
        CheckSigningState(formId, true, "first pending step");
        using (var dialog = new f209_SignDoc_Info { idBase = formId })
        {
            InvokeLoad(dialog, "f209_SignDoc_Info_Load");
            form.IsProcessing = false;
            Check(dt209_FormsBUS.Instance.AddOrUpdate(form), "complete workflow while dialog is open");
            bool allowed = (bool)typeof(f209_SignDoc_Info).GetMethod("CanProcessCurrentStep", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(dialog, null);
            Check(!allowed && !(bool)GetField(dialog, "canProcessWorkflow"), "stale dialog blocks signing after state changes");
        }
        CheckSigningState(formId, false, "結束");
        form.IsProcessing = true;
        form.IsCancel = true;
        Check(dt209_FormsBUS.Instance.AddOrUpdate(form), "return workflow fixture");
        CheckSigningState(formId, false, "退回");
        form.IsCancel = false;
        form.NextStepProg = "VNW9999999";
        Check(dt209_FormsBUS.Instance.AddOrUpdate(form), "create inconsistent pending signer fixture");
        CheckSigningState(formId, false, "不一致");
        form.NextStepProg = UserId;
        Check(dt209_FormsBUS.Instance.AddOrUpdate(form), "restore pending signer");
        var response = new dt209_ProgInfo { IdForm = formId, IdUsr = "VNW9999999", RespTime = DateTime.Now, Desc = "簽名" };
        Check(dt209_ProgInfoBUS.Instance.Add(response) > 0, "create unmatched history fixture");
        CheckSigningState(formId, false, "不完整");
        response.IdUsr = UserId;
        Check(dt209_ProgInfoBUS.Instance.AddOrUpdate(response), "match history to final step");
        CheckSigningState(formId, false, "所有步驟");
        Check(dt209_ProgressBUS.Instance.Add(new dt209_Progress { IdForm = formId, IdUsr = UserId, IdRole = 2 }) > 0, "add confirmation step for the same signer");
        CheckSigningState(formId, true, "repeated signer proceeds to their next role");
        var steps = dt209_ProgressBUS.Instance.GetListByIdForm(formId).OrderBy(r => r.Id).ToList();
        steps[1].IdUsr = "VNW9999999";
        Check(dt209_ProgressBUS.Instance.AddOrUpdate(steps[1]), "create missing signer fixture");
        CheckSigningState(formId, false, "人員或角色");
        steps[1].IdUsr = UserId;
        steps[1].IdRole = 999;
        Check(dt209_ProgressBUS.Instance.AddOrUpdate(steps[1]), "create missing role fixture");
        CheckSigningState(formId, false, "人員或角色");
    }

    private static void TestLargeRecordId()
    {
        using (var dialog = new f209_AuditDoc_Info())
        using (var timer = new System.Windows.Forms.Timer { Interval = 100 })
        {
            var grid = (DevExpress.XtraGrid.GridControl)GetField(dialog, "gcData");
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)GetField(dialog, "gvData");
            grid.CreateControl();
            grid.DataSource = new[] { new { data = new dt209_Forms { Id = 40000 } } };
            grid.ForceInitialize();
            view.FocusedRowHandle = 0;
            bool received = false;
            timer.Tick += delegate
            {
                var history = Application.OpenForms.OfType<f209_SignProg_Detail>().FirstOrDefault();
                if (history == null) return;
                received = history.idBase == 40000;
                history.Close();
            };
            timer.Start();
            typeof(f209_AuditDoc_Info).GetMethod("ItemApprovalHis_Click", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(dialog, new object[] { dialog, EventArgs.Empty });
            timer.Stop();
            Check(received, "document history action passes ID 40000 without Int16 overflow");
        }
    }

    private static void TestConstructors()
    {
        var assembly = typeof(TPConfigs).Assembly;
        var types = assembly.GetTypes().Where(t => t.Namespace == "KnowledgeSystem.Views._02_StandardsAndTechs._09_ISO14001Docs" && typeof(Control).IsAssignableFrom(t) && t.IsPublic).ToList();
        Check(types.Count == 11, "11 document forms/user controls retained after removing calibration options");
        foreach (var type in types)
        {
            object[] args = new object[0];
            using (var control = (Control)Activator.CreateInstance(type, args))
            {
                var sourceType = assembly.GetType(type.FullName.Replace("_09_ISO14001Docs", "_01_ISOAuditDocs").Replace("f209_", "f201_").Replace("uc209_", "uc201_"), true);
                using (var source = (Control)Activator.CreateInstance(sourceType, args))
                {
                    var expected = Flatten(source).Where(item => !item.Contains(":btnAdvClibAtts:") && !item.Contains(":cbbWatermark:"));
                    var actual = Flatten(control);
                    if (type == typeof(f209_AddAttachment) || type == typeof(f209_AddRangeAtts))
                    {
                        // Layout redistributes space after removing the HC controls.
                        actual = actual.Select(item => item.Substring(0, item.LastIndexOf(':')));
                        expected = expected.Select(item => item.Substring(0, item.LastIndexOf(':')));
                    }
                    Check(control.Size == source.Size && actual.SequenceEqual(expected), type.Name + " retains document controls without calibration options");
                }
            }
        }
    }

    private static IEnumerable<string> Flatten(Control control)
    {
        foreach (Control child in control.Controls)
        {
            yield return child.GetType().FullName + ":" + child.Name.Replace("209", "201") + ":" + child.Size;
            foreach (string item in Flatten(child)) yield return item;
        }
    }

    private static void CaptureMain()
    {
        using (var host = new Form { Width = 1400, Height = 850 })
        using (var control = new uc209_AuditISOMain())
        {
            host.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            Capture(host, "209-Documents.png");
        }
    }

    private static void Capture(Form form, string filename)
    {
        form.Show();
        Application.DoEvents();
        using (var image = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
            image.Save(Path.Combine(ArtifactFolder, filename));
        }
        form.Hide();
        checks++;
        Console.WriteLine("PASS: loaded and rendered " + filename);
    }
}
