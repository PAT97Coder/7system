using DataAccessLayer;
using System;
using System.Globalization;
using System.Linq;

namespace KnowledgeSystem.Helpers
{
    internal static class OffboardingUi
    {
        internal static void DrawEmptyList(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null || view.RowCount != 0) return;
            System.Windows.Forms.TextRenderer.DrawText(e.Cache.Graphics, "目前沒有資料。", view.Appearance.Row.Font,
                e.Bounds, System.Drawing.Color.Black, System.Windows.Forms.TextFormatFlags.HorizontalCenter
                | System.Windows.Forms.TextFormatFlags.VerticalCenter);
            e.Handled = true;
        }

        internal static string EmployeeCaption(dm_User user)
        {
            if (user == null) return "";
            string name = new[] { user.DisplayName, user.DisplayNameVN }
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            return string.IsNullOrWhiteSpace(name) ? user.Id : user.Id + "　" + name;
        }

        internal static string FormatFileSize(long size)
        {
            if (size >= 1024L * 1024) return (size / (1024d * 1024)).ToString("0.##", CultureInfo.InvariantCulture) + " MB";
            if (size >= 1024) return (size / 1024d).ToString("0.##", CultureInfo.InvariantCulture) + " KB";
            return size.ToString(CultureInfo.InvariantCulture) + " B";
        }
    }
}
