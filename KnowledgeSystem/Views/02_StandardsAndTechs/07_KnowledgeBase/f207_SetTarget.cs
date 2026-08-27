using BusinessLayer;
using DataAccessLayer;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_SetTarget : DevExpress.XtraEditors.XtraForm
    {
        private List<dm_Departments> activeDepartments = new List<dm_Departments>();

        public f207_SetTarget()
        {
            InitializeComponent();
            gvData.ShowingEditor += gvData_ShowingEditor;
            gvData.CellValueChanged += gvData_CellValueChanged;
            gvData.RowCellStyle += gvData_RowCellStyle;
        }

        private class TargetKnowedge
        {
            public string Id { get; set; }
            public string Grade { get; set; }
            public string Class { get; set; }
            public int Targets { get; set; }
            public bool IsCalculated { get; set; }
        }

        private void f207_SetTarget_Load(object sender, EventArgs e)
        {
            var lsTargets = dt207_TargetsBUS.Instance.GetList();
            activeDepartments = dm_DeptBUS.Instance.GetActiveList();
            var allDepts = dm_DeptBUS.Instance.GetList();

            dm_Departments gradeDel = allDepts.FirstOrDefault(r => r.Id == "7");

            var lsDeptTargets = (from data in activeDepartments
                                 join names in allDepts
                                 on data.IdParent equals names.IdChild into dgt
                                 from d in dgt.DefaultIfEmpty()
                                 select new TargetKnowedge
                                 {
                                     Id = data.Id,
                                     Grade = d?.DisplayName ?? gradeDel?.DisplayName ?? string.Empty,
                                     Class = data.DisplayName,
                                 }
                                 into dtDept
                                 join targets in lsTargets on dtDept.Id equals targets.IdDept into dgt
                                 from g in dgt.DefaultIfEmpty()
                                 select new TargetKnowedge
                                 {
                                     Id = dtDept.Id,
                                     Grade = dtDept.Grade,
                                     Class = dtDept.Class,
                                     Targets = g?.Targets ?? 0
                                 }
                                 ).ToList();

            RecalculateParentTargets(lsDeptTargets);
            gcData.DataSource = lsDeptTargets;
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

        private void gvData_ShowingEditor(object sender, CancelEventArgs e)
        {
            var row = gvData.GetFocusedRow() as TargetKnowedge;
            if (gvData.FocusedColumn == gridColumn4 && row?.IsCalculated == true)
            {
                e.Cancel = true;
            }
        }

        private void gvData_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column != gridColumn4) return;

            var rows = gcData.DataSource as List<TargetKnowedge>;
            RecalculateParentTargets(rows);
            gcData.RefreshDataSource();
        }

        private void gvData_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column != gridColumn4) return;

            var row = gvData.GetRow(e.RowHandle) as TargetKnowedge;
            if (row?.IsCalculated == true)
            {
                e.Appearance.BackColor = Color.Gainsboro;
                e.Appearance.FontStyleDelta = FontStyle.Italic;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvData.PostEditor();
            gvData.UpdateCurrentRow();

            List<TargetKnowedge> lsSource = gcData.DataSource as List<TargetKnowedge>;
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
