using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_SettingExam : DevExpress.XtraEditors.XtraForm
    {
        public f312_SettingExam()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!ValidateChildren() || new[] { txbCountEmp, txbTime, txbPassScore, txbTotalQues, choicePercentEdit, essayPercentEdit }
                    .Any(edit => string.IsNullOrWhiteSpace(edit.Text) || edit.Value != decimal.Truncate(edit.Value)))
                    throw new ArgumentException("請填寫所有欄位，並使用整數。");
                if (choicePercentEdit.Value + essayPercentEdit.Value != 100)
                    throw new ArgumentException("選擇題與申論題比例合計必須為100%。");
                var existing = dt312_SettingBUS.Instance.GetItemById(1);
                dt312_Setting settingData = new dt312_Setting()
                {
                    id = 1,
                    CountEmp = Convert.ToInt32(txbCountEmp.Value),
                    PassingScore = Convert.ToInt32(txbPassScore.Value),
                    TestDuration = Convert.ToInt32(txbTime.Value),
                    QuesCount = Convert.ToInt32(txbTotalQues.Value),
                    ChoicePercent = Convert.ToInt32(choicePercentEdit.Value),
                    ChoiceScorePercent = null,
                    CycleCreate = existing?.CycleCreate ?? 1
                };
                if (!dt312_SettingBUS.Instance.AddOrUpdate(settingData))
                    throw new InvalidOperationException("無法儲存考試設定，請重試。");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex) { MsgTP.MsgError(ex.Message); }
            catch (InvalidOperationException ex) { MsgTP.MsgError(ex.Message); }
            catch (Exception ex) { MsgTP.MsgError(Exam312Ui.ErrorText(ex)); }
        }

        private bool syncingRatio;
        private void ChoicePercentChanged(object sender, EventArgs e) { SyncRatio(true); }
        private void EssayPercentChanged(object sender, EventArgs e) { SyncRatio(false); }
        private void SyncRatio(bool choiceChanged)
        {
            if (syncingRatio) return;
            syncingRatio = true;
            try
            {
                if (choiceChanged) essayPercentEdit.EditValue = 100m - choicePercentEdit.Value;
                else choicePercentEdit.EditValue = 100m - essayPercentEdit.Value;
            }
            finally { syncingRatio = false; }
            UpdateRatioPreview();
        }

        private void UpdateRatioPreview()
        {
            int total = Convert.ToInt32(txbTotalQues.Value);
            if (total <= 0) { ratioPreview.Text = "請輸入題目數量。"; return; }
            int choice = dt312_SettingBUS.ChoiceCount(total, Convert.ToInt32(choicePercentEdit.Value));
            ratioPreview.Text = $"出題：選擇題 {choice} 題 ／ 申論題 {total - choice} 題\r\n選擇題四捨五入，申論題補足總題數。";
            var maxima = dt312_ExamScoring.AllocateEqualPoints(Enumerable.Range(0, total).Select(i => i >= choice).ToArray());
            choiceScoreEdit.Value = maxima.Take(choice).Sum();
            essayScoreEdit.Value = 100m - choiceScoreEdit.Value;
            ratioPreview.Text += "\r\n配分：選擇題 " + maxima.Take(choice).Sum().ToString("0.##") + " 分 ／ 申論題 " + maxima.Skip(choice).Sum().ToString("0.##")
                + " 分\r\n每題滿分約 " + (100m / total).ToString("0.##") + " 分；配分依題數自動計算。\r\n每題配分最多兩位小數，尾差分配至個別題目。";
        }

        private void f312_SettingExam_Load(object sender, EventArgs e)
        {
            var dataInfo = dt312_SettingBUS.Instance.GetItemById(1);

            if (dataInfo != null)
            {
                txbTime.EditValue = dataInfo.TestDuration;
                txbPassScore.EditValue = dataInfo.PassingScore;
                txbTotalQues.EditValue = dataInfo.QuesCount;
                txbCountEmp.EditValue = dataInfo.CountEmp;
                choicePercentEdit.EditValue = dataInfo.ChoicePercent;
            }
            SyncRatio(true);
        }
    }
}
