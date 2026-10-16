using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.IO;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._16_ImpartialityAudit
{
    public partial class f316_UpdateSop : XtraForm
    {
        // Đường dẫn PDF hợp lệ được trả về cho màn hình uc316 sau khi người dùng xác nhận.
        public string SelectedPdfPath { get; private set; }

        public f316_UpdateSop()
        {
            InitializeComponent();
            InitializeIcon();
        }

        // Khởi tạo biểu tượng theo bộ icon dùng chung của project.
        private void InitializeIcon()
        {
            btnSave.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;
        }

        // Mở hộp thoại Windows và chỉ cho phép chọn một file PDF đang tồn tại.
        private void txbPdfPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "選擇SOP檔案";
                dialog.Filter = "PDF Files (*.pdf)|*.pdf";
                dialog.FilterIndex = 1;
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                txbPdfPath.Text = dialog.FileName;
            }
        }

        // Kiểm tra lại đường dẫn trước khi cho phép chuyển sang bước cập nhật CSDL.
        private bool TryGetValidPdfPath(out string pdfPath)
        {
            pdfPath = txbPdfPath.Text.Trim();

            if (!File.Exists(pdfPath) ||
                !string.Equals(Path.GetExtension(pdfPath), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                XtraMessageBox.Show(
                    "Vui lòng chọn một file PDF hợp lệ.",
                    TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Xác nhận thao tác cập nhật; hiện tại chỉ trả đường dẫn về uc316, chưa ghi CSDL.
        private void btnSave_Click(object sender, EventArgs e)
        {
            string pdfPath;
            if (!TryGetValidPdfPath(out pdfPath))
                return;

            if (MsgTP.MsgYesNoQuestion("Bạn có chắc chắn cập nhật không?") != DialogResult.Yes)
                return;

            SelectedPdfPath = pdfPath;
            DialogResult = DialogResult.OK;
            Close();
        }

        // Đóng hộp thoại mà không thay đổi dữ liệu đang có.
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
