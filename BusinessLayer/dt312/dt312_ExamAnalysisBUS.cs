using DataAccessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace BusinessLayer
{
    public sealed class Exam312AnalysisRow
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int PeopleCount { get; set; }
        public int PaperCount { get; set; }
        public int QuestionCount { get; set; }
        public int WrongCount { get; set; }
        // Đơn vị 0–100, dùng chung cho grid và chart, không nhân thêm 100 khi hiển thị.
        public decimal ErrorRate => QuestionCount == 0 ? 0 : WrongCount * 100m / QuestionCount;
        public string ChartLabel => GroupName == "未知群組（ID：" + GroupId + "）" ? GroupName : GroupName + "（ID：" + GroupId + "）";
        public string ChartToolTip => GroupName + " — " + ErrorRate.ToString("0.00") + "%（" + WrongCount + "/" + QuestionCount + "）";
    }

    public sealed class Exam312AnalysisResult
    {
        public List<Exam312AnalysisRow> Rows { get; set; } = new List<Exam312AnalysisRow>();
        public int SubmittedPapers { get; set; }
        public int SkippedPapers { get; set; }
        public int SkippedQuestions { get; set; }
    }

    public sealed class Exam312AnalysisExamOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public sealed class Exam312AnalysisPersonOption
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Display => string.IsNullOrEmpty(UserId) ? Name : UserId + " " + Name;
    }

    public sealed class Exam312AnalysisOptions
    {
        public List<Exam312AnalysisExamOption> Exams { get; set; }
        public List<Exam312AnalysisPersonOption> People { get; set; }
    }

    public sealed class dt312_ExamAnalysisBUS
    {
        public static dt312_ExamAnalysisBUS Instance { get; } = new dt312_ExamAnalysisBUS();
        private dt312_ExamAnalysisBUS() { }
        private static string Normalize(string value) => (value ?? "").Trim().ToUpperInvariant();

        public bool CanAnalyze(string actorUserId)
        {
            actorUserId = Normalize(actorUserId);
            if (!System.Text.RegularExpressions.Regex.IsMatch(actorUserId, @"^VNW[0-9]{7}$")) return false;
            using (var db = new DBDocumentManagementSystemEntities())
                return (from role in db.dm_UserRole.AsNoTracking()
                        join grant in db.dm_FunctionRole.AsNoTracking() on role.IdRole equals grant.IdRole
                        join function in db.dm_Function.AsNoTracking() on grant.IdFunction equals function.Id
                        where role.IdUser == actorUserId && (function.ControlName == "uc312_ExamAnalysis"
                            || function.ControlName.EndsWith(".uc312_ExamAnalysis"))
                        select function.Id).Any();
        }

        private void Authorize(string actorUserId)
        {
            if (!CanAnalyze(actorUserId)) throw new UnauthorizedAccessException("您沒有錯題分析的權限，請先登錄功能並由管理員授權。");
        }

        public void EnsureCanAnalyze(string actorUserId) => Authorize(actorUserId);

        private static void ValidateExam(int? examId)
        {
            if (examId.HasValue && examId.Value <= 0) throw new ArgumentException("請選擇有效的考試。");
        }

        public Exam312AnalysisOptions GetOptions(string actorUserId, int? examId)
        {
            Authorize(actorUserId); ValidateExam(examId);
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var exams = db.dt312_ExamMgmt.AsNoTracking().OrderByDescending(e => e.CreateAt).ThenByDescending(e => e.Id)
                    .Select(e => new Exam312AnalysisExamOption { Id = e.Id, Name = e.DisplayName }).ToList();
                var assignments = db.dt312_ExamUser.AsNoTracking().AsQueryable();
                if (examId.HasValue) assignments = assignments.Where(a => a.ExamId == examId.Value);
                var ids = assignments.Select(a => a.UserId).Distinct();
                var people = (from id in ids
                              join user in db.dm_User.AsNoTracking() on id equals user.Id into profiles
                              from user in profiles.DefaultIfEmpty()
                              select new { UserId = id, TW = user.DisplayName, VN = user.DisplayNameVN }).ToList()
                    .GroupBy(p => Normalize(p.UserId)).Where(g => g.Key.Length > 0)
                    .Select(g => new Exam312AnalysisPersonOption { UserId = g.Key,
                        Name = g.Select(p => p.TW).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n))
                            ?? g.Select(p => p.VN).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n)) ?? g.Key })
                    .OrderBy(p => p.UserId).ToList();
                people.Insert(0, new Exam312AnalysisPersonOption { UserId = "", Name = "全部人員" });
                return new Exam312AnalysisOptions { Exams = exams, People = people };
            }
        }

        public Exam312AnalysisResult GetStatistics(string actorUserId, int? examId, string subjectUserId)
        {
            Authorize(actorUserId); ValidateExam(examId);
            subjectUserId = Normalize(subjectUserId);
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var query = db.dt312_ExamUser.AsNoTracking().Where(a => a.SubmitAt.HasValue);
                if (examId.HasValue) query = query.Where(a => a.ExamId == examId.Value);
                if (subjectUserId.Length > 0) query = query.Where(a => a.UserId.Trim().ToUpper() == subjectUserId);
                var papers = query.Select(a => new { a.Id, a.UserId, a.ExamData }).ToList();
                // Không lọc RemoveAt: giữ tên nhóm đã ngừng sử dụng trong lịch sử.
                var names = db.dt312_Groups.AsNoTracking().Select(g => new { g.Id, g.DisplayName })
                    .ToDictionary(g => g.Id, g => g.DisplayName);
                var result = new Exam312AnalysisResult { SubmittedPapers = papers.Count };
                var counts = new Dictionary<int, GroupCounts>();
                foreach (var paper in papers)
                {
                    JArray questions;
                    try
                    {
                        questions = string.IsNullOrWhiteSpace(paper.ExamData) ? null : JToken.Parse(paper.ExamData) as JArray;
                        if (questions == null || questions.Count == 0) throw new JsonException("Empty or invalid result array");
                    }
                    catch (JsonException ex)
                    {
                        result.SkippedPapers++;
                        Trace.TraceWarning("312 analysis: skip paper {0}: {1}", paper.Id, ex.Message);
                        continue;
                    }
                    int skipped = 0;
                    foreach (var token in questions)
                    {
                        int groupId; bool correct, essay;
                        if (!TryRead(token, out groupId, out correct, out essay)) { skipped++; continue; }
                        if (essay) continue;
                        GroupCounts group;
                        if (!counts.TryGetValue(groupId, out group)) counts[groupId] = group = new GroupCounts();
                        group.Questions++; if (!correct) group.Wrong++;
                        group.Papers.Add(paper.Id); group.People.Add(Normalize(paper.UserId));
                    }
                    result.SkippedQuestions += skipped;
                    if (skipped > 0) Trace.TraceWarning("312 analysis: paper {0}, skip {1} invalid questions", paper.Id, skipped);
                }
                result.Rows = counts.Select(g => new Exam312AnalysisRow {
                    GroupId = g.Key, GroupName = names.ContainsKey(g.Key) && !string.IsNullOrWhiteSpace(names[g.Key])
                        ? names[g.Key] : "未知群組（ID：" + g.Key + "）",
                    PeopleCount = g.Value.People.Count, PaperCount = g.Value.Papers.Count,
                    QuestionCount = g.Value.Questions, WrongCount = g.Value.Wrong
                }).OrderByDescending(g => g.ErrorRate).ThenByDescending(g => g.QuestionCount).ThenBy(g => g.GroupId).ToList();
                return result;
            }
        }

        private sealed class GroupCounts
        {
            internal int Questions, Wrong;
            internal readonly HashSet<int> Papers = new HashSet<int>();
            internal readonly HashSet<string> People = new HashSet<string>();
        }

        // DTO đọc snapshot: nullable bool phân biệt false hợp lệ và trường bị thiếu.
        private sealed class Snapshot
        {
            public bool? IsCorrect { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
        }

        private static bool TryRead(JToken token, out int groupId, out bool correct, out bool essay)
        {
            groupId = 0; correct = false; essay = false;
            var item = token as JObject;
            var question = item?["Questions"] as JObject;
            if (question == null) return false;
            var essayToken = question["IsEssay"];
            if (essayToken != null && essayToken.Type != JTokenType.Boolean) return false;
            essay = essayToken != null && essayToken.Value<bool>();
            if (essay) return true; // JSON cũ không có IsEssay mặc định trắc nghiệm.
            var id = question["GroupId"];
            if (id == null || id.Type != JTokenType.Integer || !int.TryParse(id.ToString(), out groupId) || groupId <= 0) return false;
            var isCorrect = item["IsCorrect"];
            if (isCorrect != null && isCorrect.Type != JTokenType.Boolean) return false;
            if (isCorrect != null) { correct = isCorrect.Value<bool>(); return true; }
            // Chỉ tính lại JSON cũ khi có khóa đáp án và trường câu trả lời rõ ràng.
            if (item["CorrectAnswer"]?.Type != JTokenType.String || string.IsNullOrWhiteSpace((string)item["CorrectAnswer"])) return false;
            var answer = item["UserAnswer"];
            if (answer == null || (answer.Type != JTokenType.String && answer.Type != JTokenType.Null)) return false;
            var snapshot = item.ToObject<Snapshot>();
            correct = dt312_ExamScoring.IsCorrect(snapshot.CorrectAnswer, snapshot.UserAnswer);
            return true;
        }
    }
}
