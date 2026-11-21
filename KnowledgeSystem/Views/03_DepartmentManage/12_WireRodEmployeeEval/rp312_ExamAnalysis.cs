using BusinessLayer;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public sealed class Exam312AnalysisReportPage
    {
        public string Title { get; set; }
        public List<Exam312AnalysisRow> Rows { get; set; }
    }

    public sealed class Exam312AnalysisReportSource
    {
        public List<Exam312AnalysisRow> Rows { get; set; }
        public List<Exam312AnalysisReportPage> ChartPages { get; set; }
    }

    public partial class rp312_ExamAnalysis : XtraReport
    {
        private readonly Dictionary<string, string> chartNames = new Dictionary<string, string>();

        public rp312_ExamAnalysis()
        {
            InitializeComponent();
        }

        public rp312_ExamAnalysis(IEnumerable<Exam312AnalysisRow> rows, string scope, string exam, string person,
            string filter, string status, DateTime loadedAt) : this()
        {
            // Sao chép dữ liệu đã hiển thị: không truy vấn lại và không phụ thuộc thay đổi của màn hình gọi.
            var copy = rows.Select(r => new Exam312AnalysisRow { GroupId = r.GroupId, GroupName = r.GroupName,
                PeopleCount = r.PeopleCount, PaperCount = r.PaperCount, QuestionCount = r.QuestionCount, WrongCount = r.WrongCount }).ToList();
            if (copy.Count == 0) throw new ArgumentException("沒有可匯出的分析資料。");
            var pages = new List<Exam312AnalysisReportPage>();
            for (int start = 0; start < copy.Count; start += 8)
                pages.Add(new Exam312AnalysisReportPage { Rows = copy.Skip(start).Take(8).ToList(),
                    Title = "錯誤率比較（第 " + (start + 1) + "–" + Math.Min(start + 8, copy.Count) + " 組，共 " + copy.Count + " 組）" });
            DataSource = new[] { new Exam312AnalysisReportSource { Rows = copy, ChartPages = pages } };
            scopeLabel.Text = "範圍：" + scope + "　考試：" + exam;
            personLabel.Text = "人員：" + person;
            filterLabel.Text = "表格篩選：" + (string.IsNullOrWhiteSpace(filter) ? "無" : filter);
            timeLabel.Text = "資料載入時間：" + loadedAt.ToString("yyyy/MM/dd HH:mm:ss")
                + "　匯出時間：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            statusLabel.Text = "所選範圍資料狀態：" + status;
            noteLabel.Text = "本報告含目前可見的 " + copy.Count + " 個群組；申論題不列入統計。錯誤率＝答錯次數／題目次數 × 100%。";
            chartScopeLabel.Text = scopeLabel.Text + "\r\n" + personLabel.Text;
            DisplayName = "錯題分析報告";
            ExportOptions.Pdf.DocumentOptions.Title = "錯題分析報告";
        }

        private void ChartDetail_BeforePrint(object sender, PrintEventArgs e)
        {
            var page = chartsReport.GetCurrentRow() as Exam312AnalysisReportPage;
            chartNames.Clear(); reportChart.Series[0].Points.Clear();
            if (page == null) return;
            chartTitle.Text = page.Title;
            foreach (var row in page.Rows)
            {
                chartNames[row.ChartLabel] = row.GroupName == "未知群組（ID：" + row.GroupId + "）" ? "未知群組" : row.GroupName;
                reportChart.Series[0].Points.Add(new SeriesPoint(row.ChartLabel, (double)row.ErrorRate));
            }
        }

        private void ReportChart_CustomDrawAxisLabel(object sender, CustomDrawAxisLabelEventArgs e)
        {
            string name;
            if (chartNames.TryGetValue(e.Item.Text, out name)) e.Item.Text = name;
        }
    }
}
