using VehicleRepair.Desktop.Services;

namespace VehicleRepair.Desktop.Forms;

public class RepairIssueForm : Form
{
    private readonly ApiClient _api;
    private readonly AuthTokenService _auth;

    private DataGridView _grid = null!;
    private DateTimePicker _dtpIssued = null!;
    private Button _btnStart = null!;
    private Button _btnComplete = null!;
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
        Text = "Ремонты в работе";
        Size = new Size(960, 560);
        StartPosition = FormStartPosition.CenterParent;

        var pnl = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(8) };
        var dtLbl = new Label { Text = "Дата выдачи:", Location = new Point(8, 20), AutoSize = true };
        _dtpIssued = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(100, 16), Width = 120, Value = DateTime.Today };

        _btnStart = MakeActionButton("Начать ремонт", 240);
        _btnComplete = MakeActionButton("Завершить ремонт", 400);
        _btnIssue = MakeActionButton("Выдать выбранное", 570);

        _btnStart.Click += BtnStart_Click;
        _btnComplete.Click += BtnComplete_Click;
        _btnIssue.Click += BtnIssue_Click;

        // "Начать"/"Завершить" на сервере разрешены только роли Executor — остальным их не показываем.
        _btnStart.Visible = _btnComplete.Visible = _auth.Role == "Executor";

        pnl.Controls.AddRange(new Control[] { dtLbl, _dtpIssued, _btnStart, _btnComplete, _btnIssue });

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
            AutoGenerateColumns = false,
            BackgroundColor = Color.White
        };
        _grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Гос. номер", DataPropertyName = "LicensePlate", FillWeight = 10 },
            new DataGridViewTextBoxColumn { HeaderText = "ТС", DataPropertyName = "VehicleMakeModel", FillWeight = 18 },
            new DataGridViewTextBoxColumn { HeaderText = "Заказчик", DataPropertyName = "CustomerName", FillWeight = 18 },
            new DataGridViewTextBoxColumn { HeaderText = "Вид ремонта", DataPropertyName = "RepairTypeName", FillWeight = 18 },
            new DataGridViewTextBoxColumn { HeaderText = "Дата приёмки", DataPropertyName = "ReceivedAt", FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" } },
            new DataGridViewTextBoxColumn { HeaderText = "Стоимость", DataPropertyName = "Cost", FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight } },
            new DataGridViewTextBoxColumn { HeaderText = "Пробег", DataPropertyName = "Mileage", FillWeight = 8 },
            new DataGridViewTextBoxColumn { HeaderText = "Статус", DataPropertyName = "StatusLabel", FillWeight = 10 }
        );
        _grid.SelectionChanged += (_, _) => UpdateActionButtons();

        Controls.Add(_grid);
        Controls.Add(_lblStatus);
        Controls.Add(pnl);

        UpdateActionButtons();
        Load += async (_, _) => await LoadDataAsync();
    }

    private static Button MakeActionButton(string text, int x) => new()
    {
        Text = text,
        Location = new Point(x, 12),
        Width = 150,
        Height = 32,
        BackColor = Color.FromArgb(22, 119, 255),
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat
    };

    private RepairDto? SelectedRepair =>
        _grid.SelectedRows.Count > 0 ? _grid.SelectedRows[0].DataBoundItem as RepairDto : null;

    private void UpdateActionButtons()
    {
        var status = SelectedRepair?.Status;
        _btnStart.Enabled = status == "Received";
        _btnComplete.Enabled = status == "InProgress";
        _btnIssue.Enabled = status is not null;
        _dtpIssued.Enabled = status is not null;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var res = await _api.GetAsync<PagedResultDto<RepairDto>>("/repairs?pageSize=200");
            _repairs = (res.Data?.Items ?? [])
                .Where(r => r.Status != "Issued")
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
        finally
        {
            UpdateActionButtons();
        }
    }

    private async Task RunActionAsync(Func<RepairDto, string> buildPath, string confirmMessage, string successMessage, Button trigger)
    {
        var repair = SelectedRepair;
        if (repair == null)
        {
            MessageBox.Show("Выберите ремонт.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            string.Format(confirmMessage, repair.LicensePlate, repair.VehicleMakeModel),
            "Подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        trigger.Enabled = false;
        try
        {
            var res = await _api.PatchAsync<string>(buildPath(repair), new { });
            if (!res.Success)
            {
                MessageBox.Show(res.Error?.Message ?? "Ошибка выполнения операции.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(successMessage, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UpdateActionButtons();
        }
    }

    private async void BtnStart_Click(object? sender, EventArgs e) =>
        await RunActionAsync(
            repair => $"/repairs/{repair.Id}/start",
            "Взять в работу ремонт {0} ({1})?",
            "Ремонт взят в работу.",
            _btnStart);

    private async void BtnComplete_Click(object? sender, EventArgs e) =>
        await RunActionAsync(
            repair => $"/repairs/{repair.Id}/complete",
            "Отметить ремонт {0} ({1}) завершённым?",
            "Ремонт завершён.",
            _btnComplete);

    private async void BtnIssue_Click(object? sender, EventArgs e)
    {
        var repair = SelectedRepair;
        if (repair == null)
        {
            MessageBox.Show("Выберите ремонт для выдачи.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            MessageBox.Show($"ТС успешно выдано из ремонта. Дата выдачи: {_dtpIssued.Value:dd.MM.yyyy}.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UpdateActionButtons();
        }
    }
}
