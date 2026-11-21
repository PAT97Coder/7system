using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class uc312ExamTest
    {
        private System.ComponentModel.IContainer components;
        private LayoutControl layout;
        private LayoutControlGroup root;
        private LayoutControlItem resultsItem;
        private BarManager barManager;
        private Bar toolbar;
        private BarDockControl barDockTop, barDockBottom, barDockLeft, barDockRight;
        private BarButtonItem btnInternal, btnExternal, btnRefresh;
        private GridControl resultsGrid;
        private GridView resultsView;
        private GridColumn colKind, colCode, colName, colDepartment, colDate, colScore;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layout = new LayoutControl(); root = new LayoutControlGroup();
            resultsItem = new LayoutControlItem();
            barManager = new BarManager(components); toolbar = new Bar();
            barDockTop = new BarDockControl(); barDockBottom = new BarDockControl(); barDockLeft = new BarDockControl(); barDockRight = new BarDockControl();
            btnInternal = new BarButtonItem(); btnExternal = new BarButtonItem(); btnRefresh = new BarButtonItem();
            resultsGrid = new GridControl(); resultsView = new GridView();
            colKind = new GridColumn(); colCode = new GridColumn(); colName = new GridColumn();
            colDepartment = new GridColumn(); colDate = new GridColumn(); colScore = new GridColumn();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resultsItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resultsGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resultsView).BeginInit();
            layout.Dock = DockStyle.Fill; layout.Name = "layout"; layout.AllowCustomization = false; layout.Root = root;
            layout.Controls.Add(resultsGrid);
            root.Name = "root"; root.GroupBordersVisible = false; root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);
            root.Items.AddRange(new BaseLayoutItem[] { resultsItem });
            resultsItem.Name = "resultsItem"; resultsItem.Control = resultsGrid; resultsItem.TextVisible = false;
            barManager.Form = this; barManager.Bars.Add(toolbar); barManager.MainMenu = toolbar;
            barManager.Items.AddRange(new BarItem[] { btnInternal, btnExternal, btnRefresh });
            barDockTop.Name = "barDockTop"; barDockBottom.Name = "barDockBottom"; barDockLeft.Name = "barDockLeft"; barDockRight.Name = "barDockRight";
            barDockTop.Dock = DockStyle.Top; barDockBottom.Dock = DockStyle.Bottom; barDockLeft.Dock = DockStyle.Left; barDockRight.Dock = DockStyle.Right;
            foreach (var dock in new[] { barDockTop, barDockBottom, barDockLeft, barDockRight })
            { dock.Manager = barManager; dock.CausesValidation = false; barManager.DockControls.Add(dock); }
            toolbar.BarName = "模擬測驗工具列"; toolbar.DockStyle = BarDockStyle.Top; toolbar.CanDockStyle = BarCanDockStyle.Top;
            toolbar.DockCol = 0; toolbar.DockRow = 0;
            toolbar.LinksPersistInfo.AddRange(new LinkPersistInfo[] {
                new LinkPersistInfo(btnInternal), new LinkPersistInfo(btnExternal, true), new LinkPersistInfo(btnRefresh, true)
            });
            Exam312Ui.StyleBar(toolbar); toolbar.OptionsBar.MultiLine = true;
            foreach (var appearance in new[] { toolbar.BarAppearance.Normal, toolbar.BarAppearance.Hovered, toolbar.BarAppearance.Pressed })
            { appearance.Font = new Font("Microsoft JhengHei UI", 14.25F); appearance.ForeColor = Color.Black; appearance.Options.UseFont = appearance.Options.UseForeColor = true; }
            btnInternal.Name = "btnInternal"; btnInternal.Caption = "開始內部測驗"; btnInternal.Id = 0; btnInternal.Enabled = false;
            btnExternal.Name = "btnExternal"; btnExternal.Caption = "開始外部測驗"; btnExternal.Id = 1; btnExternal.Enabled = false;
            btnRefresh.Name = "btnRefresh"; btnRefresh.Caption = "刷新"; btnRefresh.Id = 2; btnRefresh.Enabled = false;
            btnInternal.ImageOptions.SvgImage = btnExternal.ImageOptions.SvgImage = Helpers.TPSvgimages.Start;
            btnRefresh.ImageOptions.SvgImage = Helpers.TPSvgimages.Reload;
            foreach (var button in new[] { btnInternal, btnExternal, btnRefresh })
            { Exam312Ui.StyleButton(button); button.ItemAppearance.Hovered.ForeColor = Color.Blue; }
            btnInternal.ItemClick += Internal_ItemClick; btnExternal.ItemClick += External_ItemClick; btnRefresh.ItemClick += Refresh_ItemClick;
            resultsGrid.Name = "resultsGrid"; resultsGrid.MainView = resultsView;
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
            resultsView.OptionsView.ShowGroupPanel = false; resultsView.OptionsView.ShowAutoFilterRow = true;
            resultsView.OptionsView.ColumnAutoWidth = false; resultsView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            resultsView.OptionsView.EnableAppearanceOddRow = true; resultsView.OptionsView.ShowFooter = true;
            resultsView.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            resultsView.OptionsSelection.EnableAppearanceFocusedCell = false;
            resultsView.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F); resultsView.Appearance.Row.Options.UseFont = true;
            resultsView.Appearance.Row.ForeColor = Color.Black; resultsView.Appearance.Row.Options.UseForeColor = true;
            resultsView.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F); resultsView.Appearance.HeaderPanel.Options.UseFont = true;
            resultsView.Appearance.HeaderPanel.ForeColor = Color.Black; resultsView.Appearance.HeaderPanel.Options.UseForeColor = true;
            resultsView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center; resultsView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            resultsView.Appearance.FooterPanel.Font = new Font("Microsoft JhengHei UI", 12F); resultsView.Appearance.FooterPanel.Options.UseFont = true;
            resultsView.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center; resultsView.Appearance.FooterPanel.Options.UseTextOptions = true;
            colKind.Name = "colKind"; colKind.FieldName = "PracticeKind"; colKind.Caption = "測驗類型"; colKind.Width = 120;
            colKind.FilterMode = ColumnFilterMode.DisplayText;
            colKind.Summary.Add(new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "PracticeKind", "筆數={0}"));
            colCode.Name = "colCode"; colCode.FieldName = "EmployeeCode"; colCode.Caption = "員工編號"; colCode.Width = 150;
            colName.Name = "colName"; colName.FieldName = "EmployeeName"; colName.Caption = "姓名"; colName.Width = 180;
            colDepartment.Name = "colDepartment"; colDepartment.FieldName = "Department"; colDepartment.Caption = "部門"; colDepartment.Width = 180;
            colDate.Name = "colDate"; colDate.FieldName = "SubmitAt"; colDate.Caption = "考試日期"; colDate.Width = 210;
            colScore.Name = "colScore"; colScore.FieldName = "Score"; colScore.Caption = "分數"; colScore.Width = 90;
            colKind.Visible = colCode.Visible = colName.Visible = colDepartment.Visible = colDate.Visible = colScore.Visible = true;
            colKind.VisibleIndex = 0; colCode.VisibleIndex = 1; colName.VisibleIndex = 2;
            colDepartment.VisibleIndex = 3; colDate.VisibleIndex = 4; colScore.VisibleIndex = 5;
            colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; colDate.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            colScore.AppearanceCell.Options.UseTextOptions = true; colScore.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            resultsView.Columns.AddRange(new GridColumn[] { colKind, colCode, colName, colDepartment, colDate, colScore });
            resultsView.CustomColumnDisplayText += Result_DisplayText; resultsView.PopupMenuShowing += Results_PopupMenuShowing;
            Controls.AddRange(new Control[] { layout, barDockLeft, barDockRight, barDockBottom, barDockTop });
            AutoScaleDimensions = new SizeF(7F, 14F); AutoScaleMode = AutoScaleMode.Font;
            Name = "uc312ExamTest"; Size = new Size(1000, 650); Load += uc312ExamTest_Load;
            ((System.ComponentModel.ISupportInitialize)resultsView).EndInit(); ((System.ComponentModel.ISupportInitialize)resultsGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)resultsItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)root).EndInit(); ((System.ComponentModel.ISupportInitialize)layout).EndInit();
            ResumeLayout(false); PerformLayout();
        }
    }
}
