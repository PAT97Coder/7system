using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt312_ExamUserBUS
    {
        TPLogger logger;

        private static dt312_ExamUserBUS instance;

        public static dt312_ExamUserBUS Instance
        {
            get { if (instance == null) instance = new dt312_ExamUserBUS(); return instance; }
            private set { instance = value; }
        }

        private dt312_ExamUserBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public bool CanGrade(string userId)
        {
            userId = (userId ?? "").Trim().ToUpperInvariant();
            if (userId.Length == 0) return false;
            using (var db = new DBDocumentManagementSystemEntities())
                return (from role in db.dm_UserRole
                        join grant in db.dm_FunctionRole on role.IdRole equals grant.IdRole
                        join function in db.dm_Function on grant.IdFunction equals function.Id
                        where role.IdUser == userId && (function.ControlName == "f312_ExamReview_Grading"
                            || function.ControlName.EndsWith(".f312_ExamReview_Grading"))
                        select function.Id).Any();
        }

        public void FinishGrading(string userId, int attemptId, string originalData, IDictionary<int, decimal> essayMarks)
        {
            if (!CanGrade(userId)) throw new UnauthorizedAccessException("您沒有評分權限。");
            using (var db = new DBDocumentManagementSystemEntities())
            using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                var attempt = db.dt312_ExamUser.SingleOrDefault(a => a.Id == attemptId);
                if (attempt == null || !attempt.SubmitAt.HasValue || string.IsNullOrEmpty(attempt.ExamData))
                    throw new InvalidOperationException("找不到已提交的試卷。");
                if (attempt.ExamData != originalData) throw new InvalidOperationException("試卷已更新，請重新開啟。");
                var rows = Newtonsoft.Json.Linq.JArray.Parse(dt312_ExamScoring.PreparePendingGradingData(attempt.ExamData));
                var essays = rows.Where(r => (bool?)r["Questions"]?["IsEssay"] == true).ToList();
                if (essays.Count == 0 || essays.Any(r => r["EssayScore"] != null && r["EssayScore"].Type != Newtonsoft.Json.Linq.JTokenType.Null))
                    throw new InvalidOperationException("試卷沒有待評分的申論題，或已完成評分。");
                if (essayMarks == null || essayMarks.Count != essays.Count || essays.Any(r => !essayMarks.ContainsKey((int)r["QuestionIndex"])))
                    throw new ArgumentException("請完成所有申論題評分。");
                var marks = new List<decimal>();
                bool weighted = rows.All(r => r["MaxScore"] != null && r["MaxScore"].Type != Newtonsoft.Json.Linq.JTokenType.Null);
                if (weighted && (rows.Any(r => (decimal)r["MaxScore"] < 0) || rows.Sum(r => (decimal)r["MaxScore"]) != 100m))
                    throw new InvalidOperationException("試卷配分資料不正確。");
                foreach (var row in rows)
                {
                    if ((bool?)row["Questions"]?["IsEssay"] == true)
                    {
                        decimal mark = essayMarks[(int)row["QuestionIndex"]];
                        decimal maximum = weighted ? (decimal)row["MaxScore"] : 100m;
                        if (mark < 0 || mark > maximum || decimal.Round(mark, 2) != mark)
                            throw new ArgumentException("評分不可超過該題滿分，最多兩位小數。");
                        row["EssayScore"] = mark;
                        row["GradedBy"] = userId;
                        row["GradedAt"] = DateTime.Now;
                        marks.Add(mark);
                    }
                    else
                    {
                        bool correct = dt312_ExamScoring.IsCorrect((string)row["CorrectAnswer"], (string)row["UserAnswer"]);
                        row["IsCorrect"] = correct;
                        marks.Add(correct ? (weighted ? (decimal)row["MaxScore"] : 100m) : 0m);
                    }
                }
                var setting = db.dt312_Setting.Single(s => s.id == 1);
                attempt.Score = weighted ? dt312_ExamScoring.WeightedScore(marks) : dt312_ExamScoring.CombinedScore(marks);
                attempt.IsPass = attempt.Score.Value >= ((int?)rows.First["SnapshotPassingScore"] ?? setting.PassingScore);
                attempt.ExamData = rows.ToString(Newtonsoft.Json.Formatting.None);
                db.SaveChanges(); transaction.Commit();
            }
        }

        public List<dt312_ExamUser> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_ExamUser.AsEnumerable().Select(x => new dt312_ExamUser
                    {
                        Id = x.Id,
                        ExamId = x.ExamId,
                        UserId = x.UserId,
                        SubmitAt = x.SubmitAt,
                        Score = x.Score,
                        IsPass = x.IsPass
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt312_ExamUser> GetListNeedDoByUserId(string userId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_ExamUser.Where(r => r.UserId == userId && string.IsNullOrEmpty(r.SubmitAt.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt312_ExamUser> GetListByUserId(string userId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_ExamUser.Where(r => r.UserId == userId).AsEnumerable().Select(x => new dt312_ExamUser
                    {
                        Id = x.Id,
                        ExamId = x.ExamId,
                        UserId = x.UserId,
                        SubmitAt = x.SubmitAt,
                        Score = x.Score,
                        IsPass = x.IsPass
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt312_ExamUser GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt312_ExamUser.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt312_ExamUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_ExamUser.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddRange(List<dt312_ExamUser> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_ExamUser.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool AddOrUpdate(dt312_ExamUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt312_ExamUser.AddOrUpdate(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt312_ExamUser.FirstOrDefault(r => r.Id == id);
                    _context.dt312_ExamUser.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
