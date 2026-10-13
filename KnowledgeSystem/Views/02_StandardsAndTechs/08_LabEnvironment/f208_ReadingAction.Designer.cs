namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_ReadingAction
    {
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TableLayoutPanel contentPanel;
        private DevExpress.XtraEditors.MemoEdit txbCause, txbAction, txbResult;
        private DevExpress.XtraEditors.CheckEdit chkClosed;
        private DevExpress.XtraEditors.SimpleButton btnSave, btnCancel;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            contentPanel = new System.Windows.Forms.TableLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, Padding = new System.Windows.Forms.Padding(15), ColumnCount = 2, RowCount = 4 };
            contentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110));
            contentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            txbCause = new DevExpress.XtraEditors.MemoEdit { Dock = System.Windows.Forms.DockStyle.Fill }; txbAction = new DevExpress.XtraEditors.MemoEdit { Dock = System.Windows.Forms.DockStyle.Fill }; txbResult = new DevExpress.XtraEditors.MemoEdit { Dock = System.Windows.Forms.DockStyle.Fill };
            txbCause.Properties.MaxLength = txbAction.Properties.MaxLength = txbResult.Properties.MaxLength = 1000;
            var memos = new[] { txbCause, txbAction, txbResult }; var labels = new[] { "原因 *", "處理措施 *", "處理結果 / 複測" };
            for (int i = 0; i < 3; i++) { contentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30)); contentPanel.Controls.Add(new DevExpress.XtraEditors.LabelControl { Text = labels[i] }, 0, i); contentPanel.Controls.Add(memos[i], 1, i); }
            chkClosed = new DevExpress.XtraEditors.CheckEdit { Text = "已完成處理 / 結案", Dock = System.Windows.Forms.DockStyle.Fill }; contentPanel.Controls.Add(chkClosed, 1, 3);
            var buttons = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Bottom, Height = 45, FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft };
            btnSave = new DevExpress.XtraEditors.SimpleButton { Text = "保存紀錄", Width = 100, Height = 32 }; btnSave.Click += Save_Click;
            btnCancel = new DevExpress.XtraEditors.SimpleButton { Text = "取消", Width = 100, Height = 32, DialogResult = System.Windows.Forms.DialogResult.Cancel };
            buttons.Controls.Add(btnCancel); buttons.Controls.Add(btnSave); Controls.Add(contentPanel); Controls.Add(buttons);
            AcceptButton = btnSave; CancelButton = btnCancel; Text = "記錄超限處理";
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(650, 450); StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f208_ReadingAction";
        }
    }
}
