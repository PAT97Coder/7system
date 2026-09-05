using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._16_ImpartialityAudit
{
    public partial class uc316_ImpartialityAudit : XtraUserControl
    {
        private const string PlanFileDept = "PLAN";
        private readonly BindingSource sourcePlans = new BindingSource();
        private readonly RefreshHelper helper;

        // Nạp một lần trong LoadData và dùng lại cho ba GridView như uc302.
        // Việc cache này tránh gọi CSDL lặp lại trong từng sự kiện master-detail.
        private List<dt316_Report> reports = new List<dt316_Report>();
        private List<dt316_PlanUser> planUsers = new List<dt316_PlanUser>();
        private List<dm_User> users = new List<dm_User>();
        private List<dm_Attachment> attachments = new List<dm_Attachment>();
        public string PendingSopPdfPath { get; private set; }

        private DXMenuItem itemViewFile;

        public uc316_ImpartialityAudit()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
            helper = new RefreshHelper(gvData, "Id");
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSop.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnViewSop.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnUpdateSop.ImageOptions.SvgImage = TPSvgimages.Num2;
        }

        // Tạo menu chuột phải theo cùng cách uc302_NewPersonnelMain đang sử dụng.
        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }

        private void InitializeMenuItems()
        {
            itemViewFile = CreateMenuItem("讀取檔案", ItemViewFile_Click, TPSvgimages.View);
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                // CSDL: các BUS dùng Entity Framework/LINQ to Entities và chỉ lấy
                // kế hoạch, tài liệu chưa bị xóa mềm (RemoveAt == null).
                var plans = dt316_PlanBUS.Instance.GetList();
                reports = dt316_ReportBUS.Instance.GetList();
                planUsers = dt316_PlanUserBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();

                // CSDL: chỉ lấy attachment thuộc mô-đun 316, tương tự Thread = "302" của uc302.
                attachments = dm_AttachmentBUS.Instance.GetListByThread("316");

                // LINQ: PDF kế hoạch dùng dòng Report kỹ thuật IdDept = PLAN và chỉ
                // hiển thị ở cột 計劃 của gvData, tách biệt report 7810/7820.
                sourcePlans.DataSource = plans.Select(plan =>
                {
                    var planReport = reports.FirstOrDefault(r =>
                        r.IdPlan == plan.Id && r.IdDept == PlanFileDept);
                    var attachment = planReport?.IdAdt == null
                        ? null
                        : attachments.FirstOrDefault(r => r.Id == planReport.IdAdt.Value);

                    return new
                    {
                        plan.Id,
                        plan.DisplayName,
                        PlanFileName = attachment?.ActualName ?? "尚未上傳"
                    };
                }).ToList();
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
                if (gvData.FocusedRowHandle >= 0)
                    gvData.ExpandMasterRow(gvData.FocusedRowHandle);
            }
        }

        private void uc316_ImpartialityAudit_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvReport.ReadOnlyGridView();
            gvReport.OptionsView.AllowCellMerge = true;

            LoadData();
            gcData.DataSource = sourcePlans;
            gvData.BestFitColumns();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont =
                new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Mở form thêm kế hoạch 316 được sao chép từ giao diện f306_NewSignDoc.
            using (var addForm = new f316_Add())
            {
                if (addForm.ShowDialog(this) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        // Master-detail cấp 1: dt316_Plan -> dt316_Report.
        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idPlan = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColId));

            e.IsEmpty = !planUsers.Any(r => r.IdPlan == idPlan);
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idPlan = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColId));

            // LINQ: mỗi người của kế hoạch là một dòng. Report và file được ghép
            // theo IdPlan + đơn vị để gvReport có dạng: 單位 | 查核人員 | 報告檔案.
            var planReports = reports.Where(r => r.IdPlan == idPlan).ToList();

            e.ChildList = (from planUser in planUsers
                           where planUser.IdPlan == idPlan
                           let idDept = GetAuditDept(planUser.IdDept)
                           let user = users.FirstOrDefault(r => r.Id == planUser.IdUser)
                           let report = planReports.FirstOrDefault(r => r.IdDept == idDept)
                           let attachment = report?.IdAdt == null
                               ? null
                               : attachments.FirstOrDefault(r => r.Id == report.IdAdt.Value)
                           orderby idDept, planUser.IdUser
                           select new
                           {
                               Id = report?.Id ?? 0,
                               IdDept = idDept,
                               UserName = user == null
                                   ? planUser.IdUser
                                   : $"{planUser.IdUser} {user.DisplayName}",
                               ActualName = attachment?.ActualName ?? "尚未上傳"
                           }).ToList();
        }

        private string GetAuditDept(string idDept)
        {
            if (idDept?.StartsWith("7810") == true) return "7810";
            if (idDept?.StartsWith("7820") == true) return "7820";
            return idDept ?? string.Empty;
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "報告進度";
        }

        private void gridView_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int relationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, relationIndex) as GridView;
            detailView?.BestFitColumns();
        }

        private void gridView_ExpandMasterRow(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view != null && view.FocusedRowHandle >= 0)
                view.ExpandMasterRow(view.FocusedRowHandle);
        }

        private void gvReport_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell) return;

            GridView view = sender as GridView;
            view.FocusedRowHandle = e.HitInfo.RowHandle;
            int idReport = Convert.ToInt32(view.GetRowCellValue(e.HitInfo.RowHandle, gColIdReport));

            var report = reports.FirstOrDefault(r => r.Id == idReport);
            if (report?.IdAdt != null && attachments.Any(r => r.Id == report.IdAdt.Value))
                e.Menu.Items.Add(itemViewFile);
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell) return;

            GridView view = sender as GridView;
            view.FocusedRowHandle = e.HitInfo.RowHandle;
            int idPlan = Convert.ToInt32(view.GetRowCellValue(e.HitInfo.RowHandle, gColId));

            if (GetPlanAttachmentId(idPlan).HasValue)
                e.Menu.Items.Add(itemViewFile);
        }

        private void gvReport_CellMerge(object sender, CellMergeEventArgs e)
        {
            if (e.Column.FieldName != "IdDept" && e.Column.FieldName != "ActualName")
            {
                e.Merge = false;
                e.Handled = true;
                return;
            }

            GridView view = sender as GridView;
            string dept1 = view.GetRowCellValue(e.RowHandle1, "IdDept")?.ToString();
            string dept2 = view.GetRowCellValue(e.RowHandle2, "IdDept")?.ToString();

            e.Merge = dept1 == dept2;
            if (e.Column.FieldName == "ActualName")
            {
                string file1 = view.GetRowCellValue(e.RowHandle1, "ActualName")?.ToString();
                string file2 = view.GetRowCellValue(e.RowHandle2, "ActualName")?.ToString();
                e.Merge = e.Merge && file1 == file2;
            }
            e.Handled = true;
        }

        private void gvReport_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || view.FocusedRowHandle < 0) return;

            int idReport = Convert.ToInt32(
                view.GetRowCellValue(view.FocusedRowHandle, gColIdReport));
            var report = reports.FirstOrDefault(r => r.Id == idReport);
            if (report?.IdAdt != null) OpenAttachment(report.IdAdt.Value);
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || view.FocusedRowHandle < 0) return;

            var hitInfo = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));
            if (hitInfo.InRowCell && hitInfo.Column == gridColumn1)
            {
                int idPlan = Convert.ToInt32(
                    view.GetRowCellValue(view.FocusedRowHandle, gColId));
                int? idAttachment = GetPlanAttachmentId(idPlan);
                if (idAttachment.HasValue) OpenAttachment(idAttachment.Value);
                return;
            }

            view.ExpandMasterRow(view.FocusedRowHandle);
        }

        private int? GetPlanAttachmentId(int idPlan)
        {
            return reports.FirstOrDefault(r =>
                r.IdPlan == idPlan && r.IdDept == PlanFileDept)?.IdAdt;
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            GridView focusedView = gcData.FocusedView as GridView;
            if (focusedView == null || focusedView.FocusedRowHandle < 0) return;

            int? idAttachment;
            if (focusedView.Columns.ColumnByFieldName("PlanFileName") != null)
            {
                int idPlan = Convert.ToInt32(
                    focusedView.GetRowCellValue(focusedView.FocusedRowHandle, gColId));
                idAttachment = GetPlanAttachmentId(idPlan);
            }
            else
            {
                int idReport = Convert.ToInt32(
                    focusedView.GetRowCellValue(focusedView.FocusedRowHandle, gColIdReport));
                idAttachment = reports.FirstOrDefault(r => r.Id == idReport)?.IdAdt;
            }

            if (idAttachment.HasValue) OpenAttachment(idAttachment.Value);
        }

        private void OpenAttachment(int idAttachment)
        {
            // CSDL: đọc metadata file từ dm_Attachment theo khóa chính.
            var attachment = dm_AttachmentBUS.Instance.GetItemById(idAttachment);
            if (attachment == null)
            {
                XtraMessageBox.Show("找不到附件。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sourcePath = Path.Combine(TPConfigs.Folder316, attachment.EncryptionName);
            if (!File.Exists(sourcePath))
            {
                XtraMessageBox.Show("附件檔案不存在。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            string destinationPath = Path.Combine(TPConfigs.TempFolderData,
                $"{DateTime.Now:yyMMddHHmmssfff} {attachment.ActualName}");

            File.Copy(sourcePath, destinationPath, true);
            new f00_VIewFile(destinationPath).ShowDialog();
        }

        private void btnViewSop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Schema 316 hiện tại chưa có trường lưu SOP.
            XtraMessageBox.Show("尚未設定SOP。", TPConfigs.SoftNameTW,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdateSop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var updateForm = new f316_UpdateSop())
            {
                if (updateForm.ShowDialog(this) != DialogResult.OK) return;
                PendingSopPdfPath = updateForm.SelectedPdfPath;
            }
        }
    }
}
