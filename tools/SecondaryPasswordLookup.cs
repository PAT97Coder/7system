using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new PasswordLookupForm());
    }
}

internal sealed class PasswordLookupForm : Form
{
    private readonly TextBox userIdTextBox = new TextBox();
    private readonly TextBox passwordTextBox = new TextBox();
    private readonly Button lookupButton = new Button();
    private readonly Button copyButton = new Button();
    private readonly Label statusLabel = new Label();

    public PasswordLookupForm()
    {
        Text = "Tra cứu mật khẩu";
        ClientSize = new Size(520, 175);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10F);

        Label userLabel = CreateLabel("Mã số:", 20, 25, 90, 28);
        userIdTextBox.SetBounds(110, 22, 270, 30);
        userIdTextBox.MaxLength = 7;
        userIdTextBox.KeyPress += OnlyAllowDigits;

        lookupButton.Text = "Tra cứu";
        lookupButton.SetBounds(395, 21, 100, 32);
        lookupButton.Click += async delegate { await LookupAsync(); };

        Label passwordLabel = CreateLabel("Mật khẩu:", 20, 76, 90, 28);
        passwordTextBox.SetBounds(110, 73, 270, 30);
        passwordTextBox.ReadOnly = true;
        passwordTextBox.Font = new Font("Consolas", 11F);

        copyButton.Text = "Sao chép";
        copyButton.SetBounds(395, 72, 100, 32);
        copyButton.Click += CopyPassword;

        statusLabel.SetBounds(20, 125, 475, 30);
        statusLabel.Text = "Chỉ nhập phần số, ví dụ: 14732.";

        Controls.AddRange(new Control[]
        {
            userLabel, userIdTextBox, lookupButton,
            passwordLabel, passwordTextBox, copyButton, statusLabel
        });

        AcceptButton = lookupButton;
        Shown += delegate { userIdTextBox.Focus(); };
    }

    private static Label CreateLabel(string text, int x, int y, int width, int height)
    {
        return new Label
        {
            Text = text,
            Left = x,
            Top = y,
            Width = width,
            Height = height,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    private async Task LookupAsync()
    {
        string employeeNumber = userIdTextBox.Text.Trim();
        if (employeeNumber.Length == 0 ||
            employeeNumber.Length > 7 ||
            !IsDigitsOnly(employeeNumber))
        {
            MessageBox.Show(
                this,
                "Chỉ nhập từ 1 đến 7 chữ số.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            userIdTextBox.Focus();
            return;
        }

        string userId = "VNW" + employeeNumber.PadLeft(7, '0');

        lookupButton.Enabled = false;
        statusLabel.Text = "Đang tra cứu " + userId + "...";

        try
        {
            string password = await Task.Run(delegate { return Lookup(userId); });
            passwordTextBox.Text = password;
            statusLabel.Text = "Đã tra cứu thành công " + userId + ".";
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Tra cứu thất bại.";
            MessageBox.Show(
                this,
                exception.Message,
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            lookupButton.Enabled = true;
        }
    }

    private static string Lookup(string userId)
    {
        string connectionString = LoadConnectionString();
        string encryptedPassword;

        using (SqlConnection connection = new SqlConnection(connectionString))
        using (SqlCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "SELECT SecondaryPassword FROM dbo.dm_User WHERE Id = @Id";
            command.Parameters.Add("@Id", SqlDbType.VarChar, 10).Value = userId;
            connection.Open();

            object value = command.ExecuteScalar();
            if (value == null || value == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy nhân viên hoặc SecondaryPassword đang trống: " + userId);
            }

            encryptedPassword = Convert.ToString(value);
        }

        return DecryptPass(encryptedPassword);
    }

    private static string LoadConnectionString()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            DataSource = "10.198.138.103",
            InitialCatalog = "DBDocumentManagementSystem",
            UserID = "PAT",
            Password = "Anhtuan312",
            ConnectTimeout = 8,
            MultipleActiveResultSets = true,
            ApplicationName = "SecondaryPasswordLookup"
        };

        return builder.ConnectionString;
    }

    // Port từ BusinessLayer/EncryptionHelper.DecryptPass.
    internal static string DecryptPass(string encryptedText)
    {
        if (String.IsNullOrEmpty(encryptedText))
        {
            throw new ArgumentException("SecondaryPassword đang trống.");
        }

        string[] splitText = encryptedText.Split('|');
        string originalString = splitText[splitText.Length - 1];
        char[] characters = originalString.ToCharArray();
        Array.Reverse(characters);
        string reversedString = new string(characters);

        int salt3Length = 0;
        for (int index = 0; index < reversedString.Length / 4; index++)
        {
            char character = reversedString[index * 4 + 3];
            if (!Char.IsLetterOrDigit(character))
            {
                salt3Length++;
            }
        }

        StringBuilder result = new StringBuilder();
        int firstLength = salt3Length * 4;
        for (int index = 0; index < firstLength / 4; index++)
        {
            result.Append(reversedString[index * 4 + 2]);
        }

        if (reversedString.Length != firstLength)
        {
            string second = reversedString.Substring(firstLength, 64 - firstLength);
            for (int index = 0; index < second.Length / 3; index++)
            {
                result.Append(second[index * 3 + 2]);
            }
        }

        return result.ToString();
    }

    private void CopyPassword(object sender, EventArgs eventArgs)
    {
        if (!String.IsNullOrEmpty(passwordTextBox.Text))
        {
            Clipboard.SetText(passwordTextBox.Text);
            statusLabel.Text = "Đã sao chép mật khẩu vào clipboard.";
        }
    }

    private static void OnlyAllowDigits(object sender, KeyPressEventArgs eventArgs)
    {
        if (!Char.IsControl(eventArgs.KeyChar) && !Char.IsDigit(eventArgs.KeyChar))
        {
            eventArgs.Handled = true;
        }
    }

    private static bool IsDigitsOnly(string value)
    {
        for (int index = 0; index < value.Length; index++)
        {
            if (!Char.IsDigit(value[index]))
            {
                return false;
            }
        }

        return true;
    }
}
