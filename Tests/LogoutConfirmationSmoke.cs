// Không đăng xuất phiên thật, không chạy Load của Main và không mở kết nối database.
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

internal static class LogoutConfirmationSmoke
{
    private static T Field<T>(object instance, string name) =>
        (T)instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static readonly dm_User User = new dm_User
    {
        Id = "VNW0014732", DisplayName = "陳文安", DisplayNameVN = "Trần Văn An", IdDepartment = "7820", Status = 0
    };

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            Directory.CreateDirectory(args[0]);
            using (var dialog = new f00_ConfirmLogout(User))
            {
                Assert(Field<LabelControl>(dialog, "identityLabel").Text == "陳文安 · VNW0014732", "Wrong account identity.");
                Assert(dialog.Controls.Find("btnChangePass", true).Length == 0, "Password action still present.");
                Assert(ReferenceEquals(dialog.AcceptButton, dialog.CancelButton), "Cancel is not the default action.");
                Assert(Field<SimpleButton>(dialog, "btnLogout").ImageOptions.SvgImage != null, "Logout SVG missing.");
            }
            using (var dialog = new f00_ConfirmLogout(new dm_User { Id = User.Id, DisplayNameVN = User.DisplayNameVN }))
                Assert(Field<LabelControl>(dialog, "identityLabel").Text.StartsWith(User.DisplayNameVN), "Vietnamese name fallback failed.");
            using (var dialog = new f00_ConfirmLogout(new dm_User { Id = User.Id }))
                Assert(Field<LabelControl>(dialog, "identityLabel").Text == User.Id, "User ID fallback failed.");
            using (var dialog = new f00_ConfirmLogout(null))
                Assert(!Field<SimpleButton>(dialog, "btnLogout").Enabled, "Logout allowed without an account.");
            foreach (string action in new[] { "cancel", "escape", "enter", "close", "logout" })
                TestDialogAction(action, args[0]);
            TestMainCancellation(false);
            TestMainCancellation(true);
            Console.WriteLine("PASS: account name fallbacks, missing account, safe default, cancel/ESC/Enter/window close/logout results.");
            Console.WriteLine("PASS: opening from Main tile and ESC; cancellation preserves the current session.");
            Console.WriteLine("PASS: layout bounds and SVG rendering; logout-confirmation.png saved.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static void TestDialogAction(string action, string output)
    {
        using (var dialog = new f00_ConfirmLogout(User))
        using (var timer = new Timer { Interval = 150 })
        {
            Exception failure = null;
            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Location = new Point(-30000, -30000);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                try
                {
                    Assert(Field<SimpleButton>(dialog, "btnCancel").ContainsFocus, "Cancel is not focused.");
                    var identity = Field<LabelControl>(dialog, "identityLabel");
                    var question = Field<LabelControl>(dialog, "questionLabel");
                    var cancel = Field<SimpleButton>(dialog, "btnCancel");
                    var logout = Field<SimpleButton>(dialog, "btnLogout");
                    Assert(identity.Bottom <= question.Top && question.Bottom <= cancel.Top, "Dialog content overlaps.");
                    Assert(cancel.Right <= logout.Left && logout.Right <= logout.Parent.ClientSize.Width, "Buttons clipped or overlapping.");
                    if (action == "cancel")
                    {
                        using (var bitmap = new Bitmap(dialog.Width, dialog.Height))
                        {
                            dialog.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                            bitmap.Save(Path.Combine(output, "logout-confirmation.png"));
                        }
                        cancel.PerformClick();
                    }
                    else if (action == "logout") logout.PerformClick();
                    else if (action == "close") dialog.Close();
                    else
                        typeof(Form).GetMethod("ProcessDialogKey", BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(dialog, new object[] { action == "escape" ? Keys.Escape : Keys.Enter });
                }
                catch (Exception ex) { failure = ex; dialog.Close(); }
            };
            timer.Start();
            DialogResult result = dialog.ShowDialog();
            if (failure != null) throw failure;
            Assert(result == (action == "logout" ? DialogResult.OK : DialogResult.Cancel), "Wrong result for " + action);
        }
    }

    private static void TestMainCancellation(bool keyboard)
    {
        TPConfigs.LoginUser = User;
        TPConfigs.LoginSuccessful = true;
        using (var main = new f00_Main())
        using (var timer = new Timer { Interval = 150 })
        {
            var load = main.GetType().GetMethod("fMain_Load", BindingFlags.Instance | BindingFlags.NonPublic);
            main.Load -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), main, load);
            main.StartPosition = FormStartPosition.Manual;
            main.Location = new Point(-30000, -30000);
            bool seen = false;
            int ticks = 0;
            timer.Tick += (s, e) =>
            {
                var dialog = Application.OpenForms.OfType<f00_ConfirmLogout>().FirstOrDefault();
                if (dialog != null)
                {
                    seen = true;
                    timer.Stop();
                    Field<SimpleButton>(dialog, "btnCancel").PerformClick();
                }
                else if (++ticks > 50)
                {
                    timer.Stop();
                    foreach (Form other in Application.OpenForms.Cast<Form>().Where(x => x != main).ToArray()) other.Close();
                }
            };
            timer.Start();
            if (keyboard)
                main.GetType().GetMethod("ProcessCmdKey", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(main, new object[] { new Message(), Keys.Escape });
            else
                main.GetType().GetMethod("tileInfoUser_ItemClick", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(main, new object[] { main, null });
            timer.Stop();
            Assert(seen, "Main did not open the confirmation dialog.");
            Assert(ReferenceEquals(TPConfigs.LoginUser, User) && TPConfigs.LoginSuccessful, "Cancelling changed the session.");
        }
    }
}
