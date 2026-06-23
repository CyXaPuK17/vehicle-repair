using VehicleRepair.Desktop.Services;

namespace VehicleRepair.Desktop.Forms;

public class RepairReceiveForm : Form
{
    private readonly ApiClient _api;
    private readonly AuthTokenService _auth;

    private ComboBox _cmbVehicle = null!;
    private ComboBox _cmbRepairType = null!;
    private DateTimePicker _dtpReceived = null!;
    private NumericUpDown _nudCost = null!;
    private NumericUpDown _nudMileage = null!;
    private TextBox _txtComment = null!;
    private Button _btnSave = null!;
    private Label _lblStatus = null!;

    private List<VehicleDto> _vehicles = [];
    private List<RepairTypeDto> _repairTypes = [];

    public RepairReceiveForm(ApiClient api, AuthTokenService auth)
    {
        _api = api;
        _auth = auth;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Приёмка транспортного средства в ремонт";
        Size = new Size(480, 460);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        int y = 16;
        int lw = 200;
        int fw = 240;
        int lx = 16;
        int fx = 220;

        Label Lbl(string t) => new() { Text = t, Location = new Point(lx, y), AutoSize = true };
        void AddRow(string label, Control ctl)
        {
            ctl.Location = new Point(fx, y);
            ctl.Width = fw;
            Controls.Add(Lbl(label));
            Controls.Add(ctl);
            y += 40;
        }

        _cmbVehicle = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbRepairType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        _dtpReceived = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        _nudCost = new NumericUpDown { Minimum = 0, Maximum = 99_999_999, DecimalPlaces = 2, ThousandsSeparator = true };
        _nudMileage = new NumericUpDown { Minimum = 0, Maximum = 9_999_999 };
        _txtComment = new TextBox { Multiline = true, Height = 60 };

        AddRow("Транспортное средство:", _cmbVehicle);
        AddRow("Вид ремонта:", _cmbRepairType);
        AddRow("Дата приёмки:", _dtpReceived);
        AddRow("Стоимость (руб.):", _nudCost);
        AddRow("Пробег (км):", _nudMileage);

        Controls.Add(new Label { Text = "Комментарий:", Location = new Point(lx, y), AutoSize = true });
        y += 20;
        _txtComment.Location = new Point(lx, y);
        _txtComment.Width = 430;
        Controls.Add(_txtComment);
        y += 70;

        _lblStatus = new Label { Location = new Point(lx, y), Width = 430, ForeColor = Color.Red };
        Controls.Add(_lblStatus);
        y += 28;

        _btnSave = new Button
        {
            Text = "Принять в ремонт",
            Location = new Point(lx, y),
            Width = 200,
            Height = 36,
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        _btnSave.FlatAppearance.BorderSize = 0;
        _btnSave.Click += BtnSave_Click;
        Controls.Add(_btnSave);

        Load += async (_, _) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var vRes = await _api.GetAsync<List<VehicleDto>>("/vehicles");
            var rRes = await _api.GetAsync<List<RepairTypeDto>>("/repair-types");

            _vehicles = vRes.Data ?? [];
            _repairTypes = (rRes.Data ?? []).Where(r => r.IsActive).ToList();

            _cmbVehicle.DataSource = _vehicles;
            _cmbVehicle.DisplayMember = nameof(VehicleDto.LicensePlate);
            _cmbVehicle.ValueMember = nameof(VehicleDto.Id);

            _cmbRepairType.DataSource = _repairTypes;
            _cmbRepairType.DisplayMember = nameof(RepairTypeDto.Name);
            _cmbRepairType.ValueMember = nameof(RepairTypeDto.Id);
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Ошибка загрузки данных: {ex.Message}";
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        _lblStatus.Text = "";

        if (_cmbVehicle.SelectedValue is not string vehicleId || string.IsNullOrEmpty(vehicleId))
        {
            _lblStatus.Text = "Выберите транспортное средство.";
            return;
        }
        if (_cmbRepairType.SelectedValue is not string repairTypeId || string.IsNullOrEmpty(repairTypeId))
        {
            _lblStatus.Text = "Выберите вид ремонта.";
            return;
        }

        _btnSave.Enabled = false;
        try
        {
            var req = new CreateRepairRequest
            {
                VehicleId = vehicleId,
                RepairTypeId = repairTypeId,
                ReceivedAt = _dtpReceived.Value,
                Cost = _nudCost.Value,
                Mileage = (int)_nudMileage.Value,
                Comment = string.IsNullOrWhiteSpace(_txtComment.Text) ? null : _txtComment.Text.Trim()
            };

            var res = await _api.PostAsync<string>("/repairs", req);
            if (!res.Success)
            {
                _lblStatus.Text = res.Error?.Message ?? "Ошибка сохранения";
                return;
            }

            MessageBox.Show("ТС принято в ремонт.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _lblStatus.Text = ex.Message;
        }
        finally
        {
            _btnSave.Enabled = true;
        }
    }
}
