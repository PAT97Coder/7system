using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class f401_OffboardingDocuments : XtraForm
    {
        private readonly EmployeeOffboardingService service = new EmployeeOffboardingService();
        private readonly string userId;
        private bool busy;
        private bool canView;
        private bool canUpload;
        private bool canDownload;
        private string actorId => TPConfigs.LoginUser.Id;

        public f401_OffboardingDocuments()
        {
            InitializeComponent();
            OffboardingGridLocalizer.Activate();
        }

        public f401_OffboardingDocuments(string employeeUserId) : this()
        {
            userId = employeeUserId;
        }

        private async void Form_Load(object sender, EventArgs e)
        {
            await Run(async () =>
            {
                var access = await Task.Run(() => new[]
                {
                    service.CanAccess(actorId, userId, EmployeeOffboardingService.ViewPermission),
                    service.CanAccess(actorId, userId, EmployeeOffboardingService.UploadPermission),
                    service.CanAccess(actorId, userId, EmployeeOffboardingService.DownloadPermission)
                });
                canView = access[0]; canUpload = access[1]; canDownload = access[2];
                if (!canView)
                {
                    XtraMessageBox.Show("您沒有操作此員工離職文件的權限。", "離職文件");
                    return;
                }
                var user = await Task.Run(() => dm_UserBUS.Instance.GetItemById(userId));
                employeeEdit.Text = OffboardingUi.EmployeeCaption(user);
                await LoadRecords();
            });
            if (!canView) Close();
        }

        private async Task Run(Func<Task> action)
        {
            if (busy) return;
            busy = true;
            UpdateCommands();
            try { await action(); }
            catch (Exception ex)
            {
                new Logger.TPLogger(typeof(f401_OffboardingDocuments).FullName).Error("離職文件", ex.ToString());
                string message = ex is OffboardingException
                    ? ex.Message : "作業失敗，請確認資料庫已更新且共用文件儲存位置可存取，再重新嘗試。";
                XtraMessageBox.Show(message, "離職文件", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                busy = false;
                if (!IsDisposed) UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            btnNew.Enabled = !busy && canUpload;
            btnRefresh.Enabled = !busy && canView;
            btnClose.Enabled = !busy;
            recordsGrid.Enabled = !busy;
            filesGrid.Enabled = !busy;
            UseWaitCursor = busy;
        }

        private async Task LoadRecords(int? focusId = null)
        {
            var current = recordsView.GetFocusedRow() as EmployeeOffboarding;
            int? id = focusId ?? current?.Id;
            var rows = await Task.Run(() => service.GetRecords(actorId, userId));
            recordsGrid.DataSource = rows;
            hintLabel.Text = rows.Count == 0 ? (canUpload ? "尚無離職文件紀錄。可按「新增附件」建立紀錄。" : "尚無離職文件紀錄。")
                : "共 " + rows.Count + " 筆紀錄。選取紀錄後，在附件上按右鍵可預覽或下載。";
            if (id.HasValue)
            {
                int handle = recordsView.LocateByValue("Id", id.Value);
                if (handle >= 0) recordsView.FocusedRowHandle = handle;
            }
            await LoadSelectedFiles();
        }

        private async Task LoadSelectedFiles()
        {
            var record = recordsView.GetFocusedRow() as EmployeeOffboarding;
            reasonEdit.Text = record?.Reason ?? "";
            filesGrid.DataSource = null;
            filesGrid.DataSource = record == null ? null
                : await Task.Run(() => service.GetFiles(actorId, userId, record.Id));
        }

        private async void RecordsView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (!busy) await Run(LoadSelectedFiles);
        }

        private async void New_Click(object sender, ItemClickEventArgs e)
        {
            if (!canUpload || busy) return;
            using (var dialog = new f401_OffboardingUpload(userId))
                if (dialog.ShowDialog(this) == DialogResult.OK) await Run(() => LoadRecords(dialog.CreatedRecordId));
        }

        private async void Refresh_Click(object sender, ItemClickEventArgs e) { await Run(() => LoadRecords()); }
        private void Close_Click(object sender, ItemClickEventArgs e) { Close(); }
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (busy && DialogResult != DialogResult.OK) e.Cancel = true;
        }

        private void FilesView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (busy || !e.HitInfo.InDataRow || e.Menu == null) return;
            filesView.FocusedRowHandle = e.HitInfo.RowHandle;
            var preview = new DXMenuItem("預覽", async (s, args) => await Preview(), TPSvgimages.View, DXMenuItemPriority.Normal)
                { BeginGroup = true, Enabled = canView && CanPreview(filesView.GetFocusedRow() as EmployeeOffboardingFile) };
            var download = new DXMenuItem("下載", async (s, args) => await Download(), TPSvgimages.Copy, DXMenuItemPriority.Normal)
                { Enabled = canDownload };
            foreach (var item in new[] { preview, download })
            {
                item.Appearance.Font = new Font("Microsoft JhengHei UI", 12F);
                item.Appearance.Options.UseFont = true;
                e.Menu.Items.Add(item);
            }
        }

        private static bool CanPreview(EmployeeOffboardingFile file)
        {
            return file != null && new[] { ".pdf", ".png", ".jpg", ".jpeg" }
                .Contains(Path.GetExtension(file.OriginalName).ToLowerInvariant());
        }

        private void FilesView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == fileSize && e.Value is long) e.DisplayText = OffboardingUi.FormatFileSize((long)e.Value);
        }

        private async Task Preview()
        {
            var file = filesView.GetFocusedRow() as EmployeeOffboardingFile;
            if (file == null || !canView || busy) return;
            if (!CanPreview(file))
            {
                XtraMessageBox.Show("此格式不支援預覽，請下載後開啟。", "離職文件");
                return;
            }
            await Run(async () =>
            {
                string folder = Path.Combine(Path.GetTempPath(), "7system-Offboarding-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, Path.GetFileName(file.OriginalName));
                try
                {
                    await Task.Run(() => service.CopyFile(actorId, userId, file.Id, path, false));
                    using (var viewer = new f00_VIewFile(path, false)) viewer.ShowDialog(this);
                }
                finally
                {
                    try { File.Delete(path); Directory.Delete(folder, false); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            });
        }

        private async Task Download()
        {
            var file = filesView.GetFocusedRow() as EmployeeOffboardingFile;
            if (file == null || !canDownload || busy) return;
            saveDialog.FileName = Path.GetFileName(file.OriginalName);
            {
                if (saveDialog.ShowDialog(this) != DialogResult.OK) return;
                await Run(async () =>
                {
                    await Task.Run(() => service.CopyFile(actorId, userId, file.Id, saveDialog.FileName, true));
                    XtraMessageBox.Show("附件下載完成。", "離職文件");
                });
            }
        }

    }
}
