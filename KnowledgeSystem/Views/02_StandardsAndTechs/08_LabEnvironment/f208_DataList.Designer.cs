namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_DataList
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grid = new DevExpress.XtraGrid.GridControl { Dock = System.Windows.Forms.DockStyle.Fill };
            view = new DevExpress.XtraGrid.Views.Grid.GridView(grid); grid.MainView = view;
            var buttons = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Top, Height = 42 };
            var export = new DevExpress.XtraEditors.SimpleButton { Text = "匯出Excel", Width = 110, Height = 32 }; export.Click += Export_Click;
            var print = new DevExpress.XtraEditors.SimpleButton { Text = "列印預覽", Width = 110, Height = 32 }; print.Click += (s, e) => grid.ShowPrintPreview();
            buttons.Controls.Add(export); buttons.Controls.Add(print);
            Controls.Add(grid); Controls.Add(buttons); Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; ClientSize = new System.Drawing.Size(1100, 650);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f208_DataList";
        }
    }
}
