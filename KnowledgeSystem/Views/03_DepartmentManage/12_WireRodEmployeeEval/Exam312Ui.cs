using DevExpress.XtraBars;
using System.Drawing;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    internal static class Exam312Ui
    {
        internal static void StyleBar(Bar bar)
        {
            bar.OptionsBar.UseWholeRow = true; bar.OptionsBar.DrawDragBorder = false;
            bar.OptionsBar.AllowQuickCustomization = false;
            bar.BarAppearance.Normal.Font = new Font("Microsoft JhengHei UI", 14.25F);
            bar.BarAppearance.Normal.Options.UseFont = true;
            bar.BarAppearance.Disabled.Font = new Font("Microsoft JhengHei UI", 12F);
            bar.BarAppearance.Disabled.Options.UseFont = true;
        }
        internal static void StyleButton(BarItem item)
        {
            item.PaintStyle = BarItemPaintStyle.CaptionGlyph; item.ImageOptions.SvgImageSize = new Size(32, 32);
            foreach (var appearance in new[] { item.ItemAppearance.Normal, item.ItemAppearance.Hovered, item.ItemAppearance.Pressed })
            {
                appearance.Font = new Font("Microsoft JhengHei UI", 14.25F); appearance.ForeColor = Color.Black;
                appearance.Options.UseFont = appearance.Options.UseForeColor = true;
            }
            item.ItemAppearance.Disabled.Font = new Font("Microsoft JhengHei UI", 12F); item.ItemAppearance.Disabled.Options.UseFont = true;
        }
    }
}
