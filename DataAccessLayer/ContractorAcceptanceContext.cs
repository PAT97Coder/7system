using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer
{
    public class ContractorAcceptance
    {
        [Key, MaxLength(100)]
        public string DocumentNumber { get; set; }
        [Required, MaxLength(2), Column(TypeName = "varchar")]
        public string DepartmentGroup { get; set; }
        [Required, MaxLength(7), Column(TypeName = "varchar")]
        public string Period { get; set; }
        [Column(TypeName = "date")]
        public DateTime ReportDate { get; set; }
        [Required, MaxLength(100)]
        public string CostCenter { get; set; }
        [Required, MaxLength(100)]
        public string ContractNumber { get; set; }
        [Required, MaxLength(255)]
        public string Contractor { get; set; }
        public decimal Adjustment { get; set; }
        public decimal BeforeTax { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        [Required, MaxLength(255)]
        public string SourceFileName { get; set; }
        [Required, MaxLength(31)]
        public string SheetName { get; set; }
        [Required, MaxLength(10), Column(TypeName = "varchar")]
        public string CreatedByUserId { get; set; }
        [Required, MaxLength(10), Column(TypeName = "varchar")]
        public string UpdatedByUserId { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime CreatedUtc { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedUtc { get; set; }
        [NotMapped]
        public DateTime CreatedLocal { get { return DateTime.SpecifyKind(CreatedUtc, DateTimeKind.Utc).ToLocalTime(); } }
        [NotMapped]
        public DateTime UpdatedLocal { get { return DateTime.SpecifyKind(UpdatedUtc, DateTimeKind.Utc).ToLocalTime(); } }
        [Timestamp]
        public byte[] Version { get; set; }
        [NotMapped]
        public string SourcePath { get; set; }
        [NotMapped]
        public byte[] SourceContent { get; set; }
        [NotMapped]
        public string Validation { get; set; } = "檢查通過";
        [NotMapped]
        public List<ContractorAcceptanceLine> Lines { get; set; } = new List<ContractorAcceptanceLine>();
    }

    public class ContractorAcceptanceLine
    {
        [Key, Column(Order = 0), MaxLength(100)]
        public string DocumentNumber { get; set; }
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ExcelRow { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(100)]
        public string WorkCode { get; set; }
        [MaxLength(255)]
        public string Location { get; set; }
        [MaxLength(100)]
        public string WorkPeriod { get; set; }
        [MaxLength(100)]
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [NotMapped]
        public decimal Amount { get { return Quantity * UnitPrice; } }
        [MaxLength(10)]
        public string Currency { get; set; }
    }

    public class ContractorAcceptanceFile
    {
        [Key, MaxLength(100)]
        public string DocumentNumber { get; set; }
        [Required]
        public byte[] Content { get; set; }
    }

    // Maps the tables installed by Database/20261123_add_311_contractor_acceptance.sql.
    public sealed class ContractorAcceptanceContext : DbContext
    {
        static ContractorAcceptanceContext()
        {
            Database.SetInitializer<ContractorAcceptanceContext>(null);
        }

        public ContractorAcceptanceContext()
            : base(new EntityConnectionStringBuilder(SingleConnection.ConString).ProviderConnectionString)
        {
        }

        public DbSet<ContractorAcceptance> Acceptances { get; set; }
        public DbSet<ContractorAcceptanceLine> Items { get; set; }
        public DbSet<ContractorAcceptanceFile> Files { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<ContractorAcceptance>().ToTable("dt311_ContractorAcceptance");
            builder.Entity<ContractorAcceptanceLine>().ToTable("dt311_ContractorAcceptanceItem");
            builder.Entity<ContractorAcceptanceFile>().ToTable("dt311_ContractorAcceptanceFile");
            var acceptance = builder.Entity<ContractorAcceptance>();
            acceptance.Property(item => item.Adjustment).HasPrecision(28, 8);
            acceptance.Property(item => item.BeforeTax).HasPrecision(28, 8);
            acceptance.Property(item => item.Tax).HasPrecision(28, 8);
            acceptance.Property(item => item.Total).HasPrecision(28, 8);
            acceptance.Property(item => item.CreatedUtc).HasPrecision(3);
            acceptance.Property(item => item.UpdatedUtc).HasPrecision(3);
            builder.Entity<ContractorAcceptanceLine>().Property(item => item.Quantity).HasPrecision(28, 8);
            builder.Entity<ContractorAcceptanceLine>().Property(item => item.UnitPrice).HasPrecision(28, 8);
        }
    }
}
