using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace BusinessLayer
{
    public sealed class Interview315CandidateInput
    {
        public long? ProfileId { get; set; }
        public string CandidateId { get; set; }
        public bool UsesDefaultInterviewers { get; set; }
        public List<string> InterviewerIds { get; set; } = new List<string>();
        public string SourcePdfPath { get; set; }
    }

    public sealed class Interview315SaveRequest
    {
        public string ReportId { get; set; }
        public string DisplayName { get; set; }
        public string ActorId { get; set; }
        public string StorageRoot { get; set; }
        public List<string> DefaultInterviewerIds { get; set; } = new List<string>();
        public List<Interview315CandidateInput> Candidates { get; set; } = new List<Interview315CandidateInput>();
    }

    public sealed class Interview315CandidateDetail
    {
        public long ProfileId { get; set; }
        public string CandidateId { get; set; }
        public bool UsesDefaultInterviewers { get; set; }
        public string OriginalFileName { get; set; }
        public string RelativePath { get; set; }
        public long? FileSize { get; set; }
        public List<string> InterviewerIds { get; set; } = new List<string>();
        public int SubmittedCount { get; set; }
        public List<Interview315AssignmentDetail> Assignments { get; set; } = new List<Interview315AssignmentDetail>();
    }

    public sealed class Interview315AssignmentDetail
    {
        public long AssignmentId { get; set; }
        public string InterviewerId { get; set; }
        public long? ScoreId { get; set; }
        public decimal? Total { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReopenedAt { get; set; }
        public bool IsSubmitted => ScoreId.HasValue && (!ReopenedAt.HasValue || SubmittedAt > ReopenedAt);
    }

    public sealed class Interview315ReportDetail
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> DefaultInterviewerIds { get; set; } = new List<string>();
        public List<Interview315CandidateDetail> Candidates { get; set; } = new List<Interview315CandidateDetail>();
    }

    public sealed class Interview315ReportRow
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CandidateCount { get; set; }
        public int SubmittedCount { get; set; }
        public int AssignmentCount { get; set; }
        public string Progress => $"{SubmittedCount}/{AssignmentCount}";
        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case "Draft": return "草稿";
                    case "Open": return "評核中";
                    case "Completed": return "已完成";
                    case "Closed": return "已關閉";
                    case "Archived": return "已封存";
                    default: return Status;
                }
            }
        }
    }

    public class dt315_InterviewAssessmentBUS
    {
        private readonly TPLogger logger;
        private static dt315_InterviewAssessmentBUS instance;

        public static dt315_InterviewAssessmentBUS Instance
        {
            get { return instance ?? (instance = new dt315_InterviewAssessmentBUS()); }
        }

        private dt315_InterviewAssessmentBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public List<Interview315ReportRow> GetReportRows()
        {
            using (var context = new DBDocumentManagementSystemEntities())
            {
                return context.dt315_InterviewReport
                    .AsNoTracking()
                    .Select(report => new Interview315ReportRow
                    {
                        Id = report.Id,
                        DisplayName = report.DisplayName,
                        StartAt = report.StartAt,
                        EndAt = report.EndAt,
                        Status = report.Status,
                        CreatedAt = report.CreatedAt,
                        CandidateCount = report.dt315_InterviewCandidate.Count(),
                        AssignmentCount = report.dt315_InterviewCandidate
                            .SelectMany(candidate => candidate.dt315_InterviewAssignment)
                            .Count(assignment => assignment.IsActive),
                        SubmittedCount = report.dt315_InterviewCandidate
                            .SelectMany(candidate => candidate.dt315_InterviewAssignment)
                            .Where(assignment => assignment.IsActive)
                            .SelectMany(assignment => assignment.dt315_InterviewScore)
                            .Count(score => score.ReopenedAt == null || score.SubmittedAt > score.ReopenedAt)
                    })
                    .OrderByDescending(report => report.CreatedAt)
                    .ToList();
            }
        }

        public Interview315ReportDetail GetReportDetail(string reportId)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            {
                var report = context.dt315_InterviewReport
                    .AsNoTracking()
                    .FirstOrDefault(item => item.Id == reportId);
                if (report == null) return null;

                var result = new Interview315ReportDetail
                {
                    Id = report.Id,
                    DisplayName = report.DisplayName,
                    StartAt = report.StartAt,
                    EndAt = report.EndAt,
                    Status = report.Status,
                    CreatedBy = report.CreatedBy,
                    CreatedAt = report.CreatedAt,
                    DefaultInterviewerIds = context.dt315_InterviewDefaultInterviewer
                        .AsNoTracking()
                        .Where(item => item.ReportId == reportId)
                        .Select(item => item.InterviewerId)
                        .ToList()
                };

                var candidates = context.dt315_InterviewCandidate
                    .AsNoTracking()
                    .Where(item => item.ReportId == reportId)
                    .ToList();
                var candidateIds = candidates.Select(item => item.Id).ToList();
                var assignments = context.dt315_InterviewAssignment
                    .AsNoTracking()
                    .Where(item => candidateIds.Contains(item.CandidateProfileId) && item.IsActive)
                    .ToList();
                var assignmentIds = assignments.Select(item => item.Id).ToList();
                var scores = context.dt315_InterviewScore
                    .AsNoTracking()
                    .Where(item => assignmentIds.Contains(item.AssignmentId))
                    .ToList();

                result.Candidates = candidates.Select(candidate => new Interview315CandidateDetail
                {
                    ProfileId = candidate.Id,
                    CandidateId = candidate.CandidateId,
                    UsesDefaultInterviewers = candidate.UsesDefaultInterviewers,
                    OriginalFileName = candidate.OriginalFileName,
                    RelativePath = candidate.RelativePath,
                    FileSize = candidate.FileSize,
                    InterviewerIds = assignments
                        .Where(item => item.CandidateProfileId == candidate.Id)
                        .Select(item => item.InterviewerId)
                        .ToList(),
                    Assignments = assignments.Where(item => item.CandidateProfileId == candidate.Id)
                        .Select(assignment =>
                        {
                            var score = scores.FirstOrDefault(item => item.AssignmentId == assignment.Id);
                            return new Interview315AssignmentDetail
                            {
                                AssignmentId = assignment.Id,
                                InterviewerId = assignment.InterviewerId,
                                ScoreId = score == null ? (long?)null : score.Id,
                                Total = score == null ? (decimal?)null : score.Total,
                                SubmittedAt = score == null ? (DateTime?)null : score.SubmittedAt,
                                ReopenedAt = score == null ? (DateTime?)null : score.ReopenedAt
                            };
                        }).ToList()
                }).ToList();
                foreach (var candidate in result.Candidates)
                    candidate.SubmittedCount = candidate.Assignments.Count(item => item.IsSubmitted);
                return result;
            }
        }

        public string Save(Interview315SaveRequest request)
        {
            ValidateRequest(request);
            var copiedFiles = new List<string>();
            var oldFiles = new List<string>();

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var now = DateTime.Now;
                    var report = string.IsNullOrWhiteSpace(request.ReportId)
                        ? null
                        : context.dt315_InterviewReport.FirstOrDefault(item => item.Id == request.ReportId);

                    if (report == null)
                    {
                        report = new dt315_InterviewReport
                        {
                            Id = GenerateReportId(context, now),
                            // Kept only for compatibility with the existing database schema.
                            // Availability is controlled manually by Status (Open/Closed).
                            StartAt = now,
                            EndAt = now.AddYears(100),
                            Status = "Draft",
                            CreatedBy = request.ActorId,
                            CreatedAt = now
                        };
                        context.dt315_InterviewReport.Add(report);
                    }
                    else if (report.Status != "Draft")
                    {
                        throw new InvalidOperationException("只有草稿狀態可修改。");
                    }

                    report.DisplayName = request.DisplayName.Trim();
                    report.UpdatedBy = request.ActorId;
                    report.UpdatedAt = now;
                    context.SaveChanges();

                    SyncDefaults(context, report.Id, request.DefaultInterviewerIds, request.ActorId, now);
                    SyncCandidates(context, report.Id, request, now, copiedFiles, oldFiles);
                    context.SaveChanges();
                    transaction.Commit();

                    foreach (var oldFile in oldFiles.Distinct()) TryDeleteFile(oldFile);
                    return report.Id;
                }
            }
            catch (Exception ex)
            {
                foreach (var copiedFile in copiedFiles.Distinct()) TryDeleteFile(copiedFile);
                logger.Error(MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
        }

        public void OpenReport(string reportId, string actorId)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            using (var transaction = context.Database.BeginTransaction())
            {
                var report = context.dt315_InterviewReport.FirstOrDefault(item => item.Id == reportId);
                if (report == null) throw new InvalidOperationException("找不到評核批次。");
                if (report.Status != "Draft" && report.Status != "Closed")
                    throw new InvalidOperationException("只有草稿或已關閉的評核可開放。");
                var candidates = context.dt315_InterviewCandidate.Where(item => item.ReportId == reportId).ToList();
                if (!candidates.Any()) throw new InvalidOperationException("請至少選擇一位受評人員。");
                if (candidates.Any(item => string.IsNullOrWhiteSpace(item.RelativePath)))
                    throw new InvalidOperationException("所有受評人員都必須上傳 PDF。");
                var ids = candidates.Select(item => item.Id).ToList();
                if (context.dt315_InterviewAssignment.Any(item => ids.Contains(item.CandidateProfileId) && item.IsActive) == false
                    || candidates.Any(candidate => !context.dt315_InterviewAssignment.Any(item => item.CandidateProfileId == candidate.Id && item.IsActive)))
                    throw new InvalidOperationException("所有受評人員都必須分配至少一位委員。");

                report.Status = "Open";
                report.OpenedAt = DateTime.Now;
                report.CompletedAt = null;
                report.UpdatedBy = actorId;
                report.UpdatedAt = DateTime.Now;
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void CloseReport(string reportId, string actorId)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            using (var transaction = context.Database.BeginTransaction())
            {
                var report = context.dt315_InterviewReport.FirstOrDefault(item => item.Id == reportId);
                if (report == null) throw new InvalidOperationException("找不到評核批次。");
                if (report.Status != "Open") throw new InvalidOperationException("只有開放中的評核可關閉。");

                var now = DateTime.Now;
                report.Status = "Closed";
                report.CompletedAt = now;
                report.UpdatedBy = actorId;
                report.UpdatedAt = now;
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void DeleteDraft(string reportId, string storageRoot)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            using (var transaction = context.Database.BeginTransaction())
            {
                var report = context.dt315_InterviewReport.FirstOrDefault(item => item.Id == reportId);
                if (report == null) return;
                if (report.Status != "Draft") throw new InvalidOperationException("只有草稿可刪除。");
                var candidates = context.dt315_InterviewCandidate.Where(item => item.ReportId == reportId).ToList();
                var candidateIds = candidates.Select(item => item.Id).ToList();
                var assignments = context.dt315_InterviewAssignment.Where(item => candidateIds.Contains(item.CandidateProfileId)).ToList();
                var assignmentIds = assignments.Select(item => item.Id).ToList();
                if (context.dt315_InterviewScore.Any(item => assignmentIds.Contains(item.AssignmentId)))
                    throw new InvalidOperationException("已有評分，不能刪除。");

                context.dt315_InterviewAssignmentAudit.RemoveRange(
                    context.dt315_InterviewAssignmentAudit.Where(item => assignmentIds.Contains(item.AssignmentId)));
                context.dt315_InterviewAssignment.RemoveRange(assignments);
                context.dt315_InterviewDefaultInterviewer.RemoveRange(
                    context.dt315_InterviewDefaultInterviewer.Where(item => item.ReportId == reportId));
                context.dt315_InterviewCandidate.RemoveRange(candidates);
                context.dt315_InterviewReport.Remove(report);
                context.SaveChanges();
                transaction.Commit();
            }

            var reportFolder = Path.Combine(storageRoot, reportId);
            if (Directory.Exists(reportFolder)) Directory.Delete(reportFolder, true);
        }

        public void ReopenScore(long scoreId, string actorId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason)) throw new InvalidOperationException("請輸入解除鎖定原因。");
            using (var context = new DBDocumentManagementSystemEntities())
            using (var transaction = context.Database.BeginTransaction())
            {
                var score = context.dt315_InterviewScore.FirstOrDefault(item => item.Id == scoreId);
                if (score == null) throw new InvalidOperationException("找不到評分資料。");
                if (score.ReopenedAt.HasValue && score.SubmittedAt <= score.ReopenedAt.Value)
                    throw new InvalidOperationException("此評分已解除鎖定。");
                var now = DateTime.Now;
                context.dt315_InterviewScoreAudit.Add(new dt315_InterviewScoreAudit
                {
                    ScoreId = score.Id,
                    Action = "Reopened",
                    ActorId = actorId,
                    ActionAt = now,
                    Reason = reason.Trim(),
                    ProfessionalSkill = score.ProfessionalSkill,
                    ProfessionalSkillNote = score.ProfessionalSkillNote,
                    Responsiveness = score.Responsiveness,
                    ResponsivenessNote = score.ResponsivenessNote,
                    Communication = score.Communication,
                    CommunicationNote = score.CommunicationNote,
                    ReportQuality = score.ReportQuality,
                    ReportQualityNote = score.ReportQualityNote,
                    Total = score.Total,
                    SubmittedAt = score.SubmittedAt
                });
                score.ReopenedAt = now;
                score.ReopenedBy = actorId;
                score.UpdatedAt = now;
                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static void ValidateRequest(Interview315SaveRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.DisplayName)) throw new InvalidOperationException("請輸入評核名稱。");
            if (string.IsNullOrWhiteSpace(request.ActorId)) throw new InvalidOperationException("無法取得目前使用者。");
            if (string.IsNullOrWhiteSpace(request.StorageRoot)) throw new InvalidOperationException("尚未設定 PDF 儲存路徑。");
            request.DefaultInterviewerIds = request.DefaultInterviewerIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
            request.Candidates = request.Candidates.GroupBy(item => item.CandidateId).Select(group => group.First()).ToList();
        }

        private static string GenerateReportId(DBDocumentManagementSystemEntities context, DateTime now)
        {
            var prefix = now.ToString("yyyyMM");
            var maxId = context.dt315_InterviewReport.Where(item => item.Id.StartsWith(prefix))
                .OrderByDescending(item => item.Id).Select(item => item.Id).FirstOrDefault();
            var next = string.IsNullOrEmpty(maxId) ? 1 : int.Parse(maxId.Substring(6, 2)) + 1;
            if (next > 99) throw new InvalidOperationException("本月評核批次已超過 99 筆。");
            return prefix + next.ToString("D2");
        }

        private static void SyncDefaults(DBDocumentManagementSystemEntities context, string reportId,
            IEnumerable<string> desiredIds, string actorId, DateTime now)
        {
            var desired = new HashSet<string>(desiredIds);
            var existing = context.dt315_InterviewDefaultInterviewer.Where(item => item.ReportId == reportId).ToList();
            context.dt315_InterviewDefaultInterviewer.RemoveRange(existing.Where(item => !desired.Contains(item.InterviewerId)));
            foreach (var id in desired.Where(id => existing.All(item => item.InterviewerId != id)))
            {
                context.dt315_InterviewDefaultInterviewer.Add(new dt315_InterviewDefaultInterviewer
                {
                    ReportId = reportId,
                    InterviewerId = id,
                    AssignedBy = actorId,
                    AssignedAt = now
                });
            }
        }

        private static void SyncCandidates(DBDocumentManagementSystemEntities context, string reportId,
            Interview315SaveRequest request, DateTime now, List<string> copiedFiles, List<string> oldFiles)
        {
            var existing = context.dt315_InterviewCandidate.Where(item => item.ReportId == reportId).ToList();
            var desiredIds = new HashSet<string>(request.Candidates.Select(item => item.CandidateId));
            foreach (var removed in existing.Where(item => !desiredIds.Contains(item.CandidateId)).ToList())
            {
                var assignments = context.dt315_InterviewAssignment.Where(item => item.CandidateProfileId == removed.Id).ToList();
                var assignmentIds = assignments.Select(item => item.Id).ToList();
                if (context.dt315_InterviewScore.Any(item => assignmentIds.Contains(item.AssignmentId)))
                    throw new InvalidOperationException("已有評分的受評人員不能移除。");
                context.dt315_InterviewAssignmentAudit.RemoveRange(
                    context.dt315_InterviewAssignmentAudit.Where(item => assignmentIds.Contains(item.AssignmentId)));
                context.dt315_InterviewAssignment.RemoveRange(assignments);
                context.dt315_InterviewCandidate.Remove(removed);
                if (!string.IsNullOrWhiteSpace(removed.RelativePath))
                    oldFiles.Add(Path.Combine(request.StorageRoot, removed.RelativePath));
            }

            context.SaveChanges();
            foreach (var input in request.Candidates)
            {
                var candidate = existing.FirstOrDefault(item => item.CandidateId == input.CandidateId);
                if (candidate == null)
                {
                    candidate = new dt315_InterviewCandidate
                    {
                        ReportId = reportId,
                        CandidateId = input.CandidateId,
                        CreatedAt = now
                    };
                    context.dt315_InterviewCandidate.Add(candidate);
                }
                candidate.UsesDefaultInterviewers = input.UsesDefaultInterviewers;
                candidate.UpdatedAt = now;

                if (!string.IsNullOrWhiteSpace(input.SourcePdfPath))
                    ApplyPdf(candidate, reportId, input.SourcePdfPath, request, now, copiedFiles, oldFiles);

                context.SaveChanges();
                var desiredInterviewers = input.UsesDefaultInterviewers
                    ? request.DefaultInterviewerIds
                    : input.InterviewerIds;
                SyncAssignments(context, candidate, desiredInterviewers, input.UsesDefaultInterviewers ? "Default" : "Custom", request.ActorId, now);
            }
        }

        private static void SyncAssignments(DBDocumentManagementSystemEntities context, dt315_InterviewCandidate candidate,
            IEnumerable<string> desiredIds, string source, string actorId, DateTime now)
        {
            var desired = new HashSet<string>(desiredIds.Where(id => !string.IsNullOrWhiteSpace(id)));
            var existing = context.dt315_InterviewAssignment.Where(item => item.CandidateProfileId == candidate.Id && item.IsActive).ToList();
            foreach (var assignment in existing.Where(item => !desired.Contains(item.InterviewerId)))
            {
                if (context.dt315_InterviewScore.Any(item => item.AssignmentId == assignment.Id))
                    throw new InvalidOperationException("已有評分的委員分配不能移除，請先解除評分鎖定。");
                assignment.IsActive = false;
                assignment.RemovedBy = actorId;
                assignment.RemovedAt = now;
                AddAssignmentAudit(context, assignment, "Removed", actorId, now);
            }

            foreach (var id in desired.Where(id => existing.All(item => item.InterviewerId != id)))
            {
                var assignment = new dt315_InterviewAssignment
                {
                    CandidateProfileId = candidate.Id,
                    InterviewerId = id,
                    Source = source,
                    IsActive = true,
                    AssignedBy = actorId,
                    AssignedAt = now
                };
                context.dt315_InterviewAssignment.Add(assignment);
                context.SaveChanges();
                AddAssignmentAudit(context, assignment, "Assigned", actorId, now);
            }
        }

        private static void AddAssignmentAudit(DBDocumentManagementSystemEntities context,
            dt315_InterviewAssignment assignment, string action, string actorId, DateTime now)
        {
            context.dt315_InterviewAssignmentAudit.Add(new dt315_InterviewAssignmentAudit
            {
                AssignmentId = assignment.Id,
                Action = action,
                ActorId = actorId,
                ActionAt = now,
                CandidateProfileId = assignment.CandidateProfileId,
                InterviewerId = assignment.InterviewerId,
                Source = assignment.Source
            });
        }

        private static void ApplyPdf(dt315_InterviewCandidate candidate, string reportId, string sourcePath,
            Interview315SaveRequest request, DateTime now, List<string> copiedFiles, List<string> oldFiles)
        {
            ValidatePdf(sourcePath);
            var fileName = Guid.NewGuid().ToString("N") + ".pdf";
            var relativePath = Path.Combine(reportId, candidate.CandidateId, fileName);
            var targetPath = Path.Combine(request.StorageRoot, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            File.Copy(sourcePath, targetPath, false);
            copiedFiles.Add(targetPath);

            if (!string.IsNullOrWhiteSpace(candidate.RelativePath))
                oldFiles.Add(Path.Combine(request.StorageRoot, candidate.RelativePath));
            candidate.OriginalFileName = Path.GetFileName(sourcePath);
            candidate.PhysicalFileName = fileName;
            candidate.RelativePath = relativePath;
            candidate.FileSize = new FileInfo(sourcePath).Length;
            candidate.Sha256 = ComputeSha256(targetPath);
            candidate.UploadedBy = request.ActorId;
            candidate.UploadedAt = now;
        }

        private static void ValidatePdf(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("找不到 PDF。", path);
            if (!string.Equals(Path.GetExtension(path), ".pdf", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("只允許 PDF 檔案。");
            var length = new FileInfo(path).Length;
            if (length <= 0 || length > 20L * 1024 * 1024)
                throw new InvalidOperationException("PDF 大小必須介於 1 byte 與 20 MB 之間。");
            var signature = new byte[5];
            using (var stream = File.OpenRead(path))
            {
                if (stream.Read(signature, 0, signature.Length) != signature.Length
                    || System.Text.Encoding.ASCII.GetString(signature) != "%PDF-")
                    throw new InvalidOperationException("檔案內容不是有效的 PDF。");
            }
        }

        private static string ComputeSha256(string path)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static void TryDeleteFile(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch { }
        }
    }
}
