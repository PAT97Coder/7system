using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.IO;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    public partial class f318_Settings : XtraForm
    {
        private readonly string settingsFile;

        public f318_Settings() : this(null) { }

        internal f318_Settings(string settingsFile)
        {
            this.settingsFile = settingsFile;
            InitializeComponent();
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            var paths = Task318WorkflowSettings.LoadPaths(settingsFile);
            txbSource7810.Text = paths.Source7810;
            txbSource7820.Text = paths.Source7820;
            txbCommonRoot.Text = paths.CommonRoot;
        }

        private void SelectFolder(ButtonEdit editor, string caption, bool allowCreate)
        {
            using (var dialog = new FolderBrowserDialog
            {
                Description = caption,
                SelectedPath = Directory.Exists(editor.Text.Trim()) ? editor.Text.Trim() : "",
                ShowNewFolderButton = allowCreate
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK) editor.Text = dialog.SelectedPath;
            }
        }

        private void txbSource7810_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            => SelectFolder(txbSource7810, "7810 下載路徑", false);

        private void txbSource7820_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            => SelectFolder(txbSource7820, "7820 下載路徑", false);

        private void txbCommonRoot_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
            => SelectFolder(txbCommonRoot, "建立檔案路徑", true);

        private void SaveSettings()
        {
            Task318WorkflowSettings.SavePaths(new Task318PathSettings
            {
                Source7810 = txbSource7810.Text,
                Source7820 = txbSource7820.Text,
                CommonRoot = txbCommonRoot.Text
            }, settingsFile);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { SaveSettings(); }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}
