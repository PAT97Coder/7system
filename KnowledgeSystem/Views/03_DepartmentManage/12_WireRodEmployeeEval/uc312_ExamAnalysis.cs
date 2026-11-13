using BusinessLayer;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class uc312_ExamAnalysis : XtraUserControl
    {
        private readonly dt312_ExamAnalysisBUS bus = dt312_ExamAnalysisBUS.Instance;
        private readonly Dictionary<string, string> chartNames = new Dictionary<string, string>();
        private bool changingFilters, authorized, binding;
        private volatile int loadVersion;
        private bool loading;
        private SelectionData loadedSelection;
        private DateTime loadedAt;

        public uc312_ExamAnalysis()
        {
            InitializeComponent();
            TraditionalGridLocalizer.Install();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            loadVersion++;
            base.OnHandleDestroyed(e);
        }

        private void Analysis_Load(object sender, EventArgs e) => Reload();
        private void Refresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => Reload();

        private void Filter_EditValueChanged(object sender, EventArgs e)
        {
            if (changingFilters || !authorized) return;
            changingFilters = true;
            try
            {
                bool single = Convert.ToString(barScope.EditValue) == "單次考試";
                barExam.Enabled = single;
                if (single && barExam.EditValue == null)
                {
                    var exams = examEditor.DataSource as List<Exam312AnalysisExamOption>;
                    barExam.EditValue = exams?.FirstOrDefault()?.Id;
                }
            }
            finally { changingFilters = false; }
            Reload();
        }

        private bool IsCurrent(int version) => !IsDisposed && !Disposing && version == loadVersion;

        private void EnableFilters(bool enabled)
        {
            barScope.Enabled = barPerson.Enabled = btnRefresh.Enabled = enabled;
            barExam.Enabled = enabled && Convert.ToString(barScope.EditValue) == "單次考試";
            UpdateExportState();
        }

        private sealed class SelectionData
        {
            internal Exam312AnalysisOptions Options;
            internal int? ExamId;
            internal string PersonId;
            internal bool NoExams;
            internal Exam312AnalysisResult Result;
        }

        // Chỉ xử lý dữ liệu ở worker; không đọc/ghi bất kỳ control nào tại đây.
        private SelectionData FetchSelection(string actor, bool single, int? requestedExam, string personId)
        {
            var options = bus.GetOptions(actor, single ? requestedExam : null);
            int? examId = single ? requestedExam : null;
            if (single && (!examId.HasValue || !options.Exams.Any(x => x.Id == examId.Value)))
            {
                examId = options.Exams.FirstOrDefault()?.Id;
                personId = "";
                if (examId.HasValue) options = bus.GetOptions(actor, examId);
            }
            if (single && !examId.HasValue)
            {
                options.People = options.People.Where(p => p.UserId == "").ToList();
                return new SelectionData { Options = options, PersonId = "", NoExams = true, Result = new Exam312AnalysisResult() };
            }
            if (!options.People.Any(p => p.UserId == personId)) personId = "";
            return new SelectionData { Options = options, ExamId = examId, PersonId = personId,
                Result = bus.GetStatistics(actor, examId, personId) };
        }

        private void Reload()
        {
            int version = ++loadVersion;
            string actor = TPConfigs.LoginUser?.Id;
            bool single = Convert.ToString(barScope.EditValue) == "單次考試";
            int? examId = single && barExam.EditValue != null ? (int?)Convert.ToInt32(barExam.EditValue) : null;
            string personId = Convert.ToString(barPerson.EditValue) ?? "";
            loading = true; statusLabel.Text = "正在載入錯題分析，請稍候…";
            ClearResults();
            Task.Run(() => FetchSelection(actor, single, examId, personId)).ContinueWith(task =>
            {
                // Quan sát lỗi kể cả khi UC đã đóng, không để task lỗi bị bỏ quên.
                var error = task.Exception?.GetBaseException();
                if (!IsCurrent(version) || !IsHandleCreated) return;
                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (!IsCurrent(version)) return;
                        ApplySelection(error == null ? task.Result : null, error);
                    }));
                }
                catch (InvalidOperationException) when (IsDisposed || Disposing || !IsHandleCreated)
                {
                    // Handle bị hủy trong lúc chuyển kết quả về UI.
                }
            }, TaskScheduler.Default);
        }

        private void ApplySelection(SelectionData data, Exception error)
        {
            try
            {
                if (error != null) throw error;
                authorized = true;
                changingFilters = true;
                try
                {
                    examEditor.DataSource = data.Options.Exams; barExam.EditValue = data.ExamId;
                    personEditor.DataSource = data.Options.People; barPerson.EditValue = data.PersonId;
                    ResizeLookupPopup(examEditor, examsView, data.Options.Exams.Count);
                    ResizeLookupPopup(personEditor, peopleView, data.Options.People.Count);
                }
                finally { changingFilters = false; }
                var result = data.Result;
                loadedSelection = data; loadedAt = DateTime.Now;
                binding = true;
                try { resultsGrid.DataSource = result.Rows; }
                finally { binding = false; }
                UpdateChart();
                statusLabel.Text = (data.NoExams ? "沒有可供分析的考試。" : result.Rows.Count == 0 ? "沒有可供分析的選擇題資料。" : "分析完成。")
                    + " 已提交試卷：" + result.SubmittedPapers + "；略過無效試卷：" + result.SkippedPapers
                    + "；略過無效題目：" + result.SkippedQuestions + "。申論題不列入統計。";
            }
            catch (Exception ex)
            {
                ClearResults();
                if (ex is UnauthorizedAccessException) { authorized = false; EnableFilters(false); }
                statusLabel.Text = Exam312Ui.ErrorText(ex);
                if (ex is UnauthorizedAccessException) statusLabel.Text = "您沒有錯題分析的權限，請先登錄功能並由管理員授權。";
            }
            finally
            {
                loading = false; EnableFilters(authorized);
            }
        }

        private void ClearResults()
        {
            loadedSelection = null; btnExport.Enabled = false;
            binding = true;
            try { resultsGrid.DataSource = new List<Exam312AnalysisRow>(); chartNames.Clear(); chart.Series[0].Points.Clear(); }
            finally { binding = false; }
        }

        private void ResizeLookupPopup(DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit editor, GridView view, int count)
        {
            // Rộng theo BestFitMode; cao theo số dòng để danh sách ít người không dư khoảng trắng.
            int rowHeight = Math.Max(view.RowHeight, view.Appearance.Row.Font.Height + 8);
            int height = (Math.Min(Math.Max(count, 1), 10) + 4) * rowHeight + 32;
            int maxHeight = Math.Max(180, System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height - 120);
            editor.PopupFormSize = new System.Drawing.Size(340, Math.Min(height, maxHeight));
        }

        private void Results_Changed(object sender, EventArgs e)
        {
            if (!binding) { UpdateChart(); UpdateExportState(); }
        }

        private void UpdateExportState() => btnExport.Enabled = authorized && !loading && loadedSelection != null && resultsView.DataRowCount > 0;

        private List<Exam312AnalysisRow> VisibleRows()
        {
            var rows = new List<Exam312AnalysisRow>();
            for (int i = 0; i < resultsView.DataRowCount; i++)
            {
                var row = resultsView.GetRow(resultsView.GetVisibleRowHandle(i)) as Exam312AnalysisRow;
                if (row != null) rows.Add(row);
            }
            return rows;
        }

        private rp312_ExamAnalysis CreateAnalysisReport()
        {
            bus.EnsureCanAnalyze(TPConfigs.LoginUser?.Id);
            if (loading || loadedSelection == null || resultsView.DataRowCount == 0)
                throw new InvalidOperationException("沒有可匯出的分析資料，請先完成查詢。");
            var data = loadedSelection;
            string scope = data.ExamId.HasValue ? "單次考試" : "全部歷史";
            string exam = data.ExamId.HasValue ? data.Options.Exams.FirstOrDefault(x => x.Id == data.ExamId.Value)?.Name ?? "未知考試" : "全部考試";
            string person = data.Options.People.FirstOrDefault(x => x.UserId == data.PersonId)?.Display ?? data.PersonId;
            return new rp312_ExamAnalysis(VisibleRows(), scope, exam, person,
                resultsView.GetFilterDisplayText(resultsView.ActiveFilterCriteria), statusLabel.Text, loadedAt);
        }

        private void Export_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!btnExport.Enabled || loading) return;
            reportSaveDialog.FileName = "錯題分析_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (reportSaveDialog.ShowDialog(FindForm()) != System.Windows.Forms.DialogResult.OK) return;
            string path = reportSaveDialog.FileName;
            string extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension != ".pdf" && extension != ".xlsx")
            {
                MsgTP.MsgShowInfomation("請選擇 PDF（.pdf）或 Excel（.xlsx）格式。");
                return;
            }
            try
            {
                using (var wait = DevExpress.XtraSplashScreen.SplashScreenManager.ShowOverlayForm(this)) SaveAnalysisReport(path);
                MsgTP.MsgShowInfomation("分析報告已匯出：\r\n" + path);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException) { authorized = false; ClearResults(); EnableFilters(false); }
                MsgTP.MsgError("無法匯出分析報告：\r\n" + Exam312Ui.ErrorText(ex));
            }
        }

        private void SaveAnalysisReport(string path)
        {
            using (var report = CreateAnalysisReport())
            {
                string temp = Path.Combine(Path.GetDirectoryName(path), "." + Path.GetFileName(path) + "." + Guid.NewGuid().ToString("N") + ".tmp");
                try
                {
                    if (string.Equals(Path.GetExtension(path), ".pdf", StringComparison.OrdinalIgnoreCase)) report.ExportToPdf(temp);
                    else if (string.Equals(Path.GetExtension(path), ".xlsx", StringComparison.OrdinalIgnoreCase)) report.ExportToXlsx(temp);
                    else throw new ArgumentException("不支援的報告格式。");
                    // Chỉ thay file đích sau khi tạo xong toàn bộ báo cáo.
                    if (File.Exists(path)) File.Replace(temp, path, null);
                    else File.Move(temp, path);
                }
                finally { if (File.Exists(temp)) File.Delete(temp); }
            }
        }

        private void UpdateChart()
        {
            var series = chart.Series[0];
            chartNames.Clear();
            series.Points.Clear();
            foreach (var row in VisibleRows())
            {
                chartNames[row.ChartLabel] = row.GroupName == "未知群組（ID：" + row.GroupId + "）" ? "未知群組" : row.GroupName;
                series.Points.Add(new SeriesPoint(row.ChartLabel, (double)row.ErrorRate) { ToolTipHint = row.ChartToolTip });
            }
            var diagram = (XYDiagram)chart.Diagram;
            diagram.AxisX.VisualRange.Auto = true;
            if (series.Points.Count > 8)
            {
                diagram.AxisX.VisualRange.Auto = false;
                diagram.AxisX.VisualRange.SetMinMaxValues(series.Points[0].Argument, series.Points[7].Argument);
            }
        }

        private void Chart_CustomDrawAxisLabel(object sender, CustomDrawAxisLabelEventArgs e)
        {
            string name;
            if (chartNames.TryGetValue(e.Item.Text, out name)) e.Item.Text = name;
        }

        private void Results_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu != null) TraditionalGridLocalizer.StyleMenu(e.Menu);
        }
    }
}
