// Chạy bằng tools/Test-EmployeeOffboarding.ps1. Không mở kết nối hoặc sửa database.
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

internal static class EmployeeOffboardingSmoke
{
    private static void Assert(bool success, string message)
    {
        if (!success) throw new Exception(message);
    }

    private static T Field<T>(object instance, string name)
    {
        return (T)instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);
    }

    private static void Invoke(object instance, string name)
    {
        instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(instance, null);
    }

    private static void ExpectValidation(Action action)
    {
        try { action(); }
        catch (OffboardingException) { return; }
        throw new Exception("Validation did not reject invalid input.");
    }

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            string output = args[0];
            Directory.CreateDirectory(output);
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var metadata = ((IObjectContextAdapter)db).ObjectContext.MetadataWorkspace;
                var entities = metadata.GetItems<EntityType>(DataSpace.CSpace);
                Assert(entities.Any(x => x.Name == "EmployeeOffboarding"), "Missing record model.");
                Assert(entities.Any(x => x.Name == "EmployeeOffboardingFile"), "Missing file model.");
                Assert(db.EmployeeOffboardings.Where(x => x.UserId == "VNW0014732").ToString().Contains("EmployeeOffboarding"), "Invalid record mapping.");
                Assert(db.EmployeeOffboardingFiles.Where(x => x.OffboardingId == 1).ToString().Contains("EmployeeOffboardingFile"), "Invalid file mapping.");
            }
            Console.WriteLine("PASS: EF metadata and SQL generation without database connection.");
            var service = new EmployeeOffboardingService();
            ExpectValidation(() => service.Upload("invalid", "VNW0014732", "reason", new[] { "file.pdf" }));
            ExpectValidation(() => service.Upload("VNW0014732", "VNW0014732", " ", new[] { "file.pdf" }));
            ExpectValidation(() => service.Upload("VNW0014732", "VNW0014732", new string('a', 2001), new[] { "file.pdf" }));
            ExpectValidation(() => service.Upload("VNW0014732", "VNW0014732", "reason", new string[0]));
            Console.WriteLine("PASS: employee ID, required reason, reason length and required files validation.");
            TestFileStorage(output);
            Application.EnableVisualStyles();
            TPConfigs.LoginUser = new dm_User { Id = "VNW0000001", IdDepartment = "7820", Status = 0 };
            using (var screen = new uc401_UserManage())
            {
                Assert(Field<object>(screen, "itemOffboardingHistory") != null, "History menu not initialized.");
                Assert(Field<object>(screen, "itemOffboardingUpload") != null, "Upload menu not initialized.");
                var combo = Field<BarEditItem>(screen, "statusFilter");
                var view = Field<DevExpress.XtraGrid.Views.Grid.GridView>(screen, "gvData");
                combo.EditValue = "已離職人員";
                Assert(view.Columns["StatusName"].FilterInfo.FilterString.Contains("離職"), "Departed filter is not applied.");
                combo.EditValue = "全部人員";
                Assert(string.IsNullOrEmpty(view.Columns["StatusName"].FilterInfo.FilterString), "All-employee filter not cleared.");
                var loadHandler = screen.GetType().GetMethod("f401_UserManager_Load", BindingFlags.NonPublic | BindingFlags.Instance);
                screen.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), screen, loadHandler);
                using (var host = new XtraForm { StartPosition = FormStartPosition.Manual, Location = new Point(-30000, -30000) })
                {
                    screen.Dock = DockStyle.Fill;
                    host.Controls.Add(screen);
                    host.Show();
                    Application.DoEvents();
                    var grid = Field<GridControl>(screen, "gcData");
                    var first = new { Data = new dm_User { Id = "VNW0000001" }, StatusName = "離職" };
                    var second = new { Data = new dm_User { Id = "VNW0000002" }, StatusName = "離職" };
                    grid.DataSource = new[] { first, second };
                    grid.ForceInitialize();
                    Assert(view.DataRowCount == 2, "Employee refresh fixture did not bind.");
                    view.FocusedRowHandle = 1;
                    var refresh = Field<RefreshHelper>(screen, "helper");
                    refresh.SaveViewInfo();
                    grid.DataSource = new[] { second, first };
                    refresh.LoadViewInfo();
                    Assert((string)view.GetFocusedRowCellValue("Data.Id") == second.Data.Id, "Selected employee lost on refresh.");
                }
            }
            foreach (GridStringId id in Enum.GetValues(typeof(GridStringId)))
            {
                if (id.ToString().StartsWith("Menu"))
                {
                    string caption = GridLocalizer.Active.GetLocalizedString(id);
                    Assert(caption.Any(x => x >= '\u4e00' && x <= '\u9fff'), "Unlocalized menu: " + id);
                }
            }
            foreach (var id in Enum.GetValues(typeof(DevExpress.XtraEditors.Controls.StringId)).Cast<DevExpress.XtraEditors.Controls.StringId>()
                .Where(x => x.ToString().StartsWith("FilterClause") || x.ToString().StartsWith("TextEditMenu")))
                Assert(DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(id).Any(x => x >= '\u4e00' && x <= '\u9fff'), "Unlocalized editor menu: " + id);
            Console.WriteLine("PASS: personnel menu initialization and Traditional Chinese grid menu captions.");
            Render(true, output);
            Render(false, output);
            TestUploadState();
            TestEditorButtons(output);
            Console.WriteLine("PASS: separate upload/history forms, ISO styling, layout bounds, resize and save/dirty states.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void Render(bool upload, string output)
    {
        using (XtraForm form = upload ? (XtraForm)new f401_OffboardingUpload("VNW0014732") : new f401_OffboardingDocuments("VNW0014732"))
        using (var reference = new f201_AddNode())
        {
            MethodInfo handler = form.GetType().GetMethod("Form_Load", BindingFlags.NonPublic | BindingFlags.Instance);
            form.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), form, handler);
            form.GetType().GetField("busy", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, true);
            Field<TextEdit>(form, "employeeEdit").Text = "VNW0014732　陳文安";
            var bar = Field<Bar>(form, "commandBar");
            var referenceBar = Field<Bar>(reference, "bar2");
            Assert(bar.BarAppearance.Normal.Font.Equals(referenceBar.BarAppearance.Normal.Font), "Normal bar font differs from ISO.");
            Assert(bar.BarAppearance.Hovered.Font.Equals(referenceBar.BarAppearance.Hovered.Font), "Hovered bar font differs from ISO.");
            Assert(bar.BarAppearance.Pressed.Font.Equals(referenceBar.BarAppearance.Pressed.Font), "Pressed bar font differs from ISO.");
            Assert(bar.BarAppearance.Disabled.Font.Equals(referenceBar.BarAppearance.Disabled.Font), "Disabled bar font differs from ISO.");
            Assert(bar.OptionsBar.UseWholeRow && !bar.OptionsBar.DrawDragBorder, "ISO toolbar layout mismatch.");
            Assert(form.Appearance.BackColor == reference.Appearance.BackColor, "ISO background mismatch.");
            Assert(form.AutoScaleMode == reference.AutoScaleMode, "ISO autoscaling mismatch.");
            Assert(Field<LayoutControlGroup>(form, "root").Padding.Left == 10, "Outer padding is not 10.");
            Assert(Field<LayoutControlItem>(form, "reasonItem").Padding.Left == 2, "Item padding is not 2.");
            Assert(Field<LayoutControlItem>(form, "reasonItem").TextToControlDistance == 4, "Caption gap is not 4.");
            Assert(Field<DevExpress.XtraEditors.StyleController>(form, "style").Appearance.Font.Size == 14.25F, "Editor font mismatch.");
            if (upload) Assert(form.FormBorderStyle == reference.FormBorderStyle, "Upload window style differs from ISO.");
            if (!upload)
            {
                Field<GridControl>(form, "recordsGrid").DataSource = new[]
                {
                    new EmployeeOffboarding { Id = 1, UserId = "VNW0014732", Reason = "個人生涯規劃，已完成工作交接。", ResignDate = new DateTime(2026,11,4), CreatedAt = new DateTime(2026,11,4,10,30,0), CreatedByUserId = "VNW0000001" }
                };
            }
            Field<DevExpress.XtraEditors.MemoEdit>(form, "reasonEdit").Text = "個人生涯規劃，已完成工作交接。";
            Field<GridControl>(form, "filesGrid").DataSource = new[]
            {
                new EmployeeOffboardingFile { Id = 1, OriginalName = "離職申請書.pdf", FileSize = 123456, CreatedAt = new DateTime(2026,11,4,10,30,0), CreatedByUserId = "VNW0000001" },
                new EmployeeOffboardingFile { Id = 2, OriginalName = "工作交接確認.jpg", FileSize = 987654, CreatedAt = new DateTime(2026,11,4,10,30,0), CreatedByUserId = "VNW0000001" }
            };
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-30000, -30000);
            form.Show();
            Application.DoEvents();
            if (!upload)
            {
                var view = Field<DevExpress.XtraGrid.Views.Grid.GridView>(form, "recordsView");
                var changed = form.GetType().GetMethod("RecordsView_FocusedRowChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                view.FocusedRowChanged -= (DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler)Delegate.CreateDelegate(
                    typeof(DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler), form, changed);
            }
            else
            {
                var rows = (System.Collections.IList)Field<object>(form, "pending");
                var rowType = form.GetType().GetNestedType("PendingFile", BindingFlags.NonPublic);
                foreach (var data in new[] { new { Name = "離職申請書.pdf", Size = 123456L }, new { Name = "工作交接確認.jpg", Size = 987654L } })
                {
                    var row = Activator.CreateInstance(rowType, true);
                    rowType.GetProperty("OriginalName").SetValue(row, data.Name);
                    rowType.GetProperty("FileSize").SetValue(row, data.Size);
                    rows.Add(row);
                }
                Field<GridControl>(form, "filesGrid").DataSource = rows;
            }
            form.GetType().GetField("canUpload", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, true);
            form.GetType().GetField("busy", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, false);
            Invoke(form, "UpdateCommands");
            form.ActiveControl = Field<DevExpress.XtraEditors.MemoEdit>(form, "reasonEdit");
            Application.DoEvents();
            var grid = Field<GridControl>(form, "filesGrid");
            var reason = Field<DevExpress.XtraEditors.MemoEdit>(form, "reasonEdit");
            Assert(grid.Height >= 120, "Attachment list too small.");
            Assert(reason.Height >= 50 && reason.Height <= 150, "Reason editor size is not bounded.");
            Assert(reason.Bottom <= grid.Top, "Reason overlaps attachment list.");
            Assert(grid.Bottom <= grid.Parent.ClientSize.Height, "Attachment list extends beyond layout.");
            foreach (BarItemLink link in bar.ItemLinks)
            {
                Assert(link.Item.ImageOptions.SvgImageSize == new Size(32,32), "Toolbar SVG size mismatch.");
                Assert(link.Item.PaintStyle == BarItemPaintStyle.CaptionGlyph, "Toolbar caption/glyph style mismatch.");
            }
            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(Path.Combine(output, upload ? "offboarding-upload.png" : "offboarding-history.png"));
            }
            if (!upload)
            {
                int before = grid.Height;
                form.Size = new Size(form.Width + 200, form.Height + 150);
                Application.DoEvents();
                Assert(grid.Height > before, "Attachment list does not resize.");
                form.Size = form.MinimumSize;
                Application.DoEvents();
                Assert(reason.Bottom <= grid.Top && grid.Height >= 120, "Layout overlaps at minimum size.");
                Assert(Field<DevExpress.XtraEditors.MemoEdit>(form, "reasonEdit").Properties.ReadOnly, "History reason is editable.");
            }
        }
    }

    private static void TestUploadState()
    {
        using (var form = new f401_OffboardingUpload("VNW0014732"))
        {
            var reason = Field<DevExpress.XtraEditors.MemoEdit>(form, "reasonEdit");
            var dirty = form.GetType().GetProperty("HasUnsavedChanges", BindingFlags.NonPublic | BindingFlags.Instance);
            form.GetType().GetField("canUpload", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, true);
            reason.Text = "已輸入但尚未選擇附件";
            Assert((bool)dirty.GetValue(form), "Reason-only changes are not tracked.");
            Assert(!Field<BarButtonItem>(form, "btnSave").Enabled, "Save enabled without files.");
            var rows = (System.Collections.IList)Field<GridControl>(form, "filesGrid").DataSource;
            var rowType = form.GetType().GetNestedType("PendingFile", BindingFlags.NonPublic);
            var row = Activator.CreateInstance(rowType, true);
            rowType.GetProperty("OriginalName").SetValue(row, "附件.pdf");
            rows.Add(row);
            Assert(Field<BarButtonItem>(form, "btnSave").Enabled, "Save disabled with reason and files.");
            form.GetType().GetField("busy", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(form, true);
            Invoke(form, "UpdateCommands");
            Assert(!Field<BarButtonItem>(form, "btnSave").Enabled, "Double submission possible while busy.");
            Assert(reason.Properties.ReadOnly, "Reason can change during save.");
            var closing = new FormClosingEventArgs(CloseReason.UserClosing, false);
            form.GetType().GetMethod("Form_FormClosing", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(form, new object[] { form, closing });
            Assert(closing.Cancel, "Form closes during upload.");
        }
    }

    private static void TestEditorButtons(string output)
    {
        using (var owner = new f401_UserInfo())
        {
            var load = owner.GetType().GetMethod("f401_UserInfo_Load", BindingFlags.NonPublic | BindingFlags.Instance);
            owner.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), owner, load);
            foreach (string field in new[] { "txbDOB", "txbDateStart", "txbJobEffectiveDate", "txbResignDate" })
            {
                var editor = Field<DateEdit>(owner, field);
                Assert(editor.Properties.Buttons.Count == 1, field + " has duplicate dropdown buttons.");
            }
            foreach (string prompt in new[] { "請選離職日期", "請選職務生效日", "請選調任日期" })
            {
                Exception failure = null;
                using (var timer = new Timer { Interval = 150 })
                {
                    timer.Tick += (s, e) =>
                    {
                        var dialog = Application.OpenForms.Cast<Form>().LastOrDefault(x => x != owner);
                        if (dialog == null) return;
                        timer.Stop();
                        try
                        {
                            var editor = Descendants(dialog).OfType<DateEdit>().Single();
                            Assert(editor.Properties.Buttons.Count == 1, "Date prompt has duplicate dropdown buttons.");
                            Assert(editor.Properties.CalendarTimeProperties.Buttons.Count == 1, "Date calendar has duplicate buttons.");
                            DateTime selected = editor.DateTime.Date;
                            editor.ShowPopup();
                            Application.DoEvents();
                            Assert(editor.IsPopupOpen, "Date dropdown does not open.");
                            editor.ClosePopup();
                            Assert(editor.DateTime.Date == selected, "Opening the calendar changed the date.");
                            if (prompt == "請選離職日期")
                                using (var bitmap = new Bitmap(dialog.Width, dialog.Height))
                                {
                                    dialog.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                                    bitmap.Save(Path.Combine(output, "date-single-arrow.png"));
                                }
                        }
                        catch (Exception ex) { failure = ex; }
                        finally { dialog.DialogResult = DialogResult.Cancel; dialog.Close(); }
                    };
                    timer.Start();
                    var result = owner.GetType().GetMethod("TryPromptDate", BindingFlags.NonPublic | BindingFlags.Instance)
                        .Invoke(owner, new object[] { prompt, new DateTime(2026,11,4), new DateTime(2026,11,4) });
                    if (failure != null) throw failure;
                    Assert(!(bool)result, "Cancelling the date prompt changed the result.");
                }
            }
        }
        using (var spare = new KnowledgeSystem.Views._03_DepartmentManage._09_SparePart.f309_Transaction_Info())
        {
            Assert(Field<ImageComboBoxEdit>(spare, "cbbRecoveryOption").Properties.Buttons.Count == 1, "Recovery option has duplicate buttons.");
            Assert(Field<DateEdit>(spare, "dePlannedDisposeDate").Properties.Buttons.Count == 1, "Disposal date has duplicate buttons.");
            Assert(Field<DateEdit>(spare, "dePlannedDisposeDate").Properties.CalendarTimeProperties.Buttons.Count == 1, "Disposal calendar has duplicate buttons.");
            Assert(Field<LookUpEdit>(spare, "cbbRestockStorage").Properties.Buttons.Count == 1, "Restock storage has duplicate buttons.");
            Assert(Field<SearchLookUpEdit>(spare, "sleAssignedUser").Properties.Buttons.Count == 1, "Assigned user has duplicate buttons.");
        }
        using (var screen = new uc401_UserManage())
            Assert(Field<DevExpress.XtraEditors.Repository.RepositoryItemComboBox>(screen, "statusFilterEditor").Buttons.Count == 1,
                "Personnel status filter has duplicate buttons.");
        Console.WriteLine("PASS: single dropdown button in personnel date prompts, fixed date editors, status filter and Spare recovery editors; calendar opening/cancelling.");
    }

    private static System.Collections.Generic.IEnumerable<Control> Descendants(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            yield return child;
            foreach (Control descendant in Descendants(child)) yield return descendant;
        }
    }

    private static void TestFileStorage(string output)
    {
        string root = Path.Combine(output, "storage-" + Guid.NewGuid().ToString("N"));
        string left = Path.Combine(root, "left"), right = Path.Combine(root, "right"), storage = Path.Combine(root, "store");
        Directory.CreateDirectory(left); Directory.CreateDirectory(right); Directory.CreateDirectory(storage);
        string a = Path.Combine(left, "同名附件.pdf"), b = Path.Combine(right, "同名附件.pdf");
        File.WriteAllText(a, "first"); File.WriteAllText(b, "second");
        using (var batch = OffboardingFileStorage.Prepare(storage, new[] { a, b, a }))
        {
            Assert(batch.Files.Count == 2, "Duplicate source not excluded.");
            Assert(batch.Files[0].StoredPath != batch.Files[1].StoredPath, "Same filenames overwrite.");
            Assert(batch.Files[0].OriginalName == batch.Files[1].OriginalName, "Original filename not preserved.");
            string stored = OffboardingFileStorage.ResolvePath(storage, batch.Files[0].StoredPath);
            Assert(File.ReadAllText(stored) == "first", "Stored content corrupt.");
            string destination = Path.Combine(root, "download.pdf");
            File.WriteAllText(destination, "old");
            OffboardingFileStorage.CopyToDestination(stored, destination);
            Assert(File.ReadAllText(destination) == "first", "Download content mismatch.");
            using (var locked = new FileStream(stored, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                bool failed = false;
                try { OffboardingFileStorage.CopyToDestination(stored, destination); } catch (IOException) { failed = true; }
                Assert(failed, "Locked source was copied.");
                Assert(File.ReadAllText(destination) == "first", "Failed copy overwrote destination.");
            }
            Assert(!Directory.GetFiles(root, "*.tmp").Any(), "Download staging file leaked.");
            ExpectValidation(() => OffboardingFileStorage.ResolvePath(storage, "../outside.pdf"));
            ExpectValidation(() => OffboardingFileStorage.CopyToDestination(stored, stored));
        }
        Assert(!Directory.GetDirectories(storage).Any(), "Uncommitted batch not cleaned.");
        using (var locked = new FileStream(b, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            bool failed = false;
            try { using (OffboardingFileStorage.Prepare(storage, new[] { a, b })) { } }
            catch (IOException) { failed = true; }
            Assert(failed && !Directory.GetDirectories(storage).Any(), "Partial upload not cleaned.");
        }
        string empty = Path.Combine(root, "empty.pdf"); File.WriteAllText(empty, "");
        ExpectValidation(() => { using (OffboardingFileStorage.Prepare(storage, new[] { empty })) { } });
        string oversized = Path.Combine(root, "large.pdf");
        using (var stream = File.Create(oversized)) stream.SetLength(EmployeeOffboardingService.MaxFileSize + 1);
        ExpectValidation(() => { using (OffboardingFileStorage.Prepare(storage, new[] { oversized })) { } });
        File.Delete(oversized);
        string completed;
        using (var batch = OffboardingFileStorage.Prepare(storage, new[] { a }))
        {
            completed = OffboardingFileStorage.ResolvePath(storage, batch.Files[0].StoredPath);
            batch.Complete();
        }
        Assert(File.Exists(completed), "Committed batch deleted on dispose.");
        Console.WriteLine("PASS: real filesystem copies, duplicate names, partial failure cleanup, limits, path guard and atomic download.");
    }
}
