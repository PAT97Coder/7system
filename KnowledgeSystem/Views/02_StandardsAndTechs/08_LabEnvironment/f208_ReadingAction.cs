using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public partial class f208_ReadingAction : XtraForm
    {
        private readonly string userId;
        private readonly long readingId;
        private bool saving;
        public f208_ReadingAction(string userId, long readingId)
        {
            InitializeComponent(); this.userId = userId; this.readingId = readingId;
            FormClosing += (s, e) => { if (saving) e.Cancel = true; };
        }
        private async void Save_Click(object sender, EventArgs e)
        {
            if (saving) return;
            string cause = txbCause.Text.Trim(), action = txbAction.Text.Trim(), result = txbResult.Text.Trim(); bool closed = chkClosed.Checked;
            if (cause.Length == 0 || action.Length == 0 || (closed && result.Length == 0))
            { MsgTP.MsgError("請填寫原因、處理措施；結案時須填寫處理結果。"); return; }
            saving = true; contentPanel.Enabled = btnSave.Enabled = btnCancel.Enabled = false;
            bool saved = false;
            try { await Task.Run(() => dt208_EnvironmentBUS.Instance.AddAction(userId, readingId, cause, action, result, closed)); saved = true; }
            catch (Exception ex) { MsgTP.MsgError("保存失敗：\r\n" + ex.Message); }
            finally { saving = false; contentPanel.Enabled = btnSave.Enabled = btnCancel.Enabled = true; }
            if (saved) { DialogResult = DialogResult.OK; Close(); }
        }
    }
}
