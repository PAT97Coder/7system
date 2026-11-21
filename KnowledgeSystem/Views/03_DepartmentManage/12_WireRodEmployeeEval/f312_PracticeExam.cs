namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    // Điểm vào riêng cho menu Thi thu, dùng chung toàn bộ luồng thi thử của màn hình Thi.
    public class f312_PracticeExam : f312_DoExam
    {
        public f312_PracticeExam() : base(true)
        {
            Text = "模擬測驗";
        }
    }
}
