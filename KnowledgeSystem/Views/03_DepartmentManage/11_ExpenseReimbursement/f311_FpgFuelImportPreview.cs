using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class f311_FpgFuelImportPreview : XtraForm
    {
        public f311_FpgFuelImportPreview() { InitializeComponent(); TraditionalGridLocalizer.Install(); }

        public void SetPreview(object rows, int updateCount, int skippedCount)
        {
            grid.DataSource = rows;
            summary.Text = string.Format("可更新：{0} 筆；略過：{1} 筆", updateCount, skippedCount);
            confirm.Caption = string.Format("確認匯入 ({0})", updateCount);
            confirm.Enabled = updateCount > 0;
        }

        private void view_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu != null) TraditionalGridLocalizer.StyleMenu(e.Menu);
        }

        private void confirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (confirm.Enabled) { DialogResult = DialogResult.OK; Close(); }
        }

        private void cancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
