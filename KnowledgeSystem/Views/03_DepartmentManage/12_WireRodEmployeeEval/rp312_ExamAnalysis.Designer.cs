using DevExpress.XtraCharts;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.Drawing.Printing;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class rp312_ExamAnalysis
    {
        private System.ComponentModel.IContainer components;
        private TopMarginBand topMargin;
        private BottomMarginBand bottomMargin;
        private ReportHeaderBand reportHeader;
        private DetailBand masterDetail, rowsDetail, chartDetail;
        private DetailReportBand rowsReport, chartsReport;
        private GroupHeaderBand rowsHeader;
        private PageFooterBand footer;
        private XRLabel titleLabel, scopeLabel, personLabel, filterLabel, timeLabel, statusLabel, noteLabel, chartTitle, chartScopeLabel;
        private XRPageInfo pageInfo;
        private XRTable headerTable, dataTable;
        private XRTableRow headerRow, dataRow;
        private XRTableCell headerGroup, headerPeople, headerPapers, headerQuestions, headerWrong, headerRate;
        private XRTableCell dataGroup, dataPeople, dataPapers, dataQuestions, dataWrong, dataRate;
        private XRChart reportChart;
        private XYDiagram reportDiagram;
        private Series reportSeries;
        private SideBySideBarSeriesView reportBarView;
        private SideBySideBarSeriesLabel reportBarLabel;
        private XRControlStyle oddStyle;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            topMargin = new TopMarginBand(); bottomMargin = new BottomMarginBand(); reportHeader = new ReportHeaderBand();
            masterDetail = new DetailBand(); rowsDetail = new DetailBand(); chartDetail = new DetailBand();
            rowsReport = new DetailReportBand(); chartsReport = new DetailReportBand(); rowsHeader = new GroupHeaderBand(); footer = new PageFooterBand();
            titleLabel = new XRLabel(); scopeLabel = new XRLabel(); personLabel = new XRLabel(); filterLabel = new XRLabel();
            timeLabel = new XRLabel(); statusLabel = new XRLabel(); noteLabel = new XRLabel(); chartTitle = new XRLabel(); chartScopeLabel = new XRLabel();
            pageInfo = new XRPageInfo(); headerTable = new XRTable(); dataTable = new XRTable(); headerRow = new XRTableRow(); dataRow = new XRTableRow();
            headerGroup = new XRTableCell(); headerPeople = new XRTableCell(); headerPapers = new XRTableCell();
            headerQuestions = new XRTableCell(); headerWrong = new XRTableCell(); headerRate = new XRTableCell();
            dataGroup = new XRTableCell(); dataPeople = new XRTableCell(); dataPapers = new XRTableCell();
            dataQuestions = new XRTableCell(); dataWrong = new XRTableCell(); dataRate = new XRTableCell();
            reportChart = new XRChart(); reportDiagram = new XYDiagram(); reportSeries = new Series("錯誤率", ViewType.Bar);
            reportBarView = new SideBySideBarSeriesView(); reportBarLabel = new SideBySideBarSeriesLabel(); oddStyle = new XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            ((System.ComponentModel.ISupportInitialize)headerTable).BeginInit(); ((System.ComponentModel.ISupportInitialize)dataTable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)reportChart).BeginInit(); ((System.ComponentModel.ISupportInitialize)reportDiagram).BeginInit();
            ((System.ComponentModel.ISupportInitialize)reportSeries).BeginInit(); ((System.ComponentModel.ISupportInitialize)reportBarView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)reportBarLabel).BeginInit();

            Name = "rp312_ExamAnalysis"; Version = "22.1"; Font = new Font("Microsoft JhengHei UI", 11F);
            PaperKind = PaperKind.A4; Landscape = true; Margins = new Margins(40,40,40,40);
            PageWidth = 1169; PageHeight = 827;
            Bands.AddRange(new Band[] { topMargin, bottomMargin, reportHeader, masterDetail, rowsReport, chartsReport, footer });
            topMargin.HeightF = bottomMargin.HeightF = 40; masterDetail.HeightF = 0;
            reportHeader.HeightF = 270; reportHeader.KeepTogether = true;
            reportHeader.Controls.AddRange(new XRControl[] { titleLabel, scopeLabel, personLabel, filterLabel, timeLabel, statusLabel, noteLabel });
            titleLabel.Name = "titleLabel"; titleLabel.Text = "錯題分析報告"; titleLabel.LocationF = new PointF(0,0); titleLabel.SizeF = new SizeF(1089,38);
            titleLabel.Font = new Font("Microsoft JhengHei UI", 22F,FontStyle.Bold); titleLabel.ForeColor = Color.FromArgb(16,75,115);
            scopeLabel.Name = "scopeLabel"; scopeLabel.LocationF = new PointF(0,45); scopeLabel.SizeF = new SizeF(1089,28);
            personLabel.Name = "personLabel"; personLabel.LocationF = new PointF(0,78); personLabel.SizeF = new SizeF(1089,28);
            filterLabel.Name = "filterLabel"; filterLabel.LocationF = new PointF(0,111); filterLabel.SizeF = new SizeF(1089,42);
            timeLabel.Name = "timeLabel"; timeLabel.LocationF = new PointF(0,158); timeLabel.SizeF = new SizeF(1089,25);
            statusLabel.Name = "statusLabel"; statusLabel.LocationF = new PointF(0,190); statusLabel.SizeF = new SizeF(1089,36);
            noteLabel.Name = "noteLabel"; noteLabel.LocationF = new PointF(0,235); noteLabel.SizeF = new SizeF(1089,30);
            foreach (var label in new[] { scopeLabel, personLabel, filterLabel, timeLabel, statusLabel, noteLabel })
            { label.CanGrow = true; label.Multiline = true; label.TextAlignment = TextAlignment.MiddleLeft; }
            timeLabel.Font = statusLabel.Font = noteLabel.Font = new Font("Microsoft JhengHei UI", 10F);
            statusLabel.ForeColor = noteLabel.ForeColor = Color.FromArgb(75,85,100);

            rowsReport.Name = "rowsReport"; rowsReport.Level = 0; rowsReport.DataMember = "Rows";
            rowsReport.Bands.AddRange(new Band[] { rowsHeader, rowsDetail });
            rowsHeader.HeightF = 36; rowsHeader.RepeatEveryPage = true; rowsHeader.GroupUnion = GroupUnion.WithFirstDetail;
            rowsHeader.Controls.Add(headerTable); rowsDetail.HeightF = 34; rowsDetail.KeepTogether = true; rowsDetail.Controls.Add(dataTable);
            headerTable.Name = "headerTable"; headerTable.SizeF = new SizeF(1089,36); headerTable.Rows.Add(headerRow);
            dataTable.Name = "dataTable"; dataTable.SizeF = new SizeF(1089,34); dataTable.Rows.Add(dataRow);
            headerRow.Cells.AddRange(new[] { headerGroup,headerPeople,headerPapers,headerQuestions,headerWrong,headerRate });
            dataRow.Cells.AddRange(new[] { dataGroup,dataPeople,dataPapers,dataQuestions,dataWrong,dataRate });
            headerGroup.Text = "題目群組"; headerPeople.Text = "作答人數"; headerPapers.Text = "試卷數";
            headerQuestions.Text = "題目次數"; headerWrong.Text = "答錯次數"; headerRate.Text = "錯誤率";
            headerGroup.Weight = dataGroup.Weight = 4.2;
            foreach (var cell in new[] { headerPeople,headerPapers,headerQuestions,headerWrong,headerRate,dataPeople,dataPapers,dataQuestions,dataWrong,dataRate }) cell.Weight = 1.25;
            foreach (var table in new[] { headerTable,dataTable })
            { table.Borders = BorderSide.All; table.BorderColor = Color.FromArgb(215,222,232); table.BorderWidth = 0.5F; table.Padding = new PaddingInfo(8,8,6,6); }
            headerTable.BackColor = Color.FromArgb(235,240,246); headerTable.Font = new Font("Microsoft JhengHei UI", 12F,FontStyle.Bold); headerTable.TextAlignment = TextAlignment.MiddleCenter;
            dataTable.Font = new Font("Microsoft JhengHei UI", 11F); dataTable.TextAlignment = TextAlignment.MiddleRight;
            dataGroup.TextAlignment = TextAlignment.MiddleLeft;
            dataGroup.Name = "dataGroup"; dataPeople.Name = "dataPeople"; dataPapers.Name = "dataPapers";
            dataQuestions.Name = "dataQuestions"; dataWrong.Name = "dataWrong"; dataRate.Name = "dataRate";
            dataGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint","Text","[GroupName]"));
            dataPeople.ExpressionBindings.Add(new ExpressionBinding("BeforePrint","Text","[PeopleCount]"));
            dataPapers.ExpressionBindings.Add(new ExpressionBinding("BeforePrint","Text","[PaperCount]"));
            dataQuestions.ExpressionBindings.Add(new ExpressionBinding("BeforePrint","Text","[QuestionCount]"));
            dataWrong.ExpressionBindings.Add(new ExpressionBinding("BeforePrint","Text","[WrongCount]"));
            dataRate.ExpressionBindings.Add(new ExpressionBinding("BeforePrint","Text","[ErrorRate]")); dataRate.TextFormatString = "{0:0.00}%";
            oddStyle.Name = "oddStyle"; oddStyle.BackColor = Color.FromArgb(247,249,252); StyleSheet.Add(oddStyle); dataTable.OddStyleName = oddStyle.Name;

            chartsReport.Name = "chartsReport"; chartsReport.Level = 1; chartsReport.DataMember = "ChartPages";
            chartsReport.Bands.Add(chartDetail); chartDetail.HeightF = 620; chartDetail.KeepTogether = true; chartDetail.PageBreak = PageBreak.BeforeBand;
            chartDetail.Controls.AddRange(new XRControl[] { chartTitle,chartScopeLabel,reportChart }); chartDetail.BeforePrint += ChartDetail_BeforePrint;
            chartTitle.Name = "chartTitle"; chartTitle.LocationF = new PointF(0,0); chartTitle.SizeF = new SizeF(1089,35); chartTitle.Font = new Font("Microsoft JhengHei UI",16F,FontStyle.Bold);
            chartScopeLabel.Name = "chartScopeLabel"; chartScopeLabel.LocationF = new PointF(0,42); chartScopeLabel.SizeF = new SizeF(1089,65); chartScopeLabel.Multiline = true;
            reportChart.Name = "reportChart"; reportChart.LocationF = new PointF(0,115); reportChart.SizeF = new SizeF(1089,480);
            reportChart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False; reportChart.Diagram = reportDiagram;
            reportDiagram.Rotated = false; reportDiagram.AxisX.Label.Font = reportDiagram.AxisY.Label.Font = new Font("Microsoft JhengHei UI",10F);
            reportDiagram.AxisX.Label.MaxWidth = 150; reportDiagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
            reportDiagram.AxisY.Label.TextPattern = "{V:0}%"; reportDiagram.AxisY.NumericScaleOptions.AutoGrid = false; reportDiagram.AxisY.NumericScaleOptions.GridSpacing = 20;
            reportDiagram.AxisY.WholeRange.Auto = false; reportDiagram.AxisY.WholeRange.AutoSideMargins = false; reportDiagram.AxisY.WholeRange.SetMinMaxValues(0,100);
            reportDiagram.AxisY.VisualRange.Auto = false; reportDiagram.AxisY.VisualRange.AutoSideMargins = false; reportDiagram.AxisY.VisualRange.SetMinMaxValues(0,100);
            reportSeries.ArgumentScaleType = ScaleType.Qualitative; reportSeries.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            reportBarView.Color = Color.FromArgb(16,110,190); reportSeries.View = reportBarView;
            reportBarLabel.Font = new Font("Microsoft JhengHei UI",10F); reportBarLabel.TextPattern = "{V:0.00}%"; reportBarLabel.Position = BarSeriesLabelPosition.TopInside;
            reportSeries.Label = reportBarLabel; reportChart.Series.Add(reportSeries); reportChart.CustomDrawAxisLabel += ReportChart_CustomDrawAxisLabel;

            footer.HeightF = 30; footer.Controls.Add(pageInfo); pageInfo.LocationF = new PointF(0,0); pageInfo.SizeF = new SizeF(1089,25);
            pageInfo.Font = new Font("Microsoft JhengHei UI",10F); pageInfo.TextAlignment = TextAlignment.MiddleRight;
            pageInfo.PageInfo = PageInfo.NumberOfTotal; pageInfo.TextFormatString = "第 {0} 頁／共 {1} 頁";

            ((System.ComponentModel.ISupportInitialize)reportBarLabel).EndInit(); ((System.ComponentModel.ISupportInitialize)reportBarView).EndInit();
            ((System.ComponentModel.ISupportInitialize)reportSeries).EndInit(); ((System.ComponentModel.ISupportInitialize)reportDiagram).EndInit(); ((System.ComponentModel.ISupportInitialize)reportChart).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataTable).EndInit(); ((System.ComponentModel.ISupportInitialize)headerTable).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
        }
    }
}
