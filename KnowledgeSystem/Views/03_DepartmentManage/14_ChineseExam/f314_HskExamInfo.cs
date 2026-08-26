using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._07_Quiz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    public partial class f314_HskExamInfo : XtraForm
    {
        private readonly BindingSource sourceUsers = new BindingSource();
        private readonly List<dm_User> users = new List<dm_User>();

        public f314_HskExamInfo()
        {
            InitializeComponent();
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnAddUsr.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRemoveUsr.ImageOptions.SvgImage = TPSvgimages.Remove;
        }

        private void f314_HskExamInfo_Load(object sender, EventArgs e)
        {
            txbExamType.Properties.Items.AddRange(new object[] { Hsk314ExamOptions.ExamTypeMock, Hsk314ExamOptions.ExamTypeOfficial, Hsk314ExamOptions.ExamTypeRetakeFirst, Hsk314ExamOptions.ExamTypeRetakeSecond });
            txbHskRatio.Properties.Items.AddRange(new object[] { "9:1", "8:2", "7:3", "6:4", "5:5", "4:6", "3:7", "2:8", "1:9" });
            txbExamType.EditValue = Hsk314ExamOptions.DefaultExamType;
            txbHskRatio.EditValue = Hsk314ExamOptions.DefaultRatioText;
            txbTime.EditValue = 60;
            txbPassScore.EditValue = 80;
            txbReading.EditValue = 50;
            gcData.DataSource = sourceUsers;
            LoadUsers();
        }

        private void LoadUsers()
        {
            var depts = dm_DeptBUS.Instance.GetList();
            var data = from usr in users
                       join dept in depts on usr.IdDepartment equals dept.Id into deptJoin
                       from dept in deptJoin.DefaultIfEmpty()
                       select new
                       {
                           usr,
                           UserName = $"{usr.Id}\r\n{usr.DisplayName}\r\n{usr.DisplayNameVN}",
                           DeptName = dept == null ? usr.IdDepartment : $"{dept.Id}\r\n{dept.DisplayName}"
                       };
            sourceUsers.DataSource = data.ToList();
            gvData.BestFitColumns();
        }

        private void btnAddUsr_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (f307_UsersData frm = new f307_UsersData())
            {
                frm.UsersInput = users;
                frm.ShowDialog();
                if (frm.UsersOutput != null) users.AddRange(frm.UsersOutput);
            }
            LoadUsers();
        }

        private void btnRemoveUsr_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (int row in gvData.GetSelectedRows())
            {
                dynamic data = gvData.GetRow(row);
                dm_User usr = data?.usr as dm_User;
                if (usr != null) users.Remove(usr);
            }
            LoadUsers();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbExamName.Text))
            {
                MsgTP.MsgError("請填寫考試名稱！");
                return;
            }
            if (users.Count == 0)
            {
                MsgTP.MsgError("請選擇考試人員！");
                return;
            }

            int hsk4Ratio;
            int hsk5Ratio;
            if (!Hsk314ExamOptions.TryParseRatio(txbHskRatio.Text, out hsk4Ratio, out hsk5Ratio))
            {
                MsgTP.MsgError("HSK4/5 ratio format is invalid. Example: 9:1");
                return;
            }

            int reading = Convert.ToInt32(txbReading.EditValue);
            Hsk314ExamOptions options = new Hsk314ExamOptions()
            {
                ExamType = txbExamType.Text.Trim(),
                RatioText = txbHskRatio.Text.Trim(),
                Hsk4Ratio = hsk4Ratio,
                Hsk5Ratio = hsk5Ratio
            };
            string validate = Hsk314ExamBuilder.ValidateBank(reading, options);
            if (!string.IsNullOrEmpty(validate))
            {
                MsgTP.MsgError(validate);
                return;
            }

            string code = $"HSK{DateTime.Now:yyyyMMddHHmmss}";
            var exam = new dt314_HskExamMgmt()
            {
                Code = code,
                DisplayName = txbExamName.Text.Trim(),
                CreateTime = DateTime.Now,
                TestDuration = Convert.ToInt32(txbTime.EditValue),
                PassingScore = Convert.ToInt32(txbPassScore.EditValue),
                ReadingCount = reading,
                WritingCount = 0,
                CreatedBy = TPConfigs.LoginUser?.Id,
                ExamType = options.ExamType,
                HskRatio = options.RatioText
            };

            int id = dt314_HskExamMgmtBUS.Instance.Add(exam);
            if (id <= 0)
            {
                MsgTP.MsgError("建立考試失敗！");
                return;
            }

            dt314_HskExamUserBUS.Instance.AddRange(users.Select(r => new dt314_HskExamUser()
            {
                ExamCode = code,
                IdUser = r.Id,
                IsPass = null
            }).ToList());

            Close();
        }
    }
}
