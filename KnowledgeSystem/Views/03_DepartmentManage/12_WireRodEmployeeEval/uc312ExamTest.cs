using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class uc312ExamTest : XtraUserControl
    {
        public uc312ExamTest()
        {
            TraditionalGridLocalizer.Install();
            InitializeComponent();
        }

        private bool starting;
        private bool canPractice;

        private void Internal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => Internal_Click(sender, EventArgs.Empty);
        private void External_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => External_Click(sender, EventArgs.Empty);
        private void Refresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => Refresh_Click(sender, EventArgs.Empty);

        private void Results_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            if (e.Menu != null) TraditionalGridLocalizer.StyleMenu(e.Menu);
            if (view == null || e.Menu == null || !e.HitInfo.InDataRow || !e.HitInfo.InRowCell) return;
            var row = view.GetRow(e.HitInfo.RowHandle) as dt312_PracticeResult;
            if (row == null) return;
            view.FocusedRowHandle = e.HitInfo.RowHandle;
            // Capture the clicked attempt, independent of the previously focused row or sorting.
            int attemptId = row.Id;
            string kind = row.PracticeKind;
            var item = new DXMenuItem("查看詳情", (s, args) => ShowResultDetail(attemptId, kind), TPSvgimages.View, DXMenuItemPriority.Normal)
                { BeginGroup = true, Enabled = canPractice };
            e.Menu.Items.Add(item);
            TraditionalGridLocalizer.StyleMenu(e.Menu);
        }

        private void ShowResultDetail(int attemptId, string kind)
        {
            try
            {
                var detail = dt312_PracticeBUS.Instance.GetResultDetail(TPConfigs.LoginUser.Id, attemptId, kind);
                if (detail == null || string.IsNullOrWhiteSpace(detail.ExamData))
                {
                    MsgTP.MsgShowInfomation("找不到已提交的測驗作答資料。");
                    return;
                }
                var results = JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>(detail.ExamData);
                if (results == null || results.Count == 0 || results.Any(r => r == null || r.Questions == null))
                    throw new dt312_PracticeValidationException("此測驗的作答資料不完整。");
                using (var review = new f312_ExamReview(results, detail)) review.ShowDialog(FindForm());
            }
            catch (Exception ex) { MsgTP.MsgError("無法開啟測驗詳情：\r\n" + Exam312Ui.ErrorText(ex)); }
        }

        private void Result_DisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.ListSourceRowIndex >= 0 && e.Column.FieldName == "Score" && (e.Value == null || e.Value == DBNull.Value)) e.DisplayText = "不計分";
            if (e.Column.FieldName == "PracticeKind")
            {
                if (Convert.ToString(e.Value) == "INTERNAL") e.DisplayText = "內部";
                else if (Convert.ToString(e.Value) == "EXTERNAL") e.DisplayText = "外部";
            }
        }

        private void uc312ExamTest_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            RefreshResults();
        }

        private void RefreshResults()
        {
            canPractice = false;
            btnInternal.Enabled = btnExternal.Enabled = btnRefresh.Enabled = false;
            int? selectedId = (resultsView.GetFocusedRow() as dt312_PracticeResult)?.Id;
            try
            {
                string userId = TPConfigs.LoginUser.Id;
                canPractice = dt312_QuestionsBUS.Instance.CanPractice(userId);
                btnRefresh.Enabled = canPractice;
                dt312_PracticeBUS.Instance.EnsureAvailable(userId);
                var user = dm_UserBUS.Instance.GetItemById(userId);
                bool internalUser = user != null && dt312_PracticeBUS.IsInternalDepartment(user.IdDepartment);
                resultsGrid.DataSource = dt312_PracticeBUS.Instance.GetResults(userId);
                if (selectedId.HasValue)
                {
                    int rowHandle = resultsView.LocateByValue("Id", selectedId.Value);
                    if (rowHandle >= 0) resultsView.FocusedRowHandle = rowHandle;
                }
                canPractice = true;
                btnInternal.Enabled = internalUser;
                btnExternal.Enabled = true;
                btnRefresh.Enabled = true;
            }
            catch (Exception ex)
            {
                resultsGrid.DataSource = null;
                MsgTP.MsgError("無法載入模擬測驗：\r\n" + Exam312Ui.ErrorText(ex));
            }
        }

        private void Refresh_Click(object sender, EventArgs e) => RefreshResults();

        private void Internal_Click(object sender, EventArgs e)
        {
            if (starting || !btnInternal.Enabled) return;
            RunPractice(() => dt312_PracticeBUS.Instance.StartInternal(TPConfigs.LoginUser.Id));
        }

        private void External_Click(object sender, EventArgs e)
        {
            if (starting || !btnExternal.Enabled) return;
            using (var candidate = new f312_ExternalCandidate())
            {
                if (candidate.ShowDialog(FindForm()) != DialogResult.OK) return;
                RunPractice(() => dt312_PracticeBUS.Instance.StartExternal(TPConfigs.LoginUser.Id,
                    candidate.EmployeeCode, candidate.EmployeeName, candidate.Department));
            }
        }

        private void RunPractice(Func<int> start)
        {
            starting = true;
            btnInternal.Enabled = btnExternal.Enabled = false;
            try
            {
                string operatorId = TPConfigs.LoginUser.Id;
                int attemptId = start(); // Persist candidate + attempt before entering the exam.
                using (var exam = new f312_DoExam(true))
                {
                    exam.SavePracticeResult = (score, data) => dt312_PracticeBUS.Instance.Submit(operatorId, attemptId, score, data);
                    exam.ShowDialog(FindForm());
                }
            }
            catch (Exception ex) { MsgTP.MsgError("無法開始模擬測驗：\r\n" + Exam312Ui.ErrorText(ex)); }
            finally
            {
                starting = false;
                RefreshResults();
            }
        }
    }
}
