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
        var mnuIssue = new ToolStripMenuItem("Ремонты в работе");
        mnuIssue.Click += (_, _) => OpenForm(new RepairIssueForm(_api, _auth));
        repairMenu.DropDownItems.Add(mnuIssue);

        // Приёмку ТС в ремонт может выполнять только Исполнитель — сервер отклоняет остальные роли.
        if (_auth.Role == "Executor")
        {
            var mnuReceive = new ToolStripMenuItem("Приёмка ТС в ремонт");
            mnuReceive.Click += (_, _) => OpenForm(new RepairReceiveForm(_api, _auth));
            repairMenu.DropDownItems.Insert(0, mnuReceive);
            repairMenu.DropDownItems.Insert(1, new ToolStripSeparator());
        }

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
            Text = $"Добро пожаловать, {_auth.Role ?? "пользователь"}!\n\nИспользуйте меню для работы с ремонтами:\n  • Приёмка ТС — регистрация поступления в ремонт\n  • Ремонты в работе — начать/завершить/выдать ремонт",
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
