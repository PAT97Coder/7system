using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public partial class uc208_LabEnvironment : XtraUserControl
    {
        private readonly dt208_EnvironmentBUS bus = dt208_EnvironmentBUS.Instance;
        private DataTable departments;
        private DataTable devices;
        private readonly Dictionary<int, DataRow> rows = new Dictionary<int, DataRow>();
        private bool busy;
        private bool loaded;
        private string UserId => TPConfigs.LoginUser.Id;
        public uc208_LabEnvironment() { Lab208Ui.InitializeLocalization(); InitializeComponent(); }

        private async void Module_Load(object sender, EventArgs e)
        {
            if (loaded) return;
            loaded = true;
            busy = true;
            try
            {
                if (!await Task.Run(() => bus.IsInstalled()))
                {
                    lblStatus.Caption = "模組尚未安裝，請聯絡系統管理員。";
                    return;
                }
                var allowed = await Task.Run(() => bus.GetDepartments(UserId));
                bool admin = await Task.Run(() => bus.IsAdmin(UserId));
                if (IsDisposed) return;
                departments = allowed;
                repoDept.Items.Clear();
                repoDept.Items.AddRange(departments.AsEnumerable().Select(r => new DevExpress.XtraEditors.Controls.CheckedListBoxItem
                {
                    Value = r["Id"].ToString(), Description = r["Id"].ToString(), CheckState = CheckState.Checked
                }).ToArray());
                btnUnknown.Visibility = admin ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                if (departments.Rows.Count == 0)
                {
                    lblStatus.Caption = "尚未授予任何部門的溫濕度查看權限。";
                    return;
                }
                cbbDept.EditValue = string.Join(", ", departments.AsEnumerable().Select(r => r["Id"].ToString()));
                if (Parent != null) refreshTimer.Start();
            }
            catch (Exception ex) { ShowError(ex); }
            finally { busy = false; UpdateButtons(); }
            await ReloadAsync();
        }

        private DataRow SelectedDevice()
        {
            if (treeData.FocusedNode == null) return null;
            int id = Convert.ToInt32(treeData.FocusedNode.GetValue("IdDevice"));
            return rows.TryGetValue(id, out DataRow row) ? row : null;
        }
        private string[] SelectedDepartments()
        {
            if (departments == null) return new string[0];
            var selected = new HashSet<string>((cbbDept.EditValue?.ToString() ?? "").Split(repoDept.SeparatorChar)
                .Select(id => id.Trim()).Where(id => id.Length > 0));
            return departments.AsEnumerable().Select(r => r["Id"].ToString()).Where(selected.Contains).Distinct().ToArray();
        }
        private bool CanEdit(string department)
        {
            var dept = departments?.AsEnumerable().FirstOrDefault(r => r["Id"].ToString() == department);
            return dept != null && Convert.ToBoolean(dept["CanEdit"]);
        }
        private string NewDeviceDepartment() => SelectedDepartments().FirstOrDefault(CanEdit);
        private bool CanEditSelectedDevice() => SelectedDevice() != null && CanEdit(SelectedDevice()["IdDept"].ToString());
        private bool CanHandleSelectedReading()
        {
            var row = SelectedDevice();
            if (row == null || row.IsNull("ReadingId")) return false;
            var dept = departments?.AsEnumerable().FirstOrDefault(r => r["Id"].ToString() == row["IdDept"].ToString());
            return dept != null && !dept.IsNull("CanHandle") && Convert.ToBoolean(dept["CanHandle"]);
        }
        private void UpdateButtons()
        {
            bool selected = SelectedDevice() != null;
            btnAdd.Enabled = !busy && NewDeviceDepartment() != null;
            btnEdit.Enabled = !busy && selected && CanEditSelectedDevice();
            btnHistory.Enabled = btnConfigHistory.Enabled = !busy && selected;
            btnReload.Enabled = btnExport.Enabled = !busy && devices != null;
            btnMonthlyReport.Enabled = !busy && devices != null && devices.Rows.Count > 0;
            cbbDept.Enabled = !busy && departments != null && departments.Rows.Count > 0;
            cbbStatus.Enabled = !busy && devices != null;
            btnUnknown.Enabled = !busy;
        }
        private void Tree_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.HitInfo?.Node == null) return;
            treeData.FocusedNode = e.HitInfo.Node;
            UpdateButtons();
            if (SelectedDevice() == null) return;
            e.Menu.Items.Add(Lab208Ui.Menu("歷史與圖表", (s, a) => OpenHistory(), Helpers.TPSvgimages.View, btnHistory.Enabled, true));
            e.Menu.Items.Add(Lab208Ui.Menu("設定紀錄", (s, a) => ConfigHistory_Click(s, null), Helpers.TPSvgimages.Info, btnConfigHistory.Enabled));
            if (CanHandleSelectedReading())
                e.Menu.Items.Add(Lab208Ui.Menu("取樣超限處理", (s, a) => OpenHistory(), Helpers.TPSvgimages.Edit, !busy, true));
            if (CanEditSelectedDevice()) e.Menu.Items.Add(Lab208Ui.Menu("編輯設備", (s, a) => EditDevice_Click(s, null), Helpers.TPSvgimages.Edit, btnEdit.Enabled, true));
        }
        private async Task ReloadAsync()
        {
            if (busy || departments == null || IsDisposed) return;
            string[] selectedDepartments = SelectedDepartments();
            string userId = UserId;
            busy = true;
            UpdateButtons();
            try
            {
                var result = await Task.Run(() =>
                {
                    DataTable combined = null;
                    foreach (string dept in selectedDepartments)
                    {
                        var data = bus.GetDevices(userId, dept);
                        if (combined == null) combined = data; else combined.Merge(data);
                    }
                    return combined;
                });
                if (IsDisposed) return;
                devices = result ?? devices?.Clone() ?? new DataTable();
                BindTree();
            }
            catch (Exception ex) { ShowError(ex); }
            finally { busy = false; if (!IsDisposed) UpdateButtons(); }
        }
        private void BindTree()
        {
            int selectedId = SelectedDevice() == null ? 0 : Convert.ToInt32(SelectedDevice()["Id"]);
            var collapsed = new HashSet<string>(treeData.Nodes.Cast<DevExpress.XtraTreeList.Nodes.TreeListNode>()
                .Where(n => !n.Expanded).Select(n => n.GetValue("DisplayName").ToString()));
            var table = new DataTable();
            table.Columns.Add("NodeId", typeof(int)); table.Columns.Add("ParentId", typeof(int)); table.Columns.Add("IdDevice", typeof(int));
            foreach (string col in new[] { "DisplayName", "SN", "Temperature", "Humidity", "TempRange", "HumidityRange", "MeasuredAt", "ManagerName", "Status" })
                table.Columns.Add(col);
            rows.Clear();
            int next = 1;
            DateTime now = dt208_EnvironmentRules.VietnamNow;
            int abnormal = 0, stale = 0, noData = 0;
            foreach (var group in devices.AsEnumerable().GroupBy(r => new { Department = r["IdDept"].ToString(), Location = r["Location"].ToString() }))
            {
                int parent = next++;
                var children = new List<DataRow>();
                foreach (DataRow row in group)
                {
                    string status = dt208_EnvironmentRules.DeviceStatus(row, now);
                    if (status.Contains("超限")) abnormal++;
                    if (status.Contains("逾時")) stale++;
                    if (status == "尚無資料") noData++;
                    string filter = cbbStatus.EditValue?.ToString() ?? "全部";
                    if (filter == "使用中" && Convert.ToInt32(row["DeviceState"]) != 0) continue;
                    if (filter == "超限" && !status.Contains("超限")) continue;
                    if (filter == "資料逾時" && !status.Contains("逾時")) continue;
                    if (filter == "尚無資料" && status != "尚無資料") continue;
                    var child = table.NewRow();
                    child["NodeId"] = next++; child["ParentId"] = parent; child["IdDevice"] = row["Id"];
                    child["DisplayName"] = row["DisplayName"]; child["SN"] = row["SN"];
                    child["Temperature"] = row.IsNull("Temperature") ? "—" : $"{row["Temperature"]:0.##} °C";
                    child["Humidity"] = row.IsNull("Humidity") ? "—" : $"{row["Humidity"]:0.##} %RH";
                    child["TempRange"] = dt208_EnvironmentRules.Range((decimal)row["TempMin"], (decimal)row["TempMax"], "°C");
                    child["HumidityRange"] = dt208_EnvironmentRules.Range((decimal)row["HumidityMin"], (decimal)row["HumidityMax"], "%RH");
                    child["MeasuredAt"] = row.IsNull("MeasuredAt") ? "—" : ((DateTime)row["MeasuredAt"]).ToString("yyyy/MM/dd HH:mm:ss");
                    child["ManagerName"] = row["ManagerName"]; child["Status"] = status;
                    rows[Convert.ToInt32(row["Id"])] = row;
                    children.Add(child);
                }
                if (children.Count == 0) continue;
                var location = table.NewRow(); location["NodeId"] = parent; location["ParentId"] = 0; location["IdDevice"] = 0;
                location["DisplayName"] = SelectedDepartments().Length > 1 ? $"{group.Key.Department}／{group.Key.Location}" : group.Key.Location; table.Rows.Add(location);
                foreach (var child in children) table.Rows.Add(child);
            }
            treeData.BeginUpdate();
            try
            {
                treeData.DataSource = table;
                treeData.ExpandAll();
                foreach (DevExpress.XtraTreeList.Nodes.TreeListNode node in treeData.Nodes)
                    if (collapsed.Contains(node.GetValue("DisplayName").ToString())) node.Expanded = false;
                var selected = treeData.FindNodeByFieldValue("IdDevice", selectedId);
                if (selectedId > 0 && selected != null) treeData.FocusedNode = selected;
            }
            finally { treeData.EndUpdate(); }
            lblStatus.Caption = $"設備 {devices.Rows.Count} | 超限 {abnormal} | 逾時 {stale} | 尚無資料 {noData} | 更新 {now:HH:mm:ss} (UTC+07)";
            UpdateButtons();
        }
        private async void AddDevice_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { await EditDeviceAsync(null); }
        private async void EditDevice_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { await EditDeviceAsync(SelectedDevice()); }
        private async Task EditDeviceAsync(DataRow row)
        {
            string department = row == null ? NewDeviceDepartment() : row["IdDept"].ToString();
            if (busy || department == null || !CanEdit(department)) return;
            busy = true; UpdateButtons();
            try
            {
                var managers = await Task.Run(() => bus.GetManagers());
                if (IsDisposed) return;
                using (var form = new f208_DeviceInfo(UserId, departments, managers, department, row))
                    if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
            }
            catch (Exception ex) { ShowError(ex); }
            finally { busy = false; if (!IsDisposed) UpdateButtons(); }
            await ReloadAsync();
        }
        private void History_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenHistory(); }
        private void OpenHistory()
        {
            var row = SelectedDevice();
            if (busy || row == null) return;
            using (var form = new f208_ReadingHistory(UserId, row["SN"].ToString())) form.ShowDialog(FindForm());
        }
        private async void ConfigHistory_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var row = SelectedDevice();
            if (busy || row == null) return;
            await ShowTableAsync("設備設定變更紀錄", () => bus.GetConfigurationHistory(UserId, Convert.ToInt32(row["Id"])));
        }
        private async void Unknown_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            await ShowTableAsync("未登記設備 — 請依設備序號新增設備", () => bus.GetUnknownDevices(UserId));
        }
        private async Task ShowTableAsync(string title, Func<DataTable> load)
        {
            if (busy) return;
            busy = true; UpdateButtons();
            try
            {
                var data = await Task.Run(load);
                if (IsDisposed) return;
                using (var form = new f208_DataList(title, data)) form.ShowDialog(FindForm());
            }
            catch (Exception ex) { ShowError(ex); }
            finally { busy = false; if (!IsDisposed) UpdateButtons(); }
        }
        private async void Reload_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { await ReloadAsync(); }
        private async void Department_Changed(object sender, EventArgs e) { await ReloadAsync(); }
        private void Status_Changed(object sender, EventArgs e) { if (devices != null && !busy) BindTree(); }
        private async void Refresh_Tick(object sender, EventArgs e) { if (Visible && Parent != null) await ReloadAsync(); }
        private void Export_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "溫濕度設備.xlsx" })
                if (dialog.ShowDialog() == DialogResult.OK) try { treeData.ExportToXlsx(dialog.FileName); } catch (Exception ex) { ShowError(ex); }
        }
        private void MonthlyReport_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (busy || devices == null || devices.Rows.Count == 0) return;
            using (var form = new f208_MonthlyReport(UserId, devices)) form.ShowDialog(FindForm());
        }
        private void ShowError(Exception ex) { if (!IsDisposed) MsgTP.MsgError("溫濕度資料處理失敗：\r\n" + ex.Message); }
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (refreshTimer == null) return;
            if (Parent == null) refreshTimer.Stop(); else if (loaded && departments?.Rows.Count > 0) refreshTimer.Start();
        }
    }
}
