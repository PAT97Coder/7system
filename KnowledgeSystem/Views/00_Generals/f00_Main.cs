using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_Main : DevExpress.XtraEditors.XtraForm
    {
        public f00_Main()
        {
            InitializeComponent();
        }

        TileItemElement elementName = new TileItemElement();
        TileItemElement elementIdDept = new TileItemElement();
        TileItemElement elementGrade = new TileItemElement();
        TileItemElement elementClass = new TileItemElement();

        private void InitializeControl()
        {
            elementName.Appearance.Normal.Font = new Font("DFKai-SB", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementName.Appearance.Normal.Options.UseFont = true;
            elementName.TextAlignment = TileItemContentAlignment.TopLeft;

            elementIdDept.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementIdDept.Appearance.Normal.Options.UseFont = true;
            elementIdDept.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementIdDept.TextLocation = new Point(5, -40);

            elementGrade.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementGrade.Appearance.Normal.Options.UseFont = true;
            elementGrade.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementGrade.TextLocation = new Point(5, -20);

            elementClass.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementClass.Appearance.Normal.Options.UseFont = true;
            elementClass.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementClass.TextLocation = new Point(5, 0);

            tileInfoUser.Elements.Add(elementName);
            tileInfoUser.Elements.Add(elementIdDept);
            tileInfoUser.Elements.Add(elementGrade);
            tileInfoUser.Elements.Add(elementClass);

            var lsFuncs = dm_FunctionBUS.Instance.GetList();
            btnSysAdmin.Text = lsFuncs.First(r => r.Id == AppPermission.SysAdmin).DisplayName;
            btnMod.Text = lsFuncs.First(r => r.Id == AppPermission.Mod).DisplayName;
            //btnKnowHow.Text = lsFuncs.First(r => r.Id == AppPermission.KnowledgeMain).DisplayName;
            //btnSafetyCert.Text = lsFuncs.First(r => r.Id == AppPermission.SafetyCertMain).DisplayName;
        }

        private void GetUserLogin()
        {
            f00_Login frm = new f00_Login();
            frm.ShowDialog();

            if (!TPConfigs.LoginSuccessful)
            {
                Close();
                return;
            }

            WindowState = FormWindowState.Maximized;
            Size = new Size(1000, 600);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                           (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);

            var _userLogin = dm_UserBUS.Instance.GetItemById(TPConfigs.LoginUser.Id);
            string userName = _userLogin.DisplayName;
            string idDept = _userLogin.IdDepartment;
            var gradeName = dm_DeptBUS.Instance.GetItemById(idDept.Substring(0, 2)).DisplayName;
            var gradeClass = dm_DeptBUS.Instance.GetItemById(idDept).DisplayName;
            elementName.Text = userName;
            elementIdDept.Text = idDept;
            elementGrade.Text = gradeName;
            elementClass.Text = gradeClass;
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            BackgroundImage = TPSvgimages.Background;

#if DEBUG
            // Không cần check update khi debug
#else
            using (WebClient client = new WebClient())
            {
                string result = client.DownloadString(TPConfigs.UrlUpdate);

                var lsUpdateInfos = JsonConvert.DeserializeObject<List<UpdateInfo>>(result)
                    .Where(r => r.app == TPConfigs.SoftNameEN).ToList();
                if (lsUpdateInfos != null && lsUpdateInfos.Count > 0)
                {
                    UpdateInfo newUpdate = lsUpdateInfos.First();
                    if (newUpdate.version != AppCopyRight.version)
                    {
                        var dialogResult = MsgTP.MsgUpdateSoftware();
                        if (dialogResult == DialogResult.OK)
                        {
                            f00_UpdateSoftware f00_Update = new f00_UpdateSoftware(newUpdate.url);
                            f00_Update.ShowDialog();
                        }

                        Close();
                    }
                }
            }
#endif

            GetUserLogin();

            try
            {
                // Xoá thư mục tạm nơi lưu các file tải về để xem
                Directory.Delete(TPConfigs.TempFolderData, true);
            }
            catch { }

            Text = TPConfigs.SoftNameTW + AppCopyRight.CopyRightString();
            lbSoftName.Text = TPConfigs.SoftNameTW;

            InitializeControl();
        }

        private void fMain_Shown(object sender, EventArgs e)
        {

        }

        private void ShowFromByFrame(int IdForm_, TileItemEventArgs e)
        {
            // Check xem thời gian đăng nhập có quá lâu không, nếu quá lâu thì yêu cầu login lại
            TimeSpan loginTimeout = TimeSpan.FromDays(1);
            DateTime now = DateTime.Now;

#if DEBUG
#else
            if (TPConfigs.LoginUser.LastUpdate <= now - loginTimeout)
            {
                XtraMessageBox.Show("登入已逾時，請重新開啟應用程式以繼續！", "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }
#endif

            bool IsGranted = AppPermission.Instance.CheckAppPermission(IdForm_);
            if (!IsGranted)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            TPConfigs.IdParentControl = IdForm_;
            f00_FluentFrame formShow = new f00_FluentFrame(IdForm_);
            formShow.Text = FormatFrameTitle(e.Item.Text);
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private static string FormatFrameTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            return title
                .Replace("\r\n", " - ")
                .Replace("\n", " - ")
                .Replace("\r", " - ")
                .Trim();
        }

        private void btnKnowHow_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.KnowledgeMain, e);
        }

        private void btnUserManage_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.Mod, e);
        }

        private void btnRoleManage_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SysAdmin, e);
        }

        private void btnSafetyCert_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SafetyCertMain, e);
        }

        private void btnWorkManagement_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.WorkManagementMain, e);
        }

        private bool logoutDialogOpen;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape && TPConfigs.LoginUser != null)
            {
                ConfirmLogout();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void tileInfoUser_ItemClick(object sender, TileItemEventArgs e)
        {
            ConfirmLogout();
        }

        private void ConfirmLogout()
        {
            if (logoutDialogOpen || TPConfigs.LoginUser == null) return;
            logoutDialogOpen = true;
            try
            {
                using (var dialog = new f00_ConfirmLogout(TPConfigs.LoginUser))
                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                AppPermission.Instance.Dispose();
                GetUserLogin();
            }
            finally
            {
                logoutDialogOpen = false;
            }
        }

        private void btnJFEnCSC_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.JFEnCSCMain, e);
        }

        private void btnISOAuditDocs_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.ISOAuditDocsMain, e);
        }

        private void btnTechnicalProjects_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.TechnicalPrjMain, e);
        }

        private void btnSignature_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SignatureDigital, e);
        }

        private void btnQuiz_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.QuizMain, e);
        }

        private void btnExtensions_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.Extensions, e);
        }

        private void btnSoftwareManual_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SoftManual, e);
        }

        private void btnContractMgmt_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.ContractMgmt, e);
        }

        private void btnSparePart_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.SparePart, e);
        }

        private void btnInternationalStd_ItemClick(object sender, TileItemEventArgs e)
        {
            ShowFromByFrame(AppPermission.InternationalStd, e);
        }
    }
}
