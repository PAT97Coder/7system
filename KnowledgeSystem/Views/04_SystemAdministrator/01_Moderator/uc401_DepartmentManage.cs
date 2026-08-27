using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data.Mask;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class uc401_DepartmentManage : XtraUserControl
    {
        readonly BindingSource sourceDepartments = new BindingSource();

        DXMenuItem itemViewInfo;
        DXMenuItem itemEditHeadcount;
        DXMenuItem itemToggleActive;

        public uc401_DepartmentManage()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new Font("Microsoft JhengHei UI", 12F);
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("\u67e5\u770b\u8cc7\u8a0a", ItemViewInfo_Click, TPSvgimages.View);
            itemEditHeadcount = CreateMenuItem("\u4fee\u6539\u7de8\u5236", ItemEditHeadcount_Click, TPSvgimages.Edit);
            itemToggleActive = CreateMenuItem("\u505c\u7528", ItemToggleActive_Click, TPSvgimages.Disable);
        }

        private void LoadData()
        {
            var data = dm_DeptBUS.Instance.GetList()
                .OrderBy(r => r.IdParent)
                .ThenBy(r => r.IdChild)
                .Select(r => new DepartmentManageRow
                {
                    Id = r.Id,
                    IdChild = r.IdChild,
                    IdParent = r.IdParent,
                    DisplayName = r.DisplayName,
                    DisplayNameVN = r.DisplayNameVN,
                    IsGroup = r.IsGroup,
                    AuthorizedHeadcount = r.AuthorizedHeadcount,
                    IsActive = r.IsActive
                })
                .ToList();

            sourceDepartments.DataSource = data;
            treeData.DataSource = sourceDepartments;
            treeData.KeyFieldName = nameof(DepartmentManageRow.IdChild);
            treeData.ParentFieldName = nameof(DepartmentManageRow.IdParent);
            treeData.ExpandToLevel(1);
            treeData.BestFitColumns();
        }

        private void uc401_DepartmentManage_Load(object sender, EventArgs e)
        {
            treeData.OptionsSelection.SelectNodesOnRightClick = true;
            treeData.KeyDown += GridControlHelper.TreeViewCopyCellData_KeyDown;
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = new f401_DepartmentInfo { EventInfo = EventFormInfo.Create })
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private DepartmentManageRow GetFocusedRow()
        {
            return treeData.GetDataRecordByNode(treeData.FocusedNode) as DepartmentManageRow;
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null) return;

            using (var form = new f401_DepartmentInfo { DeptId = row.Id })
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void ItemEditHeadcount_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null) return;

            var editor = new TextEdit
            {
                Font = new Font("Microsoft JhengHei UI", 14F),
                EditValue = row.AuthorizedHeadcount?.ToString() ?? string.Empty
            };
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            editor.Properties.MaskSettings.Set("mask", "d");
            editor.Properties.UseMaskAsDisplayFormat = true;

            var args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "\u8acb\u8f38\u5165\u7de8\u5236",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = editor.EditValue
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            int? headcount = null;
            var input = result.ToString().Trim();
            if (!string.IsNullOrEmpty(input))
            {
                if (!int.TryParse(input, out int value) || value < 0)
                {
                    XtraMessageBox.Show("\u7de8\u5236\u5fc5\u9808\u662f\u975e\u8ca0\u6574\u6578\u3002", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                headcount = value;
            }

            var dept = dm_DeptBUS.Instance.GetItemById(row.Id);
            if (dept == null) return;

            dept.AuthorizedHeadcount = headcount;
            dm_DeptBUS.Instance.AddOrUpdate(dept);
            LoadData();
        }

        private void ItemToggleActive_Click(object sender, EventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null) return;

            var nextActive = !row.IsActive;
            var actionText = nextActive ? "\u555f\u7528" : "\u505c\u7528";
            var confirm = XtraMessageBox.Show(
                string.Format("\u78ba\u5b9a\u8981{0}\u90e8\u9580 [{1}] \u55ce\uff1f", actionText, row.Id),
                TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            var dept = dm_DeptBUS.Instance.GetItemById(row.Id);
            if (dept == null) return;

            dept.IsActive = nextActive;
            dm_DeptBUS.Instance.AddOrUpdate(dept);
            LoadData();
        }

        private void treeData_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            var row = treeData.GetDataRecordByNode(e.Node) as DepartmentManageRow;
            if (row != null && !row.IsActive)
            {
                e.Appearance.ForeColor = Color.Gray;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
            }
        }

        private void treeData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            foreach (var item in e.Menu.Items.ToList())
            {
                if (item.Caption == "Expand")
                {
                    item.Caption = "\u5c55\u958b";
                }
                else if (item.Caption == "Collapse")
                {
                    item.Caption = "\u6536\u5408";
                }
                else if (item.Caption == "Full Expand")
                {
                    item.Caption = "\u5168\u90e8\u5c55\u958b";
                }
                else if (item.Caption == "Full Collapse")
                {
                    item.Caption = "\u5168\u90e8\u6536\u5408";
                }
            }

            if (e.HitInfo?.InRowCell != true || e.HitInfo.Node == null) return;

            treeData.FocusedNode = e.HitInfo.Node;
            var row = GetFocusedRow();
            if (row == null) return;

            itemToggleActive.Caption = row.IsActive ? "\u505c\u7528" : "\u555f\u7528";
            itemToggleActive.ImageOptions.SvgImage = row.IsActive ? TPSvgimages.Disable : TPSvgimages.Confirm;
            itemViewInfo.BeginGroup = true;
            itemEditHeadcount.BeginGroup = false;
            itemToggleActive.BeginGroup = false;

            e.Menu.Items.Add(itemViewInfo);
            e.Menu.Items.Add(itemEditHeadcount);
            e.Menu.Items.Add(itemToggleActive);
        }

        private class DepartmentManageRow
        {
            public string Id { get; set; }
            public int? IdChild { get; set; }
            public int? IdParent { get; set; }
            public string DisplayName { get; set; }
            public string DisplayNameVN { get; set; }
            public bool? IsGroup { get; set; }
            public int? AuthorizedHeadcount { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
