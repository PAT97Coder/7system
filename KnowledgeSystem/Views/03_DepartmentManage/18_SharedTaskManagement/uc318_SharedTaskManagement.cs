using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    public partial class uc318_SharedTaskManagement : XtraUserControl
    {
        private readonly BindingList<Task318Draft> tasks = new BindingList<Task318Draft>();
        private readonly BindingSource sourceTasks = new BindingSource();
        private DXMenuItem itemViewDescription;
        private DXMenuItem itemEdit;

        public uc318_SharedTaskManagement()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeGrid();
            InitializeMenuItems();

            sourceTasks.DataSource = tasks;
            gcData.DataSource = sourceTasks;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void InitializeGrid()
        {
            gvData.ReadOnlyGridView();
            gvData.OptionsView.ColumnAutoWidth = true;
            gvData.OptionsView.ShowAutoFilterRow = true;
            gvData.OptionsView.ShowGroupPanel = false;
            gvData.OptionsBehavior.AutoExpandAllGroups = true;
            gvData.Columns.Clear();

            gvData.Columns.AddVisible(nameof(Task318Draft.Id), "編碼").Width = 70;
            gvData.Columns.AddVisible(nameof(Task318Draft.Status), "狀態").Width = 100;
            gvData.Columns.AddVisible(nameof(Task318Draft.Subject), "主旨").Width = 420;
            gvData.Columns.AddVisible(nameof(Task318Draft.DueDate), "到期日").Width = 120;
            gvData.Columns.AddVisible(nameof(Task318Draft.Assignee), "負責人").Width = 130;
            gvData.Columns.AddVisible(nameof(Task318Draft.AttachmentCount), "附件數").Width = 90;
            gvData.Columns.AddVisible(nameof(Task318Draft.TargetFolderPath), "共用資料夾").Width = 420;
            gvData.Columns.AddVisible(nameof(Task318Draft.CreatedAt), "建立時間").Width = 150;

            gvData.Columns[nameof(Task318Draft.DueDate)].DisplayFormat.FormatType =
                DevExpress.Utils.FormatType.DateTime;
            gvData.Columns[nameof(Task318Draft.DueDate)].DisplayFormat.FormatString = "yyyy-MM-dd";
            gvData.Columns[nameof(Task318Draft.CreatedAt)].DisplayFormat.FormatType =
                DevExpress.Utils.FormatType.DateTime;
            gvData.Columns[nameof(Task318Draft.CreatedAt)].DisplayFormat.FormatString =
                "yyyy-MM-dd HH:mm";
        }

        private void InitializeMenuItems()
        {
            itemViewDescription = CreateMenuItem(
                "查看說明", (s, e) => ViewFocusedDescription(), TPSvgimages.View);
            itemEdit = CreateMenuItem(
                "編輯", (s, e) => EditFocusedTask(), TPSvgimages.Edit);
        }

        private static DXMenuItem CreateMenuItem(
            string caption,
            EventHandler clickEvent,
            SvgImage image)
        {
            var item = new DXMenuItem(caption, clickEvent, image, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            item.AppearanceHovered.ForeColor = Color.Blue;
            return item;
        }

        private Task318Draft FocusedTask => gvData.GetFocusedRow() as Task318Draft;

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = new f318_TaskInfo())
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;

                form.Draft.Id = tasks.Count + 1;
                form.Draft.Status = "待處理";
                form.Draft.CreatedAt = DateTime.Now;
                tasks.Add(form.Draft);
                gvData.BestFitColumns();
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Giai đoạn giao diện: dữ liệu chỉ nằm trong RAM, chưa đọc CSDL.
            sourceTasks.ResetBindings(false);
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType != GridMenuType.Row || FocusedTask == null) return;
            e.Menu.Items.Add(itemViewDescription);
            e.Menu.Items.Add(itemEdit);
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            ViewFocusedDescription();
        }

        private void ViewFocusedDescription()
        {
            if (FocusedTask == null) return;
            using (var form = new f318_Description(FocusedTask))
                form.ShowDialog(this);
        }

        private void EditFocusedTask()
        {
            var task = FocusedTask;
            if (task == null) return;

            using (var form = new f318_TaskInfo(task))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;
                sourceTasks.ResetBindings(false);
            }
        }
    }
}
