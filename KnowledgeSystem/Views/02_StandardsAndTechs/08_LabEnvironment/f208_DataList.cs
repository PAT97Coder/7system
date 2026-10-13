using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._08_LabEnvironment
{
    public partial class f208_DataList : XtraForm
    {
        internal static readonly Dictionary<string, string> Captions = new Dictionary<string, string>
        {
            {"Id", "紀錄編號"}, {"SN", "SN"}, {"DisplayName", "設備名稱"}, {"Model", "型號"}, {"IdDept", "部門"},
            {"Location", "位置"}, {"IdManager", "管理人代碼"}, {"ManagerName", "管理人"}, {"MeasuredAt", "測量時間"}, {"ReceivedAt", "接收時間"},
            {"Temperature", "溫度 °C"}, {"Humidity", "濕度 %RH"}, {"TempMin", "溫度下限"}, {"TempMax", "溫度上限"},
            {"HumidityMin", "濕度下限"}, {"HumidityMax", "濕度上限"}, {"ExpectedMinutes", "逾時 (分鐘)"}, {"DeviceState", "設備狀態"},
            {"EffectiveFrom", "生效時間"}, {"ChangedBy", "變更人"}, {"Reason", "變更原因"}, {"Cause", "原因"}, {"Action", "處理措施"},
            {"Result", "處理結果"}, {"IsClosed", "已結案"}, {"CreatedBy", "處理人"}, {"CreatedAt", "處理時間"}, {"ActionAt", "最後處理時間"},
            {"ReadingCount", "數據筆數"}, {"LastMeasuredAt", "最後測量時間"}, {"ConflictCount", "衝突筆數"}, {"Status", "判定"}
        };
        public f208_DataList(string title, DataTable data)
        {
            InitializeComponent(); Text = title; grid.DataSource = data; FormatColumns(view);
        }
        internal static void FormatColumns(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            view.PopulateColumns();
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.Columns)
            {
                if (Captions.TryGetValue(column.FieldName, out string caption)) column.Caption = caption;
                else column.Visible = false;
                if (column.ColumnType == typeof(DateTime)) { column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; column.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss"; }
                if (column.FieldName == "DeviceState")
                {
                    var state = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
                    for (int i = 0; i < BusinessLayer.dt208_EnvironmentRules.DeviceStates.Length; i++) state.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(BusinessLayer.dt208_EnvironmentRules.DeviceStates[i], i));
                    view.GridControl.RepositoryItems.Add(state); column.ColumnEdit = state;
                }
            }
            view.OptionsBehavior.Editable = false; view.OptionsView.ShowAutoFilterRow = true; view.OptionsView.ColumnAutoWidth = false; view.BestFitColumns();
        }
        private void Export_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "溫濕度紀錄.xlsx" })
                if (dialog.ShowDialog() == DialogResult.OK) try { grid.ExportToXlsx(dialog.FileName); } catch (Exception ex) { Helpers.MsgTP.MsgError(ex.Message); }
        }
    }
}
