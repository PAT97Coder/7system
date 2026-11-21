namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class f312_EssayQuestion
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar toolbar;
        private DevExpress.XtraBars.BarButtonItem btnSave, btnCancel;
        private DevExpress.XtraLayout.LayoutControl layout;
        private DevExpress.XtraEditors.MemoEdit questionEdit, referenceEdit;
        protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            barManager = new DevExpress.XtraBars.BarManager(components) { Form = this };
            toolbar = new DevExpress.XtraBars.Bar(barManager, "工具列") { DockStyle = DevExpress.XtraBars.BarDockStyle.Top };
            Exam312Ui.StyleBar(toolbar); barManager.MainMenu = toolbar;
            btnSave = new DevExpress.XtraBars.BarButtonItem(barManager, "儲存") { Enabled = false };
            btnCancel = new DevExpress.XtraBars.BarButtonItem(barManager, "關閉");
            btnSave.ImageOptions.SvgImage = Helpers.TPSvgimages.Confirm; btnCancel.ImageOptions.SvgImage = Helpers.TPSvgimages.Cancel;
            Exam312Ui.StyleButton(btnSave); Exam312Ui.StyleButton(btnCancel);
            btnSave.ItemClick += Save_Click; btnCancel.ItemClick += (s, e) => Close();
            toolbar.AddItem(btnSave); toolbar.AddItem(btnCancel).BeginGroup = true;
            var top = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Top };
            var bottom = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Bottom };
            var left = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Left };
            var right = new DevExpress.XtraBars.BarDockControl { Manager = barManager, Dock = System.Windows.Forms.DockStyle.Right };
            Controls.AddRange(new System.Windows.Forms.Control[] { top, bottom, left, right });
            layout = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            var root = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false, EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True, Padding = new DevExpress.XtraLayout.Utils.Padding(10) };
            layout.Root = root;
            questionEdit = new DevExpress.XtraEditors.MemoEdit(); referenceEdit = new DevExpress.XtraEditors.MemoEdit();
            questionEdit.Properties.MaxLength = 2048; referenceEdit.Properties.MaxLength = 16000;
            foreach (var edit in new[] { questionEdit, referenceEdit })
            {
                edit.StyleController = layout; edit.MenuManager = barManager;
                edit.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
                edit.Properties.Appearance.Options.UseFont = true;
            }
            var questionItem = new DevExpress.XtraLayout.LayoutControlItem { Control = questionEdit, Text = "題目<color=red>*</color>", AllowHtmlStringInCaption = true, Size = new System.Drawing.Size(736, 180), Location = new System.Drawing.Point(0, 0) };
            var referenceItem = new DevExpress.XtraLayout.LayoutControlItem { Control = referenceEdit, Text = "參考答案（不計分）", Size = new System.Drawing.Size(736, 180), Location = new System.Drawing.Point(0, 180) };
            foreach (var item in new[] { questionItem, referenceItem })
            {
                item.TextLocation = DevExpress.Utils.Locations.Top;
                item.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F); item.AppearanceItemCaption.Options.UseFont = true;
                item.Padding = new DevExpress.XtraLayout.Utils.Padding(2); item.TextToControlDistance = 4;
            }
            layout.Controls.AddRange(new System.Windows.Forms.Control[] { questionEdit, referenceEdit });
            root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { questionItem, referenceItem });
            Controls.Add(layout); layout.BringToFront();
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244); Appearance.Options.UseBackColor = true;
            ClientSize = new System.Drawing.Size(760, 460); FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; MaximizeBox = false; MinimizeBox = false;
            Name = "f312_EssayQuestion"; Text = "申論題"; ResumeLayout(false); PerformLayout();
        }
    }
}
