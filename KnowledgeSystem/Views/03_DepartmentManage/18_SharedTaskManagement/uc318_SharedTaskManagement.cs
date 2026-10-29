using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    public partial class uc318_SharedTaskManagement : XtraUserControl
    {
        private readonly BindingSource sourceTasks = new BindingSource();
        private DXMenuItem itemViewDescription;
        private DXMenuItem itemStartProcessing;
        private DXMenuItem itemWorkflow7810;
        private DXMenuItem itemWorkflow7820;

        public uc318_SharedTaskManagement()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeGrid();
            InitializeMenuItems();
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new Font(
                "Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSetting.ImageOptions.SvgImage = TPSvgimages.Gears;
        }

        private void InitializeGrid()
        {
            gvData.OptionsBehavior.Editable = false;
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gColDueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gColDueDate.DisplayFormat.FormatString = "yyyy-MM-dd";
            gColAssignee.Caption = "原收件人";
            gColDueDate.Width = 110;
            gColAssignee.Width = 120;
            gColSubject.Width = 300;
            gvData.OptionsView.ColumnAutoWidth = true;
            gvData.Bands.Clear();
            var info = gvData.Bands.AddBand("交辦事項（雙擊查看流程）");
            info.Columns.Add(gColDueDate); info.Columns.Add(gColSubject); info.Columns.Add(gColAssignee);
            AddColumn(info, "StatusText", "整體狀態", 130);
            var check = new RepositoryItemCheckEdit();
            gcData.RepositoryItems.Add(check);
            foreach (string code in new[] { "7810", "7820" })
            {
                var band = gvData.Bands.AddBand(code + "課");
                foreach (var column in new[] { AddColumn(band, "Shi" + code, "是", 45), AddColumn(band, "Fou" + code, "否", 45) })
                {
                    column.ColumnEdit = check; column.MinWidth = 40; column.OptionsColumn.FixedWidth = true;
                    column.ToolTip = "由本課負責人確認是否參與；兩欄皆空白表示尚未確認。";
                }
                AddColumn(band, "Progress" + code, "進度／目前處理人", 190);
            }
            gvData.Appearance.BandPanel.Font = new Font("Microsoft JhengHei UI", 14F);
            gvData.RowCellClick += DecisionClick;
            gvData.RowCellStyle += (s, e) =>
            {
                var row = gvData.GetRow(e.RowHandle) as Task318ListRow;
                if (row == null) return;
                if (IsDecision(e.Column.FieldName))
                {
                    var team = e.Column.FieldName.EndsWith("7810") ? row.Team7810 : row.Team7820;
                    if (team?.IdNguoiPhuTrach != TPConfigs.LoginUser.Id) e.Appearance.BackColor = Color.WhiteSmoke;
                }
                if ((e.Column.FieldName == "Progress7810" && row.CanXuLy7810 == null)
                    || (e.Column.FieldName == "Progress7820" && row.CanXuLy7820 == null))
                    e.Appearance.ForeColor = Color.DarkOrange;
                if ((e.Column.FieldName == "Progress7810" && row.Team7810?.TrangThaiXuLy == "HoanThanh")
                    || (e.Column.FieldName == "Progress7820" && row.Team7820?.TrangThaiXuLy == "HoanThanh"))
                    e.Appearance.ForeColor = Color.ForestGreen;
            };
            gvData.DoubleClick += (s, e) =>
            {
                var hit = gvData.CalcHitInfo(gcData.PointToClient(Control.MousePosition));
                if (!hit.InRowCell || hit.Column == null || IsDecision(hit.Column.FieldName)) return;
                OpenWorkflow(hit.Column.FieldName.EndsWith("7820") ? "7820"
                    : hit.Column.FieldName.EndsWith("7810") ? "7810" : null);
            };
        }

        private BandedGridColumn AddColumn(GridBand band, string field, string caption, int width)
        {
            var column = new BandedGridColumn { FieldName = field, Caption = caption, Visible = true, Width = width };
            gvData.Columns.Add(column); band.Columns.Add(column);
            return column;
        }

        private static bool IsDecision(string field) => field.StartsWith("Shi") || field.StartsWith("Fou");

        private void DecisionClick(object sender, RowCellClickEventArgs e)
        {
            if (e.RowHandle < 0 || e.Button != MouseButtons.Left || e.Clicks != 1 || !IsDecision(e.Column.FieldName)) return;
            var row = gvData.GetRow(e.RowHandle) as Task318ListRow;
            if (row == null) return;
            string teamCode = e.Column.FieldName.EndsWith("7810") ? "7810" : "7820";
            var team = teamCode == "7810" ? row.Team7810 : row.Team7820;
            bool decision = e.Column.FieldName.StartsWith("Shi");
            try
            {
                if (team == null) throw new InvalidOperationException("請先重新載入課別資料。");
                if (team.IdNguoiPhuTrach != TPConfigs.LoginUser.Id) throw new InvalidOperationException("只能確認自己負責的課別。");
                if ((teamCode == "7810" ? row.CanXuLy7810 : row.CanXuLy7820) == decision) return;
                dt318_TaskWorkflowBUS.Instance.ConfirmTeam(row.Id, teamCode, decision, TPConfigs.LoginUser.Id, team.RowVersion);
                LoadData(row.Id);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void uc318_SharedTaskManagement_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceTasks;
            ImportNewFolders();
            LoadData();
        }

        private void LoadData(int? focusedId = null)
        {
            try
            {
                using (SplashScreenManager.ShowOverlayForm(gcData))
                {
                    dt318_TaskWorkflowBUS.Instance.PrepareExistingTasks(Task318WorkflowSettings.Participants(), TPConfigs.LoginUser.Id);
                    sourceTasks.DataSource = dt318_TasksBUS.Instance.GetListRows();
                }
                if (focusedId.HasValue) gvData.FocusedRowHandle = gvData.LocateByValue("Id", focusedId.Value);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeMenuItems()
        {
            itemViewDescription = CreateMenuItem(
                "查看說明", (s, e) => ViewFocusedDescription(), TPSvgimages.View);
            itemStartProcessing = CreateMenuItem(
                "開始處理", (s, e) => OpenFocusedTaskFolder(), TPSvgimages.Start);
            itemWorkflow7810 = CreateMenuItem("7810 流程／確認", (s, e) => OpenWorkflow("7810"), TPSvgimages.View);
            itemWorkflow7820 = CreateMenuItem("7820 流程／確認", (s, e) => OpenWorkflow("7820"), TPSvgimages.View);
        }

        private static DXMenuItem CreateMenuItem(
            string caption,
            EventHandler clickEvent,
            SvgImage image)
        {
            var item = new DXMenuItem(caption, clickEvent, image, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            item.AppearanceHovered.ForeColor = Color.Blue;
            return item;
        }

        private Task318ListRow FocusedTask => gvData.GetFocusedRow() as Task318ListRow;

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (var form = new f318_TaskInfo())
                {
                    if (form.ShowDialog(this) != DialogResult.OK) return;

                    DateTime now = DateTime.Now;
                    var item = new dt318_Tasks
                    {
                        ChuDe = form.Draft.Subject,
                        GiaiThich = form.Draft.Description,
                        NgayDenHan = form.Draft.DueDate,
                        TrangThai = "ChoXuLy",
                        DuongDanThuMucChung = form.Draft.SharedRootPath,
                        DuongDanThuMucCongViec = form.Draft.TargetFolderPath,
                        TrangThaiOCR = "HoanThanh",
                        ThongBaoOCR = "Đã đọc trực tiếp nội dung XPS/OXPS.",
                        ThoiGianOCR = now,
                        NgayTao = now,
                        NguoiTao = TPConfigs.LoginUser.Id
                    };
                    int id = new Task318FolderImportService().ImportSelectedFiles(item, form.Draft.SourceOxpsPath,
                        form.Draft.Attachments.Select(r => r.SourcePath), form.Draft.RecipientId, Task318WorkflowSettings.Participants());
                    if (id <= 0)
                    {
                        XtraMessageBox.Show(id == 0 ? "此來源檔案已匯入。" : "新增交辦事項失敗。", TPConfigs.SoftNameTW,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    LoadData(id);
                }
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportNewFolders();
            LoadData();
        }

        private void btnSetting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (var form = new f318_Settings()) form.ShowDialog(this);
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ImportNewFolders()
        {
            try
            {
                var options = new Task318ImportOptions
                {
                    CommonRootPath = Task318WorkflowSettings.Required("Module318:CommonRoot"),
                    ImportedBy = TPConfigs.LoginUser.Id,
                    Participants = Task318WorkflowSettings.Participants()
                };
                string tuanUserId = Task318WorkflowSettings.Required("Module318:UserTuan");
                string hungUserId = Task318WorkflowSettings.Required("Module318:UserHung");
                options.Sources.Add(new Task318ImportSource
                {
                    RootPath = Task318WorkflowSettings.Required("Module318:SourceTuan"),
                    AssigneeId = tuanUserId,
                    FolderName = "Tuan"
                });
                options.Sources.Add(new Task318ImportSource
                {
                    RootPath = Task318WorkflowSettings.Required("Module318:SourceHung"),
                    AssigneeId = hungUserId,
                    FolderName = "Hung"
                });

                Task318ImportSummary result = new Task318FolderImportService().Import(options);
                if (result.FailedCount == 0) return;

                var failedItems = result.Items
                    .Where(item => !item.Imported && !item.Skipped && !item.Ready)
                    .ToList();
                int tuanFailedCount = failedItems.Count(item => item.AssigneeId == tuanUserId);
                int hungFailedCount = failedItems.Count(item => item.AssigneeId == hungUserId);
                var messages = new System.Collections.Generic.List<string>();
                if (tuanFailedCount > 0)
                    messages.Add($"Tuấn có {tuanFailedCount} YeQiaHan chưa đọc được");
                if (hungFailedCount > 0)
                    messages.Add($"Hùng có {hungFailedCount} YeQiaHan chưa đọc được");
                int unknownCount = failedItems.Count - tuanFailedCount - hungFailedCount;
                if (unknownCount > 0)
                    messages.Add($"Có {unknownCount} YeQiaHan chưa xác định người phụ trách chưa đọc được");

                XtraMessageBox.Show(
                    $"{string.Join("; ", messages)}. Vui lòng in hoặc kiểm tra lại file OXPS.",
                    TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row || e.HitInfo.RowHandle < 0) return;
            gvData.FocusedRowHandle = e.HitInfo.RowHandle;
            if (FocusedTask == null) return;
            e.Menu.Items.Add(itemViewDescription);
            e.Menu.Items.Add(itemStartProcessing);
            e.Menu.Items.Add(itemWorkflow7810);
            e.Menu.Items.Add(itemWorkflow7820);
        }

        private void OpenWorkflow(string team)
        {
            var row = FocusedTask;
            if (row == null) return;
            try
            {
                using (var form = new f318_Workflow(row.Id, team)) form.ShowDialog(this);
                LoadData(row.Id);
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ViewFocusedDescription()
        {
            if (FocusedTask == null) return;
            using (var form = new f318_Description(ToDraft(FocusedTask)))
                form.ShowDialog(this);
        }

        private void OpenFocusedTaskFolder()
        {
            var task = FocusedTask;
            if (task == null) return;

            if (string.IsNullOrWhiteSpace(task.TargetFolderPath)
                || !Directory.Exists(task.TargetFolderPath))
            {
                XtraMessageBox.Show("找不到此工作的共用資料夾，請確認網路磁碟已連線。",
                    TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(task.TargetFolderPath);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Task318Draft ToDraft(Task318ListRow row)
        {
            var draft = new Task318Draft
            {
                Id = row.Id,
                Status = row.Status,
                Subject = row.Subject,
                Description = row.Description,
                DueDate = row.DueDate,
                Assignee = row.Assignee,
                RecipientId = row.RecipientId, RecipientTeam = row.RecipientTeam,
                SharedRootPath = row.SharedRootPath,
                TargetFolderPath = row.TargetFolderPath,
                CreatedAt = row.CreatedAt,
                SourceOxpsPath = FindSourceDocument(row.TargetFolderPath)
            };

            return draft;
        }

        private static string FindSourceDocument(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return null;

            return Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories)
                .FirstOrDefault(path =>
                {
                    string extension = Path.GetExtension(path);
                    return string.Equals(extension, ".xps", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(extension, ".oxps", StringComparison.OrdinalIgnoreCase);
                });
        }
    }
}
