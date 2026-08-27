namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    partial class uc401_DepartmentManage
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeData = new DevExpress.XtraTreeList.TreeList();
            this.tColId = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColIdChild = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColIdParent = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColDisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColDisplayNameVN = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColIsGroup = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColAuthorizedHeadcount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColIsActive = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
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
            this.btnAdd,
            this.btnReload});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 2;
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 12F);
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAdd, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "新增";
            this.btnAdd.Id = 0;
            this.btnAdd.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAdd_ItemClick);
            // 
            // btnReload
            // 
            this.btnReload.Caption = "刷新";
            this.btnReload.Id = 1;
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.Name = "btnReload";
            this.btnReload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReload_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(884, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 516);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(884, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 467);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(884, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 467);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.treeData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(884, 467);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeData
            // 
            this.treeData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.treeData.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.treeData.Appearance.HeaderPanel.Options.UseFont = true;
            this.treeData.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.treeData.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.treeData.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.treeData.Appearance.Row.Options.UseFont = true;
            this.treeData.Appearance.Row.Options.UseForeColor = true;
            this.treeData.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tColId,
            this.tColIdChild,
            this.tColIdParent,
            this.tColDisplayName,
            this.tColDisplayNameVN,
            this.tColIsGroup,
            this.tColAuthorizedHeadcount,
            this.tColIsActive});
            this.treeData.Location = new System.Drawing.Point(12, 12);
            this.treeData.MenuManager = this.barManagerTP;
            this.treeData.Name = "treeData";
            this.treeData.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeData.OptionsBehavior.Editable = false;
            this.treeData.OptionsView.AutoWidth = false;
            this.treeData.OptionsView.EnableAppearanceOddRow = true;
            this.treeData.OptionsView.ShowAutoFilterRow = true;
            this.treeData.OptionsView.ShowIndicator = false;
            this.treeData.Size = new System.Drawing.Size(860, 443);
            this.treeData.TabIndex = 5;
            this.treeData.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeData_NodeCellStyle);
            this.treeData.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeData_PopupMenuShowing);
            // 
            // tColId
            // 
            this.tColId.Caption = "部門代號";
            this.tColId.FieldName = "Id";
            this.tColId.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.tColId.Name = "tColId";
            this.tColId.OptionsColumn.ReadOnly = true;
            this.tColId.Visible = true;
            this.tColId.VisibleIndex = 0;
            this.tColId.Width = 130;
            // 
            // tColIdChild
            // 
            this.tColIdChild.Caption = "IdChild";
            this.tColIdChild.FieldName = "IdChild";
            this.tColIdChild.Name = "tColIdChild";
            this.tColIdChild.OptionsColumn.ReadOnly = true;
            this.tColIdChild.Visible = true;
            this.tColIdChild.VisibleIndex = 1;
            this.tColIdChild.Width = 90;
            // 
            // tColIdParent
            // 
            this.tColIdParent.Caption = "IdParent";
            this.tColIdParent.FieldName = "IdParent";
            this.tColIdParent.Name = "tColIdParent";
            this.tColIdParent.OptionsColumn.ReadOnly = true;
            this.tColIdParent.Visible = true;
            this.tColIdParent.VisibleIndex = 2;
            this.tColIdParent.Width = 90;
            // 
            // tColDisplayName
            // 
            this.tColDisplayName.Caption = "部門名稱";
            this.tColDisplayName.FieldName = "DisplayName";
            this.tColDisplayName.Name = "tColDisplayName";
            this.tColDisplayName.OptionsColumn.ReadOnly = true;
            this.tColDisplayName.Visible = true;
            this.tColDisplayName.VisibleIndex = 3;
            this.tColDisplayName.Width = 180;
            // 
            // tColDisplayNameVN
            // 
            this.tColDisplayNameVN.Caption = "越文名稱";
            this.tColDisplayNameVN.FieldName = "DisplayNameVN";
            this.tColDisplayNameVN.Name = "tColDisplayNameVN";
            this.tColDisplayNameVN.OptionsColumn.ReadOnly = true;
            this.tColDisplayNameVN.Visible = true;
            this.tColDisplayNameVN.VisibleIndex = 4;
            this.tColDisplayNameVN.Width = 280;
            // 
            // tColIsGroup
            // 
            this.tColIsGroup.Caption = "群組";
            this.tColIsGroup.FieldName = "IsGroup";
            this.tColIsGroup.Name = "tColIsGroup";
            this.tColIsGroup.OptionsColumn.ReadOnly = true;
            this.tColIsGroup.Width = 70;
            // 
            // tColAuthorizedHeadcount
            // 
            this.tColAuthorizedHeadcount.Caption = "編制";
            this.tColAuthorizedHeadcount.FieldName = "AuthorizedHeadcount";
            this.tColAuthorizedHeadcount.Name = "tColAuthorizedHeadcount";
            this.tColAuthorizedHeadcount.Visible = true;
            this.tColAuthorizedHeadcount.VisibleIndex = 6;
            this.tColAuthorizedHeadcount.Width = 80;
            // 
            // tColIsActive
            // 
            this.tColIsActive.Caption = "啟用";
            this.tColIsActive.FieldName = "IsActive";
            this.tColIsActive.Name = "tColIsActive";
            this.tColIsActive.Width = 70;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(884, 467);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.treeData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(864, 447);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // uc401_DepartmentManage
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc401_DepartmentManage";
            this.Size = new System.Drawing.Size(884, 516);
            this.Load += new System.EventHandler(this.uc401_DepartmentManage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraTreeList.TreeList treeData;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColId;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColIdChild;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColIdParent;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColDisplayName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColDisplayNameVN;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColIsGroup;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColAuthorizedHeadcount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColIsActive;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
