namespace KnowledgeSystem.Views._03_DepartmentManage._17_ExamStatistics
{
    partial class uc317_ExamStatistics
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar barMain;
        private DevExpress.XtraBars.BarEditItem barYear;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryYear;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarSubItem btnExportExcel;
        private DevExpress.XtraBars.BarButtonItem btnExportForm1;
        private DevExpress.XtraBars.BarButtonItem btnExportForm2;
        private DevExpress.XtraBars.BarButtonItem btnExportForm3;
        private DevExpress.XtraBars.BarButtonItem btnExportForm4;
        private DevExpress.XtraBars.BarButtonItem btnExportForm5;
        private DevExpress.XtraBars.BarButtonItem btnExportAllForms;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.barMain = new DevExpress.XtraBars.Bar();
            this.barYear = new DevExpress.XtraBars.BarEditItem();
            this.repositoryYear = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarSubItem();
            this.btnExportForm1 = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportForm2 = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportForm3 = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportForm4 = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportForm5 = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportAllForms = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            //
            // barManagerTP
            //
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] { this.barMain });
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
                this.barYear, this.btnReload, this.btnExportExcel, this.btnExportForm1,
                this.btnExportForm2, this.btnExportForm3, this.btnExportForm4,
                this.btnExportForm5, this.btnExportAllForms });
            this.barManagerTP.MainMenu = this.barMain;
            this.barManagerTP.MaxItemId = 9;
            this.barManagerTP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
                this.repositoryYear });
            //
            // barMain
            //
            this.barMain.BarAppearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.barMain.BarAppearance.Disabled.Options.UseFont = true;
            this.barMain.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barMain.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.barMain.BarAppearance.Hovered.Options.UseFont = true;
            this.barMain.BarAppearance.Hovered.Options.UseForeColor = true;
            this.barMain.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barMain.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.barMain.BarAppearance.Normal.Options.UseFont = true;
            this.barMain.BarAppearance.Normal.Options.UseForeColor = true;
            this.barMain.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barMain.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.barMain.BarAppearance.Pressed.Options.UseFont = true;
            this.barMain.BarAppearance.Pressed.Options.UseForeColor = true;
            this.barMain.BarName = "Main menu";
            this.barMain.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.barMain.DockCol = 0;
            this.barMain.DockRow = 0;
            this.barMain.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMain.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width,
                    this.barYear, "", false, true, true, 180),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle,
                    this.btnReload, "", true, true, true, 0, null,
                    DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle,
                    this.btnExportExcel, "", true, true, true, 0, null,
                    DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph) });
            this.barMain.OptionsBar.AllowQuickCustomization = false;
            this.barMain.OptionsBar.DrawDragBorder = false;
            this.barMain.OptionsBar.MultiLine = true;
            this.barMain.OptionsBar.UseWholeRow = true;
            this.barMain.Text = "Main menu";
            //
            // barYear
            //
            this.barYear.Caption = "年度";
            this.barYear.Edit = this.repositoryYear;
            this.barYear.Id = 0;
            this.barYear.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barYear.Name = "barYear";
            this.barYear.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barYear.EditValueChanged += new System.EventHandler(this.barYear_EditValueChanged);
            //
            // repositoryYear
            //
            this.repositoryYear.AutoHeight = false;
            this.repositoryYear.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.repositoryYear.Appearance.ForeColor = System.Drawing.Color.Black;
            this.repositoryYear.Appearance.Options.UseFont = true;
            this.repositoryYear.Appearance.Options.UseForeColor = true;
            this.repositoryYear.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(
                    DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.repositoryYear.Name = "repositoryYear";
            this.repositoryYear.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //
            // btnReload
            //
            this.btnReload.Caption = "刷新";
            this.btnReload.Id = 1;
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnReload.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnReload.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnReload.Name = "btnReload";
            this.btnReload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReload_ItemClick);
            //
            // btnExportExcel
            //
            this.btnExportExcel.Caption = "出表";
            this.btnExportExcel.Id = 2;
            this.btnExportExcel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExportExcel.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExportExcel.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExportExcel.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnExportExcel.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnExportExcel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExportExcel.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnExportExcel.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
                new DevExpress.XtraBars.LinkPersistInfo(this.btnExportForm1),
                new DevExpress.XtraBars.LinkPersistInfo(this.btnExportForm2),
                new DevExpress.XtraBars.LinkPersistInfo(this.btnExportForm3),
                new DevExpress.XtraBars.LinkPersistInfo(this.btnExportForm4),
                new DevExpress.XtraBars.LinkPersistInfo(this.btnExportForm5),
                new DevExpress.XtraBars.LinkPersistInfo(this.btnExportAllForms, true) });
            this.btnExportExcel.Name = "btnExportExcel";
            //
            // btnExportForm1
            //
            this.btnExportForm1.Caption = "表單一：學科成績";
            this.btnExportForm1.Id = 3;
            this.btnExportForm1.Name = "btnExportForm1";
            this.btnExportForm1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportForm1_ItemClick);
            //
            // btnExportForm2
            //
            this.btnExportForm2.Caption = "表單二：中文成績";
            this.btnExportForm2.Id = 4;
            this.btnExportForm2.Name = "btnExportForm2";
            this.btnExportForm2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportForm2_ItemClick);
            //
            // btnExportForm3
            //
            this.btnExportForm3.Caption = "表單三：中文補考";
            this.btnExportForm3.Id = 5;
            this.btnExportForm3.Name = "btnExportForm3";
            this.btnExportForm3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportForm3_ItemClick);
            //
            // btnExportForm4
            //
            this.btnExportForm4.Caption = "表單四：口試成績";
            this.btnExportForm4.Id = 6;
            this.btnExportForm4.Name = "btnExportForm4";
            this.btnExportForm4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportForm4_ItemClick);
            //
            // btnExportForm5
            //
            this.btnExportForm5.Caption = "表單五：結果彙總";
            this.btnExportForm5.Id = 7;
            this.btnExportForm5.Name = "btnExportForm5";
            this.btnExportForm5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportForm5_ItemClick);
            //
            // btnExportAllForms
            //
            this.btnExportAllForms.Caption = "匯出全部表單";
            this.btnExportAllForms.Id = 8;
            this.btnExportAllForms.Name = "btnExportAllForms";
            this.btnExportAllForms.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportAllForms_ItemClick);
            //
            // dock controls
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1100, 49);
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 600);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 551);
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 551);
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 551);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            //
            // gcData
            //
            this.gcData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Size = new System.Drawing.Size(1076, 527);
            this.gcData.TabIndex = 5;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gvData });
            //
            // gvData
            //
            this.gvData.Appearance.FooterPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvData.Appearance.FooterPanel.Options.UseFont = true;
            this.gvData.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gvData.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvData.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvData.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvData.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvData.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvData.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvData.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvData.Appearance.Row.Options.UseFont = true;
            this.gvData.Appearance.Row.Options.UseForeColor = true;
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
                this.layoutControlItem1 });
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1100, 551);
            this.Root.TextVisible = false;
            //
            // layoutControlItem1
            //
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1080, 531);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            //
            // uc317_ExamStatistics
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc317_ExamStatistics";
            this.Size = new System.Drawing.Size(1100, 600);
            this.Load += new System.EventHandler(this.uc317_ExamStatistics_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
