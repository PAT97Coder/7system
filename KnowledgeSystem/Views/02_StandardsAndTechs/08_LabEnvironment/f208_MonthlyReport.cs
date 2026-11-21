using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public partial class f208_MonthlyReport : XtraForm
    {
        private readonly string userId;
        private readonly DataTable devices;
        private readonly DataTable rooms = new DataTable();
        private bool exporting;

        public f208_MonthlyReport(string userId, DataTable devices)
        {
            Lab208Ui.InitializeLocalization(); InitializeComponent();
            this.userId = userId; this.devices = devices.Copy();
            rooms.Columns.Add("Id", typeof(int)); rooms.Columns.Add("IdDept"); rooms.Columns.Add("Location"); rooms.Columns.Add("Caption");
            foreach (var room in this.devices.AsEnumerable().Select(r => new { Department = r["IdDept"].ToString(), Location = r["Location"].ToString() })
                .Distinct().OrderBy(r => r.Department).ThenBy(r => r.Location))
                rooms.Rows.Add(rooms.Rows.Count + 1, room.Department, room.Location, room.Department + "／" + room.Location);
            monthEdit.EditValue = dt208_EnvironmentRules.VietnamNow.Date;
            roomEdit.Properties.DataSource = rooms;
            if (rooms.Rows.Count > 0) roomEdit.EditValue = rooms.Rows[0]["Id"];
            UpdateExportButton();
            FormClosing += (s, e) => { if (exporting) e.Cancel = true; };
        }

        private DataRow SelectedRoom() => rooms.AsEnumerable().FirstOrDefault(r => Equals(r["Id"], roomEdit.EditValue));
        private DataRow SelectedDevice()
        {
            var room = SelectedRoom();
            return room == null ? null : devices.AsEnumerable().FirstOrDefault(r => Equals(r["Id"], deviceEdit.EditValue)
                && r["IdDept"].ToString() == room["IdDept"].ToString() && r["Location"].ToString() == room["Location"].ToString());
        }
        private void Room_Changed(object sender, EventArgs e)
        {
            if (devices == null) return;
            deviceEdit.EditValue = null;
            var room = SelectedRoom();
            var available = devices.Clone();
            if (room != null)
                foreach (var row in devices.AsEnumerable().Where(r => r["IdDept"].ToString() == room["IdDept"].ToString()
                    && r["Location"].ToString() == room["Location"].ToString())) available.ImportRow(row);
            deviceEdit.Properties.DataSource = available;
            if (available.Rows.Count > 0) deviceEdit.EditValue = available.Rows[0]["Id"];
            UpdateExportButton();
        }
        private void UpdateExportButton()
        {
            btnExport.Enabled = !exporting && SelectedDevice() != null && monthEdit.EditValue is DateTime date && date.Year < 9999;
        }
        private async void Export_Click(object sender, EventArgs e)
        {
            var device = SelectedDevice();
            var room = SelectedRoom();
            if (exporting || !btnExport.Enabled || device == null || room == null || !(monthEdit.EditValue is DateTime month)) return;
            month = new DateTime(month.Year, month.Month, 1);
            string sn = device["SN"].ToString(), department = room["IdDept"].ToString(), location = room["Location"].ToString();
            string fileName = $"GA-700-B007-03_{location}_{sn}_{month:yyyyMM}.xls";
            foreach (char invalid in Path.GetInvalidFileNameChars()) fileName = fileName.Replace(invalid, '_');
            using (var save = new SaveFileDialog { Filter = "Excel 97–2003 (*.xls)|*.xls", FileName = fileName })
            {
                if (save.ShowDialog(this) != DialogResult.OK) return;
                exporting = true; contentPanel.Enabled = btnExport.Enabled = btnCancel.Enabled = false;
                try
                {
                    int filled = await Task.Run(() =>
                    {
                        var readings = LoadMonth(month, sn);
                        return Lab208MonthlyExporter.Export(save.FileName, month, department, location, sn, readings);
                    });
                    XtraMessageBox.Show(this, $"已匯出每月檢查紀錄表，共 {filled}／{DateTime.DaysInMonth(month.Year, month.Month) * 2} 次量測。\r\n缺少的量測與檢查人欄位保留空白。", "匯出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MsgTP.MsgError("每月檢查紀錄表匯出失敗：\r\n" + ex.Message); }
                finally { exporting = false; contentPanel.Enabled = btnCancel.Enabled = true; UpdateExportButton(); }
            }
        }
        private DataTable LoadMonth(DateTime month, string sn)
        {
            var bus = dt208_EnvironmentBUS.Instance;
            DateTime end = month.AddMonths(1);
            var readings = bus.GetHistory(userId, sn, month, end);
            if (readings.Rows.Count <= 10000) return readings;
            // Do not export a truncated month when sensors report at high frequency.
            readings.Clear();
            for (DateTime day = month; day < end; day = day.AddDays(1))
            {
                var daily = bus.GetHistory(userId, sn, day, day.AddDays(1));
                if (daily.Rows.Count > 10000) throw new InvalidOperationException($"{day:yyyy/MM/dd} 的量測超過10,000筆，無法完整匯出。");
                readings.Merge(daily);
            }
            return readings;
        }
    }
}
