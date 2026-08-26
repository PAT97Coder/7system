using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._14_ChineseExam
{
    public partial class uc314_HskExamMgmt : XtraUserControl
    {
        private readonly BindingSource sourceBases = new BindingSource();
        private DXMenuItem itemViewInfo;
        private DXMenuItem itemStartExam;
        private DXMenuItem itemFinishExam;
        private DXMenuItem itemExportExam;
        private DXMenuItem itemExportStatistical;

        public uc314_HskExamMgmt()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
            itemStartExam = CreateMenuItem("開始考試", ItemStartExam_Click, TPSvgimages.Start);
            itemFinishExam = CreateMenuItem("完成考試", ItemFinishExam_Click, TPSvgimages.Finish);
            itemExportExam = CreateMenuItem("導出結果", ItemExportExam_Click, TPSvgimages.Print);
            itemExportStatistical = CreateMenuItem("導出匯總表", ItemExportStatistical_Click, TPSvgimages.Excel);
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var item = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            item.AppearanceHovered.ForeColor = Color.Blue;
            return item;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                sourceBases.DataSource = dt314_HskExamMgmtBUS.Instance.GetList();
                gcData.DataSource = sourceBases;
                gvData.BestFitColumns();
            }
        }

        private void uc314_HskExamMgmt_Load(object sender, EventArgs e)
        {
            gvData.OptionsSelection.MultiSelect = true;
            gvData.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            gvData.ReadOnlyGridView();
            gridColumn4.Visible = false;
            gridColumn4.OptionsColumn.ShowInCustomizationForm = false;
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.CustomUnboundColumnData += gvData_CustomUnboundColumnData;
            LoadData();
            CreateRuleGV();
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gridColumn7, new DevExpress.Utils.AppearanceDefault() { BackColor = Color.Red, BackColor2 = Color.White }, "IsNullOrEmpty([FinishTime])");
            gvData.FormatRules.AddExpressionRule(gridColumn7, new DevExpress.Utils.AppearanceDefault() { BackColor = Color.LightGreen, BackColor2 = Color.White }, "IsNullOrEmpty([StartTime])");
        }

        private void gvData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName != "Status" || !e.IsGetData || view == null) return;

            bool isStart = !string.IsNullOrEmpty(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "StartTime")?.ToString());
            bool isFinish = !string.IsNullOrEmpty(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "FinishTime")?.ToString());
            e.Value = isFinish ? "考試完畢" : isStart ? "考試中" : "還沒開始";
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow) return;

            GridView view = sender as GridView;
            view.FocusedRowHandle = e.HitInfo.RowHandle;
            string startTime = view.GetRowCellValue(view.FocusedRowHandle, "StartTime")?.ToString();
            string finishTime = view.GetRowCellValue(view.FocusedRowHandle, "FinishTime")?.ToString();
            bool hasStartTime = !string.IsNullOrEmpty(startTime);
            bool hasFinishTime = !string.IsNullOrEmpty(finishTime);

            e.Menu.Items.Add(itemViewInfo);

            if (!hasFinishTime)
            {
                e.Menu.Items.Add(!hasStartTime ? itemStartExam : itemFinishExam);
            }
            else if (hasStartTime && hasFinishTime)
            {
                e.Menu.Items.Add(itemExportExam);
                e.Menu.Items.Add(itemExportStatistical);
            }
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            string examCode = gvData.GetRowCellValue(gvData.FocusedRowHandle, gridColumn1).ToString();
            using (f314_HskExamDetail frm = new f314_HskExamDetail())
            {
                frm.ExamCode = examCode;
                frm.ShowDialog();
            }
            LoadData();
        }

        private void ItemStartExam_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            var exam = dt314_HskExamMgmtBUS.Instance.GetItemById(id);
            exam.StartTime = DateTime.Now;
            dt314_HskExamMgmtBUS.Instance.AddOrUpdate(exam);
            LoadData();
        }

        private void ItemFinishExam_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            var exam = dt314_HskExamMgmtBUS.Instance.GetItemById(id);
            exam.FinishTime = DateTime.Now;
            dt314_HskExamMgmtBUS.Instance.AddOrUpdate(exam);
            LoadData();
        }

        private void ItemExportExam_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() != DialogResult.OK) return;
                string examCode = gvData.GetRowCellValue(gvData.FocusedRowHandle, gridColumn1).ToString();
                string saveFolder = Path.Combine(folderBrowser.SelectedPath, $"{examCode} {DateTime.Now:yyyyMMddHHmmss}");
                Directory.CreateDirectory(saveFolder);

                var examUsers = dt314_HskExamUserBUS.Instance.GetListByExamCode(examCode);
                foreach (var item in examUsers.Where(r => !string.IsNullOrWhiteSpace(r.ExamData)))
                {
                    var results = Hsk314ExamResultHelper.ParseExamResults(item.ExamData);
                    if (results.Count == 0) continue;
                    File.WriteAllText(Path.Combine(saveFolder, $"{item.IdUser}.html"), Hsk314ExamResultHelper.BuildResultHtml(item.IdUser, item.Score?.ToString() ?? "", results));
                }

                MsgTP.MsgShowInfomation("<font='Microsoft JhengHei UI' size=14>已導出完成！</font>");
            }
        }

        private void ItemExportStatistical_Click(object sender, EventArgs e)
        {
            var selectedItems = gvData.GetSelectedRows()
                .Select(rowHandle => gvData.GetRow(rowHandle) as dt314_HskExamMgmt)
                .Where(item => item != null && item.FinishTime != null)
                .ToList();
            if (selectedItems.Count == 0)
            {
                MsgTP.MsgError("請選擇已完成的考試！");
                return;
            }

            var examUsers = dt314_HskExamUserBUS.Instance.GetListByExamCodes(selectedItems.Select(r => r.Code).ToList());
            var users = dm_UserBUS.Instance.GetList();

            var excelDatas = (from data in examUsers
                              join user in users on data.IdUser equals user.Id into userJoin
                              from user in userJoin.DefaultIfEmpty()
                              orderby user?.IdDepartment, data.IdUser
                              select new
                              {
                                  dept = user?.IdDepartment ?? "",
                                  user_name = user == null ? data.IdUser : $"{user.Id} {user.DisplayName}",
                                  exam_code = data.ExamCode,
                                  data.Score,
                                  SubmitTime = data.SubmitTime?.ToString("yyyy/MM/dd HH:mm"),
                                  pass = data.IsPass == true ? "合格" : data.IsPass == false ? "不合格" : ""
                              }).ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath)) Directory.CreateDirectory(documentsPath);
            string filePath = Path.Combine(documentsPath, $"HSK考試匯總-{DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;
                ws.Cells["A1"].Value = "部門";
                ws.Cells["B1"].Value = "人員名稱";
                ws.Cells["C1"].Value = "考試編號";
                ws.Cells["D1"].Value = "得分";
                ws.Cells["E1"].Value = "提交時間";
                ws.Cells["F1"].Value = "結果";
                ws.Cells["A2"].LoadFromCollection(excelDatas, false);
                var fullRange = ws.Cells[ws.Dimension.Address];
                fullRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                fullRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                fullRange.Style.Border.Top.Style = fullRange.Style.Border.Bottom.Style =
                    fullRange.Style.Border.Left.Style = fullRange.Style.Border.Right.Style =
                    ExcelBorderStyle.Thin;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                pck.Save();
            }

            Process.Start(filePath);
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (f314_HskExamInfo frm = new f314_HskExamInfo())
                frm.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath)) Directory.CreateDirectory(documentsPath);
            string filePath = Path.Combine(documentsPath, $"HSK考試系統 - {DateTime.Now:yyyyMMddHHmm}.xlsx");
            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}
