using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;

namespace BusinessLayer
{
    public sealed class dt312_PracticeBUS
    {
        public static dt312_PracticeBUS Instance { get; } = new dt312_PracticeBUS();
        private dt312_PracticeBUS() { }

        public static bool IsInternalDepartment(string departmentId) =>
            !string.IsNullOrWhiteSpace(departmentId) && departmentId.Trim().StartsWith("7", StringComparison.Ordinal);

        public static string RequiredText(string value, int maxLength, string caption)
        {
            value = (value ?? "").Trim();
            if (value.Length == 0 || value.Length > maxLength)
                throw new ArgumentException(caption + "為必填，最多 " + maxLength + " 字。");
            return value;
        }

        private static string CheckPermission(string userId)
        {
            userId = (userId ?? "").Trim().ToUpperInvariant();
            if (!System.Text.RegularExpressions.Regex.IsMatch(userId, @"^VNW[0-9]{7}$"))
                throw new UnauthorizedAccessException("員工帳號格式不正確。");
            if (!dt312_QuestionsBUS.Instance.CanPractice(userId))
                throw new UnauthorizedAccessException("您沒有模擬測驗的權限。");
            return userId;
        }

        public void EnsureAvailable(string userId)
        {
            CheckPermission(userId);
            using (var context = new DBDocumentManagementSystemEntities())
            {
                context.dt312_PracticeAttempt.AsNoTracking().Select(a => a.Id).Take(1).ToList();
                context.dt312_ExternalParticipant.AsNoTracking().Select(a => a.Id).Take(1).ToList();
            }
        }

        public int StartInternal(string userId)
        {
            userId = CheckPermission(userId);
            return InTransaction(context =>
            {
                var user = context.dm_User.SingleOrDefault(u => u.Id == userId);
                if (user == null || !IsInternalDepartment(user.IdDepartment))
                    throw new dt312_PracticeValidationException("內部測驗僅限部門編號以 7 開頭的目前登入員工。");
                var name = !string.IsNullOrWhiteSpace(user.DisplayName) ? user.DisplayName
                    : !string.IsNullOrWhiteSpace(user.DisplayNameVN) ? user.DisplayNameVN : user.Id;
                int id = InsertAttempt(context, "INTERNAL", user.Id, null, user.Id,
                    RequiredText(name, 128, "姓名"), user.IdDepartment, userId);
                return id;
            });
        }

        public int StartExternal(string userId, string employeeCode, string employeeName, string department)
        {
            employeeCode = RequiredText(employeeCode, 64, "員工編號").ToUpperInvariant();
            employeeName = RequiredText(employeeName, 128, "姓名");
            department = RequiredText(department, 128, "部門");
            userId = CheckPermission(userId);
            return InTransaction(context =>
            {
                // Serializable EF transaction + unique code isolate concurrent starts.
                var participant = context.dt312_ExternalParticipant.SingleOrDefault(p => p.EmployeeCode == employeeCode);
                if (participant == null)
                {
                    participant = new dt312_ExternalParticipant { EmployeeCode = employeeCode };
                    context.dt312_ExternalParticipant.Add(participant);
                }
                participant.EmployeeName = employeeName;
                participant.Department = department;
                participant.UpdatedAt = DateTime.Now;
                context.SaveChanges();
                int id = InsertAttempt(context, "EXTERNAL", null, participant.Id, employeeCode, employeeName, department, userId);
                return id;
            });
        }

        private static int InsertAttempt(DBDocumentManagementSystemEntities context, string kind,
            string internalId, int? externalId, string code, string name, string department, string startedBy)
        {
            var attempt = new dt312_PracticeAttempt
            {
                PracticeKind = kind,
                InternalUserId = internalId,
                ExternalParticipantId = externalId,
                EmployeeCode = code,
                EmployeeName = name,
                Department = department,
                StartedBy = startedBy,
                StartedAt = DateTime.Now
            };
            context.dt312_PracticeAttempt.Add(attempt);
            context.SaveChanges();
            return attempt.Id;
        }

        private static T InTransaction<T>(Func<DBDocumentManagementSystemEntities, T> action)
        {
            for (int retry = 0; ; retry++)
            {
                try
                {
                    using (var context = new DBDocumentManagementSystemEntities())
                    using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        var result = action(context);
                        transaction.Commit();
                        return result;
                    }
                }
                catch (Exception ex) when (retry < 2 && IsRetryable(ex)) { }
            }
        }

        private static bool IsRetryable(Exception error)
        {
            for (var current = error; current != null; current = current.InnerException)
                if (current is SqlException sql && (sql.Number == 1205 || sql.Number == 2601 || sql.Number == 2627)) return true;
            return false;
        }

        public void Submit(string userId, int attemptId, int? score, string examData)
        {
            userId = CheckPermission(userId);
            if (score.HasValue && (score < 0 || score > 100)) throw new ArgumentOutOfRangeException(nameof(score));
            if (string.IsNullOrWhiteSpace(examData)) throw new ArgumentException("缺少作答資料。");
            InTransaction(context =>
            {
                var attempt = context.dt312_PracticeAttempt.SingleOrDefault(a => a.Id == attemptId && a.StartedBy == userId);
                if (attempt == null) throw new dt312_PracticeValidationException("找不到目前帳號的測驗。");
                if (attempt.SubmitAt.HasValue)
                {
                    if (attempt.Score != score || !string.Equals(attempt.ExamData, examData, StringComparison.Ordinal))
                        throw new dt312_PracticeValidationException("測驗已提交，不可覆寫作答結果。");
                    return true;
                }
                attempt.Score = score;
                attempt.ExamData = examData;
                var now = DateTime.Now;
                attempt.SubmitAt = now < attempt.StartedAt ? attempt.StartedAt : now;
                context.SaveChanges();
                return true;
            });
        }

        public List<dt312_PracticeResult> GetResults(string userId, string kind = null)
        {
            userId = CheckPermission(userId);
            if (kind != null && kind != "INTERNAL" && kind != "EXTERNAL") throw new ArgumentException("無效的測驗類型。");
            using (var context = new DBDocumentManagementSystemEntities())
                return context.dt312_PracticeAttempt.AsNoTracking()
                    .Where(a => (kind == null || a.PracticeKind == kind) && a.SubmitAt.HasValue)
                    .OrderByDescending(a => a.SubmitAt).ThenBy(a => a.Id)
                    .Select(a => new dt312_PracticeResult
                    {
                        Id = a.Id, PracticeKind = a.PracticeKind, EmployeeCode = a.EmployeeCode,
                        EmployeeName = a.EmployeeName, Department = a.Department, SubmitAt = a.SubmitAt.Value, Score = a.Score
                    }).ToList();
        }

        public dt312_PracticeDetail GetResultDetail(string userId, int attemptId, string kind)
        {
            userId = CheckPermission(userId);
            if (kind != "INTERNAL" && kind != "EXTERNAL") throw new ArgumentException("無效的測驗類型。");
            using (var context = new DBDocumentManagementSystemEntities())
                return context.dt312_PracticeAttempt.AsNoTracking()
                    .Where(a => a.Id == attemptId && a.PracticeKind == kind && a.SubmitAt.HasValue)
                    .Select(a => new dt312_PracticeDetail
                    {
                        Id = a.Id, PracticeKind = a.PracticeKind, EmployeeCode = a.EmployeeCode,
                        EmployeeName = a.EmployeeName, Department = a.Department, SubmitAt = a.SubmitAt.Value,
                        Score = a.Score, ExamData = a.ExamData
                    }).SingleOrDefault();
        }
    }

    public sealed class dt312_PracticeValidationException : InvalidOperationException
    {
        public dt312_PracticeValidationException(string message) : base(message) { }
    }
}
