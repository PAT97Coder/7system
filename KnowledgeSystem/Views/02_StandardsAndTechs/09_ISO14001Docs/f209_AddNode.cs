using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._09_ISO14001Docs
{
    public partial class f209_AddNode : DevExpress.XtraEditors.XtraForm
    {
        public f209_AddNode()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public dt209_Base currentData = null;
        public dt209_Base parentData = null;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);


        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbDept.Enabled = parentData == null && _enable;
            ckPaperType.Enabled = parentData?.IsPaperType != true;
        }

        private void IniControl()
        {
            if (parentData != null)
            {
                ckPaperType.Checked = parentData.IsPaperType == true;
                cbbDept.EditValue = parentData.IdDept;
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    EnabledController(false);
                    break;
                default:
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f209_AddNode_Load(object sender, EventArgs e)
        {
            if (eventInfo == EventFormInfo.Update && !Iso209Permission.EnsureCanEditCurrentUser())
            {
                Close();
                return;
            }

            lcControls = new List<LayoutControlItem>() { lcDept, lcDocCode, lcDisplayName, lcDisplayNameVN, lcDocumentCategory, lcArticles, lcDocType, lcNotifyCycle };
            lcImpControls = new List<LayoutControlItem>() { lcDocCode, lcDisplayName, lcDisplayNameVN, lcNotifyCycle, lcDocumentCategory, lcDocType };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var grpUsrs = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var depts = dm_DeptBUS.Instance.GetList();
            var groups = dm_GroupBUS.Instance.GetListByName("ISO14001組");

            var cbbDepts = (from data in groups
                            join grp in grpUsrs on data.Id equals grp.IdGroup
                            select data.IdDept).ToList();

            cbbDept.Properties.Items.AddRange(cbbDepts);
            cbbDept.SelectedIndex = 0;

            cbbDocType.Properties.Items.AddRange((TPConfigs.DocTypes209 ?? "一階;二階;三階").Split(';').Concat(new[] { "四階" }).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList());

            IniControl();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    currentData = new dt209_Base();
                    break;
                case EventFormInfo.Update:


                    txbDocCode.EditValue = currentData.DocCode;
                    txbDisplayName.EditValue = currentData.DisplayName;
                    txbDisplayNameVN.EditValue = currentData.DisplayNameVN;
                    txbArticles.EditValue = currentData.Articles ?? "";
                    cbbDocumentCategory.EditValue = currentData.DocumentCategory;
                    cbbDept.EditValue = currentData.IdDept;
                    ckPaperType.Checked = currentData.IsPaperType == true;
                    txbNotifyCycle.EditValue = currentData.NotifyCycle;
                    cbbDocType.EditValue = currentData.DocType;

                    break;
                default:
                    break;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (eventInfo == EventFormInfo.Update && !Iso209Permission.EnsureCanEditCurrentUser()) return;

            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            foreach (var item in lcImpControls)
            {
                if (string.IsNullOrEmpty(item.Control.Text)) IsValidate = false;
            }

            if (!IsValidate)
            {
                XtraMessageBox.Show("請填寫所有重要資訊！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string displayNameVN = txbDisplayNameVN.Text.Trim();

            if (StringHelper.CheckUpcase(displayNameVN, 33) && displayNameVN.Length > 20)
            {
                XtraMessageBox.Show("請關閉大寫鎖定！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                currentData.Articles = txbArticles.Text.Trim();
                currentData.DocumentCategory = cbbDocumentCategory.Text;

                currentData.DocCode = txbDocCode.Text;
                currentData.DisplayName = txbDisplayName.Text.Trim();
                currentData.DisplayNameVN = displayNameVN;
                currentData.IdDept = cbbDept.EditValue.ToString();
                currentData.IsPaperType = ckPaperType.Checked;
                currentData.NotifyCycle = Convert.ToInt16(txbNotifyCycle.EditValue?.ToString() ?? "0");
                currentData.DocType = cbbDocType.Text;

                msg = $"{currentData.DocCode} {currentData.DisplayName}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        currentData.IdParent = parentData?.Id ?? -1;

                        result = dt209_BaseBUS.Instance.Add(currentData) > 0;
                        break;
                    case EventFormInfo.Update:
                        result = dt209_BaseBUS.Instance.AddOrUpdate(currentData);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

    }
}
