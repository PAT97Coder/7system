using DevExpress.XtraGrid.Localization;
using System.Collections.Generic;

namespace KnowledgeSystem.Helpers
{
    // Dùng chung cho danh sách nhân viên và hồ sơ nghỉ việc.
    internal sealed class OffboardingGridLocalizer : GridLocalizer
    {
        public static void Activate()
        {
            if (!(Active is OffboardingGridLocalizer)) Active = new OffboardingGridLocalizer();
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = MenuFont;
            if (!(DevExpress.XtraEditors.Controls.Localizer.Active is EditorLocalizer))
                DevExpress.XtraEditors.Controls.Localizer.Active = new EditorLocalizer();
        }

        private static readonly System.Drawing.Font MenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F);

        private static readonly Dictionary<string, string> Captions = new Dictionary<string, string>
        {
            { "MenuColumnSortAscending", "遞增排序" }, { "MenuColumnSortDescending", "遞減排序" },
            { "MenuColumnClearSorting", "清除排序" }, { "MenuColumnClearAllSorting", "清除所有排序" },
            { "MenuColumnGroup", "依此欄分組" }, { "MenuColumnUnGroup", "取消分組" },
            { "MenuColumnColumnCustomization", "欄位選擇器" }, { "MenuColumnBandCustomization", "欄帶選擇器" },
            { "MenuColumnBestFit", "最佳欄寬" }, { "MenuColumnBestFitAllColumns", "所有欄位最佳欄寬" },
            { "MenuColumnFilter", "篩選" }, { "MenuColumnFilterEditor", "編輯篩選條件" },
            { "MenuColumnClearFilter", "清除篩選" }, { "MenuColumnShowColumn", "顯示欄位" },
            { "MenuColumnRemoveColumn", "隱藏欄位" }, { "MenuColumnShowAutoFilter", "顯示快速篩選列" },
            { "MenuColumnAutoFilterRowShow", "顯示快速篩選列" }, { "MenuColumnAutoFilterRowHide", "隱藏快速篩選列" },
            { "MenuColumnFindFilterShow", "顯示搜尋列" }, { "MenuColumnFindFilterHide", "隱藏搜尋列" },
            { "MenuColumnGroupBox", "顯示分組面板" }, { "MenuGroupPanelShow", "顯示分組面板" },
            { "MenuGroupPanelHide", "隱藏分組面板" }, { "MenuGroupPanelFullExpand", "全部展開" },
            { "MenuGroupPanelFullCollapse", "全部摺疊" }, { "MenuGroupPanelClearGrouping", "清除分組" },
            { "MenuGroupRowExpand", "展開" }, { "MenuGroupRowCollapse", "摺疊" },
            { "MenuColumnFilterMode", "篩選模式" }, { "MenuColumnFilterModeValue", "依值" },
            { "MenuColumnFilterModeDisplayText", "依顯示文字" }, { "MenuColumnExpressionEditor", "運算式編輯器" },
            { "MenuColumnConditionalFormatting", "設定條件格式" }, { "MenuColumnGroupSummaryEditor", "群組摘要" },
            { "MenuColumnSumSummaryTypeDescription", "總和" }, { "MenuColumnMinSummaryTypeDescription", "最小值" },
            { "MenuColumnMaxSummaryTypeDescription", "最大值" }, { "MenuColumnCountSummaryTypeDescription", "筆數" },
            { "MenuColumnAverageSummaryTypeDescription", "平均值" }, { "MenuColumnCustomSummaryTypeDescription", "自訂摘要" },
            { "MenuColumnResetGroupSummarySort", "重設摘要排序" }, { "MenuColumnGroupSummarySortFormat", "依 {0} 排序" },
            { "MenuColumnSortGroupBySummaryMenu", "依摘要排序" }, { "MenuColumnGroupIntervalMenu", "分組間隔" },
            { "MenuColumnGroupIntervalNone", "無" }, { "MenuColumnGroupIntervalDay", "日" },
            { "MenuColumnGroupIntervalMonth", "月" }, { "MenuColumnGroupIntervalYear", "年" },
            { "MenuColumnGroupIntervalSmart", "自動" }, { "MenuFooterSum", "總和" },
            { "MenuFooterMin", "最小值" }, { "MenuFooterMax", "最大值" }, { "MenuFooterCount", "筆數" },
            { "MenuFooterAverage", "平均值" }, { "MenuFooterNone", "無" },
            { "MenuFooterSumFormat", "總和：{0}" }, { "MenuFooterMinFormat", "最小值：{0}" },
            { "MenuFooterMaxFormat", "最大值：{0}" }, { "MenuFooterCountFormat", "筆數：{0}" },
            { "MenuFooterAverageFormat", "平均值：{0}" }, { "MenuFooterAddSummaryItem", "新增摘要" },
            { "MenuFooterClearSummaryItems", "清除摘要" }, { "MenuFooterShow", "顯示摘要列" },
            { "MenuFooterHide", "隱藏摘要列" }, { "MenuFooterMode", "摘要模式" },
            { "MenuFooterAllRows", "所有資料列" }, { "MenuFooterSelection", "已選取資料列" },
            { "MenuFooterMixed", "混合" }, { "MenuFooterCustomFormat", "自訂格式" },
            { "MenuFooterCountGroupFormat", "群組筆數：{0}" }, { "MenuShowSplitItem", "分割檢視" },
            { "MenuHideSplitItem", "取消分割" }, { "FilterPanelCustomizeButton", "編輯篩選" },
            { "FindControlFindButton", "搜尋" }, { "FindControlClearButton", "清除" },
            { "FindControlNextButton", "下一筆" }, { "FindControlPrevButton", "上一筆" },
            { "GroupPanelText", "將欄位拖曳至此處進行分組" }, { "FilterBuilderOkButton", "確定" },
            { "FilterBuilderCancelButton", "取消" }, { "FilterBuilderApplyButton", "套用" },
            { "FilterBuilderCaption", "篩選條件編輯器" }, { "FilterPopupAll", "全部" },
            { "FilterPopupCustom", "自訂" }, { "FilterPopupBlanks", "空白" },
            { "FilterPopupNonBlanks", "非空白" }, { "FilterPopupCheckedListCheckAll", "全選" },
            { "FilterPopupCheckedListOk", "確定" }, { "FilterPopupCheckedListCancel", "取消" },
            { "FilterPopupCheckedListShowBlanks", "空白" }, { "CustomFilterDialogCaption", "自訂篩選" },
            { "CustomFilterDialogOkButton", "確定" }, { "CustomFilterDialogCancelButton", "取消" }
        };

        public override string GetLocalizedString(GridStringId id)
        {
            string caption;
            return Captions.TryGetValue(id.ToString(), out caption) ? caption : base.GetLocalizedString(id);
        }

        private sealed class EditorLocalizer : DevExpress.XtraEditors.Controls.Localizer
        {
            private static readonly Dictionary<string, string> EditorCaptions = new Dictionary<string, string>
            {
                { "OK", "確認" }, { "Cancel", "取消" },
                { "XtraMessageBoxOkButtonText", "確認" }, { "XtraMessageBoxCancelButtonText", "取消" },
                { "NavigatorTextStringFormat", "第 {0} 筆，共 {1} 筆" },
                { "NavigatorFirstButtonHint", "第一筆" }, { "NavigatorPreviousButtonHint", "上一筆" },
                { "NavigatorPreviousPageButtonHint", "上一頁" }, { "NavigatorNextButtonHint", "下一筆" },
                { "NavigatorNextPageButtonHint", "下一頁" }, { "NavigatorLastButtonHint", "最後一筆" },
                { "TextEditMenuUndo", "復原" }, { "TextEditMenuCut", "剪下" }, { "TextEditMenuCopy", "複製" },
                { "TextEditMenuPaste", "貼上" }, { "TextEditMenuDelete", "刪除" }, { "TextEditMenuSelectAll", "全選" },
                { "FilterEditorTabText", "文字" }, { "FilterEditorTabVisual", "視覺化" }, { "FilterShowAll", "顯示全部" },
                { "FilterGroupAnd", "且" }, { "FilterGroupNotAnd", "非全部符合" }, { "FilterGroupOr", "或" },
                { "FilterGroupNotOr", "全部不符合" }, { "FilterClauseAnyOf", "符合任一值" },
                { "FilterClauseNoneOf", "不符合任一值" }, { "FilterClauseBeginsWith", "開頭為" },
                { "FilterClauseEndsWith", "結尾為" }, { "FilterClauseContains", "包含" },
                { "FilterClauseDoesNotContain", "不包含" }, { "FilterClauseEquals", "等於" },
                { "FilterClauseDoesNotEqual", "不等於" }, { "FilterClauseGreater", "大於" },
                { "FilterClauseGreaterOrEqual", "大於或等於" }, { "FilterClauseLess", "小於" },
                { "FilterClauseLessOrEqual", "小於或等於" }, { "FilterClauseLike", "符合樣式" },
                { "FilterClauseNotLike", "不符合樣式" }, { "FilterClauseBetween", "介於" },
                { "FilterClauseBetweenAnd", "與" }, { "FilterClauseNotBetween", "不介於" },
                { "FilterClauseInRange", "在範圍內" }, { "FilterClauseNotInRange", "不在範圍內" },
                { "FilterClauseInRangeFrom", "從" }, { "FilterClauseInRangeTo", "至" },
                { "FilterClauseIsNull", "空值" }, { "FilterClauseIsNotNull", "非空值" },
                { "FilterClauseIsNullOrEmpty", "空值或空白" }, { "FilterClauseIsNotNullOrEmpty", "非空值且非空白" },
                { "FilterClauseInDateRange", "在日期範圍內" }, { "FilterClauseNotInDateRange", "不在日期範圍內" },
                { "FilterEmptyEnter", "輸入值" }, { "FilterEmptyValue", "空值" }, { "FilterEmptyParameter", "參數" },
                { "FilterNewEmptyEnter", "輸入值" }, { "FilterNewEmptyParameter", "選擇參數" },
                { "FilterEmptyField", "選擇欄位" }, { "FilterExpressionEmptyText", "輸入篩選條件" },
                { "FilterMenuAddNewParameter", "新增參數" }, { "FilterMenuConditionAdd", "新增條件" },
                { "FilterMenuGroupAdd", "新增群組" }, { "FilterMenuExpressionAdd", "新增運算式" },
                { "FilterMenuClearAll", "清除全部" }, { "FilterMenuRowRemove", "移除條件" },
                { "FilterToolTipNodeAdd", "新增條件或群組" }, { "FilterToolTipNodeRemove", "移除條件或群組" },
                { "FilterToolTipNodeAction", "選擇運算子" }, { "FilterToolTipValueType", "選擇值類型" },
                { "FilterToolTipElementAdd", "新增項目" }, { "FilterToolTipKeysAdd", "新增條件" },
                { "FilterToolTipKeysRemove", "移除條件" }, { "FilterDateTextAlt", "選擇日期" },
                { "FilterOutlookDateText", "日期範圍" }, { "FilterDateTimeConstantMenuCaption", "日期常數" },
                { "FilterDateTimeOperatorMenuCaption", "日期運算子" }, { "FilterAdvancedDateTimeOperatorMenuCaption", "進階日期運算子" },
                { "FilterCustomFunctionsMenuCaption", "自訂函式" }, { "FilterFunctionsMenuCaption", "函式" },
                { "FilterPopupToolbarShowOnlyAvailableItems", "僅顯示可用項目" }, { "FilterPopupToolbarShowNewValues", "顯示新值" },
                { "FilterPopupToolbarIncrementalSearch", "即時搜尋" }, { "FilterPopupToolbarMultiSelection", "多選" },
                { "FilterPopupToolbarRadioMode", "單選" }, { "FilterPopupToolbarInvertFilter", "反向篩選" }
            };

            public override string GetLocalizedString(DevExpress.XtraEditors.Controls.StringId id)
            {
                string caption;
                return EditorCaptions.TryGetValue(id.ToString(), out caption) ? caption : base.GetLocalizedString(id);
            }
        }
    }
}
