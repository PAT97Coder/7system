namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    partial class f318_TaskInfo
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.barMain = new DevExpress.XtraBars.Bar();
            this.btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            this.btnSelectOxps = new DevExpress.XtraBars.BarButtonItem();
            this.btnRecognize = new DevExpress.XtraBars.BarButtonItem();
            this.btnPasteAttachment = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddAttachment = new DevExpress.XtraBars.BarButtonItem();
            this.btnRemoveAttachment = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.groupInfo = new DevExpress.XtraEditors.GroupControl();
            this.tableInfo = new System.Windows.Forms.TableLayoutPanel();
            this.lblOxpsSource = new DevExpress.XtraEditors.LabelControl();
            this.txbOxpsSource = new DevExpress.XtraEditors.ButtonEdit();
            this.lblSubject = new DevExpress.XtraEditors.LabelControl();
            this.txbSubject = new DevExpress.XtraEditors.TextEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.memDescription = new DevExpress.XtraEditors.MemoEdit();
            this.lblDueDate = new DevExpress.XtraEditors.LabelControl();
            this.dtDueDate = new DevExpress.XtraEditors.DateEdit();
            this.lblAssignee = new DevExpress.XtraEditors.LabelControl();
            this.cbAssignee = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblSharedRoot = new DevExpress.XtraEditors.LabelControl();
            this.txbSharedRoot = new DevExpress.XtraEditors.ButtonEdit();
            this.lblTargetFolder = new DevExpress.XtraEditors.LabelControl();
            this.txbTargetFolder = new DevExpress.XtraEditors.TextEdit();
            this.lblReadStatus = new DevExpress.XtraEditors.LabelControl();
            this.groupAttachments = new DevExpress.XtraEditors.GroupControl();
            this.gcAttachments = new DevExpress.XtraGrid.GridControl();
            this.gvAttachments = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupInfo)).BeginInit();
            this.groupInfo.SuspendLayout();
            this.tableInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbOxpsSource.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSubject.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDueDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDueDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAssignee.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSharedRoot.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTargetFolder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupAttachments)).BeginInit();
            this.groupAttachments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcAttachments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAttachments)).BeginInit();
            this.SuspendLayout();
            // 
            // barManagerTP
            // 
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMain});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnConfirm,
            this.btnSelectOxps,
            this.btnRecognize,
            this.btnPasteAttachment,
            this.btnAddAttachment,
            this.btnRemoveAttachment});
            this.barManagerTP.MainMenu = this.barMain;
            this.barManagerTP.MaxItemId = 6;
            // 
            // barMain
            // 
            this.barMain.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 13F);
            this.barMain.BarAppearance.Normal.Options.UseFont = true;
            this.barMain.BarName = "Main menu";
            this.barMain.DockCol = 0;
            this.barMain.DockRow = 0;
            this.barMain.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMain.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnSelectOxps, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRecognize, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnPasteAttachment, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAddAttachment, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRemoveAttachment, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.barMain.OptionsBar.AllowQuickCustomization = false;
            this.barMain.OptionsBar.DrawDragBorder = false;
            this.barMain.OptionsBar.UseWholeRow = true;
            this.barMain.Text = "Main menu";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 0;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            // 
            // btnSelectOxps
            // 
            this.btnSelectOxps.Caption = "選擇OXPS";
            this.btnSelectOxps.Id = 1;
            this.btnSelectOxps.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnSelectOxps.Name = "btnSelectOxps";
            this.btnSelectOxps.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSelectOxps_ItemClick);
            // 
            // btnRecognize
            // 
            this.btnRecognize.Caption = "讀取內容";
            this.btnRecognize.Id = 2;
            this.btnRecognize.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnRecognize.Name = "btnRecognize";
            this.btnRecognize.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRecognize_ItemClick);
            // 
            // btnPasteAttachment
            // 
            this.btnPasteAttachment.Caption = "貼上附件";
            this.btnPasteAttachment.Id = 3;
            this.btnPasteAttachment.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnPasteAttachment.Name = "btnPasteAttachment";
            this.btnPasteAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPasteAttachment_ItemClick);
            // 
            // btnAddAttachment
            // 
            this.btnAddAttachment.Caption = "選擇附件";
            this.btnAddAttachment.Id = 4;
            this.btnAddAttachment.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnAddAttachment.Name = "btnAddAttachment";
            this.btnAddAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddAttachment_ItemClick);
            // 
            // btnRemoveAttachment
            // 
            this.btnRemoveAttachment.Caption = "移除附件";
            this.btnRemoveAttachment.Id = 5;
            this.btnRemoveAttachment.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnRemoveAttachment.Name = "btnRemoveAttachment";
            this.btnRemoveAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRemoveAttachment_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1180, 39);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 709);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(1180, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 39);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 670);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1180, 39);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 670);
            // 
            // groupInfo
            // 
            this.groupInfo.AppearanceCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.groupInfo.AppearanceCaption.Options.UseFont = true;
            this.groupInfo.Controls.Add(this.tableInfo);
            this.groupInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupInfo.Location = new System.Drawing.Point(0, 39);
            this.groupInfo.Name = "groupInfo";
            this.groupInfo.Size = new System.Drawing.Size(1180, 401);
            this.groupInfo.TabIndex = 1;
            this.groupInfo.Text = "讀取結果與工作資料";
            // 
            // tableInfo
            // 
            this.tableInfo.ColumnCount = 2;
            this.tableInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.tableInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableInfo.Controls.Add(this.lblOxpsSource, 0, 0);
            this.tableInfo.Controls.Add(this.txbOxpsSource, 1, 0);
            this.tableInfo.Controls.Add(this.lblSubject, 0, 1);
            this.tableInfo.Controls.Add(this.txbSubject, 1, 1);
            this.tableInfo.Controls.Add(this.lblDescription, 0, 2);
            this.tableInfo.Controls.Add(this.memDescription, 1, 2);
            this.tableInfo.Controls.Add(this.lblDueDate, 0, 3);
            this.tableInfo.Controls.Add(this.dtDueDate, 1, 3);
            this.tableInfo.Controls.Add(this.lblAssignee, 0, 4);
            this.tableInfo.Controls.Add(this.cbAssignee, 1, 4);
            this.tableInfo.Controls.Add(this.lblSharedRoot, 0, 5);
            this.tableInfo.Controls.Add(this.txbSharedRoot, 1, 5);
            this.tableInfo.Controls.Add(this.lblTargetFolder, 0, 6);
            this.tableInfo.Controls.Add(this.txbTargetFolder, 1, 6);
            this.tableInfo.Controls.Add(this.lblReadStatus, 1, 7);
            this.tableInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableInfo.Location = new System.Drawing.Point(2, 23);
            this.tableInfo.Name = "tableInfo";
            this.tableInfo.Padding = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.tableInfo.RowCount = 8;
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableInfo.Size = new System.Drawing.Size(1176, 376);
            this.tableInfo.TabIndex = 0;
            // 
            // lblOxpsSource
            // 
            this.lblOxpsSource.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblOxpsSource.Appearance.Options.UseFont = true;
            this.lblOxpsSource.Appearance.Options.UseTextOptions = true;
            this.lblOxpsSource.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblOxpsSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOxpsSource.Location = new System.Drawing.Point(13, 12);
            this.lblOxpsSource.Name = "lblOxpsSource";
            this.lblOxpsSource.Size = new System.Drawing.Size(99, 29);
            this.lblOxpsSource.TabIndex = 0;
            this.lblOxpsSource.Text = "來源 OXPS*";
            // 
            // txbOxpsSource
            // 
            this.txbOxpsSource.AllowDrop = true;
            this.txbOxpsSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbOxpsSource.Location = new System.Drawing.Point(118, 12);
            this.txbOxpsSource.Name = "txbOxpsSource";
            this.txbOxpsSource.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.txbOxpsSource.Properties.Appearance.Options.UseFont = true;
            this.txbOxpsSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.txbOxpsSource.Properties.NullValuePrompt = "Ctrl+V、拖放檔案，或點選右側按鈕選擇 OXPS";
            this.txbOxpsSource.Properties.ReadOnly = true;
            this.txbOxpsSource.Size = new System.Drawing.Size(1045, 28);
            this.txbOxpsSource.TabIndex = 1;
            this.txbOxpsSource.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbOxpsSource_ButtonClick);
            this.txbOxpsSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.txbOxpsSource_DragDrop);
            this.txbOxpsSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.txbOxpsSource_DragEnter);
            // 
            // lblSubject
            // 
            this.lblSubject.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblSubject.Appearance.Options.UseFont = true;
            this.lblSubject.Appearance.Options.UseTextOptions = true;
            this.lblSubject.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubject.Location = new System.Drawing.Point(13, 47);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(99, 29);
            this.lblSubject.TabIndex = 2;
            this.lblSubject.Text = "主旨*";
            // 
            // txbSubject
            // 
            this.txbSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbSubject.Location = new System.Drawing.Point(118, 47);
            this.txbSubject.Name = "txbSubject";
            this.txbSubject.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbSubject.Properties.Appearance.Options.UseFont = true;
            this.txbSubject.Size = new System.Drawing.Size(1045, 28);
            this.txbSubject.TabIndex = 3;
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblDescription.Appearance.Options.UseFont = true;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(13, 82);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(99, 110);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "說明*";
            // 
            // memDescription
            // 
            this.memDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memDescription.Location = new System.Drawing.Point(118, 82);
            this.memDescription.Name = "memDescription";
            this.memDescription.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.memDescription.Properties.Appearance.Options.UseFont = true;
            this.memDescription.Size = new System.Drawing.Size(1045, 110);
            this.memDescription.TabIndex = 5;
            // 
            // lblDueDate
            // 
            this.lblDueDate.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblDueDate.Appearance.Options.UseFont = true;
            this.lblDueDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDueDate.Location = new System.Drawing.Point(13, 198);
            this.lblDueDate.Name = "lblDueDate";
            this.lblDueDate.Size = new System.Drawing.Size(99, 29);
            this.lblDueDate.TabIndex = 6;
            this.lblDueDate.Text = "到期日*";
            // 
            // dtDueDate
            // 
            this.dtDueDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtDueDate.EditValue = new System.DateTime(2026, 10, 2, 0, 0, 0, 0);
            this.dtDueDate.Location = new System.Drawing.Point(118, 198);
            this.dtDueDate.Name = "dtDueDate";
            this.dtDueDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.dtDueDate.Properties.Appearance.Options.UseFont = true;
            this.dtDueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDueDate.Size = new System.Drawing.Size(1045, 28);
            this.dtDueDate.TabIndex = 7;
            this.dtDueDate.EditValueChanged += new System.EventHandler(this.FolderPreview_EditValueChanged);
            // 
            // lblAssignee
            // 
            this.lblAssignee.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblAssignee.Appearance.Options.UseFont = true;
            this.lblAssignee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAssignee.Location = new System.Drawing.Point(13, 233);
            this.lblAssignee.Name = "lblAssignee";
            this.lblAssignee.Size = new System.Drawing.Size(99, 29);
            this.lblAssignee.TabIndex = 8;
            this.lblAssignee.Text = "負責人";
            // 
            // cbAssignee
            // 
            this.cbAssignee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAssignee.Location = new System.Drawing.Point(118, 233);
            this.cbAssignee.Name = "cbAssignee";
            this.cbAssignee.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.cbAssignee.Properties.Appearance.Options.UseFont = true;
            this.cbAssignee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbAssignee.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbAssignee.Size = new System.Drawing.Size(1045, 28);
            this.cbAssignee.TabIndex = 9;
            // 
            // lblSharedRoot
            // 
            this.lblSharedRoot.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblSharedRoot.Appearance.Options.UseFont = true;
            this.lblSharedRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSharedRoot.Location = new System.Drawing.Point(13, 268);
            this.lblSharedRoot.Name = "lblSharedRoot";
            this.lblSharedRoot.Size = new System.Drawing.Size(99, 29);
            this.lblSharedRoot.TabIndex = 10;
            this.lblSharedRoot.Text = "共用根目錄*";
            // 
            // txbSharedRoot
            // 
            this.txbSharedRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbSharedRoot.Location = new System.Drawing.Point(118, 268);
            this.txbSharedRoot.Name = "txbSharedRoot";
            this.txbSharedRoot.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.txbSharedRoot.Properties.Appearance.Options.UseFont = true;
            this.txbSharedRoot.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.txbSharedRoot.Size = new System.Drawing.Size(1045, 28);
            this.txbSharedRoot.TabIndex = 11;
            this.txbSharedRoot.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbSharedRoot_ButtonClick);
            this.txbSharedRoot.EditValueChanged += new System.EventHandler(this.FolderPreview_EditValueChanged);
            // 
            // lblTargetFolder
            // 
            this.lblTargetFolder.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblTargetFolder.Appearance.Options.UseFont = true;
            this.lblTargetFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetFolder.Location = new System.Drawing.Point(13, 303);
            this.lblTargetFolder.Name = "lblTargetFolder";
            this.lblTargetFolder.Size = new System.Drawing.Size(99, 29);
            this.lblTargetFolder.TabIndex = 12;
            this.lblTargetFolder.Text = "目標資料夾";
            // 
            // txbTargetFolder
            // 
            this.txbTargetFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbTargetFolder.Location = new System.Drawing.Point(118, 303);
            this.txbTargetFolder.Name = "txbTargetFolder";
            this.txbTargetFolder.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.txbTargetFolder.Properties.Appearance.Options.UseFont = true;
            this.txbTargetFolder.Properties.ReadOnly = true;
            this.txbTargetFolder.Size = new System.Drawing.Size(1045, 26);
            this.txbTargetFolder.TabIndex = 13;
            // 
            // lblReadStatus
            // 
            this.lblReadStatus.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.lblReadStatus.Appearance.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblReadStatus.Appearance.Options.UseFont = true;
            this.lblReadStatus.Appearance.Options.UseForeColor = true;
            this.lblReadStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReadStatus.Location = new System.Drawing.Point(118, 338);
            this.lblReadStatus.Name = "lblReadStatus";
            this.lblReadStatus.Size = new System.Drawing.Size(1045, 26);
            this.lblReadStatus.TabIndex = 14;
            // 
            // groupAttachments
            // 
            this.groupAttachments.AppearanceCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.groupAttachments.AppearanceCaption.Options.UseFont = true;
            this.groupAttachments.Controls.Add(this.gcAttachments);
            this.groupAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupAttachments.Location = new System.Drawing.Point(0, 440);
            this.groupAttachments.Name = "groupAttachments";
            this.groupAttachments.Size = new System.Drawing.Size(1180, 269);
            this.groupAttachments.TabIndex = 0;
            this.groupAttachments.Text = "附件（可由檔案總管複製後 Ctrl+V，或直接拖放）";
            // 
            // gcAttachments
            // 
            this.gcAttachments.AllowDrop = true;
            this.gcAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcAttachments.Location = new System.Drawing.Point(2, 23);
            this.gcAttachments.MainView = this.gvAttachments;
            this.gcAttachments.Name = "gcAttachments";
            this.gcAttachments.Size = new System.Drawing.Size(1176, 244);
            this.gcAttachments.TabIndex = 0;
            this.gcAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvAttachments});
            this.gcAttachments.DragDrop += new System.Windows.Forms.DragEventHandler(this.gcAttachments_DragDrop);
            this.gcAttachments.DragEnter += new System.Windows.Forms.DragEventHandler(this.gcAttachments_DragEnter);
            // 
            // gvAttachments
            // 
            this.gvAttachments.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvAttachments.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvAttachments.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.gvAttachments.Appearance.Row.Options.UseFont = true;
            this.gvAttachments.DetailHeight = 327;
            this.gvAttachments.GridControl = this.gcAttachments;
            this.gvAttachments.Name = "gvAttachments";
            // 
            // f318_TaskInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 709);
            this.Controls.Add(this.groupAttachments);
            this.Controls.Add(this.groupInfo);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1000, 680);
            this.Name = "f318_TaskInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新增交辦事項";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.f318_TaskInfo_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupInfo)).EndInit();
            this.groupInfo.ResumeLayout(false);
            this.tableInfo.ResumeLayout(false);
            this.tableInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbOxpsSource.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSubject.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDueDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDueDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAssignee.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbSharedRoot.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbTargetFolder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupAttachments)).EndInit();
            this.groupAttachments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcAttachments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvAttachments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar barMain;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarButtonItem btnSelectOxps;
        private DevExpress.XtraBars.BarButtonItem btnRecognize;
        private DevExpress.XtraBars.BarButtonItem btnPasteAttachment;
        private DevExpress.XtraBars.BarButtonItem btnAddAttachment;
        private DevExpress.XtraBars.BarButtonItem btnRemoveAttachment;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.GroupControl groupInfo;
        private System.Windows.Forms.TableLayoutPanel tableInfo;
        private DevExpress.XtraEditors.LabelControl lblOxpsSource;
        private DevExpress.XtraEditors.ButtonEdit txbOxpsSource;
        private DevExpress.XtraEditors.LabelControl lblSubject;
        private DevExpress.XtraEditors.TextEdit txbSubject;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.MemoEdit memDescription;
        private DevExpress.XtraEditors.LabelControl lblDueDate;
        private DevExpress.XtraEditors.DateEdit dtDueDate;
        private DevExpress.XtraEditors.LabelControl lblAssignee;
        private DevExpress.XtraEditors.ComboBoxEdit cbAssignee;
        private DevExpress.XtraEditors.LabelControl lblSharedRoot;
        private DevExpress.XtraEditors.ButtonEdit txbSharedRoot;
        private DevExpress.XtraEditors.LabelControl lblTargetFolder;
        private DevExpress.XtraEditors.TextEdit txbTargetFolder;
        private DevExpress.XtraEditors.LabelControl lblReadStatus;
        private DevExpress.XtraEditors.GroupControl groupAttachments;
        private DevExpress.XtraGrid.GridControl gcAttachments;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAttachments;
    }
}
