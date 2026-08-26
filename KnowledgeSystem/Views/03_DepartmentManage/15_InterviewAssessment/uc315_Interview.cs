using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._15_InterviewAssessment
{
    public partial class uc315_Interview : XtraUserControl
    {
        private sealed class AssignmentListRow
        {
            public string CandidateName { get; set; }
            public string InterviewerName { get; set; }
            public bool HasPdf { get; set; }
            public string Status { get; set; }
            public decimal? Total { get; set; }
            public long? ScoreId { get; set; }
            public bool CanReopen { get; set; }
        }

        private readonly BindingSource sourceReports = new BindingSource();
        private readonly List<dm_User> users = new List<dm_User>();
        private DXMenuItem itemView;
        private DXMenuItem itemOpen;
        private DXMenuItem itemClose;

        public uc315_Interview()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeGrid();
            InitializeMenuItems();
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new Font(
                "Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void InitializeGrid()
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvData.OptionsDetail.EnableMasterViewMode = true;

            gvData.Columns.Clear();
            gvData.Columns.AddVisible(nameof(Interview315ReportRow.Id), "編碼");
            gvData.Columns.AddVisible(nameof(Interview315ReportRow.StatusText), "狀態");
            gvData.Columns.AddVisible(nameof(Interview315ReportRow.DisplayName), "名稱");
            gvData.Columns.AddVisible(nameof(Interview315ReportRow.CandidateCount), "受評人數");
            gvData.Columns.AddVisible(nameof(Interview315ReportRow.CreatedAt), "建立日");
            gvData.Columns.AddVisible(nameof(Interview315ReportRow.Progress), "評分進度");

            ConfigureDetailView(gvInfo);
        }

        private void ConfigureDetailView(GridView view)
        {
            if (view == null) return;

            view.ReadOnlyGridView();
            view.OptionsCustomization.AllowGroup = false;
            view.OptionsDetail.ShowDetailTabs = false;
            view.OptionsView.ColumnAutoWidth = false;
            view.OptionsView.ShowGroupPanel = false;
            view.GroupCount = 0;
            view.SortInfo.Clear();
            view.Columns.Clear();
            view.Columns.AddVisible(nameof(AssignmentListRow.CandidateName), "受評人員");
            view.Columns.AddVisible(nameof(AssignmentListRow.InterviewerName), "委員");
            view.Columns.AddVisible(nameof(AssignmentListRow.HasPdf), "PDF");
            view.Columns.AddVisible(nameof(AssignmentListRow.Status), "評分狀態");
            view.Columns.AddVisible(nameof(AssignmentListRow.Total), "總分");
            view.KeyDown -= GridControlHelper.GridViewCopyCellData_KeyDown;
            view.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            view.PopupMenuShowing -= ScorePopupMenuShowing;
            view.PopupMenuShowing += ScorePopupMenuShowing;
            view.BestFitColumns();
        }

        private void InitializeMenuItems()
        {
            itemView = CreateMenuItem("查看／編輯", (s, e) => ViewFocusedReport(), TPSvgimages.View);
            itemOpen = CreateMenuItem("開放評核", (s, e) => OpenFocusedReport(), TPSvgimages.Start);
            itemClose = CreateMenuItem("關閉評核", (s, e) => CloseFocusedReport(), TPSvgimages.Finish);
        }

        private static DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var item = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            item.AppearanceHovered.ForeColor = Color.Blue;
            return item;
        }

        private void uc315_Interview_Load(object sender, EventArgs e)
        {
            users.Clear();
            users.AddRange(dm_UserBUS.Instance.GetList());
            gcData.DataSource = sourceReports;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SplashScreenManager.ShowOverlayForm(gcData))
                    sourceReports.DataSource = dt315_InterviewAssessmentBUS.Instance.GetReportRows();
                gvData.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Interview315ReportRow FocusedReport => gvData.GetFocusedRow() as Interview315ReportRow;

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = new f315_Interview_Info { eventInfo = EventFormInfo.Create, formName = "口試評核" })
            {
                if (form.ShowDialog(this) == DialogResult.OK) LoadData();
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void ViewFocusedReport()
        {
            var report = FocusedReport;
            if (report == null) return;
            using (var form = new f315_Interview_Info
            {
                eventInfo = EventFormInfo.View,
                formName = "口試評核",
                idBase = report.Id
            })
            {
                form.ShowDialog(this);
                LoadData();
            }
        }

        private void OpenFocusedReport()
        {
            var report = FocusedReport;
            if (report == null) return;
            if (XtraMessageBox.Show("開放後將不能修改受評人員、PDF 與委員，確定繼續？",
                TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                dt315_InterviewAssessmentBUS.Instance.OpenReport(report.Id, TPConfigs.LoginUser.Id);
                LoadData();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseFocusedReport()
        {
            var report = FocusedReport;
            if (report == null) return;
            if (XtraMessageBox.Show("關閉後將無法繼續評分，確定繼續？",
                TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                dt315_InterviewAssessmentBUS.Instance.CloseReport(report.Id, TPConfigs.LoginUser.Id);
                LoadData();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row || FocusedReport == null) return;
            e.Menu.Items.Add(itemView);
            if (FocusedReport.Status == "Draft" || FocusedReport.Status == "Closed") e.Menu.Items.Add(itemOpen);
            if (FocusedReport.Status == "Open") e.Menu.Items.Add(itemClose);
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) { e.RelationCount = 1; }
        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) { e.RelationName = "受評人員"; }
        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e) { e.IsEmpty = false; }
        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            if (masterView == null) return;

            int relationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, relationIndex) as GridView;
            ConfigureDetailView(detailView);
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var report = gvData.GetRow(e.RowHandle) as Interview315ReportRow;
            if (report == null) return;
            var detail = dt315_InterviewAssessmentBUS.Instance.GetReportDetail(report.Id);
            e.ChildList = detail.Candidates.SelectMany(candidate =>
            {
                var candidateName = $"{candidate.CandidateId} {users.FirstOrDefault(user => user.Id == candidate.CandidateId)?.DisplayName}".Trim();
                var hasPdf = !string.IsNullOrWhiteSpace(candidate.RelativePath);
                if (!candidate.Assignments.Any())
                    return new[] { new AssignmentListRow { CandidateName = candidateName, HasPdf = hasPdf, Status = "未分配委員" } };
                return candidate.Assignments.Select(assignment => new AssignmentListRow
                {
                    CandidateName = candidateName,
                    InterviewerName = $"{assignment.InterviewerId} {users.FirstOrDefault(user => user.Id == assignment.InterviewerId)?.DisplayName}".Trim(),
                    HasPdf = hasPdf,
                    Status = assignment.IsSubmitted ? "已提交" : (assignment.ScoreId.HasValue ? "已解除鎖定" : "待評分"),
                    Total = assignment.Total,
                    ScoreId = assignment.ScoreId,
                    CanReopen = assignment.IsSubmitted
                });
            }).ToList();
        }

        private void ScorePopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            GridView detailView = sender as GridView;
            var row = detailView?.GetFocusedRow() as AssignmentListRow;
            if (e.MenuType != GridMenuType.Row || row == null || !row.CanReopen || !row.ScoreId.HasValue) return;
            e.Menu.Items.Add(CreateMenuItem("解除評分鎖定", (s, a) => ReopenScore(row), TPSvgimages.Reload));
        }

        private void ReopenScore(AssignmentListRow row)
        {
            var editor = new MemoEdit();
            var reason = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = "解除評分鎖定",
                Prompt = "請輸入原因：",
                Editor = editor,
                DefaultResponse = ""
            });
            if (reason == null || string.IsNullOrWhiteSpace(reason.ToString())) return;
            try
            {
                dt315_InterviewAssessmentBUS.Instance.ReopenScore(row.ScoreId.Value, TPConfigs.LoginUser.Id, reason.ToString());
                gvData.CollapseAllDetails();
                LoadData();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvInfo_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e) { }
    }
}
