using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public string RecipientId { get; set; }
        public string RecipientTeam { get; set; }
        public bool? CanXuLy7810 { get; set; }
        public bool? CanXuLy7820 { get; set; }
        public bool Shi7810 => CanXuLy7810 == true;
        public bool Fou7810 => CanXuLy7810 == false;
        public bool Shi7820 => CanXuLy7820 == true;
        public bool Fou7820 => CanXuLy7820 == false;
        public dt318_TaskTeams Team7810 { get; set; }
        public dt318_TaskTeams Team7820 { get; set; }
        public string Progress7810 { get; set; }
        public string Progress7820 { get; set; }
        public string StatusText => Task318WorkflowState.Caption(Status);
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
                    var teams = context.dt318_TaskTeams.Where(r => taskIds.Contains(r.IdCongViec)).ToList();
                    var teamIds = teams.Select(r => r.Id).ToList();
                    var steps = context.dt318_TaskWorkflowSteps.Where(r => teamIds.Contains(r.IdTaskTeam)).ToList();
                    var userIds = taskUsers.Select(r => r.IdNguoiDung)
                        .Concat(tasks.Select(r => r.IdNguoiNhanAn))
                        .Concat(steps.Select(r => r.IdNguoiXuLy))
                        .Where(r => !string.IsNullOrWhiteSpace(r))
                        .Distinct()
                        .ToList();
                    var userNames = context.dm_User
                        .Where(r => userIds.Contains(r.Id))
                        .ToDictionary(r => r.Id, r => r.DisplayName);
                    var groupIds = steps.Where(r => r.IdNhomXuLy.HasValue).Select(r => r.IdNhomXuLy.Value).Distinct().ToList();
                    var groupNames = context.dm_Group.Where(r => groupIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r.DisplayName);

                    return tasks.Select(task =>
                    {
                        var assigneeIds = taskUsers
                            .Where(r => r.IdCongViec == task.Id)
                            .Select(r => r.IdNguoiDung)
                            .Distinct()
                            .ToList();
                        string recipient = task.IdNguoiNhanAn ?? (assigneeIds.Count == 1 ? assigneeIds[0] : null);
                        var team7810 = teams.SingleOrDefault(r => r.IdCongViec == task.Id && r.MaTo == "7810");
                        var team7820 = teams.SingleOrDefault(r => r.IdCongViec == task.Id && r.MaTo == "7820");

                        return new Task318ListRow
                        {
                            Id = task.Id,
                            DueDate = task.NgayDenHan,
                            Subject = task.ChuDe,
                            Description = task.GiaiThich,
                            Assignee = recipient == null ? "待確認收件人"
                                : userNames.TryGetValue(recipient, out string name) ? name : recipient,
                            RecipientId = recipient, RecipientTeam = task.MaToNhanAn,
                            CanXuLy7810 = task.CanXuLy7810, CanXuLy7820 = task.CanXuLy7820,
                            Team7810 = team7810, Team7820 = team7820,
                            Progress7810 = Progress(task.CanXuLy7810, team7810, steps, userNames, groupNames),
                            Progress7820 = Progress(task.CanXuLy7820, team7820, steps, userNames, groupNames),
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

        private static string Progress(bool? decision, dt318_TaskTeams team,
            List<dt318_TaskWorkflowSteps> steps, Dictionary<string, string> users, Dictionary<int, string> groups)
        {
            if (!decision.HasValue) return "未確認";
            if (!decision.Value) return "不參與";
            if (team == null) return "待建立課別資料";
            var active = steps.SingleOrDefault(r => r.IdTaskTeam == team.Id && r.TrangThai == "DangCho");
            if (active == null) return Task318WorkflowState.Caption(team.TrangThaiXuLy);
            string handler = active.IdNguoiXuLy != null
                ? users.TryGetValue(active.IdNguoiXuLy, out string user) ? user : active.IdNguoiXuLy
                : active.IdNhomXuLy.HasValue && groups.TryGetValue(active.IdNhomXuLy.Value, out string group) ? group : "未指定";
            return active.TenBuoc + "：" + handler;
        }

        /// <summary>
        /// Ghi một công việc đã nhập cùng người phụ trách và toàn bộ liên kết phụ kiện.
        /// Trả về 0 nếu MaNguon đã tồn tại, -1 nếu thất bại, ngược lại trả về Id công việc.
        /// </summary>
        public int Import(
            dt318_Tasks task,
            string assigneeId,
            List<Task318ImportAttachment> attachments,
            Task318Participants participants)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    using (var context = new DBDocumentManagementSystemEntities())
                    using (var transaction = context.Database.BeginTransaction(
                        System.Data.IsolationLevel.Serializable))
                    {
                        participants.Validate();
                        if (participants.GetTeam(assigneeId) == null)
                            throw new InvalidOperationException("收件人必須是已設定的 7810 / 7820 負責人。");
                        if (!string.IsNullOrWhiteSpace(task.MaNguon)
                            && context.dt318_Tasks.Any(r => r.MaNguon == task.MaNguon))
                            return 0;

                        task.IdNguoiNhanAn = assigneeId;
                        task.MaToNhanAn = participants.GetTeam(assigneeId);
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
                        dt318_TaskWorkflowBUS.InitializeTask(context, task, participants, task.NguoiTao, true);
                        context.SaveChanges();
                        transaction.Commit();
                        return task.Id;
                    }
                }
                catch (Exception ex)
                {
                    if (ImportConflict(ex))
                    {
                        if (attempt < 2) continue;
                        if (!string.IsNullOrWhiteSpace(task.MaNguon) && ExistsBySourceKey(task.MaNguon)) return 0;
                    }
                    logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                    return -1;
                }
            }
        }

        private static bool ImportConflict(Exception ex)
        {
            for (; ex != null; ex = ex.InnerException)
                if (ex is SqlException sql && (sql.Number == 1205 || sql.Number == 2601 || sql.Number == 2627)) return true;
            return false;
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
