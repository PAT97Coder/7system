using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_ExamReview : XtraForm
    {
        private IList<DataAccessLayer.dt312_QuestionImportItem> importItems;
        private int importIndex;
        private bool isResultView;

        private void EnableResultPrinting()
        {
            isResultView = true;
            previewBar.BarName = "測驗結果工具列";
            previewBar.Visible = true;
            btnPrevious.Visibility = btnNext.Visibility = btnConfirmImport.Visibility = btnCancelImport.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnPrint.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
        }

        private void Browser_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            if (e.Url != null && e.Url.AbsoluteUri != "about:blank") { e.Cancel = true; return; }
            btnPrint.Enabled = false;
        }

        private void Browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            btnPrint.Enabled = isResultView && browser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete
                && browser.Document?.Body != null;
        }

        private void Print_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!isResultView || !btnPrint.Enabled) return;
            try { browser.ShowPrintDialog(); }
            catch (System.Exception ex) { MsgTP.MsgError("無法列印測驗結果：\r\n" + Exam312Ui.ErrorText(ex)); }
        }

        public f312_ExamReview(DataAccessLayer.dt312_Questions question, List<DataAccessLayer.dt312_Answers> answers)
        {
            InitializeComponent(); Text = "預覽題目";
            browser.DocumentText = BuildQuestionPreviewHtml(question, answers, 1);
        }

        public f312_ExamReview(IList<DataAccessLayer.dt312_QuestionImportItem> items)
        {
            if (items == null || items.Count == 0) throw new System.ArgumentException("沒有可預覽的題目。");
            InitializeComponent(); importItems = items; previewBar.Visible = true;
            ShowImportQuestion();
        }

        private void ShowImportQuestion()
        {
            var item = importItems[importIndex];
            Text = "題目匯入預覽 — " + (importIndex + 1) + " / " + importItems.Count;
            browser.DocumentText = BuildQuestionPreviewHtml(item.Question, item.Answers, importIndex + 1);
            btnPrevious.Enabled = importIndex > 0;
            btnNext.Enabled = importIndex < importItems.Count - 1;
            btnConfirmImport.Enabled = importIndex == importItems.Count - 1;
        }

        private void PreviewPrevious_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { if (importItems != null && importIndex > 0) { importIndex--; ShowImportQuestion(); } }
        private void PreviewNext_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { if (importItems != null && importIndex < importItems.Count - 1) { importIndex++; ShowImportQuestion(); } }
        private void ConfirmImport_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { if (importItems != null && btnConfirmImport.Enabled) { DialogResult = System.Windows.Forms.DialogResult.OK; Close(); } }
        private void CancelImport_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        { DialogResult = System.Windows.Forms.DialogResult.Cancel; Close(); }

        public static string BuildQuestionPreviewHtml(DataAccessLayer.dt312_Questions question, IEnumerable<DataAccessLayer.dt312_Answers> answers, int index)
        {
            string value = question.IsEssay ? question.ReferenceAnswer
                : string.Join(",", answers.Where(a => a.TrueAns).Select(a => a.Id.ToString()));
            return BuildQuestionHtml(question, answers, index).Replace("</body>",
                "<p><b>" + (question.IsEssay ? "參考答案：" : "正確答案：") + "</b>" + Encode(value) + "</p></body>");
        }
        public static string BuildQuestionHtml(DataAccessLayer.dt312_Questions question, IEnumerable<DataAccessLayer.dt312_Answers> answers, int index)
        {
            var html = new StringBuilder("<!doctype html><html lang='zh-Hant'><head><meta charset='utf-8'><style>body{font-family:'Microsoft JhengHei UI';font-size:22px;padding:20px}.answer{padding:12px;margin:8px 0;background:#f4f4f4}</style></head><body>");
            html.Append("<h2>第 ").Append(index).Append(" 題（").Append(question.IsEssay ? "申論題，不計分" : question.IsmultiAns ? "複選題" : "單選題").Append("）</h2><p>")
                .Append(Encode(question.DisplayName)).Append("</p>").Append(Image(question.ImageName));
            if (!question.IsEssay)
                foreach (var answer in answers)
                    html.Append("<div class='answer'>").Append(answer.Id).Append("． ").Append(Encode(answer.DisplayName)).Append(Image(answer.ImageName)).Append("</div>");
            return html.Append("</body></html>").ToString();
        }
        public f312_ExamReview(IEnumerable<f312_DoExam.ExamResult> results, int? score, bool practice)
        {
            InitializeComponent(); Text = practice ? "模擬測驗成績與答案" : "考試作答詳情";
            EnableResultPrinting();
            browser.DocumentText = BuildHtml(results, score, practice);
        }
        public f312_ExamReview(IEnumerable<f312_DoExam.ExamResult> results, DataAccessLayer.dt312_PracticeResult participant)
        {
            InitializeComponent();
            Text = "模擬測驗作答詳情 — " + participant.EmployeeCode + " " + participant.EmployeeName;
            EnableResultPrinting();
            browser.DocumentText = BuildHtml(results, participant.Score, true, participant);
        }
        private static string Encode(string text) => WebUtility.HtmlEncode(text ?? "").Replace("\r\n", "\n").Replace("\n", "<br>");
        private static string Image(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return "";
            string path = Path.Combine(TPConfigs.Folder312 ?? "", file);
            return File.Exists(path) ? "<p><img style='max-width:95%;max-height:360px' src='" + ImageHelper.ConvertImageToBase64DataUri(path) + "'></p>" : "";
        }
        public static string BuildHtml(IEnumerable<f312_DoExam.ExamResult> results, int? score, bool practice)
        {
            return BuildHtml(results, score, practice, null);
        }
        public static string BuildHtml(IEnumerable<f312_DoExam.ExamResult> results, int? score, bool practice,
            DataAccessLayer.dt312_PracticeResult participant)
        {
            var rows = results.ToList();
            var html = new StringBuilder("<!doctype html><html lang='zh-Hant'><head><meta charset='utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'>");
            string title = participant == null ? (practice ? "模擬測驗成績與答案" : "考試作答詳情")
                : "模擬測驗結果 — " + participant.EmployeeCode + " " + participant.EmployeeName + " " + participant.SubmitAt.ToString("yyyyMMddHHmmss");
            html.Append("<title>").Append(WebUtility.HtmlEncode(title)).Append("</title><style>")
                .Append("body{font-family:'Microsoft JhengHei UI';margin:0;background:#eef1f5;color:#172033;font-size:15px;line-height:1.55;word-wrap:break-word}")
                .Append(".page{max-width:1120px;margin:0 auto;padding:28px}.header{background:#fff;border:1px solid #d9e0ea;padding:20px;margin-bottom:16px}.q{margin:0;padding:0}")
                .Append("h1{font-size:26px;font-weight:700;margin:0 0 12px}h2{display:inline;font-size:18px;font-weight:700;margin:0}.metric{display:block;border:1px solid #e2e7ef;padding:8px 12px;background:#f7f9fc}.metric b{font-weight:normal}")
                .Append(".note{color:#586579;margin:10px 0 0}.qhead{margin:0}.tag{display:inline-block;font-size:12px;background:#eef3f8;padding:3px 9px;margin-left:6px}.badge{display:block;font-size:14px;font-weight:normal;padding:5px 12px;margin:0 0 10px}")
                .Append(".ok{background:#e5f6ed;color:#14733b}.bad{background:#fdeaea;color:#b3261e}.neutral{background:#e8edf3;color:#52647a}.question{font-size:18px;margin:12px 0}")
                .Append(".answer{border:1px solid #e2e7ef;padding:9px 11px;margin:7px 0}.answer.correct{border-color:#65b783;background:#f0fbf4}.answer.selected{border-color:#6f9bd2;background:#eef6ff}.answer.correct.selected{border-color:#65b783;background:#f0fbf4}.answer.missed{border-color:#e2b85a;background:#fff8df}")
                .Append(".summary{margin:0}.summary-item{display:inline-block;border:1px solid #e2e7ef;padding:8px 12px;background:#f7f9fc;vertical-align:top}.response{border-left:4px solid #8aa4c0;background:#f7f9fc;padding:12px;margin:10px 0}.ref{border-left-color:#65b783}.response p{margin:6px 0 0}img{max-width:100%;height:auto}")
                .Append("@media print{@page{size:A4;margin:15mm}body{background:#fff;font-size:12pt}.page{max-width:none;padding:0}.header,.q{page-break-inside:avoid}.qhead,h1,h2{page-break-after:avoid}.answer,.response,img{page-break-inside:avoid}}</style></head><body><div class='page'>")
                .Append("<div class='header'><h1>").Append(practice ? "模擬測驗結果" : "考試作答詳情").Append("</h1>");
            if (participant != null)
                html.Append("<div class='meta'>")
                    .Append(Metric("測驗類型", participant.PracticeKind == "INTERNAL" ? "模擬測驗內部" : "模擬測驗外部"))
                    .Append(Metric("員工編號", participant.EmployeeCode)).Append(Metric("姓名", participant.EmployeeName))
                    .Append(Metric("部門", participant.Department))
                    .Append(Metric("考試日期", participant.SubmitAt.ToString("yyyy/MM/dd HH:mm:ss"))).Append("</div>");
            html.Append("<div class='metrics'><div class='metric score'><b>成績：</b>")
                .Append(score.HasValue ? score.Value + "／100" : "不計分（全為申論題）").Append("</div></div>");
            if (practice) html.Append("<p class='note'>本次為模擬測驗，不列入正式考試成績。</p>");
            html.Append("</div>");
            foreach (var result in rows.OrderBy(r => r.QuestionIndex))
            {
                bool essay = result.Questions.IsEssay;
                html.Append("<section class='q'><div class='qhead'><h2>第 ").Append(result.QuestionIndex)
                    .Append(" 題<span class='tag'>").Append(essay ? "申論題" : result.Questions.IsmultiAns ? "複選題" : "單選題")
                    .Append("</span></h2></div><div class='badge ").Append(essay ? "neutral" : result.IsCorrect ? "ok" : "bad").Append("'>")
                    .Append(essay ? "不計分" : result.IsCorrect ? "答對" : "答錯")
                    .Append("</div><div class='question'>").Append(Encode(result.Questions.DisplayName))
                    .Append("</div>").Append(Image(result.Questions.ImageName));
                if (!essay)
                {
                    var correctIds = AnswerIds(result.CorrectAnswer);
                    var selectedIds = AnswerIds(result.UserAnswer);
                    foreach (var answer in result.Answers)
                    {
                        bool isCorrect = correctIds.Contains(answer.Id.ToString());
                        bool isSelected = selectedIds.Contains(answer.Id.ToString());
                        string css = isCorrect && isSelected ? "correct selected" : isCorrect ? (selectedIds.Count == 0 ? "correct" : "missed") : isSelected ? "selected" : "";
                        html.Append("<div class='answer ").Append(css).Append("'>").Append(answer.Id).Append(". ")
                            .Append(Encode(answer.DisplayName));
                        html.Append(Image(answer.ImageName)).Append("</div>");
                    }
                }
                html.Append(essay ? "<div class='response ref'><b>參考答案：</b><p>" : "<div class='summary'><span class='summary-item'>正確答案：")
                    .Append(Encode(essay ? (string.IsNullOrWhiteSpace(result.Questions.ReferenceAnswer) ? "未提供參考答案" : result.Questions.ReferenceAnswer) : result.CorrectAnswer))
                    .Append(essay ? "</p></div><div class='response'><b>您的答案：</b><p>" : "</span><span class='summary-item'>您的答案：")
                    .Append(Encode(string.IsNullOrWhiteSpace(result.UserAnswer) ? "未作答" : result.UserAnswer))
                    .Append(essay ? "</p></div></section>" : "</span></div></section>");
            }
            return html.Append("</div></body></html>").ToString();
        }

        private static string Metric(string label, string value) => "<div class='metric'><b>" + Encode(label) + "：</b>" + Encode(value) + "</div>";

        private static HashSet<string> AnswerIds(string value) => new HashSet<string>((value ?? "")
            .Split(',').Select(id => id.Trim()).Where(id => id.Length > 0));
    }
}
