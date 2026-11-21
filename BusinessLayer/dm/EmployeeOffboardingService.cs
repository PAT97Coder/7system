using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BusinessLayer
{
    public sealed class OffboardingException : InvalidOperationException
    {
        public OffboardingException(string message) : base(message) { }
    }

    public sealed class EmployeeOffboardingService
    {
        public const long MaxFileSize = 100L * 1024 * 1024;

        private static string Normalize(string value)
        {
            value = (value ?? "").Trim().ToUpperInvariant();
            if (!Regex.IsMatch(value, @"\AVNW[0-9]{7}\z"))
                throw new OffboardingException("員工工號格式不正確。");
            return value;
        }

        public List<EmployeeOffboarding> GetRecords(string actorUserId, string userId)
        {
            actorUserId = Normalize(actorUserId);
            userId = Normalize(userId);
            using (var db = new DBDocumentManagementSystemEntities())
            {
                return db.EmployeeOffboardings.AsNoTracking().Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).ToList();
            }
        }

        public List<EmployeeOffboardingFile> GetFiles(string actorUserId, string userId, int recordId)
        {
            actorUserId = Normalize(actorUserId);
            userId = Normalize(userId);
            using (var db = new DBDocumentManagementSystemEntities())
            {
                return (from file in db.EmployeeOffboardingFiles.AsNoTracking()
                        join record in db.EmployeeOffboardings on file.OffboardingId equals record.Id
                        where record.UserId == userId && record.Id == recordId
                        orderby file.Id
                        select file).ToList();
            }
        }

        // File được ghi hoàn tất trước khi commit metadata; khi lưu thất bại chỉ dọn file do lần này tạo.
        public int Upload(string actorUserId, string userId, string reason, IEnumerable<string> sourceFiles)
        {
            actorUserId = Normalize(actorUserId);
            userId = Normalize(userId);
            reason = (reason ?? "").Trim();
            if (reason.Length == 0 || reason.Length > 2000)
                throw new OffboardingException("請填寫離職原因，最多可輸入 2000 個字。");
            var sources = (sourceFiles ?? Enumerable.Empty<string>()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (sources.Count == 0) throw new OffboardingException("請至少選擇一個附件。");
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var user = db.dm_User.AsNoTracking().Single(x => x.Id == userId);
                if (user.Status != 1 && !user.ResignPlan.HasValue)
                    throw new OffboardingException("僅限已離職或預報離職的員工上傳文件。");
                string root = GetStorageRoot(db);
                var record = new EmployeeOffboarding
                {
                    UserId = userId, Reason = reason, ResignDate = user.ResignDate,
                    DepartmentId = user.IdDepartment, JobCode = user.JobCode,
                    DisplayNameTW = user.DisplayName, DisplayNameVN = user.DisplayNameVN,
                    CreatedByUserId = actorUserId, CreatedAt = DateTime.Now
                };
                using (var batch = OffboardingFileStorage.Prepare(root, sources))
                using (var transaction = db.Database.BeginTransaction())
                {
                    var currentUser = db.dm_User.AsNoTracking().Single(x => x.Id == userId);
                    if (currentUser.Status != 1 && !currentUser.ResignPlan.HasValue)
                        throw new OffboardingException("員工狀態已變更，請重新整理後再上傳。");
                    db.EmployeeOffboardings.Add(record);
                    db.SaveChanges();
                    foreach (var file in batch.Files)
                        db.EmployeeOffboardingFiles.Add(new EmployeeOffboardingFile
                        {
                            OffboardingId = record.Id, OriginalName = file.OriginalName,
                            StoredPath = file.StoredPath, FileSize = file.FileSize,
                            CreatedByUserId = actorUserId, CreatedAt = record.CreatedAt
                        });
                    db.SaveChanges();
                    transaction.Commit();
                    batch.Complete();
                    return record.Id;
                }
            }
        }

        public void CopyFile(string actorUserId, string userId, int fileId, string destination)
        {
            actorUserId = Normalize(actorUserId);
            userId = Normalize(userId);
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var file = (from item in db.EmployeeOffboardingFiles.AsNoTracking()
                            join record in db.EmployeeOffboardings on item.OffboardingId equals record.Id
                            where item.Id == fileId && record.UserId == userId
                            select item).SingleOrDefault();
                if (file == null) throw new OffboardingException("找不到附件。");
                string source = OffboardingFileStorage.ResolvePath(GetStorageRoot(db), file.StoredPath);
                OffboardingFileStorage.CopyToDestination(source, destination);
            }
        }

        private static string GetStorageRoot(DBDocumentManagementSystemEntities db)
        {
            string root = db.sys_StaticValue.Where(x => x.KeyT == "EmployeeOffboardingFolder").Select(x => x.ValueT).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(root))
            {
                if (!Path.IsPathRooted(root)) throw new OffboardingException("共用文件儲存位置必須是完整路徑。");
                return root;
            }
            root = db.sys_StaticValue.Where(x => x.KeyT == "FolderData").Select(x => x.ValueT).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(root)) throw new OffboardingException("尚未設定共用文件儲存位置。");
            if (!Path.IsPathRooted(root)) throw new OffboardingException("共用文件儲存位置必須是完整路徑。");
            return Path.Combine(root, "401", "Offboarding");
        }
    }
}
