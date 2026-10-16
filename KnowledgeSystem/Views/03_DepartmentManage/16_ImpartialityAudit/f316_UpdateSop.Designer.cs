namespace KnowledgeSystem.Views._03_DepartmentManage._16_ImpartialityAudit
{
    partial class f316_UpdateSop
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txbPdfPath = new DevExpress.XtraEditors.ButtonEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcPdfPath = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lcSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcCancel = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbPdfPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPdfPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txbPdfPath);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(640, 116);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCancel.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.btnCancel.Location = new System.Drawing.Point(518, 54);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 50);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSave.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.btnSave.Location = new System.Drawing.Point(404, 54);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 50);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txbPdfPath
            // 
            this.txbPdfPath.Location = new System.Drawing.Point(91, 12);
            this.txbPdfPath.Name = "txbPdfPath";
            this.txbPdfPath.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txbPdfPath.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbPdfPath.Properties.Appearance.Options.UseFont = true;
            this.txbPdfPath.Properties.Appearance.Options.UseForeColor = true;
            this.txbPdfPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)});
            this.txbPdfPath.Properties.ReadOnly = true;
            this.txbPdfPath.Size = new System.Drawing.Size(537, 28);
            this.txbPdfPath.StyleController = this.layoutControl1;
            this.txbPdfPath.TabIndex = 4;
            this.txbPdfPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txbPdfPath_ButtonClick);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcPdfPath,
            this.emptySpaceItem1,
            this.lcSave,
            this.lcCancel});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(640, 116);
            this.Root.TextVisible = false;
            // 
            // lcPdfPath
            // 
            this.lcPdfPath.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lcPdfPath.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lcPdfPath.AppearanceItemCaption.Options.UseFont = true;
            this.lcPdfPath.AppearanceItemCaption.Options.UseForeColor = true;
            this.lcPdfPath.Control = this.txbPdfPath;
            this.lcPdfPath.Location = new System.Drawing.Point(0, 0);
            this.lcPdfPath.Name = "lcPdfPath";
            this.lcPdfPath.Size = new System.Drawing.Size(620, 32);
            this.lcPdfPath.Text = "PDF 檔案";
            this.lcPdfPath.TextSize = new System.Drawing.Size(67, 20);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 32);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(392, 64);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lcSave
            // 
            this.lcSave.Control = this.btnSave;
            this.lcSave.Location = new System.Drawing.Point(392, 32);
            this.lcSave.MaxSize = new System.Drawing.Size(114, 54);
            this.lcSave.MinSize = new System.Drawing.Size(114, 54);
            this.lcSave.Name = "lcSave";
            this.lcSave.Size = new System.Drawing.Size(114, 64);
            this.lcSave.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lcSave.TextSize = new System.Drawing.Size(0, 0);
            this.lcSave.TextVisible = false;
            // 
            // lcCancel
            // 
            this.lcCancel.Control = this.btnCancel;
            this.lcCancel.Location = new System.Drawing.Point(506, 32);
            this.lcCancel.MaxSize = new System.Drawing.Size(114, 54);
            this.lcCancel.MinSize = new System.Drawing.Size(114, 54);
            this.lcCancel.Name = "lcCancel";
            this.lcCancel.Size = new System.Drawing.Size(114, 64);
            this.lcCancel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lcCancel.TextSize = new System.Drawing.Size(0, 0);
            this.lcCancel.TextVisible = false;
            // 
            // f316_UpdateSop
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(640, 116);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "f316_UpdateSop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "更新SOP";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txbPdfPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcPdfPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.ButtonEdit txbPdfPath;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraLayout.LayoutControlItem lcPdfPath;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lcSave;
        private DevExpress.XtraLayout.LayoutControlItem lcCancel;
    }
}
