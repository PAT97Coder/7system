using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._20_AuditManagement
{
    public partial class uc320_mainjihua : XtraUserControl
    {
        private readonly BindingSource sourcePlans = new BindingSource();
        private readonly RefreshHelper helper;
        private readonly string[] detailNames = { "督導單位", "陪集合", "檔案" };
        private List<dt320_PlanDepartment> planDepartments = new List<dt320_PlanDepartment>();
        private List<dt320_PlanUser> planUsers = new List<dt320_PlanUser>();
        private List<dt320_Report> reports = new List<dt320_Report>();
        private List<dt320_ReportFile> reportFiles = new List<dt320_ReportFile>();
        private List<dm_User> users = new List<dm_User>();
        private List<dm_Departments> departments = new List<dm_Departments>();
        private List<dm_JobTitle> jobTitles = new List<dm_JobTitle>();
        private List<dm_Attachment> attachments = new List<dm_Attachment>();

        public uc320_mainjihua()
        {
            InitializeComponent();
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            helper = new RefreshHelper(gvData, "Id");
            btnAdd.ItemClick += btnAdd_ItemClick;
            btnReload.ItemClick += (s, e) => LoadData();
            Load += uc320_mainjihua_Load;
            gvData.MasterRowGetRelationCount += (s, e) => e.RelationCount = detailNames.Length;
            gvData.MasterRowGetRelationName += (s, e) => e.RelationName = detailNames[e.RelationIndex];
            gvData.MasterRowEmpty += (s, e) => e.IsEmpty = false;
            gvData.MasterRowGetChildList += gvData_MasterRowGetChildList;
        }

        private void uc320_mainjihua_Load(object sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            gvData.ReadOnlyGridView();
            gvSupervising.ReadOnlyGridView();
            gvAccompanying.ReadOnlyGridView();
            gvFiles.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = sourcePlans;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
                {
                    helper.SaveViewInfo();
                    // Nạp một lần qua BUS, không truy vấn CSDL lặp lại khi chuyển tab chi tiết.
                    var plans = dt320_PlanBUS.Instance.GetList();
                    var items = dt320_AuditItemBUS.Instance.GetList();
                    planDepartments = dt320_PlanDepartmentBUS.Instance.GetList();
                    planUsers = dt320_PlanUserBUS.Instance.GetList();
                    reports = dt320_ReportBUS.Instance.GetList();
                    reportFiles = dt320_ReportFileBUS.Instance.GetList();
                    users = dm_UserBUS.Instance.GetList();
                    departments = dm_DeptBUS.Instance.GetList();
                    jobTitles = dm_JobTitleBUS.Instance.GetList();
                    attachments = dm_AttachmentBUS.Instance.GetListByThread("320");

                    sourcePlans.DataSource = plans.OrderByDescending(p => p.AuditAt).Select(plan => new
                    {
                        plan.Id,
                        plan.AuditAt,
                        plan.DisplayName,
                        AuditItemName = items.FirstOrDefault(i => i.Id == plan.IdAuditItem)?.DisplayName ?? string.Empty,
                        CycleName = GetCycleName(plan.CycleType),
                        // Cấu trúc hiện tại chưa có cột liên kết file kế hoạch.
                        // Không lấy nhầm file báo cáo làm 計劃表 và không tự bổ sung bảng/cột.
                        PlanFileName = string.Empty
                    }).ToList();
                    helper.LoadViewInfo();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, "無法載入計劃資料：" + ex.Message, "稽核管理系統",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetCycleName(string cycle)
        {
            switch (cycle)
            {
                case "M": return "每月";
                case "H": return "每半年";
                case "Q": return "每季";
                case "Y": return "每年";
                default: return cycle;
            }
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var view = sender as GridView;
            int idPlan = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, "Id"));
            var participatingDepartments = planDepartments.Where(d => d.IdPlan == idPlan).ToList();

            if (e.RelationIndex < 2)
            {
                // Nhóm 1/2 lấy từ đơn vị đã lưu trong kế hoạch, không gán cứng 7810/7820.
                // Left join giữ lại dòng tham gia ngay cả khi danh mục nhân sự đã thay đổi.
                byte type = (byte)(e.RelationIndex + 1);
                e.ChildList = (from dept in participatingDepartments
                               where dept.ParticipationType == type
                               join mapping in planUsers on dept.Id equals mapping.IdPlanDepartment
                               let user = users.FirstOrDefault(u => u.Id == mapping.IdUser)
                               let department = departments.FirstOrDefault(d => d.Id == dept.IdDept)
                               let job = jobTitles.FirstOrDefault(j => j.Id == (user == null ? null : user.JobCode))
                               select new
                               {
                                   DepartmentName = dept.IdDept + " " + department?.DisplayName,
                                   UserId = mapping.IdUser,
                                   UserName = user?.DisplayName ?? string.Empty,
                                   JobTitle = job?.DisplayName ?? string.Empty
                               }).ToList();
            }
            else
            {
                // Tập hợp toàn bộ file báo cáo của các đơn vị thuộc kế hoạch.
                // Người và thời gian tải lấy từ từng ReportFile, không lấy từ báo cáo cha.
                e.ChildList = (from dept in participatingDepartments
                               join report in reports on dept.Id equals report.IdPlanDepartment
                               join file in reportFiles on report.Id equals file.IdReport
                               let attachment = attachments.FirstOrDefault(a => a.Id == file.IdAdt)
                               let uploader = users.FirstOrDefault(u => u.Id == file.CreateBy)
                               select new
                               {
                                   FileName = attachment?.ActualName ?? string.Empty,
                                   UploadedByName = uploader?.DisplayName ?? file.CreateBy,
                                   UploadedAt = file.CreateAt
                               }).ToList();
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = new f320_Add())
            {
                if (form.ShowDialog(this.FindForm()) == DialogResult.OK) LoadData();
            }
        }
    }
}
