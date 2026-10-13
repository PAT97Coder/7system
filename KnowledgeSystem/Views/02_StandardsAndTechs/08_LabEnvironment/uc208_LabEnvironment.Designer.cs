namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class uc208_LabEnvironment
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.BarEditItem cbbDept, cbbStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repoDept;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoStatus;
        private DevExpress.XtraBars.BarButtonItem btnAdd, btnEdit, btnHistory, btnConfigHistory, btnReload, btnExport, btnUnknown;
        private DevExpress.XtraBars.BarStaticItem lblStatus;
        private DevExpress.XtraTreeList.TreeList treeData;
        private System.Windows.Forms.Timer refreshTimer;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            barManager = new DevExpress.XtraBars.BarManager(components) { Form = this };
            var toolbar = new DevExpress.XtraBars.Bar(barManager, "工具列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Top };
            var status = new DevExpress.XtraBars.Bar(barManager, "狀態列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom };
            barManager.StatusBar = status;
            foreach (var dock in new[] { System.Windows.Forms.DockStyle.Top, System.Windows.Forms.DockStyle.Bottom, System.Windows.Forms.DockStyle.Left, System.Windows.Forms.DockStyle.Right })
                Controls.Add(new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = dock });
            repoDept = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit { ValueMember = "Id", DisplayMember = "Id", NullText = "選擇部門", TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor };
            repoDept.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "部門代碼"));
            repoDept.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "部門名稱"));
            repoStatus = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox { TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor };
            repoStatus.Items.AddRange(new object[] { "全部", "使用中", "超限", "資料逾時", "尚無資料" });
            barManager.RepositoryItems.Add(repoDept); barManager.RepositoryItems.Add(repoStatus);
            cbbDept = new DevExpress.XtraBars.BarEditItem(barManager) { Caption = "部門", Edit = repoDept, EditWidth = 100, PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption };
            cbbStatus = new DevExpress.XtraBars.BarEditItem(barManager) { Caption = "狀態", Edit = repoStatus, EditWidth = 105, EditValue = "全部", PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption };
            cbbDept.EditValueChanged += Department_Changed; cbbStatus.EditValueChanged += Status_Changed;
            toolbar.AddItem(cbbDept); toolbar.AddItem(cbbStatus);
            btnAdd = new DevExpress.XtraBars.BarButtonItem(barManager, "新增設備"); btnAdd.ImageOptions.SvgImage = Helpers.TPSvgimages.Add;
            btnEdit = new DevExpress.XtraBars.BarButtonItem(barManager, "編輯設備"); btnEdit.ImageOptions.SvgImage = Helpers.TPSvgimages.Edit;
            btnHistory = new DevExpress.XtraBars.BarButtonItem(barManager, "歷史與圖表");
            btnConfigHistory = new DevExpress.XtraBars.BarButtonItem(barManager, "設定紀錄");
            btnReload = new DevExpress.XtraBars.BarButtonItem(barManager, "更新"); btnReload.ImageOptions.SvgImage = Helpers.TPSvgimages.Reload;
            btnExport = new DevExpress.XtraBars.BarButtonItem(barManager, "匯出Excel"); btnExport.ImageOptions.SvgImage = Helpers.TPSvgimages.Excel;
            btnUnknown = new DevExpress.XtraBars.BarButtonItem(barManager, "未登記SN") { Visibility = DevExpress.XtraBars.BarItemVisibility.Never };
            btnAdd.ItemClick += AddDevice_Click; btnEdit.ItemClick += EditDevice_Click; btnHistory.ItemClick += History_Click;
            btnConfigHistory.ItemClick += ConfigHistory_Click; btnReload.ItemClick += Reload_Click; btnExport.ItemClick += Export_Click; btnUnknown.ItemClick += Unknown_Click;
            foreach (var button in new[] { btnAdd, btnEdit, btnHistory, btnConfigHistory, btnReload, btnExport, btnUnknown }) { button.Enabled = false; button.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph; toolbar.AddItem(button); }
            lblStatus = new DevExpress.XtraBars.BarStaticItem { Caption = "載入中…" }; barManager.Items.Add(lblStatus); status.AddItem(lblStatus);
            treeData = new DevExpress.XtraTreeList.TreeList { Dock = System.Windows.Forms.DockStyle.Fill, KeyFieldName = "NodeId", ParentFieldName = "ParentId" };
            treeData.OptionsBehavior.Editable = false; treeData.OptionsView.ShowAutoFilterRow = true;
            treeData.OptionsView.AutoWidth = false;
            treeData.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            treeData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            treeData.RowHeight = 32;
            treeData.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn { FieldName = "IdDevice", Visible = false });
            string[] fields = { "DisplayName", "SN", "Temperature", "Humidity", "TempRange", "HumidityRange", "MeasuredAt", "ManagerName", "Status" };
            string[] captions = { "位置 / 設備", "SN", "溫度", "濕度", "溫度管制範圍", "濕度管制範圍", "測量時間", "管理人", "狀態" };
            for (int i = 0; i < fields.Length; i++) treeData.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn { FieldName = fields[i], Caption = captions[i], Visible = true, VisibleIndex = i, Width = i == 0 || i == 4 || i == 5 || i == 6 || i == 8 ? 240 : 130 });
            treeData.FocusedNodeChanged += (s, e) => UpdateButtons();
            treeData.DoubleClick += (s, e) => OpenHistory();
            treeData.NodeCellStyle += (s, e) =>
            {
                string state = e.Node.GetValue("Status")?.ToString() ?? "";
                if (state.Contains("超限") || state.Contains("衝突")) e.Appearance.ForeColor = System.Drawing.Color.Firebrick;
                else if (state.Contains("逾時")) e.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
                else if (state == "尚無資料" || state == "停用" || state == "故障") e.Appearance.ForeColor = System.Drawing.Color.Gray;
            };
            Controls.Add(treeData); treeData.BringToFront();
            refreshTimer = new System.Windows.Forms.Timer(components) { Interval = 60000 }; refreshTimer.Tick += Refresh_Tick;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            Name = "uc208_LabEnvironment"; Size = new System.Drawing.Size(1250, 700);
            Load += Module_Load;
        }
    }
}
