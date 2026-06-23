using VehicleRepair.Desktop.Services;

namespace VehicleRepair.Desktop.Forms;

public class MainForm : Form
{
    private readonly ApiClient _api;
    private readonly AuthTokenService _auth;

    public MainForm(ApiClient api, AuthTokenService auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Учёт ремонтов ТС";
        Size = new Size(600, 420);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var menuStrip = new MenuStrip();

        var repairMenu = new ToolStripMenuItem("Ремонты");
        var mnuReceive = new ToolStripMenuItem("Приёмка ТС в ремонт");
        var mnuIssue = new ToolStripMenuItem("Выдача ТС из ремонта");
        mnuReceive.Click += (_, _) => OpenForm(new RepairReceiveForm(_api, _auth));
        mnuIssue.Click += (_, _) => OpenForm(new RepairIssueForm(_api, _auth));
        repairMenu.DropDownItems.AddRange(new ToolStripItem[] { mnuReceive, new ToolStripSeparator(), mnuIssue });

        var appMenu = new ToolStripMenuItem("Система");
        var mnuLogout = new ToolStripMenuItem("Выйти из системы");
        mnuLogout.Click += MnuLogout_Click;
        appMenu.DropDownItems.Add(mnuLogout);

        menuStrip.Items.AddRange(new ToolStripItem[] { repairMenu, appMenu });
        MainMenuStrip = menuStrip;
        Controls.Add(menuStrip);

        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32) };
        Controls.Add(pnl);

        var greet = new Label
        {
            Text = $"Добро пожаловать, {_auth.Role ?? "пользователь"}!\n\nИспользуйте меню для работы с ремонтами:\n  • Приёмка ТС — регистрация поступления в ремонт\n  • Выдача ТС — оформление выдачи после ремонта",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnl.Controls.Add(greet);
    }

    private void OpenForm(Form form)
    {
        form.ShowDialog(this);
    }

    private void MnuLogout_Click(object? sender, EventArgs e)
    {
        _auth.Clear();
        var login = new LoginForm(_api);
        if (login.ShowDialog() == DialogResult.OK)
        {
            // stay open — re-logged in
        }
        else
        {
            Application.Exit();
        }
    }
}
