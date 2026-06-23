using VehicleRepair.Desktop.Services;

namespace VehicleRepair.Desktop.Forms;

public class LoginForm : Form
{
    private readonly ApiClient _api;

    private TextBox _txtLogin = null!;
    private TextBox _txtPassword = null!;
    private Button _btnLogin = null!;
    private Label _lblError = null!;

    public LoginForm(ApiClient api)
    {
        _api = api;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Вход в систему — Учёт ремонтов ТС";
        Size = new Size(360, 280);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24) };
        Controls.Add(pnl);

        var title = new Label
        {
            Text = "Учёт ремонтов ТС",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 20)
        };

        var lblLogin = new Label { Text = "Логин / ИНН:", Location = new Point(24, 68), AutoSize = true };
        _txtLogin = new TextBox { Location = new Point(24, 86), Width = 290 };

        var lblPwd = new Label { Text = "Пароль:", Location = new Point(24, 122), AutoSize = true };
        _txtPassword = new TextBox { Location = new Point(24, 140), Width = 290, PasswordChar = '●' };

        _btnLogin = new Button
        {
            Text = "Войти",
            Location = new Point(24, 180),
            Width = 290,
            Height = 36,
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10)
        };
        _btnLogin.FlatAppearance.BorderSize = 0;
        _btnLogin.Click += BtnLogin_Click;

        _lblError = new Label
        {
            Location = new Point(24, 224),
            Width = 290,
            ForeColor = Color.Red,
            AutoSize = false
        };

        // Enter key triggers login
        AcceptButton = _btnLogin;

        Controls.AddRange(new Control[] { title, lblLogin, _txtLogin, lblPwd, _txtPassword, _btnLogin, _lblError });
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        _lblError.Text = "";
        _btnLogin.Enabled = false;
        _btnLogin.Text = "Входим...";

        try
        {
            await _api.LoginAsync(_txtLogin.Text.Trim(), _txtPassword.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _lblError.Text = ex.Message;
        }
        finally
        {
            _btnLogin.Enabled = true;
            _btnLogin.Text = "Войти";
        }
    }
}
