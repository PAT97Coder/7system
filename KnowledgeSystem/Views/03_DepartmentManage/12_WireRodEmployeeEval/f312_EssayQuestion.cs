using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_EssayQuestion : XtraForm
    {
        private readonly string userId;
        private readonly int groupId, questionId;
        private readonly bool canEdit;
        public f312_EssayQuestion(string userId, int groupId, dt312_Questions question = null, bool canEdit = false)
        {
            InitializeComponent(); this.userId = userId; this.groupId = groupId; this.canEdit = canEdit;
            questionId = question?.Id ?? 0;
            questionEdit.Text = question?.DisplayName ?? ""; referenceEdit.Text = question?.ReferenceAnswer ?? "";
            questionEdit.Properties.ReadOnly = referenceEdit.Properties.ReadOnly = !canEdit;
            btnSave.Enabled = canEdit;
            Text = questionId == 0 ? "新增申論題" : canEdit ? "編輯申論題" : "查看申論題";
        }
        private void Save_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!canEdit) return;
            try
            {
                dt312_QuestionsBUS.Instance.SaveEssay(userId, groupId, questionId, questionEdit.Text, referenceEdit.Text);
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { MsgTP.MsgError("儲存申論題失敗：\r\n" + ex.Message); }
        }
    }
}
