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
        public f312_ExamReview(DataAccessLayer.dt312_Questions question, List<DataAccessLayer.dt312_Answers> answers)
        {
            InitializeComponent(); Text = "預覽題目";
            string key = string.Join(",", answers.Where(a => a.TrueAns).Select(a => a.Id.ToString()));
            browser.DocumentText = BuildQuestionHtml(question, answers, 1).Replace("</body>", "<p>正確答案：" + Encode(key) + "</p></body>");
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
            browser.DocumentText = BuildHtml(results, score, practice);
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
            var rows = results.ToList(); int graded = rows.Count(r => !r.Questions.IsEssay);
            var html = new StringBuilder("<!doctype html><html lang='zh-Hant'><head><meta charset='utf-8'><meta http-equiv='X-UA-Compatible' content='IE=edge'><style>body{font-family:'Microsoft JhengHei UI';font-size:18px;padding:20px}section{border:1px solid #ccc;margin:16px 0;padding:16px}.correct{color:#176b31}.wrong{color:#b32525}.answer{padding:8px;background:#f4f4f4;margin:4px 0}.ref{white-space:normal}</style></head><body>");
            html.Append("<h1>").Append(practice ? "模擬測驗" : "考試作答詳情").Append("</h1><h2>成績：")
                .Append(score.HasValue ? score.Value + "／100" : "不計分（全為申論題）").Append("</h2><p>選擇題答對 ")
                .Append(rows.Count(r => !r.Questions.IsEssay && r.IsCorrect)).Append("／").Append(graded)
                .Append(" 題；申論題 ").Append(rows.Count - graded).Append(" 題，不計分。</p>");
            if (practice) html.Append("<p>本次為模擬測驗，不列入正式考試成績。</p>");
            foreach (var result in rows)
            {
                bool essay = result.Questions.IsEssay;
                html.Append("<section><h2>第 ").Append(result.QuestionIndex).Append(" 題：")
                    .Append(essay ? "申論題（不計分）" : result.IsCorrect ? "<span class='correct'>答對</span>" : "<span class='wrong'>答錯</span>")
                    .Append("</h2><p>").Append(Encode(result.Questions.DisplayName)).Append("</p>").Append(Image(result.Questions.ImageName));
                if (!essay)
                    foreach (var answer in result.Answers)
                        html.Append("<div class='answer'>").Append(answer.Id).Append("． ").Append(Encode(answer.DisplayName)).Append(Image(answer.ImageName)).Append("</div>");
                html.Append("<p><b>您的答案：</b>").Append(Encode(string.IsNullOrWhiteSpace(result.UserAnswer) ? "未作答" : result.UserAnswer)).Append("</p>");
                html.Append("<p class='ref'><b>").Append(essay ? "參考答案：" : "正確答案：").Append("</b>")
                    .Append(Encode(essay ? (string.IsNullOrWhiteSpace(result.Questions.ReferenceAnswer) ? "未提供參考答案" : result.Questions.ReferenceAnswer) : result.CorrectAnswer))
                    .Append("</p></section>");
            }
            return html.Append("</body></html>").ToString();
        }
    }
}
