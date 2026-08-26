using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    public partial class f314_HskExamDetail : XtraForm
    {
        public string ExamCode { get; set; } = "";

        private DXMenuItem itemViewInfo;

        public f314_HskExamDetail()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var item = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            item.AppearanceHovered.ForeColor = Color.Blue;
            return item;
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string json = view.GetRowCellValue(view.FocusedRowHandle, "data.ExamData")?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(json)) return;

            var results = Hsk314ExamResultHelper.ParseExamResults(json);
            if (results.Count == 0)
            {
                MsgTP.MsgError("資料格式不正確！");
                return;
            }

            string userName = view.GetRowCellValue(view.FocusedRowHandle, "DisplayName")?.ToString() ?? "";
            string score = view.GetRowCellValue(view.FocusedRowHandle, "data.Score")?.ToString() ?? "";

            WebBrowser webView = new WebBrowser { Dock = DockStyle.Fill };
            XtraForm formView = new XtraForm
            {
                Text = "考試結果",
                WindowState = FormWindowState.Maximized,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                IconOptions = { Image = Properties.Resources.AppIcon }
            };

            formView.Controls.Add(webView);
            formView.Load += (o, args) => webView.DocumentText = Hsk314ExamResultHelper.BuildResultHtml(userName, score, results);
            formView.ShowDialog();
        }

        private void LoadData()
        {
            var bases = dt314_HskExamUserBUS.Instance.GetListByExamCode(ExamCode);
            var users = dm_UserBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList();

            var data = (from item in bases
                        join usr in users on item.IdUser equals usr.Id into usrJoin
                        from usr in usrJoin.DefaultIfEmpty()
                        join job in jobs on usr?.ActualJobCode equals job.Id into jobJoin
                        from job in jobJoin.DefaultIfEmpty()
                        join dept in depts on usr?.IdDepartment equals dept.Id into deptJoin
                        from dept in deptJoin.DefaultIfEmpty()
                        let DeptName = dept == null ? usr?.IdDepartment ?? "" : $"{dept.Id}\r\n{dept.DisplayName}"
                        let DisplayName = usr == null ? item.IdUser : $"{usr.DisplayName}\r\n{usr.DisplayNameVN}"
                        select new
                        {
                            data = item,
                            usr,
                            job,
                            DeptName,
                            DisplayName
                        }).ToList();

            gcData.DataSource = data;
            gvData.BestFitColumns();
        }

        private void f314_HskExamDetail_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow) return;

            GridView view = sender as GridView;
            view.FocusedRowHandle = e.HitInfo.RowHandle;
            bool isComplete = !string.IsNullOrWhiteSpace(view.GetRowCellValue(view.FocusedRowHandle, "data.ExamData")?.ToString());
            if (isComplete)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath)) Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"HSK考試結果 - {DateTime.Now:yyyyMMddHHmm}.xlsx");
            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}
