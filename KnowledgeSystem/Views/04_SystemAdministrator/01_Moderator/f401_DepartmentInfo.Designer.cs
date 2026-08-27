namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    partial class f401_DepartmentInfo
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(f401_DepartmentInfo));
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbId = new DevExpress.XtraEditors.TextEdit();
            this.txbIdChild = new DevExpress.XtraEditors.TextEdit();
            this.txbIdParent = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txbDisplayName = new DevExpress.XtraEditors.TextEdit();
            this.txbDisplayNameVN = new DevExpress.XtraEditors.TextEdit();
            this.chkIsGroup = new DevExpress.XtraEditors.CheckEdit();
            this.spnAuthorizedHeadcount = new DevExpress.XtraEditors.SpinEdit();
            this.chkIsActive = new DevExpress.XtraEditors.CheckEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcId = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIdChild = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIdParent = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDisplayName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDisplayNameVN = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIsGroup = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcAuthorizedHeadcount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIsActive = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbIdChild.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbIdParent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDisplayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDisplayNameVN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnAuthorizedHeadcount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIdChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIdParent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDisplayName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDisplayNameVN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIsGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAuthorizedHeadcount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIsActive)).BeginInit();
            this.SuspendLayout();
            // 
            // barManagerTP
            // 
            this.barManagerTP.AllowMoveBarOnToolbar = false;
            this.barManagerTP.AllowQuickCustomization = false;
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnEdit,
            this.btnConfirm,
            this.btnDelete});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 3;
            // 
            // bar2
            // 
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnEdit, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnConfirm, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete, true)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawBorder = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnEdit
            // 
            this.btnEdit.Caption = "修改";
            this.btnEdit.Id = 0;
            this.btnEdit.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnEdit.ImageOptions.SvgImage")));
            this.btnEdit.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnEdit_ItemClick);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "確定";
            this.btnConfirm.Id = 1;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "刪除";
            this.btnDelete.Id = 2;
            this.btnDelete.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(640, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 341);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(640, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 292);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(640, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 292);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txbId);
            this.layoutControl1.Controls.Add(this.txbIdChild);
            this.layoutControl1.Controls.Add(this.txbIdParent);
            this.layoutControl1.Controls.Add(this.txbDisplayName);
            this.layoutControl1.Controls.Add(this.txbDisplayNameVN);
            this.layoutControl1.Controls.Add(this.chkIsGroup);
            this.layoutControl1.Controls.Add(this.spnAuthorizedHeadcount);
            this.layoutControl1.Controls.Add(this.chkIsActive);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(640, 292);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txbId
            // 
            this.txbId.Location = new System.Drawing.Point(100, 12);
            this.txbId.MenuManager = this.barManagerTP;
            this.txbId.Name = "txbId";
            this.txbId.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbId.Properties.Appearance.Options.UseFont = true;
            this.txbId.Properties.Appearance.Options.UseForeColor = true;
            this.txbId.Size = new System.Drawing.Size(528, 32);
            this.txbId.StyleController = this.layoutControl1;
            this.txbId.TabIndex = 4;
            // 
            // txbIdChild
            // 
            this.txbIdChild.Location = new System.Drawing.Point(100, 48);
            this.txbIdChild.MenuManager = this.barManagerTP;
            this.txbIdChild.Name = "txbIdChild";
            this.txbIdChild.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbIdChild.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbIdChild.Properties.Appearance.Options.UseFont = true;
            this.txbIdChild.Properties.Appearance.Options.UseForeColor = true;
            this.txbIdChild.Size = new System.Drawing.Size(528, 32);
            this.txbIdChild.StyleController = this.layoutControl1;
            this.txbIdChild.TabIndex = 5;
            // 
            // txbIdParent
            //
            this.txbIdParent.Location = new System.Drawing.Point(100, 84);
            this.txbIdParent.MenuManager = this.barManagerTP;
            this.txbIdParent.Name = "txbIdParent";
            this.txbIdParent.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbIdParent.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbIdParent.Properties.Appearance.Options.UseFont = true;
            this.txbIdParent.Properties.Appearance.Options.UseForeColor = true;
            this.txbIdParent.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbIdParent.Properties.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            this.txbIdParent.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbIdParent.Properties.AppearanceDropDown.Options.UseForeColor = true;
            this.txbIdParent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txbIdParent.Properties.NullText = "";
            this.txbIdParent.Properties.PopupView = this.gridLookUpEdit1View;
            this.txbIdParent.Size = new System.Drawing.Size(528, 32);
            this.txbIdParent.StyleController = this.layoutControl1;
            this.txbIdParent.TabIndex = 6;
            //
            // gridLookUpEdit1View
            //
            this.gridLookUpEdit1View.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gridLookUpEdit1View.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gridLookUpEdit1View.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridLookUpEdit1View.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridLookUpEdit1View.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridLookUpEdit1View.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridLookUpEdit1View.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gridLookUpEdit1View.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridLookUpEdit1View.Appearance.Row.Options.UseFont = true;
            this.gridLookUpEdit1View.Appearance.Row.Options.UseForeColor = true;
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.EnableAppearanceOddRow = true;
            this.gridLookUpEdit1View.OptionsView.ShowAutoFilterRow = true;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            this.gridLookUpEdit1View.OptionsView.ShowIndicator = false;
            //
            // txbDisplayName
            //
            this.txbDisplayName.Location = new System.Drawing.Point(100, 120);
            this.txbDisplayName.MenuManager = this.barManagerTP;
            this.txbDisplayName.Name = "txbDisplayName";
            this.txbDisplayName.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbDisplayName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbDisplayName.Properties.Appearance.Options.UseFont = true;
            this.txbDisplayName.Properties.Appearance.Options.UseForeColor = true;
            this.txbDisplayName.Size = new System.Drawing.Size(528, 32);
            this.txbDisplayName.StyleController = this.layoutControl1;
            this.txbDisplayName.TabIndex = 7;
            // 
            // txbDisplayNameVN
            // 
            this.txbDisplayNameVN.Location = new System.Drawing.Point(100, 156);
            this.txbDisplayNameVN.MenuManager = this.barManagerTP;
            this.txbDisplayNameVN.Name = "txbDisplayNameVN";
            this.txbDisplayNameVN.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbDisplayNameVN.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbDisplayNameVN.Properties.Appearance.Options.UseFont = true;
            this.txbDisplayNameVN.Properties.Appearance.Options.UseForeColor = true;
            this.txbDisplayNameVN.Size = new System.Drawing.Size(528, 32);
            this.txbDisplayNameVN.StyleController = this.layoutControl1;
            this.txbDisplayNameVN.TabIndex = 8;
            // 
            // chkIsGroup
            // 
            this.chkIsGroup.Location = new System.Drawing.Point(100, 192);
            this.chkIsGroup.MenuManager = this.barManagerTP;
            this.chkIsGroup.Name = "chkIsGroup";
            this.chkIsGroup.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.chkIsGroup.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.chkIsGroup.Properties.Appearance.Options.UseFont = true;
            this.chkIsGroup.Properties.Appearance.Options.UseForeColor = true;
            this.chkIsGroup.Properties.Caption = "";
            this.chkIsGroup.Size = new System.Drawing.Size(528, 20);
            this.chkIsGroup.StyleController = this.layoutControl1;
            this.chkIsGroup.TabIndex = 9;
            // 
            // spnAuthorizedHeadcount
            // 
            this.spnAuthorizedHeadcount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spnAuthorizedHeadcount.Location = new System.Drawing.Point(100, 220);
            this.spnAuthorizedHeadcount.MenuManager = this.barManagerTP;
            this.spnAuthorizedHeadcount.Name = "spnAuthorizedHeadcount";
            this.spnAuthorizedHeadcount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spnAuthorizedHeadcount.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.spnAuthorizedHeadcount.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.spnAuthorizedHeadcount.Properties.Appearance.Options.UseFont = true;
            this.spnAuthorizedHeadcount.Properties.Appearance.Options.UseForeColor = true;
            this.spnAuthorizedHeadcount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnAuthorizedHeadcount.Properties.IsFloatValue = false;
            this.spnAuthorizedHeadcount.Properties.Mask.EditMask = "N0";
            this.spnAuthorizedHeadcount.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.spnAuthorizedHeadcount.Properties.MaxValue = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.spnAuthorizedHeadcount.Size = new System.Drawing.Size(528, 32);
            this.spnAuthorizedHeadcount.StyleController = this.layoutControl1;
            this.spnAuthorizedHeadcount.TabIndex = 10;
            // 
            // chkIsActive
            // 
            this.chkIsActive.Location = new System.Drawing.Point(100, 256);
            this.chkIsActive.MenuManager = this.barManagerTP;
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.chkIsActive.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.chkIsActive.Properties.Appearance.Options.UseFont = true;
            this.chkIsActive.Properties.Appearance.Options.UseForeColor = true;
            this.chkIsActive.Properties.Caption = "";
            this.chkIsActive.Size = new System.Drawing.Size(528, 20);
            this.chkIsActive.StyleController = this.layoutControl1;
            this.chkIsActive.TabIndex = 11;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcId,
            this.lcIdChild,
            this.lcIdParent,
            this.lcDisplayName,
            this.lcDisplayNameVN,
            this.lcIsGroup,
            this.lcAuthorizedHeadcount,
            this.lcIsActive});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(640, 292);
            this.Root.TextVisible = false;
            // 
            // lcId
            // 
            this.lcId.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcId.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcId.AppearanceItemCaption.Options.UseFont = true;
            this.lcId.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcId.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcId.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcId.Control = this.txbId;
            this.lcId.Location = new System.Drawing.Point(0, 0);
            this.lcId.Name = "lcId";
            this.lcId.Size = new System.Drawing.Size(620, 36);
            this.lcId.Text = "部門代號";
            this.lcId.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcIdChild
            // 
            this.lcIdChild.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcIdChild.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcIdChild.AppearanceItemCaption.Options.UseFont = true;
            this.lcIdChild.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcIdChild.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcIdChild.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcIdChild.Control = this.txbIdChild;
            this.lcIdChild.Location = new System.Drawing.Point(0, 36);
            this.lcIdChild.Name = "lcIdChild";
            this.lcIdChild.Size = new System.Drawing.Size(620, 36);
            this.lcIdChild.Text = "IdChild";
            this.lcIdChild.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcIdParent
            // 
            this.lcIdParent.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcIdParent.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcIdParent.AppearanceItemCaption.Options.UseFont = true;
            this.lcIdParent.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcIdParent.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcIdParent.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcIdParent.Control = this.txbIdParent;
            this.lcIdParent.Location = new System.Drawing.Point(0, 72);
            this.lcIdParent.Name = "lcIdParent";
            this.lcIdParent.Size = new System.Drawing.Size(620, 36);
            this.lcIdParent.Text = "IdParent";
            this.lcIdParent.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcDisplayName
            // 
            this.lcDisplayName.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcDisplayName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcDisplayName.AppearanceItemCaption.Options.UseFont = true;
            this.lcDisplayName.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcDisplayName.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcDisplayName.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcDisplayName.Control = this.txbDisplayName;
            this.lcDisplayName.Location = new System.Drawing.Point(0, 108);
            this.lcDisplayName.Name = "lcDisplayName";
            this.lcDisplayName.Size = new System.Drawing.Size(620, 36);
            this.lcDisplayName.Text = "部門名稱";
            this.lcDisplayName.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcDisplayNameVN
            // 
            this.lcDisplayNameVN.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcDisplayNameVN.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcDisplayNameVN.AppearanceItemCaption.Options.UseFont = true;
            this.lcDisplayNameVN.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcDisplayNameVN.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcDisplayNameVN.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcDisplayNameVN.Control = this.txbDisplayNameVN;
            this.lcDisplayNameVN.Location = new System.Drawing.Point(0, 144);
            this.lcDisplayNameVN.Name = "lcDisplayNameVN";
            this.lcDisplayNameVN.Size = new System.Drawing.Size(620, 36);
            this.lcDisplayNameVN.Text = "越文名稱";
            this.lcDisplayNameVN.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcIsGroup
            // 
            this.lcIsGroup.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcIsGroup.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcIsGroup.AppearanceItemCaption.Options.UseFont = true;
            this.lcIsGroup.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcIsGroup.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcIsGroup.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcIsGroup.Control = this.chkIsGroup;
            this.lcIsGroup.Location = new System.Drawing.Point(0, 180);
            this.lcIsGroup.Name = "lcIsGroup";
            this.lcIsGroup.Size = new System.Drawing.Size(620, 28);
            this.lcIsGroup.Text = "群組";
            this.lcIsGroup.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcAuthorizedHeadcount
            // 
            this.lcAuthorizedHeadcount.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcAuthorizedHeadcount.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcAuthorizedHeadcount.AppearanceItemCaption.Options.UseFont = true;
            this.lcAuthorizedHeadcount.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcAuthorizedHeadcount.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcAuthorizedHeadcount.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcAuthorizedHeadcount.Control = this.spnAuthorizedHeadcount;
            this.lcAuthorizedHeadcount.Location = new System.Drawing.Point(0, 208);
            this.lcAuthorizedHeadcount.Name = "lcAuthorizedHeadcount";
            this.lcAuthorizedHeadcount.Size = new System.Drawing.Size(620, 36);
            this.lcAuthorizedHeadcount.Text = "編制";
            this.lcAuthorizedHeadcount.TextSize = new System.Drawing.Size(76, 24);
            // 
            // lcIsActive
            // 
            this.lcIsActive.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcIsActive.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcIsActive.AppearanceItemCaption.Options.UseFont = true;
            this.lcIsActive.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcIsActive.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            this.lcIsActive.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.lcIsActive.Control = this.chkIsActive;
            this.lcIsActive.Location = new System.Drawing.Point(0, 244);
            this.lcIsActive.Name = "lcIsActive";
            this.lcIsActive.Size = new System.Drawing.Size(620, 28);
            this.lcIsActive.Text = "啟用";
            this.lcIsActive.TextSize = new System.Drawing.Size(76, 24);
            // 
            // f401_DepartmentInfo
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 341);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f401_DepartmentInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "部門信息";
            this.Load += new System.EventHandler(this.f401_DepartmentInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbIdChild.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbIdParent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDisplayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDisplayNameVN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnAuthorizedHeadcount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIdChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIdParent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDisplayName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDisplayNameVN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIsGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcAuthorizedHeadcount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIsActive)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnEdit;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txbId;
        private DevExpress.XtraEditors.TextEdit txbIdChild;
        private DevExpress.XtraEditors.GridLookUpEdit txbIdParent;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txbDisplayName;
        private DevExpress.XtraEditors.TextEdit txbDisplayNameVN;
        private DevExpress.XtraEditors.CheckEdit chkIsGroup;
        private DevExpress.XtraEditors.SpinEdit spnAuthorizedHeadcount;
        private DevExpress.XtraEditors.CheckEdit chkIsActive;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcId;
        private DevExpress.XtraLayout.LayoutControlItem lcIdChild;
        private DevExpress.XtraLayout.LayoutControlItem lcIdParent;
        private DevExpress.XtraLayout.LayoutControlItem lcDisplayName;
        private DevExpress.XtraLayout.LayoutControlItem lcDisplayNameVN;
        private DevExpress.XtraLayout.LayoutControlItem lcIsGroup;
        private DevExpress.XtraLayout.LayoutControlItem lcAuthorizedHeadcount;
        private DevExpress.XtraLayout.LayoutControlItem lcIsActive;
    }
}
