using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement;
using KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset;
using KnowledgeSystem.Views._03_DepartmentManage._15_InterviewAssessment;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

internal static class ContractorAcceptanceTests
{
    private static void Check(bool result, string message)
    {
        if (!result) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }

    private static void Reject<T>(Action action, string message) where T : Exception
    {
        try { action(); }
        catch (T) { Check(true, message); return; }
        throw new Exception(message);
    }

    private static void Edit(string template, string target, Action<ExcelWorksheet> change)
    {
        using (var package = new ExcelPackage(new FileInfo(template)))
        {
            change(package.Workbook.Worksheets.First());
            package.SaveAs(new FileInfo(target));
        }
    }

    [STAThread]
    private static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        WindowsFormsSettings.ScrollUIMode = ScrollUIMode.Fluent;
        var template = args[0];
        string work = args[1];
        var sample = ContractorAcceptanceWorkbook.Read(template).Single();
        Check(sample.Period == "2026-11" && sample.ReportDate == new DateTime(2026, 11, 25)
            && sample.DocumentNumber == "7700-202611" && sample.CostCenter == "LG7730"
            && sample.ContractNumber == "XXXX" && sample.Contractor == "XXXXX", "Real R1 header mapped correctly");
        Check(sample.Lines.Count == 1 && sample.Lines[0].ExcelRow == 9 && sample.Lines[0].WorkCode == "A"
            && sample.Lines[0].Currency == "VND" && sample.Lines[0].Quantity == 1m && sample.Lines[0].UnitPrice == 15000m,
            "Blank template rows are skipped and real detail is loaded");
        Check(sample.BeforeTax == 15000m && sample.Tax == 1200m && sample.Total == 16200m && sample.Validation == "檢查通過",
            "R1 totals calculated from data and tax formula");
        var second = Path.Combine(work, "second.xlsx");
        Edit(template, second, sheet =>
        {
            sheet.Cells["C7"].Value = "7799-202611";
            sheet.Cells["B10"].Value = "測試";
            sheet.Cells["F10"].Value = "KG";
            sheet.Cells["G10"].Value = 2;
            sheet.Cells["H10"].Value = 100;
            sheet.Cells["I13"].Value = 100;
            sheet.Cells["I15"].Formula = "I14*10%";
        });
        var changed = ContractorAcceptanceWorkbook.Read(second).Single();
        Check(changed.Lines.Count == 2 && changed.Lines[1].Amount == 200m && changed.Total == 16610m,
            "Extra uncolored line, adjustment and changed tax rate are supported");
        var wrong = Path.Combine(work, "wrong.xlsx");
        Edit(template, wrong, sheet => sheet.Cells["A5"].Value = "invalid");
        Reject<InvalidDataException>(() => ContractorAcceptanceWorkbook.Read(wrong), "Unrecognized template rejected");
        var invalid = Path.Combine(work, "invalid.xlsx");
        Edit(template, invalid, sheet => sheet.Cells["G9"].Value = "1,2");
        Reject<InvalidDataException>(() => ContractorAcceptanceWorkbook.Read(invalid), "Malformed quantity rejected");
        var mismatch = Path.Combine(work, "mismatch.xlsx");
        Edit(template, mismatch, sheet => { sheet.Cells["I9"].Formula = null; sheet.Cells["I9"].Value = 999; });
        Check(ContractorAcceptanceWorkbook.Read(mismatch).Single().Validation.Contains("I9"), "Literal amount mismatch identifies source cell");
        Reject<InvalidOperationException>(() => ContractorAcceptanceWorkbook.Export(template, new[] { sample }), "Source overwrite blocked");
        var exported = Path.Combine(work, "export.xlsx");
        ContractorAcceptanceWorkbook.Export(exported, new[] { sample, changed });
        using (var package = new ExcelPackage(new FileInfo(exported)))
        {
            var header = package.Workbook.Worksheets["驗收資料"];
            var details = package.Workbook.Worksheets["工作明細"];
            Check(header.Dimension.End.Row == 3 && header.Cells["J3"].GetValue<decimal>() == 16610m &&
                details.Dimension.End.Row == 4 && details.Cells["J4"].GetValue<decimal>() == 200m, "Exported Excel retains both reports and numeric details");
        }
        CheckScreen(template, second, wrong, work, args.Length > 2 ? args[2] : null, args.Length > 3 ? args[3] : null);
    }

    private static void CheckScreen(string template, string second, string wrong, string work, string userId, string otherUserId)
    {
        using (var host = new Form { ClientSize = new Size(1280, 700) })
        using (var screen = new uc311_ContractorAcceptance { Dock = DockStyle.Fill })
        {
            host.Controls.Add(screen);
            host.Show();
            Application.DoEvents();
            var toolbar = Field<Bar>(screen, "bar");
            var import = Field<BarButtonItem>(screen, "import");
            var reload = Field<BarButtonItem>(screen, "reload");
            var export = Field<BarButtonItem>(screen, "export");
            var grid = Field<GridControl>(screen, "grid");
            var view = Field<GridView>(screen, "view");
            Check(toolbar.ItemLinks.Count == 3 && !import.Enabled && !reload.Enabled && !export.Enabled, "Toolbar actions disabled without permission");
            CheckFormAlignment(screen, grid, view);
            if (!string.IsNullOrWhiteSpace(userId)) CheckDatabase(screen, view, template, second, wrong, work, userId, otherUserId);
            CheckMasterDetail(screen, view, template, second);

            int wide = grid.Width;
            host.ClientSize = new Size(950, 540);
            Application.DoEvents();
            Check(grid.Width < wide && grid.Height >= 300 && grid.Top <= 20,
                "Master-detail grid fills the available area when the screen resizes");
            using (var bitmap = new Bitmap(screen.Width, screen.Height))
            {
                screen.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(Path.Combine(work, "screen.png"));
            }
        }
    }

    private static void CheckFormAlignment(uc311_ContractorAcceptance screen, GridControl grid, GridView view)
    {
        using (var expense = new uc311_ExpenseMain())
        using (var assets = new uc313_InspectionBatch())
        using (var interviews = new uc315_Interview())
        {
            foreach (Control reference in new Control[] { expense, assets, interviews })
            {
                var referenceGrid = Field<GridControl>(reference, "gcData");
                var referenceView = (GridView)referenceGrid.MainView;
                var referenceBar = Field<BarManager>(reference, "barManagerTP").MainMenu;
                var toolbar = Field<BarManager>(screen, "manager").MainMenu;
                Check(grid.UseEmbeddedNavigator == referenceGrid.UseEmbeddedNavigator &&
                    grid.EmbeddedNavigator.Buttons.Append.Visible == referenceGrid.EmbeddedNavigator.Buttons.Append.Visible &&
                    view.OptionsSelection.EnableAppearanceHotTrackedRow == referenceView.OptionsSelection.EnableAppearanceHotTrackedRow &&
                    toolbar.DockStyle == referenceBar.DockStyle && toolbar.OptionsBar.UseWholeRow == referenceBar.OptionsBar.UseWholeRow,
                    "Shared toolbar and list navigation match " + reference.GetType().Name);
            }
        }
        Check(!grid.EmbeddedNavigator.Buttons.Remove.Visible && !grid.EmbeddedNavigator.Buttons.Edit.Visible
            && grid.EmbeddedNavigator.TextStringFormat == "第 {0} 筆，共 {1} 筆"
            && view.VisibleColumns[0].FieldName == "DocumentNumber", "Navigator is read-only and the document number is the first column");
    }

    private static void CheckDatabase(uc311_ContractorAcceptance screen, GridView view,
        string template, string second, string wrong, string work, string userId, string otherUserId)
    {
        var sql = new System.Data.SqlClient.SqlConnectionStringBuilder(
            new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(SingleConnection.ConString).ProviderConnectionString);
        Check(sql.DataSource == "10.198.138.153" && sql.InitialCatalog == "DBDocumentManagementSystem", "Integration test targets test DB only");
        var service = new ContractorAcceptanceService();
        var permission = service.GetPermissionId(userId);
        Check(permission.HasValue, "Test actor has current menu permission");
        Reject<UnauthorizedAccessException>(() => service.Save(null, null, null), "Business layer rejects anonymous writes");
        TPConfigs.LoginUser = new dm_User { Id = userId };
        var permissions = (List<int>)typeof(TPConfigs).Assembly.GetType("KnowledgeSystem.Helpers.AppPermission")
            .GetField("lsPermissions").GetValue(null);
        permissions.Add(permission.Value);
        Invoke(screen, "Screen_Load", screen, EventArgs.Empty);

        string key = "TEST311-" + Guid.NewGuid().ToString("N").ToUpperInvariant();
        string rollbackKey = key + "-B";
        string otherKey = key + "-OTHER";
        string initial = Path.Combine(work, "db-initial.xlsx");
        string replacement = Path.Combine(work, "db-overwrite.xlsx");
        string rollbackFile = Path.Combine(work, "db-rollback.xlsx");
        Edit(second, initial, sheet => sheet.Cells["C7"].Value = key);
        Edit(template, replacement, sheet =>
        {
            sheet.Cells["C7"].Value = " " + key.ToLowerInvariant() + " ";
            sheet.Cells["H9"].Value = 25000;
        });
        Edit(template, rollbackFile, sheet => sheet.Cells["C7"].Value = rollbackKey);
        Func<int, IReadOnlyDictionary<string, byte[]>, bool> accept = (count, duplicates) => true;
        Func<int, IReadOnlyDictionary<string, byte[]>, bool> cancel = (count, duplicates) =>
        {
            Check(duplicates.Count == 1 && duplicates.ContainsKey(key), "Overwrite dialog receives normalized duplicate key");
            return false;
        };
        try
        {
            Invoke(screen, "ImportFiles", new[] { initial }, accept);
            var stored = service.GetList(userId).Single(row => row.DocumentNumber == key);
            Check(stored.Lines.Count == 2 && stored.Total == 16610m && stored.CreatedByUserId == userId
                && service.GetSource(userId, key).SequenceEqual(File.ReadAllBytes(initial)), "UI import persists header, details, audit and exact source");
            if (!string.IsNullOrWhiteSpace(otherUserId))
                CheckDepartmentIsolation(service, stored, template, work, userId, otherUserId, otherKey);
            var firstDetail = ExpandDetails(view, key);
            Check(view.LocateByValue("DocumentNumber", key) == view.FocusedRowHandle && firstDetail.DataRowCount == 2,
                "Stored report and details selected after import");
            Check(view.Columns["CreatedByUserId"].Visible && view.Columns["CreatedLocal"].Visible
                && (string)view.GetFocusedRowCellValue("CreatedByUserId") == userId
                && (DateTime)view.GetFocusedRowCellValue("CreatedLocal") == stored.CreatedLocal,
                "The screen displays the saved uploader and import time");
            using (var reopened = new uc311_ContractorAcceptance())
            {
                Invoke(reopened, "Screen_Load", reopened, EventArgs.Empty);
                Check(Field<List<ContractorAcceptance>>(reopened, "records").Any(row => row.DocumentNumber == key && row.Lines.Count == 2),
                    "A new screen loads the persisted report and details");
            }

            Invoke(screen, "ImportFiles", new[] { replacement }, cancel);
            Check(service.GetList(userId).Single(row => row.DocumentNumber == key).Version.SequenceEqual(stored.Version)
                && service.GetSource(userId, key).SequenceEqual(File.ReadAllBytes(initial)), "Cancel preserves existing version and original file");

            var incoming = ContractorAcceptanceWorkbook.Read(replacement);
            var approved = service.FindDuplicates(userId, incoming);
            Reject<InvalidOperationException>(() => service.Save(userId, incoming, null), "Duplicate write without confirmation rejected");
            Reject<InvalidDataException>(() => service.FindDuplicates(userId, incoming.Concat(incoming).ToList()),
                "Duplicate numbers within one upload rejected");
            var invalidBatch = new[] { incoming.Single(), ContractorAcceptanceWorkbook.Read(rollbackFile).Single() };
            invalidBatch[1].Contractor = new string('X', 256);
            Reject<System.Data.Entity.Validation.DbEntityValidationException>(() => service.Save(userId, invalidBatch, approved),
                "Failed second write rolls back overwrite in the same batch");
            Check(service.GetList(userId).Single(row => row.DocumentNumber == key).Version.SequenceEqual(stored.Version)
                && !service.GetList(userId).Any(row => row.DocumentNumber == rollbackKey)
                && service.GetSource(userId, key).SequenceEqual(File.ReadAllBytes(initial)), "Rollback restores original header, lines and source");

            Invoke(screen, "ImportFiles", new[] { replacement }, accept);
            var updated = service.GetList(userId).Single(row => row.DocumentNumber == key);
            Check(updated.Lines.Count == 1 && updated.Lines[0].UnitPrice == 25000m && updated.Total == 27000m
                && updated.CreatedUtc == stored.CreatedUtc && !updated.Version.SequenceEqual(stored.Version)
                && service.GetSource(userId, key).SequenceEqual(File.ReadAllBytes(replacement)), "Overwrite replaces data and file without duplicating report");
            Reject<InvalidOperationException>(() => service.Save(userId, incoming, approved), "Stale overwrite confirmation rejected");
            Reject<TargetInvocationException>(() => Invoke(screen, "ImportFiles", new[] { wrong }, accept), "Invalid file rejected before storage");

            view.ActiveFilterString = "[DocumentNumber] = '" + key + "'";
            var filtered = (List<ContractorAcceptance>)Invoke(screen, "GetVisibleRecords");
            Check(filtered.Count == 1 && filtered[0].DocumentNumber == key, "Export selection follows grid filter");
            Invoke(screen, "ReloadRecords", (object)null);
            var pinned = (ContractorAcceptance)view.GetFocusedRow();
            var refreshedDetail = view.GetDetailView(view.FocusedRowHandle, 0) as GridView;
            Check(pinned.DocumentNumber == key && refreshedDetail != null && refreshedDetail.DataRowCount == 1
                && (decimal)view.GetFocusedRowCellValue("Total") == 27000m, "Refresh restores selection and expansion and shows the latest totals");

            int target = view.LocateByValue("DocumentNumber", key);
            var row = ((DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo)view.GetViewInfo()).GetGridRowInfo(target);
            var hit = view.CalcHitInfo(new Point(row.Bounds.Left + 55, row.Bounds.Top + row.Bounds.Height / 2));
            var menu = new GridViewMenu(view);
            var original = new DXMenuItem("原有項目");
            menu.Items.Add(original);
            Invoke(screen, "PopupMenuShowing", view, new PopupMenuShowingEventArgs(GridMenuType.Row, menu, hit, true));
            Check(view.FocusedRowHandle == target && menu.Items.Count == 3 && menu.Items[0] == original
                && menu.Items[1].Caption == "ERP：外包工作驗收輸入" && menu.Items[1].Enabled
                && menu.Items[2].Caption == "下載來源 Excel", "ERP and source download appear for the clicked DB record");
            permissions.Remove(permission.Value);
            var deniedMenu = new GridViewMenu(view);
            Invoke(screen, "PopupMenuShowing", view, new PopupMenuShowingEventArgs(GridMenuType.Row, deniedMenu, hit, true));
            Check(deniedMenu.Items.Count == 0, "Row actions hidden without permission");
        }
        finally
        {
            using (var context = new ContractorAcceptanceContext())
            {
                context.Acceptances.RemoveRange(context.Acceptances.Where(row => row.DocumentNumber == key
                    || row.DocumentNumber == rollbackKey || row.DocumentNumber == otherKey).ToList());
                context.SaveChanges();
                Check(!context.Items.Any(row => row.DocumentNumber == key) && !context.Files.Any(row => row.DocumentNumber == key),
                    "Integration test rows and dependent file removed from test DB");
            }
        }
    }

    private static GridView ExpandDetails(GridView view, string documentNumber)
    {
        int handle = view.LocateByValue("DocumentNumber", documentNumber);
        if (handle < 0) throw new Exception("Master report not found: " + documentNumber);
        view.SetMasterRowExpanded(handle, true);
        Application.DoEvents();
        return (GridView)view.GetDetailView(handle, 0);
    }

    private static void CheckMasterDetail(uc311_ContractorAcceptance screen, GridView view, string template, string second)
    {
        var rows = ContractorAcceptanceWorkbook.Read(template).Concat(ContractorAcceptanceWorkbook.Read(second)).ToList();
        view.ActiveFilterString = "";
        Invoke(screen, "BindRecords", rows, rows[0].DocumentNumber);
        var first = ExpandDetails(view, rows[0].DocumentNumber);
        Check(first != null && first.DataRowCount == 1 && first.Columns["Description"].Caption == "工作項目"
            && !first.OptionsBehavior.Editable, "Expanding a master row creates the configured read-only detail grid");
        Check(!first.OptionsView.ShowViewCaption && first.OptionsView.ShowAutoFilterRow && !first.OptionsView.ColumnAutoWidth
            && !view.OptionsDetail.ShowDetailTabs && first.Appearance.HeaderPanel.TextOptions.HAlignment == DevExpress.Utils.HorzAlignment.Center,
            "Detail grid follows the expense screen's caption, filter, widths and centered headers");
        var other = ExpandDetails(view, rows[1].DocumentNumber);
        Check(other != null && other.DataRowCount == 2 && (string)other.GetRowCellValue(1, "Description") == "測試",
            "Each master row displays only its own work items");
        Check(!view.GetMasterRowExpanded(view.LocateByValue("DocumentNumber", rows[0].DocumentNumber)),
            "Only one master row stays expanded");
        Check((string)view.GetFocusedRowCellValue("DocumentNumber") == rows[1].DocumentNumber,
            "Opening a master row selects that same report");
    }

    private static void CheckDepartmentIsolation(ContractorAcceptanceService service, ContractorAcceptance first,
        string template, string work, string userId, string otherUserId, string otherKey)
    {
        var otherFile = Path.Combine(work, "other-department.xlsx");
        Edit(template, otherFile, sheet => sheet.Cells["C7"].Value = otherKey);
        var otherImport = ContractorAcceptanceWorkbook.Read(otherFile);
        service.Save(otherUserId, otherImport, null);
        var otherRecords = service.GetList(otherUserId);
        var other = otherRecords.Single(row => row.DocumentNumber == otherKey);
        Check(first.DepartmentGroup == "77" && other.DepartmentGroup == "78" && other.CostCenter == "LG7730",
            "Group comes from uploader profile, including when Excel cost center starts with another group");
        Check(!otherRecords.Any(row => row.DocumentNumber == first.DocumentNumber)
            && !service.GetList(userId).Any(row => row.DocumentNumber == otherKey),
            "77 and 78 cannot list each other's reports or detail rows");
        Check(service.GetSource(otherUserId, first.DocumentNumber) == null && service.GetSource(userId, otherKey) == null
            && service.GetSource(otherUserId, otherKey).SequenceEqual(File.ReadAllBytes(otherFile)),
            "Original Excel download is limited to the uploader's department group");
        Reject<UnauthorizedAccessException>(() => service.FindDuplicates(userId, otherImport),
            "Duplicate lookup does not disclose confirmation version across groups");
        var forgedApproval = new Dictionary<string, byte[]> { { otherKey, other.Version } };
        Reject<UnauthorizedAccessException>(() => service.Save(userId, otherImport, forgedApproval),
            "Cross-group overwrite is rejected even with a valid row version");
        Check(service.GetList(otherUserId).Single(row => row.DocumentNumber == otherKey).Version.SequenceEqual(other.Version),
            "Cross-group attempts preserve the original report");
    }

    private static T Field<T>(object target, string name)
    {
        return (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    }

    private static object Invoke(object target, string name, params object[] values)
    {
        return target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, values);
    }
}
