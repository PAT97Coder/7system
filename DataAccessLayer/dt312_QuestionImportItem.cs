using System.Collections.Generic;

namespace DataAccessLayer
{
    // Excel numbers are temporary identifiers, never database question IDs.
    public class dt312_QuestionImportItem
    {
        public int ExcelNumber { get; set; }
        public dt312_Questions Question { get; set; }
        public List<dt312_Answers> Answers { get; set; } = new List<dt312_Answers>();
    }
}
