using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._15_InterviewAssessment
{
    public partial class f315_UsersData : XtraForm
    {
        public List<dm_User> UsersInput { get; set; } = new List<dm_User>();
        public List<dm_User> UsersOutput { get; set; }
        public bool IsFullUser { get; set; }
        public string DepartmentId { get; set; }

        public f315_UsersData()
        {
            InitializeComponent();
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f315_UsersData_Load(object sender, EventArgs e)
        {
            var excludedIds = new HashSet<string>((UsersInput ?? new List<dm_User>()).Select(item => item.Id));
            var departmentId = string.IsNullOrWhiteSpace(DepartmentId)
                ? "7"
                : DepartmentId;
            var departments = dm_DeptBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var users = dm_UserBUS.Instance.GetListByDept(departmentId)
                .Where(item => item.Status == 0
                    && !excludedIds.Contains(item.Id)
                    && item.ActualJobCode != null
                    && (IsFullUser || item.ActualJobCode.EndsWith("J")))
                .ToList();

            gcData.DataSource = (from user in users
                                 join job in jobs on user.ActualJobCode equals job.Id into jobJoin
                                 from job in jobJoin.DefaultIfEmpty()
                                 join department in departments on user.IdDepartment equals department.Id into departmentJoin
                                 from department in departmentJoin.DefaultIfEmpty()
                                 select new
                                 {
                                     usr = user,
                                     job,
                                     dept = department,
                                     DeptName = department == null ? user.IdDepartment : $"{department.Id}\r\n{department.DisplayName}",
                                     DisplayName = $"{user.DisplayName}\r\n{user.DisplayNameVN}"
                                 }).ToList();
            gvData.BestFitColumns();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UsersOutput = gvData.GetSelectedRows()
                .Select(row => ((dynamic)gvData.GetRow(row)).usr as dm_User)
                .Where(user => user != null)
                .ToList();
            Close();
        }
    }
}
