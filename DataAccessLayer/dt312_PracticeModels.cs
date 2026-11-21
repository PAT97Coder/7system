using System;

namespace DataAccessLayer
{
    // View DTOs only. Table entities are generated from ModelDocManager.edmx.
    public class dt312_PracticeResult
    {
        public int Id { get; set; }
        public string PracticeKind { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public DateTime SubmitAt { get; set; }
        public int? Score { get; set; }
    }

    public class dt312_PracticeDetail : dt312_PracticeResult
    {
        public string ExamData { get; set; }
    }
}
