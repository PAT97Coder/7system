using BusinessLayer;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._17_ExamStatistics
{
    public partial class uc317_ExamStatistics : XtraUserControl
    {
        private readonly BindingSource sourceStatistics = new BindingSource();
        private bool isLoaded;

        public uc317_ExamStatistics()
        {
            InitializeComponent();
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportForm1.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportForm2.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportForm3.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportForm4.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportForm5.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportAllForms.ImageOptions.SvgImage = TPSvgimages.Excel;
            foreach (BarButtonItem item in new[]
            {
                btnExportForm1, btnExportForm2, btnExportForm3,
                btnExportForm4, btnExportForm5, btnExportAllForms
            })
                ApplyExportButtonAppearance(item);
            ConfigureGrid();
        }

        private static void ApplyExportButtonAppearance(BarButtonItem item)
        {
            var font = new Font("Microsoft JhengHei UI", 14.25F);
            item.ImageOptions.SvgImageSize = new Size(32, 32);
            item.ItemAppearance.Normal.Font = font;
            item.ItemAppearance.Normal.ForeColor = Color.Black;
            item.ItemAppearance.Normal.Options.UseFont = true;
            item.ItemAppearance.Normal.Options.UseForeColor = true;
            item.ItemAppearance.Hovered.Font = font;
            item.ItemAppearance.Hovered.ForeColor = Color.Blue;
            item.ItemAppearance.Hovered.Options.UseFont = true;
            item.ItemAppearance.Hovered.Options.UseForeColor = true;
            item.ItemAppearance.Pressed.Font = font;
            item.ItemAppearance.Pressed.ForeColor = Color.Blue;
            item.ItemAppearance.Pressed.Options.UseFont = true;
            item.ItemAppearance.Pressed.Options.UseForeColor = true;
        }

        private void ConfigureGrid()
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsView.ColumnAutoWidth = false;
            gvData.OptionsView.EnableAppearanceOddRow = true;
            gvData.OptionsView.ShowAutoFilterRow = true;
            gvData.OptionsView.ShowGroupPanel = false;
            gvData.Columns.Clear();

            AddColumn(nameof(Exam317StatisticsRow.Rank), "排名", 60, HorzAlignment.Center);
            AddColumn(nameof(Exam317StatisticsRow.UserId), "人員代號", 100);
            AddColumn(nameof(Exam317StatisticsRow.UserName), "人員名稱", 180);
            AddColumn(nameof(Exam317StatisticsRow.DepartmentName), "部門", 140);
            AddScoreColumn(nameof(Exam317StatisticsRow.ProfessionalScore), "專業成績 (20%)");
            AddScoreColumn(nameof(Exam317StatisticsRow.ChineseScore), "漢語成績 (30%)");
            AddScoreColumn(nameof(Exam317StatisticsRow.InterviewScore), "口試成績 (50%)");
            AddColumn(nameof(Exam317StatisticsRow.WeightedScoreDetails), "計分明細", 180,
                HorzAlignment.Far);
            AddScoreColumn(nameof(Exam317StatisticsRow.TotalScore), "總成績");
            AddColumn(nameof(Exam317StatisticsRow.CompletionStatus), "資料狀態", 80, HorzAlignment.Center);

            GridColumn totalColumn = gvData.Columns.ColumnByFieldName(nameof(Exam317StatisticsRow.TotalScore));
            totalColumn.AppearanceCell.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            totalColumn.AppearanceCell.ForeColor = Color.Blue;
        }

        private GridColumn AddColumn(string fieldName, string caption, int width,
            HorzAlignment alignment = HorzAlignment.Near)
        {
            GridColumn column = gvData.Columns.AddVisible(fieldName, caption);
            column.Width = width;
            column.AppearanceCell.TextOptions.HAlignment = alignment;
            return column;
        }

        private void AddScoreColumn(string fieldName, string caption)
        {
            GridColumn column = AddColumn(fieldName, caption, 105, HorzAlignment.Far);
            column.DisplayFormat.FormatType = FormatType.Numeric;
            column.DisplayFormat.FormatString = "0.##";
        }

        private void uc317_ExamStatistics_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceStatistics;
            repositoryYear.Items.Clear();
            repositoryYear.Items.AddRange(dt317_ExamStatisticsBUS.Instance.GetAvailableYears());
            barYear.EditValue = DateTime.Now.Year;
            isLoaded = true;
            LoadData();
        }

        private int SelectedYear
        {
            get
            {
                return int.TryParse(Convert.ToString(barYear.EditValue), out int year)
                    ? year
                    : DateTime.Now.Year;
            }
        }

        private void LoadData()
        {
            try
            {
                using (SplashScreenManager.ShowOverlayForm(gcData))
                    sourceStatistics.DataSource = dt317_ExamStatisticsBUS.Instance.GetStatistics(SelectedYear);
                gvData.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barYear_EditValueChanged(object sender, EventArgs e)
        {
            if (isLoaded) LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportForm1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportForm(Exam317TemplateExporter.ProfessionalForm);
        }

        private void btnExportForm2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportForm(Exam317TemplateExporter.ChineseForm);
        }

        private void btnExportForm3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportForm(Exam317TemplateExporter.ChineseRetakeForm);
        }

        private void btnExportForm4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportForm(Exam317TemplateExporter.InterviewForm);
        }

        private void btnExportForm5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportForm(Exam317TemplateExporter.SummaryForm);
        }

        private void btnExportAllForms_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string folder;
                using (SplashScreenManager.ShowOverlayForm(gcData))
                {
                    Exam317ExportData data = dt317_ExamStatisticsBUS.Instance.GetExportData(SelectedYear);
                    folder = GetExportFolder(data);
                    Exam317TemplateExporter.ExportAll(data, folder);
                }
                Process.Start(folder);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportForm(int formNumber)
        {
            try
            {
                string filePath;
                using (SplashScreenManager.ShowOverlayForm(gcData))
                {
                    Exam317ExportData data = dt317_ExamStatisticsBUS.Instance.GetExportData(SelectedYear);
                    filePath = Exam317TemplateExporter.ExportForm(formNumber, data, GetExportFolder(data));
                }
                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetExportFolder(Exam317ExportData data)
        {
            string exportBatch = string.Format("{0}-{1:yyyyMMdd-HHmmss}",
                data.Year, data.ExportedAt);
            string folder = Path.Combine(TPConfigs.DocumentPath(), "317", exportBatch);
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
