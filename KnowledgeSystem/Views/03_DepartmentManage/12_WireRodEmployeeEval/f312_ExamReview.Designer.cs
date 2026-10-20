namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class f312_ExamReview
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.WebBrowser browser;
        protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var layout = new DevExpress.XtraLayout.LayoutControl { Dock = System.Windows.Forms.DockStyle.Fill, AllowCustomization = false };
            var root = new DevExpress.XtraLayout.LayoutControlGroup { GroupBordersVisible = false };
            layout.Root = root;
            browser = new System.Windows.Forms.WebBrowser { ScriptErrorsSuppressed = true, AllowWebBrowserDrop = false, IsWebBrowserContextMenuEnabled = false };
            browser.Navigating += (s, e) => { if (e.Url != null && e.Url.AbsoluteUri != "about:blank") e.Cancel = true; };
            layout.Controls.Add(browser);
            root.AddItem(new DevExpress.XtraLayout.LayoutControlItem { Control = browser, TextVisible = false });
            Controls.Add(layout); ClientSize = new System.Drawing.Size(1100, 780); MinimumSize = new System.Drawing.Size(800, 550);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f312_ExamReview"; Text = "作答詳情";
        }
    }
}
