using DevExpress.XtraEditors;
using System;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    public partial class f318_Description : XtraForm
    {
        public f318_Description(Task318Draft task)
        {
            InitializeComponent();
            txbSubject.Text = task?.Subject ?? string.Empty;
            memDescription.Text = task?.Description ?? string.Empty;
            lblDueDate.Text = task == null
                ? string.Empty
                : $"到期日：{task.DueDate:yyyy-MM-dd}    負責人：{task.Assignee}";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
