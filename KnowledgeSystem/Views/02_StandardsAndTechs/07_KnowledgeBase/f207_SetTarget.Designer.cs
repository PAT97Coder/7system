namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    partial class f207_SetTarget
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeTargets = new DevExpress.XtraTreeList.TreeList();
            this.tColClass = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColId = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tColTarget = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryTarget = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.tColMode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.btnExpandAll = new DevExpress.XtraBars.BarButtonItem();
            this.btnCollapseAll = new DevExpress.XtraBars.BarButtonItem();
            this.barHint = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeTargets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.treeTargets);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(920, 551);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeTargets
            // 
            this.treeTargets.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.treeTargets.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.treeTargets.Appearance.HeaderPanel.Options.UseFont = true;
            this.treeTargets.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.treeTargets.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.treeTargets.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeTargets.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.treeTargets.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.treeTargets.Appearance.Row.Options.UseFont = true;
            this.treeTargets.Appearance.Row.Options.UseForeColor = true;
            this.treeTargets.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tColClass,
            this.tColId,
            this.tColTarget,
            this.tColMode});
            this.treeTargets.Location = new System.Drawing.Point(12, 12);
            this.treeTargets.MenuManager = this.barManager1;
            this.treeTargets.Name = "treeTargets";
            this.treeTargets.OptionsBehavior.Editable = true;
            this.treeTargets.OptionsView.AutoWidth = false;
            this.treeTargets.OptionsView.EnableAppearanceOddRow = true;
            this.treeTargets.OptionsView.ShowAutoFilterRow = true;
            this.treeTargets.OptionsView.ShowIndicator = false;
            this.treeTargets.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryTarget});
            this.treeTargets.Size = new System.Drawing.Size(896, 527);
            this.treeTargets.TabIndex = 4;
            // 
            // tColClass
            // 
            this.tColClass.Caption = "部門名稱";
            this.tColClass.FieldName = "Class";
            this.tColClass.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.tColClass.Name = "tColClass";
            this.tColClass.OptionsColumn.AllowEdit = false;
            this.tColClass.OptionsColumn.ReadOnly = true;
            this.tColClass.Visible = true;
            this.tColClass.VisibleIndex = 0;
            this.tColClass.Width = 390;
            // 
            // tColId
            // 
            this.tColId.Caption = "部門代號";
            this.tColId.FieldName = "Id";
            this.tColId.Name = "tColId";
            this.tColId.OptionsColumn.AllowEdit = false;
            this.tColId.OptionsColumn.ReadOnly = true;
            this.tColId.Visible = true;
            this.tColId.VisibleIndex = 1;
            this.tColId.Width = 130;
            // 
            // tColTarget
            // 
            this.tColTarget.AppearanceCell.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.tColTarget.AppearanceCell.Options.UseFont = true;
            this.tColTarget.AppearanceCell.Options.UseTextOptions = true;
            this.tColTarget.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tColTarget.Caption = "目標數量";
            this.tColTarget.ColumnEdit = this.repositoryTarget;
            this.tColTarget.FieldName = "Targets";
            this.tColTarget.Name = "tColTarget";
            this.tColTarget.Visible = true;
            this.tColTarget.VisibleIndex = 2;
            this.tColTarget.Width = 130;
            // 
            // repositoryTarget
            // 
            this.repositoryTarget.AutoHeight = false;
            this.repositoryTarget.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryTarget.IsFloatValue = false;
            this.repositoryTarget.MaskSettings.Set("mask", "d");
            this.repositoryTarget.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.repositoryTarget.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.repositoryTarget.Name = "repositoryTarget";
            // 
            // tColMode
            // 
            this.tColMode.Caption = "設定方式";
            this.tColMode.FieldName = "TargetMode";
            this.tColMode.Name = "tColMode";
            this.tColMode.OptionsColumn.AllowEdit = false;
            this.tColMode.OptionsColumn.ReadOnly = true;
            this.tColMode.Visible = true;
            this.tColMode.VisibleIndex = 3;
            this.tColMode.Width = 130;
            // 
            // barManager1
            // 
            this.barManager1.AllowCustomization = false;
            this.barManager1.AllowMoveBarOnToolbar = false;
            this.barManager1.AllowQuickCustomization = false;
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnConfirm,
            this.btnExpandAll,
            this.btnCollapseAll,
            this.barHint});
            this.barManager1.MainMenu = this.bar1;
            this.barManager1.MaxItemId = 4;
            // 
            // bar1
            // 
            this.bar1.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar1.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar1.BarAppearance.Hovered.Options.UseFont = true;
            this.bar1.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar1.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar1.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar1.BarAppearance.Normal.Options.UseFont = true;
            this.bar1.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar1.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar1.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar1.BarAppearance.Pressed.Options.UseFont = true;
            this.bar1.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar1.BarName = "Main menu";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExpandAll, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCollapseAll),
            new DevExpress.XtraBars.LinkPersistInfo(this.barHint, true)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawBorder = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.MultiLine = true;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Main menu";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "儲存";
            this.btnConfirm.Id = 0;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // btnExpandAll
            // 
            this.btnExpandAll.Caption = "全部展開";
            this.btnExpandAll.Id = 1;
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExpandAll_ItemClick);
            // 
            // btnCollapseAll
            // 
            this.btnCollapseAll.Caption = "全部收合";
            this.btnCollapseAll.Id = 2;
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCollapseAll_ItemClick);
            // 
            // barHint
            // 
            this.barHint.Caption = "提示：僅顯示啟用中的前三層部門；最下層可輸入目標，上層將自動加總。";
            this.barHint.Id = 3;
            this.barHint.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.barHint.ItemAppearance.Normal.ForeColor = System.Drawing.Color.DimGray;
            this.barHint.ItemAppearance.Normal.Options.UseFont = true;
            this.barHint.ItemAppearance.Normal.Options.UseForeColor = true;
            this.barHint.Name = "barHint";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(920, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 600);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(920, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 551);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(920, 49);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 551);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(920, 551);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.treeTargets;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(900, 531);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // f207_SetTarget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 600);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f207_SetTarget";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "知識文件上傳目標設定";
            this.Load += new System.EventHandler(this.f207_SetTarget_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeTargets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraTreeList.TreeList treeTargets;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColClass;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColId;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColTarget;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tColMode;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryTarget;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarButtonItem btnExpandAll;
        private DevExpress.XtraBars.BarButtonItem btnCollapseAll;
        private DevExpress.XtraBars.BarStaticItem barHint;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
