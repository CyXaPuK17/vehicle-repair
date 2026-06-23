using VehicleRepair.Desktop.Services;

namespace VehicleRepair.Desktop.Forms;

public class RepairIssueForm : Form
{
    private readonly ApiClient _api;
    private readonly AuthTokenService _auth;

    private DataGridView _grid = null!;
    private DateTimePicker _dtpIssued = null!;
    private Button _btnIssue = null!;
    private Label _lblStatus = null!;

    private List<RepairDto> _repairs = [];

    public RepairIssueForm(ApiClient api, AuthTokenService auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Выдача транспортного средства из ремонта";
        Size = new Size(900, 560);
        StartPosition = FormStartPosition.CenterParent;

        var pnl = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
        var dtLbl = new Label { Text = "Дата выдачи:", Location = new Point(8, 20), AutoSize = true };
        _dtpIssued = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(110, 16), Width = 140, Value = DateTime.Today };
        _btnIssue = new Button
        {
            Text = "Выдать выбранное",
            Location = new Point(270, 12),
            Width = 180,
            Height = 32,
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        _btnIssue.FlatAppearance.BorderSize = 0;
        _btnIssue.Click += BtnIssue_Click;
        pnl.Controls.AddRange(new Control[] { dtLbl, _dtpIssued, _btnIssue });

        _lblStatus = new Label { Dock = DockStyle.Top, Height = 24, ForeColor = Color.Red, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White
        };
        _grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Гос. номер", DataPropertyName = "LicensePlate", FillWeight = 10 },
            new DataGridViewTextBoxColumn { HeaderText = "ТС", DataPropertyName = "VehicleMakeModel", FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "Заказчик", DataPropertyName = "CustomerName", FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "Вид ремонта", DataPropertyName = "RepairTypeName", FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "Дата приёмки", DataPropertyName = "ReceivedAt", FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" } },
            new DataGridViewTextBoxColumn { HeaderText = "Стоимость", DataPropertyName = "Cost", FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { HeaderText = "Пробег", DataPropertyName = "Mileage", FillWeight = 8 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", DataPropertyName = "StatusLabel", FillWeight = 10 }
        );

        Controls.Add(_grid);
        Controls.Add(_lblStatus);
        Controls.Add(pnl);

        Load += async (_, _) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var res = await _api.GetAsync<List<RepairDto>>("/repairs");
            _repairs = (res.Data ?? [])
                .Where(r => r.Status < 4)
                .OrderBy(r => r.ReceivedAt)
                .ToList();

            _grid.DataSource = new System.ComponentModel.BindingList<RepairDto>(_repairs);
            _lblStatus.Text = $"Ремонтов в работе: {_repairs.Count}";
            _lblStatus.ForeColor = Color.DarkGreen;
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Ошибка загрузки: {ex.Message}";
            _lblStatus.ForeColor = Color.Red;
        }
    }

    private async void BtnIssue_Click(object? sender, EventArgs e)
    {
        if (_grid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите ремонт для выдачи.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var repair = _grid.SelectedRows[0].DataBoundItem as RepairDto;
        if (repair == null) return;

        if (repair.Status == 4)
        {
            MessageBox.Show("ТС уже выдано.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Выдать ТС {repair.LicensePlate} ({repair.VehicleMakeModel}) из ремонта?",
            "Подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        _btnIssue.Enabled = false;
        try
        {
            var req = new IssueRepairRequest { IssuedAt = _dtpIssued.Value };
            var res = await _api.PatchAsync<string>($"/repairs/{repair.Id}/issue", req);
            if (!res.Success)
            {
                MessageBox.Show(res.Error?.Message ?? "Ошибка при выдаче.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("ТС успешно выдано из ремонта.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _btnIssue.Enabled = true;
        }
    }
}
