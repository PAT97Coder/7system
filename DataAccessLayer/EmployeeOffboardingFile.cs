using System;
namespace DataAccessLayer
{
    public partial class EmployeeOffboardingFile
    {
        public int Id { get; set; }
        public int OffboardingId { get; set; }
        public string OriginalName { get; set; }
        public string StoredPath { get; set; }
        public long FileSize { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
