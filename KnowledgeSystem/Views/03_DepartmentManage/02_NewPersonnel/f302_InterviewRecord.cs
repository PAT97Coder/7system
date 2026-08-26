using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel
{
    public partial class f302_InterviewRecord : DevExpress.XtraEditors.XtraForm
    {
        public f302_InterviewRecord()
        {
            InitializeComponent();
        }

        string idDept2word;
        dm_User usrInterview;

        private void f302_InterviewRecord_Load(object sender, EventArgs e)
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;

            idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

            var users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            cbbBossLv2.Properties.DataSource = users;
            cbbBossLv2.Properties.DisplayMember = "DisplayName";
            cbbBossLv2.Properties.ValueMember = "Id";

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            cbbRecordNo.Properties.Items.AddRange(new[] { "第一次", "第二次", "第三次", "第四次", "第五次" });

            var evaluationComments = new AutoCompleteStringCollection();
            evaluationComments.AddRange(new string[] 
            { 
                " 團隊合作良好，樂於協助同事", 
                " 對工作負責，按時完成任務", 
                " 在工作中有所進步", 
                " 積極進取，願意學習", 
                " 工作細心，注意細節", 
                " 與同事溝通協作良好", 
                " 工作態度認真，對工作充滿承諾", 
                " 能夠獨立完成工作，並按時達成目標", 
                " 迅速有效地解決問題", 
                " 積極主動改進工作流程" 
            });

            txbRemark.Properties.UseAdvancedMode = DevExpress.Utils.DefaultBoolean.True;
            txbRemark.Properties.AdvancedModeOptions.AutoCompleteMode = TextEditAutoCompleteMode.SuggestAppend;
            txbRemark.Properties.AdvancedModeOptions.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txbRemark.Properties.AdvancedModeOptions.AutoCompleteCustomSource = evaluationComments;
        }

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            usrInterview = dm_UserBUS.Instance.GetItemById(txbUserId.EditValue?.ToString());
            if (usrInterview == null) return;

            txbUserNameTW.EditValue = usrInterview.DisplayName?.Trim();
            txbDept.EditValue = usrInterview.IdDepartment;
            cbbJobTitle.EditValue = usrInterview.ActualJobCode;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraMessageBox.Show("Chức năng đang lỗi!", TPConfigs.SoftNameTW);
        }

        private void cbbRecordNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usrInterview == null) return;
            txbDateRecord.EditValue = usrInterview.DateCreate.AddDays(14 * (cbbRecordNo.SelectedIndex + 1));
        }
    }
}
