using System;
using System.Data;
using System.Drawing;
using System.IO;
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
            depts.Rows.Add("D1", "Department 1", true, true);
            var managers = new DataTable(); managers.Columns.Add("Id"); managers.Columns.Add("DisplayName"); managers.Rows.Add("M1", "Manager");
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
                row["SN"] = "LAB-00" + i; row["DisplayName"] = "Nhiệt ẩm kế " + i; row["Model"] = "Model"; row["IdDept"] = "D1"; row["Location"] = "Phòng A"; row["IdManager"] = "M1"; row["ManagerName"] = "Manager";
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
            }
            using (var module = new uc208_LabEnvironment())
            using (var host = new Form { Opacity = 0, ShowInTaskbar = false, Size = new Size(1400, 850) })
            {
                module.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), module, module.GetType().GetMethod("Module_Load", F));
                Set(module, "busy", true); Set(module, "departments", depts); Set(module, "devices", data);
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbDept")).EditValue = "D1";
                host.Controls.Add(module); module.Dock = DockStyle.Fill; host.Show(); Application.DoEvents();
                Set(module, "busy", false); Call(module, "BindTree");
                var tree = (TreeList)Get(module, "treeData");
                Check(tree.Nodes.Count == 1 && tree.Nodes[0].Nodes.Count == 2, "Location/device tree incorrect");
                tree.FocusedNode = tree.Nodes[0].Nodes[0];
                Check((DataRow)Call(module, "SelectedDevice") == data.Rows[0], "Device selection does not resolve DataRow");
                Check(((DevExpress.XtraBars.BarButtonItem)Get(module, "btnEdit")).Enabled, "Edit not enabled for granted department");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbStatus")).EditValue = "超限";
                Check(tree.Nodes[0].Nodes.Count == 1, "Abnormal filter includes unrelated sensor");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbStatus")).EditValue = "尚無資料";
                Check(tree.Nodes[0].Nodes.Count == 1 && tree.Nodes[0].Nodes[0].GetValue("SN").ToString() == "LAB-002", "Missing data treated as normal");
                ((DevExpress.XtraBars.BarEditItem)Get(module, "cbbStatus")).EditValue = "全部";
                using (var image = new Bitmap(module.Width, module.Height)) { module.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "Lab208-Overview.png")); }
                Check(module.Controls.Contains(tree) && tree.Height > 500, "Tree layout missing");
            }
            using (var history = new f208_ReadingHistory("test", "LAB-001"))
            {
                var readings = data.Clone(); readings.Columns.Add("Status"); readings.Columns.Add("CanHandle", typeof(bool));
                var row = readings.NewRow(); row.ItemArray = data.Rows[0].ItemArray;
                row["Status"] = "溫度超限"; row["CanHandle"] = true; readings.Rows.Add(row);
                Set(history, "readings", readings); Call(history, "DrawCharts");
                Check(((DevExpress.XtraCharts.ChartControl)Get(history, "chartTemperature")).Series.Count == 3, "Threshold series missing");
                // A reading before registration must not get a fabricated threshold.
                row["TempMin"] = DBNull.Value; row["HumidityMin"] = DBNull.Value; Call(history, "DrawCharts");
                Check(dt208_EnvironmentRules.MeasurementStatus(row) == "未有有效設定", "Unconfigured reading evaluated as passing");
            }
            using (var action = new f208_ReadingAction("test", 1)) { action.CreateControl(); }
            using (var audit = new f208_DataList("Audit", data)) { audit.CreateControl(); }
            Console.WriteLine("PASS: form initialization; ± range round trip; immutable SN; tree hierarchy/selection; edit permission; abnormal/no-data filters; charts/thresholds and unconfigured data.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
