using BusinessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_EquipmentHistory : XtraForm
    {
        public f310_EquipmentHistory(string equipmentCode, List<dt310_EquipmentInfoBUS.EquipmentHistory> history)
        {
            InitializeComponent();
            Text = "設備交接紀錄";
            btnClose.Caption = "關閉";
            btnClose.ImageOptions.SvgImage = TPSvgimages.Close;

            var rows = history ?? new List<dt310_EquipmentInfoBUS.EquipmentHistory>();
            lblSummary.Text = $"設備編號：{equipmentCode}　　交接紀錄：{rows.Count} 筆";
            ConfigureGrid();
            gcData.DataSource = rows;
            Shown += (sender, args) => gvData.BestFitColumns();
        }

        private void ConfigureGrid()
        {
            gvData.OptionsBehavior.AutoPopulateColumns = false;
            gvData.ReadOnlyGridView();
            gvData.OptionsBehavior.Editable = false;
            gvData.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvData.OptionsView.ColumnAutoWidth = false;
            gvData.OptionsView.EnableAppearanceOddRow = true;
            gvData.OptionsView.ShowGroupPanel = false;
            gvData.OptionsView.ShowAutoFilterRow = true;

            string[] fields = { "ChangedAt", "PreviousManager", "NewManager", "PreviousDeptId", "NewDeptId", "ChangedBy", "HandoverId" };
            string[] captions = { "交接時間", "原管理人", "接任人", "原部門", "新部門", "經辦人", "交接單號" };
            int[] widths = { 160, 140, 140, 85, 85, 140, 105 };
            for (int i = 0; i < fields.Length; i++) gvData.Columns.AddVisible(fields[i], captions[i]).Width = widths[i];
            gvData.Columns["ChangedAt"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gvData.Columns["ChangedAt"].DisplayFormat.FormatString = "yyyy/MM/dd HH:mm";
        }

        private void f310_EquipmentHistory_Load(object sender, EventArgs e)
        {
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private void btnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }
    }
}
