using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    public static class dt312_ExamScoring
    {
        public static bool IsGradingComplete(string examData)
        {
            if (string.IsNullOrWhiteSpace(examData)) return false;
            var rows = Newtonsoft.Json.Linq.JArray.Parse(examData);
            return rows.Count > 0 && rows.All(r => r["Questions"] is Newtonsoft.Json.Linq.JObject
                && ((bool?)r["Questions"]["IsEssay"] != true
                    || (r["EssayScore"] != null && r["EssayScore"].Type != Newtonsoft.Json.Linq.JTokenType.Null)));
        }

        public static string PreparePendingGradingData(string examData)
        {
            var rows = Newtonsoft.Json.Linq.JArray.Parse(examData);
            var essays = rows.Where(r => (bool?)r["Questions"]?["IsEssay"] == true).ToList();
            if (essays.Count == 0 || essays.Any(r => r["EssayScore"] != null && r["EssayScore"].Type != Newtonsoft.Json.Linq.JTokenType.Null))
                return examData;
            int withMaximum = rows.Count(r => r["MaxScore"] != null && r["MaxScore"].Type != Newtonsoft.Json.Linq.JTokenType.Null);
            if (withMaximum == rows.Count) return examData;
            if (withMaximum != 0) throw new InvalidOperationException("試卷配分資料不完整。");
            var maxima = AllocateEqualPoints(rows.Select(r => (bool?)r["Questions"]?["IsEssay"] == true).ToArray());
            for (int i = 0; i < rows.Count; i++) rows[i]["MaxScore"] = maxima[i];
            return rows.ToString(Newtonsoft.Json.Formatting.None);
        }

        public static decimal[] AllocateEqualPoints(bool[] essays)
        {
            if (essays == null || essays.Length == 0)
                throw new ArgumentException("題目數量必須大於0。");
            var result = new decimal[essays.Length];
            // Distribute hundredths in a stable order so the preview matches the generated paper.
            var indices = Enumerable.Range(0, essays.Length).OrderBy(i => essays[i]).ThenBy(i => i).ToArray();
            for (int i = 0; i < indices.Length; i++)
                result[indices[i]] = (10000 / indices.Length + (i < 10000 % indices.Length ? 1 : 0)) / 100m;
            return result;
        }

        public static int WeightedScore(IEnumerable<decimal> points)
        {
            decimal sum = points.Sum();
            if (sum < 0 || sum > 100) throw new ArgumentException("總分必須介於0至100。");
            return (int)Math.Round(sum, MidpointRounding.AwayFromZero);
        }
        // Each question has equal weight. Essay marks are entered on a 0–100 scale.
        public static int CombinedScore(IEnumerable<decimal> marks)
        {
            var values = marks.ToList();
            if (values.Count == 0 || values.Any(v => v < 0 || v > 100))
                throw new ArgumentException("題目評分必須介於0至100之間。");
            return (int)Math.Round(values.Average(), MidpointRounding.AwayFromZero);
        }
        public static bool IsCorrect(string correctAnswer, string userAnswer)
        {
            var correct = Tokens(correctAnswer);
            var selected = Tokens(userAnswer);
            return correct.Count > 0 && selected.Count > 0 && correct.SetEquals(selected);
        }
        private static HashSet<string> Tokens(string value) => new HashSet<string>((value ?? "").Split(',').Select(t => t.Trim()).Where(t => t.Length > 0));
        public static int? Score(int correct, int graded)
        {
            if (graded == 0) return null;
            if (graded < 0 || correct < 0 || correct > graded) throw new ArgumentOutOfRangeException(nameof(correct));
            return (int)Math.Round(correct * 100.0 / graded);
        }
    }
}
