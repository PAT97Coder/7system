namespace KnowledgeSystem.Views._03_DepartmentManage._20_AuditManagement
{
    partial class uc320_mainjihua
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode3 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uc320_mainjihua));
            this.gvSupervising = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColSupervisingDepartment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColSupervisingUserId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColSupervisingUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColSupervisingJobTitle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvAccompanying = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColAccompanyingDepartment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColAccompanyingUserId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColAccompanyingUserName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColAccompanyingJobTitle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvFiles = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColFileName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColUploadedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColUploadedAt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcData = new DevExpress.XtraGrid.GridControl();
            this.gvData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColIdMaterial = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColStatus309 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColReplacementCode309 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColReplacementDate309 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPhoto309 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDisplayName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColUserMngr = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barCbbDept = new DevExpress.XtraBars.BarEditItem();
            this.cbbDept = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarSubItem();
            this.btnExcelBySpare = new DevExpress.XtraBars.BarButtonItem();
            this.btnExcelByMachine = new DevExpress.XtraBars.BarButtonItem();
            this.btnExcelByNotify = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.gvSupervising)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAccompanying)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept)).BeginInit();
            this.SuspendLayout();
            // gvSupervising - 督導單位
            this.gvSupervising.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvSupervising.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            this.gvSupervising.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvSupervising.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvSupervising.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvSupervising.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvSupervising.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvSupervising.Appearance.Row.Options.UseFont = true;
            this.gvSupervising.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColSupervisingDepartment,
            this.gColSupervisingUserId,
            this.gColSupervisingUserName,
            this.gColSupervisingJobTitle});
            this.gvSupervising.GridControl = this.gcData;
            this.gvSupervising.Name = "gvSupervising";
            this.gvSupervising.ViewCaption = "督導單位";
            this.gvSupervising.OptionsBehavior.Editable = false;
            this.gvSupervising.OptionsView.ColumnAutoWidth = false;
            this.gvSupervising.OptionsView.EnableAppearanceOddRow = true;
            this.gvSupervising.OptionsView.ShowAutoFilterRow = true;
            this.gvSupervising.OptionsView.ShowGroupPanel = false;
            // gColSupervisingDepartment
            this.gColSupervisingDepartment.Caption = "部門";
            this.gColSupervisingDepartment.FieldName = "DepartmentName";
            this.gColSupervisingDepartment.Name = "gColSupervisingDepartment";
            this.gColSupervisingDepartment.Visible = true;
            this.gColSupervisingDepartment.VisibleIndex = 0;
            this.gColSupervisingDepartment.Width = 150;
            // gColSupervisingUserId
            this.gColSupervisingUserId.Caption = "VNW";
            this.gColSupervisingUserId.FieldName = "UserId";
            this.gColSupervisingUserId.Name = "gColSupervisingUserId";
            this.gColSupervisingUserId.Visible = true;
            this.gColSupervisingUserId.VisibleIndex = 1;
            this.gColSupervisingUserId.Width = 140;
            // gColSupervisingUserName
            this.gColSupervisingUserName.Caption = "姓名";
            this.gColSupervisingUserName.FieldName = "UserName";
            this.gColSupervisingUserName.Name = "gColSupervisingUserName";
            this.gColSupervisingUserName.Visible = true;
            this.gColSupervisingUserName.VisibleIndex = 2;
            this.gColSupervisingUserName.Width = 180;
            // gColSupervisingJobTitle
            this.gColSupervisingJobTitle.Caption = "職務";
            this.gColSupervisingJobTitle.FieldName = "JobTitle";
            this.gColSupervisingJobTitle.Name = "gColSupervisingJobTitle";
            this.gColSupervisingJobTitle.Visible = true;
            this.gColSupervisingJobTitle.VisibleIndex = 3;
            this.gColSupervisingJobTitle.Width = 180;
            // gcData: three detail tabs for each plan.
            this.gcData.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.gcData.EmbeddedNavigator.Buttons.Remove.Visible = false;
            gridLevelNode1.LevelTemplate = this.gvSupervising;
            gridLevelNode1.RelationName = "督導單位";
            gridLevelNode2.LevelTemplate = this.gvAccompanying;
            gridLevelNode2.RelationName = "陪集合";
            gridLevelNode3.LevelTemplate = this.gvFiles;
            gridLevelNode3.RelationName = "檔案";
            this.gcData.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1,
            gridLevelNode2,
            gridLevelNode3});
            this.gcData.Location = new System.Drawing.Point(12, 12);
            this.gcData.MainView = this.gvData;
            this.gcData.Name = "gcData";
            this.gcData.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gcData.Size = new System.Drawing.Size(1268, 458);
            this.gcData.TabIndex = 4;
            this.gcData.UseEmbeddedNavigator = true;
            this.gcData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSupervising,
            this.gvAccompanying,
            this.gvFiles,
            this.gvData});
            // gvAccompanying - 陪集合
            this.gvAccompanying.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvAccompanying.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            this.gvAccompanying.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvAccompanying.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvAccompanying.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvAccompanying.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvAccompanying.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvAccompanying.Appearance.Row.Options.UseFont = true;
            this.gvAccompanying.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColAccompanyingDepartment,
            this.gColAccompanyingUserId,
            this.gColAccompanyingUserName,
            this.gColAccompanyingJobTitle});
            this.gvAccompanying.GridControl = this.gcData;
            this.gvAccompanying.Name = "gvAccompanying";
            this.gvAccompanying.ViewCaption = "陪集合";
            this.gvAccompanying.OptionsBehavior.Editable = false;
            this.gvAccompanying.OptionsView.ColumnAutoWidth = false;
            this.gvAccompanying.OptionsView.EnableAppearanceOddRow = true;
            this.gvAccompanying.OptionsView.ShowAutoFilterRow = true;
            this.gvAccompanying.OptionsView.ShowGroupPanel = false;
            // gColAccompanyingDepartment
            this.gColAccompanyingDepartment.Caption = "部門";
            this.gColAccompanyingDepartment.FieldName = "DepartmentName";
            this.gColAccompanyingDepartment.Name = "gColAccompanyingDepartment";
            this.gColAccompanyingDepartment.Visible = true;
            this.gColAccompanyingDepartment.VisibleIndex = 0;
            this.gColAccompanyingDepartment.Width = 150;
            // gColAccompanyingUserId
            this.gColAccompanyingUserId.Caption = "VNW";
            this.gColAccompanyingUserId.FieldName = "UserId";
            this.gColAccompanyingUserId.Name = "gColAccompanyingUserId";
            this.gColAccompanyingUserId.Visible = true;
            this.gColAccompanyingUserId.VisibleIndex = 1;
            this.gColAccompanyingUserId.Width = 140;
            // gColAccompanyingUserName
            this.gColAccompanyingUserName.Caption = "姓名";
            this.gColAccompanyingUserName.FieldName = "UserName";
            this.gColAccompanyingUserName.Name = "gColAccompanyingUserName";
            this.gColAccompanyingUserName.Visible = true;
            this.gColAccompanyingUserName.VisibleIndex = 2;
            this.gColAccompanyingUserName.Width = 180;
            // gColAccompanyingJobTitle
            this.gColAccompanyingJobTitle.Caption = "職務";
            this.gColAccompanyingJobTitle.FieldName = "JobTitle";
            this.gColAccompanyingJobTitle.Name = "gColAccompanyingJobTitle";
            this.gColAccompanyingJobTitle.Visible = true;
            this.gColAccompanyingJobTitle.VisibleIndex = 3;
            this.gColAccompanyingJobTitle.Width = 180;
            // gvFiles - 檔案
            this.gvFiles.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvFiles.Appearance.HeaderPanel.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            this.gvFiles.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvFiles.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvFiles.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvFiles.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvFiles.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvFiles.Appearance.Row.Options.UseFont = true;
            this.gvFiles.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColFileName,
            this.gColUploadedBy,
            this.gColUploadedAt});
            this.gvFiles.GridControl = this.gcData;
            this.gvFiles.Name = "gvFiles";
            this.gvFiles.ViewCaption = "檔案";
            this.gvFiles.OptionsBehavior.Editable = false;
            this.gvFiles.OptionsView.ColumnAutoWidth = false;
            this.gvFiles.OptionsView.EnableAppearanceOddRow = true;
            this.gvFiles.OptionsView.ShowAutoFilterRow = true;
            this.gvFiles.OptionsView.ShowGroupPanel = false;
            // gColFileName
            this.gColFileName.Caption = "檔案名稱";
            this.gColFileName.FieldName = "FileName";
            this.gColFileName.Name = "gColFileName";
            this.gColFileName.Visible = true;
            this.gColFileName.VisibleIndex = 0;
            this.gColFileName.Width = 360;
            // gColUploadedBy
            this.gColUploadedBy.Caption = "上傳人";
            this.gColUploadedBy.FieldName = "UploadedByName";
            this.gColUploadedBy.Name = "gColUploadedBy";
            this.gColUploadedBy.Visible = true;
            this.gColUploadedBy.VisibleIndex = 1;
            this.gColUploadedBy.Width = 180;
            // gColUploadedAt
            this.gColUploadedAt.Caption = "上傳時間";
            this.gColUploadedAt.FieldName = "UploadedAt";
            this.gColUploadedAt.Name = "gColUploadedAt";
            this.gColUploadedAt.Visible = true;
            this.gColUploadedAt.VisibleIndex = 2;
            this.gColUploadedAt.Width = 200;
            this.gColUploadedAt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gColUploadedAt.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            // 
            // gvData
            // 
            this.gvData.Appearance.FooterPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvData.Appearance.FooterPanel.Options.UseFont = true;
            this.gvData.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gvData.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.gvData.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.gvData.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvData.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gvData.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gvData.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvData.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvData.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gvData.Appearance.Row.Options.UseFont = true;
            this.gvData.Appearance.Row.Options.UseForeColor = true;
            this.gvData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColIdMaterial,
            this.gColStatus309,
            this.gridColumn4,
            this.gColReplacementCode309,
            this.gColReplacementDate309,
            this.gColPhoto309,
            this.gridColumn1,
            this.gColDisplayName,
            this.gridColumn3,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn15,
            this.gridColumn16,
            this.gridColumn17,
            this.gridColumn18,
            this.gColUserMngr,
            this.gridColumn6});
            this.gvData.GridControl = this.gcData;
            this.gvData.Name = "gvData";
            this.gvData.OptionsScrollAnnotations.ShowSelectedRows = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gvData.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.gvData.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gvData.OptionsDetail.ShowDetailTabs = true;
            this.gvData.OptionsView.ColumnAutoWidth = false;
            this.gvData.OptionsView.EnableAppearanceOddRow = true;
            this.gvData.OptionsView.ShowAutoFilterRow = true;
            this.gvData.OptionsView.ShowGroupPanel = false;
            // 
            // gColIdMaterial
            // 
            this.gColIdMaterial.Caption = "Id";
            this.gColIdMaterial.FieldName = "Id";
            this.gColIdMaterial.Name = "gColIdMaterial";
            // 
            // gColStatus309
            // 
            this.gColStatus309.Caption = "稽核時間";
            this.gColStatus309.FieldName = "AuditAt";
            this.gColStatus309.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gColStatus309.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gColStatus309.Name = "gColStatus309";
            this.gColStatus309.Visible = true;
            this.gColStatus309.VisibleIndex = 0;
            this.gColStatus309.Width = 140;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "計劃";
            this.gridColumn4.FieldName = "DisplayName";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            this.gridColumn4.Width = 260;
            // 
            // gColReplacementCode309
            // 
            this.gColReplacementCode309.Caption = "替代料號";
            this.gColReplacementCode309.FieldName = "ReplacementMaterialCode";
            this.gColReplacementCode309.Name = "gColReplacementCode309";
            this.gColReplacementCode309.Visible = false;
            this.gColReplacementCode309.VisibleIndex = -1;
            this.gColReplacementCode309.Width = 120;
            // 
            // gColReplacementDate309
            // 
            this.gColReplacementDate309.Caption = "替代日期";
            this.gColReplacementDate309.DisplayFormat.FormatString = "yyyy/MM/dd";
            this.gColReplacementDate309.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gColReplacementDate309.FieldName = "ReplacementDate";
            this.gColReplacementDate309.Name = "gColReplacementDate309";
            this.gColReplacementDate309.Width = 120;
            // 
            // gColPhoto309
            // 
            this.gColPhoto309.AppearanceCell.Options.UseTextOptions = true;
            this.gColPhoto309.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gColPhoto309.Caption = "計劃表";
            this.gColPhoto309.FieldName = "PlanFileName";
            this.gColPhoto309.Name = "gColPhoto309";
            this.gColPhoto309.Visible = true;
            this.gColPhoto309.VisibleIndex = 4;
            this.gColPhoto309.Width = 240;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "稽核項目";
            this.gridColumn1.FieldName = "AuditItemName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 2;
            this.gridColumn1.Width = 240;
            // 
            // gColDisplayName
            // 
            this.gColDisplayName.Caption = "週期";
            this.gColDisplayName.FieldName = "CycleName";
            this.gColDisplayName.MaxWidth = 600;
            this.gColDisplayName.Name = "gColDisplayName";
            this.gColDisplayName.Visible = true;
            this.gColDisplayName.VisibleIndex = 3;
            this.gColDisplayName.Width = 120;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "用途";
            this.gridColumn3.FieldName = "data.TypeUse";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = false;
            this.gridColumn3.VisibleIndex = -1;
            this.gridColumn3.Width = 87;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "安全數量";
            this.gridColumn13.FieldName = "data.MinQuantity";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = false;
            this.gridColumn13.VisibleIndex = -1;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "課庫數量";
            this.gridColumn14.FieldName = "data.QuantityInStorage";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = false;
            this.gridColumn14.VisibleIndex = -1;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "機邊庫數量";
            this.gridColumn15.FieldName = "data.QuantityInMachine";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = false;
            this.gridColumn15.VisibleIndex = -1;
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "料位";
            this.gridColumn16.FieldName = "data.Location";
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.Visible = false;
            this.gridColumn16.VisibleIndex = -1;
            // 
            // gridColumn17
            // 
            this.gridColumn17.Caption = "單價(VNĐ)";
            this.gridColumn17.DisplayFormat.FormatString = "N0";
            this.gridColumn17.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn17.FieldName = "data.Price";
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.Visible = false;
            this.gridColumn17.VisibleIndex = -1;
            // 
            // gridColumn18
            // 
            this.gridColumn18.Caption = "總金額(VNĐ)";
            this.gridColumn18.DisplayFormat.FormatString = "#,##0";
            this.gridColumn18.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridColumn18.FieldName = "TotalPrice";
            this.gridColumn18.Name = "gridColumn18";
            this.gridColumn18.UnboundDataType = typeof(double);
            this.gridColumn18.Visible = false;
            this.gridColumn18.VisibleIndex = -1;
            // 
            // gColUserMngr
            // 
            this.gColUserMngr.Caption = "管理人";
            this.gColUserMngr.FieldName = "UserMngr";
            this.gColUserMngr.Name = "gColUserMngr";
            this.gColUserMngr.Visible = false;
            this.gColUserMngr.VisibleIndex = -1;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "有效日到";
            this.gridColumn6.DisplayFormat.FormatString = "yyyy/MM/dd";
            this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn6.FieldName = "data.ExpDate";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = false;
            this.gridColumn6.VisibleIndex = -1;
            this.gridColumn6.Width = 94;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gcData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1292, 482);
            this.layoutControl1.TabIndex = 6;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1292, 482);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gcData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1272, 462);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // barManagerTP
            // 
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnAdd,
            this.btnReload,
            this.barCbbDept,
            this.btnExportExcel,
            this.btnExcelBySpare,
            this.btnExcelByMachine,
            this.btnExcelByNotify});
            this.barManagerTP.MainMenu = this.bar2;
            this.barManagerTP.MaxItemId = 17;
            this.barManagerTP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cbbDept});
            // 
            // bar2
            // 
            this.bar2.BarAppearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.bar2.BarAppearance.Disabled.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Hovered.Options.UseFont = true;
            this.bar2.BarAppearance.Hovered.Options.UseForeColor = true;
            this.bar2.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Normal.Options.UseFont = true;
            this.bar2.BarAppearance.Normal.Options.UseForeColor = true;
            this.bar2.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.bar2.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.bar2.BarAppearance.Pressed.Options.UseFont = true;
            this.bar2.BarAppearance.Pressed.Options.UseForeColor = true;
            this.bar2.BarName = "Main menu";
            this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAdd, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExportExcel, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barCbbDept
            // 
            this.barCbbDept.Caption = "單位";
            this.barCbbDept.Edit = this.cbbDept;
            this.barCbbDept.Id = 11;
            this.barCbbDept.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.barCbbDept.Name = "barCbbDept";
            this.barCbbDept.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // cbbDept
            // 
            this.cbbDept.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbDept.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbDept.Appearance.Options.UseFont = true;
            this.cbbDept.Appearance.Options.UseForeColor = true;
            this.cbbDept.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbDept.AppearanceDropDown.Options.UseFont = true;
            this.cbbDept.AutoHeight = false;
            this.cbbDept.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDept.Name = "cbbDept";
            this.cbbDept.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "新增";
            this.btnAdd.Id = 0;
            this.btnAdd.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAdd.ImageOptions.SvgImage")));
            this.btnAdd.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnAdd.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnAdd.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnAdd.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ItemAppearance.Normal.Options.UseFont = true;
            this.btnAdd.Name = "btnAdd";
            // 
            // btnReload
            // 
            this.btnReload.Caption = "刷新";
            this.btnReload.Id = 1;
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnReload.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.Name = "btnReload";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Caption = "導出表單";
            this.btnExportExcel.Id = 13;
            this.btnExportExcel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExportExcel.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExportExcel.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExportExcel.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportExcel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExportExcel.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExcelBySpare, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExcelByMachine, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExcelByNotify, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.btnExportExcel.Name = "btnExportExcel";
            // 
            // btnExcelBySpare
            // 
            this.btnExcelBySpare.Caption = "按照備品";
            this.btnExcelBySpare.Id = 14;
            this.btnExcelBySpare.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExcelBySpare.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExcelBySpare.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExcelBySpare.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcelBySpare.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExcelBySpare.Name = "btnExcelBySpare";
            // 
            // btnExcelByMachine
            // 
            this.btnExcelByMachine.Caption = "按照設備";
            this.btnExcelByMachine.Id = 15;
            this.btnExcelByMachine.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExcelByMachine.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExcelByMachine.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExcelByMachine.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcelByMachine.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExcelByMachine.Name = "btnExcelByMachine";
            // 
            // btnExcelByNotify
            // 
            this.btnExcelByNotify.Caption = "提醒備品";
            this.btnExcelByNotify.Id = 16;
            this.btnExcelByNotify.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnExcelByNotify.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnExcelByNotify.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnExcelByNotify.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcelByNotify.ItemAppearance.Normal.Options.UseFont = true;
            this.btnExcelByNotify.Name = "btnExcelByNotify";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1292, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 531);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(1292, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 482);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1292, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 482);
            // 
            // uc320_mainjihua
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "uc320_mainjihua";
            this.Size = new System.Drawing.Size(1292, 531);
            ((System.ComponentModel.ISupportInitialize)(this.gvSupervising)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAccompanying)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDept)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSupervising;
        private DevExpress.XtraGrid.Columns.GridColumn gColSupervisingDepartment;
        private DevExpress.XtraGrid.Columns.GridColumn gColSupervisingUserId;
        private DevExpress.XtraGrid.Columns.GridColumn gColSupervisingUserName;
        private DevExpress.XtraGrid.Columns.GridColumn gColSupervisingJobTitle;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAccompanying;
        private DevExpress.XtraGrid.Columns.GridColumn gColAccompanyingDepartment;
        private DevExpress.XtraGrid.Columns.GridColumn gColAccompanyingUserId;
        private DevExpress.XtraGrid.Columns.GridColumn gColAccompanyingUserName;
        private DevExpress.XtraGrid.Columns.GridColumn gColAccompanyingJobTitle;
        private DevExpress.XtraGrid.Views.Grid.GridView gvFiles;
        private DevExpress.XtraGrid.Columns.GridColumn gColFileName;
        private DevExpress.XtraGrid.Columns.GridColumn gColUploadedBy;
        private DevExpress.XtraGrid.Columns.GridColumn gColUploadedAt;
        private DevExpress.XtraGrid.GridControl gcData;

        private DevExpress.XtraGrid.Views.Grid.GridView gvData;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gColDisplayName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gColReplacementCode309;
        private DevExpress.XtraGrid.Columns.GridColumn gColReplacementDate309;
        private DevExpress.XtraGrid.Columns.GridColumn gColPhoto309;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
        private DevExpress.XtraGrid.Columns.GridColumn gColUserMngr;
        private DevExpress.XtraGrid.Columns.GridColumn gColIdMaterial;


        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraBars.BarEditItem barCbbDept;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cbbDept;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn gColStatus309;
        private DevExpress.XtraBars.BarSubItem btnExportExcel;
        private DevExpress.XtraBars.BarButtonItem btnExcelBySpare;
        private DevExpress.XtraBars.BarButtonItem btnExcelByMachine;
        private DevExpress.XtraBars.BarButtonItem btnExcelByNotify;
    }
}
