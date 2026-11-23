using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLayer
{
    public sealed class ContractorAcceptanceService
    {
        public const int MaximumFileBytes = 10 * 1024 * 1024;
        public const string FunctionName = "uc311_ContractorAcceptance";

        public static string NormalizeNumber(string number)
        {
            return (number ?? "").Normalize(NormalizationForm.FormKC).Trim().ToUpperInvariant();
        }

        public int? GetPermissionId(string userId)
        {
            return GetAccess(userId)?.PermissionId;
        }

        private sealed class AcceptanceAccess
        {
            public int PermissionId { get; set; }
            public string UserId { get; set; }
            public string DepartmentGroup { get; set; }
        }

        private static AcceptanceAccess GetAccess(string userId)
        {
            userId = (userId ?? "").Trim().ToUpperInvariant();
            if (!Regex.IsMatch(userId, @"^VNW[0-9]{7}$")) return null;
            using (var context = new DBDocumentManagementSystemEntities())
            {
                var user = context.dm_User.Where(row => row.Id == userId && row.Status == 0)
                    .Select(row => new { row.IdDepartment }).SingleOrDefault();
                string department = user?.IdDepartment?.Trim() ?? "";
                if (!Regex.IsMatch(department, @"^[0-9]{2}")) return null;
                int? permissionId = (from userRole in context.dm_UserRole
                        join functionRole in context.dm_FunctionRole on userRole.IdRole equals functionRole.IdRole
                        join function in context.dm_Function on functionRole.IdFunction equals function.Id
                        where userRole.IdUser == userId && function.ControlName == FunctionName && function.Status == true
                        select (int?)function.Id).Distinct().SingleOrDefault();
                return permissionId.HasValue ? new AcceptanceAccess
                {
                    PermissionId = permissionId.Value, UserId = userId, DepartmentGroup = department.Substring(0, 2)
                } : null;
            }
        }

        private static AcceptanceAccess CheckPermission(string userId)
        {
            return GetAccess(userId) ?? throw new UnauthorizedAccessException("您沒有使用外包工作驗收功能的權限，或尚未設定有效部門。");
        }

        public List<ContractorAcceptance> GetList(string userId)
        {
            var access = CheckPermission(userId);
            using (var context = new ContractorAcceptanceContext())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var scoped = context.Acceptances.AsNoTracking().Where(item => item.DepartmentGroup == access.DepartmentGroup);
                var items = scoped.OrderByDescending(item => item.ReportDate)
                    .ThenBy(item => item.DocumentNumber).ToList();
                var lines = (from line in context.Items.AsNoTracking()
                             join item in scoped on line.DocumentNumber equals item.DocumentNumber
                             orderby line.ExcelRow
                             select line).ToList()
                    .ToLookup(item => item.DocumentNumber, StringComparer.OrdinalIgnoreCase);
                foreach (var item in items) item.Lines = lines[item.DocumentNumber].ToList();
                transaction.Commit();
                return items;
            }
        }

        public byte[] GetSource(string userId, string number)
        {
            var access = CheckPermission(userId);
            using (var context = new ContractorAcceptanceContext())
            {
                var key = NormalizeNumber(number);
                return (from file in context.Files.AsNoTracking()
                        join item in context.Acceptances on file.DocumentNumber equals item.DocumentNumber
                        where file.DocumentNumber == key && item.DepartmentGroup == access.DepartmentGroup
                        select file.Content).SingleOrDefault();
            }
        }

        public Dictionary<string, byte[]> FindDuplicates(string userId, IList<ContractorAcceptance> items)
        {
            var access = CheckPermission(userId);
            ValidateBatch(items);
            using (var context = new ContractorAcceptanceContext())
            {
                var result = new Dictionary<string, byte[]>(StringComparer.Ordinal);
                foreach (var item in items)
                {
                    var key = NormalizeNumber(item.DocumentNumber);
                    var existing = context.Acceptances.AsNoTracking().Where(row => row.DocumentNumber == key)
                        .Select(row => new { row.DepartmentGroup, row.Version }).SingleOrDefault();
                    if (existing == null) continue;
                    CheckDepartment(existing.DepartmentGroup, access);
                    result.Add(key, existing.Version);
                }
                return result;
            }
        }

        private static void CheckDepartment(string department, AcceptanceAccess access)
        {
            if (department != access.DepartmentGroup)
                throw new UnauthorizedAccessException("本單編號無法在您的部門群組使用。");
        }

        public void Save(string userId, IList<ContractorAcceptance> items, IReadOnlyDictionary<string, byte[]> confirmed)
        {
            var actor = CheckPermission(userId);
            ValidateBatch(items);
            try
            {
                using (var context = new ContractorAcceptanceContext())
                using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var entries = items.OrderBy(item => NormalizeNumber(item.DocumentNumber), StringComparer.Ordinal)
                        .Select(item => new { Incoming = item, Key = NormalizeNumber(item.DocumentNumber) }).ToList();
                    foreach (var entry in entries)
                    {
                        var existing = context.Acceptances.Find(entry.Key);
                        if (existing != null) CheckDepartment(existing.DepartmentGroup, actor);
                        byte[] approvedVersion = null;
                        bool approved = confirmed != null && confirmed.TryGetValue(entry.Key, out approvedVersion);
                        if (existing == null ? approved : !approved || approvedVersion == null || !approvedVersion.SequenceEqual(existing.Version))
                            throw new InvalidOperationException(entry.Key + "：資料已新增或變更，請重新匯入並確認是否覆寫。");
                    }
                    var now = DateTime.UtcNow;
                    foreach (var entry in entries)
                    {
                        var target = context.Acceptances.Find(entry.Key);
                        if (target == null)
                        {
                            target = context.Acceptances.Add(new ContractorAcceptance
                            {
                                DocumentNumber = entry.Key, DepartmentGroup = actor.DepartmentGroup,
                                CreatedByUserId = actor.UserId, CreatedUtc = now
                            });
                        }
                        CopyHeader(entry.Incoming, target, actor.UserId, now);
                        bool replacing = context.Entry(target).State != EntityState.Added;
                        if (replacing) context.Entry(target).Property(row => row.UpdatedUtc).IsModified = true;
                        context.SaveChanges();
                        if (replacing)
                        {
                            context.Items.RemoveRange(context.Items.Where(line => line.DocumentNumber == entry.Key).ToList());
                            context.SaveChanges();
                        }
                        context.Items.AddRange(entry.Incoming.Lines.Select(line => new ContractorAcceptanceLine
                        {
                            DocumentNumber = entry.Key, ExcelRow = line.ExcelRow, Description = line.Description,
                            WorkCode = line.WorkCode, Location = line.Location, WorkPeriod = line.WorkPeriod, Unit = line.Unit,
                            Quantity = line.Quantity, UnitPrice = line.UnitPrice, Currency = line.Currency
                        }));
                        var file = context.Files.Find(entry.Key);
                        if (file == null)
                            context.Files.Add(new ContractorAcceptanceFile { DocumentNumber = entry.Key, Content = entry.Incoming.SourceContent });
                        else
                            file.Content = entry.Incoming.SourceContent;
                        context.SaveChanges();
                    }
                    transaction.Commit();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("資料已被其他使用者更新，請重新匯入。");
            }
        }

        private static void CopyHeader(ContractorAcceptance from, ContractorAcceptance to, string actor, DateTime now)
        {
            to.Period = from.Period;
            to.ReportDate = from.ReportDate;
            to.CostCenter = from.CostCenter;
            to.ContractNumber = from.ContractNumber;
            to.Contractor = from.Contractor;
            to.Adjustment = from.Adjustment;
            to.BeforeTax = from.BeforeTax;
            to.Tax = from.Tax;
            to.Total = from.Total;
            to.SourceFileName = from.SourceFileName;
            to.SheetName = from.SheetName;
            to.UpdatedByUserId = actor;
            to.UpdatedUtc = now;
        }

        private static void ValidateBatch(IList<ContractorAcceptance> items)
        {
            if (items == null || items.Count == 0) throw new InvalidDataException("沒有可匯入的資料。");
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var item in items)
            {
                var key = NormalizeNumber(item.DocumentNumber);
                if (key.Length == 0 || key.Length > 100)
                    throw new InvalidDataException("本單編號不可空白或超過 100 字元。");
                if (!keys.Add(key)) throw new InvalidDataException(key + "：本批次有重複本單編號，請只保留一份。");
                if (item.Validation != "檢查通過")
                    throw new InvalidDataException(key + "：" + item.Validation);
                if (item.SourceContent == null || item.SourceContent.Length == 0 || item.SourceContent.Length > MaximumFileBytes)
                    throw new InvalidDataException(key + "：來源檔案無效或超過 10 MB。");
                if (item.Lines == null || item.Lines.Count == 0 || item.Lines.Any(line => line.Quantity <= 0 || line.UnitPrice < 0)
                    || item.Lines.Select(line => line.ExcelRow).Distinct().Count() != item.Lines.Count
                    || item.BeforeTax != item.Lines.Sum(line => line.Amount) - item.Adjustment
                    || item.BeforeTax < 0 || item.Tax < 0 || item.Total != item.BeforeTax + item.Tax)
                    throw new InvalidDataException(key + "：工作明細或金額不符。");
                if (item.Lines.SelectMany(line => new[] { line.Quantity, line.UnitPrice })
                    .Concat(new[] { item.Adjustment, item.BeforeTax, item.Tax, item.Total })
                    .Any(value => decimal.Round(value, 8) != value || Math.Abs(value) >= 100000000000000000000m))
                    throw new InvalidDataException(key + "：數值超出可儲存的精度。");
            }
        }
    }
}
