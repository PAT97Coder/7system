using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChartDataSource = KnowledgeSystem.Helpers.ChartDataSource;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_DataStatistics : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_DataStatistics()
        {
            InitializeComponent();
        }

        #region parameters

        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();

        List<dm_Departments> lsDepts;
        Dictionary<int, dm_Departments> departmentsByChild;

        private class DataStatisticsChart
        {
            public string NodeId { get; set; }
            public string ParentNodeId { get; set; }
            public string DisplayName { get; set; }
            public int Achieve { get; set; }
            public int Target { get; set; }
            public int Progress => GetProgress(this);
            public string Remark => GetStatus(this);
            public bool IsLeaf;
            public bool IsUser;
        }

        private class DepartmentLookup
        {
            public string Id { get; set; }
            public string Code { get; set; }
            public string DisplayName { get; set; }
        }

        private class DocumentStatistic
        {
            public string DepartmentId { get; set; }
            public string UserUploadName { get; set; }
        }

        const int MIN_DEPARTMENT_LEVEL = 2;
        const int MAX_DEPARTMENT_LEVEL = 3;
        const string STATISTICS_ROOT_DEPARTMENT_ID = "7";
        const string ALL_DEPARTMENTS_ID = "__ALL__";
        const string NAME_DEPARTMENT = "部門";
        const string NAME_CLASS = "課別";
        const string NAME_USER = "上傳者";

        const string NAME_UPPER = "超標";
        const string NAME_EQUAL = "達標";
        const string NAME_LOWER = "未達標";
        const string NAME_NOT_SET = "尚未設定";

        List<DataStatisticsChart> lsDataStatistic = new List<DataStatisticsChart>();

        BindingSource source = new BindingSource();

        #endregion

        #region methods

        private void LoadData()
        {
            var allDepartments = dm_DeptBUS.Instance.GetList();
            departmentsByChild = allDepartments
                .Where(r => r.IdChild.HasValue)
                .GroupBy(r => r.IdChild.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var activeDepartments = dm_DeptBUS.Instance.GetActiveList();
            lsDepts = OrderDepartments(activeDepartments
                .Where(r => IsInStatisticsBranch(r))
                .Where(r =>
                {
                    int level = GetHierarchyLevel(r);
                    return level >= MIN_DEPARTMENT_LEVEL && level <= MAX_DEPARTMENT_LEVEL;
                })
                .ToList());

            var departmentLookup = new List<DepartmentLookup>
            {
                new DepartmentLookup { Id = ALL_DEPARTMENTS_ID, Code = "全部", DisplayName = "全部部門" }
            };
            departmentLookup.AddRange(lsDepts.Select(r => new DepartmentLookup
            {
                Id = r.Id,
                Code = r.Id,
                DisplayName = r.DisplayName
            }));

            cbbGrade.Properties.DataSource = departmentLookup;
            cbbGrade.Properties.DisplayMember = "DisplayName";
            cbbGrade.Properties.ValueMember = "Id";
            cbbGrade.EditValue = ALL_DEPARTMENTS_ID;
        }

        private void CreateRuleGV()
        {
            gcData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault { BackColor = Color.FromArgb(220, 235, 252), ForeColor = Color.FromArgb(25, 85, 150) }, $"[Remark] = \'{NAME_UPPER}\'");
            gcData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault { BackColor = Color.FromArgb(224, 242, 228), ForeColor = Color.FromArgb(30, 120, 55) }, $"[Remark] = \'{NAME_EQUAL}\'");
            gcData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault { BackColor = Color.FromArgb(252, 230, 230), ForeColor = Color.FromArgb(190, 45, 45) }, $"[Remark] = \'{NAME_LOWER}\'");
            gcData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault { BackColor = Color.FromArgb(238, 238, 238), ForeColor = Color.DimGray }, $"[Remark] = \'{NAME_NOT_SET}\'");
        }

        private int GetHierarchyLevel(dm_Departments department)
        {
            int level = 1;
            var visited = new HashSet<int>();
            var current = department;

            if (current.IdChild.HasValue)
            {
                visited.Add(current.IdChild.Value);
            }

            while (current.IdParent.HasValue &&
                   departmentsByChild.TryGetValue(current.IdParent.Value, out dm_Departments parent))
            {
                if (!parent.IdChild.HasValue || !visited.Add(parent.IdChild.Value))
                {
                    return int.MaxValue;
                }

                level++;
                current = parent;
            }

            return level;
        }

        private bool IsInStatisticsBranch(dm_Departments department)
        {
            var current = department;
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            while (current != null && visited.Add(current.Id))
            {
                if (string.Equals(current.Id, STATISTICS_ROOT_DEPARTMENT_ID, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (!current.IdParent.HasValue ||
                    !departmentsByChild.TryGetValue(current.IdParent.Value, out current))
                {
                    return false;
                }
            }

            return false;
        }

        private List<dm_Departments> OrderDepartments(List<dm_Departments> departments)
        {
            var result = new List<dm_Departments>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var includedChildren = departments
                .Where(r => r.IdChild.HasValue)
                .ToDictionary(r => r.IdChild.Value, r => r);
            var childrenByParent = departments
                .Where(r => r.IdParent.HasValue)
                .GroupBy(r => r.IdParent.Value)
                .ToDictionary(g => g.Key, g => g.OrderBy(r => r.IdChild).ToList());

            void AddBranch(dm_Departments department)
            {
                if (department == null || !visited.Add(department.Id)) return;

                result.Add(department);
                if (department.IdChild.HasValue &&
                    childrenByParent.TryGetValue(department.IdChild.Value, out List<dm_Departments> children))
                {
                    foreach (var child in children)
                    {
                        AddBranch(child);
                    }
                }
            }

            var roots = departments
                .Where(r => !r.IdParent.HasValue || !includedChildren.ContainsKey(r.IdParent.Value))
                .OrderBy(r => r.IdChild)
                .ToList();

            foreach (var root in roots)
            {
                AddBranch(root);
            }

            foreach (var department in departments.OrderBy(r => r.IdChild))
            {
                AddBranch(department);
            }

            return result;
        }

        private HashSet<string> GetActiveDepartmentIdsInBranch(dm_Departments department)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { department.Id };
            if (!department.IdChild.HasValue) return result;

            var pendingParents = new Queue<int>();
            var visitedParents = new HashSet<int>();
            pendingParents.Enqueue(department.IdChild.Value);

            while (pendingParents.Count > 0)
            {
                int parentId = pendingParents.Dequeue();
                if (!visitedParents.Add(parentId)) continue;

                foreach (var child in lsDepts.Where(r => r.IdParent == parentId))
                {
                    result.Add(child.Id);
                    if (child.IdChild.HasValue)
                    {
                        pendingParents.Enqueue(child.IdChild.Value);
                    }
                }
            }

            return result;
        }

        private bool HasDisplayedChildren(dm_Departments department)
        {
            return department.IdChild.HasValue && lsDepts.Any(r => r.IdParent == department.IdChild.Value);
        }

        private void AddDepartmentStatistic(
            dm_Departments department,
            string displayName,
            IReadOnlyDictionary<string, int> targetMap,
            List<DocumentStatistic> documents,
            bool isSummaryLeaf,
            string parentNodeId)
        {
            var departmentIds = GetActiveDepartmentIdsInBranch(department);
            lsDataStatistic.Add(new DataStatisticsChart
            {
                NodeId = "D:" + department.Id,
                ParentNodeId = parentNodeId,
                DisplayName = displayName,
                Achieve = documents.Count(r => departmentIds.Contains(r.DepartmentId)),
                Target = targetMap.TryGetValue(department.Id, out int target) ? target : 0,
                IsLeaf = isSummaryLeaf
            });
        }

        private static int GetStatusOrder(DataStatisticsChart row)
        {
            if (row.Target <= 0) return 1;
            if (row.Achieve < row.Target) return 0;
            return 2;
        }

        private static int GetProgress(DataStatisticsChart row)
        {
            return row.Target <= 0
                ? 0
                : Math.Min(100, (int)Math.Round(row.Achieve * 100.0 / row.Target));
        }

        private static string GetStatus(DataStatisticsChart row)
        {
            if (row.Target <= 0) return NAME_NOT_SET;
            if (row.Achieve < row.Target) return NAME_LOWER;
            if (row.Achieve == row.Target) return NAME_EQUAL;
            return NAME_UPPER;
        }

        private void StatisticsData()
        {
            string idDept = cbbGrade.EditValue?.ToString();

            if (string.IsNullOrWhiteSpace(idDept))
            {
                XtraMessageBox.Show("找不到啟用中的部門資料！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fromDate = txbFromDate.DateTime.Date;
            DateTime toDate = txbToDate.DateTime.Date.AddDays(1).AddSeconds(-1);

            if (string.IsNullOrEmpty(txbFromDate.Text) || string.IsNullOrEmpty(txbToDate.Text) || toDate < fromDate)
            {
                XtraMessageBox.Show("請選擇正確的日期數據！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var targetMap = dt207_TargetsBUS.Instance.GetList()
                .GroupBy(r => r.IdDept)
                .ToDictionary(g => g.Key, g => g.First().Targets, StringComparer.OrdinalIgnoreCase);
            var lsBase207 = _dt207_BaseBUS.GetListByDate(fromDate, toDate);
            var lsUsers = dm_UserBUS.Instance.GetList();
            var lsBaseProcessing = dt207_DocProcessingBUS.Instance.GetListNotComplete();

            // Văn kiện đang trong quy trình ký duyệt chưa được tính là đã tải hoàn tất.
            var lsIdBaseProcessing = lsBaseProcessing.Select(r => r.IdKnowledgeBase).Distinct().ToList();
            var lsDoc = (from data in lsBase207
                         join users in lsUsers on data.UserUpload equals users.Id
                         where !lsIdBaseProcessing.Contains(data.Id) && !string.IsNullOrEmpty(users.IdDepartment)
                         select new DocumentStatistic
                         {
                             DepartmentId = users.IdDepartment,
                             UserUploadName = data.UserUpload + " " + users.DisplayName
                         }).ToList();

            lsDataStatistic.Clear();

            bool showUsers = false;
            if (idDept == ALL_DEPARTMENTS_ID)
            {
                gColType.Caption = NAME_DEPARTMENT;
                foreach (var department in lsDepts)
                {
                    var visibleParent = department.IdParent.HasValue
                        ? lsDepts.FirstOrDefault(r => r.IdChild == department.IdParent.Value)
                        : null;
                    AddDepartmentStatistic(
                        department,
                        department.DisplayName,
                        targetMap,
                        lsDoc,
                        !HasDisplayedChildren(department),
                        visibleParent == null ? null : "D:" + visibleParent.Id);
                }
            }
            else
            {
                var selectedDepartment = lsDepts.FirstOrDefault(r => r.Id == idDept);
                if (selectedDepartment == null) return;

                var children = selectedDepartment.IdChild.HasValue
                    ? lsDepts.Where(r => r.IdParent == selectedDepartment.IdChild.Value).ToList()
                    : new List<dm_Departments>();

                if (children.Count > 0)
                {
                    gColType.Caption = NAME_CLASS;
                    foreach (var child in children)
                    {
                        AddDepartmentStatistic(child, child.DisplayName, targetMap, lsDoc, true, null);
                    }

                    lsDataStatistic.Sort((left, right) =>
                    {
                        int statusComparison = GetStatusOrder(left).CompareTo(GetStatusOrder(right));
                        return statusComparison != 0
                            ? statusComparison
                            : string.Compare(left.DisplayName, right.DisplayName, StringComparison.CurrentCulture);
                    });
                }
                else
                {
                    showUsers = true;
                    gColType.Caption = NAME_USER;
                    var departmentIds = GetActiveDepartmentIdsInBranch(selectedDepartment);
                    lsDataStatistic.AddRange(lsDoc
                        .Where(r => departmentIds.Contains(r.DepartmentId))
                        .GroupBy(r => r.UserUploadName)
                        .OrderBy(r => r.Key)
                        .Select(r => new DataStatisticsChart
                        {
                            NodeId = "U:" + r.Key,
                            DisplayName = r.Key,
                            Achieve = r.Count(),
                            IsLeaf = true,
                            IsUser = true
                        }));
                }
            }

            gColTarget.Visible = !showUsers;
            gColProgress.Visible = !showUsers;
            gColRemark.Visible = !showUsers;
            source.ResetBindings(false);
            gcData.RefreshDataSource();
            gcData.ExpandAll();
            gcData.BestFitColumns();

            var summaryRows = lsDataStatistic.Where(r => r.IsLeaf).ToList();
            int totalAchieve = summaryRows.Sum(r => r.Achieve);
            int totalTarget = summaryRows.Sum(r => r.Target);
            int configuredCount = summaryRows.Count(r => r.Target > 0);
            int achievedCount = summaryRows.Count(r => r.Target > 0 && r.Achieve >= r.Target);
            label1.Text = showUsers
                ? "資料上傳統計"
                : $"資料上傳統計　｜　總進度 {totalAchieve}/{totalTarget}　｜　達標 {achievedCount}/{configuredCount}";
        }

        #endregion

        private void uc207_DataStatistics_Load(object sender, EventArgs e)
        {
            source.DataSource = lsDataStatistic;
            gcData.KeyFieldName = nameof(DataStatisticsChart.NodeId);
            gcData.ParentFieldName = nameof(DataStatisticsChart.ParentNodeId);
            gcData.DataSource = source;

            CreateRuleGV();
            gcData.NodeCellStyle += gcData_NodeCellStyle;
            LoadData();

            btnExcel.Text = "導出\r\nExcel";
            btnStatistics.Text = "資料\r\n統計";
            btnChart.Text = "繪製\r\n圖表";
            btnTarget.Text = "設定\r\n目標";

            DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            txbFromDate.EditValue = firstDayOfMonth;

            DateTime lastDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            txbToDate.EditValue = lastDayOfMonth;

            StatisticsData();
        }

        private void gcData_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            var row = gcData.GetDataRecordByNode(e.Node) as DataStatisticsChart;
            if (row != null && !row.IsLeaf && !row.IsUser)
            {
                e.Appearance.BackColor = Color.FromArgb(235, 243, 250);
                e.Appearance.ForeColor = Color.FromArgb(55, 79, 107);
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
        }

        private void cbbGrade_EditValueChanged(object sender, EventArgs e)
        {
            string idGrade = cbbGrade.EditValue?.ToString();
            if (string.IsNullOrWhiteSpace(idGrade)) return;

            if (idGrade == ALL_DEPARTMENTS_ID)
            {
                gColType.Caption = NAME_DEPARTMENT;
                return;
            }

            var selectedDepartment = lsDepts.FirstOrDefault(r => r.Id == idGrade);
            gColType.Caption = selectedDepartment != null && HasDisplayedChildren(selectedDepartment)
                ? NAME_CLASS
                : NAME_USER;
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            StatisticsData();
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            List<ChartDataSource> sourceChart = new List<ChartDataSource>();
            var chartRows = lsDataStatistic.Where(r => r.IsLeaf).ToList();

            sourceChart.AddRange(chartRows.Select(r => new ChartDataSource() { SeriesName = "Actual", XAxis = r.DisplayName, YAxis = r.Achieve }));
            if (gColTarget.Visible)
            {
                sourceChart.AddRange(chartRows.Select(r => new ChartDataSource() { SeriesName = "Targets", XAxis = r.DisplayName, YAxis = r.Target }));
            }

            f207_ChartStatistics f207_Chart = new f207_ChartStatistics(sourceChart);
            f207_Chart.ShowDialog();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            string nameFile = "";
            switch (gColType.Caption)
            {
                case NAME_DEPARTMENT:
                    nameFile = "部門資料上傳統計表";
                    break;
                case NAME_CLASS:
                    nameFile = "各處資料上傳統計表";
                    break;
                case NAME_USER:
                    nameFile = "各單位同仁資料上傳統計表";
                    break;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "導出資料上傳統計表";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.Filter = "Excel Files|*.xlsx";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = $"{DateTime.Now:yyyyMMddHHmmss}-{nameFile}";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            string newFilePath = Path.Combine(saveFileDialog1.FileName);

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(newFilePath))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("資料上傳統計表");
                    ws.Cells.Style.Font.Name = "DFKai-SB";
                    ws.Cells.Style.Font.Size = 14;
                    ws.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    bool includeEvaluation = gColTarget.Visible;
                    int exportedColumnCount = includeEvaluation ? 5 : 2;

                    ws.Column(1).Width = 35;
                    ws.Column(2).Width = 20;
                    if (includeEvaluation)
                    {
                        ws.Column(3).Width = 20;
                        ws.Column(4).Width = 20;
                        ws.Column(5).Width = 18;
                        var exportRows = lsDataStatistic.Select(r => new
                        {
                            r.DisplayName,
                            r.Achieve,
                            r.Target,
                            Progress = GetProgress(r) + "%",
                            Status = GetStatus(r)
                        });
                        ws.Cells["A2"].LoadFromCollection(exportRows, true, TableStyles.Medium2);
                    }
                    else
                    {
                        var exportRows = lsDataStatistic.Select(r => new
                        {
                            r.DisplayName,
                            r.Achieve
                        });
                        ws.Cells["A2"].LoadFromCollection(exportRows, true, TableStyles.Medium2);
                    }

                    ws.Cells["A1"].Value = nameFile;
                    ws.Cells["A1"].Style.Font.Size = 24;
                    ws.Cells[1, 1, 1, exportedColumnCount].Merge = true;

                    ws.Cells["A2"].Value = gColType.Caption;
                    ws.Cells["B2"].Value = gColAchieve.Caption;
                    if (includeEvaluation)
                    {
                        ws.Cells["C2"].Value = gColTarget.Caption;
                        ws.Cells["D2"].Value = gColProgress.Caption;
                        ws.Cells["E2"].Value = gColRemark.Caption;
                    }

                    // Vẽ đồ thị
                    int sumRow = lsDataStatistic.Count();
                    if (sumRow > 0)
                    {
                        ExcelChart chart = ws.Drawings.AddChart("FindingsChart", eChartType.ColumnClustered);
                        chart.Title.Text = nameFile;
                        chart.SetPosition(1, 0, 4, 0);
                        chart.SetSize(1000, 300);
                        var ser1 = (ExcelBarChartSerie)(chart.Series.Add(ws.Cells[$"B3:B{sumRow + 2}"], ws.Cells[$"A3:A{sumRow + 2}"]));
                        ser1.Header = gColAchieve.Caption;

                        ser1.DataLabel.ShowValue = true;
                        ser1.DataLabel.Position = eLabelPosition.OutEnd;
                        if (includeEvaluation)
                        {
                            var ser2 = (ExcelBarChartSerie)(chart.Series.Add(ws.Cells[$"C3:C{sumRow + 2}"], ws.Cells[$"A3:A{sumRow + 2}"]));
                            ser2.Header = gColTarget.Caption;
                            ser2.DataLabel.ShowValue = true;
                            ser2.DataLabel.Position = eLabelPosition.OutEnd;
                        }

                        chart.Legend.Add();
                        chart.Legend.Border.Width = 0;
                        chart.Legend.Font.Size = 10;
                        chart.Legend.Font.Bold = true;
                        chart.Legend.Position = eLegendPosition.Top;
                        chart.StyleManager.SetChartStyle(ePresetChartStyle.StackedColumnChartStyle1);
                    }

                    pck.Save();
                }

                Process.Start(newFilePath);
            }
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            f207_SetTarget setTarget = new f207_SetTarget();
            setTarget.ShowDialog();
        }
    }
}
