using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Configuration;
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
        }

        private void InitializeGrid()
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gColDueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gColDueDate.DisplayFormat.FormatString = "yyyy-MM-dd";
        }

        private void uc318_SharedTaskManagement_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceTasks;
            ImportNewFolders();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SplashScreenManager.ShowOverlayForm(gcData))
                    sourceTasks.DataSource = dt318_TasksBUS.Instance.GetListRows();
                gvData.BestFitColumns();
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
                if (dt318_TasksBUS.Instance.Add(item) <= 0)
                {
                    XtraMessageBox.Show("新增交辦事項失敗。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                LoadData();
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportNewFolders();
            LoadData();
        }

        private void ImportNewFolders()
        {
            try
            {
                var options = new Task318ImportOptions
                {
                    CommonRootPath = GetRequiredSetting("Module318:CommonRoot"),
                    ImportedBy = TPConfigs.LoginUser.Id
                };
                string tuanUserId = GetRequiredSetting("Module318:UserTuan");
                string hungUserId = GetRequiredSetting("Module318:UserHung");
                options.Sources.Add(new Task318ImportSource
                {
                    RootPath = GetRequiredSetting("Module318:SourceTuan"),
                    AssigneeId = tuanUserId,
                    FolderName = "Tuan"
                });
                options.Sources.Add(new Task318ImportSource
                {
                    RootPath = GetRequiredSetting("Module318:SourceHung"),
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

        private static string GetRequiredSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationErrorsException($"尚未設定 {key}。");
            return value.Trim();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row || FocusedTask == null) return;
            e.Menu.Items.Add(itemViewDescription);
            e.Menu.Items.Add(itemStartProcessing);
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
