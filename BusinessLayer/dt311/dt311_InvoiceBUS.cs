using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt311_InvoiceBUS
    {
        TPLogger logger;

        private static dt311_InvoiceBUS instance;

        public static dt311_InvoiceBUS Instance
        {
            get { if (instance == null) instance = new dt311_InvoiceBUS(); return instance; }
            private set { instance = value; }
        }

        private dt311_InvoiceBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt311_Invoice> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetListByStartDeptId(string deptId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.Where(r => r.IdDept.StartsWith(deptId)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetListFuleByStartDeptId(string deptId, int year)
        {
            if (year < 1 || year > 9998) throw new ArgumentOutOfRangeException(nameof(year));
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1);
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.Where(r => r.IdDept.StartsWith(deptId)
                        && !string.IsNullOrEmpty(r.LicensePlate)
                        && r.IssueDate >= startDate && r.IssueDate < endDate).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetListByStartDeptId(string deptId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice
                        .Where(r => r.IdDept.StartsWith(deptId)
                                    && r.IssueDate >= startDate
                                    && r.IssueDate <= endDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetFuelLogWithPrevious(string deptId, DateTime startDate, DateTime endDate)
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                // 🔹 Lấy toàn bộ dữ liệu trước endDate để tìm lần đổ trước đó
                var allLogs = _context.dt311_Invoice
                    .Where(f => f.IdDept.StartsWith(deptId)
                             && f.IssueDate <= endDate
                             && !string.IsNullOrEmpty(f.LicensePlate))
                    .OrderBy(f => f.LicensePlate)
                    .ThenBy(f => f.IssueDate)
                    .ToList();

                // 🔹 Lọc ra các bản ghi nằm trong khoảng thời gian mong muốn
                var rangeLogs = allLogs
                    .Where(f => f.IssueDate >= startDate && f.IssueDate <= endDate)
                    .ToList();

                // 🔹 Tạo danh sách kết quả kèm theo bản ghi lần đổ trước đó (nếu có)
                var result = rangeLogs
                    .Select(f =>
                    {
                        // Lấy bản ghi trước đó theo xe, có thể nằm ngoài startDate
                        var prev = allLogs
                            .Where(p => p.LicensePlate == f.LicensePlate && p.IssueDate < f.IssueDate)
                            .OrderByDescending(p => p.IssueDate)
                            .FirstOrDefault();

                        return new dt311_Invoice
                        {
                            LicensePlate = f.LicensePlate,
                              IssueDate = f.IssueDate,
                            // BuyerTax dùng để hiển thị ngày trước và sau (tuỳ bạn đặt tên cột)
                            BuyerTax = $"{prev?.IssueDate:yyyy/MM/dd} - {f.IssueDate:yyyy/MM/dd}",
                            // SellerTax hiển thị km trước và sau
                            SellerTax = $"{prev?.OdometerReading} - {f.OdometerReading}",
                            // OdometerReading hiển thị chênh lệch km
                            OdometerReading = prev != null ? f.OdometerReading - prev.OdometerReading : 0,
                            FuelFilledBy = f.FuelFilledBy,
                            InvoiceCode = f.InvoiceCode,
                            InvoiceNumber = f.InvoiceNumber,
                            TransactionID = f.TransactionID,
                            TotalAfterVAT = f.TotalAfterVAT,
                            TotalBeforeVAT = f.TotalBeforeVAT
                        };
                    })
                    .ToList();

                return result;
            }
        }

        public dt311_Invoice GetItemById(string id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.FirstOrDefault(r => r.TransactionID == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt311_Invoice GetItemBytransactionID(string transactionID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.FirstOrDefault(r => r.TransactionID == transactionID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public string Add(dt311_Invoice item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt311_Invoice.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    if (affectedRecords > 0)
                        return item.TransactionID;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return null;
            }
        }

        public bool AddRange(List<dt311_Invoice> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt311_Invoice.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public string AddOrUpdate(dt311_Invoice item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt311_Invoice.AddOrUpdate(item);
                    int affectedRecords = _context.SaveChanges();
                    if (affectedRecords > 0)
                        return item.TransactionID;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return null;
            }
        }

        // Dùng quyền mở chức năng hóa đơn hiện có, không tạo mã quyền song song.
        public bool CanImportFpgFuel(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;
            userId = userId.Trim().ToUpperInvariant();
            using (var context = new DBDocumentManagementSystemEntities())
            {
                return HasFpgImportPermission(context, userId);
            }
        }

        private static bool HasFpgImportPermission(DBDocumentManagementSystemEntities context, string userId)
        {
            return context.dm_User.Any(user => user.Id == userId && user.Status == 0)
                && (from userRole in context.dm_UserRole
                    join functionRole in context.dm_FunctionRole on userRole.IdRole equals functionRole.IdRole
                    join function in context.dm_Function on functionRole.IdFunction equals function.Id
                    where userRole.IdUser == userId && function.ControlName == "uc311_ExpenseMain"
                    select function.Id).Any();
        }

        public bool UpdateFuelInfoBatch(string userId, List<dt311_Invoice> items, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(userId))
            {
                message = "您沒有匯入加油資料的權限。";
                return false;
            }
            userId = userId.Trim().ToUpperInvariant();
            if (items == null || items.Count == 0)
            {
                message = "沒有可更新的資料。";
                return false;
            }

            try
            {
                var distinctItems = items
                    .Where(item => item != null && !string.IsNullOrWhiteSpace(item.TransactionID))
                    .GroupBy(item => item.TransactionID)
                    .Select(group => group.First())
                    .ToList();
                if (distinctItems.Count == 0)
                {
                    message = "沒有有效的發票資料可更新。";
                    return false;
                }

                var transactionIds = distinctItems.Select(item => item.TransactionID).ToList();

                using (var _context = new DBDocumentManagementSystemEntities())
                using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    if (!HasFpgImportPermission(_context, userId))
                    {
                        message = "您沒有匯入加油資料的權限。";
                        return false;
                    }
                    var actor = _context.dm_User.Single(user => user.Id == userId);
                    if (string.IsNullOrWhiteSpace(actor.IdDepartment) || actor.IdDepartment.Length < 2)
                    {
                        message = "使用者部門資料不完整，無法匯入。";
                        return false;
                    }
                    string departmentPrefix = actor.IdDepartment.Substring(0, 2);
                    var invoices = _context.dt311_Invoice
                        .Where(invoice => transactionIds.Contains(invoice.TransactionID)
                            && invoice.IdDept.StartsWith(departmentPrefix))
                        .ToList();

                    if (invoices.Count != transactionIds.Count)
                    {
                        message = "部分發票已不存在，未更新任何資料。請重新載入後再試。";
                        transaction.Rollback();
                        return false;
                    }

                    var updatesById = distinctItems.ToDictionary(item => item.TransactionID);
                    foreach (dt311_Invoice invoice in invoices)
                    {
                        dt311_Invoice update = updatesById[invoice.TransactionID];
                        // Không ghi đè dữ liệu đã thay đổi sau khi mở bản xem trước.
                        if ((!string.IsNullOrWhiteSpace(invoice.LicensePlate)
                                && !string.Equals(
                                    System.Text.RegularExpressions.Regex.Replace(invoice.LicensePlate, "[^A-Za-z0-9]", ""),
                                    System.Text.RegularExpressions.Regex.Replace(update.LicensePlate ?? "", "[^A-Za-z0-9]", ""),
                                    StringComparison.OrdinalIgnoreCase))
                            || (invoice.OdometerReading.HasValue && invoice.OdometerReading != update.OdometerReading)
                            || (!string.IsNullOrWhiteSpace(invoice.FuelFilledBy)
                                && invoice.FuelFilledBy != update.FuelFilledBy))
                        {
                            message = "發票資料已變更，未更新任何資料。請重新匯入。";
                            return false;
                        }
                        invoice.LicensePlate = update.LicensePlate;
                        invoice.OdometerReading = update.OdometerReading;
                        invoice.FuelFilledBy = update.FuelFilledBy;
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                message = "批次更新加油資料失敗：" + ex.Message;
                return false;
            }
        }

        public bool RemoveById(string id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt311_Invoice.FirstOrDefault(r => r.TransactionID == id);
                    _context.dt311_Invoice.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
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
