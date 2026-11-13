using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt310_EquipmentInfoBUS
    {
        TPLogger logger;

        private static dt310_EquipmentInfoBUS instance;

        public static dt310_EquipmentInfoBUS Instance
        {
            get { if (instance == null) instance = new dt310_EquipmentInfoBUS(); return instance; }
            private set { instance = value; }
        }

        private dt310_EquipmentInfoBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt310_EquipmentInfo> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_EquipmentInfo
                        .Where(r => r.DeletedAt == null)
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt310_EquipmentInfo> GetListByDeptId(string deptId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_EquipmentInfo
                        .Where(r => r.DeletedAt == null && r.DeptId == deptId)
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt310_EquipmentInfo> GetListByDeptPrefix(string deptPrefix)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_EquipmentInfo
                        .Where(r => r.DeletedAt == null && r.DeptId != null && r.DeptId.StartsWith(deptPrefix))
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt310_EquipmentInfo> GetListByManagerId(string managerId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_EquipmentInfo
                        .Where(r => r.DeletedAt == null && r.ManagerId == managerId)
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt310_EquipmentInfo GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_EquipmentInfo.FirstOrDefault(r => r.Id == id && r.DeletedAt == null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt310_EquipmentInfo GetItemByCode(string code)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_EquipmentInfo
                        .FirstOrDefault(r => r.DeletedAt == null && r.Code == code);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt310_EquipmentInfo item)
        {
            ValidateEquipmentData(item);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    ValidateManager(_context, item.ManagerId);
                    ValidateDepartment(_context, item.DeptId);
                    EnsurePermission(_context, item.DeptId, item.CreatedBy);
                    _context.dt310_EquipmentInfo.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    WriteHistory(_context, item, "", "", item.CreatedBy, null);
                    transaction.Commit();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddRange(List<dt310_EquipmentInfo> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items) ValidateEquipmentData(item);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    foreach (var item in items)
                    {
                        ValidateManager(_context, item.ManagerId);
                        ValidateDepartment(_context, item.DeptId);
                        EnsurePermission(_context, item.DeptId, item.CreatedBy);
                    }
                    _context.dt310_EquipmentInfo.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    foreach (var item in items) WriteHistory(_context, item, "", "", item.CreatedBy, null);
                    transaction.Commit();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddOrUpdate(dt310_EquipmentInfo item, string changedBy, dt310_EquipmentInfo original)
        {
            ValidateEquipmentData(item);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var current = _context.dt310_EquipmentInfo.FirstOrDefault(r => r.Id == item.Id && r.DeletedAt == null);
                    EnsureUnchanged(current, original);
                    EnsurePermission(_context, current.DeptId, changedBy);
                    EnsurePermission(_context, item.DeptId, changedBy);
                    ValidateManager(_context, item.ManagerId);
                    ValidateDepartment(_context, item.DeptId);
                    string previousManager = current.ManagerId;
                    string previousDept = current.DeptId;
                    _context.Entry(current).CurrentValues.SetValues(item);
                    _context.SaveChanges();
                    WriteHistory(_context, current, previousManager, previousDept, changedBy, null);
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public sealed class ManagerChange
        {
            public int EquipmentId { get; set; }
            public string PreviousManagerId { get; set; }
            public string DeptId { get; set; }
            public string NewManagerId { get; set; }
        }

        public sealed class EquipmentHistory
        {
            public DateTime ChangedAt { get; set; }
            public string PreviousManager { get; set; }
            public string NewManager { get; set; }
            public string PreviousDeptId { get; set; }
            public string NewDeptId { get; set; }
            public string ChangedBy { get; set; }
            public int? HandoverId { get; set; }
        }

        public List<EquipmentHistory> GetHistory(int equipmentId)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            {
                return context.Database.SqlQuery<EquipmentHistory>(@"
SELECT h.ChangedAt,
       h.PreviousManagerId + N' ' + ISNULL(p.DisplayName, N'') AS PreviousManager,
       h.NewManagerId + N' ' + ISNULL(n.DisplayName, N'') AS NewManager,
       h.PreviousDeptId, h.NewDeptId,
       h.ChangedBy + N' ' + ISNULL(a.DisplayName, N'') AS ChangedBy, h.HandoverId
FROM dbo.dt310_EquipmentHistory h
LEFT JOIN dbo.dm_User p ON p.Id = h.PreviousManagerId
LEFT JOIN dbo.dm_User n ON n.Id = h.NewManagerId
LEFT JOIN dbo.dm_User a ON a.Id = h.ChangedBy
WHERE h.EquipmentId = @p0 ORDER BY h.Id DESC", equipmentId).ToList();
            }
        }

        private static void ValidateManager(DBDocumentManagementSystemEntities context, string managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId) || !context.dm_User.Any(r => r.Id == managerId && r.Status == 0))
                throw new InvalidOperationException("接任人必須是在職人員，請重新選擇。");
        }

        private static void ValidateEquipmentData(dt310_EquipmentInfo item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Code) || item.Code.Length > 50 ||
                string.IsNullOrWhiteSpace(item.DisplayNameVN) || item.DisplayNameVN.Length > 255 ||
                string.IsNullOrWhiteSpace(item.DisplayNameTW) || item.DisplayNameTW.Length > 255 ||
                string.IsNullOrWhiteSpace(item.DeptId) || item.DeptId.Length > 20 ||
                string.IsNullOrWhiteSpace(item.ManagerId) || item.ManagerId.Length > 20 || (item.Note?.Length ?? 0) > 500)
                throw new InvalidOperationException("請填寫完整設備資料，並確認欄位長度符合限制。");
        }

        private static void ValidateDepartment(DBDocumentManagementSystemEntities context, string deptId)
        {
            if (string.IsNullOrWhiteSpace(deptId) || !context.dm_Departments.Any(r => r.Id == deptId && r.IsActive && r.IsGroup != true))
                throw new InvalidOperationException("請選擇有效的管理部門。");
        }

        private static void EnsureUnchanged(dt310_EquipmentInfo current, dt310_EquipmentInfo original)
        {
            if (current == null || original == null || current.DeletedAt.HasValue || current.Id != original.Id ||
                current.Code != original.Code || current.DisplayNameVN != original.DisplayNameVN ||
                current.DisplayNameTW != original.DisplayNameTW || current.DeptId != original.DeptId ||
                current.ManagerId != original.ManagerId || current.Note != original.Note)
                throw new InvalidOperationException("設備資料已變更或已刪除，請關閉後重新載入。");
        }

        private static void EnsurePermission(DBDocumentManagementSystemEntities context, string deptId, string actor)
        {
            var names = (from membership in context.dm_GroupUser
                         join groupInfo in context.dm_Group on membership.IdGroup equals groupInfo.Id
                         where membership.IdUser == actor
                         select groupInfo.DisplayName).ToList();
            if (!names.Any(r => r?.Trim() == "安衛環7" || r?.Trim() == "安衛環" + deptId))
                throw new InvalidOperationException("您無權管理此部門的設備。");
        }

        private static void WriteHistory(DBDocumentManagementSystemEntities context, dt310_EquipmentInfo item,
            string previousManager, string previousDept, string changedBy, int? handoverId)
        {
            if (previousManager == item.ManagerId && previousDept == item.DeptId) return;
            context.Database.ExecuteSqlCommand(@"
INSERT INTO dbo.dt310_EquipmentHistory
    (EquipmentId, PreviousManagerId, NewManagerId, PreviousDeptId, NewDeptId, ChangedBy, ChangedAt, HandoverId)
VALUES (@p0, @p1, @p2, @p3, @p4, @p5, GETDATE(), @p6)",
                item.Id, previousManager, item.ManagerId, previousDept, item.DeptId, changedBy, (object)handoverId ?? DBNull.Value);
        }

        // Compare the reviewed owner and department under one transaction; never partially transfer a selection.
        public void TransferManagers(List<ManagerChange> changes, string changedBy, int? handoverId = null,
            string expectedDataJson = null, Action<DBDocumentManagementSystemEntities> applyRelatedChanges = null)
        {
            changes = changes ?? new List<ManagerChange>();
            if (changes.Count == 0 && !handoverId.HasValue) return;
            if (changes.Any(r => r == null || r.EquipmentId <= 0 || string.IsNullOrWhiteSpace(r.DeptId) ||
                string.IsNullOrWhiteSpace(r.PreviousManagerId) || string.IsNullOrWhiteSpace(r.NewManagerId) || r.NewManagerId == r.PreviousManagerId))
                throw new InvalidOperationException("交接資料不完整，或接任人與原管理人相同，請重新選擇。");
            if (changes.Select(r => r.EquipmentId).Distinct().Count() != changes.Count)
                throw new InvalidOperationException("設備資料重複，請重新載入。");

            using (var context = new DBDocumentManagementSystemEntities())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var memberships = context.dm_GroupUser.Where(r => r.IdUser == changedBy).Select(r => r.IdGroup).ToList();
                var groupNames = context.dm_Group.Where(r => memberships.Contains(r.Id)).Select(r => r.DisplayName).ToList();
                bool isAdmin = groupNames.Any(r => r?.Trim() == "安衛環7");
                dt310_UpdateLeaveUser workflow = null;
                if (handoverId.HasValue)
                {
                    workflow = context.dt310_UpdateLeaveUser.FirstOrDefault(r => r.Id == handoverId.Value);
                    if (workflow == null || workflow.IsCancel || !workflow.IsProcess || !memberships.Contains(workflow.IdGroupProcess ?? -1)
                        || workflow.DataJson != expectedDataJson)
                        throw new InvalidOperationException("您無權審核此交接申請，或申請狀態已變更。");
                    var detail = context.dt310_UpdateLeaveUser_detail.FirstOrDefault(r => r.IdUpdateData == workflow.Id && r.IdGroup == workflow.IdGroupProcess);
                    if (detail == null || context.dt310_UpdateLeaveUser_detail.Any(r => r.IdUpdateData == workflow.Id && r.IndexStep > detail.IndexStep))
                        throw new InvalidOperationException("設備只能在最後一級審核完成時交接。");
                    var ownedIds = context.dt310_EquipmentInfo.Where(r => r.DeletedAt == null && r.ManagerId == workflow.IdUserLeave).Select(r => r.Id).ToList();
                    if (ownedIds.Count != changes.Count || changes.Any(r => !ownedIds.Contains(r.EquipmentId) || r.PreviousManagerId != workflow.IdUserLeave))
                        throw new InvalidOperationException("設備清單已變更或舊申請未包含設備，請退回重新填寫交接資料。");
                    detail.IdUser = changedBy;
                    detail.TimeSubmit = DateTime.Now;
                    workflow.IsProcess = false;
                    workflow.IdGroupProcess = -1;
                }

                foreach (var change in changes)
                {
                    var equipment = context.dt310_EquipmentInfo.FirstOrDefault(r => r.Id == change.EquipmentId && r.DeletedAt == null);
                    if (equipment == null || equipment.ManagerId != change.PreviousManagerId || equipment.DeptId != change.DeptId)
                        throw new InvalidOperationException("設備管理人或部門已變更，請重新載入並確認交接資料。");
                    if (!handoverId.HasValue && !isAdmin && !groupNames.Any(r => r?.Trim() == "安衛環" + equipment.DeptId))
                        throw new InvalidOperationException("您無權管理所選設備。");
                    ValidateManager(context, change.NewManagerId);
                    if (change.PreviousManagerId == change.NewManagerId)
                        throw new InvalidOperationException("接任人不可與原管理人相同。");
                    equipment.ManagerId = change.NewManagerId;
                    WriteHistory(context, equipment, change.PreviousManagerId, change.DeptId, changedBy, handoverId);
                }
                applyRelatedChanges?.Invoke(context);
                context.SaveChanges();
                transaction.Commit();
            }
        }

        public bool RemoveById(int id, string userDel, dt310_EquipmentInfo original)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                using (var transaction = _context.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var itemRemove = _context.dt310_EquipmentInfo.FirstOrDefault(r => r.Id == id);
                    EnsureUnchanged(itemRemove, original);
                    EnsurePermission(_context, itemRemove.DeptId, userDel);

                    itemRemove.DeletedAt = DateTime.Now;
                    itemRemove.DeletedBy = userDel;

                    int affectedRecords = _context.SaveChanges();
                    transaction.Commit();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
