namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class uc208_LabEnvironment
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarDockControl dockTop, dockBottom, dockLeft, dockRight;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.BarEditItem cbbDept, cbbStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repoDept;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoStatus;
        private DevExpress.XtraBars.BarButtonItem btnAdd, btnEdit, btnHistory, btnConfigHistory, btnReload, btnExport, btnUnknown, btnMonthlyReport;
        private DevExpress.XtraBars.BarStaticItem lblStatus;
        private DevExpress.XtraTreeList.TreeList treeData;
        private System.Windows.Forms.Timer refreshTimer;
        private DevExpress.XtraLayout.LayoutControl contentLayout;
        private DevExpress.XtraLayout.LayoutControlGroup layoutRoot;
        private DevExpress.XtraLayout.LayoutControlItem treeItem;
        private DevExpress.XtraBars.Bar toolbar, status;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDeviceId, colDisplayName, colSN, colTemperature, colHumidity, colTempRange, colHumidityRange, colMeasuredAt, colManagerName, colStatus;
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            SuspendLayout();
            components = new System.ComponentModel.Container();
            barManager = new DevExpress.XtraBars.BarManager(components) { Form = this };
            toolbar = new DevExpress.XtraBars.Bar(barManager, "工具列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Top };
            status = new DevExpress.XtraBars.Bar(barManager, "狀態列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom };
            toolbar.OptionsBar.AllowQuickCustomization = false;
            Lab208Ui.StyleToolbar(toolbar);
            barManager.MainMenu = toolbar;
            barManager.StatusBar = status;
            status.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            status.BarAppearance.Normal.Options.UseFont = true;
            dockTop = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Top };
            dockBottom = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Bottom };
            dockLeft = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Left };
            dockRight = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Right };
            Controls.AddRange(new System.Windows.Forms.Control[] { dockTop, dockBottom, dockLeft, dockRight });
            repoDept = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit { NullText = "選擇部門", SelectAllItemCaption = "全選", SeparatorChar = ',', TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor };
            repoStatus = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox { TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor };
            repoStatus.Items.AddRange(new object[] { "全部", "使用中", "超限", "資料逾時", "尚無資料" });
            barManager.RepositoryItems.Add(repoDept); barManager.RepositoryItems.Add(repoStatus);
            cbbDept = new DevExpress.XtraBars.BarEditItem(barManager) { Caption = "實驗室", Edit = repoDept, EditWidth = 150, PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption };
            cbbStatus = new DevExpress.XtraBars.BarEditItem(barManager) { Caption = "狀態", Edit = repoStatus, EditWidth = 105, EditValue = "全部", PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption };
            cbbDept.EditValueChanged += Department_Changed; cbbStatus.EditValueChanged += Status_Changed;
            cbbDept.Enabled = cbbStatus.Enabled = false;
            btnAdd = new DevExpress.XtraBars.BarButtonItem(barManager, "新增設備"); btnAdd.ImageOptions.SvgImage = Helpers.TPSvgimages.Add;
            btnEdit = new DevExpress.XtraBars.BarButtonItem(barManager, "編輯設備"); btnEdit.ImageOptions.SvgImage = Helpers.TPSvgimages.Edit;
            btnHistory = new DevExpress.XtraBars.BarButtonItem(barManager, "歷史與圖表");
            btnConfigHistory = new DevExpress.XtraBars.BarButtonItem(barManager, "設定紀錄");
            btnReload = new DevExpress.XtraBars.BarButtonItem(barManager, "刷新"); btnReload.ImageOptions.SvgImage = Helpers.TPSvgimages.Reload;
            btnExport = new DevExpress.XtraBars.BarButtonItem(barManager, "出表"); btnExport.ImageOptions.SvgImage = Helpers.TPSvgimages.Excel;
            btnUnknown = new DevExpress.XtraBars.BarButtonItem(barManager, "未登記設備序號") { Visibility = DevExpress.XtraBars.BarItemVisibility.Never };
            btnAdd.ItemClick += AddDevice_Click; btnEdit.ItemClick += EditDevice_Click; btnHistory.ItemClick += History_Click;
            btnConfigHistory.ItemClick += ConfigHistory_Click; btnReload.ItemClick += Reload_Click; btnExport.ItemClick += Export_Click; btnUnknown.ItemClick += Unknown_Click;
            btnUnknown.ImageOptions.SvgImage = Helpers.TPSvgimages.Search;
            foreach (var button in new[] { btnAdd, btnEdit, btnHistory, btnConfigHistory, btnReload, btnExport, btnUnknown }) { button.Enabled = false; Lab208Ui.StyleBarItem(button); }
            toolbar.AddItem(btnAdd);
            toolbar.AddItem(btnReload).BeginGroup = true;
            toolbar.AddItem(btnExport).BeginGroup = true;
            btnMonthlyReport = new DevExpress.XtraBars.BarButtonItem(barManager, "月檢查表") { Enabled = false };
            btnMonthlyReport.ImageOptions.SvgImage = Helpers.TPSvgimages.Excel;
            Lab208Ui.StyleBarItem(btnMonthlyReport); btnMonthlyReport.ItemClick += MonthlyReport_Click;
            toolbar.AddItem(cbbDept).BeginGroup = true;
            toolbar.AddItem(cbbStatus).BeginGroup = true;
            toolbar.AddItem(btnUnknown).BeginGroup = true;
            toolbar.AddItem(btnMonthlyReport).BeginGroup = true;
            Lab208Ui.StyleBarItem(cbbDept); Lab208Ui.StyleBarItem(cbbStatus);
            cbbDept.EditWidth = cbbStatus.EditWidth = 150;
            cbbDept.PaintStyle = cbbStatus.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            Lab208Ui.StyleRepository(repoDept); Lab208Ui.StyleRepository(repoStatus);
            repoDept.AutoHeight = repoStatus.AutoHeight = false;
            lblStatus = new DevExpress.XtraBars.BarStaticItem { Caption = "載入中…" }; barManager.Items.Add(lblStatus); status.AddItem(lblStatus);
            treeData = new DevExpress.XtraTreeList.TreeList { Dock = System.Windows.Forms.DockStyle.Fill, KeyFieldName = "NodeId", ParentFieldName = "ParentId" };
            treeData.OptionsBehavior.Editable = false; treeData.OptionsView.ShowAutoFilterRow = true;
            treeData.OptionsView.AutoWidth = false;
            treeData.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            treeData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            treeData.Appearance.Row.Options.UseFont = true;
            treeData.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            treeData.Appearance.HeaderPanel.Options.UseFont = true;
            treeData.Appearance.HeaderPanel.Options.UseForeColor = true;
            treeData.Appearance.HeaderPanel.Options.UseTextOptions = true;
            treeData.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            treeData.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            treeData.MenuManager = barManager;
            treeData.OptionsBehavior.PopulateServiceColumns = true;
            treeData.OptionsFilter.ExpandNodesOnFiltering = true;
            treeData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            treeData.OptionsSelection.SelectNodesOnRightClick = true;
            treeData.OptionsView.EnableAppearanceOddRow = true;
            colDeviceId = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colDeviceId", FieldName = "IdDevice", Visible = false };
            colDisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colDisplayName", FieldName = "DisplayName", Caption = "位置／設備", Visible = true, VisibleIndex = 0, Width = 240, MaxWidth = 300 };
            colSN = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colSN", FieldName = "SN", Caption = "設備序號", Visible = true, VisibleIndex = 1, Width = 130 };
            colTemperature = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colTemperature", FieldName = "Temperature", Caption = "溫度", Visible = true, VisibleIndex = 2, Width = 130 };
            colHumidity = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colHumidity", FieldName = "Humidity", Caption = "濕度", Visible = true, VisibleIndex = 3, Width = 130 };
            colTempRange = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colTempRange", FieldName = "TempRange", Caption = "溫度管制範圍", Visible = true, VisibleIndex = 4, Width = 240, MaxWidth = 300 };
            colHumidityRange = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colHumidityRange", FieldName = "HumidityRange", Caption = "濕度管制範圍", Visible = true, VisibleIndex = 5, Width = 240, MaxWidth = 300 };
            colMeasuredAt = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colMeasuredAt", FieldName = "MeasuredAt", Caption = "測量時間", Visible = true, VisibleIndex = 6, Width = 240 };
            colManagerName = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colManagerName", FieldName = "ManagerName", Caption = "管理人", Visible = true, VisibleIndex = 7, Width = 130 };
            colStatus = new DevExpress.XtraTreeList.Columns.TreeListColumn { Name = "colStatus", FieldName = "Status", Caption = "狀態", Visible = true, VisibleIndex = 8, Width = 240, MaxWidth = 300 };
            treeData.Columns.AddRange(new[] { colDeviceId, colDisplayName, colSN, colTemperature, colHumidity, colTempRange, colHumidityRange, colMeasuredAt, colManagerName, colStatus });
            colTemperature.AppearanceCell.Options.UseTextOptions = colHumidity.AppearanceCell.Options.UseTextOptions = true;
            colTemperature.AppearanceCell.TextOptions.HAlignment = colHumidity.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            colMeasuredAt.AppearanceCell.Options.UseTextOptions = true;
            colMeasuredAt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            treeData.FocusedNodeChanged += (s, e) => UpdateButtons();
            treeData.DoubleClick += (s, e) => OpenHistory();
            treeData.PopupMenuShowing += Tree_PopupMenuShowing;
            treeData.NodeCellStyle += (s, e) =>
            {
                string state = e.Node.GetValue("Status")?.ToString() ?? "";
                if (state.Contains("超限") || state.Contains("衝突")) e.Appearance.ForeColor = System.Drawing.Color.Firebrick;
                else if (state.Contains("逾時")) e.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
                else if (state == "尚無資料" || state == "停用" || state == "故障") e.Appearance.ForeColor = System.Drawing.Color.Gray;
            };
            contentLayout = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            layoutRoot = new DevExpress.XtraLayout.LayoutControlGroup { EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True, GroupBordersVisible = false };
            treeItem = new DevExpress.XtraLayout.LayoutControlItem { Control = treeData, TextVisible = false };
            contentLayout.Root = layoutRoot; contentLayout.Controls.Add(treeData); layoutRoot.AddItem(treeItem);
            Controls.Add(contentLayout); contentLayout.BringToFront();
            refreshTimer = new System.Windows.Forms.Timer(components) { Interval = 60000 }; refreshTimer.Tick += Refresh_Tick;
            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244);
            Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 8.25F);
            Name = "uc208_LabEnvironment"; Size = new System.Drawing.Size(1037, 585);
            Load += Module_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
