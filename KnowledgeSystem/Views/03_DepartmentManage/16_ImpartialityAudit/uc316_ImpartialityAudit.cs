using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._16_ImpartialityAudit
{
    public partial class uc316_ImpartialityAudit : XtraUserControl
    {
        // Lưu tạm đường dẫn PDF người dùng vừa chọn; chưa ghi vào CSDL ở giai đoạn này.
        public string PendingSopPdfPath { get; private set; }

        public uc316_ImpartialityAudit()
        {
            InitializeComponent();
            InitializeIcon();
        }

        // Khởi tạo biểu tượng cho thanh công cụ và hai chức năng SOP.
        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSop.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnViewSop.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnUpdateSop.ImageOptions.SvgImage = TPSvgimages.Num2;
        }

        private void btnViewSop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // TODO: Đọc file PDF SOP hiện tại từ CSDL và mở bằng trình xem PDF.
        }

        private void btnUpdateSop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Mở hộp thoại để người dùng chọn file PDF thay thế.
            using (var updateForm = new f316_UpdateSop())
            {
                if (updateForm.ShowDialog(this) != DialogResult.OK)
                    return;

                // Nhận file đã xác nhận để tầng CSDL sử dụng ở bước phát triển sau.
                PendingSopPdfPath = updateForm.SelectedPdfPath;

                // TODO: Ghi đè file SOP cũ trong CSDL bằng file tại PendingSopPdfPath.
            }
        }
    }
}
