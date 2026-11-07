using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraReports.UI;
using KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel;
using KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce;
using Newtonsoft.Json;
using Scriban;
class EquipmentSmoke
{
    static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }
    static object Field(object obj, string name) { return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj); }
    static object Call(object obj, string name, params object[] args) { return obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, args); }
    static void SetField(object obj, string name, object value) { obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value); }
    static void WithShownForm(Form form, Action inspect)
    {
        using (var timer = new Timer { Interval = 150 })
        {
            Exception failure = null;
            bool inspected = false;
            form.Shown += (s, e) => timer.Start();
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                try { inspect(); inspected = true; }
                catch (Exception ex) { failure = ex; }
                finally { form.Close(); }
            };
            form.ShowDialog();
            if (failure != null) throw failure;
            Check(inspected, "runtime form inspection completed");
        }
    }

    static void TestManagerPopup()
    {
        var factory = typeof(uc310_EquipmentInfo).GetMethod("CreateManagerEditor", BindingFlags.Static | BindingFlags.NonPublic);
        foreach (bool populated in new[] { false, true })
        {
            var users = populated ? new List<dm_User> {
                new dm_User { Id = "VNW000001", IdDepartment = "7810", DisplayName = "阮文安", DisplayNameVN = "Nguyễn Văn An" }
            } : new List<dm_User>();
            using (var editor = (DevExpress.XtraEditors.SearchLookUpEdit)factory.Invoke(null, new object[] { users }))
            using (var form = new Form { Width = 850, Height = 180 })
            {
                editor.Dock = DockStyle.Top;
                form.Controls.Add(editor);
                WithShownForm(form, () => {
                    editor.ShowPopup(); Application.DoEvents();
                    var view = (GridView)editor.Properties.PopupView;
                    Check(editor.IsPopupOpen && view.Columns.Count == 3 && view.DataRowCount == users.Count, "manager lookup popup opens with empty/populated data: " + populated);
                    Check(view.Appearance.HeaderPanel.Font.SizeInPoints == 14.25f && view.Appearance.Row.Font.SizeInPoints == 12f &&
                        view.Appearance.HeaderPanel.TextOptions.HAlignment == DevExpress.Utils.HorzAlignment.Center && view.OptionsView.EnableAppearanceOddRow,
                        "manager popup follows standard grid appearance: " + populated);
                    if (populated)
                    {
                        editor.EditValue = users[0].Id;
                        Check(editor.Text.Contains("Nguyễn Văn An") && editor.Text.Contains("阮文安"), "manager lookup displays Vietnamese and Chinese names");
                    }
                    editor.ClosePopup();
                });
            }
        }
    }

    static void TestEquipmentGrid()
    {
        using (var form = new Form { Width = 1200, Height = 600 })
        using (var control = new uc310_EquipmentInfo { Dock = DockStyle.Fill })
        {
            // Load synthetic records through the production binder, without opening the application DB.
            control.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), control, "uc310_EquipmentInfo_Load");
            form.Controls.Add(control);
            Call(control, "BindEquipmentData", new List<vw310_EquipmentInfo> {
                new vw310_EquipmentInfo { Id = 1, Code = "EQ-1", DeptId = "7810", ManagerId = "ACTIVE" },
                new vw310_EquipmentInfo { Id = 2, Code = "EQ-2", DeptId = "7810", ManagerId = "LEAVER" },
                new vw310_EquipmentInfo { Id = 3, Code = "EQ-3", DeptId = "7820", ManagerId = "MISSING" },
                new vw310_EquipmentInfo { Id = 4, Code = "EQ-4", DeptId = "7820", ManagerId = "PLANNED" }
            }, new List<dm_User> {
                new dm_User { Id = "ACTIVE", Status = 0, IdDepartment = "7810" },
                new dm_User { Id = "LEAVER", Status = 1, IdDepartment = "7810" },
                new dm_User { Id = "PLANNED", Status = 0, IdDepartment = "7820", ResignPlan = new DateTime(2026, 12, 1) }
            });
            WithShownForm(form, () => {
                var view = (GridView)Field(control, "gvData");
                Check(view.DataRowCount == 4, "main equipment grid binds records after showing");
                Check((string)view.GetRowCellValue(1, "ManagerStatus") == "待交接：已離職" && (string)view.GetRowCellValue(2, "ManagerStatus") == "待指定管理人", "departed and missing managers flagged");
                var viewerMenu = new DXPopupMenu();
                Call(control, "PopulateEquipmentMenu", viewerMenu, 0);
                Check(!viewerMenu.Items.Contains((DXMenuItem)Field(control, "itemBatchManager")), "viewer cannot access transfer action");
                SetField(control, "canManage", true);
                SetField(control, "editableDeptIds", new List<string> { "7810" });
                Call(control, "PopulateEquipmentMenu", new DXPopupMenu(), 0);
                Check(((DXMenuItem)Field(control, "itemBatchManager")).Enabled, "transfer enabled for permitted department");
                Call(control, "PopulateEquipmentMenu", new DXPopupMenu(), 2);
                Check(!((DXMenuItem)Field(control, "itemBatchManager")).Enabled, "transfer disabled outside permitted department");
                Call(control, "ItemMultiselect_Click", null, EventArgs.Empty);
                view.ClearSelection(); view.SelectRow(0); view.SelectRow(1);
                Call(control, "PopulateEquipmentMenu", new DXPopupMenu(), 1);
                Check(((List<vw310_EquipmentInfo>)Call(control, "GetSelectedEquipmentInfos")).Count == 2, "right-click selected row preserves batch");
                Call(control, "PopulateEquipmentMenu", new DXPopupMenu(), 2);
                Check(((List<vw310_EquipmentInfo>)Call(control, "GetSelectedEquipmentInfos")).Single().Id == 3, "right-click unselected row targets clicked equipment");
                var groupMenu = new DXPopupMenu();
                Call(control, "PopulateEquipmentMenu", groupMenu, -1);
                Check(groupMenu.Items.Count == 0, "non-data rows have no equipment actions");
                SetField(control, "isEHSAdmin", true);
                Call(control, "PopulateEquipmentMenu", new DXPopupMenu(), 2);
                Check(((DXMenuItem)Field(control, "itemBatchManager")).Enabled, "EHS admin can transfer across departments");
                var toolbar = (DevExpress.XtraBars.Bar)Field(control, "bar2");
                Check(toolbar.ItemLinks.Count == 3, "toolbar only contains add, reload and pending filter");
                var filter = (DevExpress.XtraBars.BarButtonItem)toolbar.ItemLinks.Cast<DevExpress.XtraBars.BarItemLink>().First(r => r.Item.Caption == "待交接設備").Item;
                filter.PerformClick();
                Check(view.DataRowCount == 3, "pending filter includes departed, planned and missing managers");
                filter.PerformClick();
                Check(view.DataRowCount == 4, "pending filter toggles off without losing data");
            });
        }
    }

    static void TestInputValidation()
    {
        var valid = new dt310_EquipmentInfo { Code = "EQ-VALID", DisplayNameVN = "Thiết bị", DisplayNameTW = "設備", DeptId = "7810", ManagerId = "USER" };
        var validation = typeof(dt310_EquipmentInfoBUS).GetMethod("ValidateEquipmentData", BindingFlags.Static | BindingFlags.NonPublic);
        validation.Invoke(null, new object[] { valid });
        foreach (Action<dt310_EquipmentInfo> invalidate in new Action<dt310_EquipmentInfo>[] {
            r => r.Code = " ", r => r.Code = new string('X', 51), r => r.DisplayNameVN = null,
            r => r.DisplayNameTW = new string('設', 256), r => r.DeptId = "", r => r.ManagerId = null,
            r => r.Note = new string('N', 501)
        })
        {
            var draft = JsonConvert.DeserializeObject<dt310_EquipmentInfo>(JsonConvert.SerializeObject(valid));
            invalidate(draft);
            bool blocked = false;
            try { dt310_EquipmentInfoBUS.Instance.Add(draft); }
            catch (InvalidOperationException) { blocked = true; }
            Check(blocked, "invalid equipment rejected before database access");
        }
        foreach (var changes in new[] {
            new List<dt310_EquipmentInfoBUS.ManagerChange> { null },
            new List<dt310_EquipmentInfoBUS.ManagerChange> { new dt310_EquipmentInfoBUS.ManagerChange { EquipmentId = 1, DeptId = "7810", PreviousManagerId = "OLD", NewManagerId = "OLD" } },
            new List<dt310_EquipmentInfoBUS.ManagerChange> { new dt310_EquipmentInfoBUS.ManagerChange { EquipmentId = 1, DeptId = "7810", PreviousManagerId = "OLD", NewManagerId = "" } }
        })
        {
            bool blocked = false;
            try { dt310_EquipmentInfoBUS.Instance.TransferManagers(changes, "ACTOR"); }
            catch (InvalidOperationException) { blocked = true; }
            Check(blocked, "invalid transfer rejected before database access");
        }
    }

    static void TestEquipmentEditor()
    {
        using (var form = new f310_EquipmentInfo_Info())
        {
            form.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, "f310_EquipmentInfo_Info_Load");
            SetField(form, "equipmentInfo", new dt310_EquipmentInfo { Id = 1, DeptId = "7810" });
            SetField(form, "canManage", true);
            SetField(form, "editableDeptIds", new List<string> { "7810" });
            var layouts = new[] { "lcCode", "lcNameVN", "lcNameTW", "lcDept", "lcManager", "lcNote" }
                .Select(name => (LayoutControlItem)Field(form, name)).ToList();
            SetField(form, "lcControls", layouts);
            SetField(form, "lcImpControls", layouts.Take(5).ToList());
            form.eventInfo = KnowledgeSystem.Helpers.EventFormInfo.View;
            Call(form, "LockControl");
            WithShownForm(form, () => {
                var code = (DevExpress.XtraEditors.TextEdit)Field(form, "txbCode");
                Check(!code.Enabled && ((DevExpress.XtraBars.BarButtonItem)Field(form, "btnEdit")).Visibility == DevExpress.XtraBars.BarItemVisibility.Always, "equipment detail opens read-only with permitted edit action");
                Check(form.StartPosition == FormStartPosition.CenterParent && !form.MinimizeBox && !form.MaximizeBox, "equipment detail uses standard parent-centered child frame");
                form.eventInfo = KnowledgeSystem.Helpers.EventFormInfo.Update;
                Call(form, "LockControl"); Call(form, "LockControl");
                Check(code.Enabled && layouts[0].Text.Split(new[] { "<color=red>*</color>" }, StringSplitOptions.None).Length == 2, "edit mode enables fields without duplicating required markers");
                SetField(form, "editableDeptIds", new List<string>());
                Call(form, "LockControl");
                Check(!code.Enabled && ((DevExpress.XtraBars.BarButtonItem)Field(form, "btnConfirm")).Visibility == DevExpress.XtraBars.BarItemVisibility.Never, "equipment detail blocks editing after permission is removed");
            });
        }
        using (var form = new f310_UpdateLeaveUser_Info())
        {
            form.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, "f310_UpdateLeaveUser_Info_Load");
            var view = (GridView)Field(form, "gvEquipment");
            view.GridControl.DataSource = new List<f310_UpdateLeaveUser_Info.EquipmentHandoverData> {
                new f310_UpdateLeaveUser_Info.EquipmentHandoverData { EquipmentId = 1, Code = "EQ-1", DeptId = "7810", PreviousManagerId = "OLD" }
            };
            var editor = (DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit)Field(form, "equipmentUserEditor");
            editor.DataSource = new[] { new { Id = "NEW", DisplayName = "New manager" } };
            editor.DisplayMember = "DisplayName"; editor.ValueMember = "Id";
            var tabs = (TabbedControlGroup)Field(form, "tabbedControlGroup1");
            tabs.SelectedTabPage = tabs.TabPages[3];
            WithShownForm(form, () => {
                Check(view.DataRowCount == 1 && view.Columns.Count == 6, "handover equipment tab displays its actual bound grid");
                Check(view.Appearance.HeaderPanel.Font.SizeInPoints == 14.25f && view.Appearance.Row.Font.SizeInPoints == 12f &&
                    view.GridControl.UseEmbeddedNavigator && view.GridControl.MenuManager != null && !view.OptionsView.ColumnAutoWidth,
                    "equipment handover matches sibling tab appearance and navigator");
                view.FocusedRowHandle = 0; view.FocusedColumn = view.Columns["UserId"]; view.ShowEditor();
                Check(view.ActiveEditor != null, "handover successor editor opens");
                view.ActiveEditor.EditValue = "NEW"; view.PostEditor(); view.CloseEditor(); view.UpdateCurrentRow();
                var row = (f310_UpdateLeaveUser_Info.EquipmentHandoverData)view.GetRow(0);
                Check(row.UserId == "NEW", "handover successor selection is posted into submitted data");
            });
        }
    }

    static void TestEditSnapshot()
    {
        using (var form = new f310_EquipmentInfo_Info())
        {
            var original = new dt310_EquipmentInfo { Id = 7, Code = "ORIGINAL", DeptId = "7810", ManagerId = "OLD", Note = "before" };
            SetField(form, "equipmentInfo", original);
            ((DevExpress.XtraEditors.TextEdit)Field(form, "txbCode")).EditValue = "NEW-CODE";
            ((DevExpress.XtraEditors.TextEdit)Field(form, "txbNameVN")).EditValue = "New name";
            ((DevExpress.XtraEditors.TextEdit)Field(form, "txbNameTW")).EditValue = "New name TW";
            ((DevExpress.XtraEditors.SearchLookUpEdit)Field(form, "cbbDept")).EditValue = "7820";
            ((DevExpress.XtraEditors.SearchLookUpEdit)Field(form, "cbbManager")).EditValue = "NEW";
            var draft = (dt310_EquipmentInfo)Call(form, "CreateEquipmentDraft");
            Check(draft.Code == "NEW-CODE" && draft.ManagerId == "NEW" && original.Code == "ORIGINAL" && original.ManagerId == "OLD", "editing draft preserves loaded ownership and fields");
            draft.Id = 999; // Simulate an identity assigned before a failed transaction rolls back.
            var retry = (dt310_EquipmentInfo)Call(form, "CreateEquipmentDraft");
            Check(retry.Id == 7 && original.DeptId == "7810", "retry after failed save retains original snapshot");
            var guard = typeof(dt310_EquipmentInfoBUS).GetMethod("EnsureUnchanged", BindingFlags.Static | BindingFlags.NonPublic);
            guard.Invoke(null, new object[] { original, original });
            foreach (var changed in new[] {
                new dt310_EquipmentInfo { Id = 7, Code = "RENAMED", DeptId = "7810", ManagerId = "OLD", Note = "before" },
                new dt310_EquipmentInfo { Id = 7, Code = "ORIGINAL", DeptId = "7810", ManagerId = "OTHER", Note = "before" },
                new dt310_EquipmentInfo { Id = 7, Code = "ORIGINAL", DeptId = "7810", ManagerId = "OLD", Note = "before", DeletedAt = DateTime.Now },
                null
            })
            {
                bool blocked = false;
                try { guard.Invoke(null, new object[] { changed, original }); }
                catch (TargetInvocationException ex) { blocked = ex.InnerException is InvalidOperationException; }
                Check(blocked, "stale edit/delete rejected for changed or missing record");
            }
        }
    }
    static void TestHistoryDialog(List<dt310_EquipmentInfoBUS.EquipmentHistory> rows, string scenario)
    {
        var factory = typeof(uc310_EquipmentInfo).GetMethod("CreateHistoryDialog", BindingFlags.Static | BindingFlags.NonPublic);
        using (var form = (Form)factory.Invoke(null, new object[] { "EQ-HISTORY", rows }))
        using (var closeTimer = new Timer { Interval = 150 })
        {
            Exception failure = null;
            bool inspected = false;
            form.Shown += (s, e) => closeTimer.Start();
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                try
                {
                    var grid = (DevExpress.XtraGrid.GridControl)Field(form, "gcData");
                    var view = (GridView)grid.MainView;
                    Check(form.FormBorderStyle == FormBorderStyle.FixedToolWindow && !form.MaximizeBox && !form.MinimizeBox, "history uses standard child-window frame: " + scenario);
                    Check(view.Appearance.HeaderPanel.Font.SizeInPoints == 14.25f && view.Appearance.Row.Font.SizeInPoints == 12f && view.Appearance.HeaderPanel.ForeColor == System.Drawing.Color.Black, "history matches standard grid typography: " + scenario);
                    Check(view.Appearance.HeaderPanel.TextOptions.HAlignment == DevExpress.Utils.HorzAlignment.Center && view.OptionsView.EnableAppearanceOddRow && !view.OptionsView.ColumnAutoWidth, "history matches standard column layout: " + scenario);
                    var closeButton = (DevExpress.XtraBars.BarButtonItem)Field(form, "btnClose");
                    Check(closeButton.Caption == "關閉" && closeButton.ImageOptions.SvgImage != null && ((DevExpress.XtraBars.Bar)Field(form, "bar2")).BarAppearance.Normal.Font.SizeInPoints == 14.25f, "history has standard toolbar with close action: " + scenario);
                    Check(form.Visible && grid.IsHandleCreated && view.Columns.Count == 7, "history dialog opens with seven columns: " + scenario);
                    Check(view.DataRowCount == (rows == null ? 0 : rows.Count) && !view.OptionsBehavior.Editable, "history rows and read-only mode: " + scenario);
                    Check(view.Columns.Cast<DevExpress.XtraGrid.Columns.GridColumn>().Sum(c => c.Width) < grid.ClientSize.Width, "history default columns fit inside dialog: " + scenario);
                    if (rows != null && rows.Count > 0)
                    {
                        Check((string)view.GetRowCellValue(0, "NewManager") == rows[0].NewManager, "history displays successor: " + scenario);
                        Check(view.GetRowCellDisplayText(0, "ChangedAt") == rows[0].ChangedAt.ToString("yyyy/MM/dd HH:mm"), "history timestamp formatted: " + scenario);
                    }
                    using (var screenshot = new System.Drawing.Bitmap(form.Width, form.Height))
                    {
                        form.DrawToBitmap(screenshot, new System.Drawing.Rectangle(System.Drawing.Point.Empty, form.Size));
                        screenshot.Save("../310-history-ui-" + (rows == null ? "null" : rows.Count == 0 ? "empty" : "populated") + ".png");
                    }
                    inspected = true;
                }
                catch (Exception ex) { failure = ex; }
                finally { form.Close(); }
            };
            form.ShowDialog();
            if (failure != null) throw failure;
            Check(inspected, "history shown event completed: " + scenario);
        }
    }
    [STAThread]
    static void Main()
    {
        TestEditSnapshot();
        TestInputValidation();
        TestEquipmentEditor();
        TestManagerPopup();
        TestEquipmentGrid();
        TestHistoryDialog(new List<dt310_EquipmentInfoBUS.EquipmentHistory>(), "empty history");
        TestHistoryDialog(new List<dt310_EquipmentInfoBUS.EquipmentHistory> {
            new dt310_EquipmentInfoBUS.EquipmentHistory { ChangedAt = new DateTime(2026, 11, 7, 15, 30, 0), PreviousManager = "OLD", NewManager = "NEW", PreviousDeptId = "7810", NewDeptId = "7820", ChangedBy = "ADMIN", HandoverId = 12 },
            new dt310_EquipmentInfoBUS.EquipmentHistory { ChangedAt = new DateTime(2026, 11, 6, 9, 0, 0), PreviousManager = "", NewManager = "OLD", PreviousDeptId = "", NewDeptId = "7810", ChangedBy = "ADMIN", HandoverId = null }
        }, "populated history");
        TestHistoryDialog(null, "null history");
        using (var form = new f310_UpdateLeaveUser_Info())
        {
            Check(((TabbedControlGroup)Field(form, "tabbedControlGroup1")).TabPages.Count == 4, "handover includes equipment tab");
            var grid = (GridView)Field(form, "gvEquipment");
            Check(grid.Columns["UserId"].OptionsColumn.AllowEdit && grid.Columns.Where(c => c.FieldName != "UserId").All(c => !c.OptionsColumn.AllowEdit), "only successor editable");
        }
        using (var control = new uc310_EquipmentInfo())
        {
            var grid = (GridView)Field(control, "gvData");
            Check(grid.Columns["ManagerStatus"] != null && !grid.OptionsSelection.MultiSelect, "equipment status and default single selection available");
            control.GetType().GetMethod("ItemMultiselect_Click", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(control, new object[] { null, EventArgs.Empty });
            Check(grid.OptionsSelection.MultiSelect, "popup action enables batch selection");
        }
        var old = JsonConvert.DeserializeObject<f310_UpdateLeaveUser_Info.UpdateLeaveUserJson>("{\"UnitEHSOrg\":[],\"EHSFunction\":[],\"Area5SResponsible\":[]}");
        Check(old != null && old.Equipment == null, "legacy handover JSON remains readable");
        bool rejected = false;
        try { dt310_EquipmentInfoBUS.Instance.TransferManagers(new List<dt310_EquipmentInfoBUS.ManagerChange> { new dt310_EquipmentInfoBUS.ManagerChange { EquipmentId = 1, DeptId = "7810", PreviousManagerId = "OLD", NewManagerId = "NEW" }, new dt310_EquipmentInfoBUS.ManagerChange { EquipmentId = 1, DeptId = "7810", PreviousManagerId = "OLD", NewManagerId = "NEW" } }, "test"); }
        catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "duplicate equipment rejected before database access");
        var template = Template.Parse(File.ReadAllText("../../KnowledgeSystem/AppResources/310_updateleaveruser.html"));
        Check(!template.HasErrors, "handover template parses");
        foreach (bool include in new[] { false, true })
        {
            var html = template.Render(new {
                userleave = "OLD", createby = "ADMIN", unitlist = new object[0], funclist = new object[0], arealist = new object[0],
                equipmentlist = include ? new[] { new { code = "EQ-001", namevn = "Thiet bi", nametw = "Device", dept = "7810", oldemp = "OLD", reciver = "NEW" } } : new[] { new { code = "", namevn = "", nametw = "", dept = "", oldemp = "", reciver = "" } }.Take(0).ToArray(),
                sign1st = (object)null, sign2nd = (object)null, sign3th = (object)null, sign4th = (object)null
            });
            Check(html.Contains("EQ-001") == include, "equipment section rendered only when populated: " + include);
        }
        using (var report = new rpDeviceManagement())
        {
            report.DataSource = new[] {
                new { NameVN = "Máy đo kiểm thiết bị", NameTW = "設備量測儀器", Code = "EQ-001", Dept = "7810", UserVN = "Nguyễn Văn An", UserTW = "阮文安" },
                new { NameVN = "Thiết bị thứ hai", NameTW = "第二台設備", Code = "EQ-002", Dept = "7820", UserVN = "Trần Văn Bình", UserTW = "陳文平" }
            }.Cast<object>().ToList();
            report.CreateDocument();
            Check(report.Pages.Count == 2, "one printed label per equipment");
            report.ExportToPdf("../310-equipment-label-smoke.pdf");
        }
        using (var report = new rpDeviceManagement())
        {
            report.DataSource = new[] { new { NameVN = new string('A', 255), NameTW = new string('設', 255), Code = new string('C', 50), Dept = "7810", UserVN = new string('N', 128), UserTW = "阮文安" } };
            report.CreateDocument();
            Check(report.Pages.Count == 1, "maximum-length equipment fields stay on one label");
            report.ExportToPdf("../310-equipment-long-label-smoke.pdf");
        }
    }
}
