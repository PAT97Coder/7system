using System;
namespace DataAccessLayer
{
    public partial class EmployeeOffboarding
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
        public DateTime? ResignDate { get; set; }
        public string DepartmentId { get; set; }
        public string JobCode { get; set; }
        public string DisplayNameTW { get; set; }
        public string DisplayNameVN { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
