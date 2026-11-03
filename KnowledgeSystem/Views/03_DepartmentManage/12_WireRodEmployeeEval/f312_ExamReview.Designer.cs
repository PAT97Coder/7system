namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class f312_ExamReview
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.WebBrowser browser;
        private DevExpress.XtraLayout.LayoutControl layout;
        private DevExpress.XtraLayout.LayoutControlGroup root;
        private DevExpress.XtraLayout.LayoutControlItem browserItem;
        private DevExpress.XtraBars.BarManager previewManager;
        private DevExpress.XtraBars.Bar previewBar;
        private DevExpress.XtraBars.BarButtonItem btnPrevious, btnNext, btnConfirmImport, btnCancelImport;
        private DevExpress.XtraBars.BarDockControl previewTop, previewBottom, previewLeft, previewRight;
        protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layout = new DevExpress.XtraLayout.LayoutControl(); root = new DevExpress.XtraLayout.LayoutControlGroup();
            browserItem = new DevExpress.XtraLayout.LayoutControlItem(); browser = new System.Windows.Forms.WebBrowser();
            previewManager = new DevExpress.XtraBars.BarManager(components); previewBar = new DevExpress.XtraBars.Bar();
            btnPrevious = new DevExpress.XtraBars.BarButtonItem(); btnNext = new DevExpress.XtraBars.BarButtonItem();
            btnConfirmImport = new DevExpress.XtraBars.BarButtonItem(); btnCancelImport = new DevExpress.XtraBars.BarButtonItem();
            previewTop = new DevExpress.XtraBars.BarDockControl(); previewBottom = new DevExpress.XtraBars.BarDockControl();
            previewLeft = new DevExpress.XtraBars.BarDockControl(); previewRight = new DevExpress.XtraBars.BarDockControl();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layout).BeginInit(); ((System.ComponentModel.ISupportInitialize)root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)browserItem).BeginInit(); ((System.ComponentModel.ISupportInitialize)previewManager).BeginInit();
            layout.Dock = System.Windows.Forms.DockStyle.Fill; layout.Name = "layout"; layout.AllowCustomization = false;
            root.Name = "root"; root.GroupBordersVisible = false; layout.Root = root;
            browser.Name = "browser"; browser.ScriptErrorsSuppressed = true;
            browser.AllowWebBrowserDrop = false; browser.IsWebBrowserContextMenuEnabled = false;
            browser.Navigating += (s, e) => { if (e.Url != null && e.Url.AbsoluteUri != "about:blank") e.Cancel = true; };
            layout.Controls.Add(browser); browserItem.Name = "browserItem"; browserItem.Control = browser; browserItem.TextVisible = false;
            root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { browserItem });
            previewManager.Form = this; previewManager.Bars.Add(previewBar);
            previewManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] { btnPrevious, btnNext, btnConfirmImport, btnCancelImport });
            previewManager.DockControls.Add(previewTop); previewManager.DockControls.Add(previewBottom);
            previewManager.DockControls.Add(previewLeft); previewManager.DockControls.Add(previewRight);
            previewTop.Manager = previewBottom.Manager = previewLeft.Manager = previewRight.Manager = previewManager;
            previewTop.Dock = System.Windows.Forms.DockStyle.Top; previewBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            previewLeft.Dock = System.Windows.Forms.DockStyle.Left; previewRight.Dock = System.Windows.Forms.DockStyle.Right;
            previewTop.Name = "previewTop"; previewBottom.Name = "previewBottom"; previewLeft.Name = "previewLeft"; previewRight.Name = "previewRight";
            previewBar.BarName = "匯入預覽工具列"; previewBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            previewBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
                new DevExpress.XtraBars.LinkPersistInfo(btnPrevious), new DevExpress.XtraBars.LinkPersistInfo(btnNext),
                new DevExpress.XtraBars.LinkPersistInfo(btnConfirmImport, true), new DevExpress.XtraBars.LinkPersistInfo(btnCancelImport)
            });
            Exam312Ui.StyleBar(previewBar); previewBar.Visible = false;
            btnPrevious.Name = "btnPrevious"; btnPrevious.Caption = "上一題"; btnPrevious.Id = 0;
            btnNext.Name = "btnNext"; btnNext.Caption = "下一題"; btnNext.Id = 1;
            btnConfirmImport.Name = "btnConfirmImport"; btnConfirmImport.Caption = "確認匯入"; btnConfirmImport.Id = 2; btnConfirmImport.Enabled = false;
            btnCancelImport.Name = "btnCancelImport"; btnCancelImport.Caption = "取消"; btnCancelImport.Id = 3;
            btnPrevious.ImageOptions.SvgImage = btnNext.ImageOptions.SvgImage = Helpers.TPSvgimages.View;
            btnConfirmImport.ImageOptions.SvgImage = Helpers.TPSvgimages.Confirm; btnCancelImport.ImageOptions.SvgImage = Helpers.TPSvgimages.Cancel;
            Exam312Ui.StyleButton(btnPrevious); Exam312Ui.StyleButton(btnNext); Exam312Ui.StyleButton(btnConfirmImport); Exam312Ui.StyleButton(btnCancelImport);
            btnPrevious.ItemClick += PreviewPrevious_Click; btnNext.ItemClick += PreviewNext_Click;
            btnConfirmImport.ItemClick += ConfirmImport_Click; btnCancelImport.ItemClick += CancelImport_Click;
            Controls.AddRange(new System.Windows.Forms.Control[] { layout, previewTop, previewBottom, previewLeft, previewRight });
            layout.BringToFront(); ClientSize = new System.Drawing.Size(1100, 780); MinimumSize = new System.Drawing.Size(800, 550);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f312_ExamReview"; Text = "作答詳情";
            ((System.ComponentModel.ISupportInitialize)previewManager).EndInit(); ((System.ComponentModel.ISupportInitialize)browserItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)root).EndInit(); ((System.ComponentModel.ISupportInitialize)layout).EndInit();
            ResumeLayout(false); PerformLayout();
        }
    }
}
