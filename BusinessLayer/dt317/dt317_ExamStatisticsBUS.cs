using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLayer
{
    public sealed class Exam317StatisticsRow
    {
        public int? Rank { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DepartmentName { get; set; }
        public decimal? ProfessionalScore { get; set; }
        public decimal? ProfessionalWeighted { get; set; }
        public DateTime? ProfessionalSubmittedAt { get; set; }
        public decimal? ChineseScore { get; set; }
        public decimal? ChineseWeighted { get; set; }
        public DateTime? ChineseSubmittedAt { get; set; }
        public decimal? InterviewScore { get; set; }
        public decimal? InterviewWeighted { get; set; }
        public DateTime? InterviewSubmittedAt { get; set; }
        public string WeightedScoreDetails => string.Format("{0} + {1} + {2}",
            FormatScore(ProfessionalWeighted),
            FormatScore(ChineseWeighted),
            FormatScore(InterviewWeighted));
        public decimal? TotalScore { get; set; }
        public bool IsComplete { get; set; }
        public string CompletionStatus => IsComplete ? "完整" : "缺項";

        private static string FormatScore(decimal? score)
        {
            return score.HasValue ? score.Value.ToString("0.##") : "0";
        }
    }

    public sealed class Exam317ExportData
    {
        public int Year { get; set; }
        public DateTime ExportedAt { get; set; }
        public List<Exam317ExportPerson> People { get; set; } = new List<Exam317ExportPerson>();
        public List<Exam317InterviewExportCandidate> InterviewCandidates { get; set; }
            = new List<Exam317InterviewExportCandidate>();
    }

    public sealed class Exam317ExportPerson
    {
        public int? Rank { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string JobCode { get; set; }
        public string JobName { get; set; }
        public bool IsProfessionalCandidate { get; set; }
        public bool IsChineseCandidate { get; set; }
        public bool IsChineseRetakeFormCandidate { get; set; }
        public bool IsInterviewCandidate { get; set; }
        public decimal? ProfessionalScore { get; set; }
        public decimal? ChineseOfficialScore { get; set; }
        public decimal? ChineseRetakeActualScore { get; set; }
        public decimal? ChineseRecognizedScore { get; set; }
        public bool? ChinesePassed { get; set; }
        public bool? ChineseRetakePassed { get; set; }
        public decimal? InterviewScore { get; set; }
        public decimal TotalScore { get; set; }
    }

    public sealed class Exam317InterviewExportCandidate
    {
        public string UserId { get; set; }
        public List<Exam317InterviewExportScore> Scores { get; set; }
            = new List<Exam317InterviewExportScore>();
    }

    public sealed class Exam317InterviewExportScore
    {
        public string InterviewerName { get; set; }
        public int? ProfessionalSkill { get; set; }
        public string ProfessionalSkillNote { get; set; }
        public int? Responsiveness { get; set; }
        public string ResponsivenessNote { get; set; }
        public int? Communication { get; set; }
        public string CommunicationNote { get; set; }
        public int? ReportQuality { get; set; }
        public string ReportQualityNote { get; set; }
        public decimal? Total { get; set; }
    }

    public sealed class dt317_ExamStatisticsBUS
    {
        private const string ChineseOfficialExam = "正式考試";
        private const string ChineseFirstRetake = "第一次補考";
        private const string ChineseSecondRetake = "第二次補考";

        private sealed class ScoreAttempt
        {
            public string UserId { get; set; }
            public decimal Score { get; set; }
            public DateTime SubmittedAt { get; set; }
            public int Sequence { get; set; }
        }

        private sealed class ProfessionalAssignment
        {
            public string UserId { get; set; }
            public string JobCode { get; set; }
            public decimal? Score { get; set; }
            public DateTime? SubmittedAt { get; set; }
            public DateTime ExamDate { get; set; }
            public int Sequence { get; set; }
        }

        private sealed class ChineseAttempt
        {
            public string UserId { get; set; }
            public string ExamType { get; set; }
            public decimal? Score { get; set; }
            public bool? IsPass { get; set; }
            public int PassingScore { get; set; }
            public DateTime? SubmittedAt { get; set; }
            public DateTime ExamDate { get; set; }
            public int Sequence { get; set; }
        }

        private sealed class InterviewAttempt
        {
            public string UserId { get; set; }
            public string ReportId { get; set; }
            public decimal Score { get; set; }
            public DateTime SubmittedAt { get; set; }
            public DateTime? ReopenedAt { get; set; }
        }

        private sealed class InterviewRound
        {
            public long CandidateProfileId { get; set; }
            public string UserId { get; set; }
            public string ReportId { get; set; }
            public DateTime ReportDate { get; set; }
        }

        private sealed class InterviewAssignmentInfo
        {
            public long Id { get; set; }
            public long CandidateProfileId { get; set; }
            public string InterviewerId { get; set; }
        }

        private sealed class InterviewScoreInfo
        {
            public long AssignmentId { get; set; }
            public int ProfessionalSkill { get; set; }
            public string ProfessionalSkillNote { get; set; }
            public int Responsiveness { get; set; }
            public string ResponsivenessNote { get; set; }
            public int Communication { get; set; }
            public string CommunicationNote { get; set; }
            public int ReportQuality { get; set; }
            public string ReportQualityNote { get; set; }
            public decimal Total { get; set; }
            public DateTime SubmittedAt { get; set; }
            public DateTime? ReopenedAt { get; set; }
        }

        private static dt317_ExamStatisticsBUS instance;
        public static dt317_ExamStatisticsBUS Instance
        {
            get
            {
                if (instance == null) instance = new dt317_ExamStatisticsBUS();
                return instance;
            }
        }

        private dt317_ExamStatisticsBUS() { }

        public List<int> GetAvailableYears()
        {
            using (var context = new DBDocumentManagementSystemEntities())
            {
                var professionalYears = context.dt307_ExamMgmt
                    .Select(item => (item.StartTime ?? item.CreateTime).Year)
                    .Distinct()
                    .ToList();
                var chineseYears = context.dt314_HskExamMgmt
                    .Where(item => item.ExamType == ChineseOfficialExam
                                   || item.ExamType == ChineseFirstRetake
                                   || item.ExamType == ChineseSecondRetake)
                    .Select(item => (item.StartTime ?? item.CreateTime).Year)
                    .Distinct()
                    .ToList();
                var interviewYears = context.dt315_InterviewReport
                    .Select(item => item.CreatedAt.Year)
                    .Distinct()
                    .ToList();

                var years = professionalYears
                    .Concat(chineseYears)
                    .Concat(interviewYears)
                    .Distinct()
                    .ToList();

                if (!years.Contains(DateTime.Now.Year)) years.Add(DateTime.Now.Year);
                return years.OrderByDescending(year => year).ToList();
            }
        }

        public List<Exam317StatisticsRow> GetStatistics(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);

            using (var context = new DBDocumentManagementSystemEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;

                var professionalExamCodes = context.dt307_ExamMgmt
                    .AsNoTracking()
                    .Where(item => (item.StartTime ?? item.CreateTime) >= startDate
                                   && (item.StartTime ?? item.CreateTime) < endDate)
                    .Select(item => item.Code)
                    .ToList();
                var professionalUserIds = context.dt307_ExamUser
                    .AsNoTracking()
                    .Where(item => professionalExamCodes.Contains(item.ExamCode))
                    .Select(item => item.IdUser)
                    .Distinct()
                    .ToList();
                var professionalAttempts = context.dt307_ExamUser
                    .AsNoTracking()
                    .Where(item => professionalExamCodes.Contains(item.ExamCode)
                                   && item.SubmitTime.HasValue
                                   && item.Score.HasValue)
                    .Select(item => new ScoreAttempt
                    {
                        UserId = item.IdUser,
                        Score = item.Score.Value,
                        SubmittedAt = item.SubmitTime.Value,
                        Sequence = item.Id
                    })
                    .ToList();

                var chineseAttempts = LoadChineseAttempts(context, startDate, endDate);
                var chineseUserIds = chineseAttempts
                    .Select(item => item.UserId)
                    .Distinct()
                    .ToList();

                var interviewRounds = LoadInterviewRounds(context, startDate, endDate);
                var interviewReportIds = interviewRounds
                    .Select(item => item.ReportId)
                    .Distinct()
                    .ToList();
                var interviewAttempts = (from score in context.dt315_InterviewScore.AsNoTracking()
                                         join assignment in context.dt315_InterviewAssignment.AsNoTracking()
                                             on score.AssignmentId equals assignment.Id
                                         join candidate in context.dt315_InterviewCandidate.AsNoTracking()
                                             on assignment.CandidateProfileId equals candidate.Id
                                         where assignment.IsActive
                                               && interviewReportIds.Contains(candidate.ReportId)
                                         select new InterviewAttempt
                                         {
                                             UserId = candidate.CandidateId,
                                             ReportId = candidate.ReportId,
                                             Score = score.Total,
                                             SubmittedAt = score.SubmittedAt,
                                             ReopenedAt = score.ReopenedAt
                                         }).ToList();

                var latestProfessional = professionalAttempts
                    .GroupBy(item => item.UserId)
                    .ToDictionary(group => group.Key,
                        group => group.OrderByDescending(item => item.SubmittedAt)
                            .ThenByDescending(item => item.Sequence).First());

                var latestChinese = GetLatestFinalChineseAttempts(chineseAttempts);

                var latestInterview = interviewRounds
                    .GroupBy(item => item.UserId)
                    .ToDictionary(group => group.Key, group =>
                    {
                        var latestReport = group
                            .OrderByDescending(item => item.ReportDate)
                            .ThenByDescending(item => item.ReportId)
                            .First();
                        var validScores = interviewAttempts
                            .Where(item => item.UserId == group.Key
                                           && item.ReportId == latestReport.ReportId)
                            .Where(item => !item.ReopenedAt.HasValue || item.SubmittedAt > item.ReopenedAt.Value)
                            .ToList();
                        return new
                        {
                            Score = validScores.Any()
                                ? (decimal?)Math.Round(validScores.Average(item => item.Score), 2)
                                : null,
                            SubmittedAt = validScores.Any()
                                ? (DateTime?)validScores.Max(item => item.SubmittedAt)
                                : null
                        };
                    });

                var userIds = professionalUserIds
                    .Concat(chineseUserIds)
                    .Concat(interviewRounds.Select(item => item.UserId))
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct()
                    .ToList();

                var users = context.dm_User.AsNoTracking()
                    .Where(user => userIds.Contains(user.Id))
                    .ToList();
                var departmentIds = users.Select(user => user.IdDepartment)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct()
                    .ToList();
                var departments = context.dm_Departments.AsNoTracking()
                    .Where(department => departmentIds.Contains(department.Id))
                    .ToDictionary(department => department.Id, department => department.DisplayName);

                var rows = userIds.Select(userId =>
                {
                    latestProfessional.TryGetValue(userId, out ScoreAttempt professional);
                    latestChinese.TryGetValue(userId, out ChineseAttempt chinese);
                    latestInterview.TryGetValue(userId, out var interview);
                    var user = users.FirstOrDefault(item => item.Id == userId);

                    decimal? submittedProfessionalScore = professional?.Score;
                    decimal? submittedChineseScore = GetRecognizedChineseScore(chinese);
                    decimal? submittedInterviewScore = interview?.Score;
                    bool complete = submittedProfessionalScore.HasValue
                                    && submittedChineseScore.HasValue
                                    && submittedInterviewScore.HasValue;
                    decimal professionalScore = submittedProfessionalScore ?? 0m;
                    decimal chineseScore = submittedChineseScore ?? 0m;
                    decimal interviewScore = submittedInterviewScore ?? 0m;

                    return new Exam317StatisticsRow
                    {
                        UserId = userId,
                        UserName = $"{user?.DisplayName} {user?.DisplayNameVN}".Trim(),
                        DepartmentName = user != null && user.IdDepartment != null
                            && departments.TryGetValue(user.IdDepartment, out string departmentName)
                                ? departmentName
                                : user?.IdDepartment,
                        ProfessionalScore = submittedProfessionalScore,
                        ProfessionalWeighted = Math.Round(professionalScore * 0.20m, 2),
                        ProfessionalSubmittedAt = professional?.SubmittedAt,
                        ChineseScore = submittedChineseScore,
                        ChineseWeighted = Math.Round(chineseScore * 0.30m, 2),
                        ChineseSubmittedAt = chinese?.SubmittedAt,
                        InterviewScore = submittedInterviewScore,
                        InterviewWeighted = Math.Round(interviewScore * 0.50m, 2),
                        InterviewSubmittedAt = interview?.SubmittedAt,
                        TotalScore = Math.Round(professionalScore * 0.20m
                            + chineseScore * 0.30m
                            + interviewScore * 0.50m, 2),
                        IsComplete = complete
                    };
                })
                .OrderByDescending(row => row.TotalScore)
                .ThenBy(row => row.UserId)
                .ToList();

                int rank = 0;
                decimal? previousScore = null;
                for (int index = 0; index < rows.Count; index++)
                {
                    if (!previousScore.HasValue || rows[index].TotalScore != previousScore)
                        rank++;
                    rows[index].Rank = rank;
                    previousScore = rows[index].TotalScore;
                }

                return rows;
            }
        }

        public Exam317ExportData GetExportData(int year)
        {
            List<Exam317StatisticsRow> statistics = GetStatistics(year);
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);

            using (var context = new DBDocumentManagementSystemEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;

                var professionalAssignments = (from examUser in context.dt307_ExamUser.AsNoTracking()
                                               join exam in context.dt307_ExamMgmt.AsNoTracking()
                                                   on examUser.ExamCode equals exam.Code
                                               where (exam.StartTime ?? exam.CreateTime) >= startDate
                                                     && (exam.StartTime ?? exam.CreateTime) < endDate
                                               select new ProfessionalAssignment
                                               {
                                                   UserId = examUser.IdUser,
                                                   JobCode = examUser.IdJob,
                                                   Score = examUser.Score,
                                                   SubmittedAt = examUser.SubmitTime,
                                                   ExamDate = exam.StartTime ?? exam.CreateTime,
                                                   Sequence = examUser.Id
                                               }).ToList();
                var selectedProfessional = professionalAssignments
                    .GroupBy(item => item.UserId)
                    .ToDictionary(group => group.Key, group => SelectProfessionalForForm(group));

                var chineseAttempts = LoadChineseAttempts(context, startDate, endDate);
                var selectedOfficial = chineseAttempts
                    .Where(item => item.ExamType == ChineseOfficialExam)
                    .GroupBy(item => item.UserId)
                    .ToDictionary(group => group.Key, group => SelectChineseForForm(group));
                var selectedRetake = chineseAttempts
                    .Where(IsRetake)
                    .GroupBy(item => item.UserId)
                    .ToDictionary(group => group.Key, group => SelectChineseForForm(group));
                var selectedFinal = GetLatestFinalChineseAttempts(chineseAttempts);

                var interviewRounds = LoadInterviewRounds(context, startDate, endDate);
                var selectedInterviewRounds = interviewRounds
                    .GroupBy(item => item.UserId)
                    .ToDictionary(group => group.Key, group => group
                        .OrderByDescending(item => item.ReportDate)
                        .ThenByDescending(item => item.ReportId)
                        .First());
                var selectedCandidateIds = selectedInterviewRounds.Values
                    .Select(item => item.CandidateProfileId)
                    .Distinct()
                    .ToList();
                var assignments = context.dt315_InterviewAssignment.AsNoTracking()
                    .Where(item => item.IsActive && selectedCandidateIds.Contains(item.CandidateProfileId))
                    .Select(item => new InterviewAssignmentInfo
                    {
                        Id = item.Id,
                        CandidateProfileId = item.CandidateProfileId,
                        InterviewerId = item.InterviewerId
                    })
                    .ToList();
                var assignmentIds = assignments.Select(item => item.Id).ToList();
                var interviewScores = context.dt315_InterviewScore.AsNoTracking()
                    .Where(item => assignmentIds.Contains(item.AssignmentId))
                    .Select(item => new InterviewScoreInfo
                    {
                        AssignmentId = item.AssignmentId,
                        ProfessionalSkill = item.ProfessionalSkill,
                        ProfessionalSkillNote = item.ProfessionalSkillNote,
                        Responsiveness = item.Responsiveness,
                        ResponsivenessNote = item.ResponsivenessNote,
                        Communication = item.Communication,
                        CommunicationNote = item.CommunicationNote,
                        ReportQuality = item.ReportQuality,
                        ReportQualityNote = item.ReportQualityNote,
                        Total = item.Total,
                        SubmittedAt = item.SubmittedAt,
                        ReopenedAt = item.ReopenedAt
                    })
                    .ToList();

                var userIds = statistics.Select(item => item.UserId)
                    .Concat(assignments.Select(item => item.InterviewerId))
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct()
                    .ToList();
                var users = context.dm_User.AsNoTracking()
                    .Where(item => userIds.Contains(item.Id))
                    .ToDictionary(item => item.Id);
                var departmentIds = users.Values
                    .Select(item => item.IdDepartment)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct()
                    .ToList();
                var departments = context.dm_Departments.AsNoTracking()
                    .Where(item => departmentIds.Contains(item.Id))
                    .ToDictionary(item => item.Id, item => item.DisplayName);
                var jobCodes = users.Values
                    .Select(item => item.ActualJobCode ?? item.JobCode)
                    .Concat(selectedProfessional.Values.Select(item => item.JobCode))
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct()
                    .ToList();
                var jobs = context.dm_JobTitle.AsNoTracking()
                    .Where(item => jobCodes.Contains(item.Id))
                    .ToDictionary(item => item.Id, item => item.DisplayName);

                var data = new Exam317ExportData
                {
                    Year = year,
                    ExportedAt = DateTime.Now
                };

                foreach (Exam317StatisticsRow statistic in statistics)
                {
                    users.TryGetValue(statistic.UserId, out dm_User user);
                    selectedProfessional.TryGetValue(statistic.UserId, out ProfessionalAssignment professional);
                    selectedOfficial.TryGetValue(statistic.UserId, out ChineseAttempt official);
                    selectedRetake.TryGetValue(statistic.UserId, out ChineseAttempt retake);
                    selectedFinal.TryGetValue(statistic.UserId, out ChineseAttempt finalChinese);
                    string departmentCode = user?.IdDepartment;
                    string jobCode = professional?.JobCode ?? user?.ActualJobCode ?? user?.JobCode;

                    data.People.Add(new Exam317ExportPerson
                    {
                        Rank = statistic.Rank,
                        UserId = statistic.UserId,
                        UserName = user?.DisplayName ?? user?.DisplayNameVN ?? statistic.UserName,
                        DepartmentCode = departmentCode,
                        DepartmentName = departmentCode != null
                            && departments.TryGetValue(departmentCode, out string departmentName)
                                ? departmentName
                                : departmentCode,
                        JobCode = jobCode,
                        JobName = jobCode != null && jobs.TryGetValue(jobCode, out string jobName)
                            ? jobName
                            : jobCode,
                        IsProfessionalCandidate = selectedProfessional.ContainsKey(statistic.UserId),
                        IsChineseCandidate = selectedOfficial.ContainsKey(statistic.UserId)
                                             || selectedRetake.ContainsKey(statistic.UserId),
                        IsChineseRetakeFormCandidate = selectedOfficial.ContainsKey(statistic.UserId)
                                                       || selectedRetake.ContainsKey(statistic.UserId),
                        IsInterviewCandidate = selectedInterviewRounds.ContainsKey(statistic.UserId),
                        ProfessionalScore = statistic.ProfessionalScore,
                        ChineseOfficialScore = official?.SubmittedAt.HasValue == true ? official.Score : null,
                        ChineseRetakeActualScore = retake?.SubmittedAt.HasValue == true ? retake.Score : null,
                        ChineseRecognizedScore = GetRecognizedChineseScore(finalChinese),
                        ChinesePassed = GetChinesePassed(finalChinese),
                        ChineseRetakePassed = GetChinesePassed(retake),
                        InterviewScore = statistic.InterviewScore,
                        TotalScore = statistic.TotalScore ?? 0m
                    });
                }

                foreach (InterviewRound candidate in selectedInterviewRounds.Values
                    .OrderBy(item => item.UserId))
                {
                    var exportCandidate = new Exam317InterviewExportCandidate
                    {
                        UserId = candidate.UserId
                    };
                    foreach (InterviewAssignmentInfo assignment in assignments
                        .Where(item => item.CandidateProfileId == candidate.CandidateProfileId)
                        .OrderBy(item => item.InterviewerId))
                    {
                        users.TryGetValue(assignment.InterviewerId, out dm_User interviewer);
                        InterviewScoreInfo score = interviewScores
                            .Where(item => item.AssignmentId == assignment.Id)
                            .Where(item => !item.ReopenedAt.HasValue
                                           || item.SubmittedAt > item.ReopenedAt.Value)
                            .OrderByDescending(item => item.SubmittedAt)
                            .FirstOrDefault();
                        exportCandidate.Scores.Add(new Exam317InterviewExportScore
                        {
                            InterviewerName = interviewer?.DisplayName
                                              ?? interviewer?.DisplayNameVN
                                              ?? assignment.InterviewerId,
                            ProfessionalSkill = score?.ProfessionalSkill,
                            ProfessionalSkillNote = score?.ProfessionalSkillNote,
                            Responsiveness = score?.Responsiveness,
                            ResponsivenessNote = score?.ResponsivenessNote,
                            Communication = score?.Communication,
                            CommunicationNote = score?.CommunicationNote,
                            ReportQuality = score?.ReportQuality,
                            ReportQualityNote = score?.ReportQualityNote,
                            Total = score?.Total
                        });
                    }
                    data.InterviewCandidates.Add(exportCandidate);
                }

                return data;
            }
        }

        private static List<ChineseAttempt> LoadChineseAttempts(
            DBDocumentManagementSystemEntities context, DateTime startDate, DateTime endDate)
        {
            return (from examUser in context.dt314_HskExamUser.AsNoTracking()
                    join exam in context.dt314_HskExamMgmt.AsNoTracking()
                        on examUser.ExamCode equals exam.Code
                    where (exam.StartTime ?? exam.CreateTime) >= startDate
                          && (exam.StartTime ?? exam.CreateTime) < endDate
                          && (exam.ExamType == ChineseOfficialExam
                              || exam.ExamType == ChineseFirstRetake
                              || exam.ExamType == ChineseSecondRetake)
                    select new ChineseAttempt
                    {
                        UserId = examUser.IdUser,
                        ExamType = exam.ExamType,
                        Score = examUser.Score,
                        IsPass = examUser.IsPass,
                        PassingScore = exam.PassingScore,
                        SubmittedAt = examUser.SubmitTime,
                        ExamDate = exam.StartTime ?? exam.CreateTime,
                        Sequence = examUser.Id
                    }).ToList();
        }

        private static List<InterviewRound> LoadInterviewRounds(
            DBDocumentManagementSystemEntities context, DateTime startDate, DateTime endDate)
        {
            return (from candidate in context.dt315_InterviewCandidate.AsNoTracking()
                    join report in context.dt315_InterviewReport.AsNoTracking()
                        on candidate.ReportId equals report.Id
                    where report.CreatedAt >= startDate && report.CreatedAt < endDate
                    select new InterviewRound
                    {
                        CandidateProfileId = candidate.Id,
                        UserId = candidate.CandidateId,
                        ReportId = candidate.ReportId,
                        ReportDate = report.CreatedAt
                    }).ToList();
        }

        private static Dictionary<string, ChineseAttempt> GetLatestFinalChineseAttempts(
            IEnumerable<ChineseAttempt> attempts)
        {
            var submitted = attempts
                .Where(item => item.SubmittedAt.HasValue && item.Score.HasValue)
                .ToList();
            var official = submitted
                .Where(item => item.ExamType == ChineseOfficialExam)
                .GroupBy(item => item.UserId)
                .ToDictionary(group => group.Key, group => SelectLatestSubmitted(group));
            var retakes = submitted
                .Where(IsRetake)
                .GroupBy(item => item.UserId)
                .ToDictionary(group => group.Key, group => SelectLatestSubmitted(group));

            return official.Keys.Concat(retakes.Keys)
                .Distinct()
                .ToDictionary(userId => userId,
                    userId => retakes.TryGetValue(userId, out ChineseAttempt retake)
                        ? retake
                        : official[userId]);
        }

        private static ChineseAttempt SelectLatestSubmitted(IEnumerable<ChineseAttempt> attempts)
        {
            return attempts
                .OrderByDescending(item => item.SubmittedAt)
                .ThenByDescending(item => item.Sequence)
                .First();
        }

        private static ChineseAttempt SelectChineseForForm(IEnumerable<ChineseAttempt> attempts)
        {
            return attempts
                .OrderByDescending(item => item.SubmittedAt.HasValue)
                .ThenByDescending(item => item.SubmittedAt)
                .ThenByDescending(item => item.ExamDate)
                .ThenByDescending(item => item.Sequence)
                .First();
        }

        private static ProfessionalAssignment SelectProfessionalForForm(
            IEnumerable<ProfessionalAssignment> attempts)
        {
            return attempts
                .OrderByDescending(item => item.SubmittedAt.HasValue && item.Score.HasValue)
                .ThenByDescending(item => item.SubmittedAt)
                .ThenByDescending(item => item.ExamDate)
                .ThenByDescending(item => item.Sequence)
                .First();
        }

        private static bool IsRetake(ChineseAttempt attempt)
        {
            return attempt.ExamType == ChineseFirstRetake
                   || attempt.ExamType == ChineseSecondRetake;
        }

        private static bool? GetChinesePassed(ChineseAttempt attempt)
        {
            if (attempt == null || !attempt.Score.HasValue || !attempt.SubmittedAt.HasValue)
                return null;
            return attempt.IsPass ?? attempt.Score.Value >= attempt.PassingScore;
        }

        private static decimal? GetRecognizedChineseScore(ChineseAttempt attempt)
        {
            if (attempt == null || !attempt.Score.HasValue || !attempt.SubmittedAt.HasValue)
                return null;
            if (IsRetake(attempt) && GetChinesePassed(attempt) == true)
                return 78m;
            return attempt.Score.Value;
        }
    }
}
