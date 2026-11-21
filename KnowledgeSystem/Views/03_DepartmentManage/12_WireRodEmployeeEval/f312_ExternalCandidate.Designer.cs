using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    partial class f312_ExternalCandidate
    {
        private System.ComponentModel.IContainer components;
        private LayoutControl layout;
        private LayoutControlGroup root;
        private LayoutControlItem toolbarItem, codeItem, nameItem, departmentItem;
        private EmptySpaceItem remainingSpace;
        private TextEdit codeEdit, nameEdit, departmentEdit;
        private BarManager barManager;
        private Bar toolbar;
        private StandaloneBarDockControl toolbarDock;
        private BarButtonItem btnStart, btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layout = new LayoutControl(); root = new LayoutControlGroup();
            toolbarItem = new LayoutControlItem(); codeItem = new LayoutControlItem(); nameItem = new LayoutControlItem(); departmentItem = new LayoutControlItem();
            remainingSpace = new EmptySpaceItem();
            codeEdit = new TextEdit(); nameEdit = new TextEdit(); departmentEdit = new TextEdit();
            barManager = new BarManager(components); toolbar = new Bar(); toolbarDock = new StandaloneBarDockControl();
            btnStart = new BarButtonItem(); btnCancel = new BarButtonItem();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)layout).BeginInit(); ((System.ComponentModel.ISupportInitialize)root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)barManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)codeEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nameEdit.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)departmentEdit.Properties).BeginInit();
            layout.Name = "layout"; layout.Dock = DockStyle.Fill; layout.AllowCustomization = false; layout.Root = root;
            layout.Controls.AddRange(new Control[] { toolbarDock, codeEdit, nameEdit, departmentEdit });
            root.Name = "root"; root.GroupBordersVisible = false; root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            root.Padding = new DevExpress.XtraLayout.Utils.Padding(16); root.LayoutMode = LayoutMode.Table;
            root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition { SizeType = SizeType.Percent, Width = 100 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Absolute, Height = 64 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Absolute, Height = 58 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Absolute, Height = 58 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Absolute, Height = 58 });
            root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition { SizeType = SizeType.Percent, Height = 100 });
            root.Items.AddRange(new BaseLayoutItem[] { toolbarItem, codeItem, nameItem, departmentItem, remainingSpace });
            toolbarItem.Name = "toolbarItem"; toolbarItem.Control = toolbarDock; toolbarItem.TextVisible = false;
            codeItem.Name = "codeItem"; codeItem.Control = codeEdit; codeItem.Text = "員工編號 <color=red>*</color>"; codeItem.OptionsTableLayoutItem.RowIndex = 1;
            nameItem.Name = "nameItem"; nameItem.Control = nameEdit; nameItem.Text = "姓名 <color=red>*</color>"; nameItem.OptionsTableLayoutItem.RowIndex = 2;
            departmentItem.Name = "departmentItem"; departmentItem.Control = departmentEdit; departmentItem.Text = "部門 <color=red>*</color>"; departmentItem.OptionsTableLayoutItem.RowIndex = 3;
            codeItem.AllowHtmlStringInCaption = nameItem.AllowHtmlStringInCaption = departmentItem.AllowHtmlStringInCaption = true;
            remainingSpace.Name = "remainingSpace"; remainingSpace.OptionsTableLayoutItem.RowIndex = 4;
            codeItem.AppearanceItemCaption.Font = nameItem.AppearanceItemCaption.Font = departmentItem.AppearanceItemCaption.Font = new Font("Microsoft JhengHei UI", 14.25F);
            codeItem.AppearanceItemCaption.Options.UseFont = nameItem.AppearanceItemCaption.Options.UseFont = departmentItem.AppearanceItemCaption.Options.UseFont = true;
            codeEdit.Name = "codeEdit"; nameEdit.Name = "nameEdit"; departmentEdit.Name = "departmentEdit";
            codeEdit.StyleController = nameEdit.StyleController = departmentEdit.StyleController = layout;
            codeEdit.MenuManager = nameEdit.MenuManager = departmentEdit.MenuManager = barManager;
            codeEdit.Properties.Appearance.Font = nameEdit.Properties.Appearance.Font = departmentEdit.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            codeEdit.Properties.Appearance.Options.UseFont = nameEdit.Properties.Appearance.Options.UseFont = departmentEdit.Properties.Appearance.Options.UseFont = true;
            codeEdit.Properties.MaxLength = 64; nameEdit.Properties.MaxLength = departmentEdit.Properties.MaxLength = 128;
            barManager.Form = this; barManager.Bars.Add(toolbar); barManager.Items.AddRange(new BarItem[] { btnStart, btnCancel });
            barManager.DockControls.Add(toolbarDock); toolbarDock.Manager = barManager; toolbarDock.Name = "toolbarDock";
            toolbar.BarName = "外部測驗工具列"; toolbar.DockStyle = BarDockStyle.Standalone; toolbar.StandaloneBarDockControl = toolbarDock;
            toolbar.LinksPersistInfo.AddRange(new LinkPersistInfo[] { new LinkPersistInfo(btnStart), new LinkPersistInfo(btnCancel, true) });
            btnStart.Name = "btnStart"; btnStart.Id = 0; btnStart.Caption = "開始模擬測驗"; btnStart.Enabled = false;
            btnCancel.Name = "btnCancel"; btnCancel.Id = 1; btnCancel.Caption = "取消";
            btnStart.ImageOptions.SvgImage = Helpers.TPSvgimages.Start; btnCancel.ImageOptions.SvgImage = Helpers.TPSvgimages.Cancel;
            Exam312Ui.StyleBar(toolbar); Exam312Ui.StyleButton(btnStart); Exam312Ui.StyleButton(btnCancel);
            btnStart.ItemClick += Start_ItemClick; btnCancel.ItemClick += Cancel_ItemClick;
            Controls.Add(layout); AutoScaleDimensions = new SizeF(7F, 14F); AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 340); FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false; StartPosition = FormStartPosition.CenterParent;
            Name = "f312_ExternalCandidate"; Text = "模擬測驗外部 — 員工資料"; Load += Candidate_Load;
            ((System.ComponentModel.ISupportInitialize)codeEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)nameEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)departmentEdit.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)barManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)root).EndInit(); ((System.ComponentModel.ISupportInitialize)layout).EndInit();
            ResumeLayout(false);
        }
    }
}
