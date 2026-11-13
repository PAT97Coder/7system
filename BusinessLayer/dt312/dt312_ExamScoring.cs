using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    public static class dt312_ExamScoring
    {
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
