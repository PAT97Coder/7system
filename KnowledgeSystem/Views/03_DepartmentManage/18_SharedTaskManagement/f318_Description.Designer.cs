namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    partial class f318_Description
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txbSubject = new DevExpress.XtraEditors.TextEdit();
            this.lblDueDate = new DevExpress.XtraEditors.LabelControl();
            this.memDescription = new DevExpress.XtraEditors.MemoEdit();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbSubject.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txbSubject, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDueDate, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.memDescription, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(12);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            //
            // txbSubject
            //
            this.txbSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txbSubject.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F, System.Drawing.FontStyle.Bold);
            this.txbSubject.Properties.Appearance.Options.UseFont = true;
            this.txbSubject.Properties.ReadOnly = true;
            //
            // lblDueDate
            //
            this.lblDueDate.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 11F);
            this.lblDueDate.Appearance.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblDueDate.Appearance.Options.UseFont = true;
            this.lblDueDate.Appearance.Options.UseForeColor = true;
            this.lblDueDate.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // memDescription
            //
            this.memDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memDescription.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.memDescription.Properties.Appearance.Options.UseFont = true;
            this.memDescription.Properties.ReadOnly = true;
            //
            // btnClose
            //
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Size = new System.Drawing.Size(110, 38);
            this.btnClose.Text = "關閉";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // f318_Description
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimizeBox = false;
            this.Name = "f318_Description";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "查看說明";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbSubject.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memDescription.Properties)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.TextEdit txbSubject;
        private DevExpress.XtraEditors.LabelControl lblDueDate;
        private DevExpress.XtraEditors.MemoEdit memDescription;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
