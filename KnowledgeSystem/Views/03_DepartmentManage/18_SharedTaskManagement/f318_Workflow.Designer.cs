namespace KnowledgeSystem.Views._03_DepartmentManage._18_SharedTaskManagement
{
    partial class f318_Workflow
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.BarButtonItem btnConfirm;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txbTeam, txbOwner, txbTitle;
        private DevExpress.XtraEditors.SimpleButton btnDefaulProgress, btnComplete;
        private DevExpress.XtraEditors.LabelControl lbCurrentStep;
        private DevExpress.XtraEditors.MemoEdit txbComment;
        private DevExpress.XtraGrid.GridControl gcProgress;
        private DevExpress.XtraGrid.Views.Grid.GridView gvProgress;
        private DevExpress.XtraGrid.Columns.GridColumn gColUser, gColDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit lookupUser;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnDelProg;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit lockedCell;
        private DevExpress.XtraLayout.LayoutControlItem lcCurrentStep, lcComment, lcComplete;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        // Bố cục, BarManager và appearance lấy từ f306_NewSignDoc.Designer.cs.
        // Chỉ thay các trường thông tin văn kiện bằng thông tin Án và bỏ nhóm tab.
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            barManagerTP = new DevExpress.XtraBars.BarManager(components);
            var bar2 = new DevExpress.XtraBars.Bar();
            btnConfirm = new DevExpress.XtraBars.BarButtonItem();
            var barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            var barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            var barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            var barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            var root = new DevExpress.XtraLayout.LayoutControlGroup();
            txbTeam = new DevExpress.XtraEditors.TextEdit();
            txbOwner = new DevExpress.XtraEditors.TextEdit();
            txbTitle = new DevExpress.XtraEditors.TextEdit();
            btnDefaulProgress = new DevExpress.XtraEditors.SimpleButton();
            gcProgress = new DevExpress.XtraGrid.GridControl();
            gvProgress = new DevExpress.XtraGrid.Views.Grid.GridView(gcProgress);
            lookupUser = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            var lookupView = new DevExpress.XtraGrid.Views.Grid.GridView();
            btnDelProg = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            lockedCell = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            var lookupRole = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            lbCurrentStep = new DevExpress.XtraEditors.LabelControl();
            txbComment = new DevExpress.XtraEditors.MemoEdit();
            btnComplete = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)barManagerTP).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gcProgress).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gvProgress).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lookupUser).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lookupView).BeginInit();
            SuspendLayout();
            layoutControl1.SuspendLayout();

            var font14 = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            var font12 = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            barManagerTP.Form = this;
            barManagerTP.Bars.Add(bar2);
            barManagerTP.MainMenu = bar2;
            barManagerTP.DockControls.Add(barDockControlTop);
            barManagerTP.DockControls.Add(barDockControlBottom);
            barManagerTP.DockControls.Add(barDockControlLeft);
            barManagerTP.DockControls.Add(barDockControlRight);
            barManagerTP.Items.Add(btnConfirm);
            barManagerTP.MaxItemId = 1;
            bar2.BarName = "Main menu";
            bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            bar2.DockCol = 0;
            bar2.DockRow = 0;
            bar2.BarAppearance.Disabled.Font = font12;
            bar2.BarAppearance.Disabled.Options.UseFont = true;
            foreach (var appearance in new[] { bar2.BarAppearance.Normal, bar2.BarAppearance.Hovered, bar2.BarAppearance.Pressed })
            {
                appearance.Font = font14;
                appearance.ForeColor = System.Drawing.Color.Black;
                appearance.Options.UseFont = true;
                appearance.Options.UseForeColor = true;
            }
            bar2.LinksPersistInfo.Add(new DevExpress.XtraBars.LinkPersistInfo(
                DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, btnConfirm, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph));
            bar2.OptionsBar.AllowQuickCustomization = false;
            bar2.OptionsBar.DrawDragBorder = false;
            bar2.OptionsBar.MultiLine = true;
            bar2.OptionsBar.UseWholeRow = true;
            btnConfirm.Caption = "確認";
            btnConfirm.Id = 0;
            btnConfirm.Name = "btnConfirm";
            btnConfirm.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            foreach (var dock in new[] { barDockControlTop, barDockControlBottom, barDockControlLeft, barDockControlRight })
            {
                dock.Manager = barManagerTP;
                dock.CausesValidation = false;
            }

            layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = root;
            layoutControl1.Location = new System.Drawing.Point(0, 49);
            layoutControl1.Size = new System.Drawing.Size(668, 430);
            root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            root.GroupBordersVisible = false;
            root.TextVisible = false;
            root.Name = "Root";
            root.Size = new System.Drawing.Size(668, 430);
            foreach (var field in new[] { txbTeam, txbOwner, txbTitle })
            {
                field.Properties.Appearance.Font = font14;
                field.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
                field.Properties.Appearance.Options.UseFont = true;
                field.Properties.Appearance.Options.UseForeColor = true;
                field.Properties.ReadOnly = true;
                field.StyleController = layoutControl1;
                field.MenuManager = barManagerTP;
            }
            txbTeam.Name = "txbTeam"; txbOwner.Name = "txbOwner"; txbTitle.Name = "txbTitle";
            btnDefaulProgress.Name = "btnDefaulProgress";
            btnDefaulProgress.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnDefaulProgress.Appearance.ForeColor = System.Drawing.Color.Black;
            btnDefaulProgress.Appearance.Options.UseFont = true;
            btnDefaulProgress.Appearance.Options.UseForeColor = true;
            btnDefaulProgress.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            btnDefaulProgress.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            btnDefaulProgress.Text = "固定流程";
            btnDefaulProgress.StyleController = layoutControl1;
            gcProgress.Name = "gcProgress";
            gcProgress.MainView = gvProgress;
            gcProgress.MenuManager = barManagerTP;
            gcProgress.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            gcProgress.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { lookupUser, lookupRole, btnDelProg, lockedCell });
            gvProgress.Name = "gvProgress";
            gvProgress.OptionsBehavior.AutoPopulateColumns = false;
            gvProgress.Appearance.HeaderPanel.Font = font14;
            gvProgress.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            gvProgress.Appearance.HeaderPanel.Options.UseFont = true;
            gvProgress.Appearance.HeaderPanel.Options.UseForeColor = true;
            gvProgress.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gvProgress.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gvProgress.Appearance.Row.Font = font12;
            gvProgress.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            gvProgress.Appearance.Row.Options.UseFont = true;
            gvProgress.Appearance.Row.Options.UseForeColor = true;
            gvProgress.Appearance.TopNewRow.Font = font12;
            gvProgress.Appearance.TopNewRow.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            gvProgress.Appearance.TopNewRow.Options.UseFont = true;
            gvProgress.Appearance.TopNewRow.Options.UseForeColor = true;
            gvProgress.NewItemRowText = "按一下此處新增流程";
            gvProgress.OptionsCustomization.AllowFilter = false;
            gvProgress.OptionsCustomization.AllowSort = false;
            gvProgress.OptionsCustomization.AllowGroup = false;
            gvProgress.OptionsCustomization.AllowColumnMoving = false;
            gvProgress.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            gvProgress.OptionsView.EnableAppearanceOddRow = true;
            gvProgress.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            gvProgress.OptionsView.ShowGroupPanel = false;
            gvProgress.IndicatorWidth = 35;
            var job = gvProgress.Columns.AddVisible("JobName", "職務"); job.OptionsColumn.AllowEdit = false;
            gColUser = gvProgress.Columns.AddVisible("IdUser", "工號"); gColUser.ColumnEdit = lookupUser;
            var name = gvProgress.Columns.AddVisible("UserName", "名稱"); name.OptionsColumn.AllowEdit = false;
            var role = gvProgress.Columns.AddVisible("Permission", "權限"); role.ColumnEdit = lookupRole;
            gColDelete = gvProgress.Columns.AddVisible("DeleteText", " "); gColDelete.ColumnEdit = btnDelProg;
            gColDelete.MinWidth = 30; gColDelete.MaxWidth = 30; gColDelete.Width = 30;
            lookupUser.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            lookupUser.Appearance.Font = lookupUser.AppearanceDropDown.Font = font12;
            lookupUser.Appearance.ForeColor = lookupUser.AppearanceDropDown.ForeColor = System.Drawing.Color.Black;
            lookupUser.Appearance.Options.UseFont = lookupUser.AppearanceDropDown.Options.UseFont = true;
            lookupUser.Appearance.Options.UseForeColor = lookupUser.AppearanceDropDown.Options.UseForeColor = true;
            lookupUser.AutoHeight = false;
            lookupUser.ValueMember = lookupUser.DisplayMember = "Id";
            lookupUser.NullText = "";
            lookupUser.PopupView = lookupView;
            lookupUser.PopupFormSize = new System.Drawing.Size(380, 300);
            lookupView.OptionsBehavior.AutoPopulateColumns = false;
            lookupView.Appearance.HeaderPanel.Font = lookupView.Appearance.Row.Font = font12;
            lookupView.Appearance.HeaderPanel.ForeColor = lookupView.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            lookupView.Appearance.HeaderPanel.Options.UseFont = lookupView.Appearance.Row.Options.UseFont = true;
            lookupView.Appearance.HeaderPanel.Options.UseForeColor = lookupView.Appearance.Row.Options.UseForeColor = true;
            lookupView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            lookupView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lookupView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            lookupView.OptionsSelection.EnableAppearanceFocusedCell = false;
            lookupView.OptionsView.EnableAppearanceOddRow = true;
            lookupView.OptionsView.ShowAutoFilterRow = true;
            lookupView.OptionsView.ShowGroupPanel = false;
            lookupView.Columns.AddVisible("Department", "單位").Width = 65;
            lookupView.Columns.AddVisible("Id", "工號").Width = 125;
            lookupView.Columns.AddVisible("Name", "名稱").Width = 120;
            lookupRole.AutoHeight = false;
            lookupRole.Appearance.Font = lookupRole.AppearanceDropDown.Font = font12;
            lookupRole.Appearance.Options.UseFont = lookupRole.AppearanceDropDown.Options.UseFont = true;
            lookupRole.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            lookupRole.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("確認", "確認"));
            btnDelProg.AutoHeight = false;
            btnDelProg.Buttons.Clear();
            btnDelProg.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Close));
            btnDelProg.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            lockedCell.ReadOnly = true;
            lbCurrentStep.Appearance.Font = font12;
            lbCurrentStep.Appearance.Options.UseFont = true;
            lbCurrentStep.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            txbComment.Properties.Appearance.Font = font12;
            txbComment.Properties.Appearance.Options.UseFont = true;
            txbComment.Properties.MaxLength = 2000;
            txbComment.Properties.NullValuePrompt = "處理／確認意見（選填）";
            btnComplete.Appearance.Font = font12;
            btnComplete.Appearance.Options.UseFont = true;
            foreach (var control in new System.Windows.Forms.Control[] { txbTeam, txbOwner, txbTitle, btnDefaulProgress, gcProgress, lbCurrentStep, txbComment, btnComplete })
                layoutControl1.Controls.Add(control);

            var lcTeam = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcTeam", Control = txbTeam, Text = "課別", Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(280, 36) };
            var lcOwner = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcOwner", Control = txbOwner, Text = "負責人", Location = new System.Drawing.Point(280, 0), Size = new System.Drawing.Size(368, 36) };
            var lcTitle = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcTitle", Control = txbTitle, Text = "名稱", Location = new System.Drawing.Point(0, 36), Size = new System.Drawing.Size(648, 36) };
            foreach (var item in new[] { lcTeam, lcOwner, lcTitle })
            {
                item.AppearanceItemCaption.Font = font14;
                item.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                item.AppearanceItemCaption.Options.UseFont = true;
                item.AppearanceItemCaption.Options.UseForeColor = true;
                item.TextSize = new System.Drawing.Size(57, 24);
                item.MinSize = new System.Drawing.Size(150, 36);
                item.MaxSize = new System.Drawing.Size(0, 36);
                item.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            }
            var lcFixed = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcFixed", Control = btnDefaulProgress, TextVisible = false, Location = new System.Drawing.Point(0, 72), Size = new System.Drawing.Size(143, 40) };
            lcFixed.MinSize = lcFixed.MaxSize = new System.Drawing.Size(143, 40);
            lcFixed.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            var space = new DevExpress.XtraLayout.EmptySpaceItem { AllowHotTrack = false, Location = new System.Drawing.Point(143, 72), Size = new System.Drawing.Size(505, 40) };
            var lcGrid = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcGrid", Control = gcProgress, TextVisible = false, Location = new System.Drawing.Point(0, 112), Size = new System.Drawing.Size(648, 210) };
            lcCurrentStep = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcCurrentStep", Control = lbCurrentStep, TextVisible = false, Location = new System.Drawing.Point(0, 322), Size = new System.Drawing.Size(648, 28) };
            lcCurrentStep.MinSize = new System.Drawing.Size(100, 28); lcCurrentStep.MaxSize = new System.Drawing.Size(0, 28); lcCurrentStep.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            lcComment = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcComment", Control = txbComment, TextVisible = false, Location = new System.Drawing.Point(0, 350), Size = new System.Drawing.Size(493, 60) };
            lcComment.MinSize = new System.Drawing.Size(100, 60); lcComment.MaxSize = new System.Drawing.Size(0, 60); lcComment.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            lcComplete = new DevExpress.XtraLayout.LayoutControlItem { Name = "lcComplete", Control = btnComplete, TextVisible = false, Location = new System.Drawing.Point(493, 350), Size = new System.Drawing.Size(155, 60) };
            lcComplete.MinSize = lcComplete.MaxSize = new System.Drawing.Size(155, 60); lcComplete.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lcTeam, lcOwner, lcTitle, lcFixed, space, lcGrid, lcCurrentStep, lcComment, lcComplete });

            Appearance.BackColor = System.Drawing.Color.FromArgb(244, 244, 244);
            Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(668, 479);
            Controls.Add(layoutControl1);
            Controls.Add(barDockControlLeft); Controls.Add(barDockControlRight);
            Controls.Add(barDockControlBottom); Controls.Add(barDockControlTop);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            IconOptions.Image = global::KnowledgeSystem.Properties.Resources.AppIcon;
            Name = "f318_Workflow";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)lookupView).EndInit();
            ((System.ComponentModel.ISupportInitialize)lookupUser).EndInit();
            ((System.ComponentModel.ISupportInitialize)gvProgress).EndInit();
            ((System.ComponentModel.ISupportInitialize)gcProgress).EndInit();
            ((System.ComponentModel.ISupportInitialize)root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManagerTP).EndInit();
            layoutControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
