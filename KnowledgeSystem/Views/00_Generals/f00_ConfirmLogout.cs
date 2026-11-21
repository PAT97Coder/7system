using DataAccessLayer;
using DevExpress.XtraEditors;
using System;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_ConfirmLogout : XtraForm
    {
        public f00_ConfirmLogout()
        {
            InitializeComponent();
        }

        public f00_ConfirmLogout(dm_User user) : this()
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
            {
                identityLabel.Text = "尚未登入";
                btnLogout.Enabled = false;
                return;
            }

            string name = !string.IsNullOrWhiteSpace(user.DisplayName) ? user.DisplayName
                : !string.IsNullOrWhiteSpace(user.DisplayNameVN) ? user.DisplayNameVN : null;
            identityLabel.Text = string.IsNullOrWhiteSpace(name) ? user.Id : name + " · " + user.Id;
            btnLogout.Enabled = true;
        }

        private void ConfirmLogout_Shown(object sender, EventArgs e)
        {
            btnCancel.Focus();
        }
    }
}
