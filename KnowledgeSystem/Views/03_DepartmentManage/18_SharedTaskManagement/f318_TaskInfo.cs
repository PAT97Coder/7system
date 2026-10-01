using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
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
            btnPasteImage.ImageOptions.SvgImage = TPSvgimages.Copy;
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

            picSource.Properties.NullText = "請按 Ctrl+V 或點選「貼上圖片」";
            picSource.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            lblOcrStatus.Text = "辨識狀態：尚未辨識";
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
            if (task.SourceImage != null)
                picSource.Image = new Bitmap(task.SourceImage);

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

        private void btnPasteImage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PasteImageFromClipboard();
        }

        private void btnRecognize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (picSource.Image == null)
            {
                XtraMessageBox.Show("請先貼上工作截圖。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Giai đoạn giao diện: giữ điểm nối OCR tại đây, chưa gửi ảnh ra dịch vụ ngoài.
            lblOcrStatus.Text = "辨識狀態：介面已準備，尚未連接OCR服務";
            XtraMessageBox.Show(
                "圖片已接收。OCR服務將在下一階段接入；目前請手動確認主旨與說明。",
                TPConfigs.SoftNameTW,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            txbSubject.Focus();
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

            if (task.SourceImage != null)
                task.SourceImage.Dispose();
            task.SourceImage = picSource.Image == null ? null : new Bitmap(picSource.Image);
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
            if (picSource.Image == null)
            {
                XtraMessageBox.Show("請貼上包含主旨與說明的圖片。", TPConfigs.SoftNameTW,
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

        private void PasteImageFromClipboard()
        {
            if (!Clipboard.ContainsImage())
            {
                XtraMessageBox.Show("剪貼簿中沒有圖片。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var clipboardImage = Clipboard.GetImage();
            if (clipboardImage == null) return;
            picSource.Image = new Bitmap(clipboardImage);
            lblOcrStatus.Text = "辨識狀態：圖片已貼上，等待辨識";
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

            if (Clipboard.ContainsImage())
                PasteImageFromClipboard();
            else if (Clipboard.ContainsFileDropList())
                PasteAttachmentsFromClipboard();

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void picSource_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void picSource_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            string imagePath = files?.FirstOrDefault(path =>
                File.Exists(path) && IsImageFile(path));
            if (imagePath == null) return;

            using (var image = Image.FromFile(imagePath))
                picSource.Image = new Bitmap(image);
            lblOcrStatus.Text = "辨識狀態：圖片已載入，等待辨識";
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

        private static bool IsImageFile(string path)
        {
            string extension = Path.GetExtension(path);
            return new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" }
                .Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
