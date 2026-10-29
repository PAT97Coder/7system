using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    /// <summary>
    /// Nghiệp vụ 318 dùng chung một context/transaction cho Án, tổ, bước và lịch sử.
    /// Không gửi hoặc xếp hàng Notes; service thông báo được tích hợp sau.
    /// </summary>
    public sealed class dt318_TaskWorkflowBUS
    {
        private static dt318_TaskWorkflowBUS instance;
        public static dt318_TaskWorkflowBUS Instance => instance ?? (instance = new dt318_TaskWorkflowBUS());
        private readonly TPLogger logger = new TPLogger(typeof(dt318_TaskWorkflowBUS).FullName);

        private dt318_TaskWorkflowBUS() { }

        private T Write<T>(Func<DBDocumentManagementSystemEntities, T> action)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    using (var context = new DBDocumentManagementSystemEntities())
                    using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        T result = action(context);
                        context.SaveChanges();
                        transaction.Commit();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    // Serializable bảo vệ các liên kết không có UNIQUE/FK. Khi có
                    // deadlock do hai phiên nâng khóa, thử lại bằng context mới.
                    if (attempt < 2 && IsDeadlock(ex)) continue;
                    logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                    throw;
                }
            }
        }

        private static bool IsDeadlock(Exception ex)
        {
            for (; ex != null; ex = ex.InnerException)
                if (ex is SqlException sql && sql.Number == 1205) return true;
            return false;
        }

        private static void RequireActor(DBDocumentManagementSystemEntities context, string actor)
        {
            if (string.IsNullOrWhiteSpace(actor) || !context.dm_User.Any(r => r.Id == actor))
                throw new InvalidOperationException("找不到目前使用者，請重新登入。");
        }

        private static dt318_Tasks Task(DBDocumentManagementSystemEntities context, int taskId)
        {
            var task = context.dt318_Tasks.SingleOrDefault(r => r.Id == taskId && r.NgayXoa == null);
            if (task == null) throw new InvalidOperationException("交辦事項不存在或已刪除。");
            return task;
        }

        public static bool? Decision(dt318_Tasks task, string team)
        {
            if (team == "7810") return task.CanXuLy7810;
            if (team == "7820") return task.CanXuLy7820;
            throw new InvalidOperationException("課別必須是 7810 或 7820。");
        }

        private static void SetDecision(dt318_Tasks task, string team, bool value)
        {
            if (team == "7810") task.CanXuLy7810 = value;
            else if (team == "7820") task.CanXuLy7820 = value;
            else throw new InvalidOperationException("課別必須是 7810 或 7820。");
        }

        private static void Version(byte[] actual, byte[] expected)
        {
            // EDMX Database First chỉ đánh dấu Computed cho rowversion, nên
            // kiểm tra token trực tiếp trong giao dịch, không dựa vào AddOrUpdate.
            if (expected == null || actual == null || !actual.SequenceEqual(expected))
                throw new InvalidOperationException("資料已被其他使用者更新，請重新載入後再操作。");
        }

        private static void Stamp(DBDocumentManagementSystemEntities context, dt318_Tasks task, dt318_TaskTeams team, string actor)
        {
            task.NgayCapNhat = DateTime.Now;
            task.NguoiCapNhat = actor;
            team.NgayCapNhat = DateTime.Now;
            team.NguoiCapNhat = actor;
            context.Entry(team).Property(r => r.NgayCapNhat).IsModified = true;
        }

        private static void History(DBDocumentManagementSystemEntities context, dt318_Tasks task,
            dt318_TaskTeams team, string actor, string kind, string oldValue, string newValue,
            string description, int? stepId = null)
        {
            context.dt318_TaskHistory.Add(new dt318_TaskHistory
            {
                IdCongViec = task.Id, IdTaskTeam = team?.Id, IdWorkflowStep = stepId,
                LoaiSuKien = kind, GiaTriCu = oldValue, GiaTriMoi = newValue,
                NoiDung = description, NguoiThucHien = actor, NgayThucHien = DateTime.Now
            });
        }

        internal static void InitializeTask(DBDocumentManagementSystemEntities context,
            dt318_Tasks task, Task318Participants participants, string actor, bool isNew)
        {
            participants.Validate();
            RequireActor(context, actor);
            if (!context.dm_User.Any(r => r.Id == participants.User7810)
                || !context.dm_User.Any(r => r.Id == participants.User7820))
                throw new InvalidOperationException("找不到設定的 7810 / 7820 負責人。");

            bool recipientChanged = false;
            if (string.IsNullOrWhiteSpace(task.IdNguoiNhanAn))
            {
                var recipients = context.dt318_TaskUsers
                    .Where(r => r.IdCongViec == task.Id && r.NgayXoa == null)
                    .Select(r => r.IdNguoiDung).Distinct().ToList();
                string singleRecipient = recipients.Count == 1 ? recipients[0] : null;
                if (singleRecipient != null && context.dm_User.Any(r => r.Id == singleRecipient))
                {
                    task.IdNguoiNhanAn = singleRecipient;
                    recipientChanged = true;
                }
            }
            if (string.IsNullOrWhiteSpace(task.MaToNhanAn))
            {
                string recipientTeam = participants.GetTeam(task.IdNguoiNhanAn);
                if (recipientTeam != null)
                {
                    task.MaToNhanAn = recipientTeam;
                    recipientChanged = true;
                }
            }
            if (recipientChanged)
                History(context, task, null, actor, "XacDinhNguoiNhan", null,
                    task.IdNguoiNhanAn + " / " + task.MaToNhanAn, "補上原收件人資料。");

            var teams = context.dt318_TaskTeams.Where(r => r.IdCongViec == task.Id).ToList();
            bool createdTeam = false;
            if (teams.GroupBy(r => r.MaTo).Any(g => g.Count() > 1))
                throw new InvalidOperationException("同一事項存在重複課別資料，請先確認資料。");
            foreach (string code in new[] { "7810", "7820" })
            {
                var team = teams.SingleOrDefault(r => r.MaTo == code);
                // Chỉ đặt mặc định khi khởi tạo dòng tổ. Một lần tải lại không
                // được ghi đè 是/否 hoặc khôi phục lựa chọn người dùng đã thay đổi.
                if (team != null) continue;
                if (task.MaToNhanAn == code && !Decision(task, code).HasValue)
                    SetDecision(task, code, true);
                bool? decision = Decision(task, code);
                team = new dt318_TaskTeams
                {
                    IdCongViec = task.Id, MaTo = code, IdNguoiPhuTrach = participants.GetUser(code),
                    NguonXacNhan = decision.HasValue ? (isNew ? "MacDinh" : "ChuyenDoi") : null,
                    NgayXacNhan = decision.HasValue ? (DateTime?)DateTime.Now : null,
                    TrangThaiXuLy = !decision.HasValue ? "ChuaXacNhan" : decision.Value ? "ChoXuLy" : "KhongThamGia",
                    NgayTao = DateTime.Now, NguoiTao = actor
                };
                context.dt318_TaskTeams.Add(team);
                createdTeam = true;
                context.SaveChanges();
                teams.Add(team);
                History(context, task, team, actor, "KhoiTaoTo", null,
                    decision.HasValue ? decision.Value ? "是" : "否" : "未確認", "建立課別處理資料。");
            }
            if (isNew || (createdTeam && task.TrangThai == "ChoXuLy")) Overall(task, teams);
        }

        public void PrepareExistingTasks(Task318Participants participants, string actor)
        {
            Write(context =>
            {
                participants.Validate();
                RequireActor(context, actor);
                if (!context.dm_User.Any(r => r.Id == participants.User7810)
                    || !context.dm_User.Any(r => r.Id == participants.User7820))
                    throw new InvalidOperationException("找不到設定的兩課負責人。");
                var keys = context.dt318_TaskTeams.Select(r => new { r.IdCongViec, r.MaTo }).ToList();
                var ready = new HashSet<int>(keys.GroupBy(r => r.IdCongViec)
                    .Where(g => g.Count() == 2 && g.Any(r => r.MaTo == "7810") && g.Any(r => r.MaTo == "7820"))
                    .Select(g => g.Key));
                var pending = context.dt318_Tasks.Where(r => r.NgayXoa == null).OrderBy(r => r.Id).ToList()
                    .Where(r => !ready.Contains(r.Id) || string.IsNullOrWhiteSpace(r.IdNguoiNhanAn) || string.IsNullOrWhiteSpace(r.MaToNhanAn));
                foreach (var task in pending)
                    InitializeTask(context, task, participants, actor, false);
                return true;
            });
        }

        private static void Overall(dt318_Tasks task, List<dt318_TaskTeams> teams)
        {
            if (!task.CanXuLy7810.HasValue || !task.CanXuLy7820.HasValue)
                task.TrangThai = "ChoXacNhanTo";
            else
            {
                var participating = teams.Where(r => Decision(task, r.MaTo) == true).ToList();
                task.TrangThai = participating.Count == 0 ? "KhongThamGia"
                    : participating.All(r => r.TrangThaiXuLy == "HoanThanh") ? "HoanThanh"
                    : participating.Any(r => r.TrangThaiXuLy == "DangXuLy" || r.TrangThaiXuLy == "ChoXacNhan")
                        ? "DangXuLy" : "ChoXuLy";
            }
        }

        private static dt318_TaskTeams Team(DBDocumentManagementSystemEntities context,
            int taskId, string code)
        {
            if (code != "7810" && code != "7820") throw new InvalidOperationException("課別錯誤。");
            var team = context.dt318_TaskTeams.SingleOrDefault(r => r.IdCongViec == taskId && r.MaTo == code);
            if (team == null) throw new InvalidOperationException("尚未建立課別資料，請重新載入。");
            return team;
        }

        private static void Owner(dt318_TaskTeams team, string actor)
        {
            if (team.IdNguoiPhuTrach != actor)
                throw new InvalidOperationException("只有本課負責人可以確認參與或設定流程。");
        }

        public void ConfirmTeam(int taskId, string code, bool decision, string actor, byte[] teamVersion)
        {
            Write(context =>
            {
                RequireActor(context, actor);
                var task = Task(context, taskId);
                var team = Team(context, taskId, code);
                Owner(team, actor);
                Version(team.RowVersion, teamVersion);
                if (Decision(task, code) == decision) return false;
                var steps = context.dt318_TaskWorkflowSteps.Where(r => r.IdTaskTeam == team.Id).ToList();
                if (!decision && steps.Any(r => r.TrangThai == "DaXacNhan"))
                    throw new InvalidOperationException("已有步驟完成確認，不能取消本課參與。");
                string before = Decision(task, code).HasValue ? Decision(task, code).Value ? "是" : "否" : "未確認";
                SetDecision(task, code, decision);
                team.NguonXacNhan = "NguoiDung";
                team.NguoiXacNhan = actor;
                team.NgayXacNhan = DateTime.Now;
                team.TrangThaiXuLy = decision ? "ChoXuLy" : "KhongThamGia";
                foreach (var step in steps.Where(r => r.TrangThai == "DangCho")) step.TrangThai = "ChuaDen";
                Stamp(context, task, team, actor);
                History(context, task, team, actor, "XacNhanTo", before, decision ? "是" : "否", "確認本課是否參與。");
                Overall(task, context.dt318_TaskTeams.Where(r => r.IdCongViec == taskId).ToList());
                return true;
            });
        }

        private static string Describe(DBDocumentManagementSystemEntities context, IEnumerable<dt318_TaskWorkflowSteps> steps)
        {
            var rows = steps.OrderBy(r => r.ThuTu).ToList();
            var userIds = rows.Where(r => r.IdNguoiXuLy != null).Select(r => r.IdNguoiXuLy).Distinct().ToList();
            var groupIds = rows.Where(r => r.IdNhomXuLy.HasValue).Select(r => r.IdNhomXuLy.Value).Distinct().ToList();
            var users = context.dm_User.Where(r => userIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r.DisplayName);
            var groups = context.dm_Group.Where(r => groupIds.Contains(r.Id)).ToDictionary(r => r.Id, r => r.DisplayName);
            return string.Join("\n", rows.Select(r =>
            {
                string name = r.IdNguoiXuLy != null
                    ? users.TryGetValue(r.IdNguoiXuLy, out string user) ? user ?? r.IdNguoiXuLy : r.IdNguoiXuLy
                    : r.IdNhomXuLy.HasValue && groups.TryGetValue(r.IdNhomXuLy.Value, out string group) ? group : "未指定";
                return $"{r.ThuTu}. {r.TenBuoc} → {name}（{Task318WorkflowState.Caption(r.TrangThai)}）";
            }));
        }

        public void SaveWorkflow(int taskId, string code, IList<Task318StepDraft> drafts,
            string actor, byte[] teamVersion)
        {
            Write(context =>
            {
                RequireActor(context, actor);
                var task = Task(context, taskId);
                var team = Team(context, taskId, code);
                Owner(team, actor);
                Version(team.RowVersion, teamVersion);
                if (Decision(task, code) != true) throw new InvalidOperationException("請先確認本課參與（是）。");
                if (team.TrangThaiXuLy == "HoanThanh") throw new InvalidOperationException("本課流程已完成，不能修改。");
                if (drafts == null || drafts.Count == 0) throw new InvalidOperationException("流程至少需要一個處理步驟。");
                var steps = context.dt318_TaskWorkflowSteps.Where(r => r.IdTaskTeam == team.Id).OrderBy(r => r.ThuTu).ToList();
                string before = Describe(context, steps);
                var completed = steps.Where(r => r.TrangThai == "DaXacNhan").ToList();
                if (drafts.Count < completed.Count || drafts.Where(r => r.Id != 0).GroupBy(r => r.Id).Any(g => g.Count() > 1))
                    throw new InvalidOperationException("流程有重複步驟，或刪除了已確認的步驟。");
                for (int i = 0; i < completed.Count; i++)
                {
                    var old = completed[i]; var draft = drafts[i];
                    if (draft.Id != old.Id || old.ThuTu != i + 1 || draft.TenBuoc != old.TenBuoc
                        || draft.VaiTro != old.VaiTro || draft.IdNguoiXuLy != old.IdNguoiXuLy
                        || draft.IdNhomXuLy != old.IdNhomXuLy)
                        throw new InvalidOperationException("已確認的步驟、處理人與順序必須保留。");
                }
                if (drafts[0].VaiTro != "NguoiXuLy" || drafts.Skip(1).Any(r => r.VaiTro == "NguoiXuLy"))
                    throw new InvalidOperationException("第一步必須是處理人，後續為確認步驟。");
                var userIds = drafts.Select(r => r.IdNguoiXuLy).Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();
                var groupIds = drafts.Where(r => r.IdNhomXuLy.HasValue).Select(r => r.IdNhomXuLy.Value).Distinct().ToList();
                if (context.dm_User.Count(r => userIds.Contains(r.Id)) != userIds.Count
                    || context.dm_Group.Count(r => groupIds.Contains(r.Id)) != groupIds.Count)
                    throw new InvalidOperationException("流程內有不存在的使用者或群組。");
                foreach (var draft in drafts)
                {
                    if (string.IsNullOrWhiteSpace(draft.TenBuoc) || draft.TenBuoc.Length > 100
                        || string.IsNullOrWhiteSpace(draft.VaiTro) || draft.VaiTro.Length > 20
                        || (string.IsNullOrWhiteSpace(draft.IdNguoiXuLy) == !draft.IdNhomXuLy.HasValue))
                        throw new InvalidOperationException("每一步須填寫名稱、角色，並選擇一位人員或一個群組。");
                    if (draft.Id != 0 && !steps.Any(r => r.Id == draft.Id))
                        throw new InvalidOperationException("步驟不屬於此課別流程，請重新載入。");
                }
                var kept = new HashSet<int>(drafts.Select(r => r.Id));
                context.dt318_TaskWorkflowSteps.RemoveRange(steps.Where(r => !kept.Contains(r.Id)));
                for (int i = completed.Count; i < drafts.Count; i++)
                {
                    var draft = drafts[i];
                    var step = draft.Id == 0 ? new dt318_TaskWorkflowSteps
                    {
                        IdTaskTeam = team.Id, NgayTao = DateTime.Now, NguoiTao = actor
                    } : steps.Single(r => r.Id == draft.Id);
                    if (draft.Id == 0) context.dt318_TaskWorkflowSteps.Add(step);
                    step.ThuTu = i + 1; step.TenBuoc = draft.TenBuoc.Trim(); step.VaiTro = draft.VaiTro;
                    step.IdNguoiXuLy = string.IsNullOrWhiteSpace(draft.IdNguoiXuLy) ? null : draft.IdNguoiXuLy;
                    step.IdNhomXuLy = draft.IdNhomXuLy;
                    step.TrangThai = i == completed.Count ? "DangCho" : "ChuaDen";
                }
                team.TrangThaiXuLy = completed.Count == drafts.Count ? "HoanThanh"
                    : completed.Count == 0 ? "DangXuLy" : "ChoXacNhan";
                Stamp(context, task, team, actor);
                context.SaveChanges();
                var saved = context.dt318_TaskWorkflowSteps.Where(r => r.IdTaskTeam == team.Id).OrderBy(r => r.ThuTu).ToList();
                History(context, task, team, actor, "LuuLuuTrinh", before, Describe(context, saved), "儲存流程；已確認步驟保持不變。");
                Overall(task, context.dt318_TaskTeams.Where(r => r.IdCongViec == taskId).ToList());
                return true;
            });
        }

        private static bool Assigned(DBDocumentManagementSystemEntities context, dt318_TaskWorkflowSteps step, string actor)
        {
            return step.IdNguoiXuLy == actor || (step.IdNhomXuLy.HasValue
                && context.dm_GroupUser.Any(r => r.IdGroup == step.IdNhomXuLy.Value && r.IdUser == actor));
        }

        public void CompleteStep(int taskId, string code, int stepId, string actor,
            byte[] teamVersion, byte[] stepVersion, string comment)
        {
            Write(context =>
            {
                RequireActor(context, actor);
                var task = Task(context, taskId);
                var team = Team(context, taskId, code);
                Version(team.RowVersion, teamVersion);
                if (Decision(task, code) != true) throw new InvalidOperationException("本課尚未確認參與。");
                var steps = context.dt318_TaskWorkflowSteps.Where(r => r.IdTaskTeam == team.Id).OrderBy(r => r.ThuTu).ToList();
                var current = steps.SingleOrDefault(r => r.TrangThai == "DangCho");
                if (current == null || current.Id != stepId)
                    throw new InvalidOperationException("目前步驟已改變或已完成，請重新載入。");
                Version(current.RowVersion, stepVersion);
                if (!Assigned(context, current, actor)) throw new InvalidOperationException("您不是目前步驟的指定處理人或群組成員。");
                if (steps.Any(r => r.ThuTu < current.ThuTu && r.TrangThai != "DaXacNhan"))
                    throw new InvalidOperationException("前面的步驟尚未完成。");
                if (comment != null && comment.Length > 2000) throw new InvalidOperationException("確認意見最多 2000 字。");
                current.TrangThai = "DaXacNhan";
                current.NguoiXacNhan = actor; current.NgayXacNhan = DateTime.Now; current.YKien = comment;
                if (current.ThuTu == 1)
                {
                    team.NguoiXuLyXong = actor; team.NgayXuLyXong = DateTime.Now;
                }
                var next = steps.FirstOrDefault(r => r.ThuTu > current.ThuTu);
                if (next != null) next.TrangThai = "DangCho";
                team.TrangThaiXuLy = next == null ? "HoanThanh" : "ChoXacNhan";
                Stamp(context, task, team, actor);
                History(context, task, team, actor, current.ThuTu == 1 ? "XuLyXong" : "XacNhanBuoc",
                    Task318WorkflowState.Caption("DangCho"), Task318WorkflowState.Caption("DaXacNhan"),
                    current.TenBuoc + (string.IsNullOrWhiteSpace(comment) ? "" : ": " + comment), current.Id);
                Overall(task, context.dt318_TaskTeams.Where(r => r.IdCongViec == taskId).ToList());
                return true;
            });
        }

        public Task318WorkflowSnapshot GetSnapshot(int taskId, string actor)
        {
            using (var context = new DBDocumentManagementSystemEntities())
            {
                RequireActor(context, actor);
                var task = Task(context, taskId);
                var teams = context.dt318_TaskTeams.Where(r => r.IdCongViec == taskId).OrderBy(r => r.MaTo).ToList();
                var ids = teams.Select(r => r.Id).ToList();
                var steps = context.dt318_TaskWorkflowSteps.Where(r => ids.Contains(r.IdTaskTeam)).OrderBy(r => r.ThuTu).ToList();
                var snapshots = teams.Select(team =>
                {
                    var teamSteps = steps.Where(r => r.IdTaskTeam == team.Id).ToList();
                    var active = teamSteps.SingleOrDefault(r => r.TrangThai == "DangCho");
                    return new Task318TeamSnapshot
                    {
                        Team = team, Decision = Decision(task, team.MaTo), Steps = teamSteps,
                        CanEdit = team.IdNguoiPhuTrach == actor && Decision(task, team.MaTo) == true && team.TrangThaiXuLy != "HoanThanh",
                        CanComplete = Decision(task, team.MaTo) == true && active != null && Assigned(context, active, actor)
                    };
                }).ToList();
                return new Task318WorkflowSnapshot
                {
                    Task = task, Teams = snapshots,
                    History = context.dt318_TaskHistory.Where(r => r.IdCongViec == taskId)
                        .OrderByDescending(r => r.NgayThucHien).ThenByDescending(r => r.Id).ToList(),
                    Users = (from user in context.dm_User
                             join job in context.dm_JobTitle on user.JobCode equals job.Id into jobs
                             from job in jobs.DefaultIfEmpty()
                             orderby user.Id
                             select new Task318UserOption
                             {
                                 Id = user.Id, Name = user.DisplayName, Department = user.IdDepartment,
                                 JobName = job == null ? "" : job.DisplayName
                             }).ToList(),
                    Groups = context.dm_Group.OrderBy(r => r.DisplayName).Select(r => new Task318GroupOption { Id = r.Id, Name = r.DisplayName }).ToList()
                };
            }
        }
    }
}
