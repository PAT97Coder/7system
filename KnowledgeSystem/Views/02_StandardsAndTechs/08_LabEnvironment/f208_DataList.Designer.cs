namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_DataList
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarDockControl dockTop, dockBottom, dockLeft, dockRight;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repoDeviceState;
        private DevExpress.XtraLayout.LayoutControl contentLayout;
        private DevExpress.XtraLayout.LayoutControlGroup layoutRoot;
        private DevExpress.XtraLayout.LayoutControlItem gridItem;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar toolbar;
        private DevExpress.XtraBars.BarButtonItem btnExport, btnPrint;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            repoDeviceState = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            for (int i = 0; i < BusinessLayer.dt208_EnvironmentRules.DeviceStates.Length; i++)
                repoDeviceState.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(BusinessLayer.dt208_EnvironmentRules.DeviceStates[i], i));
            Lab208Ui.StyleRepository(repoDeviceState);
            grid = new DevExpress.XtraGrid.GridControl { Dock = System.Windows.Forms.DockStyle.Fill };
            view = new DevExpress.XtraGrid.Views.Grid.GridView(grid); grid.MainView = view;
            grid.MenuManager = barManager;
            view.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            view.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            view.Appearance.HeaderPanel.Options.UseFont = view.Appearance.HeaderPanel.Options.UseForeColor = view.Appearance.HeaderPanel.Options.UseTextOptions = true;
            view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            view.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            view.Appearance.Row.Options.UseFont = true;
            view.OptionsView.EnableAppearanceOddRow = true;
            grid.RepositoryItems.Add(repoDeviceState);
            barManager = new DevExpress.XtraBars.BarManager(components) { Form = this };
            toolbar = new DevExpress.XtraBars.Bar(barManager, "工具列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Top };
            toolbar.OptionsBar.AllowQuickCustomization = false;
            Lab208Ui.StyleDialogToolbar(toolbar);
            barManager.MainMenu = toolbar;
            dockTop = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Top };
            dockBottom = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Bottom };
            dockLeft = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Left };
            dockRight = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Right };
            Controls.AddRange(new System.Windows.Forms.Control[] { dockTop, dockBottom, dockLeft, dockRight });
            btnExport = new DevExpress.XtraBars.BarButtonItem(barManager, "匯出試算表");
            btnPrint = new DevExpress.XtraBars.BarButtonItem(barManager, "列印預覽");
            btnExport.ImageOptions.SvgImage = Helpers.TPSvgimages.Excel; btnPrint.ImageOptions.SvgImage = Helpers.TPSvgimages.Print;
            Lab208Ui.StyleDialogBarItem(btnExport); Lab208Ui.StyleDialogBarItem(btnPrint);
            btnExport.ItemClick += (s, e) => Export_Click(s, e); btnPrint.ItemClick += (s, e) => grid.ShowPrintPreview();
            toolbar.AddItem(btnExport); toolbar.AddItem(btnPrint);
            contentLayout = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            layoutRoot = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false, EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True, Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10) };
            gridItem = new DevExpress.XtraLayout.LayoutControlItem { Control = grid, TextVisible = false };
            contentLayout.Root = layoutRoot; contentLayout.Controls.Add(grid); layoutRoot.AddItem(gridItem);
            Controls.Add(contentLayout); contentLayout.BringToFront();
            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244); Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 8.25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; ClientSize = new System.Drawing.Size(1100, 650);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f208_DataList";
        }
    }
}
