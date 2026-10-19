namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_MonthlyReport
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar toolbar;
        private DevExpress.XtraBars.BarButtonItem btnExport, btnCancel;
        private DevExpress.XtraBars.BarDockControl dockTop, dockBottom, dockLeft, dockRight;
        private DevExpress.XtraLayout.LayoutControl contentPanel;
        private DevExpress.XtraLayout.LayoutControlGroup layoutRoot;
        private DevExpress.XtraEditors.DateEdit monthEdit;
        private DevExpress.XtraEditors.LookUpEdit roomEdit, deviceEdit;
        private DevExpress.XtraEditors.LabelControl lblRule;
        private DevExpress.XtraLayout.LayoutControlItem monthItem, roomItem, deviceItem, ruleItem;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpace;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            SuspendLayout();
            components = new System.ComponentModel.Container();
            barManager = new DevExpress.XtraBars.BarManager(components) { Form = this };
            toolbar = new DevExpress.XtraBars.Bar(barManager, "工具列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Top };
            Lab208Ui.StyleDialogToolbar(toolbar); barManager.MainMenu = toolbar;
            dockTop = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Top };
            dockBottom = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Bottom };
            dockLeft = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Left };
            dockRight = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Right };
            Controls.AddRange(new System.Windows.Forms.Control[] { dockTop, dockBottom, dockLeft, dockRight });
            btnExport = new DevExpress.XtraBars.BarButtonItem(barManager, "匯出月檢查表") { Enabled = false };
            btnCancel = new DevExpress.XtraBars.BarButtonItem(barManager, "關閉");
            btnExport.ImageOptions.SvgImage = Helpers.TPSvgimages.Excel; btnCancel.ImageOptions.SvgImage = Helpers.TPSvgimages.Cancel;
            Lab208Ui.StyleDialogBarItem(btnExport); Lab208Ui.StyleDialogBarItem(btnCancel);
            btnExport.ItemClick += (s, e) => Export_Click(s, e);
            btnCancel.ItemClick += (s, e) => Close();
            toolbar.AddItem(btnExport); toolbar.AddItem(btnCancel).BeginGroup = true;
            contentPanel = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            layoutRoot = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false, EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True, Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10) };
            ((System.ComponentModel.ISupportInitialize)contentPanel).BeginInit(); contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutRoot).BeginInit(); contentPanel.Root = layoutRoot;
            monthEdit = new DevExpress.XtraEditors.DateEdit();
            monthEdit.Properties.DisplayFormat.FormatType = monthEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            monthEdit.Properties.DisplayFormat.FormatString = monthEdit.Properties.EditFormat.FormatString = "yyyy/MM";
            monthEdit.Properties.Mask.EditMask = "yyyy/MM";
            monthEdit.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
            monthEdit.EditValueChanged += (s, e) => UpdateExportButton();
            roomEdit = new DevExpress.XtraEditors.LookUpEdit();
            roomEdit.Properties.ValueMember = "Id"; roomEdit.Properties.DisplayMember = "Caption";
            roomEdit.Properties.NullText = "請選擇實驗室";
            roomEdit.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "實驗室"));
            roomEdit.EditValueChanged += Room_Changed;
            deviceEdit = new DevExpress.XtraEditors.LookUpEdit();
            deviceEdit.Properties.ValueMember = "Id"; deviceEdit.Properties.DisplayMember = "SN";
            deviceEdit.Properties.NullText = "請選擇設備序號";
            deviceEdit.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SN", "設備序號"));
            deviceEdit.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "設備名稱"));
            deviceEdit.EditValueChanged += (s, e) => UpdateExportButton();
            foreach (var editor in new DevExpress.XtraEditors.BaseEdit[] { monthEdit, roomEdit, deviceEdit })
                Lab208Ui.StyleDialogEditor(editor, contentPanel, barManager);
            lblRule = new DevExpress.XtraEditors.LabelControl { Text = "每日一列：08:00／16:00，取前後30分鐘內最近量測。\r\n無量測則留白；檢查人及主管簽核欄位留供人工填寫。\r\n依所選實驗室及設備篩選，並附實際量測時間與超限處理明細。" };
            lblRule.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F); lblRule.Appearance.Options.UseFont = true;
            lblRule.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            monthItem = new DevExpress.XtraLayout.LayoutControlItem { Control = monthEdit, Text = "月份<color=red>*</color>", Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(636, 40), AllowHtmlStringInCaption = true };
            roomItem = new DevExpress.XtraLayout.LayoutControlItem { Control = roomEdit, Text = "實驗室<color=red>*</color>", Location = new System.Drawing.Point(0, 40), Size = new System.Drawing.Size(636, 40), AllowHtmlStringInCaption = true };
            deviceItem = new DevExpress.XtraLayout.LayoutControlItem { Control = deviceEdit, Text = "設備序號<color=red>*</color>", Location = new System.Drawing.Point(0, 80), Size = new System.Drawing.Size(636, 40), AllowHtmlStringInCaption = true };
            ruleItem = new DevExpress.XtraLayout.LayoutControlItem { Control = lblRule, TextVisible = false, Location = new System.Drawing.Point(0, 120), Size = new System.Drawing.Size(636, 90) };
            emptySpace = new DevExpress.XtraLayout.EmptySpaceItem { Location = new System.Drawing.Point(0, 210), Size = new System.Drawing.Size(636, 10) };
            foreach (var item in new[] { monthItem, roomItem, deviceItem, ruleItem }) Lab208Ui.StyleDialogItem(item);
            contentPanel.Controls.AddRange(new System.Windows.Forms.Control[] { monthEdit, roomEdit, deviceEdit, lblRule });
            layoutRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { monthItem, roomItem, deviceItem, ruleItem, emptySpace });
            Controls.Add(contentPanel); contentPanel.BringToFront();
            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244); Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 8.25F);
            ClientSize = new System.Drawing.Size(660, 320);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; MaximizeBox = false; MinimizeBox = false;
            IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            Name = "f208_MonthlyReport"; Text = "每月實驗室溫濕度檢查紀錄表";
            ((System.ComponentModel.ISupportInitialize)layoutRoot).EndInit(); ((System.ComponentModel.ISupportInitialize)contentPanel).EndInit();
            contentPanel.ResumeLayout(false); ResumeLayout(false); PerformLayout();
        }
    }
}
