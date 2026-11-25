namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class f312_SettingExam
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SpinEdit txbTime, txbPassScore, txbTotalQues, txbCountEmp;
        private DevExpress.XtraEditors.SpinEdit choicePercentEdit, essayPercentEdit;
        private DevExpress.XtraEditors.SpinEdit choiceScoreEdit, essayScoreEdit;
        private DevExpress.XtraEditors.LabelControl ratioPreview;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraBars.BarDockControl barDockControlTop, barDockControlBottom, barDockControlLeft, barDockControlRight;

        protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            txbTime = new DevExpress.XtraEditors.SpinEdit();
            txbPassScore = new DevExpress.XtraEditors.SpinEdit();
            txbTotalQues = new DevExpress.XtraEditors.SpinEdit();
            txbCountEmp = new DevExpress.XtraEditors.SpinEdit();
            choicePercentEdit = new DevExpress.XtraEditors.SpinEdit();
            essayPercentEdit = new DevExpress.XtraEditors.SpinEdit();
            choiceScoreEdit = new DevExpress.XtraEditors.SpinEdit();
            essayScoreEdit = new DevExpress.XtraEditors.SpinEdit();
            ratioPreview = new DevExpress.XtraEditors.LabelControl();
            barManagerTP = new DevExpress.XtraBars.BarManager(components);
            bar2 = new DevExpress.XtraBars.Bar();
            btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManagerTP).BeginInit();
            layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl1.Name = "layoutControl1";
            layoutControl1.AllowCustomization = false;
            layoutControl1.Root = Root;
            Root.Name = "Root";
            Root.GroupBordersVisible = false;
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.Padding = new DevExpress.XtraLayout.Utils.Padding(10);
            AddSettingRow(txbTime, "txbTime", "時期限制", 1, 32767, "分鐘");
            AddSettingRow(txbPassScore, "txbPassScore", "及格分數", 0, 100, "/100");
            AddSettingRow(txbTotalQues, "txbTotalQues", "題目數量", 1, 32767);
            AddSettingRow(txbCountEmp, "txbCountEmp", "參與人數", 1, 32767);
            AddSettingRow(choicePercentEdit, "choicePercentEdit", "選擇題比例", 0, 100, "%");
            AddSettingRow(essayPercentEdit, "essayPercentEdit", "申論題比例", 0, 100, "%");
            choicePercentEdit.EditValue = 70m;
            essayPercentEdit.EditValue = 30m;
            AddSettingRow(choiceScoreEdit, "choiceScoreEdit", "選擇題配分（%）", 0, 100);
            AddSettingRow(essayScoreEdit, "essayScoreEdit", "申論題配分（%）", 0, 100);
            choiceScoreEdit.EditValue = 70m; essayScoreEdit.EditValue = 30m;
            foreach (var scoreEdit in new[] { choiceScoreEdit, essayScoreEdit })
            {
                scoreEdit.Properties.ReadOnly = true;
                scoreEdit.Properties.IsFloatValue = true;
                scoreEdit.Properties.Mask.EditMask = "0.##";
                scoreEdit.Properties.Buttons.Clear();
                scoreEdit.TabStop = false;
            }
            ratioPreview.Name = "ratioPreview";
            ratioPreview.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            ratioPreview.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            layoutControl1.Controls.Add(ratioPreview);
            var previewItem = Root.AddItem();
            previewItem.Name = "ratioPreviewItem"; previewItem.Control = ratioPreview; previewItem.TextVisible = false;
            previewItem.Move(Root.Items[Root.Items.Count - 2], DevExpress.XtraLayout.Utils.InsertType.Bottom);
            layoutControl1.GetItemByControl(choiceScoreEdit).Parent.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControl1.GetItemByControl(essayScoreEdit).Parent.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            previewItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            choicePercentEdit.EditValueChanged += ChoicePercentChanged;
            essayPercentEdit.EditValueChanged += EssayPercentChanged;
            txbTotalQues.EditValueChanged += (s, e) => UpdateRatioPreview();
            barManagerTP.Form = this;
            barManagerTP.Bars.Add(bar2); barManagerTP.Items.Add(btnConfirm);
            barManagerTP.DockControls.Add(barDockControlTop); barManagerTP.DockControls.Add(barDockControlBottom);
            barManagerTP.DockControls.Add(barDockControlLeft); barManagerTP.DockControls.Add(barDockControlRight);
            barDockControlTop.Manager = barDockControlBottom.Manager = barDockControlLeft.Manager = barDockControlRight.Manager = barManagerTP;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            bar2.BarName = "考試資訊設定"; bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            bar2.AddItem(btnConfirm); Exam312Ui.StyleBar(bar2);
            btnConfirm.Name = "btnConfirm"; btnConfirm.Caption = "確認"; btnConfirm.Id = 0;
            btnConfirm.ImageOptions.SvgImage = Helpers.TPSvgimages.Confirm;
            Exam312Ui.StyleButton(btnConfirm); btnConfirm.ItemClick += btnConfirm_ItemClick;
            Controls.AddRange(new System.Windows.Forms.Control[] { layoutControl1, barDockControlTop, barDockControlBottom, barDockControlLeft, barDockControlRight });
            layoutControl1.BringToFront();
            ClientSize = new System.Drawing.Size(289, 285);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            MaximizeBox = false; MinimizeBox = false;
            IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            Name = "f312_SettingExam"; Text = "考試資訊設定";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Load += f312_SettingExam_Load;
            ((System.ComponentModel.ISupportInitialize)barManagerTP).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            ResumeLayout(false); PerformLayout();
        }

        private void AddSettingRow(DevExpress.XtraEditors.SpinEdit editor, string name, string caption, int minimum, int maximum, string unit = "")
        {
            editor.Name = name;
            editor.Properties.IsFloatValue = false;
            editor.Properties.MinValue = minimum; editor.Properties.MaxValue = maximum;
            editor.Properties.Mask.EditMask = "d";
            editor.Properties.Buttons.Clear();
            editor.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            editor.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            editor.EditValue = minimum;
            layoutControl1.Controls.Add(editor);
            var row = Root.AddGroup();
            row.Name = name + "Row";
            row.GroupBordersVisible = false;
            row.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
            row.Spacing = new DevExpress.XtraLayout.Utils.Padding(0);
            row.MinSize = new System.Drawing.Size(0, 36);
            row.MaxSize = new System.Drawing.Size(0, 36);
            if (Root.Items.Count > 1) row.Move(Root.Items[Root.Items.Count - 2], DevExpress.XtraLayout.Utils.InsertType.Bottom);
            var item = row.AddItem(caption + (unit == "%" ? "" : "<color=red>*</color>"), editor);
            item.Name = name + "Item";
            item.AllowHtmlStringInCaption = true;
            item.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            item.TextSize = new System.Drawing.Size(105, 24);
            item.TextToControlDistance = 12;
            item.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            item.MinSize = new System.Drawing.Size(200, 36);
            item.MaxSize = new System.Drawing.Size(200, 36);
            item.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            var unitLabel = new DevExpress.XtraEditors.LabelControl { Name = name + "Unit", Text = unit };
            unitLabel.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            layoutControl1.Controls.Add(unitLabel);
            var unitItem = row.AddItem();
            unitItem.Name = name + "UnitItem"; unitItem.Control = unitLabel; unitItem.TextVisible = false;
            unitItem.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 2, 2, 2);
            unitItem.Move(item, DevExpress.XtraLayout.Utils.InsertType.Right);
        }
    }
}
