using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._15_InterviewAssessment
{
    public partial class f315_Interview_Info : XtraForm
    {
        private sealed class CandidateEditorRow
        {
            public dm_User User { get; set; }
            public long? ProfileId { get; set; }
            public string Id => User?.Id;
            public string DisplayName => $"{User?.DisplayName} {User?.DisplayNameVN}".Trim();
            public string IdDepartment => User?.IdDepartment;
            public string JobCode => User?.JobCode;
            public bool UsesDefaultInterviewers { get; set; } = true;
            public List<string> InterviewerIds { get; set; } = new List<string>();
            public string OriginalFileName { get; set; }
            public string RelativePath { get; set; }
            public string SourcePdfPath { get; set; }
            public int SubmittedCount { get; set; }
            public string PdfStatus => !string.IsNullOrWhiteSpace(SourcePdfPath)
                ? $"待上傳：{Path.GetFileName(SourcePdfPath)}"
                : (!string.IsNullOrWhiteSpace(RelativePath) ? OriginalFileName : "尚未上傳");
            public string AssignmentMode => UsesDefaultInterviewers ? "依批次預設" : "個別設定";
            public string AssignmentText { get; set; }
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "口試評核";
        public string idBase = "";

        private readonly BindingSource sourceInterviewers = new BindingSource();
        private readonly BindingSource sourceCandidates = new BindingSource();
        private readonly List<dm_User> users = new List<dm_User>();
        private List<dm_User> defaultInterviewers = new List<dm_User>();
        private List<CandidateEditorRow> candidates = new List<CandidateEditorRow>();
        private Interview315ReportDetail loadedReport;
        private string StorageRoot => TPConfigs.Folder315InterviewReports;
        private bool IsEditable => eventInfo == EventFormInfo.Create || eventInfo == EventFormInfo.Update;

        public f315_Interview_Info()
        {
            InitializeComponent();
            InitializeExtraControls();
            InitializeIcon();
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new Font(
                "Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnAddInterviewer.ImageOptions.SvgImage = TPSvgimages.Add;
            btnAddInterviewee.ImageOptions.SvgImage = TPSvgimages.Add;
            btnDelInterviewer.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnDelInterviewee.ImageOptions.SvgImage = TPSvgimages.Remove;
        }

        private void InitializeExtraControls()
        {
            layoutControlGroup1.Text = "預設委員";
            layoutControlGroup2.Text = "受評人員（右鍵管理 PDF／個別委員）";
            lcDisplayName.Text = "評核名稱";
            gvInterviewee.PopupMenuShowing += CandidatePopupMenuShowing;
            gvInterviewee.DoubleClick += (s, e) => ViewCandidatePdf();
            gvInterviewee.Columns.AddVisible("PdfStatus", "PDF");
            gvInterviewee.Columns.AddVisible("AssignmentMode", "委員設定");
            gvInterviewee.Columns.AddVisible("AssignmentText", "委員名單");
        }

        private void f315_Interview_Info_Load(object sender, EventArgs e)
        {
            users.AddRange(dm_UserBUS.Instance.GetList());
            gcInterviewer.DataSource = sourceInterviewers;
            gcInterviewee.DataSource = sourceCandidates;
            gvInterviewer.ReadOnlyGridView();
            gvInterviewee.ReadOnlyGridView();
            gvInterviewer.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvInterviewee.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            if (eventInfo != EventFormInfo.Create)
            {
                LoadExistingReport();
            }
            RefreshGrids();
            ApplyMode();
        }

        private void LoadExistingReport()
        {
            loadedReport = dt315_InterviewAssessmentBUS.Instance.GetReportDetail(idBase);
            if (loadedReport == null) throw new InvalidOperationException("找不到評核批次。");
            txbDisplayName.EditValue = loadedReport.DisplayName;
            defaultInterviewers = users.Where(user => loadedReport.DefaultInterviewerIds.Contains(user.Id)).ToList();
            candidates = loadedReport.Candidates.Select(item => new CandidateEditorRow
            {
                User = users.FirstOrDefault(user => user.Id == item.CandidateId) ?? new dm_User { Id = item.CandidateId, DisplayName = item.CandidateId },
                ProfileId = item.ProfileId,
                UsesDefaultInterviewers = item.UsesDefaultInterviewers,
                InterviewerIds = item.InterviewerIds.ToList(),
                OriginalFileName = item.OriginalFileName,
                RelativePath = item.RelativePath,
                SubmittedCount = item.SubmittedCount
            }).ToList();
        }

        private void ApplyMode()
        {
            var editable = IsEditable && (loadedReport == null || loadedReport.Status == "Draft");
            txbDisplayName.Enabled = editable;
            btnAddInterviewer.Enabled = editable;
            btnDelInterviewer.Enabled = editable;
            btnAddInterviewee.Enabled = editable;
            btnDelInterviewee.Enabled = editable;
            btnConfirm.Visibility = editable ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnEdit.Visibility = eventInfo == EventFormInfo.View && loadedReport?.Status == "Draft"
                ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = eventInfo == EventFormInfo.View && loadedReport?.Status == "Draft"
                ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            Text = eventInfo == EventFormInfo.Create ? "新增口試評核" : $"口試評核 - {loadedReport?.DisplayName}";
        }

        private void RefreshGrids()
        {
            sourceInterviewers.DataSource = defaultInterviewers.Select(user => new
            {
                usr = user,
                user.Id,
                DisplayName = $"{user.DisplayName} {user.DisplayNameVN}".Trim(),
                user.IdDepartment,
                user.JobCode
            }).ToList();
            foreach (var candidate in candidates)
            {
                var ids = candidate.UsesDefaultInterviewers
                    ? defaultInterviewers.Select(item => item.Id)
                    : candidate.InterviewerIds;
                candidate.AssignmentText = string.Join(", ", ids.Select(id =>
                {
                    var user = users.FirstOrDefault(item => item.Id == id);
                    return user == null ? id : $"{id} {user.DisplayName}";
                }));
            }
            sourceCandidates.DataSource = candidates.ToList();
            gvInterviewer.BestFitColumns();
            gvInterviewee.BestFitColumns();
        }

        private List<dm_User> SelectUsers(List<dm_User> selected, bool fullUser)
        {
            using (var form = new f315_UsersData { UsersInput = selected.ToList(), IsFullUser = fullUser })
            {
                form.ShowDialog(this);
                if (form.UsersOutput != null) selected.AddRange(form.UsersOutput.Where(item => selected.All(old => old.Id != item.Id)));
            }
            return selected;
        }

        private void btnAddInterviewer_Click(object sender, EventArgs e)
        {
            if (!IsEditable) return;
            SelectUsers(defaultInterviewers, true);
            RefreshGrids();
        }

        private void btnAddInterviewee_Click(object sender, EventArgs e)
        {
            if (!IsEditable) return;
            var selected = candidates.Select(item => item.User).ToList();
            var before = new HashSet<string>(selected.Select(item => item.Id));
            SelectUsers(selected, false);
            candidates.AddRange(selected.Where(item => !before.Contains(item.Id)).Select(item => new CandidateEditorRow { User = item }));
            RefreshGrids();
        }

        private void btnDelInterviewer_Click(object sender, EventArgs e)
        {
            if (!IsEditable) return;
            var ids = gvInterviewer.GetSelectedRows().Select(row => ((dynamic)gvInterviewer.GetRow(row)).Id as string).ToList();
            defaultInterviewers.RemoveAll(item => ids.Contains(item.Id));
            RefreshGrids();
        }

        private void btnDelInterviewee_Click(object sender, EventArgs e)
        {
            if (!IsEditable) return;
            var selected = gvInterviewee.GetSelectedRows().Select(row => gvInterviewee.GetRow(row) as CandidateEditorRow).Where(item => item != null).ToList();
            if (selected.Any(item => item.SubmittedCount > 0))
            {
                XtraMessageBox.Show("已有評分的受評人員不能移除。", TPConfigs.SoftNameTW);
                return;
            }
            candidates.RemoveAll(item => selected.Contains(item));
            RefreshGrids();
        }

        private CandidateEditorRow FocusedCandidate => gvInterviewee.GetFocusedRow() as CandidateEditorRow;

        private static DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var item = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            item.AppearanceHovered.ForeColor = Color.Blue;
            return item;
        }

        private void CandidatePopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row || FocusedCandidate == null) return;
            e.Menu.Items.Add(CreateMenuItem("查看 PDF", (s, a) => ViewCandidatePdf(), TPSvgimages.View));
            if (!IsEditable) return;
            e.Menu.Items.Add(CreateMenuItem("上傳／更換 PDF", (s, a) => SelectCandidatePdf(), TPSvgimages.UploadFile));
            e.Menu.Items.Add(CreateMenuItem("個別設定委員", (s, a) => CustomizeCandidateInterviewers(), TPSvgimages.Edit));
            e.Menu.Items.Add(CreateMenuItem("恢復批次預設委員", (s, a) => ResetCandidateInterviewers(), TPSvgimages.Reload));
        }

        private void SelectCandidatePdf()
        {
            var candidate = FocusedCandidate;
            if (candidate == null || candidate.SubmittedCount > 0)
            {
                XtraMessageBox.Show("已有評分，不能更換 PDF。", TPConfigs.SoftNameTW);
                return;
            }
            using (var dialog = new OpenFileDialog { Filter = "PDF (*.pdf)|*.pdf", Multiselect = false })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK) candidate.SourcePdfPath = dialog.FileName;
            }
            RefreshGrids();
        }

        private void ViewCandidatePdf()
        {
            var candidate = FocusedCandidate;
            if (candidate == null) return;
            var path = !string.IsNullOrWhiteSpace(candidate.SourcePdfPath)
                ? candidate.SourcePdfPath
                : (!string.IsNullOrWhiteSpace(candidate.RelativePath) ? Path.Combine(StorageRoot, candidate.RelativePath) : null);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                XtraMessageBox.Show("找不到 PDF。", TPConfigs.SoftNameTW);
                return;
            }
            using (var viewer = new f00_VIewFile(path, false))
            {
                viewer.ShowDialog(this);
            }
        }

        private void CustomizeCandidateInterviewers()
        {
            var candidate = FocusedCandidate;
            if (candidate == null || candidate.SubmittedCount > 0) return;
            var currentIds = candidate.UsesDefaultInterviewers
                ? defaultInterviewers.Select(item => item.Id).ToList()
                : candidate.InterviewerIds.ToList();
            var selected = users.Where(item => currentIds.Contains(item.Id)).ToList();
            SelectUsers(selected, true);
            candidate.UsesDefaultInterviewers = false;
            candidate.InterviewerIds = selected.Select(item => item.Id).Distinct().ToList();
            RefreshGrids();
        }

        private void ResetCandidateInterviewers()
        {
            var candidate = FocusedCandidate;
            if (candidate == null || candidate.SubmittedCount > 0) return;
            candidate.UsesDefaultInterviewers = true;
            candidate.InterviewerIds.Clear();
            RefreshGrids();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var request = new Interview315SaveRequest
                {
                    ReportId = loadedReport?.Id,
                    DisplayName = Convert.ToString(txbDisplayName.EditValue),
                    ActorId = TPConfigs.LoginUser.Id,
                    StorageRoot = StorageRoot,
                    DefaultInterviewerIds = defaultInterviewers.Select(item => item.Id).ToList(),
                    Candidates = candidates.Select(item => new Interview315CandidateInput
                    {
                        ProfileId = item.ProfileId,
                        CandidateId = item.Id,
                        UsesDefaultInterviewers = item.UsesDefaultInterviewers,
                        InterviewerIds = item.InterviewerIds.ToList(),
                        SourcePdfPath = item.SourcePdfPath
                    }).ToList()
                };
                using (SplashScreenManager.ShowOverlayForm(this))
                    idBase = dt315_InterviewAssessmentBUS.Instance.Save(request);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            ApplyMode();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("確定刪除此口試評核？", TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                dt315_InterviewAssessmentBUS.Instance.DeleteDraft(loadedReport.Id, StorageRoot);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
