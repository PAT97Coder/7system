namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    partial class f318_Settings
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbSource7810 = new DevExpress.XtraEditors.ButtonEdit();
            this.txbSource7820 = new DevExpress.XtraEditors.ButtonEdit();
            this.txbCommonRoot = new DevExpress.XtraEditors.ButtonEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcSource7810 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcSource7820 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCommonRoot = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbSource7810.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSource7820.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCommonRoot.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSource7810)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSource7820)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCommonRoot)).BeginInit();
            this.SuspendLayout();
            // barManagerTP
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] { this.bar2 });
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] { this.btnConfirm });
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 1;
            // bar2: cùng thanh xác nhận của form thiết lập kiểm tra.
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
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph) });
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // btnConfirm
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 0;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // dock controls
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(640, 49);
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Manager = this.barManagerTP;
            // layoutControl1
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.txbSource7810);
            this.layoutControl1.Controls.Add(this.txbSource7820);
            this.layoutControl1.Controls.Add(this.txbCommonRoot);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(640, 128);
            // txbSource7810
            this.txbSource7810.Location = new System.Drawing.Point(160, 12);
            this.txbSource7810.MenuManager = this.barManagerTP;
            this.txbSource7810.Name = "txbSource7810";
            this.txbSource7810.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbSource7810.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbSource7810.Properties.Appearance.Options.UseFont = true;
            this.txbSource7810.Properties.Appearance.Options.UseForeColor = true;
            this.txbSource7810.Properties.Buttons.Clear();
            this.txbSource7810.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis) });
            this.txbSource7810.Size = new System.Drawing.Size(468, 32);
            this.txbSource7810.StyleController = this.layoutControl1;
            this.txbSource7810.TabIndex = 0;
            this.txbSource7810.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbSource7810_ButtonClick);
            // txbSource7820
            this.txbSource7820.Location = new System.Drawing.Point(160, 48);
            this.txbSource7820.MenuManager = this.barManagerTP;
            this.txbSource7820.Name = "txbSource7820";
            this.txbSource7820.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbSource7820.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbSource7820.Properties.Appearance.Options.UseFont = true;
            this.txbSource7820.Properties.Appearance.Options.UseForeColor = true;
            this.txbSource7820.Properties.Buttons.Clear();
            this.txbSource7820.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis) });
            this.txbSource7820.Size = new System.Drawing.Size(468, 32);
            this.txbSource7820.StyleController = this.layoutControl1;
            this.txbSource7820.TabIndex = 1;
            this.txbSource7820.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbSource7820_ButtonClick);
            // txbCommonRoot
            this.txbCommonRoot.Location = new System.Drawing.Point(160, 84);
            this.txbCommonRoot.MenuManager = this.barManagerTP;
            this.txbCommonRoot.Name = "txbCommonRoot";
            this.txbCommonRoot.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbCommonRoot.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbCommonRoot.Properties.Appearance.Options.UseFont = true;
            this.txbCommonRoot.Properties.Appearance.Options.UseForeColor = true;
            this.txbCommonRoot.Properties.Buttons.Clear();
            this.txbCommonRoot.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis) });
            this.txbCommonRoot.Size = new System.Drawing.Size(468, 32);
            this.txbCommonRoot.StyleController = this.layoutControl1;
            this.txbCommonRoot.TabIndex = 2;
            this.txbCommonRoot.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbCommonRoot_ButtonClick);
            // Root
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { this.lcSource7810, this.lcSource7820, this.lcCommonRoot });
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(640, 128);
            this.Root.TextVisible = false;
            // lcSource7810
            this.lcSource7810.AllowHtmlStringInCaption = true;
            this.lcSource7810.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcSource7810.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcSource7810.AppearanceItemCaption.Options.UseFont = true;
            this.lcSource7810.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcSource7810.Control = this.txbSource7810;
            this.lcSource7810.Location = new System.Drawing.Point(0, 0);
            this.lcSource7810.Name = "lcSource7810";
            this.lcSource7810.Size = new System.Drawing.Size(620, 36);
            this.lcSource7810.Text = "7810下載路徑<color=red>*</color>";
            this.lcSource7810.TextSize = new System.Drawing.Size(144, 24);
            // lcSource7820
            this.lcSource7820.AllowHtmlStringInCaption = true;
            this.lcSource7820.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcSource7820.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcSource7820.AppearanceItemCaption.Options.UseFont = true;
            this.lcSource7820.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcSource7820.Control = this.txbSource7820;
            this.lcSource7820.Location = new System.Drawing.Point(0, 36);
            this.lcSource7820.Name = "lcSource7820";
            this.lcSource7820.Size = new System.Drawing.Size(620, 36);
            this.lcSource7820.Text = "7820下載路徑<color=red>*</color>";
            this.lcSource7820.TextSize = new System.Drawing.Size(144, 24);
            // lcCommonRoot
            this.lcCommonRoot.AllowHtmlStringInCaption = true;
            this.lcCommonRoot.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.lcCommonRoot.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcCommonRoot.AppearanceItemCaption.Options.UseFont = true;
            this.lcCommonRoot.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcCommonRoot.Control = this.txbCommonRoot;
            this.lcCommonRoot.Location = new System.Drawing.Point(0, 72);
            this.lcCommonRoot.Name = "lcCommonRoot";
            this.lcCommonRoot.Size = new System.Drawing.Size(620, 36);
            this.lcCommonRoot.Text = "建立檔案路徑<color=red>*</color>";
            this.lcCommonRoot.TextSize = new System.Drawing.Size(144, 24);
            // f318_Settings
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 177);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            this.Name = "f318_Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "路徑設定";
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSource7810.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSource7820.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCommonRoot.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSource7810)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSource7820)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCommonRoot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lcSource7810;
        private DevExpress.XtraLayout.LayoutControlItem lcSource7820;
        private DevExpress.XtraLayout.LayoutControlItem lcCommonRoot;
        private DevExpress.XtraEditors.ButtonEdit txbSource7810;
        private DevExpress.XtraEditors.ButtonEdit txbSource7820;
        private DevExpress.XtraEditors.ButtonEdit txbCommonRoot;
    }
}
