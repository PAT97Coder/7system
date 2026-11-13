using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using Spire.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._09_ISO14001Docs
{
    public partial class f209_SignDoc_Info : DevExpress.XtraEditors.XtraForm
    {
        public f209_SignDoc_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        string idDept2word = (TPConfigs.LoginUser.IdDepartment ?? "").Length >= 2
            ? TPConfigs.LoginUser.IdDepartment.Substring(0, 2) : TPConfigs.LoginUser.IdDepartment ?? "";

        int idRoleConfirm = -1;
        string nextStepProg = "";

        List<dm_JobTitle> jobTitles;
        List<dt209_Role> roleConfirms;

        List<dt209_ProgInfo> progInfos;
        List<dt209_Progress> progress;

        BindingSource sourceAtts = new BindingSource();
        Attachment attachment;

        DateTime minTimeRespValue = default;

        private class Attachment : dm_Attachment
        {
            public bool IsCancel { get; set; }
            public string Desc { get; set; }
            public dm_Attachment BaseAtt { get; set; }
        }

        bool IsLastStep = false;
        bool canProcessWorkflow;

        private void BlockWorkflow(string message)
        {
            canProcessWorkflow = false;
            idRoleConfirm = -1;
            nextStepProg = "";
            btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnApproval.Enabled = btnConfirm.Enabled = btnCancel.Enabled = false;
            gcDocs.Enabled = false;
            Text = $"待簽文件 | {message}";
        }

        private static int ResolveNextStepIndex(List<dt209_Progress> steps, List<dt209_ProgInfo> history)
        {
            if (steps == null || steps.Count == 0 || history == null) return -1;
            var responses = history.Where(r => r.IdUsr != "VNW0000000")
                .OrderBy(r => r.RespTime).ThenBy(r => r.Id).ToList();
            if (responses.Count > steps.Count) return -1;
            for (int index = 0; index < responses.Count; index++)
            {
                if (responses[index].IdUsr != steps[index].IdUsr) return -1;
            }
            return responses.Count;
        }

        private bool CanProcessCurrentStep()
        {
            if (!canProcessWorkflow) return false;
            var form = dt209_FormsBUS.Instance.GetItemById(idBase);
            var steps = dt209_ProgressBUS.Instance.GetListByIdBase(idBase).OrderBy(r => r.Id).ToList();
            var history = dt209_ProgInfoBUS.Instance.GetListByIdForm(idBase);
            int nextIndex = ResolveNextStepIndex(steps, history);
            if (form == null || form.IsDel == true || form.IsCancel == true || form.IsProcessing != true ||
                nextIndex < 0 || nextIndex >= steps.Count ||
                steps[nextIndex].IdUsr != TPConfigs.LoginUser.Id ||
                steps[nextIndex].IdRole != idRoleConfirm || form.NextStepProg != TPConfigs.LoginUser.Id)
            {
                BlockWorkflow("簽核狀態已變更，請關閉後重新開啟。");
                return false;
            }
            IsLastStep = nextIndex == steps.Count - 1;
            nextStepProg = IsLastStep ? "" : steps[nextIndex + 1].IdUsr;
            return true;
        }

        private void InitializeIcon()
        {
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;
            btnApproval.ImageOptions.SvgImage = TPSvgimages.EmailSend;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.EmailSend;
        }

        private void f209_SignDoc_Info_Load(object sender, EventArgs e)
        {
            BlockWorkflow("正在載入簽核流程…");
            var baseData = dt209_FormsBUS.Instance.GetItemById(idBase);
            if (baseData == null || baseData.IsDel == true)
            {
                BlockWorkflow("文件不存在或已刪除。");
                return;
            }
            minTimeRespValue = baseData.UploadTime; // Lấy thời gian đưa lên làm mốc để ký

            Text = $"待簽文件 | {baseData.Code} | {baseData.DisplayName}";
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roleConfirms = dt209_RoleBUS.Instance.GetList();

            var users = dm_UserBUS.Instance.GetList();
            progress = dt209_ProgressBUS.Instance.GetListByIdBase(idBase).OrderBy(r => r.Id).ToList();
            progInfos = dt209_ProgInfoBUS.Instance.GetListByIdForm(idBase).Where(r => r.IdUsr != "VNW0000000").ToList();
            if (baseData.IsCancel == true || baseData.IsProcessing != true)
            {
                BlockWorkflow(baseData.IsCancel == true ? "文件已退回，無法繼續簽核。" : "文件已結束簽核，無待簽步驟。");
                return;
            }
            int nextStepIndex = ResolveNextStepIndex(progress, progInfos);
            if (nextStepIndex < 0)
            {
                BlockWorkflow("簽核流程或歷史資料不完整，請聯絡管理員。");
                return;
            }
            if (nextStepIndex >= progress.Count)
            {
                BlockWorkflow("所有步驟均已簽核，無待簽步驟。");
                return;
            }
            if (progress.Any(step => !users.Any(user => user.Id == step.IdUsr) ||
                !roleConfirms.Any(role => role.Id == step.IdRole && (role.Id == 1 || role.Id == 2))))
            {
                BlockWorkflow("簽核人員或角色不存在，請聯絡管理員。");
                return;
            }

            var progressInfo = (from data in progress
                                join usr in users on data.IdUsr equals usr.Id
                                select new { data, usr }).ToList();

            // Thêm danh sách các bước vào StepProgressBar
            foreach (var item in progressInfo)
            {
                var barItem = new StepProgressBarItem();
                barItem.ContentBlock1.Caption = $"{item.usr.IdDepartment} {item.usr.DisplayName}";
                barItem.ContentBlock1.Description = $"{item.usr.Id}\r\n{jobTitles.FirstOrDefault(r => r.Id == item.usr.JobCode)?.DisplayName}";
                barItem.ContentBlock2.Caption = roleConfirms.FirstOrDefault(r => r.Id == item.data.IdRole)?.DisplayName;
                stepProgressDoc.Items.Add(barItem);
            }
            stepProgressDoc.ItemOptions.Indicator.Width = 40;

            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();
            minTimeRespValue = progNow?.RespTime ?? minTimeRespValue;

            int stepNow = nextStepIndex - 1;
            stepProgressDoc.SelectedItemIndex = stepNow; // Focus đến bước hiện tại

            var nextStepUsr = progress[nextStepIndex].IdUsr;
            nextStepProg = nextStepIndex + 1 >= progress.Count ? "" : progress[nextStepIndex + 1].IdUsr;

            IsLastStep = nextStepIndex == progress.Count - 1;

            // Thêm lịch sử trình ký vào gridProcess
            var lsHistoryProcess = (from data in progInfos
                                    join usr in users on data.IdUsr equals usr.Id
                                    join job in jobTitles on usr.JobCode equals job.Id
                                    select new
                                    {
                                        data,
                                        usr,
                                        job,
                                        DisplayName = $"{usr.Id} LG{usr.IdDepartment}/{usr.DisplayName}"
                                    }).ToList();

            gcHistoryProcess.DataSource = lsHistoryProcess;

            gvHistoryProcess.ReadOnlyGridView();

            idRoleConfirm = progress[nextStepIndex].IdRole;

            btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            // Thêm các file vào gvDocs
            attachment = new Attachment() { BaseAtt = dm_AttachmentBUS.Instance.GetItemById(baseData.AttId ?? -1) };
            if (attachment.BaseAtt == null || string.IsNullOrWhiteSpace(attachment.BaseAtt.EncryptionName))
            {
                BlockWorkflow("附件資料不完整，無法簽核。");
                return;
            }
            sourceAtts.DataSource = attachment;
            gcDocs.DataSource = sourceAtts;
            gvDocs.ReadOnlyGridView();

            if (nextStepUsr == TPConfigs.LoginUser.Id && baseData.NextStepProg == nextStepUsr)
            {
                canProcessWorkflow = true;
                gcDocs.Enabled = true;
                btnApproval.Enabled = btnConfirm.Enabled = btnCancel.Enabled = true;
                switch (idRoleConfirm)
                {
                    case 1:
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;
                    case 2:
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;
                }
            }
            else
            {
                BlockWorkflow(baseData.NextStepProg == nextStepUsr
                    ? "尚未輪到您簽核，無法操作。" : "待簽人員與流程不一致，請聯絡管理員。");
            }
        }

        private void gvDocs_DoubleClick(object sender, EventArgs e)
        {
            if (!CanProcessCurrentStep()) return;
            GridView view = sender as GridView;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            string fileName = view.GetRowCellValue(view.FocusedRowHandle, gColEncryptName).ToString();

            string sourceFolder = Path.Combine(TPConfigs.Folder209, attachment.BaseAtt.Id.ToString());
            string sourcePath = Path.Combine(sourceFolder, fileName);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            if (!File.Exists(sourcePath))
            {
                BlockWorkflow("附件檔案不存在，請聯絡管理員。");
                return;
            }

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            switch (idRoleConfirm)
            {
                case 1:
                    f00_PdfTools pdfTools = new f00_PdfTools(destPath, sourceFolder);
                    pdfTools.ShowDialog();

                    // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
                    string encrytFileName = pdfTools.OutFileName;
                    string describe = pdfTools.Describe;

                    if (string.IsNullOrEmpty(encrytFileName))
                    {
                        attachment.IsCancel = true;
                        attachment.Desc = describe;
                        attachment.BaseAtt.EncryptionName = null;
                    }
                    else
                    {
                        attachment.Desc = "已簽名";
                        attachment.BaseAtt.EncryptionName = encrytFileName;
                    }

                    gvDocs.RefreshData();

                    // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
                    bool CanConfirm = attachment.BaseAtt.EncryptionName != null;
                    btnApproval.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
                case 2:
                    f00_VIewFile fView = new f00_VIewFile(destPath, false, true);
                    fView.ShowDialog();

                    // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
                    bool IsConfirm = fView.IsConfirm;
                    describe = fView.Describe;

                    if (IsConfirm != true) return;// Kiểm tra xem đã xác nhận hay chưa

                    if (string.IsNullOrEmpty(describe))
                    {
                        attachment.Desc = "已確認";
                    }
                    else
                    {
                        attachment.Desc = describe;
                        attachment.IsCancel = true;
                        attachment.BaseAtt.EncryptionName = null;
                    }

                    gvDocs.RefreshData();

                    // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
                    CanConfirm = attachment.IsCancel != true;
                    btnConfirm.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
            }
        }

        private void btnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!CanProcessCurrentStep()) return;
            // 1. Kiểm tra mô tả đính kèm
            if (string.IsNullOrEmpty(attachment.Desc))
            {
                ShowMessage("請處理所有文件！");
                return;
            }

            // 2. Tạo TextEdit với định dạng giờ
            var editor = new TextEdit
            {
                Font = new Font("Microsoft JhengHei UI", 14F)
            };
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
            editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd HH:mm:ss");
            editor.Properties.MaskSettings.Set("useAdvancingCaret", true);
            editor.Properties.UseMaskAsDisplayFormat = true;

            // 3. Hiển thị hộp nhập giờ phản hồi
            string defaultTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>輸入核准時間</font>",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = defaultTime
            });

            if (string.IsNullOrWhiteSpace(result?.ToString())) return;

            // 4. Xử lý kết quả nhập
            if (!DateTime.TryParse(result.ToString(), out DateTime respTime))
            {
                ShowMessage($"{result}\r\n時間格式不正確，請重新輸入！");
                return;
            }

            if (!DateTimeHelper.IsWithinWorkingHours(respTime))
            {
                ShowMessage("請選擇工作時間內的時間！");
                return;
            }

            if (minTimeRespValue == DateTime.MinValue || respTime < minTimeRespValue)
            {
                ShowMessage("簽署時間無效！");
                return;
            }

            if (MsgTP.MsgYesNoQuestion($"文件將於<color=red>{respTime:yyyy/MM/dd HH:mm}</color>簽署") != DialogResult.Yes)
                return;

            // 5. Nếu là bước cuối → cập nhật SendNoteTime
            if (IsLastStep)
            {
                var progInfoSendNote = dt209_ProgInfoBUS.Instance.GetListByIdForm(idBase)
                    .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString()))
                    .ToList();

                foreach (var item in progInfoSendNote)
                {
                    item.SendNoteTime = DateTime.Now;
                    dt209_ProgInfoBUS.Instance.AddOrUpdate(item);
                }
            }

            // 6. Tạo dòng phản hồi
            var progInfo = new dt209_ProgInfo
            {
                IdForm = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = respTime,
                Desc = "簽名"
            };
            dt209_ProgInfoBUS.Instance.Add(progInfo);

            // 7. Cập nhật form chính
            var form = dt209_FormsBUS.Instance.GetItemById(idBase);
            form.NextStepProg = nextStepProg;

            // 8. Cập nhật file đính kèm
            var baseAtt = attachment.BaseAtt;
            dm_AttachmentBUS.Instance.AddOrUpdate(baseAtt);

            // 9. Nếu là bước cuối → ngưng xử lý và copy file
            if (IsLastStep)
            {
                form.IsProcessing = false;

                string sourceFile = Path.Combine(TPConfigs.Folder209, baseAtt.Id.ToString(), baseAtt.EncryptionName);
                string destFile = Path.Combine(TPConfigs.Folder209, baseAtt.EncryptionName);
                File.Copy(sourceFile, destFile, true);
            }

            // 10. Lưu form
            dt209_FormsBUS.Instance.AddOrUpdate(form);

            // 11. Đóng form
            Close();
        }

        void ShowMessage(string message)
        {
            MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{message}</font>");
        }


        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!CanProcessCurrentStep()) return;
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            string describe = result?.ToString() ?? "";

            var baseData = dt209_FormsBUS.Instance.GetItemById(idBase);
            baseData.IsProcessing = false;
            baseData.NextStepProg = "";
            baseData.IsCancel = true;
            baseData.Descript = $"已由{TPConfigs.LoginUser.DisplayName}退回，退回說明：{describe}";

            dt209_FormsBUS.Instance.AddOrUpdate(baseData);

            // Các bước trước nếu chưa gửi note thì khỏi gửi luôn
            var progInfoSendNote = dt209_ProgInfoBUS.Instance.GetListByIdForm(idBase)
                .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString())).ToList();
            foreach (var item in progInfoSendNote)
            {
                item.SendNoteTime = DateTime.Now;
                dt209_ProgInfoBUS.Instance.AddOrUpdate(item);
            }

            // Gửi bước cuối cùng
            dt209_ProgInfo info = new dt209_ProgInfo()
            {
                IdForm = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Desc = "退回"
            };

            dt209_ProgInfoBUS.Instance.Add(info);

            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!CanProcessCurrentStep()) return;
            // kiểm tra xem đã xử lý hết các file chưa
            bool validate = string.IsNullOrEmpty(attachment.Desc);
            if (validate)
            {
                string msg = "請處理所有文件！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                return;
            }

            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            // Thiết lập mask để buộc nhập đúng định dạng
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
            editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd HH:mm:ss");
            editor.Properties.MaskSettings.Set("useAdvancingCaret", true);
            editor.Properties.UseMaskAsDisplayFormat = true;

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>輸入核准時間</font>",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") // Định dạng mặc định
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;

            // Xử lý kết quả nhập
            if (!DateTime.TryParse(result.ToString(), out DateTime respTime))
            {
                ShowMessage($"{result}\r\n時間格式不正確，請重新輸入！");
                return;
            }

            if (!DateTimeHelper.IsWithinWorkingHours(respTime))
            {
                string msg = "請選擇工作時間內的時間！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
            }

            if (respTime < minTimeRespValue)
            {
                string msg = "簽署時間無效！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                return;
            }

            if (MsgTP.MsgYesNoQuestion($"文件將於<color=red>{respTime:yyyy/MM/dd HH:mm}</color>簽署") != DialogResult.Yes)
                return;

            // Các bước trước nếu chưa gửi note thì khỏi gửi luôn
            if (IsLastStep)
            {
                var progInfoSendNote = dt209_ProgInfoBUS.Instance.GetListByIdForm(idBase)
                    .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString())).ToList();
                foreach (var item in progInfoSendNote)
                {
                    item.SendNoteTime = DateTime.Now;
                    dt209_ProgInfoBUS.Instance.AddOrUpdate(item);
                }
            }

            dt209_ProgInfo info = new dt209_ProgInfo()
            {
                IdForm = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = respTime,
                Desc = "確認"
            };

            dt209_ProgInfoBUS.Instance.Add(info);

            var baseData = dt209_FormsBUS.Instance.GetItemById(idBase);
            baseData.NextStepProg = nextStepProg;

            if (IsLastStep)
            {
                baseData.IsProcessing = false;

                var att = attachment.BaseAtt;
                string sourceFile = Path.Combine(TPConfigs.Folder209, att.Id.ToString(), att.EncryptionName);
                string destFile = Path.Combine(TPConfigs.Folder209, att.EncryptionName);

                File.Copy(sourceFile, destFile, true);
            }

            dt209_FormsBUS.Instance.AddOrUpdate(baseData);

            Close();
        }
    }
}
