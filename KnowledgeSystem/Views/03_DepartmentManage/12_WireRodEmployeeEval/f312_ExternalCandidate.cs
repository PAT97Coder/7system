using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_ExternalCandidate : XtraForm
    {
        public f312_ExternalCandidate() { InitializeComponent(); }
        public string EmployeeCode { get; private set; }
        public string EmployeeName { get; private set; }
        public string Department { get; private set; }

        private void Candidate_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            try { btnStart.Enabled = dt312_QuestionsBUS.Instance.CanPractice(TPConfigs.LoginUser?.Id); }
            catch { btnStart.Enabled = false; MsgTP.MsgError("無法確認測驗權限，請稍後再試。"); }
        }

        private void Start_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => Start_Click(sender, EventArgs.Empty);
        private void Cancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter) { Start_Click(this, EventArgs.Empty); return true; }
            if (keyData == Keys.Escape) { Cancel_ItemClick(this, null); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (!btnStart.Enabled) return;
            try
            {
                EmployeeCode = dt312_PracticeBUS.RequiredText(codeEdit.Text, 64, "員工編號").ToUpperInvariant();
                EmployeeName = dt312_PracticeBUS.RequiredText(nameEdit.Text, 128, "姓名");
                Department = dt312_PracticeBUS.RequiredText(departmentEdit.Text, 128, "部門");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex) { MsgTP.MsgError(ex.Message); }
        }
    }
}
