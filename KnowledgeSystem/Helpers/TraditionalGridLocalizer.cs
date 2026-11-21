using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Localization;
using System.Collections.Generic;
using System.Drawing;

namespace KnowledgeSystem.Helpers
{
    // Shared GridView captions for zh-Hant screens; business items retain the original DevExpress menu.
    internal sealed class TraditionalGridLocalizer : GridLocalizer
    {
        private static readonly Font MenuFont = new Font("Microsoft JhengHei UI", 12F);
        private static readonly Dictionary<string, string> Captions = new Dictionary<string, string>
        {
            { "MenuColumnSortAscending", "遞增排序" }, { "MenuColumnSortDescending", "遞減排序" },
            { "MenuColumnClearSorting", "清除排序" }, { "MenuColumnClearAllSorting", "清除所有排序" },
            { "MenuColumnShowColumn", "顯示欄位" }, { "MenuColumnRemoveColumn", "隱藏欄位" },
            { "MenuColumnGroup", "依此欄位分組" }, { "MenuColumnUnGroup", "取消此欄位分組" },
            { "MenuColumnColumnCustomization", "欄位選擇器" }, { "MenuColumnBandCustomization", "欄帶選擇器" },
            { "MenuColumnBestFit", "調整欄寬" }, { "MenuColumnBestFitAllColumns", "調整所有欄寬" },
            { "MenuColumnFilter", "篩選" }, { "MenuColumnClearFilter", "清除篩選" },
            { "MenuColumnShowAutoFilter", "顯示自動篩選列" }, { "MenuColumnAutoFilterRowShow", "顯示自動篩選列" },
            { "MenuColumnAutoFilterRowHide", "隱藏自動篩選列" }, { "MenuColumnFilterEditor", "編輯篩選條件" },
            { "MenuColumnFindFilterShow", "顯示搜尋面板" }, { "MenuColumnFindFilterHide", "隱藏搜尋面板" },
            { "MenuColumnFilterMode", "篩選模式" }, { "MenuColumnFilterModeValue", "值" }, { "MenuColumnFilterModeDisplayText", "顯示文字" },
            { "MenuColumnGroupBox", "分組面板" }, { "MenuGroupPanelShow", "顯示分組面板" }, { "MenuGroupPanelHide", "隱藏分組面板" },
            { "MenuGroupPanelFullExpand", "全部展開" }, { "MenuGroupPanelFullCollapse", "全部收合" }, { "MenuGroupPanelClearGrouping", "清除分組" },
            { "MenuGroupRowExpand", "展開" }, { "MenuGroupRowCollapse", "收合" },
            { "MenuColumnConditionalFormatting", "條件格式" }, { "MenuColumnExpressionEditor", "運算式編輯器" },
            { "MenuColumnGroupSummaryEditor", "分組摘要編輯器" }, { "MenuColumnSortGroupBySummaryMenu", "依摘要排序" },
            { "MenuColumnResetGroupSummarySort", "重設摘要排序" }, { "MenuColumnGroupSummarySortFormat", "{0}（{1}）" },
            { "MenuColumnGroupIntervalMenu", "分組間隔" }, { "MenuColumnGroupIntervalNone", "無" },
            { "MenuColumnGroupIntervalDay", "日" }, { "MenuColumnGroupIntervalMonth", "月" }, { "MenuColumnGroupIntervalYear", "年" }, { "MenuColumnGroupIntervalSmart", "自動" },
            { "MenuColumnSumSummaryTypeDescription", "總和" }, { "MenuColumnMinSummaryTypeDescription", "最小值" },
            { "MenuColumnMaxSummaryTypeDescription", "最大值" }, { "MenuColumnCountSummaryTypeDescription", "筆數" },
            { "MenuColumnAverageSummaryTypeDescription", "平均值" }, { "MenuColumnCustomSummaryTypeDescription", "自訂" },
            { "MenuFooterSum", "總和" }, { "MenuFooterMin", "最小值" }, { "MenuFooterMax", "最大值" }, { "MenuFooterCount", "筆數" },
            { "MenuFooterAverage", "平均值" }, { "MenuFooterNone", "無" }, { "MenuFooterCustomFormat", "自訂格式" },
            { "MenuFooterAddSummaryItem", "新增摘要" }, { "MenuFooterClearSummaryItems", "清除摘要" },
            { "MenuFooterShow", "顯示摘要列" }, { "MenuFooterHide", "隱藏摘要列" }, { "MenuFooterMode", "摘要模式" },
            { "MenuFooterAllRows", "所有資料列" }, { "MenuFooterSelection", "選取資料列" }, { "MenuFooterMixed", "混合" },
            { "MenuFooterSumFormat", "總和={0}" }, { "MenuFooterMinFormat", "最小值={0}" }, { "MenuFooterMaxFormat", "最大值={0}" },
            { "MenuFooterCountFormat", "筆數={0}" }, { "MenuFooterAverageFormat", "平均值={0}" }, { "MenuFooterCountGroupFormat", "筆數={0}" },
            { "MenuShowSplitItem", "分割檢視" }, { "MenuHideSplitItem", "取消分割" },
            { "GridGroupPanelText", "將欄位標題拖曳至此處分組" },
            { "PopupFilterAll", "全部" }, { "PopupFilterCustom", "自訂" }, { "PopupFilterBlanks", "空白" }, { "PopupFilterNonBlanks", "非空白" },
            { "FilterPanelCustomizeButton", "編輯篩選" }, { "FindNullPrompt", "輸入搜尋文字…" },
            { "FindControlFindButton", "搜尋" }, { "FindControlClearButton", "清除" }, { "FindControlNextButton", "下一筆" }, { "FindControlPrevButton", "上一筆" },
            { "FilterBuilderOkButton", "確定" }, { "FilterBuilderCancelButton", "取消" }, { "FilterBuilderApplyButton", "套用" }, { "FilterBuilderCaption", "篩選編輯器" },
            { "CustomFilterDialogFormCaption", "自訂篩選" }, { "CustomFilterDialogCaption", "顯示符合條件的資料列" },
            { "CustomFilterDialogRadioAnd", "且" }, { "CustomFilterDialogRadioOr", "或" },
            { "CustomFilterDialogOkButton", "確定" }, { "CustomFilterDialogCancelButton", "取消" },
            { "CustomFilterDialogClearFilter", "清除篩選" }, { "CustomFilterDialog2FieldCheck", "篩選第二個欄位" },
            { "CustomFilterDialogEmptyValue", "請輸入值" }, { "CustomFilterDialogEmptyOperator", "請選擇條件" },
            { "CustomFilterDialogHint", "使用條件篩選資料。" },
            { "ColumnChooserCaption", "欄位選擇器" }, { "ColumnChooserDragText", "將欄位拖曳至此處隱藏" },
            { "LayoutViewLabelShowFilterPanel", "顯示篩選面板" }, { "CardViewQuickCustomizationButtonFilter", "篩選" }
        };

        internal static void Install()
        {
            if (!(Active is TraditionalGridLocalizer)) Active = new TraditionalGridLocalizer();
            AppearanceObject.DefaultMenuFont = MenuFont;
        }

        public override string GetLocalizedString(GridStringId id) =>
            Captions.TryGetValue(id.ToString(), out var caption) ? caption : base.GetLocalizedString(id);

        internal static void StyleMenu(DXPopupMenu menu)
        {
            foreach (DXMenuItem item in menu.Items)
            {
                item.Appearance.Font = item.AppearanceHovered.Font = MenuFont;
                item.Appearance.Options.UseFont = item.AppearanceHovered.Options.UseFont = true;
                if (item is DXSubMenuItem sub) StyleMenu(sub);
            }
        }

        private static void StyleMenu(DXSubMenuItem menu)
        {
            foreach (DXMenuItem item in menu.Items)
            {
                item.Appearance.Font = item.AppearanceHovered.Font = MenuFont;
                item.Appearance.Options.UseFont = item.AppearanceHovered.Options.UseFont = true;
                if (item is DXSubMenuItem sub) StyleMenu(sub);
            }
        }
    }
}
