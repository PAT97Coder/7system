using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class f401_DepartmentInfo : XtraForm
    {
        public string DeptId { get; set; }
        public EventFormInfo EventInfo { get; set; } = EventFormInfo.View;

        dm_Departments currentDept;

        private class ParentDeptLookup
        {
            public int IdChild { get; set; }
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string DisplayNameVN { get; set; }
            public string DisplayText { get; set; }
        }

        public f401_DepartmentInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
        }

        private void EnabledController(bool enable = true)
        {
            txbId.Enabled = enable;
            txbIdParent.Enabled = enable;
            txbDisplayName.Enabled = enable;
            txbDisplayNameVN.Enabled = enable;
            chkIsGroup.Enabled = enable;
            spnAuthorizedHeadcount.Enabled = enable;
            chkIsActive.Enabled = enable;
        }

        private void LockControl()
        {
            txbId.Enabled = false;
            txbIdChild.Enabled = false;
            txbIdParent.Enabled = false;
            chkIsGroup.Enabled = false;

            switch (EventInfo)
            {
                case EventFormInfo.Create:
                    Text = "\u65b0\u589e\u90e8\u9580";
                    btnConfirm.Visibility = BarItemVisibility.Always;
                    btnEdit.Visibility = BarItemVisibility.Never;
                    btnDelete.Visibility = BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = "\u66f4\u65b0\u90e8\u9580";
                    btnConfirm.Visibility = BarItemVisibility.Always;
                    btnEdit.Visibility = BarItemVisibility.Never;
                    btnDelete.Visibility = BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.View:
                default:
                    Text = "\u90e8\u9580\u4fe1\u606f";
                    btnConfirm.Visibility = BarItemVisibility.Never;
                    btnEdit.Visibility = BarItemVisibility.Always;
                    btnDelete.Visibility = BarItemVisibility.Always;
                    EnabledController(false);
                    break;
            }

            txbIdChild.Enabled = false;
            if (EventInfo != EventFormInfo.Create)
            {
                txbId.Enabled = false;
            }
        }

        private void f401_DepartmentInfo_Load(object sender, EventArgs e)
        {
            LoadParentDepartments();

            if (EventInfo == EventFormInfo.Create)
            {
                var nextIdChild = GenerateNextIdChild();
                currentDept = new dm_Departments { IdChild = nextIdChild, IsActive = true, IsGroup = false };
                txbIdChild.EditValue = nextIdChild;
                txbIdParent.EditValue = 0;
                chkIsActive.EditValue = true;
                chkIsGroup.EditValue = false;
            }
            else
            {
                LoadData();
            }

            LockControl();
        }

        private void LoadData()
        {
            currentDept = dm_DeptBUS.Instance.GetItemById(DeptId);
            if (currentDept == null)
            {
                XtraMessageBox.Show("\u627e\u4e0d\u5230\u90e8\u9580\u8cc7\u6599\u3002", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            txbId.EditValue = currentDept.Id;
            txbIdChild.EditValue = currentDept.IdChild;
            txbIdParent.EditValue = currentDept.IdParent;
            txbDisplayName.EditValue = currentDept.DisplayName;
            txbDisplayNameVN.EditValue = currentDept.DisplayNameVN;
            chkIsGroup.EditValue = currentDept.IsGroup ?? false;
            spnAuthorizedHeadcount.EditValue = currentDept.AuthorizedHeadcount;
            chkIsActive.EditValue = currentDept.IsActive;
        }

        private void btnEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            EventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (currentDept == null) return;

            if (!BindInputToDept()) return;

            if (EventInfo == EventFormInfo.Create)
            {
                if (dm_DeptBUS.Instance.GetItemById(currentDept.Id) != null)
                {
                    XtraMessageBox.Show("\u90e8\u9580\u4ee3\u865f\u5df2\u5b58\u5728\u3002", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var sameIdChild = dm_DeptBUS.Instance.GetList().Any(r => r.IdChild == currentDept.IdChild);
                if (sameIdChild)
                {
                    XtraMessageBox.Show("IdChild \u5df2\u5b58\u5728\u3002", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dm_DeptBUS.Instance.Add(currentDept);
            }
            else
            {
                dm_DeptBUS.Instance.AddOrUpdate(currentDept);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (currentDept == null) return;

            var confirm = XtraMessageBox.Show(
                string.Format("\u60a8\u78ba\u8a8d\u8981\u522a\u9664/\u505c\u7528\u90e8\u9580: {0}", currentDept.DisplayName),
                TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            currentDept.IsActive = false;
            dm_DeptBUS.Instance.AddOrUpdate(currentDept);
            DialogResult = DialogResult.OK;
            Close();
        }

        private int? GetHeadcountValue()
        {
            if (spnAuthorizedHeadcount.EditValue == null || spnAuthorizedHeadcount.EditValue == DBNull.Value)
            {
                return null;
            }

            var valueText = spnAuthorizedHeadcount.EditValue.ToString();
            if (string.IsNullOrWhiteSpace(valueText))
            {
                return null;
            }

            return Convert.ToInt32(spnAuthorizedHeadcount.Value);
        }

        private void LoadParentDepartments()
        {
            var parentOptions = dm_DeptBUS.Instance.GetList()
                .Where(r => r.IdChild.HasValue)
                .OrderBy(r => r.IdParent)
                .ThenBy(r => r.IdChild)
                .Select(r =>
                {
                    var displayName = string.IsNullOrWhiteSpace(r.DisplayName) ? r.Id : r.DisplayName;
                    return new ParentDeptLookup
                    {
                        IdChild = r.IdChild.Value,
                        Id = r.Id,
                        DisplayName = displayName,
                        DisplayNameVN = r.DisplayNameVN,
                        DisplayText = string.Format("{0} - {1} {2}", r.IdChild.Value, r.Id, displayName)
                    };
                })
                .ToList();

            txbIdParent.Properties.DataSource = parentOptions;
            txbIdParent.Properties.DisplayMember = nameof(ParentDeptLookup.DisplayText);
            txbIdParent.Properties.ValueMember = nameof(ParentDeptLookup.IdChild);

            gridLookUpEdit1View.PopulateColumns();
            if (gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayText)] != null)
            {
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayText)].Visible = false;
            }
            if (gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.IdChild)] != null)
            {
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.IdChild)].Caption = "IdChild";
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.IdChild)].VisibleIndex = 0;
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.IdChild)].Width = 70;
            }
            if (gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.Id)] != null)
            {
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.Id)].Caption = "\u90e8\u9580\u4ee3\u865f";
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.Id)].VisibleIndex = 1;
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.Id)].Width = 90;
            }
            if (gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayName)] != null)
            {
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayName)].Caption = "\u90e8\u9580\u540d\u7a31";
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayName)].VisibleIndex = 2;
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayName)].Width = 160;
            }
            if (gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayNameVN)] != null)
            {
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayNameVN)].Caption = "\u8d8a\u6587\u540d\u7a31";
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayNameVN)].VisibleIndex = 3;
                gridLookUpEdit1View.Columns[nameof(ParentDeptLookup.DisplayNameVN)].Width = 180;
            }
        }

        private bool BindInputToDept()
        {
            var id = txbId.Text?.Trim();
            var displayName = txbDisplayName.Text?.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(displayName))
            {
                XtraMessageBox.Show("\u8acb\u586b\u5beb\u90e8\u9580\u4ee3\u865f\u548c\u90e8\u9580\u540d\u7a31\u3002", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txbIdChild.Text?.Trim(), out int idChild))
            {
                idChild = GenerateNextIdChild();
                txbIdChild.EditValue = idChild;
            }

            if (txbIdParent.EditValue == null || txbIdParent.EditValue == DBNull.Value ||
                !int.TryParse(txbIdParent.EditValue.ToString(), out int idParent))
            {
                XtraMessageBox.Show("\u8acb\u9078\u64c7 IdParent\u3002", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            currentDept.Id = id;
            currentDept.IdChild = idChild;
            currentDept.IdParent = idParent;
            currentDept.DisplayName = displayName;
            currentDept.DisplayNameVN = txbDisplayNameVN.Text?.Trim();
            currentDept.IsGroup = chkIsGroup.Checked;
            currentDept.AuthorizedHeadcount = GetHeadcountValue();
            currentDept.IsActive = chkIsActive.Checked;
            return true;
        }

        private int GenerateNextIdChild()
        {
            var maxIdChild = dm_DeptBUS.Instance.GetList()
                .Where(r => r.IdChild.HasValue)
                .Select(r => r.IdChild.Value)
                .DefaultIfEmpty(0)
                .Max();

            return maxIdChild + 1;
        }
    }
}
