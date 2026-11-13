using BusinessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class f401_OffboardingUpload : XtraForm
    {
        private readonly EmployeeOffboardingService service = new EmployeeOffboardingService();
        private readonly BindingList<PendingFile> pending = new BindingList<PendingFile>();
        private readonly string userId;
        private bool busy;
        private bool canUpload;
        private string ActorId => TPConfigs.LoginUser.Id;
        public int CreatedRecordId { get; private set; }

        public f401_OffboardingUpload()
        {
            InitializeComponent();
            OffboardingGridLocalizer.Activate();
            filesGrid.DataSource = pending;
            pending.ListChanged += (s, e) => UpdateCommands();
        }

        public f401_OffboardingUpload(string employeeUserId) : this() { userId = employeeUserId; }

        private async void Form_Load(object sender, EventArgs e)
        {
            await Run(async () =>
            {
                canUpload = await Task.Run(() => service.CanAccess(ActorId, userId, EmployeeOffboardingService.UploadPermission));
                if (!canUpload) throw new OffboardingException("您沒有上傳權限，或員工尚未辦理離職／預報離職。");
                var user = await Task.Run(() => dm_UserBUS.Instance.GetItemById(userId));
                employeeEdit.Text = OffboardingUi.EmployeeCaption(user);
                reasonEdit.Focus();
            });
            if (!canUpload) Close();
        }

        private void UpdateCommands()
        {
            btnSave.Enabled = !busy && canUpload && pending.Count > 0 && !string.IsNullOrWhiteSpace(reasonEdit.Text);
            btnAddFiles.Enabled = !busy && canUpload;
            btnClose.Enabled = !busy;
            reasonEdit.Properties.ReadOnly = busy || !canUpload;
            filesGrid.Enabled = !busy;
            hintLabel.Text = "已選擇 " + pending.Count + " 個附件。每個檔案上限 100 MB；右鍵可移除待上傳附件。";
            UseWaitCursor = busy;
        }

        private async Task Run(Func<Task> action)
        {
            if (busy) return;
            busy = true;
            UpdateCommands();
            try { await action(); }
            catch (Exception ex)
            {
                new Logger.TPLogger(typeof(f401_OffboardingUpload).FullName).Error("上傳離職文件", ex.ToString());
                XtraMessageBox.Show(ex is OffboardingException ? ex.Message
                    : "上傳失敗，請確認檔案及共用儲存位置可存取，再重新嘗試。", "離職文件", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { busy = false; if (!IsDisposed) UpdateCommands(); }
        }

        private async void AddFiles_Click(object sender, ItemClickEventArgs e)
        {
            if (!canUpload || busy) return;
            if (fileDialog.ShowDialog(this) != DialogResult.OK) return;
            await Run(() =>
            {
                foreach (string path in fileDialog.FileNames)
                {
                    string fullPath = Path.GetFullPath(path);
                    if (pending.Any(x => string.Equals(x.LocalPath, fullPath, StringComparison.OrdinalIgnoreCase))) continue;
                    var info = new FileInfo(fullPath);
                    if (!info.Exists || info.Length == 0 || info.Length > EmployeeOffboardingService.MaxFileSize || info.Name.Length > 256)
                        throw new OffboardingException("附件「" + info.Name + "」無法加入。檔案不可為空或超過 100 MB，檔名不得超過 256 個字。");
                    pending.Add(new PendingFile { OriginalName = info.Name, FileSize = info.Length, LocalPath = fullPath });
                }
                return Task.CompletedTask;
            });
        }

        private async void Save_Click(object sender, ItemClickEventArgs e)
        {
            if (!btnSave.Enabled || busy) return;
            string reason = reasonEdit.Text.Trim();
            string[] paths = pending.Select(x => x.LocalPath).ToArray();
            await Run(async () =>
            {
                CreatedRecordId = await Task.Run(() => service.Upload(ActorId, userId, reason, paths));
                DialogResult = DialogResult.OK;
                Close();
            });
        }

        private void Reason_EditValueChanged(object sender, EventArgs e) { UpdateCommands(); }
        private void Close_Click(object sender, ItemClickEventArgs e) { Close(); }
        private bool HasUnsavedChanges => pending.Count > 0 || !string.IsNullOrWhiteSpace(reasonEdit.Text);
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (busy && DialogResult != DialogResult.OK) { e.Cancel = true; return; }
            if (HasUnsavedChanges && DialogResult != DialogResult.OK)
                e.Cancel = XtraMessageBox.Show("內容尚未儲存，確定要關閉嗎？", "離職文件", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
        }

        private void FilesView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (busy || !e.HitInfo.InDataRow || e.Menu == null) return;
            filesView.FocusedRowHandle = e.HitInfo.RowHandle;
            var row = filesView.GetFocusedRow() as PendingFile;
            var remove = new DXMenuItem("移除待上傳附件", (s, args) => { if (row != null) pending.Remove(row); }, TPSvgimages.Remove, DXMenuItemPriority.Normal)
                { BeginGroup = true, Enabled = canUpload };
            remove.Appearance.Font = new Font("Microsoft JhengHei UI", 12F);
            remove.Appearance.Options.UseFont = true;
            e.Menu.Items.Add(remove);
        }

        private void FilesView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == fileSize && e.Value is long) e.DisplayText = OffboardingUi.FormatFileSize((long)e.Value);
        }

        private sealed class PendingFile
        {
            public string OriginalName { get; set; }
            public long FileSize { get; set; }
            public string LocalPath { get; set; }
        }
    }
}
