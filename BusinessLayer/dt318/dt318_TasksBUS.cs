using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public sealed class Task318ListRow
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Assignee { get; set; }
        public string Status { get; set; }
        public string SharedRootPath { get; set; }
        public string TargetFolderPath { get; set; }
        public string SourceFolderPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class Task318ImportAttachment
    {
        public dm_Attachment Attachment { get; set; }
        public string AttachmentType { get; set; }
    }

    public class dt318_TasksBUS
    {
        private readonly TPLogger logger;
        private static dt318_TasksBUS instance;

        public static dt318_TasksBUS Instance
        {
            get { if (instance == null) instance = new dt318_TasksBUS(); return instance; }
            private set { instance = value; }
        }

        private dt318_TasksBUS()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        /// <summary>
        /// Lấy các công việc chưa bị xoá, ưu tiên công việc gần đến hạn.
        /// </summary>
        public List<dt318_Tasks> GetList()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks
                        .Where(r => r.NgayXoa == null)
                        .OrderBy(r => r.NgayDenHan)
                        .ThenByDescending(r => r.NgayTao)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt318_Tasks> GetListIsRemove()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks
                        .Where(r => r.NgayXoa != null)
                        .OrderByDescending(r => r.NgayXoa)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt318_Tasks GetItemById(int id)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool ExistsBySourceKey(string sourceKey)
        {
            if (string.IsNullOrWhiteSpace(sourceKey)) return false;

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                    return context.dt318_Tasks.Any(r => r.MaNguon == sourceKey);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<Task318ListRow> GetListRows()
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var tasks = context.dt318_Tasks
                        .Where(r => r.NgayXoa == null)
                        .OrderBy(r => r.NgayDenHan)
                        .ThenByDescending(r => r.NgayTao)
                        .ToList();
                    var taskIds = tasks.Select(r => r.Id).ToList();
                    var taskUsers = context.dt318_TaskUsers
                        .Where(r => taskIds.Contains(r.IdCongViec) && r.NgayXoa == null)
                        .ToList();
                    var userIds = taskUsers.Select(r => r.IdNguoiDung)
                        .Concat(tasks.Select(r => r.NguoiTao))
                        .Where(r => !string.IsNullOrWhiteSpace(r))
                        .Distinct()
                        .ToList();
                    var userNames = context.dm_User
                        .Where(r => userIds.Contains(r.Id))
                        .ToDictionary(r => r.Id, r => r.DisplayName);

                    return tasks.Select(task =>
                    {
                        var assigneeIds = taskUsers
                            .Where(r => r.IdCongViec == task.Id)
                            .Select(r => r.IdNguoiDung)
                            .Distinct()
                            .ToList();
                        if (assigneeIds.Count == 0) assigneeIds.Add(task.NguoiTao);

                        return new Task318ListRow
                        {
                            Id = task.Id,
                            DueDate = task.NgayDenHan,
                            Subject = task.ChuDe,
                            Description = task.GiaiThich,
                            Assignee = string.Join("、", assigneeIds.Select(id =>
                                userNames.TryGetValue(id ?? string.Empty, out string name)
                                    ? name
                                    : id)),
                            Status = task.TrangThai,
                            SharedRootPath = task.DuongDanThuMucChung,
                            TargetFolderPath = task.DuongDanThuMucCongViec,
                            SourceFolderPath = task.DuongDanThuMucNguon,
                            CreatedAt = task.NgayTao
                        };
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Ghi một công việc đã nhập cùng người phụ trách và toàn bộ liên kết phụ kiện.
        /// Trả về 0 nếu MaNguon đã tồn tại, -1 nếu thất bại, ngược lại trả về Id công việc.
        /// </summary>
        public int Import(
            dt318_Tasks task,
            string assigneeId,
            List<Task318ImportAttachment> attachments)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var transaction = context.Database.BeginTransaction(
                    System.Data.IsolationLevel.Serializable))
                {
                    if (!string.IsNullOrWhiteSpace(task.MaNguon)
                        && context.dt318_Tasks.Any(r => r.MaNguon == task.MaNguon))
                        return 0;

                    context.dt318_Tasks.Add(task);
                    context.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(assigneeId))
                    {
                        context.dt318_TaskUsers.Add(new dt318_TaskUsers
                        {
                            IdCongViec = task.Id,
                            IdNguoiDung = assigneeId,
                            NgayTao = task.NgayTao,
                            NguoiTao = task.NguoiTao
                        });
                    }

                    foreach (var item in attachments ?? new List<Task318ImportAttachment>())
                    {
                        context.dm_Attachment.Add(item.Attachment);
                        context.SaveChanges();
                        context.dt318_TaskAttach.Add(new dt318_TaskAttach
                        {
                            IdCongViec = task.Id,
                            IdDinhKem = item.Attachment.Id,
                            LoaiDinhKem = item.AttachmentType
                        });
                    }

                    context.SaveChanges();
                    transaction.Commit();
                    return task.Id;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Dùng cho dịch vụ OCR lấy các công việc đang chờ nhận dạng.
        /// </summary>
        public List<dt318_Tasks> GetListByOcrStatus(string trangThaiOcr)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    return context.dt318_Tasks
                        .Where(r => r.NgayXoa == null && r.TrangThaiOCR == trangThaiOcr)
                        .OrderBy(r => r.NgayTao)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Thêm công việc và trả về Id vừa tạo.
        /// </summary>
        public int Add(dt318_Tasks item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_Tasks.Add(item);
                    return context.SaveChanges() > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddOrUpdate(dt318_Tasks item)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    context.dt318_Tasks.AddOrUpdate(item);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Xoá mềm công việc để giữ lại lịch sử dữ liệu.
        /// </summary>
        public bool RemoveById(int id, string idNguoiDung)
        {
            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                {
                    var item = context.dt318_Tasks.FirstOrDefault(r => r.Id == id);
                    if (item == null || item.NgayXoa != null) return false;

                    item.NgayXoa = DateTime.Now;
                    item.NguoiXoa = idNguoiDung;
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }
    }
}
