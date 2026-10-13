using BusinessLayer;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public partial class f208_ReadingHistory : XtraForm
    {
        private readonly string userId, sn;
        private DataTable readings;
        private bool busy;
        public f208_ReadingHistory(string userId, string sn)
        {
            InitializeComponent(); this.userId = userId; this.sn = sn;
            Text = "溫濕度歷史 — " + sn;
            dateFrom.DateTime = dt208_EnvironmentRules.VietnamNow.Date; dateTo.DateTime = dateFrom.DateTime;
            FormClosing += (s, e) => { if (busy) e.Cancel = true; };
            Load += async (s, e) => await LoadReadingsAsync();
        }
        private DataRow SelectedReading() => view.GetFocusedDataRow();
        private void UpdateButtons()
        {
            var row = SelectedReading();
            btnLoad.Enabled = dateFrom.Enabled = dateTo.Enabled = !busy;
            btnAction.Enabled = !busy && row != null && !row.IsNull("CanHandle") && Convert.ToBoolean(row["CanHandle"])
                && row["Status"].ToString().Contains("超限");
            btnActionHistory.Enabled = !busy && row != null;
            btnConflicts.Enabled = !busy && row != null && Convert.ToInt32(row["ConflictCount"]) > 0;
            btnExport.Enabled = btnPrint.Enabled = !busy && readings != null;
        }
        private async Task LoadReadingsAsync()
        {
            if (busy) return;
            DateTime from = dateFrom.DateTime.Date, to = dateTo.DateTime.Date;
            if (dateFrom.EditValue == null || dateTo.EditValue == null || from > to || to >= DateTime.MaxValue.Date)
            { MsgTP.MsgError("請選擇有效的起迄日期。"); return; }
            busy = true; UpdateButtons();
            try
            {
                var data = await Task.Run(() => dt208_EnvironmentBUS.Instance.GetHistory(userId, sn, from, to.AddDays(1)));
                if (data.Rows.Count > 10000)
                {
                    readings = null; grid.DataSource = null; chartTemperature.Series.Clear(); chartHumidity.Series.Clear();
                    lblSummary.Text = "資料超過10,000筆，請縮小日期範圍。"; return;
                }
                data.Columns.Add("Status");
                foreach (DataRow row in data.Rows) row["Status"] = dt208_EnvironmentRules.MeasurementStatus(row);
                data.DefaultView.Sort = "MeasuredAt ASC";
                readings = data; grid.DataSource = data.DefaultView;
                f208_DataList.FormatColumns(view);
                ApplyFilter(); DrawCharts();
                int abnormal = data.AsEnumerable().Count(r => r["Status"].ToString().Contains("超限"));
                lblSummary.Text = $"SN {sn} | 共 {data.Rows.Count} 筆 | 超限 {abnormal} 筆 | 時間 UTC+07";
            }
            catch (Exception ex) { MsgTP.MsgError("歷史載入失敗：\r\n" + ex.Message); }
            finally { busy = false; UpdateButtons(); }
        }
        private void ApplyFilter()
        {
            if (readings != null) readings.DefaultView.RowFilter = chkAbnormal.Checked ? "Status LIKE '%超限%'" : "";
        }
        private void DrawCharts()
        {
            DrawChart(chartTemperature, "溫度 °C", "Temperature", "TempMin", "TempMax");
            DrawChart(chartHumidity, "濕度 %RH", "Humidity", "HumidityMin", "HumidityMax");
        }
        private void DrawChart(ChartControl chart, string caption, string value, string min, string max)
        {
            chart.Series.Clear();
            var actual = new Series(caption, ViewType.Line) { ArgumentScaleType = ScaleType.DateTime };
            var lower = new Series("下限", ViewType.Line) { ArgumentScaleType = ScaleType.DateTime };
            var upper = new Series("上限", ViewType.Line) { ArgumentScaleType = ScaleType.DateTime };
            ((LineSeriesView)actual.View).Color = Color.RoyalBlue; ((LineSeriesView)lower.View).Color = Color.DarkOrange; ((LineSeriesView)upper.View).Color = Color.Firebrick;
            // The thresholds are taken from the configuration effective for each reading.
            foreach (DataRow row in readings.AsEnumerable().OrderBy(r => r["MeasuredAt"]))
            {
                DateTime time = (DateTime)row["MeasuredAt"];
                actual.Points.Add(new SeriesPoint(time, Convert.ToDouble(row[value])));
                lower.Points.Add(new SeriesPoint(time, row.IsNull(min) ? double.NaN : Convert.ToDouble(row[min])));
                upper.Points.Add(new SeriesPoint(time, row.IsNull(max) ? double.NaN : Convert.ToDouble(row[max])));
            }
            chart.Series.AddRange(new[] { actual, lower, upper });
            if (chart.Diagram is XYDiagram diagram)
            {
                diagram.AxisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Continuous;
                diagram.AxisX.Label.TextPattern = "{A:MM/dd HH:mm}";
                diagram.AxisY.Title.Text = caption; diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                diagram.EnableAxisXZooming = true; diagram.EnableAxisXScrolling = true;
            }
        }
        private async void Load_Click(object sender, EventArgs e) { await LoadReadingsAsync(); }
        private async void Action_Click(object sender, EventArgs e)
        {
            var row = SelectedReading();
            if (!btnAction.Enabled || row == null) return;
            using (var form = new f208_ReadingAction(userId, Convert.ToInt64(row["Id"]))) form.ShowDialog(this);
            await LoadReadingsAsync();
        }
        private async void ActionHistory_Click(object sender, EventArgs e)
        {
            var row = SelectedReading(); if (busy || row == null) return;
            long id = Convert.ToInt64(row["Id"]);
            await ShowDataAsync("處理紀錄", () => dt208_EnvironmentBUS.Instance.GetActions(userId, id));
        }
        private async void Conflicts_Click(object sender, EventArgs e)
        {
            var row = SelectedReading(); if (busy || row == null) return;
            long id = Convert.ToInt64(row["Id"]);
            await ShowDataAsync("同時點資料衝突 — 原始數據保持不變", () => dt208_EnvironmentBUS.Instance.GetConflicts(userId, id));
        }
        private async Task ShowDataAsync(string title, Func<DataTable> query)
        {
            busy = true; UpdateButtons();
            try { var data = await Task.Run(query); using (var form = new f208_DataList(title, data)) form.ShowDialog(this); }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
            finally { busy = false; UpdateButtons(); }
        }
        private void Export_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "溫濕度_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx" })
                if (dialog.ShowDialog() == DialogResult.OK) try { grid.ExportToXlsx(dialog.FileName); } catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }
    }
}
