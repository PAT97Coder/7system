using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment;

public static class Lab208UiTests
{
    private const BindingFlags F = BindingFlags.Instance | BindingFlags.NonPublic;
    private static object Get(object o, string name) => o.GetType().GetField(name, F).GetValue(o);
    private static void Set(object o, string name, object value) => o.GetType().GetField(name, F).SetValue(o, value);
    private static object Call(object o, string name, params object[] args) => o.GetType().GetMethod(name, F).Invoke(o, args);
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static void CheckIsoAppearance(uc208_LabEnvironment module)
    {
        using (var iso = new KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs.uc201_AuditISOMain())
        {
            var referenceTree = (TreeList)Get(iso, "tlsData");
            var tree = (TreeList)Get(module, "treeData");
            Check(tree.Appearance.HeaderPanel.Font.Equals(referenceTree.Appearance.HeaderPanel.Font), "Tree header font differs from ISO");
            Check(tree.Appearance.HeaderPanel.ForeColor == referenceTree.Appearance.HeaderPanel.ForeColor, "Tree header color differs from ISO");
            Check(tree.Appearance.HeaderPanel.TextOptions.HAlignment == referenceTree.Appearance.HeaderPanel.TextOptions.HAlignment, "Tree header alignment differs from ISO");
            Check(tree.Appearance.Row.Font.Equals(referenceTree.Appearance.Row.Font) && tree.Font.Equals(referenceTree.Font), "Tree data font differs from ISO");
            Check(tree.RowHeight == referenceTree.RowHeight, "Tree row height differs from ISO");
            Check(tree.OptionsView.EnableAppearanceOddRow == referenceTree.OptionsView.EnableAppearanceOddRow, "Alternating rows differ from ISO");
            Check(tree.OptionsSelection.EnableAppearanceHotTrackedRow == referenceTree.OptionsSelection.EnableAppearanceHotTrackedRow, "Row hover differs from ISO");
            Check(tree.OptionsFilter.ExpandNodesOnFiltering == referenceTree.OptionsFilter.ExpandNodesOnFiltering, "Filter expansion differs from ISO");
            var referenceBar = (DevExpress.XtraBars.Bar)Get(iso, "bar2");
            var bar = (DevExpress.XtraBars.Bar)Get(module, "toolbar");
            Check(bar.BarAppearance.Normal.Font.Equals(referenceBar.BarAppearance.Normal.Font), "Toolbar font differs from ISO");
            Check(bar.BarAppearance.Disabled.Font.Equals(referenceBar.BarAppearance.Disabled.Font), "Disabled toolbar font differs from ISO");
            Check(bar.OptionsBar.UseWholeRow == referenceBar.OptionsBar.UseWholeRow && bar.OptionsBar.MultiLine == referenceBar.OptionsBar.MultiLine && bar.OptionsBar.DrawDragBorder == referenceBar.OptionsBar.DrawDragBorder, "Toolbar layout differs from ISO");
            Check(((DevExpress.XtraBars.BarManager)Get(module, "barManager")).MainMenu == bar, "Toolbar is not the main menu");
            foreach (var names in new[] { new[] { "btnAdd", "btnAdd" }, new[] { "btnReload", "btnReload" }, new[] { "btnExport", "btnExportExcel" } })
            {
                var button = (DevExpress.XtraBars.BarButtonItem)Get(module, names[0]);
                var reference = (DevExpress.XtraBars.BarButtonItem)Get(iso, names[1]);
                Check(button.ImageOptions.SvgImageSize == reference.ImageOptions.SvgImageSize, "Button icon size differs from ISO: " + names[0]);
                Check(button.ItemAppearance.Normal.Font.Equals(reference.ItemAppearance.Normal.Font), "Button font differs from ISO: " + names[0]);
                Check(button.ItemAppearance.Hovered.ForeColor == reference.ItemAppearance.Hovered.ForeColor, "Button hover differs from ISO: " + names[0]);
            }
            Check(bar.ItemLinks[0].Item == Get(module, "btnAdd") && bar.ItemLinks[1].Item == Get(module, "btnReload") && bar.ItemLinks[2].Item == Get(module, "btnExport"), "Button order differs from ISO");
            Check(bar.ItemLinks[1].BeginGroup && bar.ItemLinks[2].BeginGroup && bar.ItemLinks[3].BeginGroup, "Toolbar separators differ from ISO");
            Check(module.Appearance.BackColor == iso.Appearance.BackColor, "Module background differs from ISO");
        }
    }
    private static DevExpress.XtraTreeList.Menu.TreeListMenu DeviceMenu(object module, TreeList tree, DevExpress.XtraTreeList.Nodes.TreeListNode target)
    {
        DevExpress.XtraTreeList.TreeListHitInfo hit = null;
        for (int y = 0; y < tree.Height; y++)
        {
            var candidate = tree.CalcHitInfo(new Point(100, y));
            if (candidate.Node == target && candidate.InRowCell) { hit = candidate; break; }
        }
        Check(hit != null, "Could not find target device row");
        var menu = new DevExpress.XtraTreeList.Menu.TreeListMenu(tree);
        menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("預設選單"));
        var args = new PopupMenuShowingEventArgs(menu, hit.MousePoint, true);
        typeof(PopupMenuShowingEventArgs).GetProperty("HitInfo").SetValue(args, hit);
        Call(module, "Tree_PopupMenuShowing", tree, args);
        Check(menu.Items[0].Caption == "預設選單", "Default menu replaced");
        Check(tree.FocusedNode == target, "Right click did not focus clicked device");
        return menu;
    }
    [STAThread]
    public static int Main()
    {
        try
        {
            using (var home = new KnowledgeSystem.Views._00_Generals.f00_Main())
            {
                Check(home.GetType().GetField("btnLabEnvironment", F) == null, "Unexpected temperature/humidity tile on home screen");
            }
            var depts = new DataTable(); depts.Columns.Add("Id"); depts.Columns.Add("DisplayName"); depts.Columns.Add("CanEdit", typeof(bool)); depts.Columns.Add("CanHandle", typeof(bool));
            depts.Rows.Add("D1", "實驗部門", true, true);
            depts.Rows.Add("D2", "唯讀部門", false, false);
            var managers = new DataTable(); managers.Columns.Add("Id"); managers.Columns.Add("DisplayName"); managers.Rows.Add("M1", "管理人");
            var data = new DataTable();
            foreach (string field in new[] { "Id", "DeviceState", "ExpectedMinutes", "ConflictCount" }) data.Columns.Add(field, typeof(int));
            data.Columns.Add("ConfigId", typeof(long));
            foreach (string field in new[] { "MeasuredAt", "ReceivedAt", "EffectiveFrom" }) data.Columns.Add(field, typeof(DateTime));
            foreach (string field in new[] { "Temperature", "Humidity", "TempMin", "TempMax", "HumidityMin", "HumidityMax" }) data.Columns.Add(field, typeof(decimal));
            foreach (string field in new[] { "SN", "DisplayName", "Model", "IdDept", "Location", "IdManager", "ManagerName" }) data.Columns.Add(field);
            DateTime now = dt208_EnvironmentRules.VietnamNow;
            for (int i = 1; i <= 2; i++)
            {
                var row = data.NewRow(); row["Id"] = i; row["ConfigId"] = (long)i; row["DeviceState"] = 0; row["ExpectedMinutes"] = 60; row["ConflictCount"] = 0;
                row["SN"] = "LAB-00" + i; row["DisplayName"] = "溫濕度計 " + i; row["Model"] = "Model"; row["IdDept"] = "D1"; row["Location"] = "實驗室 A"; row["IdManager"] = "M1"; row["ManagerName"] = "管理人";
                row["TempMin"] = 15m; row["TempMax"] = 25m; row["HumidityMin"] = 50m; row["HumidityMax"] = 70m; row["EffectiveFrom"] = now.Date;
                if (i == 1) { row["Temperature"] = 26m; row["Humidity"] = 65m; row["MeasuredAt"] = now.AddMinutes(-5); row["ReceivedAt"] = now; }
                data.Rows.Add(row);
            }
            using (var device = new f208_DeviceInfo("test", depts, managers, "D1", data.Rows[0]))
            {
                Check(!((TextEdit)Get(device, "txbSN")).Enabled, "Existing SN editable");
                ((ComboBoxEdit)Get(device, "temperatureFormat")).SelectedIndex = 1;
                Check(((SpinEdit)Get(device, "tempMin")).Value == 20 && ((SpinEdit)Get(device, "tempMax")).Value == 5, "Min/max to ± conversion failed");
                ((ComboBoxEdit)Get(device, "temperatureFormat")).SelectedIndex = 0;
                Check(((SpinEdit)Get(device, "tempMin")).Value == 15 && ((SpinEdit)Get(device, "tempMax")).Value == 25, "± round trip changed range");
                device.CreateControl(); device.PerformLayout();
                device.Opacity = 0; device.ShowInTaskbar = false; device.Show(); Application.DoEvents();
                var layout = (DevExpress.XtraLayout.LayoutControl)Get(device, "contentPanel");
                Check(device.FormBorderStyle == FormBorderStyle.FixedToolWindow && device.Appearance.BackColor == Color.FromArgb(244, 244, 244), "Input dialog differs from ISO frame");
                Check(layout.Root.EnableIndentsWithoutBorders == DevExpress.Utils.DefaultBoolean.True, "Dialog has no ISO padding");
                Check(((TextEdit)Get(device, "txbSN")).Top == ((TextEdit)Get(device, "txbModel")).Top, "Short fields are not paired");
                Check(((SpinEdit)Get(device, "tempMin")).Top == ((SpinEdit)Get(device, "tempMax")).Top, "Temperature limits are not paired");
                Check(((MemoEdit)Get(device, "txbReason")).Height < device.ClientSize.Height / 3, "Reason memo consumes too much of the dialog");
                foreach (BaseEdit editor in layout.Controls.OfType<BaseEdit>())
                {
                    Check(editor.Properties.Appearance.Font.Size == 14.25F && editor.Properties.Appearance.Options.UseFont, "Dialog editor font is not applied");
                    Check(editor.StyleController == layout && editor.MenuManager == Get(device, "barManager"), "Dialog editor does not share style/menu");
                    Check(editor.Left >= 10 && editor.Top >= 10 && editor.Right <= layout.Width - 10 && editor.Bottom <= layout.Height - 10, "Dialog editor is clipped or missing margins: " + editor.Name);
                }
                using (var image = new Bitmap(device.Width, device.Height)) { device.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "Lab208-DeviceDialog.png")); }
                device.Hide();
            }
            using (var module = new uc208_LabEnvironment())
            using (var host = new Form { Opacity = 0, ShowInTaskbar = false, Size = new Size(1400, 850) })
            {
                CheckIsoAppearance(module);
                module.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), module, module.GetType().GetMethod("Module_Load", F));
                Set(module, "busy", true); Set(module, "departments", depts); Set(module, "devices", data);
                var deptEditor = (DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit)Get(module, "repoDept");
                Check(deptEditor.SelectAllItemCaption == "全選", "Department select-all is not localized");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbDept")).EditValue = "D1";
                host.Controls.Add(module); module.Dock = DockStyle.Fill; host.Show(); Application.DoEvents();
                Set(module, "busy", false); Call(module, "BindTree");
                var tree = (TreeList)Get(module, "treeData");
                Check(tree.Nodes.Count == 1 && tree.Nodes[0].Nodes.Count == 2, "Location/device tree incorrect");
                tree.FocusedNode = tree.Nodes[0].Nodes[0];
                Check((DataRow)Call(module, "SelectedDevice") == data.Rows[0], "Device selection does not resolve DataRow");
                Check(((DevExpress.XtraBars.BarButtonItem)Get(module, "btnEdit")).Enabled, "Edit not enabled for granted department");
                var menu = DeviceMenu(module, tree, tree.Nodes[0].Nodes[1]);
                Check(menu.Items.Any(item => item.Caption == "編輯設備"), "Editor missing edit menu");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbStatus")).EditValue = "超限";
                Check(tree.Nodes[0].Nodes.Count == 1, "Abnormal filter includes unrelated sensor");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbStatus")).EditValue = "尚無資料";
                Check(tree.Nodes[0].Nodes.Count == 1 && tree.Nodes[0].Nodes[0].GetValue("SN").ToString() == "LAB-002", "Missing data treated as normal");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbStatus")).EditValue = "全部";
                using (var image = new Bitmap(module.Width, module.Height)) { module.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "Lab208-Overview.png")); }
                Check(tree.Parent is DevExpress.XtraLayout.LayoutControl && tree.Height > 500, "Tree layout missing");
                var toolbar = (DevExpress.XtraBars.Bar)Get(module, "toolbar");
                foreach (DevExpress.XtraBars.BarItemLink link in toolbar.ItemLinks)
                    Check(link.Item != Get(module, "btnEdit") && link.Item != Get(module, "btnHistory") && link.Item != Get(module, "btnConfigHistory"), "Row action appears on main toolbar");
                Check(tree.Appearance.HeaderPanel.Font.Size == 14.25F, "Tree header font is not standard");
                Set(module, "busy", true);
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbDept")).EditValue = "D2";
                data.Rows[0]["IdDept"] = data.Rows[1]["IdDept"] = "D2";
                Set(module, "busy", false); Call(module, "UpdateButtons");
                Check(!((DevExpress.XtraBars.BarButtonItem)Get(module, "btnAdd")).Enabled && !((DevExpress.XtraBars.BarButtonItem)Get(module, "btnEdit")).Enabled, "Viewer granted edit controls");
                menu = DeviceMenu(module, tree, tree.Nodes[0].Nodes[0]);
                Check(!menu.Items.Any(item => item.Caption == "編輯設備") && menu.Items.Any(item => item.Caption == "歷史與圖表"), "Viewer menu permissions incorrect");
                Set(module, "busy", true);
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbDept")).EditValue = "D1, D2, DENIED";
                data.Rows[0]["IdDept"] = "D1";
                Set(module, "busy", false); Call(module, "BindTree");
                Check(((string[])Call(module, "SelectedDepartments")).SequenceEqual(new[] { "D1", "D2" }), "Unauthorized department accepted by filter");
                Check(tree.Nodes.Count == 2, "Same room in different departments merged");
                Check(((DevExpress.XtraBars.BarButtonItem)Get(module, "btnAdd")).Enabled, "Cannot add in selected editable department");
                menu = DeviceMenu(module, tree, tree.FindNodeByFieldValue("IdDevice", 2));
                Check(!menu.Items.Any(item => item.Caption == "編輯設備"), "Mixed selection grants edit to read-only device");
                menu = DeviceMenu(module, tree, tree.FindNodeByFieldValue("IdDevice", 1));
                Check(menu.Items.Any(item => item.Caption == "編輯設備"), "Mixed selection hides edit for authorized device");
                Set(module, "busy", true);
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbDept")).EditValue = "";
                Set(module, "busy", false); Call(module, "UpdateButtons");
                Check(((string[])Call(module, "SelectedDepartments")).Length == 0 && !((DevExpress.XtraBars.BarButtonItem)Get(module, "btnAdd")).Enabled, "Empty department selection permits adding");
                Check(((DevExpress.XtraBars.BarButtonItem)Get(module, "btnUnknown")).Visibility == DevExpress.XtraBars.BarItemVisibility.Never, "Admin action visible before admin permission");
            }
            using (var history = new f208_ReadingHistory("test", "LAB-001"))
            {
                var readings = data.Clone(); readings.Columns.Add("Status"); readings.Columns.Add("CanHandle", typeof(bool));
                var row = readings.NewRow(); row.ItemArray = data.Rows[0].ItemArray;
                row["Status"] = "溫度超限"; row["CanHandle"] = true; readings.Rows.Add(row);
                Set(history, "readings", readings); Call(history, "DrawCharts");
                ((DevExpress.XtraGrid.GridControl)Get(history, "grid")).DataSource = readings;
                Call(history, "UpdateButtons");
                Check(((DevExpress.XtraBars.BarButtonItem)Get(history, "btnAction")).Enabled, "Handler cannot handle abnormal reading");
                row["CanHandle"] = false; Call(history, "UpdateButtons");
                Check(!((DevExpress.XtraBars.BarButtonItem)Get(history, "btnAction")).Enabled, "Viewer can handle abnormal reading");
                row["CanHandle"] = true; row["Status"] = "正常"; Call(history, "UpdateButtons");
                Check(!((DevExpress.XtraBars.BarButtonItem)Get(history, "btnAction")).Enabled, "Handler can handle normal reading");
                Check(((DevExpress.XtraCharts.ChartControl)Get(history, "chartTemperature")).Series.Count == 3, "Threshold series missing");
                // A reading before registration must not get a fabricated threshold.
                row["TempMin"] = DBNull.Value; row["HumidityMin"] = DBNull.Value; Call(history, "DrawCharts");
                Check(dt208_EnvironmentRules.MeasurementStatus(row) == "未有有效設定", "Unconfigured reading evaluated as passing");
            }
            using (var action = new f208_ReadingAction("test", 1))
            {
                action.CreateControl(); Check(!((DevExpress.XtraBars.BarButtonItem)Get(action, "btnSave")).Enabled, "Action dialog allows saving without permission");
                action.Opacity = 0; action.ShowInTaskbar = false; action.Show(); Application.DoEvents();
                var layout = (DevExpress.XtraLayout.LayoutControl)Get(action, "contentPanel");
                Check(action.FormBorderStyle == FormBorderStyle.FixedToolWindow && layout.Root.EnableIndentsWithoutBorders == DevExpress.Utils.DefaultBoolean.True, "Action dialog differs from ISO frame");
                foreach (BaseEdit editor in layout.Controls.OfType<BaseEdit>())
                    Check(editor.Left >= 10 && editor.Top >= 10 && editor.Right <= layout.Width - 10 && editor.Bottom <= layout.Height - 10, "Action dialog content clipped");
                using (var image = new Bitmap(action.Width, action.Height)) { action.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "Lab208-ActionDialog.png")); }
                action.Hide();
            }
            using (var device = new f208_DeviceInfo("test", depts, managers, "D2", null))
                Check(!((DevExpress.XtraBars.BarButtonItem)Get(device, "btnSave")).Enabled, "Viewer can save device dialog");
            using (var audit = new f208_DataList("設定紀錄", data)) { audit.CreateControl(); }
            foreach (DevExpress.XtraGrid.Localization.GridStringId id in Enum.GetValues(typeof(DevExpress.XtraGrid.Localization.GridStringId)))
                if (id.ToString().StartsWith("MenuColumn") || id.ToString().StartsWith("MenuGroup"))
                    Check(!System.Text.RegularExpressions.Regex.IsMatch(DevExpress.XtraGrid.Localization.GridLocalizer.Active.GetLocalizedString(id), "[A-Za-z]"), "Untranslated grid menu: " + id);
            foreach (DevExpress.XtraTreeList.Localization.TreeListStringId id in Enum.GetValues(typeof(DevExpress.XtraTreeList.Localization.TreeListStringId)))
                if (id.ToString().StartsWith("MenuColumn") || id.ToString().StartsWith("MenuNode"))
                    Check(!System.Text.RegularExpressions.Regex.IsMatch(DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active.GetLocalizedString(id), "[A-Za-z]"), "Untranslated tree menu: " + id);
            Console.WriteLine("PASS: form initialization; range conversion; tree/filter/chart behavior; right-click row selection and preserved menus; viewer/editor/handler controls; localized menus.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
