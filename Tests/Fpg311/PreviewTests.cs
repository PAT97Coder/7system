using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using BusinessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement;

class PreviewTests
{
    static object Field(object obj, string name) { return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj); }
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); Console.WriteLine("PASS: " + message); }
    [STAThread]
    static void Main(string[] args)
    {
        Check(!dt311_InvoiceBUS.Instance.CanImportFpgFuel(null), "Anonymous import permission rejected without querying database");
        string error;
        Check(!dt311_InvoiceBUS.Instance.UpdateFuelInfoBatch(null, null, out error) && !string.IsNullOrEmpty(error), "Business update requires authenticated actor");
        using (var form = new f311_FpgFuelImportPreview())
        {
            var confirm = (BarButtonItem)Field(form, "confirm");
            Check(!confirm.Enabled, "Confirmation disabled initially");
            var rows = new[] { new { ExcelRow = 2, DocId = "FPG001", FuelDate = DateTime.Today, LicensePlate = "51A-12345", FuelFilledBy = "VNW0014732", OdometerReading = 123456, InvoiceDisplay = "AA000001", Status = "待更新" } };
            form.SetPreview(rows, 0, 1);
            Check(!confirm.Enabled, "No eligible rows cannot be confirmed");
            form.SetPreview(rows, 1, 0);
            Check(confirm.Enabled, "Eligible rows enable confirmation");
            form.Show(); Application.DoEvents();
            var grid = (GridControl)Field(form, "grid");
            var summary = (LabelControl)Field(form, "summary");
            var view = (GridView)Field(form, "view");
            Check(view.Columns.Count == 8 && view.RowCount == 1 && !view.OptionsBehavior.Editable && view.OptionsView.ShowAutoFilterRow, "Preview shows fixed read-only filterable columns and data");
            Check(grid.Width > 800 && grid.Height > 200 && grid.Top >= summary.Bottom, "Initial grid and summary layout has no overlap");
            int oldWidth = grid.Width;
            form.Size = new Size(960, 500); Application.DoEvents();
            Check(grid.Width < oldWidth && grid.Height > 150 && grid.Top >= summary.Bottom, "Content resizes without overlap");
            form.Size = new Size(1240, 650); Application.DoEvents();
            using (var bmp = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size)); bmp.Save(args[0]); }
            typeof(f311_FpgFuelImportPreview).GetMethod("confirm_ItemClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(form, new object[] {null, null});
            Check(form.DialogResult == DialogResult.OK, "Confirmation returns OK to caller");
        }
    }
}
