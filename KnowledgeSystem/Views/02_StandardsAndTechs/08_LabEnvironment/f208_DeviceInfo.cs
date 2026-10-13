using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public partial class f208_DeviceInfo : XtraForm
    {
        private readonly string userId;
        private readonly int deviceId;
        private readonly long configId;
        private bool saving;
        private int temperatureMode, humidityMode;
        public f208_DeviceInfo(string userId, DataTable departments, DataTable managers, string department, DataRow current)
        {
            InitializeComponent();
            this.userId = userId;
            var editable = departments.Clone();
            foreach (DataRow row in departments.Rows) if (Convert.ToBoolean(row["CanEdit"])) editable.ImportRow(row);
            cbbDept.Properties.DataSource = editable;
            cbbManager.Properties.DataSource = managers;
            cbbDept.EditValue = department;
            cbbState.Properties.Items.AddRange(dt208_EnvironmentRules.DeviceStates);
            cbbState.SelectedIndex = 0;
            tempMin.Value = 15; tempMax.Value = 25; humidityMin.Value = 50; humidityMax.Value = 70; expectedMinutes.Value = 60;
            if (current != null)
            {
                deviceId = Convert.ToInt32(current["Id"]); configId = Convert.ToInt64(current["ConfigId"]);
                txbSN.Text = current["SN"].ToString(); txbSN.Enabled = false;
                txbName.Text = current["DisplayName"].ToString(); txbModel.Text = current["Model"].ToString();
                cbbDept.EditValue = current["IdDept"]; txbLocation.Text = current["Location"].ToString(); cbbManager.EditValue = current["IdManager"];
                tempMin.Value = (decimal)current["TempMin"]; tempMax.Value = (decimal)current["TempMax"];
                humidityMin.Value = (decimal)current["HumidityMin"]; humidityMax.Value = (decimal)current["HumidityMax"];
                expectedMinutes.Value = Convert.ToInt32(current["ExpectedMinutes"]); cbbState.SelectedIndex = Convert.ToInt32(current["DeviceState"]);
            }
            else txbReason.Text = "新增設備";
            temperatureFormat.SelectedIndex = 0; humidityFormat.SelectedIndex = 0;
            temperatureFormat.SelectedIndexChanged += (s, e) => ChangeRangeMode(temperatureFormat.SelectedIndex, ref temperatureMode, tempMin, tempMax, lblTempMin, lblTempMax);
            humidityFormat.SelectedIndexChanged += (s, e) => ChangeRangeMode(humidityFormat.SelectedIndex, ref humidityMode, humidityMin, humidityMax, lblHumidityMin, lblHumidityMax);
            Text = deviceId == 0 ? "新增溫濕度設備" : "編輯溫濕度設備";
            FormClosing += (s, e) => { if (saving) e.Cancel = true; };
        }
        private static void ChangeRangeMode(int mode, ref int previous, SpinEdit first, SpinEdit second, LabelControl firstLabel, LabelControl secondLabel)
        {
            if (mode == previous) return;
            decimal a = first.Value, b = second.Value;
            if (mode == 1) { first.Value = (a + b) / 2; second.Value = (b - a) / 2; }
            else { first.Value = a - b; second.Value = a + b; }
            firstLabel.Text = mode == 1 ? "中心值" : "下限"; secondLabel.Text = mode == 1 ? "公差 ±" : "上限";
            previous = mode;
        }
        private async void Save_Click(object sender, EventArgs e)
        {
            if (saving) return;
            var edit = new dt208_DeviceEdit
            {
                Id = deviceId, ExpectedConfigId = configId, SN = txbSN.Text.Trim().ToUpperInvariant(), DisplayName = txbName.Text.Trim(), Model = txbModel.Text.Trim(),
                IdDept = cbbDept.EditValue?.ToString(), Location = txbLocation.Text.Trim(), IdManager = cbbManager.EditValue?.ToString(),
                TempMin = temperatureMode == 0 ? tempMin.Value : tempMin.Value - tempMax.Value,
                TempMax = temperatureMode == 0 ? tempMax.Value : tempMin.Value + tempMax.Value,
                HumidityMin = humidityMode == 0 ? humidityMin.Value : humidityMin.Value - humidityMax.Value,
                HumidityMax = humidityMode == 0 ? humidityMax.Value : humidityMin.Value + humidityMax.Value,
                ExpectedMinutes = (int)expectedMinutes.Value, DeviceState = cbbState.SelectedIndex, Reason = txbReason.Text.Trim()
            };
            string error = dt208_EnvironmentRules.Validate(edit);
            if (error != null) { MsgTP.MsgError(error); return; }
            saving = true; contentPanel.Enabled = false; btnSave.Enabled = false; btnCancel.Enabled = false;
            bool saved = false;
            try { await Task.Run(() => dt208_EnvironmentBUS.Instance.SaveDevice(userId, edit)); saved = true; }
            catch (Exception ex) { MsgTP.MsgError("設備保存失敗：\r\n" + ex.Message); }
            finally { saving = false; contentPanel.Enabled = true; btnSave.Enabled = true; btnCancel.Enabled = true; }
            if (saved) { DialogResult = DialogResult.OK; Close(); }
        }
    }
}
