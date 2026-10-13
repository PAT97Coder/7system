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
            {"Id", "紀錄編號"}, {"SN", "設備序號"}, {"DisplayName", "設備名稱"}, {"Model", "型號"}, {"IdDept", "部門"},
            {"Location", "位置"}, {"IdManager", "管理人代碼"}, {"ManagerName", "管理人"}, {"MeasuredAt", "測量時間"}, {"ReceivedAt", "接收時間"},
            {"Temperature", "溫度 °C"}, {"Humidity", "濕度 %RH"}, {"TempMin", "溫度下限"}, {"TempMax", "溫度上限"},
            {"HumidityMin", "濕度下限"}, {"HumidityMax", "濕度上限"}, {"ExpectedMinutes", "逾時 (分鐘)"}, {"DeviceState", "設備狀態"},
            {"EffectiveFrom", "生效時間"}, {"ChangedBy", "變更人"}, {"Reason", "變更原因"}, {"Cause", "原因"}, {"Action", "處理措施"},
            {"Result", "處理結果"}, {"IsClosed", "已結案"}, {"CreatedBy", "處理人"}, {"CreatedAt", "處理時間"}, {"ActionAt", "最後處理時間"},
            {"ReadingCount", "數據筆數"}, {"LastMeasuredAt", "最後測量時間"}, {"ConflictCount", "衝突筆數"}, {"Status", "判定"}
        };
        public f208_DataList(string title, DataTable data)
        {
            Lab208Ui.InitializeLocalization(); InitializeComponent(); Text = title; grid.DataSource = data; FormatColumns(view);
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
                    foreach (DevExpress.XtraEditors.Repository.RepositoryItem editor in view.GridControl.RepositoryItems)
                        if (editor is DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox) { column.ColumnEdit = editor; break; }
                }
            }
            view.OptionsBehavior.Editable = false; view.OptionsView.ShowAutoFilterRow = true; view.OptionsView.ColumnAutoWidth = false;
            view.OptionsView.ShowGroupPanel = false; view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            view.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.Columns)
            {
                column.Width = column.ColumnType == typeof(DateTime) ? 230 : 160;
                if (column.FieldName == "Reason" || column.FieldName == "Cause" || column.FieldName == "Action" || column.FieldName == "Result" || column.FieldName == "Status") column.Width = 320;
                if (column.ColumnType == typeof(decimal)) { column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric; column.DisplayFormat.FormatString = "0.##"; }
            }
        }
        private void Export_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "溫濕度紀錄.xlsx" })
                if (dialog.ShowDialog() == DialogResult.OK) try { grid.ExportToXlsx(dialog.FileName); } catch (Exception ex) { Helpers.MsgTP.MsgError(ex.Message); }
        }
    }

    internal static class Lab208Ui
    {
        internal static void StyleBarItem(DevExpress.XtraBars.BarItem item)
        {
            item.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            item.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            foreach (var appearance in new[] { item.ItemAppearance.Normal, item.ItemAppearance.Hovered, item.ItemAppearance.Pressed })
            {
                appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
                appearance.ForeColor = System.Drawing.Color.Black;
                appearance.Options.UseFont = appearance.Options.UseForeColor = true;
            }
            item.ItemAppearance.Disabled.Options.UseFont = false;
            item.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            item.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
        }
        internal static void StyleToolbar(DevExpress.XtraBars.Bar toolbar)
        {
            // Cấu hình hiển thị lấy từ bar2 của uc201_AuditISOMain.
            toolbar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            toolbar.DockCol = 0; toolbar.DockRow = 0;
            toolbar.OptionsBar.AllowQuickCustomization = false;
            toolbar.OptionsBar.DrawDragBorder = false;
            toolbar.OptionsBar.MultiLine = true;
            toolbar.OptionsBar.UseWholeRow = true;
            toolbar.BarAppearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 12F);
            toolbar.BarAppearance.Disabled.Options.UseFont = true;
            foreach (var appearance in new[] { toolbar.BarAppearance.Normal, toolbar.BarAppearance.Hovered, toolbar.BarAppearance.Pressed })
            {
                appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
                appearance.ForeColor = System.Drawing.Color.Black;
                appearance.Options.UseFont = appearance.Options.UseForeColor = true;
            }
        }
        internal static void StyleDialogToolbar(DevExpress.XtraBars.Bar toolbar)
        {
            StyleToolbar(toolbar);
            toolbar.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        }
        internal static void StyleDialogBarItem(DevExpress.XtraBars.BarItem item)
        {
            StyleBarItem(item);
            item.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
        }
        internal static void StyleDialogEditor(DevExpress.XtraEditors.BaseEdit editor, DevExpress.XtraLayout.LayoutControl layout, DevExpress.XtraBars.BarManager manager)
        {
            editor.StyleController = layout;
            editor.MenuManager = manager;
            StyleRepository(editor.Properties);
        }
        internal static void StyleDialogItem(DevExpress.XtraLayout.LayoutControlItem item)
        {
            item.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            item.AppearanceItemCaption.ForeColor = item.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.Black;
            item.AppearanceItemCaption.Options.UseFont = item.AppearanceItemCaption.Options.UseForeColor = true;
            item.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            item.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            item.TextToControlDistance = 4;
        }
        internal static void StyleRepository(DevExpress.XtraEditors.Repository.RepositoryItem item)
        {
            item.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            item.Appearance.ForeColor = System.Drawing.Color.Black;
            item.Appearance.Options.UseFont = item.Appearance.Options.UseForeColor = true;
            if (item is DevExpress.XtraEditors.Repository.RepositoryItemPopupBase popup)
            {
                popup.AppearanceDropDown.Font = item.Appearance.Font;
                popup.AppearanceDropDown.Options.UseFont = true;
            }
        }
        internal static DevExpress.Utils.Menu.DXMenuItem Menu(string caption, EventHandler click, DevExpress.Utils.Svg.SvgImage image, bool enabled, bool beginGroup = false)
        {
            var item = new DevExpress.Utils.Menu.DXMenuItem(caption, click, image, DevExpress.Utils.Menu.DXMenuItemPriority.Normal) { Enabled = enabled, BeginGroup = beginGroup };
            item.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            item.Appearance.Font = item.AppearanceHovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            item.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
            return item;
        }
        internal static void InitializeLocalization()
        {
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            if (!(DevExpress.XtraGrid.Localization.GridLocalizer.Active is LabGridLocalizer))
                DevExpress.XtraGrid.Localization.GridLocalizer.Active = new LabGridLocalizer();
            if (!(DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active is LabTreeLocalizer))
                DevExpress.XtraTreeList.Localization.TreeListLocalizer.Active = new LabTreeLocalizer();
        }
        private static readonly Dictionary<string, string> MenuCaptions = new Dictionary<string, string>
        {
            { "Sort Ascending", "遞增排序" }, { "Sort Descending", "遞減排序" }, { "Clear Sorting", "清除排序" },
            { "Group By This Column", "依此欄分組" }, { "Group by This Column", "依此欄分組" },
            { "Group Panel", "分組面板" }, { "Show Group Panel", "顯示分組面板" }, { "Hide Group Panel", "隱藏分組面板" },
            { "Column Chooser", "欄位選擇器" }, { "Column/Band Chooser", "欄位／欄帶選擇器" },
            { "Best Fit", "最佳欄寬" }, { "Best Fit (all columns)", "所有欄位最佳欄寬" },
            { "Best Fit (all Columns)", "所有欄位最佳欄寬" }, { "Filter Editor...", "篩選編輯器…" },
            { "Show Auto Filter Row", "顯示快速篩選列" }, { "Hide Auto Filter Row", "隱藏快速篩選列" },
            { "Clear Filter", "清除篩選" }, { "Filter Editor", "篩選編輯器" },
            { "Expand", "展開" }, { "Collapse", "收合" }, { "Full Expand", "全部展開" }, { "Full Collapse", "全部收合" },
            { "Expand All", "全部展開" }, { "Collapse All", "全部收合" }, { "Show Footer", "顯示頁尾" },
            { "Hide Footer", "隱藏頁尾" }, { "Sum", "總和" }, { "Min", "最小值" }, { "Max", "最大值" },
            { "Count", "筆數" }, { "Average", "平均值" }, { "None", "無" }, { "(All)", "（全部）" },
            { "(Blanks)", "（空白）" }, { "(Non blanks)", "（非空白）" }, { "Select All", "全選" },
            { "OK", "確定" }, { "Cancel", "取消" }, { "Apply", "套用" }, { "Find", "搜尋" }, { "Clear", "清除" },
            { "Copy", "複製" }, { "Copy Cell", "複製儲存格" }, { "Show Search Panel", "顯示搜尋面板" },
            { "Hide Search Panel", "隱藏搜尋面板" }, { "Show Find Panel", "顯示搜尋面板" },
            { "Hide Find Panel", "隱藏搜尋面板" }, { "Fixed Style", "固定欄位" }, { "Left", "左側" }, { "Right", "右側" },
            { "Remove This Column", "移除此欄" }, { "Hide This Column", "隱藏此欄" },
            { "Show This Column", "顯示此欄" }, { "UnGroup", "取消分組" }, { "Can Filter", "允許篩選" },
            { "Show Column Filter", "顯示欄位篩選" }, { "Clear Summary Sorting", "清除摘要排序" },
            { "{1} by '{0}' - {2}", "依「{0}」的{1}－{2}" }, { "Custom", "自訂" },
            { "Sort by Summary", "依摘要排序" }, { "Group Interval", "分組間隔" },
            { "Day", "日" }, { "Month", "月" }, { "Year", "年" }, { "Smart", "智慧分組" },
            { "Group Summary Editor...", "分組摘要編輯器…" }, { "Expression Editor...", "運算式編輯器…" },
            { "Conditional Formatting", "條件格式" }, { "Filter Mode", "篩選模式" }, { "Value", "值" },
            { "Display Text", "顯示文字" }, { "Clear Grouping", "清除分組" }, { "Show Group By Box", "顯示分組面板" },
            { "Hide Group By Box", "隱藏分組面板" }, { "Group By Box", "分組面板" },
            { "Clear All Sorting", "清除所有排序" }, { "Add New Summary", "新增摘要" },
            { "Clear Summary Items", "清除摘要項目" }, { "Split", "分割" }, { "Remove Split", "取消分割" },
            { "Mode", "模式" }, { "All Rows", "全部資料列" }, { "All Nodes", "全部節點" },
            { "Selection", "選取資料" }, { "Mixed", "混合" }, { "Add Node", "新增節點" },
            { "Add Child Node", "新增子節點" }, { "(Custom)", "（自訂）" },
            { "SUM={0:0.##}", "總和={0:0.##}" }, { "SUM={0:#.##}", "總和={0:#.##}" },
            { "MIN={0}", "最小值={0}" }, { "MAX={0}", "最大值={0}" }, { "Count={0}", "筆數={0}" },
            { "AVG={0:0.##}", "平均值={0:0.##}" }, { "AVG={0:#.##}", "平均值={0:#.##}" }, { "Custom={0}", "自訂={0}" }
        };
        private static string Translate(string caption) => MenuCaptions.TryGetValue(caption.Replace("&", ""), out string translated) ? translated : caption;
        private sealed class LabGridLocalizer : DevExpress.XtraGrid.Localization.GridLocalizer
        {
            public override string GetLocalizedString(DevExpress.XtraGrid.Localization.GridStringId id) => Translate(base.GetLocalizedString(id));
        }
        private sealed class LabTreeLocalizer : DevExpress.XtraTreeList.Localization.TreeListLocalizer
        {
            public override string GetLocalizedString(DevExpress.XtraTreeList.Localization.TreeListStringId id) => Translate(base.GetLocalizedString(id));
        }
    }
}
