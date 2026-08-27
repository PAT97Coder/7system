using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_SetTarget : DevExpress.XtraEditors.XtraForm
    {
        private List<dm_Departments> activeDepartments = new List<dm_Departments>();

        public f207_SetTarget()
        {
            InitializeComponent();
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            treeTargets.ShowingEditor += treeTargets_ShowingEditor;
            treeTargets.CellValueChanged += treeTargets_CellValueChanged;
            treeTargets.NodeCellStyle += treeTargets_NodeCellStyle;
        }

        private class TargetKnowedge
        {
            public string Id { get; set; }
            public int? IdChild { get; set; }
            public int? IdParent { get; set; }
            public string Class { get; set; }
            public int Targets { get; set; }
            public bool IsCalculated { get; set; }
            public string TargetMode => IsCalculated ? "自動加總" : "手動設定";
        }

        private void f207_SetTarget_Load(object sender, EventArgs e)
        {
            var lsTargets = dt207_TargetsBUS.Instance.GetList();
            activeDepartments = dm_DeptBUS.Instance.GetActiveList();

            var targetMap = lsTargets
                .GroupBy(r => r.IdDept)
                .ToDictionary(g => g.Key, g => g.First().Targets, StringComparer.OrdinalIgnoreCase);

            var lsDeptTargets = activeDepartments
                .OrderBy(r => r.IdParent)
                .ThenBy(r => r.IdChild)
                .Select(r => new TargetKnowedge
                {
                    Id = r.Id,
                    IdChild = r.IdChild,
                    IdParent = r.IdParent,
                    Class = r.DisplayName,
                    Targets = targetMap.TryGetValue(r.Id, out int target) ? target : 0
                })
                .ToList();

            RecalculateParentTargets(lsDeptTargets);
            treeTargets.DataSource = lsDeptTargets;
            treeTargets.KeyFieldName = nameof(TargetKnowedge.IdChild);
            treeTargets.ParentFieldName = nameof(TargetKnowedge.IdParent);
            treeTargets.ExpandAll();
            treeTargets.BestFitColumns();
        }

        private void RecalculateParentTargets(List<TargetKnowedge> rows)
        {
            if (rows == null) return;

            var rowsById = rows.ToDictionary(r => r.Id, StringComparer.OrdinalIgnoreCase);
            var childrenByParent = activeDepartments
                .Where(r => r.IdParent.HasValue)
                .GroupBy(r => r.IdParent.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
            var calculated = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                row.IsCalculated = false;
                if (row.Targets < 0)
                {
                    row.Targets = 0;
                }
            }

            int Calculate(dm_Departments department, HashSet<string> path)
            {
                if (department == null || !rowsById.TryGetValue(department.Id, out TargetKnowedge row))
                {
                    return 0;
                }

                if (calculated.Contains(department.Id))
                {
                    return row.Targets;
                }

                if (!department.IdChild.HasValue ||
                    !childrenByParent.TryGetValue(department.IdChild.Value, out List<dm_Departments> children) ||
                    children.Count == 0)
                {
                    return row.Targets;
                }

                if (!path.Add(department.Id))
                {
                    return 0;
                }

                row.Targets = children.Sum(child => Calculate(child, path));
                row.IsCalculated = true;
                calculated.Add(department.Id);
                path.Remove(department.Id);
                return row.Targets;
            }

            foreach (var department in activeDepartments)
            {
                Calculate(department, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            }
        }

        private void treeTargets_ShowingEditor(object sender, CancelEventArgs e)
        {
            var row = treeTargets.GetDataRecordByNode(treeTargets.FocusedNode) as TargetKnowedge;
            if (treeTargets.FocusedColumn == tColTarget && row?.IsCalculated == true)
            {
                e.Cancel = true;
            }
        }

        private void treeTargets_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            if (e.Column != tColTarget) return;

            var rows = treeTargets.DataSource as List<TargetKnowedge>;
            RecalculateParentTargets(rows);
            treeTargets.RefreshDataSource();
        }

        private void treeTargets_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            var row = treeTargets.GetDataRecordByNode(e.Node) as TargetKnowedge;
            if (row?.IsCalculated == true)
            {
                e.Appearance.BackColor = Color.FromArgb(235, 243, 250);
                e.Appearance.ForeColor = Color.FromArgb(55, 79, 107);
                e.Appearance.FontStyleDelta = FontStyle.Italic;
            }
        }

        private void btnExpandAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            treeTargets.ExpandAll();
        }

        private void btnCollapseAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            treeTargets.CollapseAll();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            treeTargets.PostEditor();

            List<TargetKnowedge> lsSource = treeTargets.DataSource as List<TargetKnowedge>;
            RecalculateParentTargets(lsSource);

            List<dt207_Targets> lsTargetsUpdate = (from data in lsSource
                                                   select new dt207_Targets()
                                                   {
                                                       IdDept = data.Id,
                                                       Targets = data.Targets,
                                                   }).ToList();

            foreach (var item in lsTargetsUpdate)
            {
                dt207_TargetsBUS.Instance.AddOrUpdate(item);
            }

            XtraMessageBox.Show("更新成功", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
