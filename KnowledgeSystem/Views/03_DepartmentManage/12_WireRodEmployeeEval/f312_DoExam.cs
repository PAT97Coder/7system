using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Native;
using DevExpress.Data.Helpers;
using DevExpress.DataAccess.Native.Sql.SqlParser;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_DoExam : DevExpress.XtraEditors.XtraForm
    {
        public f312_DoExam(bool practice = false)
        {
            InitializeComponent();
            IsPractice = practice;
            Text = practice ? "模擬測驗" : "正式考試";
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ khung viền
            this.WindowState = FormWindowState.Maximized; // Phóng to toàn màn hình
            this.TopMost = !practice; // Đảm bảo form luôn ở trên cùng

            this.KeyDown += F312_DoExam_KeyDown;
            this.KeyPreview = true;  // Đảm bảo Form nhận được sự kiện KeyDown
        }

        public bool IsPractice { get; }
        private bool submitting;
        private void SaveCurrentAnswer()
        {
            if (indexQues < 0 || indexQues >= examResults.Count) return;
            examResults[indexQues].UserAnswer = examResults[indexQues].Questions.IsEssay ? essayAnswer.Text
                : string.Join(",", txbUserAns.GetTokenList().Select(r => r.Description as string));
            gcData.RefreshDataSource();
        }

        public class ExamResult
        {
            public dt312_Questions Questions { get; set; }
            public List<dt312_Answers> Answers { get; set; }
            public int QuestionIndex { get; set; }
            public string CorrectAnswer { get; set; }
            public string UserAnswer { get; set; }
            public bool IsCorrect { get; set; }
            public bool IsMultiChoice { get; set; }
        }

        private Timer countdownTimer;
        private TimeSpan timeRemaining;

        private bool IsClose = false;

        public int idExamUser = -1;
        int testDuration = 0, passingScore = 0, numQues = 0, indexQues = 0;

        List<dt312_Questions> ques = new List<dt312_Questions>();
        List<dt312_Answers> answers = new List<dt312_Answers>();
        List<ExamResult> examResults = new List<ExamResult>();

        BindingSource sourceResult = new BindingSource();

        public class GroupDistribution
        {
            public int GroupId { get; set; }
            public int MaxAvailable { get; set; } // Tổng số câu hỏi nhóm đang có
            public int CountToTake { get; set; }  // Số câu hỏi thuật toán quyết định sẽ lấy
        }

        private void NextQues()
        {
            SaveCurrentAnswer();

            if (indexQues >= ques.Count - 1) return;

            indexQues++;
            InitializeWebView2(indexQues);
        }

        private void PreviousQues()
        {
            SaveCurrentAnswer();

            if (indexQues <= 0) return;

            indexQues--;
            InitializeWebView2(indexQues);
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = gvData;
            if (view.FocusedRowHandle < 0 || examResults.Count == 0) return;
            SaveCurrentAnswer();
            indexQues = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString()) - 1;

            InitializeWebView2(indexQues);
        }

        private void btnPreviousQues_Click(object sender, EventArgs e)
        {
            PreviousQues();
        }

        private void btnNextQues_Click(object sender, EventArgs e)
        {
            NextQues();
        }

        private void F312_DoExam_KeyDown(object sender, KeyEventArgs e)
        {
            if (essayAnswer.ContainsFocus) return;
            if (e.KeyCode == Keys.Right)
            {
                NextQues();
            }
            else if (e.KeyCode == Keys.Left)
            {
                PreviousQues();
            }

            if (e.Control && e.Alt && e.Shift && e.KeyCode == Keys.Z)
            {
                var groups = dm_GroupBUS.Instance.GetItemByName("評鑒係統");
                if (groups == null) return;

                var grpUsers = dm_GroupUserBUS.Instance.GetListByIdGroup(groups.Id);
                if (!grpUsers.Any(r => r.IdUser == TPConfigs.LoginUser.Id))
                    return;

                foreach (var answer in examResults)
                    answer.UserAnswer = answer.CorrectAnswer;

                gcData.RefreshDataSource();

                GridView view = gvData;
                indexQues = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString()) - 1;

                InitializeWebView2(indexQues);
            }
        }

        private void InitializeWebView2(int index)
        {
            lbQuesNo.Text = $"題目：{index + 1}/{ques.Count}";
            bool essay = ques[index].IsEssay;
            answerGroup.Text = essay ? "申論作答（不計分）" : ques[index].IsmultiAns ? "複選作答（可選擇多個答案）" : "單選作答（請選擇一個答案）";
            answerRow.Height = essay ? 210 : 130;
            layoutControlItem6.Visibility = essay ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            essayAnswerItem.Visibility = essay ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            var options = answers.Where(a => a.QuesId == ques[index].Id).ToList();
            txbUserAns.Properties.DataSource = Enumerable.Range(1, options.Count).Select(n => n.ToString()).ToList();
            txbUserAns.EditValue = essay ? "" : examResults[index].UserAnswer;
            essayAnswer.Text = essay ? examResults[index].UserAnswer : "";
            webViewQues.DocumentText = f312_ExamReview.BuildQuestionHtml(ques[index], options, index + 1);
            gvData.FocusedRowHandle = index;
            if (essay) essayAnswer.Focus(); else txbUserAns.Focus();
        }

        private void f312_DoExam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsClose)
            {
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>請按<color=red>「提交」</color>結束考試</font>");
                e.Cancel = true;
            }
        }

        private void f312_DoExam_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                countdownTimer.Stop();
            }
            catch { }
        }

        private void Submit_Exam()
        {
            if (submitting || IsClose || examResults.Count == 0) return;
            submitting = true; countdownTimer?.Stop(); btnSubmit.Enabled = false;
            SaveCurrentAnswer();
            foreach (var answer in examResults)
                answer.IsCorrect = !answer.Questions.IsEssay && dt312_ExamScoring.IsCorrect(answer.CorrectAnswer, answer.UserAnswer);
            int graded = examResults.Count(r => !r.Questions.IsEssay);
            int correct = examResults.Count(r => r.IsCorrect);
            int? score = dt312_ExamScoring.Score(correct, graded);
            try
            {
                if (!IsPractice)
                {
                    var result = dt312_ExamUserBUS.Instance.GetItemById(idExamUser);
                    if (result == null || result.UserId != TPConfigs.LoginUser.Id || result.SubmitAt.HasValue)
                        throw new InvalidOperationException("考試不存在、已提交或不屬於目前使用者。");
                    result.IsPass = score.HasValue ? (bool?)(score.Value >= passingScore) : null;
                    result.Score = score; result.SubmitAt = DateTime.Now;
                    result.ExamData = JsonConvert.SerializeObject(examResults);
                    if (!dt312_ExamUserBUS.Instance.AddOrUpdate(result)) throw new InvalidOperationException("無法儲存考試結果，請重新提交。");
                }
                if (IsPractice)
                {
                    using (var review = new f312_ExamReview(examResults, score, true)) review.ShowDialog(this);
                }
                else
                    MsgTP.MsgShowInfomation(score.HasValue ? $"{(score.Value >= passingScore ? "考試通過" : "考試未通過")}；成績：{score}/100；選擇題答對 {correct}/{graded} 題。申論題不計分。"
                        : "已提交作答。本次全為申論題，不計分。");
                IsClose = true; Close();
            }
            catch (Exception ex) { MsgTP.MsgError("提交失敗：\r\n" + ex.Message); }
            finally
            {
                submitting = false; btnSubmit.Enabled = true;
                if (!IsClose && timeRemaining > TimeSpan.Zero) countdownTimer?.Start();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var dialog = XtraMessageBox.Show("你確定要提交考試答案嗎", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog != DialogResult.Yes) return;

            Submit_Exam();
        }

        public List<GroupDistribution> DistributeQuestions(List<GroupDistribution> groups, int totalNeeded)
        {
            // 1. Validate: Nếu tổng cần lấy lớn hơn tổng số có sẵn -> Lấy tất cả
            int totalAvailable = groups.Sum(g => g.MaxAvailable);
            if (totalNeeded >= totalAvailable)
            {
                groups.ForEach(g => g.CountToTake = g.MaxAvailable);
                return groups;
            }

            // Reset số lượng sẽ lấy về 0 trước khi tính
            groups.ForEach(g => g.CountToTake = 0);

            // Tạo danh sách các nhóm đang chờ xét duyệt (ban đầu là tất cả)
            var pendingGroups = groups.ToList();

            while (totalNeeded > 0 && pendingGroups.Count > 0)
            {
                // Tính trung bình (dùng double để so sánh chính xác)
                double avgNeeded = (double)totalNeeded / pendingGroups.Count;

                // Tìm các nhóm "nghèo" (không đủ cung cấp mức trung bình)
                var insufficientGroups = pendingGroups.Where(g => g.MaxAvailable < avgNeeded).ToList();

                if (insufficientGroups.Count > 0)
                {
                    // TRƯỜNG HỢP 1: CÓ NHÓM THIẾU
                    // Lấy hết "vốn liếng" của các nhóm này
                    foreach (var group in insufficientGroups)
                    {
                        group.CountToTake = group.MaxAvailable; // Lấy hết
                        totalNeeded -= group.CountToTake;       // Giảm tổng cần lấy
                        pendingGroups.Remove(group);            // Loại khỏi danh sách chia tiếp theo
                    }
                    // Vòng lặp sẽ quay lại bước 1 để tính lại trung bình mới cho các nhóm còn lại
                }
                else
                {
                    // TRƯỜNG HỢP 2: TẤT CẢ NHÓM CÒN LẠI ĐỀU ĐỦ (Hoặc dư)
                    int baseCount = totalNeeded / pendingGroups.Count; // Phần nguyên
                    int remainder = totalNeeded % pendingGroups.Count; // Phần dư

                    foreach (var group in pendingGroups)
                    {
                        group.CountToTake = baseCount;

                        // Chia phần dư cho các nhóm đầu tiên trong danh sách pending
                        if (remainder > 0)
                        {
                            group.CountToTake += 1;
                            remainder--;
                        }
                    }

                    // Đã chia xong hết
                    break;
                }
            }

            return groups;
        }

        private string LoadData()
        {
            if (IsPractice && !dt312_QuestionsBUS.Instance.CanPractice(TPConfigs.LoginUser.Id))
                return "您沒有模擬測驗的權限。";
            if (!IsPractice)
            {
                var assigned = dt312_ExamUserBUS.Instance.GetItemById(idExamUser);
                if (assigned == null || assigned.UserId != TPConfigs.LoginUser.Id || assigned.SubmitAt.HasValue)
                    return "此考試不存在、已提交或不屬於目前使用者。";
            }
            var setting = dt312_SettingBUS.Instance.GetItemById(1);

            testDuration = setting?.TestDuration ?? 0;
            passingScore = setting?.PassingScore ?? 0;
            numQues = setting?.QuesCount ?? 0;

            // --- PHẦN 1: LẤY DỮ LIỆU GỐC ---
            var dataQues = dt312_QuestionsBUS.Instance.GetList(); // Lấy tất cả câu hỏi
            var groupIsRemove = dt312_GroupsBUS.Instance.GetListIsRemove();

            dataQues = dataQues.Where(q => !groupIsRemove.Any(g => g.Id == q.GroupId)).ToList();
            if (testDuration <= 0 || numQues <= 0 || passingScore < 0 || passingScore > 100)
                return "請先設定有效的考試時間、題目數量及及格分數（0至100）。";
            if (dataQues.Count < numQues)
                return "題目數量不夠！";

            // Group và đếm số lượng (Code cũ của bạn)
            var quesCountsQuery = from data in dataQues
                                  group data by data.GroupId into dtg
                                  select new
                                  {
                                      IdGroup = dtg.Key,
                                      QuestionCount = dtg.Count()
                                  };

            // Loại các nhóm đã bị xóa

            quesCountsQuery = quesCountsQuery.Where(r => !groupIsRemove.Select(x => x.Id).ToList().Contains(r.IdGroup)).ToList();

            // --- PHẦN 2: CHUẨN BỊ DỮ LIỆU TÍNH TOÁN ---

            // Map sang object GroupDistribution
            var listForCalc = quesCountsQuery.Select(x => new GroupDistribution
            {
                GroupId = x.IdGroup,
                MaxAvailable = x.QuestionCount,
                CountToTake = 0
            }).ToList();

            // --- PHẦN 3: XỬ LÝ PHÂN BỔ (CODE MỚI THÊM VÀO) ---

            // BƯỚC 1: Trộn ngẫu nhiên thứ tự các nhóm 
            // (Để khi chia phần dư, nhóm nào được ưu tiên +1 sẽ là ngẫu nhiên)
            listForCalc = listForCalc.OrderBy(x => Guid.NewGuid()).ToList();

            // BƯỚC 2: Gọi hàm tính toán logic chia đều
            DistributeQuestions(listForCalc, numQues);

            // BƯỚC 3: Lấy danh sách câu hỏi thực tế dựa trên số lượng đã tính
            foreach (var groupInfo in listForCalc)
            {
                if (groupInfo.CountToTake > 0)
                {
                    // Lấy các câu hỏi thuộc nhóm này từ danh sách gốc dataQues
                    var questionsOfGroup = dataQues
                        .Where(q => q.GroupId == groupInfo.GroupId)
                        // Quan trọng: Trộn ngẫu nhiên câu hỏi TRONG nhóm để không luôn lấy các câu đầu tiên
                        .OrderBy(x => Guid.NewGuid())
                        .Take(groupInfo.CountToTake)
                        .ToList();

                    ques.AddRange(questionsOfGroup);
                }
            }

            ques = ques.OrderBy(x => Guid.NewGuid()).ToList();

            answers = dt312_AnswersBUS.Instance.GetListByListQues(ques.Select(r => r.Id).ToList())
                .OrderBy(r => r.Id).GroupBy(r => r.QuesId)
                .SelectMany(group => group.Select((answer, index) => new dt312_Answers
                {
                    Id = index + 1,
                    QuesId = answer.QuesId,
                    ImageName = answer.ImageName,
                    DisplayName = answer.DisplayName,
                    TrueAns = answer.TrueAns
                })).ToList();

            // Lấy danh sách câu trả lời đúng dưới dạng 1,2,3
            var correctAnsByIdQues = answers.Where(r => r.TrueAns).GroupBy(r => r.QuesId).Select(r => new
            {
                QuesId = r.Key,
                CorrectAnswer = string.Join(",", r.Select(g => g.Id.ToString()))
            }).ToList();

            if (ques.Any(q => !q.IsEssay && !correctAnsByIdQues.Any(a => a.QuesId == q.Id)))
                return "選擇題缺少正確答案，請聯絡題庫管理人員。";
            examResults = ques.Select((question, index) => new ExamResult
            {
                QuestionIndex = index + 1,
                CorrectAnswer = question.IsEssay ? "" : correctAnsByIdQues.First(a => a.QuesId == question.Id).CorrectAnswer,
                UserAnswer = "", Questions = question,
                Answers = answers.Where(a => a.QuesId == question.Id).ToList(),
                IsMultiChoice = question.IsmultiAns
            }).ToList();

            return "";
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            // Giảm thời gian còn lại đi 1 giây
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));

            // Cập nhật Label với thời gian còn lại
            lbTime.Text = $"剩餘時間 {(int)Math.Max(0, timeRemaining.TotalMinutes):00}:{Math.Max(0, timeRemaining.Seconds):00}";

            if (timeRemaining <= TimeSpan.FromMinutes(5))
            {
                lbTime.ForeColor = System.Drawing.Color.Red;
            }

            // Nếu hết thời gian thì dừng Timer
            if (timeRemaining <= TimeSpan.Zero)
            {
                countdownTimer.Stop();
                lbTime.Text = "時間到了！";

                Submit_Exam();
            }
        }

        private void f312_DoExam_Load(object sender, EventArgs e)
        {
            string result;
            try { result = LoadData(); }
            catch (Exception ex) { result = "無法載入題庫：" + ex.Message; }

            if (!string.IsNullOrEmpty(result))
            {
                IsClose = true;
                MsgTP.MsgError(result);
                Close();
                return;
            }

            string msg = $"<font='Microsoft JhengHei UI' size=14>點擊「<color=red>確定</color>」開始！</font>";
            MsgTP.MsgShowInfomation(msg);

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 giây
            countdownTimer.Tick += CountdownTimer_Tick;

            timeRemaining = TimeSpan.FromMinutes(testDuration);
            lbTime.Text = $"剩餘時間 {(int)Math.Max(0, timeRemaining.TotalMinutes):00}:{Math.Max(0, timeRemaining.Seconds):00}";
            countdownTimer.Start();

            txbUserAns.Focus();

            sourceResult.DataSource = examResults;
            gcData.DataSource = sourceResult;

            InitializeWebView2(indexQues);
        }
    }
}
