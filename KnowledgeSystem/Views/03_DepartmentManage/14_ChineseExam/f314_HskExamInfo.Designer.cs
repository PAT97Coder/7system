namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    partial class f314_HskExamInfo
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddUsr = new DevExpress.XtraBars.BarButtonItem();
            this.btnRemoveUsr = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txbHskRatio = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txbExamType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txbReading = new DevExpress.XtraEditors.SpinEdit();
            this.txbPassScore = new DevExpress.XtraEditors.SpinEdit();
            this.txbTime = new DevExpress.XtraEditors.SpinEdit();
            this.txbExamName = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcExamName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcExamType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcHskRatio = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcPass = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcReading = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbHskRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbReading.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPassScore.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExamName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExamType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcHskRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // barManagerTP
            // 
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnConfirm,
            this.btnAddUsr,
            this.btnRemoveUsr});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 6;
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.bar2.BarAppearance.Disabled.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Hovered.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar2.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Normal.Options.UseFont = true;
            this.bar2.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar2.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Pressed.Options.UseFont = true;
            this.bar2.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar2.BarName = "Main menu";
            this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAddUsr, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRemoveUsr, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 2;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // btnAddUsr
            // 
            this.btnAddUsr.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnAddUsr.Caption = "選擇人員";
            this.btnAddUsr.Id = 5;
            this.btnAddUsr.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnAddUsr.Name = "btnAddUsr";
            this.btnAddUsr.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddUsr_ItemClick);
            // 
            // btnRemoveUsr
            // 
            this.btnRemoveUsr.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnRemoveUsr.Caption = "刪除人員";
            this.btnRemoveUsr.Id = 4;
            this.btnRemoveUsr.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnRemoveUsr.Name = "btnRemoveUsr";
            this.btnRemoveUsr.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRemoveUsr_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(938, 40);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 533);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(938, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 40);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 493);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(938, 40);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 493);
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Controls.Add(this.txbHskRatio);
            this.layoutControl1.Controls.Add(this.txbExamType);
            this.layoutControl1.Controls.Add(this.txbReading);
            this.layoutControl1.Controls.Add(this.txbPassScore);
            this.layoutControl1.Controls.Add(this.txbTime);
            this.layoutControl1.Controls.Add(this.txbExamName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 40);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(938, 493);
            this.layoutControl1.TabIndex = 0;
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
            this.gcData.Location = new System.Drawing.Point(12, 84);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Size = new System.Drawing.Size(914, 397);
            this.gcData.TabIndex = 4;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvData});
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
            this.gvData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsDetail.ShowDetailTabs = false;
            this.gvData.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
            this.gvData.OptionsSelection.MultiSelect = true;
            this.gvData.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.RowAutoHeight = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "人員";
            this.gridColumn1.FieldName = "UserName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 220;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "部門";
            this.gridColumn2.FieldName = "DeptName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 220;
            // 
            // txbHskRatio
            // 
            this.txbHskRatio.Location = new System.Drawing.Point(352, 48);
            this.txbHskRatio.Name = "txbHskRatio";
            this.txbHskRatio.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbHskRatio.Properties.Appearance.Options.UseFont = true;
            this.txbHskRatio.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbHskRatio.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbHskRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbHskRatio.Size = new System.Drawing.Size(112, 32);
            this.txbHskRatio.StyleController = this.layoutControl1;
            this.txbHskRatio.TabIndex = 5;
            // 
            // txbExamType
            // 
            this.txbExamType.Location = new System.Drawing.Point(100, 48);
            this.txbExamType.Name = "txbExamType";
            this.txbExamType.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbExamType.Properties.Appearance.Options.UseFont = true;
            this.txbExamType.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbExamType.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbExamType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbExamType.Size = new System.Drawing.Size(142, 32);
            this.txbExamType.StyleController = this.layoutControl1;
            this.txbExamType.TabIndex = 6;
            // 
            // txbReading
            // 
            this.txbReading.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txbReading.Location = new System.Drawing.Point(864, 48);
            this.txbReading.Name = "txbReading";
            this.txbReading.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbReading.Properties.Appearance.Options.UseFont = true;
            this.txbReading.Properties.IsFloatValue = false;
            this.txbReading.Properties.MaskSettings.Set("mask", "N00");
            this.txbReading.Size = new System.Drawing.Size(62, 32);
            this.txbReading.StyleController = this.layoutControl1;
            this.txbReading.TabIndex = 7;
            // 
            // txbPassScore
            // 
            this.txbPassScore.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txbPassScore.Location = new System.Drawing.Point(713, 48);
            this.txbPassScore.Name = "txbPassScore";
            this.txbPassScore.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbPassScore.Properties.Appearance.Options.UseFont = true;
            this.txbPassScore.Properties.IsFloatValue = false;
            this.txbPassScore.Properties.MaskSettings.Set("mask", "N00");
            this.txbPassScore.Size = new System.Drawing.Size(66, 32);
            this.txbPassScore.StyleController = this.layoutControl1;
            this.txbPassScore.TabIndex = 8;
            // 
            // txbTime
            // 
            this.txbTime.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txbTime.Location = new System.Drawing.Point(549, 48);
            this.txbTime.Name = "txbTime";
            this.txbTime.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbTime.Properties.Appearance.Options.UseFont = true;
            this.txbTime.Properties.IsFloatValue = false;
            this.txbTime.Properties.MaskSettings.Set("mask", "N00");
            this.txbTime.Size = new System.Drawing.Size(79, 32);
            this.txbTime.StyleController = this.layoutControl1;
            this.txbTime.TabIndex = 9;
            // 
            // txbExamName
            // 
            this.txbExamName.Location = new System.Drawing.Point(100, 12);
            this.txbExamName.Name = "txbExamName";
            this.txbExamName.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbExamName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbExamName.Properties.Appearance.Options.UseFont = true;
            this.txbExamName.Properties.Appearance.Options.UseForeColor = true;
            this.txbExamName.Size = new System.Drawing.Size(826, 32);
            this.txbExamName.StyleController = this.layoutControl1;
            this.txbExamName.TabIndex = 10;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcExamName,
            this.lcExamType,
            this.lcHskRatio,
            this.lcTime,
            this.lcPass,
            this.lcReading,
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(938, 493);
            this.Root.TextVisible = false;
            // 
            // lcExamName
            // 
            this.lcExamName.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcExamName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcExamName.AppearanceItemCaption.Options.UseFont = true;
            this.lcExamName.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcExamName.Control = this.txbExamName;
            this.lcExamName.Location = new System.Drawing.Point(0, 0);
            this.lcExamName.Name = "lcExamName";
            this.lcExamName.Size = new System.Drawing.Size(918, 36);
            this.lcExamName.Text = "考試名稱";
            this.lcExamName.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcExamType
            // 
            this.lcExamType.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcExamType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcExamType.AppearanceItemCaption.Options.UseFont = true;
            this.lcExamType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcExamType.Control = this.txbExamType;
            this.lcExamType.Location = new System.Drawing.Point(0, 36);
            this.lcExamType.Name = "lcExamType";
            this.lcExamType.Size = new System.Drawing.Size(234, 36);
            this.lcExamType.Text = "考試類型";
            this.lcExamType.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcHskRatio
            // 
            this.lcHskRatio.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcHskRatio.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcHskRatio.AppearanceItemCaption.Options.UseFont = true;
            this.lcHskRatio.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcHskRatio.Control = this.txbHskRatio;
            this.lcHskRatio.Location = new System.Drawing.Point(234, 36);
            this.lcHskRatio.Name = "lcHskRatio";
            this.lcHskRatio.Size = new System.Drawing.Size(222, 36);
            this.lcHskRatio.Text = "HSK4:5比例";
            this.lcHskRatio.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcHskRatio.TextSize = new System.Drawing.Size(101, 24);
            this.lcHskRatio.TextToControlDistance = 5;
            // 
            // lcTime
            // 
            this.lcTime.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcTime.AppearanceItemCaption.Options.UseFont = true;
            this.lcTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcTime.Control = this.txbTime;
            this.lcTime.Location = new System.Drawing.Point(456, 36);
            this.lcTime.Name = "lcTime";
            this.lcTime.Size = new System.Drawing.Size(164, 36);
            this.lcTime.Text = "考試時長";
            this.lcTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcTime.TextSize = new System.Drawing.Size(76, 24);
            this.lcTime.TextToControlDistance = 5;
            // 
            // lcPass
            // 
            this.lcPass.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcPass.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcPass.AppearanceItemCaption.Options.UseFont = true;
            this.lcPass.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcPass.Control = this.txbPassScore;
            this.lcPass.Location = new System.Drawing.Point(620, 36);
            this.lcPass.Name = "lcPass";
            this.lcPass.Size = new System.Drawing.Size(151, 36);
            this.lcPass.Text = "及格分數";
            this.lcPass.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcPass.TextSize = new System.Drawing.Size(76, 24);
            this.lcPass.TextToControlDistance = 5;
            // 
            // lcReading
            // 
            this.lcReading.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcReading.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcReading.AppearanceItemCaption.Options.UseFont = true;
            this.lcReading.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcReading.Control = this.txbReading;
            this.lcReading.Location = new System.Drawing.Point(771, 36);
            this.lcReading.Name = "lcReading";
            this.lcReading.Size = new System.Drawing.Size(147, 36);
            this.lcReading.Text = "閱讀題數";
            this.lcReading.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lcReading.TextSize = new System.Drawing.Size(76, 24);
            this.lcReading.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(918, 401);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // f314_HskExamInfo
            // 
            this.ClientSize = new System.Drawing.Size(938, 533);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "f314_HskExamInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新增 HSK 考試";
            this.Load += new System.EventHandler(this.f314_HskExamInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbHskRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbReading.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbPassScore.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbExamName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExamName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcExamType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcHskRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcReading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarButtonItem btnAddUsr;
        private DevExpress.XtraBars.BarButtonItem btnRemoveUsr;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gcData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.SpinEdit txbReading;
        private DevExpress.XtraEditors.SpinEdit txbPassScore;
        private DevExpress.XtraEditors.SpinEdit txbTime;
        private DevExpress.XtraEditors.ComboBoxEdit txbHskRatio;
        private DevExpress.XtraEditors.ComboBoxEdit txbExamType;
        private DevExpress.XtraEditors.TextEdit txbExamName;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcExamName;
        private DevExpress.XtraLayout.LayoutControlItem lcTime;
        private DevExpress.XtraLayout.LayoutControlItem lcPass;
        private DevExpress.XtraLayout.LayoutControlItem lcReading;
        private DevExpress.XtraLayout.LayoutControlItem lcHskRatio;
        private DevExpress.XtraLayout.LayoutControlItem lcExamType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
