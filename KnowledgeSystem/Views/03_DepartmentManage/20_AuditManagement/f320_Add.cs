using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._20_AuditManagement
{
    public partial class f320_Add : XtraForm
    {
        public class ParticipantRow
        {
            public string IdUsr { get; set; }
            public string UserName { get; set; }
            public string JobName { get; set; }
            public string DepartmentName { get; set; }
        }

        public f320_Add()
        {
            InitializeComponent();
            dateAudit.EditValue = DateTime.Today;
            cbbAuditItem.EditValueChanged += (s, e) => ApplyItemCycle();
            txbAtt.ButtonClick += SelectPlanFile;
            Load += LoadParticipants;
        }

        public string PlanFilePath { get; private set; }

        private void ApplyItemCycle()
        {
            var item = cbbAuditItem.GetSelectedDataRow() as dt320_AuditItem;
            if (item == null) return;
            switch (item.CycleType)
            {
                case "M": cbbCycle.SelectedIndex = 0; break;
                case "H": cbbCycle.SelectedIndex = 1; break;
                case "Q": cbbCycle.SelectedIndex = 2; break;
                case "Y": cbbCycle.SelectedIndex = 3; break;
                default: cbbCycle.SelectedIndex = -1; break;
            }
        }

        private void SelectPlanFile(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            string path = null;
            if (e.Button == txbAtt.Properties.Buttons[1])
            {
                if (Clipboard.ContainsFileDropList())
                    path = Clipboard.GetFileDropList().Cast<string>().FirstOrDefault(System.IO.File.Exists);
            }
            else
            {
                using (var dialog = new OpenFileDialog { Filter = "所有檔案|*.*", CheckFileExists = true })
                    if (dialog.ShowDialog(this) == DialogResult.OK) path = dialog.FileName;
            }
            if (string.IsNullOrEmpty(path)) return;
            PlanFilePath = path;
            txbAtt.Text = System.IO.Path.GetFileName(path);
        }

        private void LoadParticipants(object sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            try
            {
                cbbAuditItem.Properties.DataSource = dt320_AuditItemBUS.Instance.GetActiveList();
                cbbAuditItem.Properties.ValueMember = "Id";
                cbbAuditItem.Properties.DisplayMember = "DisplayName";
                cbbAuditItem.Properties.Columns.Clear();
                cbbAuditItem.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "稽核項目"));
                var users = dm_UserBUS.Instance.GetListByDept("7")
                    .Where(u => u.Status == 0 && u.IdDepartment != null && u.IdDepartment.StartsWith("7"))
                    .ToList();
                var departments = dm_DeptBUS.Instance.GetList();
                var jobs = dm_JobTitleBUS.Instance.GetList();
                var choices = users.Select(u => new ParticipantRow
                {
                    IdUsr = u.Id,
                    UserName = u.DisplayName,
                    JobName = jobs.FirstOrDefault(j => j.Id == u.JobCode)?.DisplayName,
                    DepartmentName = u.IdDepartment + " " + departments.FirstOrDefault(d => d.Id == u.IdDepartment)?.DisplayName
                }).ToList();

                ConfigureParticipants(gcProgress, gvProgress, lookupUser, repositoryItemGridLookUpEdit1View, btnDelProg, choices);
                ConfigureParticipants(gcFiles, gvFiles, lookupUser7810, lookupUser7810View, btnDelFile, choices);
                ConfigureParticipants(gcRecipients, gvRecipients, lookupRecipient, lookupRecipientView, btnDeleteRecipient, choices);
                ConfigureParticipants(gcConfirmation, gvConfirmation, lookupConfirmation, lookupConfirmationView, btnDeleteConfirmation, choices);
                tabbedControlGroup1.SelectedTabPageIndex = 0;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, "無法載入人員資料：" + ex.Message, "稽核管理系統",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureParticipants(GridControl grid, GridView view,
            RepositoryItemGridLookUpEdit lookup, GridView popup,
            RepositoryItemButtonEdit deleteButton, System.Collections.Generic.List<ParticipantRow> choices)
        {
            var rows = new BindingList<ParticipantRow>();
            grid.DataSource = rows;
            lookup.DataSource = choices;
            lookup.ValueMember = "IdUsr";
            lookup.DisplayMember = "IdUsr";
            lookup.NullText = string.Empty;
            lookup.PopupFormMinSize = new System.Drawing.Size(600, 300);
            lookup.PopupFormSize = new System.Drawing.Size(System.Math.Max(600, grid.ClientSize.Width), 340);
            lookup.QueryPopUp += (s, e) =>
            {
                lookup.PopupFormSize = new System.Drawing.Size(System.Math.Max(600, grid.ClientSize.Width), 340);
            };
            popup.Columns.Clear();
            popup.Columns.AddVisible("DepartmentName", "部門").Width = 200;
            popup.Columns.AddVisible("IdUsr", "VNW").Width = 140;
            popup.Columns.AddVisible("UserName", "姓名").Width = 150;
            popup.Columns.AddVisible("JobName", "職務").Width = 220;
            popup.OptionsView.ColumnAutoWidth = false;
            popup.OptionsView.ShowAutoFilterRow = true;
            popup.OptionsView.ShowGroupPanel = false;

            var deptColumn = view.Columns.AddVisible("DepartmentName", "部門");
            deptColumn.VisibleIndex = 0;
            deptColumn.Width = 150;
            deptColumn.OptionsColumn.AllowEdit = false;
            view.Columns.ColumnByFieldName("IdUsr").VisibleIndex = 1;
            view.Columns.ColumnByFieldName("UserName").VisibleIndex = 2;
            view.Columns.ColumnByFieldName("JobName").VisibleIndex = 3;
            view.Columns.ColumnByFieldName("JobName").OptionsColumn.AllowEdit = false;
            view.Columns.ColumnByFieldName("UserName").OptionsColumn.AllowEdit = false;

            view.CellValueChanged += (s, e) =>
            {
                if (e.Column.FieldName != "IdUsr") return;
                var user = choices.FirstOrDefault(u => u.IdUsr == Convert.ToString(e.Value));
                view.SetRowCellValue(e.RowHandle, "DepartmentName", user?.DepartmentName);
                view.SetRowCellValue(e.RowHandle, "UserName", user?.UserName);
                view.SetRowCellValue(e.RowHandle, "JobName", user?.JobName);
            };
            view.ValidateRow += (s, e) =>
            {
                var row = e.Row as ParticipantRow;
                if (row == null) return;
                e.Valid = choices.Any(u => u.IdUsr == row.IdUsr)
                    && !rows.Any(r => !ReferenceEquals(r, row) && r.IdUsr == row.IdUsr);
                if (!e.Valid) e.ErrorText = "請選擇有效且不重複的工號。";
            };
            deleteButton.ButtonClick += (s, e) =>
            {
                var row = view.GetFocusedRow() as ParticipantRow;
                if (row != null) rows.Remove(row);
            };
        }
    }
}
