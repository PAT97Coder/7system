namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_ReadingAction
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarDockControl dockTop, dockBottom, dockLeft, dockRight;
        private DevExpress.XtraLayout.LayoutControl contentPanel;
        private DevExpress.XtraLayout.LayoutControlGroup layoutRoot;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar toolbar;
        private DevExpress.XtraBars.BarButtonItem btnSave, btnCancel;
        private DevExpress.XtraEditors.MemoEdit txbCause;
        private DevExpress.XtraEditors.MemoEdit txbAction;
        private DevExpress.XtraEditors.MemoEdit txbResult;
        private DevExpress.XtraEditors.CheckEdit chkClosed;
        private DevExpress.XtraLayout.LayoutControlItem txbCauseItem;
        private DevExpress.XtraLayout.LayoutControlItem txbActionItem;
        private DevExpress.XtraLayout.LayoutControlItem txbResultItem;
        private DevExpress.XtraLayout.LayoutControlItem chkClosedItem;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            SuspendLayout();
            components = new System.ComponentModel.Container();
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
            btnSave = new DevExpress.XtraBars.BarButtonItem(barManager, "確認") { Enabled = false };
            btnCancel = new DevExpress.XtraBars.BarButtonItem(barManager, "取消");
            btnSave.ImageOptions.SvgImage = Helpers.TPSvgimages.Confirm; btnCancel.ImageOptions.SvgImage = Helpers.TPSvgimages.Cancel;
            Lab208Ui.StyleDialogBarItem(btnSave); Lab208Ui.StyleDialogBarItem(btnCancel);
            btnSave.ItemClick += (s, e) => Save_Click(s, e); btnCancel.ItemClick += (s, e) => { DialogResult = System.Windows.Forms.DialogResult.Cancel; Close(); };
            btnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S);
            btnCancel.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Escape);
            toolbar.AddItem(btnSave); toolbar.AddItem(btnCancel).BeginGroup = true;
            contentPanel = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            layoutRoot = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false, EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True, Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10) };
            ((System.ComponentModel.ISupportInitialize)contentPanel).BeginInit();
            contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutRoot).BeginInit();
            contentPanel.Root = layoutRoot;

            txbCause = new DevExpress.XtraEditors.MemoEdit();
            txbCause.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            txbCauseItem = new DevExpress.XtraLayout.LayoutControlItem { Control = txbCause, Text = "原因 *", Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(680, 42) };
            txbCauseItem.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            txbCauseItem.AppearanceItemCaption.Options.UseFont = true;
            contentPanel.Controls.Add(txbCause);
            txbAction = new DevExpress.XtraEditors.MemoEdit();
            txbAction.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            txbActionItem = new DevExpress.XtraLayout.LayoutControlItem { Control = txbAction, Text = "處理措施 *", Location = new System.Drawing.Point(0, 42), Size = new System.Drawing.Size(680, 42) };
            txbActionItem.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            txbActionItem.AppearanceItemCaption.Options.UseFont = true;
            contentPanel.Controls.Add(txbAction);
            txbResult = new DevExpress.XtraEditors.MemoEdit();
            txbResult.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            txbResultItem = new DevExpress.XtraLayout.LayoutControlItem { Control = txbResult, Text = "處理結果／複測", Location = new System.Drawing.Point(0, 84), Size = new System.Drawing.Size(680, 42) };
            txbResultItem.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            txbResultItem.AppearanceItemCaption.Options.UseFont = true;
            contentPanel.Controls.Add(txbResult);
            chkClosed = new DevExpress.XtraEditors.CheckEdit();
            chkClosed.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            chkClosedItem = new DevExpress.XtraLayout.LayoutControlItem { Control = chkClosed, Text = "結案", Location = new System.Drawing.Point(0, 126), Size = new System.Drawing.Size(680, 42) };
            chkClosedItem.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            chkClosedItem.AppearanceItemCaption.Options.UseFont = true;
            contentPanel.Controls.Add(chkClosed);
            txbCause.Properties.MaxLength = txbAction.Properties.MaxLength = txbResult.Properties.MaxLength = 1000;
            chkClosed.Text = "已完成處理／結案"; chkClosedItem.TextVisible = false;
            txbCauseItem.Size = txbActionItem.Size = txbResultItem.Size = new System.Drawing.Size(680, 100);
            txbActionItem.Location = new System.Drawing.Point(0, 100); txbResultItem.Location = new System.Drawing.Point(0, 200); chkClosedItem.Location = new System.Drawing.Point(0, 300);
            foreach (var editor in new DevExpress.XtraEditors.BaseEdit[] { txbCause, txbAction, txbResult, chkClosed })
                Lab208Ui.StyleDialogEditor(editor, contentPanel, barManager);
            foreach (var item in new[] { txbCauseItem, txbActionItem, txbResultItem, chkClosedItem })
                Lab208Ui.StyleDialogItem(item);
            layoutRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { txbCauseItem, txbActionItem, txbResultItem, chkClosedItem });
            Controls.Add(contentPanel); contentPanel.BringToFront();
            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244);
            Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 8.25F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            ClientSize = new System.Drawing.Size(700, 450); MinimumSize = new System.Drawing.Size(700, 450);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; MaximizeBox = false; MinimizeBox = false; Name = "f208_ReadingAction"; Text = "記錄超限處理";
            ((System.ComponentModel.ISupportInitialize)layoutRoot).EndInit();
            ((System.ComponentModel.ISupportInitialize)contentPanel).EndInit();
            contentPanel.ResumeLayout(false);
            ResumeLayout(false); PerformLayout();
        }
    }
}
