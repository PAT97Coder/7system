using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class uc312_ExamAnalysis
    {
        private System.ComponentModel.IContainer components;
        private LayoutControl layout;
        private LayoutControlGroup root;
        private LayoutControlItem statusItem, contentItem;
        private BarManager barManager;
        private Bar toolbar;
        private BarDockControl barDockTop, barDockBottom, barDockLeft, barDockRight;
        private BarEditItem barScope, barExam, barPerson;
        private BarButtonItem btnRefresh;
        private RepositoryItemComboBox scopeEditor;
        private RepositoryItemSearchLookUpEdit examEditor;
        private RepositoryItemSearchLookUpEdit personEditor;
        private GridView peopleView, examsView;
        private GridColumn examNameColumn;
        private GridColumn personCodeColumn, personNameColumn;
        private LabelControl statusLabel;
        private SplitContainerControl split;
        private GridControl resultsGrid;
        private GridView resultsView;
        private GridColumn colGroup, colPeople, colPapers, colQuestions, colWrong, colRate, colGroupId;
        private ChartControl chart;
        private Series errorSeries;
        private XYDiagram diagram;
        private SideBySideBarSeriesView barView;
        private SideBySideBarSeriesLabel barLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing) { loadVersion++; components?.Dispose(); }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layout = new LayoutControl(); root = new LayoutControlGroup();
            statusItem = new LayoutControlItem(); contentItem = new LayoutControlItem();
            barManager = new BarManager(components); toolbar = new Bar();
            barDockTop = new BarDockControl(); barDockBottom = new BarDockControl(); barDockLeft = new BarDockControl(); barDockRight = new BarDockControl();
            barScope = new BarEditItem(); barExam = new BarEditItem(); barPerson = new BarEditItem(); btnRefresh = new BarButtonItem();
            scopeEditor = new RepositoryItemComboBox(); examEditor = new RepositoryItemSearchLookUpEdit(); personEditor = new RepositoryItemSearchLookUpEdit();
            peopleView = new GridView(); personCodeColumn = new GridColumn(); personNameColumn = new GridColumn();
            examsView = new GridView(); examNameColumn = new GridColumn();
            statusLabel = new LabelControl(); split = new SplitContainerControl();
            resultsGrid = new GridControl(); resultsView = new GridView();
            colGroup = new GridColumn(); colPeople = new GridColumn(); colPapers = new GridColumn();
            colQuestions = new GridColumn(); colWrong = new GridColumn(); colRate = new GridColumn(); colGroupId = new GridColumn();
            chart = new ChartControl(); errorSeries = new Series("錯誤率", ViewType.Bar);
            diagram = new XYDiagram(); barView = new SideBySideBarSeriesView(); barLabel = new SideBySideBarSeriesLabel();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layout).BeginInit(); ((System.ComponentModel.ISupportInitialize)root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)statusItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)contentItem).BeginInit(); ((System.ComponentModel.ISupportInitialize)barManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scopeEditor).BeginInit(); ((System.ComponentModel.ISupportInitialize)examEditor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)personEditor).BeginInit(); ((System.ComponentModel.ISupportInitialize)peopleView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)examsView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)split).BeginInit(); split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultsGrid).BeginInit(); ((System.ComponentModel.ISupportInitialize)resultsView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart).BeginInit(); ((System.ComponentModel.ISupportInitialize)diagram).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorSeries).BeginInit(); ((System.ComponentModel.ISupportInitialize)barView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barLabel).BeginInit();

            layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.AllowCustomization = false; layout.Root = root;
            layout.Controls.AddRange(new Control[] { statusLabel, split });
            root.Name = "root"; root.GroupBordersVisible = false; root.LayoutMode = LayoutMode.Table;
            root.EnableIndentsWithoutBorders = DefaultBoolean.True;
            root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);
            root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition { SizeType = SizeType.Percent, Width = 100 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Percent, Height = 100 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.AutoSize, Height = 28 });
            root.Items.AddRange(new BaseLayoutItem[] { contentItem, statusItem });
            statusItem.Name = "statusItem"; statusItem.Control = statusLabel; statusItem.TextVisible = false; statusItem.OptionsTableLayoutItem.RowIndex = 1;
            contentItem.Name = "contentItem"; contentItem.Control = split; contentItem.TextVisible = false;

            barManager.Form = this; barManager.Bars.Add(toolbar);
            barManager.MainMenu = toolbar;
            barManager.Items.AddRange(new BarItem[] { barScope, barExam, barPerson, btnRefresh });
            barManager.RepositoryItems.AddRange(new RepositoryItem[] { scopeEditor, examEditor, personEditor });
            barDockTop.Name = "barDockTop"; barDockBottom.Name = "barDockBottom"; barDockLeft.Name = "barDockLeft"; barDockRight.Name = "barDockRight";
            barDockTop.Dock = DockStyle.Top; barDockBottom.Dock = DockStyle.Bottom; barDockLeft.Dock = DockStyle.Left; barDockRight.Dock = DockStyle.Right;
            foreach (var dock in new[] { barDockTop, barDockBottom, barDockLeft, barDockRight })
            { dock.Manager = barManager; dock.CausesValidation = false; barManager.DockControls.Add(dock); }
            toolbar.BarName = "錯題分析工具列"; toolbar.DockStyle = BarDockStyle.Top; toolbar.CanDockStyle = BarCanDockStyle.Top;
            toolbar.DockCol = 0; toolbar.DockRow = 0;
            toolbar.LinksPersistInfo.AddRange(new[] { new LinkPersistInfo(btnRefresh), new LinkPersistInfo(barScope, true), new LinkPersistInfo(barExam), new LinkPersistInfo(barPerson) });
            Exam312Ui.StyleBar(toolbar); toolbar.OptionsBar.MultiLine = true;
            foreach (var appearance in new[] { toolbar.BarAppearance.Normal, toolbar.BarAppearance.Hovered, toolbar.BarAppearance.Pressed })
            { appearance.Font = new Font("Microsoft JhengHei UI", 14.25F); appearance.ForeColor = Color.Black; appearance.Options.UseFont = appearance.Options.UseForeColor = true; }
            barScope.Name = "barScope"; barScope.Caption = "範圍"; barScope.Id = 0; barScope.Edit = scopeEditor; barScope.EditWidth = 125; barScope.EditValue = "全部歷史";
            barExam.Name = "barExam"; barExam.Caption = "考試"; barExam.Id = 1; barExam.Edit = examEditor; barExam.EditWidth = 300;
            barPerson.Name = "barPerson"; barPerson.Caption = "人員"; barPerson.Id = 2; barPerson.Edit = personEditor; barPerson.EditWidth = 235; barPerson.EditValue = "";
            btnRefresh.Name = "btnRefresh"; btnRefresh.Caption = "刷新"; btnRefresh.Id = 3; btnRefresh.ImageOptions.SvgImage = Helpers.TPSvgimages.Reload;
            foreach (var item in new BarItem[] { barScope, barExam, barPerson, btnRefresh }) { Exam312Ui.StyleButton(item); item.Enabled = false; }
            btnRefresh.ItemAppearance.Hovered.ForeColor = Color.Blue;
            scopeEditor.Name = "scopeEditor"; scopeEditor.TextEditStyle = TextEditStyles.DisableTextEditor;
            scopeEditor.Items.AddRange(new object[] { "全部歷史", "單次考試" });
            examEditor.Name = "examEditor"; examEditor.DisplayMember = "Name"; examEditor.ValueMember = "Id"; examEditor.NullText = "請選擇考試";
            examEditor.PopupView = examsView; examEditor.PopupFormSize = new Size(340, 240);
            examEditor.PopupFormMinSize = new Size(340, 180); examEditor.BestFitMode = BestFitMode.BestFitResizePopup;
            examNameColumn.Name = "examNameColumn"; examNameColumn.FieldName = "Name"; examNameColumn.Caption = "考試名稱";
            examNameColumn.Visible = true; examNameColumn.VisibleIndex = 0; examNameColumn.Width = 300;
            examsView.Name = "examsView"; examsView.Columns.AddRange(new[] { examNameColumn });
            examsView.OptionsBehavior.Editable = false; examsView.OptionsBehavior.AutoPopulateColumns = false;
            examsView.OptionsView.ShowGroupPanel = false; examsView.OptionsView.ShowAutoFilterRow = true; examsView.OptionsView.ColumnAutoWidth = true;
            examsView.OptionsSelection.EnableAppearanceFocusedCell = false;
            examsView.OptionsFind.AlwaysVisible = true; examsView.OptionsFind.FindFilterColumns = "Name";
            examsView.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 14.25F); examsView.Appearance.Row.Options.UseFont = true;
            examsView.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F); examsView.Appearance.HeaderPanel.Options.UseFont = true;
            personEditor.Name = "personEditor"; personEditor.DisplayMember = "Display"; personEditor.ValueMember = "UserId";
            personEditor.NullText = "全部人員"; personEditor.PopupView = peopleView; personEditor.PopupFormSize = new Size(340, 240);
            personEditor.PopupFormMinSize = new Size(340, 180); personEditor.BestFitMode = BestFitMode.BestFitResizePopup;
            personCodeColumn.Name = "personCodeColumn"; personCodeColumn.FieldName = "UserId"; personCodeColumn.Caption = "員工編號"; personCodeColumn.Visible = true; personCodeColumn.VisibleIndex = 0; personCodeColumn.Width = 140;
            personNameColumn.Name = "personNameColumn"; personNameColumn.FieldName = "Name"; personNameColumn.Caption = "姓名"; personNameColumn.Visible = true; personNameColumn.VisibleIndex = 1; personNameColumn.Width = 140;
            peopleView.Name = "peopleView"; peopleView.Columns.AddRange(new[] { personCodeColumn, personNameColumn });
            peopleView.OptionsBehavior.Editable = false; peopleView.OptionsView.ShowGroupPanel = false; peopleView.OptionsView.ShowAutoFilterRow = true;
            peopleView.OptionsView.ColumnAutoWidth = true;
            peopleView.OptionsSelection.EnableAppearanceFocusedCell = false;
            peopleView.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 14.25F); peopleView.Appearance.Row.Options.UseFont = true;
            peopleView.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F); peopleView.Appearance.HeaderPanel.Options.UseFont = true;
            foreach (var editor in new RepositoryItem[] { scopeEditor, examEditor, personEditor })
            { editor.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F); editor.Appearance.Options.UseFont = true; }
            foreach (var editor in new[] { examEditor, personEditor })
            { editor.Appearance.TextOptions.HAlignment = HorzAlignment.Near; editor.Appearance.Options.UseTextOptions = true; }
            foreach (var view in new[] { examsView, peopleView })
            {
                view.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center; view.Appearance.HeaderPanel.Options.UseTextOptions = true;
                view.Appearance.Row.TextOptions.HAlignment = HorzAlignment.Near; view.Appearance.Row.Options.UseTextOptions = true;
            }
            scopeEditor.AppearanceDropDown.Font = new Font("Microsoft JhengHei UI", 14.25F);
            scopeEditor.AppearanceDropDown.Options.UseFont = true;
            barScope.EditValueChanged += Filter_EditValueChanged; barExam.EditValueChanged += Filter_EditValueChanged; barPerson.EditValueChanged += Filter_EditValueChanged;
            btnRefresh.ItemClick += Refresh_ItemClick;

            statusLabel.Name = "statusLabel"; statusLabel.Text = "請先登錄功能並授予錯題分析權限。";
            statusLabel.AutoSizeMode = LabelAutoSizeMode.Vertical; statusLabel.Appearance.Font = new Font("Microsoft JhengHei UI", 12F);
            statusLabel.Appearance.Options.UseFont = true; statusLabel.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
            split.Name = "split"; split.Horizontal = false; split.FixedPanel = SplitFixedPanel.None;
            split.Size = new Size(1100, 590); split.SplitterPosition = 250; split.Panel1.MinSize = 130; split.Panel2.MinSize = 180;
            split.Panel1.Controls.Add(resultsGrid); split.Panel2.Controls.Add(chart);

            resultsGrid.Name = "resultsGrid"; resultsGrid.Dock = DockStyle.Fill; resultsGrid.MainView = resultsView;
            resultsGrid.ViewCollection.Add(resultsView); resultsGrid.MenuManager = barManager;
            resultsGrid.UseEmbeddedNavigator = true;
            resultsGrid.EmbeddedNavigator.Buttons.Append.Visible = false; resultsGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            resultsGrid.EmbeddedNavigator.Buttons.Edit.Visible = false; resultsGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            resultsGrid.EmbeddedNavigator.Buttons.Remove.Visible = false;
            resultsGrid.EmbeddedNavigator.TextStringFormat = "第 {0} 筆，共 {1} 筆";
            resultsGrid.EmbeddedNavigator.Appearance.Font = new Font("Microsoft JhengHei UI", 12F); resultsGrid.EmbeddedNavigator.Appearance.Options.UseFont = true;
            resultsGrid.EmbeddedNavigator.Buttons.First.Hint = "第一筆"; resultsGrid.EmbeddedNavigator.Buttons.PrevPage.Hint = "上一頁";
            resultsGrid.EmbeddedNavigator.Buttons.Prev.Hint = "上一筆"; resultsGrid.EmbeddedNavigator.Buttons.Next.Hint = "下一筆";
            resultsGrid.EmbeddedNavigator.Buttons.NextPage.Hint = "下一頁"; resultsGrid.EmbeddedNavigator.Buttons.Last.Hint = "最後一筆";
            resultsView.Name = "resultsView"; resultsView.GridControl = resultsGrid;
            resultsView.OptionsBehavior.Editable = false; resultsView.OptionsBehavior.AutoPopulateColumns = false;
            resultsView.OptionsView.ShowAutoFilterRow = true; resultsView.OptionsView.ShowGroupPanel = false; resultsView.OptionsView.ColumnAutoWidth = false;
            resultsView.OptionsView.EnableAppearanceOddRow = true; resultsView.OptionsView.ShowFooter = true;
            resultsView.OptionsSelection.EnableAppearanceHotTrackedRow = DefaultBoolean.True;
            resultsView.OptionsSelection.EnableAppearanceFocusedCell = false; resultsView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            resultsView.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F); resultsView.Appearance.Row.Options.UseFont = true;
            resultsView.Appearance.Row.ForeColor = Color.Black; resultsView.Appearance.Row.Options.UseForeColor = true;
            resultsView.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F); resultsView.Appearance.HeaderPanel.Options.UseFont = true;
            resultsView.Appearance.HeaderPanel.ForeColor = Color.Black; resultsView.Appearance.HeaderPanel.Options.UseForeColor = true;
            resultsView.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center; resultsView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            resultsView.Appearance.FooterPanel.Font = new Font("Microsoft JhengHei UI", 12F); resultsView.Appearance.FooterPanel.Options.UseFont = true;
            resultsView.Appearance.FooterPanel.TextOptions.HAlignment = HorzAlignment.Center; resultsView.Appearance.FooterPanel.Options.UseTextOptions = true;
            colGroup.Name = "colGroup"; colGroup.FieldName = "GroupName"; colGroup.Caption = "題目群組"; colGroup.Width = 280;
            colPeople.Name = "colPeople"; colPeople.FieldName = "PeopleCount"; colPeople.Caption = "作答人數"; colPeople.Width = 110;
            colPapers.Name = "colPapers"; colPapers.FieldName = "PaperCount"; colPapers.Caption = "試卷數"; colPapers.Width = 100;
            colQuestions.Name = "colQuestions"; colQuestions.FieldName = "QuestionCount"; colQuestions.Caption = "題目次數"; colQuestions.Width = 115;
            colWrong.Name = "colWrong"; colWrong.FieldName = "WrongCount"; colWrong.Caption = "答錯次數"; colWrong.Width = 115;
            colRate.Name = "colRate"; colRate.FieldName = "ErrorRate"; colRate.Caption = "錯誤率"; colRate.Width = 110;
            colGroup.Summary.Add(new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "GroupName", "群組數={0}"));
            colQuestions.Summary.Add(new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "QuestionCount", "合計={0}"));
            colWrong.Summary.Add(new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "WrongCount", "合計={0}"));
            colRate.DisplayFormat.FormatType = FormatType.Numeric; colRate.DisplayFormat.FormatString = "0.00'%'";
            colGroupId.Name = "colGroupId"; colGroupId.FieldName = "GroupId"; colGroupId.Caption = "群組編號";
            int visibleIndex = 0;
            foreach (var column in new[] { colGroup, colPeople, colPapers, colQuestions, colWrong, colRate })
            { column.Visible = true; column.VisibleIndex = visibleIndex++; }
            foreach (var column in new[] { colPeople, colPapers, colQuestions, colWrong, colRate })
            { column.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far; column.AppearanceCell.Options.UseTextOptions = true; }
            resultsView.Columns.AddRange(new[] { colGroup, colPeople, colPapers, colQuestions, colWrong, colRate, colGroupId });
            resultsView.SortInfo.AddRange(new[] { new GridColumnSortInfo(colRate, DevExpress.Data.ColumnSortOrder.Descending), new GridColumnSortInfo(colQuestions, DevExpress.Data.ColumnSortOrder.Descending), new GridColumnSortInfo(colGroupId, DevExpress.Data.ColumnSortOrder.Ascending) });
            resultsView.ColumnFilterChanged += Results_Changed; resultsView.EndSorting += Results_Changed;
            resultsView.PopupMenuShowing += Results_PopupMenuShowing; peopleView.PopupMenuShowing += Results_PopupMenuShowing;
            examsView.PopupMenuShowing += Results_PopupMenuShowing;

            chart.Name = "chart"; chart.Dock = DockStyle.Fill; chart.Font = new Font("Microsoft JhengHei UI", 12F);
            chart.CustomDrawAxisLabel += Chart_CustomDrawAxisLabel;
            chart.Legend.Visibility = DefaultBoolean.False; chart.CrosshairEnabled = DefaultBoolean.False; chart.ToolTipEnabled = DefaultBoolean.True;
            chart.EmptyChartText.Text = "沒有可供分析的資料"; chart.EmptyChartText.Font = new Font("Microsoft JhengHei UI", 14.25F);
            chart.Diagram = diagram; diagram.Rotated = false; diagram.AxisX.Reverse = false;
            diagram.AxisX.Label.Font = diagram.AxisY.Label.Font = new Font("Microsoft JhengHei UI", 12F);
            diagram.AxisX.Label.ResolveOverlappingOptions.AllowHide = false;
            diagram.AxisX.Label.MaxWidth = 180;
            diagram.AxisY.Label.TextPattern = "{V:0}%";
            diagram.AxisY.NumericScaleOptions.AutoGrid = false; diagram.AxisY.NumericScaleOptions.GridSpacing = 20;
            diagram.AxisY.WholeRange.Auto = false; diagram.AxisY.WholeRange.AutoSideMargins = false; diagram.AxisY.WholeRange.SetMinMaxValues(0, 100);
            diagram.AxisY.VisualRange.Auto = false; diagram.AxisY.VisualRange.AutoSideMargins = false; diagram.AxisY.VisualRange.SetMinMaxValues(0, 100);
            diagram.EnableAxisXScrolling = true; diagram.EnableAxisYScrolling = false;
            diagram.DefaultPane.ScrollBarOptions.XAxisScrollBarVisible = true;
            errorSeries.ArgumentScaleType = ScaleType.Qualitative; errorSeries.LabelsVisibility = DefaultBoolean.True;
            errorSeries.ToolTipPointPattern = "{HINT}";
            barView.Color = Color.FromArgb(16, 110, 190); errorSeries.View = barView;
            barLabel.TextPattern = "{V:0.00}%"; barLabel.Font = new Font("Microsoft JhengHei UI", 12F); barLabel.Position = BarSeriesLabelPosition.TopInside;
            errorSeries.Label = barLabel; chart.Series.Add(errorSeries);

            Controls.AddRange(new Control[] { layout, barDockLeft, barDockRight, barDockBottom, barDockTop });
            AutoScaleDimensions = new SizeF(7F, 14F); AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Microsoft JhengHei UI", 12F); Name = "uc312_ExamAnalysis"; Text = "錯題分析"; Size = new Size(1150, 750);
            Load += Analysis_Load;
            ((System.ComponentModel.ISupportInitialize)barLabel).EndInit(); ((System.ComponentModel.ISupportInitialize)barView).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorSeries).EndInit(); ((System.ComponentModel.ISupportInitialize)diagram).EndInit(); ((System.ComponentModel.ISupportInitialize)chart).EndInit();
            ((System.ComponentModel.ISupportInitialize)resultsView).EndInit(); ((System.ComponentModel.ISupportInitialize)resultsGrid).EndInit();
            split.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)split).EndInit();
            ((System.ComponentModel.ISupportInitialize)peopleView).EndInit(); ((System.ComponentModel.ISupportInitialize)personEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)examsView).EndInit();
            ((System.ComponentModel.ISupportInitialize)examEditor).EndInit(); ((System.ComponentModel.ISupportInitialize)scopeEditor).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager).EndInit(); ((System.ComponentModel.ISupportInitialize)contentItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)statusItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)root).EndInit(); ((System.ComponentModel.ISupportInitialize)layout).EndInit();
            ResumeLayout(false); PerformLayout();
        }
    }
}
