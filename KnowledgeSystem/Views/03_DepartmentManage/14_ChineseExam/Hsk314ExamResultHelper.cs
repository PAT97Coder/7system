using KnowledgeSystem.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    internal static class Hsk314ExamResultHelper
    {
        internal class ExamAnswer
        {
            public int Id { get; set; }
            public string DisplayText { get; set; }
            public string ImageName { get; set; }
        }

        internal class ExamResult
        {
            public int QuestionIndex { get; set; }
            public int QuestionId { get; set; }
            public string LevelCode { get; set; }
            public string SectionCode { get; set; }
            public string GroupType { get; set; }
            public string InstructionText { get; set; }
            public string SharedPassage { get; set; }
            public string SharedOptionPool { get; set; }
            public string QuestionType { get; set; }
            public string QuestionText { get; set; }
            public string QuestionImage { get; set; }
            public List<ExamAnswer> Answers { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
            public bool IsMultiChoice { get; set; }
        }

        internal static List<ExamResult> ParseExamResults(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<ExamResult>();

            try
            {
                JToken token = JToken.Parse(json);
                if (token.Type == JTokenType.Array) return ParseExamResultArray(token);
                if (token.Type != JTokenType.Object) return new List<ExamResult>();

                JObject obj = (JObject)token;
                foreach (string propertyName in new[] { "ExamResults", "examResults", "Results", "results", "Questions", "questions", "Data", "data", "Items", "items" })
                {
                    JToken value = obj[propertyName];
                    if (value != null && value.Type == JTokenType.Array) return ParseExamResultArray(value);
                }

                JToken nestedArray = obj.Properties()
                    .Select(r => r.Value)
                    .FirstOrDefault(r => r.Type == JTokenType.Array && r.Any(IsExamResultToken));
                return nestedArray == null ? new List<ExamResult>() : ParseExamResultArray(nestedArray);
            }
            catch
            {
                return new List<ExamResult>();
            }
        }

        private static List<ExamResult> ParseExamResultArray(JToken token)
        {
            var result = new List<ExamResult>();
            foreach (JToken itemToken in token.Children())
            {
                ExamResult item = itemToken.ToObject<ExamResult>();
                if (item == null) continue;

                item.Answers = item.Answers ?? new List<ExamAnswer>();
                if (string.IsNullOrWhiteSpace(item.CorrectAnswer))
                    item.CorrectAnswer = JoinIds(itemToken["correctAnswerIds"] ?? itemToken["CorrectAnswerIds"]);
                if (string.IsNullOrWhiteSpace(item.UserAnswer))
                    item.UserAnswer = JoinIds(itemToken["userAnswerIds"] ?? itemToken["UserAnswerIds"]);
                result.Add(item);
            }
            return result;
        }

        private static string JoinIds(JToken token)
        {
            return token == null || token.Type != JTokenType.Array
                ? ""
                : string.Join(",", token.Values<int>());
        }

        private static bool IsExamResultToken(JToken token)
        {
            if (token.Type != JTokenType.Object) return false;
            JObject obj = (JObject)token;
            return obj["QuestionId"] != null || obj["questionId"] != null
                || obj["QuestionIndex"] != null || obj["questionIndex"] != null;
        }

        internal static string BuildResultHtml(string userName, string score, List<ExamResult> results)
        {
            results = results ?? new List<ExamResult>();
            int correct = results.Count(r => r.IsCorrect);
            var sb = new StringBuilder();
            sb.Append("<!doctype html><html><head><meta charset='utf-8'><style>");
            sb.Append("body{font-family:'Microsoft JhengHei UI','Segoe UI',Arial,sans-serif;margin:0;background:#eef1f5;color:#172033;font-size:15px;line-height:1.55}");
            sb.Append(".page{max-width:1120px;margin:0 auto;padding:28px}.header,.q{background:#fff;border:1px solid #d9e0ea;border-radius:8px;padding:20px;margin-bottom:16px}");
            sb.Append(".title{font-size:26px;font-weight:700;margin-bottom:12px}.meta,.summary{display:flex;gap:12px;flex-wrap:wrap}.metric,.summary span{border:1px solid #e2e7ef;border-radius:6px;padding:8px 12px;background:#f7f9fc}");
            sb.Append(".qhead{display:flex;justify-content:space-between;gap:12px}.qtitle{font-size:18px;font-weight:700}.tag{font-size:12px;background:#eef3f8;border-radius:999px;padding:3px 9px;margin-left:6px}");
            sb.Append(".badge{font-weight:700;border-radius:999px;padding:5px 12px}.ok{background:#e5f6ed;color:#14733b}.bad{background:#fdeaea;color:#b3261e}");
            sb.Append(".block{border-left:4px solid #8aa4c0;background:#f7f9fc;padding:12px;margin:10px 0}.pool{border-left-color:#d6a93b;background:#fff8e8}.question{font-size:18px;margin:12px 0}");
            sb.Append(".ans{border:1px solid #e2e7ef;border-radius:6px;padding:9px 11px;margin:7px 0}.ans.correct{border-color:#65b783;background:#f0fbf4}.ans.selected{border-color:#6f9bd2;background:#eef6ff}.ans.missed{border-color:#e2b85a;background:#fff8df}");
            sb.Append(".img{max-width:760px;max-height:520px;display:block;margin:10px 0}@media print{body{background:#fff}.page{max-width:none;padding:0}}</style></head><body><div class='page'>");
            sb.Append("<div class='header'><div class='title'>HSK 考試結果</div><div class='meta'>");
            sb.Append(Metric("人員", userName));
            sb.Append(Metric("得分", score));
            sb.Append(Metric("正確", correct.ToString()));
            sb.Append(Metric("錯誤", (results.Count - correct).ToString()));
            sb.Append("</div></div>");

            foreach (ExamResult item in results.OrderBy(r => r.QuestionIndex))
            {
                HashSet<string> correctIds = ToIdSet(item.CorrectAnswer);
                HashSet<string> userIds = ToIdSet(item.UserAnswer);
                sb.Append("<section class='q'><div class='qhead'>");
                sb.Append($"<div class='qtitle'>第 {item.QuestionIndex} 題 <span class='tag'>{Encode(item.LevelCode)}</span><span class='tag'>{Encode(item.SectionCode)}</span><span class='tag'>{Encode(item.QuestionType)}</span></div>");
                sb.Append(item.IsCorrect ? "<div class='badge ok'>正確</div>" : "<div class='badge bad'>錯誤</div>");
                sb.Append("</div>");
                sb.Append(TextBlock(item.InstructionText, "block"));
                sb.Append(TextBlock(item.SharedPassage, "block"));
                sb.Append(TextBlock(item.SharedOptionPool, "block pool"));
                sb.Append($"<div class='question'>{Multiline(item.QuestionText)}</div>{ImageTag(item.QuestionImage)}");

                foreach (ExamAnswer answer in item.Answers ?? new List<ExamAnswer>())
                {
                    string id = answer.Id.ToString();
                    bool isCorrect = correctIds.Contains(id);
                    bool isSelected = userIds.Contains(id);
                    string css = isCorrect && isSelected ? "ans correct selected" : isCorrect ? "ans correct" : isSelected ? "ans selected" : "ans";
                    if (isCorrect && !isSelected && userIds.Count > 0) css = "ans missed";
                    sb.Append($"<div class='{css}'><b>{answer.Id}.</b> {Multiline(answer.DisplayText)}{ImageTag(answer.ImageName)}</div>");
                }

                sb.Append($"<div class='summary'><span><b>正確答案：</b>{Encode(DisplayAnswer(item.CorrectAnswer))}</span><span><b>作答：</b>{Encode(DisplayAnswer(item.UserAnswer))}</span></div></section>");
            }

            sb.Append("</div></body></html>");
            return sb.ToString();
        }

        private static string Metric(string label, string value)
        {
            return $"<div class='metric'><b>{Encode(label)}：</b>{Encode(value)}</div>";
        }

        private static string TextBlock(string text, string cssClass)
        {
            return string.IsNullOrWhiteSpace(text) ? "" : $"<div class='{cssClass}'>{Multiline(text)}</div>";
        }

        private static string ImageTag(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return "";
            string imagePath = Path.Combine(TPConfigs.Folder307, imageName);
            string dataUri = ImageHelper.ConvertImageToBase64DataUri(imagePath);
            return string.IsNullOrWhiteSpace(dataUri) ? "" : $"<img class='img' src='{dataUri}' />";
        }

        private static string Multiline(string text)
        {
            return Encode(text).Replace("\r\n", "<br/>").Replace("\n", "<br/>");
        }

        private static string Encode(string text)
        {
            return System.Net.WebUtility.HtmlEncode(text ?? "");
        }

        private static string DisplayAnswer(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? "未作答" : text;
        }

        private static HashSet<string> ToIdSet(string text)
        {
            return new HashSet<string>((text ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()));
        }
    }
}
