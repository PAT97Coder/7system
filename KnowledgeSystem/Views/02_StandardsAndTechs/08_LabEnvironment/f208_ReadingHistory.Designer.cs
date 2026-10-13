namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    partial class f208_ReadingHistory
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraEditors.DateEdit dateFrom, dateTo;
        private DevExpress.XtraEditors.CheckEdit chkAbnormal;
        private DevExpress.XtraEditors.LabelControl lblSummary;
        private DevExpress.XtraEditors.SimpleButton btnLoad, btnAction, btnActionHistory, btnConflicts, btnExport, btnPrint;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;
        private DevExpress.XtraCharts.ChartControl chartTemperature, chartHumidity;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var toolbar = new System.Windows.Forms.FlowLayoutPanel { Dock = System.Windows.Forms.DockStyle.Top, Height = 75, Padding = new System.Windows.Forms.Padding(6), AutoScroll = true };
            dateFrom = new DevExpress.XtraEditors.DateEdit { Width = 125 }; dateTo = new DevExpress.XtraEditors.DateEdit { Width = 125 };
            dateFrom.Properties.DisplayFormat.FormatString = dateTo.Properties.DisplayFormat.FormatString = "yyyy/MM/dd";
            btnLoad = new DevExpress.XtraEditors.SimpleButton { Text = "查詢", Width = 70 }; btnLoad.Click += Load_Click;
            chkAbnormal = new DevExpress.XtraEditors.CheckEdit { Text = "僅超限", Width = 100 }; chkAbnormal.CheckedChanged += (s, e) => ApplyFilter();
            btnAction = new DevExpress.XtraEditors.SimpleButton { Text = "處理超限", Width = 100, Enabled = false }; btnAction.Click += Action_Click;
            btnActionHistory = new DevExpress.XtraEditors.SimpleButton { Text = "處理紀錄", Width = 100, Enabled = false }; btnActionHistory.Click += ActionHistory_Click;
            btnConflicts = new DevExpress.XtraEditors.SimpleButton { Text = "資料衝突", Width = 100, Enabled = false }; btnConflicts.Click += Conflicts_Click;
            btnExport = new DevExpress.XtraEditors.SimpleButton { Text = "匯出Excel", Width = 100, Enabled = false }; btnExport.Click += Export_Click;
            btnPrint = new DevExpress.XtraEditors.SimpleButton { Text = "列印預覽", Width = 100, Enabled = false }; btnPrint.Click += (s, e) => grid.ShowPrintPreview();
            toolbar.Controls.AddRange(new System.Windows.Forms.Control[] { new DevExpress.XtraEditors.LabelControl { Text = "日期", Padding = new System.Windows.Forms.Padding(0, 5, 0, 0) }, dateFrom, dateTo, btnLoad, chkAbnormal, btnAction, btnActionHistory, btnConflicts, btnExport, btnPrint });
            lblSummary = new DevExpress.XtraEditors.LabelControl { Dock = System.Windows.Forms.DockStyle.Bottom, Height = 30, AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None, Padding = new System.Windows.Forms.Padding(8), Text = "載入中…" };
            var split = new System.Windows.Forms.SplitContainer { Dock = System.Windows.Forms.DockStyle.Fill, Orientation = System.Windows.Forms.Orientation.Horizontal, Size = new System.Drawing.Size(1100, 700), SplitterDistance = 350 };
            grid = new DevExpress.XtraGrid.GridControl { Dock = System.Windows.Forms.DockStyle.Fill };
            view = new DevExpress.XtraGrid.Views.Grid.GridView(grid); grid.MainView = view; view.OptionsBehavior.Editable = false;
            view.FocusedRowChanged += (s, e) => UpdateButtons();
            view.RowStyle += (s, e) => { var row = view.GetDataRow(e.RowHandle); if (row != null && row.Table.Columns.Contains("Status") && row["Status"].ToString().Contains("超限")) e.Appearance.ForeColor = System.Drawing.Color.Firebrick; };
            split.Panel1.Controls.Add(grid);
            var charts = new System.Windows.Forms.TableLayoutPanel { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            charts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)); charts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
            chartTemperature = new DevExpress.XtraCharts.ChartControl { Dock = System.Windows.Forms.DockStyle.Fill }; chartHumidity = new DevExpress.XtraCharts.ChartControl { Dock = System.Windows.Forms.DockStyle.Fill };
            charts.Controls.Add(chartTemperature, 0, 0); charts.Controls.Add(chartHumidity, 1, 0); split.Panel2.Controls.Add(charts);
            Controls.Add(split); Controls.Add(toolbar); Controls.Add(lblSummary);
            Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1250, 800); MinimumSize = new System.Drawing.Size(950, 650);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Name = "f208_ReadingHistory";
        }
    }
}
