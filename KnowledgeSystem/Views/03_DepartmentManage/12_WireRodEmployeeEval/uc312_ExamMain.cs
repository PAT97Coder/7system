using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._07_Quiz;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class uc312_ExamMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc312_ExamMain()
        {
            InitializeComponent();
            InitializeMenuItems();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        List<dm_User> users = new List<dm_User>();

        BindingSource sourceBases = new BindingSource();
        BindingSource sourceExams = new BindingSource();

        DXMenuItem itemViewInfo;

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView activeView = gvEmp;
            if (activeView == null) return;
            if (activeView.FocusedRowHandle < 0) return;
            int idEmp = Convert.ToInt32(activeView.GetRowCellValue(activeView.FocusedRowHandle, gColEmpId));

            var examData = dt312_ExamUserBUS.Instance.GetItemById(idEmp);
            var userData = users.First(r => r.Id == examData.UserId);

            if (string.IsNullOrEmpty(examData.ExamData)) return;

            List<f312_DoExam.ExamResult> examResults = JsonConvert.DeserializeObject<List<f312_DoExam.ExamResult>>(examData.ExamData);

            using (var form = new f312_ExamReview(examResults, examData.Score, false)) form.ShowDialog(FindForm());
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                var myExams = dt312_ExamUserBUS.Instance.GetListByUserId(TPConfigs.LoginUser.Id);
                var bases = dt312_ExamMgmtBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                var settting = dt312_SettingBUS.Instance.GetItemById(1);

                var dataDisplays = (from exam in myExams
                                    join data in bases on exam.ExamId equals data.Id
                                    select new
                                    {
                                        exam,
                                        data,
                                        TestDuration = $"{settting.TestDuration}分鐘",
                                        QuesCount = $"{settting.QuesCount}題目",
                                        PassingScore = $"{settting.PassingScore}/100"
                                    }).ToList();

                sourceBases.DataSource = dataDisplays.Where(r => string.IsNullOrEmpty(r.exam.SubmitAt.ToString())).ToList();
                sourceExams.DataSource = dataDisplays;
                gvEmp.BestFitColumns();
            }
        }

        private void uc312_ExamMain_Load(object sender, EventArgs e)
        {
            btnPractice.Enabled = dt312_QuestionsBUS.Instance.CanPractice(TPConfigs.LoginUser.Id);
            gvEmp.ReadOnlyGridView();
            gvEmp.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEmp.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            gcData.DataSource = sourceBases;
            gcExamResult.DataSource = sourceExams;

            gvEmp.BestFitColumns();
        }

        private void lvData_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            LayoutView view = sender as LayoutView;
            LayoutViewHitInfo hi = view.CalcHitInfo(args.Location);
            if (hi.InCard)
            {
                int idExam = Convert.ToInt16(view.GetRowCellValue(hi.RowHandle, "exam.Id").ToString());

                f312_DoExam fDoExam = new f312_DoExam();
                fDoExam.idExamUser = idExam;
                fDoExam.ShowDialog();

                LoadData();
            }
        }

        private void Practice_Click(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!btnPractice.Enabled) return;
            using (var exam = new f312_DoExam(true)) exam.ShowDialog(FindForm());
        }

        private void gvEmp_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }
    }
}
