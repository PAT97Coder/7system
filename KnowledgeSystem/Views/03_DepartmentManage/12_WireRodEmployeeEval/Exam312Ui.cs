using DevExpress.XtraBars;
using System.Drawing;
using System;
using BusinessLayer;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    internal static class Exam312Ui
    {
        internal static string ErrorText(Exception error)
        {
            System.Diagnostics.Trace.TraceError("Practice: {0}", error);
            if (error is dt312_PracticeValidationException) return error.Message;
            if (error is UnauthorizedAccessException) return "您沒有執行此作業的權限。";
            if (error is ArgumentException) return "輸入資料不正確，請檢查必填欄位與長度。";
            if (error is Newtonsoft.Json.JsonException) return "作答資料格式不正確，無法顯示測驗詳情。";
            return "作業未完成，請稍後再試或聯絡管理員。";
        }
        internal static void StyleBar(Bar bar)
        {
            bar.OptionsBar.UseWholeRow = true; bar.OptionsBar.DrawDragBorder = false;
            bar.OptionsBar.AllowQuickCustomization = false;
            bar.BarAppearance.Normal.Font = new Font("Microsoft JhengHei UI", 14.25F);
            bar.BarAppearance.Normal.Options.UseFont = true;
            bar.BarAppearance.Disabled.Font = new Font("Microsoft JhengHei UI", 12F);
            bar.BarAppearance.Disabled.Options.UseFont = true;
        }
        internal static void StyleButton(BarItem item, float fontSize = 14.25F)
        {
            item.PaintStyle = BarItemPaintStyle.CaptionGlyph; item.ImageOptions.SvgImageSize = new Size(32, 32);
            foreach (var appearance in new[] { item.ItemAppearance.Normal, item.ItemAppearance.Hovered, item.ItemAppearance.Pressed })
            {
                appearance.Font = new Font("Microsoft JhengHei UI", fontSize); appearance.ForeColor = Color.Black;
                appearance.Options.UseFont = appearance.Options.UseForeColor = true;
            }
            item.ItemAppearance.Disabled.Font = new Font("Microsoft JhengHei UI", 12F); item.ItemAppearance.Disabled.Options.UseFont = true;
        }
    }
}
