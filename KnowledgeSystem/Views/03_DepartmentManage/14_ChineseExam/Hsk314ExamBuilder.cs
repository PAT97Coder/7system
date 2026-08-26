using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    internal class Hsk314ExamOptions
    {
        public const string DefaultExamType = "模擬考試";
        public const string DefaultRatioText = "9:1";
        public const string ExamTypeMock = "模擬考試";
        public const string ExamTypeOfficial = "正式考試";
        public const string ExamTypeRetakeFirst = "第一次補考";
        public const string ExamTypeRetakeSecond = "第二次補考";

        public string ExamType { get; set; }
        public string RatioText { get; set; }
        public int Hsk4Ratio { get; set; }
        public int Hsk5Ratio { get; set; }

        public static Hsk314ExamOptions Default()
        {
            return new Hsk314ExamOptions()
            {
                ExamType = DefaultExamType,
                RatioText = DefaultRatioText,
                Hsk4Ratio = 9,
                Hsk5Ratio = 1
            };
        }

        public static Hsk314ExamOptions FromRemark(string remark)
        {
            Hsk314ExamOptions options = Default();
            if (string.IsNullOrWhiteSpace(remark)) return options;

            foreach (string part in remark.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] tokens = part.Split(new[] { '=' }, 2);
                if (tokens.Length != 2) continue;

                string key = tokens[0].Trim();
                string value = tokens[1].Trim();
                if (key.Equals("ExamType", StringComparison.OrdinalIgnoreCase))
                {
                    options.ExamType = value;
                }
                else if (key.Equals("HskRatio", StringComparison.OrdinalIgnoreCase))
                {
                    int hsk4;
                    int hsk5;
                    if (TryParseRatio(value, out hsk4, out hsk5))
                    {
                        options.RatioText = value;
                        options.Hsk4Ratio = hsk4;
                        options.Hsk5Ratio = hsk5;
                    }
                }
            }

            return options;
        }

        public static Hsk314ExamOptions FromExam(dt314_HskExamMgmt exam)
        {
            if (exam == null) return Default();

            Hsk314ExamOptions options = FromRemark(exam.Remark);
            if (!string.IsNullOrWhiteSpace(exam.ExamType)) options.ExamType = exam.ExamType.Trim();

            int hsk4;
            int hsk5;
            if (TryParseRatio(exam.HskRatio, out hsk4, out hsk5))
            {
                options.RatioText = exam.HskRatio.Trim();
                options.Hsk4Ratio = hsk4;
                options.Hsk5Ratio = hsk5;
            }

            return options;
        }

        public static string ToRemark(string examType, string ratioText)
        {
            return $"ExamType={examType?.Trim()};HskRatio={ratioText?.Trim()}";
        }

        public static bool TryParseRatio(string ratioText, out int hsk4, out int hsk5)
        {
            hsk4 = 0;
            hsk5 = 0;
            if (string.IsNullOrWhiteSpace(ratioText)) return false;

            string[] parts = ratioText.Trim().Split(':');
            return parts.Length == 2
                && int.TryParse(parts[0].Trim(), out hsk4)
                && int.TryParse(parts[1].Trim(), out hsk5)
                && hsk4 > 0
                && hsk5 > 0;
        }

        public bool TryGetReadingCounts(int totalReadingCount, out int hsk4Count, out int hsk5Count)
        {
            hsk4Count = 0;
            hsk5Count = 0;
            int totalRatio = Hsk4Ratio + Hsk5Ratio;
            if (totalReadingCount <= 0 || totalRatio <= 0) return false;
            if (totalReadingCount * Hsk4Ratio % totalRatio != 0) return false;

            hsk4Count = totalReadingCount * Hsk4Ratio / totalRatio;
            hsk5Count = totalReadingCount - hsk4Count;
            return hsk4Count > 0 && hsk5Count > 0;
        }

        public int GetScorePenalty()
        {
            if (string.Equals(ExamType, ExamTypeRetakeFirst, StringComparison.OrdinalIgnoreCase)) return 10;
            if (string.Equals(ExamType, ExamTypeRetakeSecond, StringComparison.OrdinalIgnoreCase)) return 20;
            return 0;
        }
    }

    internal static class Hsk314ExamBuilder
    {
        public static string ValidateBank(int readingCount, Hsk314ExamOptions options)
        {
            if (options == null) options = Hsk314ExamOptions.Default();

            int hsk4Count;
            int hsk5Count;
            if (!options.TryGetReadingCounts(readingCount, out hsk4Count, out hsk5Count))
            {
                return $"Reading count {readingCount} cannot be divided by HSK4/5 ratio {options.RatioText}.";
            }

            return ValidateReadingSection(new Dictionary<string, int>()
            {
                { Hsk314Constants.LevelHsk4, hsk4Count },
                { Hsk314Constants.LevelHsk5, hsk5Count }
            });
        }

        public static List<dt314_HskExamQuestion> BuildSnapshot(string examCode, int readingCount, Hsk314ExamOptions options)
        {
            List<dt314_HskQuestions> selected = new List<dt314_HskQuestions>();
            selected.AddRange(PickReadingSection(readingCount, options));

            int order = 1;
            return selected.Select(q => new dt314_HskExamQuestion()
            {
                ExamCode = examCode,
                QuestionId = q.Id,
                LevelCode = q.LevelCode,
                SectionCode = q.SectionCode,
                QuestionType = q.QuestionType,
                DisplayOrder = order++,
                CreatedDate = DateTime.Now
            }).ToList();
        }

        private static string ValidateReadingSection(Dictionary<string, int> levelCounts)
        {
            foreach (KeyValuePair<string, int> levelCount in levelCounts)
            {
                string level = levelCount.Key;
                int target = levelCount.Value;
                List<Hsk314ReadingUnit> units = Hsk314ReadingBankRepository.GetActiveReadingUnits(level);
                int available = units.Sum(r => r.QuestionCount);
                if (available < target)
                {
                    return $"{level} reading bank is insufficient: need {target}, available {available}.";
                }

                List<Hsk314ReadingUnit> selectedUnits;
                if (!TryPickUnits(units, target, out selectedUnits))
                {
                    return $"{level} reading bank cannot compose exactly {target} questions. Please check group sizes.";
                }
            }

            return "";
        }

        private static IEnumerable<dt314_HskQuestions> PickReadingSection(int count, Hsk314ExamOptions options)
        {
            if (options == null) options = Hsk314ExamOptions.Default();

            int hsk4Count;
            int hsk5Count;
            if (!options.TryGetReadingCounts(count, out hsk4Count, out hsk5Count))
            {
                throw new InvalidOperationException($"Reading count {count} cannot be divided by HSK4/5 ratio {options.RatioText}.");
            }

            Dictionary<string, int> levelCounts = new Dictionary<string, int>()
            {
                { Hsk314Constants.LevelHsk4, hsk4Count },
                { Hsk314Constants.LevelHsk5, hsk5Count }
            };
            List<dt314_HskQuestions> result = new List<dt314_HskQuestions>();

            foreach (KeyValuePair<string, int> levelCount in levelCounts)
            {
                string level = levelCount.Key;
                int target = levelCount.Value;
                List<Hsk314ReadingUnit> units = Hsk314ReadingBankRepository.GetActiveReadingUnits(level);
                List<Hsk314ReadingUnit> selectedUnits;
                if (!TryPickUnits(units, target, out selectedUnits))
                {
                    throw new InvalidOperationException($"{level} reading bank cannot compose {target} questions.");
                }

                List<Hsk314ReadingQuestionDto> readingQuestions = Hsk314ReadingBankRepository.GetReadingQuestionsByUnits(selectedUnits);
                foreach (Hsk314ReadingUnit unit in selectedUnits)
                {
                    IEnumerable<Hsk314ReadingQuestionDto> items = unit.GroupId.HasValue
                        ? readingQuestions.Where(r => r.GroupId == unit.GroupId)
                        : readingQuestions.Where(r => r.QuestionId == unit.QuestionId);

                    foreach (Hsk314ReadingQuestionDto item in items.OrderBy(r => r.SourceQuestionNo ?? int.MaxValue).ThenBy(r => r.QuestionId))
                    {
                        result.Add(new dt314_HskQuestions()
                        {
                            Id = item.QuestionId,
                            LevelCode = item.LevelCode,
                            SectionCode = item.SectionCode,
                            QuestionType = item.QuestionType,
                            DisplayText = item.DisplayText,
                            ImageName = item.ImageName,
                            IsMultiAns = item.IsMultiAns,
                            IsActive = true
                        });
                    }
                }
            }

            return result;
        }

        private static bool TryPickUnits(List<Hsk314ReadingUnit> units, int target, out List<Hsk314ReadingUnit> selectedUnits)
        {
            selectedUnits = new List<Hsk314ReadingUnit>();
            List<Hsk314ReadingUnit> ordered = units
                .OrderByDescending(r => r.QuestionCount)
                .ThenBy(r => r.UsageCount)
                .ThenBy(r => r.LastUsedDate ?? DateTime.MinValue)
                .ThenBy(_ => Guid.NewGuid())
                .ToList();

            return TryPickUnitsRecursive(ordered, 0, target, selectedUnits);
        }

        private static bool TryPickUnitsRecursive(List<Hsk314ReadingUnit> units, int index, int remaining, List<Hsk314ReadingUnit> selected)
        {
            if (remaining == 0) return true;
            if (remaining < 0 || index >= units.Count) return false;

            int remainingCapacity = 0;
            for (int i = index; i < units.Count; i++) remainingCapacity += units[i].QuestionCount;
            if (remainingCapacity < remaining) return false;

            for (int i = index; i < units.Count; i++)
            {
                Hsk314ReadingUnit unit = units[i];
                if (unit.QuestionCount > remaining) continue;
                selected.Add(unit);
                if (TryPickUnitsRecursive(units, i + 1, remaining - unit.QuestionCount, selected)) return true;
                selected.RemoveAt(selected.Count - 1);
            }

            return false;
        }
    }
}
