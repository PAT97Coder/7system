using BusinessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    /// <summary>Một lưu trình của một tổ, dùng bố cục form điện tử ký.</summary>
    public sealed partial class f318_Workflow : XtraForm
    {
        private readonly int taskId;
        private readonly string actor = TPConfigs.LoginUser.Id;
        private readonly string teamCode;
        private Task318WorkflowSnapshot snapshot;
        private Task318TeamSnapshot team;
        private BindingList<WorkflowRow> rows;
        private bool dirty;

        public sealed class WorkflowRow
        {
            public Task318StepDraft Draft { get; set; } = new Task318StepDraft { TrangThai = "ChuaDen" };
            public string IdUser
            {
                get => Draft.IdNguoiXuLy;
                set { Draft.IdNguoiXuLy = value; if (!string.IsNullOrWhiteSpace(value)) Draft.IdNhomXuLy = null; }
            }
            public string JobName { get; set; }
            public string UserName { get; set; }
            public string Permission { get; set; } = "確認";
            public string DeleteText => "";
        }

        public f318_Workflow(int taskId, string initialTeam)
        {
            this.taskId = taskId;
            snapshot = dt318_TaskWorkflowBUS.Instance.GetSnapshot(taskId, actor);
            teamCode = initialTeam
                ?? snapshot.Teams.FirstOrDefault(r => r.CanComplete)?.Team.MaTo
                ?? snapshot.Teams.FirstOrDefault(r => r.Team.IdNguoiPhuTrach == actor)?.Team.MaTo
                ?? snapshot.Teams.FirstOrDefault()?.Team.MaTo;
            InitializeComponent();
            InitializeWorkflow();
            BindWorkflow();
            FormClosing += (s, e) => { if (!DiscardChanges()) e.Cancel = true; };
        }

        private void InitializeWorkflow()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnDefaulProgress.ImageOptions.SvgImage = TPSvgimages.Progress;
            btnConfirm.ItemClick += (s, e) => Execute(SaveSelectedWorkflow);
            btnDefaulProgress.Click += (s, e) => Execute(() =>
            {
                PostRow();
                using (var selector = new f00_SelectFixedProg())
                {
                    selector.ShowDialog(this);
                    if (selector.Id > 0)
                        ApplyFixedProgress(teamCode, dm_FixedProgressBUS.Instance.GetItemById(selector.Id)?.Progress);
                }
            });
            btnComplete.Click += (s, e) => Execute(CompleteCurrentStep);
            gvProgress.CustomDrawRowIndicator += (s, e) => { if (e.RowHandle >= 0) e.Info.DisplayText = (e.RowHandle + 1).ToString(); };
            gvProgress.ShowingEditor += (s, e) =>
            {
                if ((gvProgress.GetFocusedRow() as WorkflowRow)?.Draft.TrangThai == "DaXacNhan") e.Cancel = true;
            };
            gvProgress.RowStyle += (s, e) =>
            {
                string state = (gvProgress.GetRow(e.RowHandle) as WorkflowRow)?.Draft.TrangThai;
                if (state == "DaXacNhan") e.Appearance.BackColor = Color.Honeydew;
                else if (state == "DangCho") e.Appearance.BackColor = Color.AliceBlue;
            };
            gvProgress.CustomRowCellEdit += (s, e) =>
            {
                if (e.Column == gColDelete && (!team.CanEdit || !(gvProgress.GetRow(e.RowHandle) is WorkflowRow row) || row.Draft.TrangThai == "DaXacNhan"))
                    e.RepositoryItem = lockedCell;
            };
            gvProgress.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column == gColUser && e.ListSourceRowIndex >= 0
                    && (gvProgress.GetRow(gvProgress.GetRowHandle(e.ListSourceRowIndex)) as WorkflowRow)?.Draft.IdNhomXuLy != null)
                    e.DisplayText = "群組";
            };
            gvProgress.CellValueChanged += (s, e) =>
            {
                if (!(gvProgress.GetRow(e.RowHandle) is WorkflowRow row)) return;
                if (e.Column == gColUser) FillPerson(row);
                dirty = true; gvProgress.RefreshRow(e.RowHandle);
            };
            btnDelProg.ButtonClick += (s, e) =>
            {
                if (!team.CanEdit || !(gvProgress.GetFocusedRow() is WorkflowRow row) || row.Draft.TrangThai == "DaXacNhan") return;
                rows.Remove(row); dirty = true;
            };
        }

        private void BindWorkflow()
        {
            team = snapshot.Teams.SingleOrDefault(r => r.Team.MaTo == teamCode)
                ?? throw new InvalidOperationException("找不到此課別的流程資料，請重新整理。");
            Text = "交辦事項流程 — " + teamCode;
            txbTeam.Text = teamCode;
            txbOwner.Text = UserName(team.Team.IdNguoiPhuTrach);
            txbTitle.Text = snapshot.Task.ChuDe;
            txbTitle.ToolTip = snapshot.Task.ChuDe;
            var data = team.Steps.Select(r => CreateRow(new Task318StepDraft
            {
                Id = r.Id, TenBuoc = r.TenBuoc, VaiTro = r.VaiTro,
                IdNguoiXuLy = r.IdNguoiXuLy, IdNhomXuLy = r.IdNhomXuLy, TrangThai = r.TrangThai
            })).ToList();
            if (data.Count == 0 && team.CanEdit)
                data.Add(CreateRow(new Task318StepDraft { IdNguoiXuLy = team.Team.IdNguoiPhuTrach, TrangThai = "ChuaDen" }));
            rows = new BindingList<WorkflowRow>(data);
            lookupUser.DataSource = snapshot.Users;
            gcProgress.DataSource = rows;
            gvProgress.OptionsBehavior.Editable = team.CanEdit;
            gvProgress.OptionsView.NewItemRowPosition = team.CanEdit
                ? DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top : DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
            btnConfirm.Enabled = team.CanEdit;
            btnDefaulProgress.Enabled = team.CanEdit;
            lcCurrentStep.Visibility = lcComment.Visibility = lcComplete.Visibility = team.CanComplete
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            var active = team.Steps.SingleOrDefault(r => r.TrangThai == "DangCho");
            lbCurrentStep.Text = active == null ? "" : "目前：" + (active.IdNguoiXuLy != null ? UserName(active.IdNguoiXuLy)
                : snapshot.Groups.FirstOrDefault(r => r.Id == active.IdNhomXuLy)?.Name) + "　" + active.TenBuoc;
            btnComplete.Text = active?.ThuTu == 1 ? "已處理完成" : "確認本步驟";
            txbComment.Text = "";
            dirty = false;
        }

        private string UserName(string id) => snapshot.Users.FirstOrDefault(r => r.Id == id)?.Name ?? id ?? "未指定";

        private WorkflowRow CreateRow(Task318StepDraft draft)
        {
            var row = new WorkflowRow { Draft = draft };
            FillPerson(row);
            return row;
        }

        private void FillPerson(WorkflowRow row)
        {
            var user = snapshot.Users.FirstOrDefault(r => r.Id == row.IdUser);
            row.JobName = row.Draft.IdNhomXuLy.HasValue ? "群組" : user?.JobName ?? "";
            row.UserName = row.Draft.IdNhomXuLy.HasValue
                ? snapshot.Groups.FirstOrDefault(r => r.Id == row.Draft.IdNhomXuLy)?.Name ?? "群組"
                : user?.Name ?? row.IdUser ?? "";
        }

        private void PostRow()
        {
            gvProgress.PostEditor();
            gvProgress.UpdateCurrentRow();
        }

        private bool DiscardChanges()
        {
            PostRow();
            return !dirty || XtraMessageBox.Show("流程變更尚未儲存，確定放棄變更？", TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void Execute(Action action)
        {
            try { UseWaitCursor = true; action(); }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            finally { UseWaitCursor = false; }
        }

        private void ApplyFixedProgress(string code, string serialized)
        {
            if (code != teamCode || !team.CanEdit) throw new InvalidOperationException("只有本課負責人可以設定流程。");
            PostRow();
            var steps = FixedProgressHelper.Deserialize(serialized);
            if (steps.Count == 0) throw new InvalidOperationException("固定流程沒有設定人員。");
            if (steps.Any(r => !snapshot.Users.Any(u => u.Id == r.IdUsr)))
                throw new InvalidOperationException("固定流程包含不存在的人員，請先修改固定流程。");
            var completed = rows.TakeWhile(r => r.Draft.TrangThai == "DaXacNhan").ToList();
            if (steps.Count < completed.Count || completed.Where((r, i) => r.IdUser != steps[i].IdUsr || r.Draft.IdNhomXuLy.HasValue).Any())
                throw new InvalidOperationException("已確認的流程必須保留原人員及順序；請選擇相符的固定流程。");
            var replacement = completed.Concat(steps.Skip(completed.Count).Select(r => CreateRow(
                new Task318StepDraft { IdNguoiXuLy = r.IdUsr, TrangThai = "ChuaDen" }))).ToList();
            rows.RaiseListChangedEvents = false;
            rows.Clear();
            foreach (var row in replacement) rows.Add(row);
            rows.RaiseListChangedEvents = true;
            rows.ResetBindings(); dirty = true;
        }

        private List<Task318StepDraft> BuildDrafts()
        {
            PostRow();
            return rows.Select((row, index) => new Task318StepDraft
            {
                Id = row.Draft.Id, TrangThai = row.Draft.TrangThai,
                IdNguoiXuLy = row.Draft.IdNguoiXuLy, IdNhomXuLy = row.Draft.IdNhomXuLy,
                TenBuoc = row.Draft.TrangThai == "DaXacNhan" ? row.Draft.TenBuoc : index == 0 ? "處理完成" : "確認",
                VaiTro = row.Draft.TrangThai == "DaXacNhan" ? row.Draft.VaiTro : index == 0 ? "NguoiXuLy" : "XacNhan"
            }).ToList();
        }

        private void SaveSelectedWorkflow()
        {
            if (!team.CanEdit) return;
            dt318_TaskWorkflowBUS.Instance.SaveWorkflow(taskId, teamCode, BuildDrafts(), actor, team.Team.RowVersion);
            dirty = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CompleteCurrentStep()
        {
            PostRow();
            if (dirty) throw new InvalidOperationException("請先按上方「確認」儲存流程變更。");
            var active = team.Steps.SingleOrDefault(r => r.TrangThai == "DangCho");
            if (!team.CanComplete || active == null) throw new InvalidOperationException("您不是目前步驟的指定處理人。");
            dt318_TaskWorkflowBUS.Instance.CompleteStep(taskId, teamCode, active.Id, actor,
                team.Team.RowVersion, active.RowVersion, txbComment.Text.Trim());
            snapshot = dt318_TaskWorkflowBUS.Instance.GetSnapshot(taskId, actor);
            BindWorkflow();
        }
    }
}
