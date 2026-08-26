using DataAccessLayer;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    internal sealed class Hsk314ReadingUnit
    {
        public string UnitKey { get; set; }
        public int? GroupId { get; set; }
        public int? QuestionId { get; set; }
        public string LevelCode { get; set; }
        public string PartCode { get; set; }
        public int QuestionCount { get; set; }
        public int UsageCount { get; set; }
        public DateTime? LastUsedDate { get; set; }
    }

    internal sealed class Hsk314ReadingQuestionDto
    {
        public int QuestionId { get; set; }
        public int? GroupId { get; set; }
        public string LevelCode { get; set; }
        public string SectionCode { get; set; }
        public string PartCode { get; set; }
        public string QuestionType { get; set; }
        public string DisplayText { get; set; }
        public string ImageName { get; set; }
        public bool IsMultiAns { get; set; }
        public int? SourceQuestionNo { get; set; }
        public string GroupType { get; set; }
        public string InstructionText { get; set; }
        public string SharedPassage { get; set; }
        public string SharedOptionPool { get; set; }
    }

    internal sealed class Hsk314ReadingQuestionDisplay
    {
        public string PartCode { get; set; }
        public string GroupType { get; set; }
        public string InstructionText { get; set; }
        public string SharedPassage { get; set; }
        public string SharedOptionPool { get; set; }
    }

    internal static class Hsk314ReadingBankRepository
    {
        private sealed class GroupImportDefinition
        {
            public string GroupKey { get; set; }
            public string LevelCode { get; set; }
            public string SectionCode { get; set; }
            public string PartCode { get; set; }
            public string GroupType { get; set; }
            public string GroupTitle { get; set; }
            public string InstructionText { get; set; }
            public string SharedPassage { get; set; }
            public string SharedOptionPool { get; set; }
            public string Remark { get; set; }
            public int QuestionCount { get; set; }
        }

        private static string ProviderConnectionString
        {
            get
            {
                EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(SingleConnection.ConString);
                return builder.ProviderConnectionString;
            }
        }

        public static bool HasGroupedColumns(DataTable table)
        {
            return table != null
                && table.Columns.Contains("GroupNo")
                && table.Columns.Contains("PartCode")
                && table.Columns.Contains("GroupType");
        }

        public static List<Hsk314ReadingUnit> GetActiveReadingUnits(string levelCode)
        {
            const string sql = @"
SELECT
    'G' + CAST(g.Id AS VARCHAR(20)) AS UnitKey,
    g.Id AS GroupId,
    CAST(NULL AS INT) AS QuestionId,
    g.LevelCode,
    ISNULL(g.PartCode, 'ReadingPart3') AS PartCode,
    g.QuestionCount,
    ISNULL(MAX(ISNULL(q.UsageCount, 0)), 0) AS UsageCount,
    MAX(q.LastUsedDate) AS LastUsedDate
FROM dbo.dt314_HskQuestionGroup g
INNER JOIN dbo.dt314_HskQuestions q ON q.GroupId = g.Id
WHERE g.IsActive = 1
  AND g.SectionCode = 'Reading'
  AND g.LevelCode = @LevelCode
  AND g.RandomAsUnit = 1
  AND g.GroupType <> 'SingleQuestion'
  AND q.IsActive = 1
GROUP BY
    g.Id,
    g.LevelCode,
    g.PartCode,
    g.QuestionCount

UNION ALL

SELECT
    'Q' + CAST(q.Id AS VARCHAR(20)) AS UnitKey,
    CAST(NULL AS INT) AS GroupId,
    q.Id AS QuestionId,
    q.LevelCode,
    ISNULL(q.PartCode, 'ReadingPart3') AS PartCode,
    1 AS QuestionCount,
    ISNULL(q.UsageCount, 0) AS UsageCount,
    q.LastUsedDate
FROM dbo.dt314_HskQuestions q
WHERE q.IsActive = 1
  AND q.SectionCode = 'Reading'
  AND q.LevelCode = @LevelCode
  AND
  (
      q.GroupId IS NULL
      OR NOT EXISTS
      (
          SELECT 1
          FROM dbo.dt314_HskQuestionGroup g
          WHERE g.Id = q.GroupId
            AND g.IsActive = 1
      )
      OR EXISTS
      (
          SELECT 1
          FROM dbo.dt314_HskQuestionGroup g
          WHERE g.Id = q.GroupId
            AND g.IsActive = 1
            AND (g.RandomAsUnit = 0 OR g.GroupType = 'SingleQuestion')
      )
  );";

            List<Hsk314ReadingUnit> result = new List<Hsk314ReadingUnit>();
            using (SqlConnection conn = new SqlConnection(ProviderConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LevelCode", levelCode);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Hsk314ReadingUnit()
                        {
                            UnitKey = reader["UnitKey"]?.ToString(),
                            GroupId = reader["GroupId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["GroupId"]),
                            QuestionId = reader["QuestionId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["QuestionId"]),
                            LevelCode = reader["LevelCode"]?.ToString(),
                            PartCode = reader["PartCode"]?.ToString(),
                            QuestionCount = Convert.ToInt32(reader["QuestionCount"]),
                            UsageCount = Convert.ToInt32(reader["UsageCount"]),
                            LastUsedDate = reader["LastUsedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastUsedDate"])
                        });
                    }
                }
            }

            return result;
        }

        public static List<Hsk314ReadingQuestionDto> GetReadingQuestionsByUnits(IEnumerable<Hsk314ReadingUnit> units)
        {
            List<int> groupIds = units.Where(r => r.GroupId.HasValue).Select(r => r.GroupId.Value).Distinct().ToList();
            List<int> questionIds = units.Where(r => r.QuestionId.HasValue).Select(r => r.QuestionId.Value).Distinct().ToList();
            if (groupIds.Count == 0 && questionIds.Count == 0) return new List<Hsk314ReadingQuestionDto>();

            string groupSql = groupIds.Count == 0 ? "SELECT CAST(NULL AS INT) WHERE 1 = 0" : string.Join(",", groupIds);
            string questionSql = questionIds.Count == 0 ? "SELECT CAST(NULL AS INT) WHERE 1 = 0" : string.Join(",", questionIds);
            string sql = $@"
SELECT
    q.Id AS QuestionId,
    q.GroupId,
    q.LevelCode,
    q.SectionCode,
    q.PartCode,
    q.QuestionType,
    q.DisplayText,
    q.ImageName,
    q.IsMultiAns,
    q.SourceQuestionNo,
    g.GroupType,
    g.InstructionText,
    g.SharedPassage,
    g.SharedOptionPool
FROM dbo.dt314_HskQuestions q
LEFT JOIN dbo.dt314_HskQuestionGroup g ON g.Id = q.GroupId
WHERE q.IsActive = 1
  AND
  (
      q.GroupId IN ({groupSql})
      OR q.Id IN ({questionSql})
  );";

            List<Hsk314ReadingQuestionDto> result = new List<Hsk314ReadingQuestionDto>();
            using (SqlConnection conn = new SqlConnection(ProviderConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Hsk314ReadingQuestionDto()
                        {
                            QuestionId = Convert.ToInt32(reader["QuestionId"]),
                            GroupId = reader["GroupId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["GroupId"]),
                            LevelCode = reader["LevelCode"]?.ToString(),
                            SectionCode = reader["SectionCode"]?.ToString(),
                            PartCode = reader["PartCode"]?.ToString(),
                            QuestionType = reader["QuestionType"]?.ToString(),
                            DisplayText = reader["DisplayText"]?.ToString(),
                            ImageName = reader["ImageName"]?.ToString(),
                            IsMultiAns = Convert.ToBoolean(reader["IsMultiAns"]),
                            SourceQuestionNo = reader["SourceQuestionNo"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SourceQuestionNo"]),
                            GroupType = reader["GroupType"]?.ToString(),
                            InstructionText = reader["InstructionText"]?.ToString(),
                            SharedPassage = reader["SharedPassage"]?.ToString(),
                            SharedOptionPool = reader["SharedOptionPool"]?.ToString()
                        });
                    }
                }
            }

            return result;
        }

        public static Dictionary<int, Hsk314ReadingQuestionDisplay> GetQuestionDisplayMap(IEnumerable<int> questionIds)
        {
            List<int> ids = questionIds.Distinct().ToList();
            if (ids.Count == 0) return new Dictionary<int, Hsk314ReadingQuestionDisplay>();

            string sql = $@"
SELECT
    q.Id AS QuestionId,
    q.PartCode,
    g.GroupType,
    g.InstructionText,
    g.SharedPassage,
    g.SharedOptionPool
FROM dbo.dt314_HskQuestions q
LEFT JOIN dbo.dt314_HskQuestionGroup g ON g.Id = q.GroupId
WHERE q.Id IN ({string.Join(",", ids)});";

            Dictionary<int, Hsk314ReadingQuestionDisplay> result = new Dictionary<int, Hsk314ReadingQuestionDisplay>();
            using (SqlConnection conn = new SqlConnection(ProviderConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int questionId = Convert.ToInt32(reader["QuestionId"]);
                        result[questionId] = new Hsk314ReadingQuestionDisplay()
                        {
                            PartCode = reader["PartCode"]?.ToString(),
                            GroupType = reader["GroupType"]?.ToString(),
                            InstructionText = reader["InstructionText"]?.ToString(),
                            SharedPassage = reader["SharedPassage"]?.ToString(),
                            SharedOptionPool = reader["SharedOptionPool"]?.ToString()
                        };
                    }
                }
            }

            return result;
        }

        public static void MarkQuestionsUsed(IEnumerable<int> questionIds)
        {
            List<int> ids = questionIds.Distinct().ToList();
            if (ids.Count == 0) return;

            string sql = $@"
UPDATE dbo.dt314_HskQuestions
SET UsageCount = ISNULL(UsageCount, 0) + 1,
    LastUsedDate = GETDATE()
WHERE Id IN ({string.Join(",", ids)});";

            using (SqlConnection conn = new SqlConnection(ProviderConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static string ValidateGroupedImport(DataTable table)
        {
            string[] required =
            {
                "GroupNo",
                "QuesNo",
                "Level",
                "Section",
                "PartCode",
                "GroupType",
                "QuestionType",
                "Question",
                "Answer",
                "TrueAns"
            };

            foreach (string col in required)
            {
                if (!table.Columns.Contains(col)) return $"Missing column: {col}";
            }

            var questionGroups = table.Rows.Cast<DataRow>().GroupBy(GetQuestionKey);
            foreach (var questionGroup in questionGroups)
            {
                DataRow first = questionGroup.First();
                string questionNo = Read(first, "QuesNo");
                string questionLabel = GetQuestionLabel(first);
                string level = Read(first, "Level");
                string section = Read(first, "Section");
                string partCode = Read(first, "PartCode");
                string groupType = Read(first, "GroupType");
                string questionType = Read(first, "QuestionType");
                string question = Read(first, "Question");

                if (string.IsNullOrWhiteSpace(questionNo)) return "QuesNo is empty.";
                if (!Hsk314Constants.Levels.Contains(level)) return $"Invalid Level: {level}";
                if (!Hsk314Constants.Sections.Contains(section)) return $"Invalid Section: {section}";
                if (!Hsk314Constants.ReadingPartCodes.Contains(partCode)) return $"Invalid PartCode: {partCode}";
                if (!Hsk314Constants.ReadingGroupTypes.Contains(groupType)) return $"Invalid GroupType: {groupType}";
                if (!Hsk314Constants.QuestionTypes.Contains(questionType)) return $"Invalid QuestionType: {questionType}";
                if (string.IsNullOrWhiteSpace(question)) return $"Question {questionLabel} is empty.";
                if (!questionGroup.Any(r => Read(r, "TrueAns") == "1")) return $"Question {questionLabel} has no correct answer.";
            }

            return "";
        }

        public static void ImportGroupedQuestions(DataTable table, string imageFolder, string createdBy)
        {
            Dictionary<string, GroupImportDefinition> definitions = table.Rows.Cast<DataRow>()
                .GroupBy(r => GetGroupKey(r))
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        DataRow first = g.First();
                        return new GroupImportDefinition()
                        {
                            GroupKey = g.Key,
                            LevelCode = Read(first, "Level"),
                            SectionCode = Read(first, "Section"),
                            PartCode = Read(first, "PartCode"),
                            GroupType = Read(first, "GroupType"),
                            GroupTitle = Clean(Read(first, "GroupTitle")),
                            InstructionText = Clean(Read(first, "InstructionText")),
                            SharedPassage = Clean(Read(first, "SharedPassage")),
                            SharedOptionPool = Clean(Read(first, "SharedOptionPool")),
                            Remark = Read(first, "Remark"),
                            QuestionCount = g.Select(r => Read(r, "QuesNo")).Distinct().Count()
                        };
                    });

            using (SqlConnection conn = new SqlConnection(ProviderConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        Dictionary<string, int> groupIds = new Dictionary<string, int>();
                        foreach (GroupImportDefinition def in definitions.Values)
                        {
                            const string insertGroupSql = @"
INSERT INTO dbo.dt314_HskQuestionGroup
(
    LevelCode,
    SectionCode,
    PartCode,
    GroupType,
    GroupCode,
    Title,
    InstructionText,
    SharedPassage,
    SharedOptionPool,
    SourceQuestionFrom,
    SourceQuestionTo,
    QuestionCount,
    RandomAsUnit,
    RandomWeight,
    IsActive,
    Remark,
    CreatedBy,
    CreatedDate
)
VALUES
(
    @LevelCode,
    @SectionCode,
    @PartCode,
    @GroupType,
    @GroupCode,
    @Title,
    @InstructionText,
    @SharedPassage,
    @SharedOptionPool,
    NULL,
    NULL,
    @QuestionCount,
    @RandomAsUnit,
    1,
    1,
    @Remark,
    @CreatedBy,
    GETDATE()
);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd = new SqlCommand(insertGroupSql, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@LevelCode", def.LevelCode);
                                cmd.Parameters.AddWithValue("@SectionCode", def.SectionCode);
                                cmd.Parameters.AddWithValue("@PartCode", def.PartCode);
                                cmd.Parameters.AddWithValue("@GroupType", def.GroupType);
                                cmd.Parameters.AddWithValue("@GroupCode", def.GroupKey);
                                cmd.Parameters.AddWithValue("@Title", ValueOrDbNull(def.GroupTitle));
                                cmd.Parameters.AddWithValue("@InstructionText", ValueOrDbNull(def.InstructionText));
                                cmd.Parameters.AddWithValue("@SharedPassage", ValueOrDbNull(def.SharedPassage));
                                cmd.Parameters.AddWithValue("@SharedOptionPool", ValueOrDbNull(def.SharedOptionPool));
                                cmd.Parameters.AddWithValue("@QuestionCount", def.QuestionCount);
                                cmd.Parameters.AddWithValue("@RandomAsUnit", def.GroupType == Hsk314Constants.GroupSingleQuestion ? 0 : 1);
                                cmd.Parameters.AddWithValue("@Remark", ValueOrDbNull(def.Remark));
                                cmd.Parameters.AddWithValue("@CreatedBy", ValueOrDbNull(createdBy));
                                groupIds[def.GroupKey] = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }

                        foreach (var questionGroup in table.Rows.Cast<DataRow>().GroupBy(GetQuestionKey))
                        {
                            DataRow first = questionGroup.First();
                            string groupKey = GetGroupKey(first);
                            string questionNo = Read(first, "QuesNo");
                            int groupId = groupIds[groupKey];
                            string questionImage = CopyImage(Read(first, "QuestionImage"), imageFolder);
                            int sourceQuestionNo = ReadInt(first, "SourceQuestionNo") ?? ReadInt(first, "QuestionOrder") ?? ParseTrailingNumber(questionNo) ?? 0;

                            const string insertQuestionSql = @"
INSERT INTO dbo.dt314_HskQuestions
(
    LevelCode,
    SectionCode,
    QuestionType,
    DisplayText,
    ImageName,
    IsMultiAns,
    IsActive,
    CreatedBy,
    CreatedDate,
    Remark,
    GroupId,
    PartCode,
    SourceQuestionNo,
    QuestionCode,
    UsageCount,
    LastUsedDate
)
VALUES
(
    @LevelCode,
    @SectionCode,
    @QuestionType,
    @DisplayText,
    @ImageName,
    @IsMultiAns,
    1,
    @CreatedBy,
    GETDATE(),
    @Remark,
    @GroupId,
    @PartCode,
    @SourceQuestionNo,
    @QuestionCode,
    0,
    NULL
);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int questionId;
                            using (SqlCommand cmd = new SqlCommand(insertQuestionSql, conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@LevelCode", Read(first, "Level"));
                                cmd.Parameters.AddWithValue("@SectionCode", Read(first, "Section"));
                                cmd.Parameters.AddWithValue("@QuestionType", Read(first, "QuestionType"));
                                cmd.Parameters.AddWithValue("@DisplayText", Clean(Read(first, "Question")));
                                cmd.Parameters.AddWithValue("@ImageName", ValueOrDbNull(questionImage));
                                cmd.Parameters.AddWithValue("@IsMultiAns", questionGroup.Count(r => Read(r, "TrueAns") == "1") > 1 || Read(first, "QuestionType") == "MultiChoice");
                                cmd.Parameters.AddWithValue("@CreatedBy", ValueOrDbNull(createdBy));
                                cmd.Parameters.AddWithValue("@Remark", ValueOrDbNull(Read(first, "Remark")));
                                cmd.Parameters.AddWithValue("@GroupId", groupId);
                                cmd.Parameters.AddWithValue("@PartCode", Read(first, "PartCode"));
                                cmd.Parameters.AddWithValue("@SourceQuestionNo", sourceQuestionNo);
                                cmd.Parameters.AddWithValue("@QuestionCode", questionNo);
                                questionId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            int displayOrder = 1;
                            foreach (DataRow answerRow in questionGroup)
                            {
                                const string insertAnswerSql = @"
INSERT INTO dbo.dt314_HskAnswers
(
    QuesId,
    DisplayText,
    ImageName,
    TrueAns,
    DisplayOrder,
    IsActive
)
VALUES
(
    @QuesId,
    @DisplayText,
    @ImageName,
    @TrueAns,
    @DisplayOrder,
    1
);";

                                using (SqlCommand cmd = new SqlCommand(insertAnswerSql, conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@QuesId", questionId);
                                    cmd.Parameters.AddWithValue("@DisplayText", Clean(Read(answerRow, "Answer")));
                                    cmd.Parameters.AddWithValue("@ImageName", ValueOrDbNull(CopyImage(Read(answerRow, "AnswerImage"), imageFolder)));
                                    cmd.Parameters.AddWithValue("@TrueAns", Read(answerRow, "TrueAns") == "1");
                                    cmd.Parameters.AddWithValue("@DisplayOrder", ReadInt(answerRow, "AnswerOrder") ?? displayOrder);
                                    cmd.ExecuteNonQuery();
                                }

                                displayOrder++;
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        private static string GetGroupKey(DataRow row)
        {
            string groupNo = Read(row, "GroupNo");
            return string.IsNullOrWhiteSpace(groupNo) ? $"AUTO_{Read(row, "QuesNo")}" : groupNo;
        }

        private static string GetQuestionKey(DataRow row)
        {
            return $"{GetGroupKey(row)}|{Read(row, "QuesNo")}";
        }

        private static string GetQuestionLabel(DataRow row)
        {
            return $"{GetGroupKey(row)} / {Read(row, "QuesNo")}";
        }

        private static object ValueOrDbNull(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
        }

        private static string CopyImage(string imageName, string imageFolder)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return "";

            string source = Path.Combine(imageFolder, imageName);
            if (!File.Exists(source)) return "";

            string destFolder = TPConfigs.Folder307;
            if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

            string encrypted = $"{EncryptionHelper.EncryptionFileName(Path.GetFileNameWithoutExtension(imageName))}{Path.GetExtension(imageName)}";
            File.Copy(source, Path.Combine(destFolder, encrypted), true);
            return encrypted;
        }

        private static string Read(DataRow row, string col)
        {
            return row.Table.Columns.Contains(col) ? row[col]?.ToString().Trim() ?? "" : "";
        }

        private static int? ReadInt(DataRow row, string col)
        {
            int value;
            return int.TryParse(Read(row, col), out value) ? value : (int?)null;
        }

        private static string Clean(string input)
        {
            return Regex.Replace(input ?? "", @"[\t\n\r\s]+", m => m.Value.Contains("\n") ? "\r\n" : " ").Trim();
        }

        private static int? ParseTrailingNumber(string text)
        {
            Match match = Regex.Match(text ?? "", @"(\d+)$");
            if (!match.Success) return null;
            int value;
            return int.TryParse(match.Value, out value) ? value : (int?)null;
        }
    }
}
