// Ảnh các màn hình hiện có với dữ liệu mẫu; không chạy Load nghiệp vụ hoặc ghi database.
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static class EmployeeOffboardingPreview
{
    private static readonly DateTime LeaveDate = new DateTime(2026, 11, 4);
    private static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    private static void RemoveLoad(Control control, string name)
    {
        var handler = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), control, control.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic));
        if (control is Form) ((Form)control).Load -= handler;
        else ((uc401_UserManage)control).Load -= handler;
    }
    private static void Capture(Control control, string path)
    {
        control.Update();
        using (var image = new Bitmap(control.Width, control.Height))
        {
            control.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size));
            image.Save(path);
        }
    }
    private static void ShowOffscreen(Form form)
    {
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(-30000, -30000);
        form.Show();
        Application.DoEvents();
    }
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            TPConfigs.LoginUser = new dm_User { Id = "VNW0000001", Status = 0, IdDepartment = "7820" };
            TPConfigs.SoftNameTW = "人員管理";
            string output = args[0];
            Directory.CreateDirectory(output);
            RenderList(output);
            RenderEmployee(output);
            Console.WriteLine("Saved full personnel/offboarding previews. Sample data; database loading disabled.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static void RenderList(string output)
    {
        using (var screen = new uc401_UserManage())
        using (var host = new XtraForm { Text = "人員管理", ClientSize = new Size(1360, 640) })
        {
            RemoveLoad(screen, "f401_UserManager_Load");
            screen.Dock = DockStyle.Fill;
            host.Controls.Add(screen);
            var grid = Field<GridControl>(screen, "gcData");
            var people = new[]
            {
                new dm_User { Id="VNW0014732", DisplayName="陳文安", DisplayNameVN="Trần Văn An", IdDepartment="7820", Status=0, DateCreate=new DateTime(2019,6,10), ResignDate=null, Sex=true, JobCode="E01", Nationality="VN" },
                new dm_User { Id="VNW0015681", DisplayName="阮氏蘭", DisplayNameVN="Nguyễn Thị Lan", IdDepartment="7820", Status=0, DateCreate=new DateTime(2021,3,15), Sex=false, JobCode="E02", Nationality="VN" },
                new dm_User { Id="VNW0011045", DisplayName="黎文平", DisplayNameVN="Lê Văn Bình", IdDepartment="7820", Status=1, DateCreate=new DateTime(2017,8,1), ResignDate=LeaveDate, Sex=true, JobCode="E01", Nationality="VN" }
            };
            grid.DataSource = people.Select(x => new
            {
                Data = x, DisplayName = x.DisplayName + "\r\n" + x.DisplayNameVN, DeptName = "7820\r\n環境安全處",
                JobName = "工程師", ActualJobName = "工程師", SexName = x.Sex == true ? "男" : "女", StatusName = x.Status == 1 ? "離職" : "在職"
            }).ToList();
            ShowOffscreen(host);
            Field<BarEditItem>(screen, "statusFilter").EditValue = "在職人員";
            screen.GetType().GetMethod("StatusFilter_EditValueChanged", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(screen, new object[] { screen, EventArgs.Empty });
            Capture(host, Path.Combine(output, "01-employee-list.png"));
            Field<BarEditItem>(screen, "statusFilter").EditValue = "已離職人員";
            Application.DoEvents();
            Capture(host, Path.Combine(output, "07-departed-employee-list.png"));
        }
    }

    private static void RenderEmployee(string output)
    {
        using (var form = new f401_UserInfo())
        {
            RemoveLoad(form, "f401_UserInfo_Load");
            form.Text = "人員資訊";
            var layout = Field<TabbedControlGroup>(form, "lcUserInfo");
            foreach (string name in new[] { "lcRole", "lcSign", "lcGroup" })
                Field<LayoutControlGroup>(form, name).Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layout.SelectedTabPageIndex = 0;
            var values = new Dictionary<string, object>
            {
                { "txbUserId", "VNW0014732" }, { "txbUserNameTW", "陳文安" }, { "txbUserNameVN", "Trần Văn An" },
                { "txbDateStart", new DateTime(2019,6,10) }, { "txbDOB", new DateTime(1992,5,20) },
                { "txbCCCD", "079000000000" }, { "cbbNationality", "VN" }, { "cbbSex", "男" },
                { "cbbStatus", "在職" }, { "txbPhone1", "0900000000" }, { "txbPhone2", "" },
                { "txbAddr", "河靜省" }, { "txbRecognizedEducation", "公大學工" }, { "txbEducationName", "河內工業大學" },
                { "txbJobEffectiveDate", new DateTime(2020,1,1) }
            };
            foreach (var value in values)
            {
                var editor = Field<BaseEdit>(form, value.Key);
                editor.EditValue = value.Value;
                editor.Properties.ReadOnly = true;
            }
            var dept = Field<LookUpEdit>(form, "cbbDept");
            dept.Properties.DataSource = new[] { new { Id = "7820", DisplayName = "環境安全處" } };
            dept.Properties.ValueMember = "Id"; dept.Properties.DisplayMember = "DisplayName"; dept.EditValue = "7820";
            foreach (string name in new[] { "cbbJobTitle", "cbbActualJob" })
            {
                var editor = Field<SearchLookUpEdit>(form, name);
                editor.Properties.DataSource = new[] { new { Id = "E01", DisplayName = "工程師" } };
                editor.Properties.ValueMember = "Id"; editor.Properties.DisplayMember = "DisplayName"; editor.EditValue = "E01";
                editor.Properties.ReadOnly = true;
            }
            Field<BarButtonItem>(form, "btnConfirm").Visibility = BarItemVisibility.Never;
            Field<BarButtonItem>(form, "btnDelete").Visibility = BarItemVisibility.Never;
            Field<BarButtonItem>(form, "btnResumeWork").Visibility = BarItemVisibility.Never;
            ShowOffscreen(form);
            Capture(form, Path.Combine(output, "02-employee-profile.png"));
            var rootLink = Field<BarManager>(form, "barManagerTP").Bars.Cast<Bar>().SelectMany(x => x.ItemLinks.Cast<BarItemLink>())
                .OfType<BarSubItemLink>().First();
            rootLink.OpenMenu();
            Application.DoEvents();
            CapturePopup(form, Path.Combine(output, "03-personnel-changes-menu.png"));
            rootLink.CloseMenu();
            using (var timer = new Timer { Interval = 250 })
            {
                timer.Tick += (s, e) =>
                {
                    var dialog = Application.OpenForms.Cast<Form>().LastOrDefault(x => x != form);
                    if (dialog == null) return;
                    timer.Stop();
                    Capture(dialog, Path.Combine(output, "04-select-resign-date.png"));
                    dialog.DialogResult = DialogResult.Cancel;
                    dialog.Close();
                };
                timer.Start();
                form.GetType().GetMethod("TryPromptDate", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(form, new object[] { "請選離職日期", LeaveDate, LeaveDate });
            }
            Field<ComboBoxEdit>(form, "cbbStatus").EditValue = "離職";
            Field<DateEdit>(form, "txbResignDate").EditValue = LeaveDate;
            Field<BarButtonItem>(form, "btnEdit").Visibility = BarItemVisibility.Never;
            Field<BarSubItem>(form, "btnPersonnelChanges").Visibility = BarItemVisibility.Never;
            Field<BarButtonItem>(form, "btnConfirm").Visibility = BarItemVisibility.Always;
            Capture(form, Path.Combine(output, "05-confirm-resignation.png"));
            using (var timer = new Timer { Interval = 250 })
            {
                timer.Tick += (s, e) =>
                {
                    var dialog = Application.OpenForms.Cast<Form>().LastOrDefault(x => x != form);
                    if (dialog == null) return;
                    timer.Stop();
                    Capture(dialog, Path.Combine(output, "04b-certificate-impact.png"));
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                };
                timer.Start();
                MsgTP.MsgShowInfomation("<font='Microsoft JhengHei UI' size=14><color=blue>以下共2證書將返回狀態「無效」:</color></br>001 職業安全衛生訓練\r\n002 消防安全訓練</font>");
            }
        }
    }

    private delegate bool WindowCallback(IntPtr window, IntPtr argument);
    [DllImport("user32.dll")] private static extern bool EnumThreadWindows(uint thread, WindowCallback callback, IntPtr argument);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);
    private static void CapturePopup(Form owner, string path)
    {
        Control popup = null;
        EnumThreadWindows(GetCurrentThreadId(), (window, arg) =>
        {
            Control control = Control.FromHandle(window);
            if (window != owner.Handle && IsWindowVisible(window) && control != null && control.Width > 100 && control.Height > 100)
                if (popup == null || control.Width * control.Height > popup.Width * popup.Height) popup = control;
            return true;
        }, IntPtr.Zero);
        if (popup == null) throw new Exception("Could not locate the personnel changes popup.");
        Capture(popup, path);
    }
}
