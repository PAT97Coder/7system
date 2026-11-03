using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class uc312ExamTest
    {
        private System.ComponentModel.IContainer components;
        private LayoutControl layout;
        private LayoutControlGroup root;
        private LayoutControlItem toolbarItem, resultsItem;
        private BarManager barManager;
        private Bar toolbar;
        private StandaloneBarDockControl toolbarDock;
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
            toolbarItem = new LayoutControlItem(); resultsItem = new LayoutControlItem();
            barManager = new BarManager(components); toolbar = new Bar(); toolbarDock = new StandaloneBarDockControl();
            btnInternal = new BarButtonItem(); btnExternal = new BarButtonItem(); btnRefresh = new BarButtonItem();
            resultsGrid = new GridControl(); resultsView = new GridView();
            colKind = new GridColumn(); colCode = new GridColumn(); colName = new GridColumn();
            colDepartment = new GridColumn(); colDate = new GridColumn(); colScore = new GridColumn();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)toolbarItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resultsItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resultsGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)resultsView).BeginInit();
            layout.Dock = DockStyle.Fill; layout.Name = "layout"; layout.AllowCustomization = false; layout.Root = root;
            layout.Controls.AddRange(new Control[] { toolbarDock, resultsGrid });
            root.Name = "root"; root.GroupBordersVisible = false; root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            root.LayoutMode = LayoutMode.Table; root.Padding = new DevExpress.XtraLayout.Utils.Padding(8);
            root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition { SizeType = SizeType.Percent, Width = 100 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Absolute, Height = 64 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Percent, Height = 100 });
            root.Items.AddRange(new BaseLayoutItem[] { toolbarItem, resultsItem });
            toolbarItem.Name = "toolbarItem"; toolbarItem.Control = toolbarDock; toolbarItem.TextVisible = false;
            resultsItem.Name = "resultsItem"; resultsItem.Control = resultsGrid; resultsItem.TextVisible = false;
            resultsItem.OptionsTableLayoutItem.RowIndex = 1;
            barManager.Form = this; barManager.Bars.Add(toolbar);
            barManager.Items.AddRange(new BarItem[] { btnInternal, btnExternal, btnRefresh });
            barManager.DockControls.Add(toolbarDock); toolbarDock.Manager = barManager; toolbarDock.Name = "toolbarDock";
            toolbar.BarName = "模擬測驗工具列"; toolbar.DockStyle = BarDockStyle.Standalone; toolbar.StandaloneBarDockControl = toolbarDock;
            toolbar.LinksPersistInfo.AddRange(new LinkPersistInfo[] {
                new LinkPersistInfo(btnInternal), new LinkPersistInfo(btnExternal), new LinkPersistInfo(btnRefresh, true)
            });
            Exam312Ui.StyleBar(toolbar);
            btnInternal.Name = "btnInternal"; btnInternal.Caption = "開始內部測驗"; btnInternal.Id = 0; btnInternal.Enabled = false;
            btnExternal.Name = "btnExternal"; btnExternal.Caption = "開始外部測驗"; btnExternal.Id = 1; btnExternal.Enabled = false;
            btnRefresh.Name = "btnRefresh"; btnRefresh.Caption = "重新整理"; btnRefresh.Id = 2; btnRefresh.Enabled = false;
            btnInternal.ImageOptions.SvgImage = btnExternal.ImageOptions.SvgImage = Helpers.TPSvgimages.Start;
            btnRefresh.ImageOptions.SvgImage = Helpers.TPSvgimages.Reload;
            Exam312Ui.StyleButton(btnInternal, 16F); Exam312Ui.StyleButton(btnExternal, 16F); Exam312Ui.StyleButton(btnRefresh, 16F);
            btnInternal.ItemClick += Internal_ItemClick; btnExternal.ItemClick += External_ItemClick; btnRefresh.ItemClick += Refresh_ItemClick;
            resultsGrid.Name = "resultsGrid"; resultsGrid.MainView = resultsView;
            resultsGrid.ViewCollection.Add(resultsView); resultsGrid.MenuManager = barManager;
            resultsView.Name = "resultsView"; resultsView.GridControl = resultsGrid;
            resultsView.OptionsBehavior.Editable = false; resultsView.OptionsBehavior.AutoPopulateColumns = false;
            resultsView.OptionsView.ShowGroupPanel = false; resultsView.OptionsView.ShowAutoFilterRow = true;
            resultsView.OptionsView.ColumnAutoWidth = true; resultsView.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            resultsView.OptionsSelection.EnableAppearanceFocusedCell = false;
            resultsView.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F); resultsView.Appearance.Row.Options.UseFont = true;
            resultsView.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F); resultsView.Appearance.HeaderPanel.Options.UseFont = true;
            colKind.Name = "colKind"; colKind.FieldName = "PracticeKind"; colKind.Caption = "測驗類型"; colKind.Width = 120;
            colKind.FilterMode = ColumnFilterMode.DisplayText;
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
            Controls.Add(layout); AutoScaleDimensions = new SizeF(7F, 14F); AutoScaleMode = AutoScaleMode.Font;
            Name = "uc312ExamTest"; Size = new Size(1000, 650); Load += uc312ExamTest_Load;
            ((System.ComponentModel.ISupportInitialize)resultsView).EndInit(); ((System.ComponentModel.ISupportInitialize)resultsGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)toolbarItem).EndInit(); ((System.ComponentModel.ISupportInitialize)resultsItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)root).EndInit(); ((System.ComponentModel.ISupportInitialize)layout).EndInit();
            ResumeLayout(false);
        }
    }
}
