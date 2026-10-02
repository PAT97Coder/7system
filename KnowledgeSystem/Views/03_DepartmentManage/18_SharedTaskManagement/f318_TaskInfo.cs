using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    public partial class f318_TaskInfo : XtraForm
    {
        private readonly Task318Draft editingTask;
        private readonly BindingList<Task318Attachment> attachments =
            new BindingList<Task318Attachment>();
        private string sourceOxpsPath;

        public Task318Draft Draft { get; private set; }

        public f318_TaskInfo() : this(null)
        {
        }

        public f318_TaskInfo(Task318Draft task)
        {
            editingTask = task;
            InitializeComponent();
            InitializeIcon();
            InitializeData();
            LoadTask(task);
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnSelectOxps.ImageOptions.SvgImage = TPSvgimages.Copy;
            btnRecognize.ImageOptions.SvgImage = TPSvgimages.Bot;
            btnPasteAttachment.ImageOptions.SvgImage = TPSvgimages.Attach;
            btnAddAttachment.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRemoveAttachment.ImageOptions.SvgImage = TPSvgimages.Remove;
            txbSharedRoot.Properties.Buttons[0].ImageOptions.SvgImage = TPSvgimages.Search;
        }

        private void InitializeData()
        {
            cbAssignee.Properties.Items.AddRange(new object[]
            {
                "共同處理",
                "本人",
                "另一位同仁"
            });
            cbAssignee.SelectedIndex = 0;

            dtDueDate.DateTime = DateTime.Today.AddDays(7);
            gcAttachments.DataSource = attachments;
            gvAttachments.OptionsView.ShowGroupPanel = false;
            gvAttachments.OptionsView.ShowAutoFilterRow = true;
            gvAttachments.OptionsBehavior.Editable = false;
            gvAttachments.Columns.Clear();
            gvAttachments.Columns.AddVisible(nameof(Task318Attachment.FileName), "附件名稱").Width = 360;
            gvAttachments.Columns.AddVisible(nameof(Task318Attachment.Extension), "類型").Width = 90;
            gvAttachments.Columns.AddVisible(nameof(Task318Attachment.SizeText), "大小").Width = 100;
            gvAttachments.Columns.AddVisible(nameof(Task318Attachment.SourcePath), "來源路徑").Width = 450;

            lblReadStatus.Text = "讀取狀態：尚未讀取";
        }

        private void LoadTask(Task318Draft task)
        {
            if (task == null)
            {
                Text = "新增交辦事項";
                UpdateTargetFolderPreview();
                return;
            }

            Text = "編輯交辦事項";
            txbSubject.Text = task.Subject;
            memDescription.Text = task.Description;
            dtDueDate.DateTime = task.DueDate;
            cbAssignee.Text = task.Assignee;
            txbSharedRoot.Text = task.SharedRootPath;
            if (!string.IsNullOrWhiteSpace(task.SourceOxpsPath))
                LoadOxpsFile(task.SourceOxpsPath, false);

            foreach (var attachment in task.Attachments)
                attachments.Add(new Task318Attachment
                {
                    FileName = attachment.FileName,
                    Extension = attachment.Extension,
                    SizeText = attachment.SizeText,
                    SourcePath = attachment.SourcePath
                });

            UpdateTargetFolderPreview();
        }

        private void btnSelectOxps_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SelectOxpsFile();
        }

        private void btnRecognize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sourceOxpsPath))
                SelectOxpsFile();
            else
                ReadOxpsContent();
        }

        private void btnPasteAttachment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PasteAttachmentsFromClipboard();
        }

        private void btnAddAttachment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "選擇附件",
                Filter = "所有檔案 (*.*)|*.*",
                Multiselect = true,
                CheckFileExists = true
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    AddAttachmentFiles(dialog.FileNames);
            }
        }

        private void btnRemoveAttachment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var item = gvAttachments.GetFocusedRow() as Task318Attachment;
            if (item != null) attachments.Remove(item);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!ValidateData()) return;

            var task = editingTask ?? new Task318Draft();
            task.Subject = txbSubject.Text.Trim();
            task.Description = memDescription.Text.Trim();
            task.DueDate = dtDueDate.DateTime.Date;
            task.Assignee = cbAssignee.Text.Trim();
            task.SharedRootPath = txbSharedRoot.Text.Trim();
            task.TargetFolderPath = txbTargetFolder.Text.Trim();

            task.SourceOxpsPath = sourceOxpsPath;
            task.ReplaceAttachments(attachments.Select(item => new Task318Attachment
            {
                FileName = item.FileName,
                Extension = item.Extension,
                SizeText = item.SizeText,
                SourcePath = item.SourcePath
            }));

            Draft = task;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(sourceOxpsPath) || !File.Exists(sourceOxpsPath))
            {
                XtraMessageBox.Show("請選擇包含主旨與說明的 OXPS 檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txbSubject.Text) ||
                string.IsNullOrWhiteSpace(memDescription.Text))
            {
                XtraMessageBox.Show("請確認主旨與說明。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtDueDate.DateTime.Date < DateTime.Today)
            {
                XtraMessageBox.Show("到期日不可早於今天。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txbSharedRoot.Text))
            {
                XtraMessageBox.Show("請選擇共用根目錄。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SelectOxpsFile()
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "選擇 OXPS 檔案",
                Filter = "OXPS File (*.oxps)|*.oxps",
                CheckFileExists = true,
                Multiselect = false
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                LoadOxpsFile(dialog.FileName, true);
            }
        }

        private void PasteOxpsFromClipboard()
        {
            if (!Clipboard.ContainsFileDropList())
            {
                XtraMessageBox.Show("剪貼簿中沒有 OXPS 檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = Clipboard.GetFileDropList()
                .Cast<string>()
                .FirstOrDefault(IsOxpsFile);
            if (path == null)
            {
                XtraMessageBox.Show("剪貼簿中沒有 OXPS 檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LoadOxpsFile(path, true);
        }

        private void LoadOxpsFile(string path, bool readContent)
        {
            if (!IsOxpsFile(path))
            {
                XtraMessageBox.Show("只支援 OXPS 檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sourceOxpsPath = Path.GetFullPath(path);
            txbOxpsSource.Text = sourceOxpsPath;

            if (readContent)
                ReadOxpsContent();
            else
                lblReadStatus.Text = "讀取狀態：已載入 OXPS";
        }

        private void ReadOxpsContent()
        {
            if (string.IsNullOrWhiteSpace(sourceOxpsPath) || !File.Exists(sourceOxpsPath))
            {
                XtraMessageBox.Show("請先選擇 OXPS 檔案。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                UseWaitCursor = true;
                lblReadStatus.Text = "讀取狀態：正在讀取 OXPS...";
                var result = OxpsParser.ReadSubjectAndDescription(sourceOxpsPath);

                txbSubject.Text = result.Subject;
                memDescription.Text = result.Description;
                if (result.DueDate.HasValue)
                    dtDueDate.DateTime = result.DueDate.Value;
                lblReadStatus.Text = "讀取狀態：讀取完成";
                txbSubject.Focus();
            }
            catch (Exception ex)
            {
                lblReadStatus.Text = "讀取狀態：讀取失敗";
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void PasteAttachmentsFromClipboard()
        {
            if (!Clipboard.ContainsFileDropList())
            {
                XtraMessageBox.Show("請先在檔案總管複製附件。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AddAttachmentFiles(Clipboard.GetFileDropList().Cast<string>());
        }

        private void AddAttachmentFiles(System.Collections.Generic.IEnumerable<string> paths)
        {
            foreach (string path in paths.Where(File.Exists))
            {
                if (attachments.Any(item => string.Equals(
                    item.SourcePath, path, StringComparison.OrdinalIgnoreCase))) continue;

                var file = new FileInfo(path);
                attachments.Add(new Task318Attachment
                {
                    FileName = file.Name,
                    Extension = file.Extension.TrimStart('.').ToUpperInvariant(),
                    SizeText = FormatFileSize(file.Length),
                    SourcePath = file.FullName
                });
            }
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes >= 1024L * 1024L)
                return $"{bytes / 1024d / 1024d:0.##} MB";
            if (bytes >= 1024L)
                return $"{bytes / 1024d:0.##} KB";
            return $"{bytes} B";
        }

        private void txbSharedRoot_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog
            {
                Description = "選擇兩位使用者都能存取的共用根目錄",
                SelectedPath = Directory.Exists(txbSharedRoot.Text)
                    ? txbSharedRoot.Text
                    : string.Empty
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                txbSharedRoot.Text = dialog.SelectedPath;
            }
        }

        private void FolderPreview_EditValueChanged(object sender, EventArgs e)
        {
            UpdateTargetFolderPreview();
        }

        private void UpdateTargetFolderPreview()
        {
            string root = txbSharedRoot.Text.Trim();
            DateTime dueDate = dtDueDate.DateTime == DateTime.MinValue
                ? DateTime.Today
                : dtDueDate.DateTime;

            if (string.IsNullOrWhiteSpace(root))
            {
                txbTargetFolder.Text = string.Empty;
                return;
            }

            txbTargetFolder.Text = Path.Combine(
                root,
                $"Tháng {dueDate.Month}{dueDate.Year}",
                dueDate.ToString("MMdd"));
        }

        private void f318_TaskInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.V) return;

            if (Clipboard.ContainsFileDropList())
            {
                var paths = Clipboard.GetFileDropList().Cast<string>().ToList();
                if (paths.Any(IsOxpsFile))
                    PasteOxpsFromClipboard();
                else
                    AddAttachmentFiles(paths);
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void txbOxpsSource_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            SelectOxpsFile();
        }

        private void txbOxpsSource_DragEnter(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Any(IsOxpsFile))
                e.Effect = DragDropEffects.Copy;
        }

        private void txbOxpsSource_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            string oxpsPath = files?.FirstOrDefault(IsOxpsFile);
            if (oxpsPath != null) LoadOxpsFile(oxpsPath, true);
        }

        private void gcAttachments_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void gcAttachments_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null) AddAttachmentFiles(files);
        }

        private static bool IsOxpsFile(string path)
        {
            return File.Exists(path)
                && string.Equals(Path.GetExtension(path), ".oxps", StringComparison.OrdinalIgnoreCase);
        }
    }
}
