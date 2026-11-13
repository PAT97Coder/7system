namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_ReadingHistory
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarDockControl dockTop, dockBottom, dockLeft, dockRight;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar toolbar, status;
        private DevExpress.XtraBars.BarEditItem dateFrom, dateTo;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repoDate;
        private DevExpress.XtraBars.BarCheckItem chkAbnormal, chkReportSamples;
        private DevExpress.XtraBars.BarStaticItem lblSummary;
        private DevExpress.XtraBars.BarButtonItem btnLoad, btnAction, btnActionHistory, btnConflicts, btnExport, btnPrint;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repoDeviceState;
        private DevExpress.XtraCharts.ChartControl chartTemperature, chartHumidity;
        private DevExpress.XtraLayout.LayoutControl contentLayout;
        private DevExpress.XtraLayout.LayoutControlGroup layoutRoot, chartGroup;
        private DevExpress.XtraLayout.LayoutControlItem gridItem, temperatureItem, humidityItem;
        private DevExpress.XtraLayout.SplitterItem chartSplitter;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            repoDeviceState = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            for (int i = 0; i < BusinessLayer.dt208_EnvironmentRules.DeviceStates.Length; i++)
                repoDeviceState.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(BusinessLayer.dt208_EnvironmentRules.DeviceStates[i], i));
            Lab208Ui.StyleRepository(repoDeviceState);
            barManager = new DevExpress.XtraBars.BarManager(components) { Form = this };
            toolbar = new DevExpress.XtraBars.Bar(barManager, "工具列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Top };
            toolbar.OptionsBar.AllowQuickCustomization = false;
            Lab208Ui.StyleDialogToolbar(toolbar);
            barManager.MainMenu = toolbar;
            status = new DevExpress.XtraBars.Bar(barManager, "狀態列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom }; barManager.StatusBar = status;
            dockTop = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Top };
            dockBottom = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Bottom };
            dockLeft = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Left };
            dockRight = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Right };
            Controls.AddRange(new System.Windows.Forms.Control[] { dockTop, dockBottom, dockLeft, dockRight });
            repoDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            repoDate.DisplayFormat.FormatType = repoDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repoDate.DisplayFormat.FormatString = repoDate.EditFormat.FormatString = "yyyy/MM/dd"; repoDate.Mask.EditMask = "yyyy/MM/dd";
            Lab208Ui.StyleRepository(repoDate); barManager.RepositoryItems.Add(repoDate);
            dateFrom = new DevExpress.XtraBars.BarEditItem(barManager) { Caption = "起日", Edit = repoDate, EditWidth = 150 };
            dateTo = new DevExpress.XtraBars.BarEditItem(barManager) { Caption = "迄日", Edit = repoDate, EditWidth = 150 };
            btnLoad = new DevExpress.XtraBars.BarButtonItem(barManager, "查詢"); btnLoad.ImageOptions.SvgImage = Helpers.TPSvgimages.Search; btnLoad.ItemClick += (s, e) => Load_Click(s, e);
            chkAbnormal = new DevExpress.XtraBars.BarCheckItem(barManager) { Caption = "僅超限" }; chkAbnormal.CheckedChanged += (s, e) => ApplyFilter();
            chkReportSamples = new DevExpress.XtraBars.BarCheckItem(barManager) { Caption = "僅月表取樣", Checked = true };
            chkReportSamples.CheckedChanged += (s, e) => ApplyFilter();
            btnAction = new DevExpress.XtraBars.BarButtonItem(barManager, "處理超限") { Enabled = false };
            btnActionHistory = new DevExpress.XtraBars.BarButtonItem(barManager, "處理紀錄") { Enabled = false };
            btnConflicts = new DevExpress.XtraBars.BarButtonItem(barManager, "資料衝突") { Enabled = false };
            btnExport = new DevExpress.XtraBars.BarButtonItem(barManager, "匯出試算表") { Enabled = false }; btnExport.ImageOptions.SvgImage = Helpers.TPSvgimages.Excel; btnExport.ItemClick += (s, e) => Export_Click(s, e);
            btnPrint = new DevExpress.XtraBars.BarButtonItem(barManager, "列印預覽") { Enabled = false }; btnPrint.ImageOptions.SvgImage = Helpers.TPSvgimages.Print; btnPrint.ItemClick += (s, e) => grid.ShowPrintPreview();
            foreach (var item in new DevExpress.XtraBars.BarItem[] { dateFrom, dateTo, btnLoad, chkReportSamples, chkAbnormal, btnExport, btnPrint }) { Lab208Ui.StyleDialogBarItem(item); toolbar.AddItem(item); }
            lblSummary = new DevExpress.XtraBars.BarStaticItem { Caption = "載入中…" }; barManager.Items.Add(lblSummary); status.AddItem(lblSummary);
            grid = new DevExpress.XtraGrid.GridControl();
            view = new DevExpress.XtraGrid.Views.Grid.GridView(grid); grid.MainView = view;
            grid.MenuManager = barManager;
            view.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            view.Appearance.HeaderPanel.Options.UseFont = view.Appearance.HeaderPanel.Options.UseForeColor = view.Appearance.HeaderPanel.Options.UseTextOptions = true;
            view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            view.Appearance.Row.Options.UseFont = true;
            view.OptionsView.EnableAppearanceOddRow = true;
            grid.RepositoryItems.Add(repoDeviceState);
            view.OptionsBehavior.Editable = false; view.OptionsView.ShowGroupPanel = false; view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            view.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            view.FocusedRowChanged += (s, e) => UpdateButtons(); view.PopupMenuShowing += Reading_PopupMenuShowing;
            view.RowStyle += (s, e) => { var row = view.GetDataRow(e.RowHandle); if (row != null && row.Table.Columns.Contains("Status") && row["Status"].ToString().Contains("超限")) e.Appearance.ForeColor = System.Drawing.Color.Firebrick; };
            chartTemperature = new DevExpress.XtraCharts.ChartControl(); chartHumidity = new DevExpress.XtraCharts.ChartControl();
            chartTemperature.Font = chartHumidity.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            chartTemperature.CrosshairEnabled = chartHumidity.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.True;
            chartTemperature.ToolTipEnabled = chartHumidity.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.True;
            chartTemperature.CrosshairOptions.GroupHeaderPattern = chartHumidity.CrosshairOptions.GroupHeaderPattern = "{A:yyyy/MM/dd HH:mm:ss} (UTC+07)";
            contentLayout = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            layoutRoot = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false, EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True, Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10) };
            chartGroup = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false, Location = new System.Drawing.Point(0, 360), Size = new System.Drawing.Size(1220, 300) };
            gridItem = new DevExpress.XtraLayout.LayoutControlItem { Control = grid, TextVisible = false, Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(1220, 350) };
            chartSplitter = new DevExpress.XtraLayout.SplitterItem { Location = new System.Drawing.Point(0, 350), Size = new System.Drawing.Size(1220, 10) };
            temperatureItem = new DevExpress.XtraLayout.LayoutControlItem { Control = chartTemperature, TextVisible = false, Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(610, 300) };
            humidityItem = new DevExpress.XtraLayout.LayoutControlItem { Control = chartHumidity, TextVisible = false, Location = new System.Drawing.Point(610, 0), Size = new System.Drawing.Size(610, 300) };
            contentLayout.Root = layoutRoot; contentLayout.Controls.AddRange(new System.Windows.Forms.Control[] { grid, chartTemperature, chartHumidity });
            layoutRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { gridItem, chartSplitter, chartGroup }); chartGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { temperatureItem, humidityItem });
            Controls.Add(contentLayout); contentLayout.BringToFront();
            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244); Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 8.25F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1250, 800); MinimumSize = new System.Drawing.Size(950, 650);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f208_ReadingHistory";
        }
    }
}
