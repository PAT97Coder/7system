using System.Collections.Generic;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    internal static class Hsk314Constants
    {
        public const string LevelHsk4 = "HSK4";
        public const string LevelHsk5 = "HSK5";
        public const string SectionReading = "Reading";
        public const string PartReading1 = "ReadingPart1";
        public const string PartReading2 = "ReadingPart2";
        public const string PartReading3 = "ReadingPart3";
        public const string GroupSingleQuestion = "SingleQuestion";
        public const string GroupSharedPassage = "SharedPassage";
        public const string GroupSharedWordBank = "SharedWordBank";
        public const string GroupSentenceOrder = "SentenceOrder";
        public const string GroupPassageCloze = "PassageCloze";
        public const int MaxExamRetries = 3;

        public static readonly string[] Levels = { LevelHsk4, LevelHsk5 };
        public static readonly string[] Sections = { SectionReading };
        public static readonly string[] QuestionTypes = { "SingleChoice", "SentenceOrder" };
        public static readonly string[] ReadingPartCodes = { PartReading1, PartReading2, PartReading3 };
        public static readonly string[] ReadingGroupTypes = { GroupSingleQuestion, GroupSharedPassage, GroupSharedWordBank, GroupSentenceOrder, GroupPassageCloze };

        public static readonly Dictionary<string, string> SectionNames = new Dictionary<string, string>
        {
            { SectionReading, "閱讀" }
        };
    }
}
