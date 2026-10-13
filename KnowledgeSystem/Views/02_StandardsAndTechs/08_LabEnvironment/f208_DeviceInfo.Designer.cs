namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_DeviceInfo
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TableLayoutPanel contentPanel;
        private DevExpress.XtraEditors.TextEdit txbSN, txbName, txbModel, txbLocation;
        private DevExpress.XtraEditors.MemoEdit txbReason;
        private DevExpress.XtraEditors.LookUpEdit cbbDept, cbbManager;
        private DevExpress.XtraEditors.ComboBoxEdit cbbState, temperatureFormat, humidityFormat;
        private DevExpress.XtraEditors.SpinEdit tempMin, tempMax, humidityMin, humidityMax, expectedMinutes;
        private DevExpress.XtraEditors.LabelControl lblTempMin, lblTempMax, lblHumidityMin, lblHumidityMax;
        private DevExpress.XtraEditors.SimpleButton btnSave, btnCancel;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            contentPanel = new System.Windows.Forms.TableLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 2, RowCount = 13, Padding = new System.Windows.Forms.Padding(16), AutoScroll = true };
            contentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140));
            contentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            txbSN = new DevExpress.XtraEditors.TextEdit(); txbSN.Properties.MaxLength = 100;
            txbName = new DevExpress.XtraEditors.TextEdit(); txbName.Properties.MaxLength = 200;
            txbModel = new DevExpress.XtraEditors.TextEdit(); txbModel.Properties.MaxLength = 100;
            txbLocation = new DevExpress.XtraEditors.TextEdit(); txbLocation.Properties.MaxLength = 200;
            cbbDept = CreateLookup(); cbbManager = CreateLookup();
            cbbState = new DevExpress.XtraEditors.ComboBoxEdit(); cbbState.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            temperatureFormat = CreateFormat(); humidityFormat = CreateFormat();
            tempMin = CreateNumber(-999999, 999999, false); tempMax = CreateNumber(-999999, 999999, false);
            humidityMin = CreateNumber(0, 100, false); humidityMax = CreateNumber(0, 100, false);
            expectedMinutes = CreateNumber(1, 10080, true);
            lblTempMin = new DevExpress.XtraEditors.LabelControl { Text = "下限" }; lblTempMax = new DevExpress.XtraEditors.LabelControl { Text = "上限" };
            lblHumidityMin = new DevExpress.XtraEditors.LabelControl { Text = "下限" }; lblHumidityMax = new DevExpress.XtraEditors.LabelControl { Text = "上限" };
            txbReason = new DevExpress.XtraEditors.MemoEdit { Height = 65 }; txbReason.Properties.MaxLength = 500;
            AddField(0, "SN *", txbSN); AddField(1, "設備名稱 *", txbName); AddField(2, "型號", txbModel);
            AddField(3, "部門 *", cbbDept); AddField(4, "位置 / 房間 *", txbLocation); AddField(5, "管理人 *", cbbManager);
            AddField(6, "溫度管制 °C", CreateRangePanel(temperatureFormat, lblTempMin, tempMin, lblTempMax, tempMax));
            AddField(7, "濕度管制 %RH", CreateRangePanel(humidityFormat, lblHumidityMin, humidityMin, lblHumidityMax, humidityMax));
            AddField(8, "資料逾時 (分鐘)", expectedMinutes); AddField(9, "設備狀態", cbbState); AddField(10, "變更原因 *", txbReason);
            var note = new DevExpress.XtraEditors.LabelControl { Text = "設定保存後立即生效；歷史數據保留當時的管制範圍。", AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical, Dock = System.Windows.Forms.DockStyle.Fill };
            contentPanel.Controls.Add(note, 0, 11); contentPanel.SetColumnSpan(note, 2);
            var buttons = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Bottom, Height = 50, FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft, Padding = new System.Windows.Forms.Padding(8) };
            btnSave = new DevExpress.XtraEditors.SimpleButton { Text = "保存", Width = 100, Height = 32 }; btnSave.Click += Save_Click;
            btnCancel = new DevExpress.XtraEditors.SimpleButton { Text = "取消", Width = 100, Height = 32, DialogResult = System.Windows.Forms.DialogResult.Cancel };
            buttons.Controls.Add(btnCancel); buttons.Controls.Add(btnSave);
            Controls.Add(contentPanel); Controls.Add(buttons);
            AcceptButton = btnSave; CancelButton = btnCancel;
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(720, 610); MinimumSize = new System.Drawing.Size(700, 620);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; MaximizeBox = false; MinimizeBox = false; Name = "f208_DeviceInfo";
        }
        private void AddField(int row, string caption, System.Windows.Forms.Control control)
        {
            contentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            contentPanel.Controls.Add(new DevExpress.XtraEditors.LabelControl { Text = caption, Anchor = System.Windows.Forms.AnchorStyles.Left, Margin = new System.Windows.Forms.Padding(3, 10, 3, 10) }, 0, row);
            control.Dock = System.Windows.Forms.DockStyle.Fill; control.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6); contentPanel.Controls.Add(control, 1, row);
        }
        private static DevExpress.XtraEditors.LookUpEdit CreateLookup()
        {
            var control = new DevExpress.XtraEditors.LookUpEdit();
            control.Properties.ValueMember = "Id"; control.Properties.DisplayMember = "DisplayName"; control.Properties.NullText = "請選擇";
            control.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "代碼"));
            control.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "名稱"));
            control.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            return control;
        }
        private static DevExpress.XtraEditors.SpinEdit CreateNumber(decimal min, decimal max, bool integer)
        {
            var control = new DevExpress.XtraEditors.SpinEdit(); control.Properties.MinValue = min; control.Properties.MaxValue = max;
            control.Properties.IsFloatValue = !integer; control.Properties.Mask.EditMask = integer ? "d" : "f2"; return control;
        }
        private static DevExpress.XtraEditors.ComboBoxEdit CreateFormat()
        {
            var control = new DevExpress.XtraEditors.ComboBoxEdit { Width = 125 };
            control.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            control.Properties.Items.AddRange(new object[] { "上下限", "中心 ± 公差" }); return control;
        }
        private static System.Windows.Forms.Control CreateRangePanel(DevExpress.XtraEditors.ComboBoxEdit format, DevExpress.XtraEditors.LabelControl firstLabel, DevExpress.XtraEditors.SpinEdit first, DevExpress.XtraEditors.LabelControl secondLabel, DevExpress.XtraEditors.SpinEdit second)
        {
            var panel = new System.Windows.Forms.FlowLayoutPanel { AutoSize = true, WrapContents = false };
            first.Width = second.Width = 95; firstLabel.Margin = secondLabel.Margin = new System.Windows.Forms.Padding(6, 8, 3, 3);
            panel.Controls.AddRange(new System.Windows.Forms.Control[] { format, firstLabel, first, secondLabel, second }); return panel;
        }
    }
}
