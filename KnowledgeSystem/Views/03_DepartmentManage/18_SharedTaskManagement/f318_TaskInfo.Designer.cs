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
            this.btnPasteImage = new DevExpress.XtraBars.BarButtonItem();
            this.btnRecognize = new DevExpress.XtraBars.BarButtonItem();
            this.btnPasteAttachment = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddAttachment = new DevExpress.XtraBars.BarButtonItem();
            this.btnRemoveAttachment = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.splitMain = new DevExpress.XtraEditors.SplitContainerControl();
            this.groupImage = new DevExpress.XtraEditors.GroupControl();
            this.picSource = new DevExpress.XtraEditors.PictureEdit();
            this.groupInfo = new DevExpress.XtraEditors.GroupControl();
            this.tableInfo = new System.Windows.Forms.TableLayoutPanel();
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
            this.lblOcrStatus = new DevExpress.XtraEditors.LabelControl();
            this.groupAttachments = new DevExpress.XtraEditors.GroupControl();
            this.gcAttachments = new DevExpress.XtraGrid.GridControl();
            this.gvAttachments = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel1)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel2)).BeginInit();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupImage)).BeginInit();
            this.groupImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSource.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupInfo)).BeginInit();
            this.groupInfo.SuspendLayout();
            this.tableInfo.SuspendLayout();
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
            // barManagerTP / barMain
            //
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] { this.barMain });
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
                this.btnConfirm, this.btnPasteImage, this.btnRecognize,
                this.btnPasteAttachment, this.btnAddAttachment, this.btnRemoveAttachment });
            this.barManagerTP.MainMenu = this.barMain;
            this.barManagerTP.MaxItemId = 6;
            this.barMain.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 13F);
            this.barMain.BarAppearance.Normal.Options.UseFont = true;
            this.barMain.BarName = "Main menu";
            this.barMain.DockCol = 0;
            this.barMain.DockRow = 0;
            this.barMain.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMain.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnPasteImage, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRecognize, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnPasteAttachment, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAddAttachment, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
                new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnRemoveAttachment, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph) });
            this.barMain.OptionsBar.AllowQuickCustomization = false;
            this.barMain.OptionsBar.DrawDragBorder = false;
            this.barMain.OptionsBar.UseWholeRow = true;
            //
            // buttons
            //
            this.btnConfirm.Caption = "確認";
            this.btnConfirm.Id = 0;
            this.btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnConfirm.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnConfirm_ItemClick);
            this.btnPasteImage.Caption = "貼上圖片";
            this.btnPasteImage.Id = 1;
            this.btnPasteImage.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnPasteImage.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPasteImage_ItemClick);
            this.btnRecognize.Caption = "辨識圖片";
            this.btnRecognize.Id = 2;
            this.btnRecognize.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnRecognize.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRecognize_ItemClick);
            this.btnPasteAttachment.Caption = "貼上附件";
            this.btnPasteAttachment.Id = 3;
            this.btnPasteAttachment.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnPasteAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnPasteAttachment_ItemClick);
            this.btnAddAttachment.Caption = "選擇附件";
            this.btnAddAttachment.Id = 4;
            this.btnAddAttachment.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnAddAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddAttachment_ItemClick);
            this.btnRemoveAttachment.Caption = "移除附件";
            this.btnRemoveAttachment.Id = 5;
            this.btnRemoveAttachment.ImageOptions.SvgImageSize = new System.Drawing.Size(30, 30);
            this.btnRemoveAttachment.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRemoveAttachment_ItemClick);
            //
            // dock controls
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1180, 47);
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Manager = this.barManagerTP;
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitMain.Location = new System.Drawing.Point(0, 47);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.groupImage);
            this.splitMain.Panel2.Controls.Add(this.groupInfo);
            this.splitMain.Size = new System.Drawing.Size(1180, 430);
            this.splitMain.SplitterPosition = 500;
            this.splitMain.TabIndex = 4;
            //
            // groupImage / picSource
            //
            this.groupImage.AppearanceCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.groupImage.AppearanceCaption.Options.UseFont = true;
            this.groupImage.Controls.Add(this.picSource);
            this.groupImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupImage.Text = "來源圖片（Ctrl+V 或拖放圖片）";
            this.picSource.AllowDrop = true;
            this.picSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picSource.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picSource.DragDrop += new System.Windows.Forms.DragEventHandler(this.picSource_DragDrop);
            this.picSource.DragEnter += new System.Windows.Forms.DragEventHandler(this.picSource_DragEnter);
            //
            // groupInfo / tableInfo
            //
            this.groupInfo.AppearanceCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.groupInfo.AppearanceCaption.Options.UseFont = true;
            this.groupInfo.Controls.Add(this.tableInfo);
            this.groupInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupInfo.Text = "辨識結果與工作資料";
            this.tableInfo.ColumnCount = 2;
            this.tableInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.tableInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableInfo.Controls.Add(this.lblSubject, 0, 0);
            this.tableInfo.Controls.Add(this.txbSubject, 1, 0);
            this.tableInfo.Controls.Add(this.lblDescription, 0, 1);
            this.tableInfo.Controls.Add(this.memDescription, 1, 1);
            this.tableInfo.Controls.Add(this.lblDueDate, 0, 2);
            this.tableInfo.Controls.Add(this.dtDueDate, 1, 2);
            this.tableInfo.Controls.Add(this.lblAssignee, 0, 3);
            this.tableInfo.Controls.Add(this.cbAssignee, 1, 3);
            this.tableInfo.Controls.Add(this.lblSharedRoot, 0, 4);
            this.tableInfo.Controls.Add(this.txbSharedRoot, 1, 4);
            this.tableInfo.Controls.Add(this.lblTargetFolder, 0, 5);
            this.tableInfo.Controls.Add(this.txbTargetFolder, 1, 5);
            this.tableInfo.Controls.Add(this.lblOcrStatus, 1, 6);
            this.tableInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableInfo.Padding = new System.Windows.Forms.Padding(10);
            this.tableInfo.RowCount = 7;
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            //
            // labels and editors
            //
            this.lblSubject.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblSubject.Appearance.Options.UseFont = true;
            this.lblSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubject.Text = "主旨*";
            this.lblSubject.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.txbSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbSubject.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbSubject.Properties.Appearance.Options.UseFont = true;
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblDescription.Appearance.Options.UseFont = true;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Text = "說明*";
            this.memDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memDescription.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.memDescription.Properties.Appearance.Options.UseFont = true;
            this.lblDueDate.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblDueDate.Appearance.Options.UseFont = true;
            this.lblDueDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDueDate.Text = "到期日*";
            this.dtDueDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtDueDate.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.dtDueDate.Properties.Appearance.Options.UseFont = true;
            this.dtDueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.dtDueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.dtDueDate.EditValueChanged += new System.EventHandler(this.FolderPreview_EditValueChanged);
            this.lblAssignee.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblAssignee.Appearance.Options.UseFont = true;
            this.lblAssignee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAssignee.Text = "負責人";
            this.cbAssignee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbAssignee.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.cbAssignee.Properties.Appearance.Options.UseFont = true;
            this.cbAssignee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.cbAssignee.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.lblSharedRoot.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblSharedRoot.Appearance.Options.UseFont = true;
            this.lblSharedRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSharedRoot.Text = "共用根目錄*";
            this.txbSharedRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbSharedRoot.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.txbSharedRoot.Properties.Appearance.Options.UseFont = true;
            this.txbSharedRoot.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis) });
            this.txbSharedRoot.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbSharedRoot_ButtonClick);
            this.txbSharedRoot.EditValueChanged += new System.EventHandler(this.FolderPreview_EditValueChanged);
            this.lblTargetFolder.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblTargetFolder.Appearance.Options.UseFont = true;
            this.lblTargetFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetFolder.Text = "目標資料夾";
            this.txbTargetFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbTargetFolder.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.txbTargetFolder.Properties.Appearance.Options.UseFont = true;
            this.txbTargetFolder.Properties.ReadOnly = true;
            this.lblOcrStatus.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.lblOcrStatus.Appearance.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblOcrStatus.Appearance.Options.UseFont = true;
            this.lblOcrStatus.Appearance.Options.UseForeColor = true;
            this.lblOcrStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // groupAttachments / grid
            //
            this.groupAttachments.AppearanceCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.groupAttachments.AppearanceCaption.Options.UseFont = true;
            this.groupAttachments.Controls.Add(this.gcAttachments);
            this.groupAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupAttachments.Location = new System.Drawing.Point(0, 477);
            this.groupAttachments.Text = "附件（可由檔案總管複製後 Ctrl+V，或直接拖放）";
            this.gcAttachments.AllowDrop = true;
            this.gcAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcAttachments.MainView = this.gvAttachments;
            this.gcAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gvAttachments });
            this.gcAttachments.DragDrop += new System.Windows.Forms.DragEventHandler(this.gcAttachments_DragDrop);
            this.gcAttachments.DragEnter += new System.Windows.Forms.DragEventHandler(this.gcAttachments_DragEnter);
            this.gvAttachments.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.gvAttachments.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvAttachments.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.gvAttachments.Appearance.Row.Options.UseFont = true;
            this.gvAttachments.GridControl = this.gcAttachments;
            //
            // f318_TaskInfo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.Controls.Add(this.groupAttachments);
            this.Controls.Add(this.splitMain);
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
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel1)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain.Panel2)).EndInit();
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupImage)).EndInit();
            this.groupImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSource.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupInfo)).EndInit();
            this.groupInfo.ResumeLayout(false);
            this.tableInfo.ResumeLayout(false);
            this.tableInfo.PerformLayout();
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
        private DevExpress.XtraBars.BarButtonItem btnPasteImage;
        private DevExpress.XtraBars.BarButtonItem btnRecognize;
        private DevExpress.XtraBars.BarButtonItem btnPasteAttachment;
        private DevExpress.XtraBars.BarButtonItem btnAddAttachment;
        private DevExpress.XtraBars.BarButtonItem btnRemoveAttachment;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SplitContainerControl splitMain;
        private DevExpress.XtraEditors.GroupControl groupImage;
        private DevExpress.XtraEditors.PictureEdit picSource;
        private DevExpress.XtraEditors.GroupControl groupInfo;
        private System.Windows.Forms.TableLayoutPanel tableInfo;
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
        private DevExpress.XtraEditors.LabelControl lblOcrStatus;
        private DevExpress.XtraEditors.GroupControl groupAttachments;
        private DevExpress.XtraGrid.GridControl gcAttachments;
        private DevExpress.XtraGrid.Views.Grid.GridView gvAttachments;
    }
}
