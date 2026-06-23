using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Infrastructure.Persistence;

namespace VehicleRepair.Infrastructure.Seed;

public class DatabaseSeeder
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IPasswordHasher _hasher;

    public DatabaseSeeder(AppDbContext db, IConfiguration config, ILogger<DatabaseSeeder> logger, IPasswordHasher hasher)
    {
        _db = db;
        _config = config;
        _logger = logger;
        _hasher = hasher;
    }

    // ── Admin / Manager ────────────────────────────────────────────────────────
    private static readonly Guid AdminId   = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid ManagerId = new("00000000-0000-0000-0000-000000000002");

    // ── Repair Types ───────────────────────────────────────────────────────────
    private static readonly Guid RT1  = new("a0000001-0000-0000-0000-000000000001"); // ТО-1
    private static readonly Guid RT2  = new("a0000001-0000-0000-0000-000000000002"); // ТО-2
    private static readonly Guid RT3  = new("a0000001-0000-0000-0000-000000000003"); // Кузовной ремонт
    private static readonly Guid RT4  = new("a0000001-0000-0000-0000-000000000004"); // Двигатель
    private static readonly Guid RT5  = new("a0000001-0000-0000-0000-000000000005"); // Трансмиссия
    private static readonly Guid RT6  = new("a0000001-0000-0000-0000-000000000006"); // Подвеска
    private static readonly Guid RT7  = new("a0000001-0000-0000-0000-000000000007"); // Тормозная система
    private static readonly Guid RT8  = new("a0000001-0000-0000-0000-000000000008"); // Электрика
    private static readonly Guid RT9  = new("a0000001-0000-0000-0000-000000000009"); // Шиномонтаж
    private static readonly Guid RT10 = new("a0000001-0000-0000-0000-000000000010"); // Прочее

    // ── Customers — юр. лица ──────────────────────────────────────────────────
    private static readonly Guid C1 = new("10000001-0000-0000-0000-000000000001"); // ООО «Городской Транспорт»
    private static readonly Guid C2 = new("10000001-0000-0000-0000-000000000002"); // АО «СпецТранс»
    private static readonly Guid C3 = new("10000001-0000-0000-0000-000000000003"); // МУП «Городские Перевозки»
    private static readonly Guid C4 = new("10000001-0000-0000-0000-000000000004"); // ООО «Логистик Груп»
    private static readonly Guid C5 = new("10000001-0000-0000-0000-000000000005"); // ЗАО «РусТранс»

    // ── Customers — физ. лица ─────────────────────────────────────────────────
    private static readonly Guid C6 = new("10000001-0000-0000-0000-000000000006"); // Иванов Иван Иванович
    private static readonly Guid C7 = new("10000001-0000-0000-0000-000000000007"); // Петрова Анна Сергеевна
    private static readonly Guid C8 = new("10000001-0000-0000-0000-000000000008"); // Сидоров Алексей Михайлович
    private static readonly Guid C9 = new("10000001-0000-0000-0000-000000000009"); // Козлова Мария Дмитриевна

    // ── Executors ──────────────────────────────────────────────────────────────
    private static readonly Guid E1 = new("20000002-0000-0000-0000-000000000001"); // ООО «АвтоСервис Профи»
    private static readonly Guid E2 = new("20000002-0000-0000-0000-000000000002"); // ООО «ТехМастер»
    private static readonly Guid E3 = new("20000002-0000-0000-0000-000000000003"); // ИП Петров С.В.
    private static readonly Guid E4 = new("20000002-0000-0000-0000-000000000004"); // ООО «МоторТех»

    // ── Vehicles ───────────────────────────────────────────────────────────────
    // C1 — ООО «Городской Транспорт» (автобусы)
    private static readonly Guid V01 = new("30000003-0000-0000-0000-000000000001"); // А101АА77 ПАЗ 3205
    private static readonly Guid V02 = new("30000003-0000-0000-0000-000000000002"); // В202ВВ77 ЛиАЗ 5292
    private static readonly Guid V03 = new("30000003-0000-0000-0000-000000000003"); // К303КК77 НефАЗ 5299
    // C2 — АО «СпецТранс» (спецтехника)
    private static readonly Guid V04 = new("30000003-0000-0000-0000-000000000004"); // С001СС77 КамАЗ 65115
    private static readonly Guid V05 = new("30000003-0000-0000-0000-000000000005"); // Р002РР77 МАЗ 5551
    private static readonly Guid V06 = new("30000003-0000-0000-0000-000000000006"); // Т003ТТ77 Урал 4320
    // C3 — МУП «Городские Перевозки» (автобусы)
    private static readonly Guid V07 = new("30000003-0000-0000-0000-000000000007"); // М101МА50 MAN Lion's City
    private static readonly Guid V08 = new("30000003-0000-0000-0000-000000000008"); // М102МВ50 МАЗ 203
    private static readonly Guid V09 = new("30000003-0000-0000-0000-000000000009"); // М103МК50 НефАЗ 5299
    // C4 — ООО «Логистик Груп» (грузовики)
    private static readonly Guid V10 = new("30000003-0000-0000-0000-000000000010"); // О201ОА50 КамАЗ 5490
    private static readonly Guid V11 = new("30000003-0000-0000-0000-000000000011"); // О202ОВ50 Volvo FH
    private static readonly Guid V12 = new("30000003-0000-0000-0000-000000000012"); // О203ОК50 MAN TGX
    private static readonly Guid V13 = new("30000003-0000-0000-0000-000000000013"); // О204ОМ50 Scania R500
    // C5 — ЗАО «РусТранс» (смешанный парк)
    private static readonly Guid V14 = new("30000003-0000-0000-0000-000000000014"); // У301УА66 Toyota Camry
    private static readonly Guid V15 = new("30000003-0000-0000-0000-000000000015"); // У302УВ66 Mercedes Sprinter
    private static readonly Guid V16 = new("30000003-0000-0000-0000-000000000016"); // У303УК66 VW Crafter
    // Физ. лица
    private static readonly Guid V17 = new("30000003-0000-0000-0000-000000000017"); // А777ОО77 Lada Vesta (C6)
    private static readonly Guid V18 = new("30000003-0000-0000-0000-000000000018"); // В321РС77 Hyundai Solaris (C7)
    private static readonly Guid V19 = new("30000003-0000-0000-0000-000000000019"); // К456МН50 Kia Rio (C8)
    private static readonly Guid V20 = new("30000003-0000-0000-0000-000000000020"); // Т789ОР66 VW Polo (C9)

    // ──────────────────────────────────────────────────────────────────────────

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await _db.Database.MigrateAsync(ct);

        await SeedRepairTypesAsync(ct);
        await SeedAdminUsersAsync(ct);
        await SeedCustomersAsync(ct);
        await SeedExecutorsAsync(ct);
        await SeedVehiclesAsync(ct);
        await SeedRepairsAsync(ct);

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Database seeding completed.");
    }

    private async Task SeedRepairTypesAsync(CancellationToken ct)
    {
        if (await _db.RepairTypes.AnyAsync(ct)) return;

        _db.RepairTypes.AddRange(
            RT("ТО-1",             "Техническое обслуживание №1 — регламентные работы",                  RT1),
            RT("ТО-2",             "Техническое обслуживание №2 — расширенный регламент",                 RT2),
            RT("Кузовной ремонт",  "Рихтовка, окраска, устранение повреждений кузова",                   RT3),
            RT("Двигатель",        "Ремонт и замена двигателя, ГРМ, систем охлаждения",                  RT4),
            RT("Трансмиссия",      "КПП, сцепление, карданный вал, редуктор",                            RT5),
            RT("Подвеска",         "Амортизаторы, рычаги, рулевые тяги, ШРУС",                           RT6),
            RT("Тормозная система", "Колодки, диски, суппорты, тормозные цилиндры",                      RT7),
            RT("Электрика",        "Электрооборудование, проводка, АКБ, генератор",                      RT8),
            RT("Шиномонтаж",       "Замена, балансировка и хранение шин",                                RT9),
            RT("Прочее",           "Иные виды работ, не вошедшие в классификатор",                       RT10)
        );

        _logger.LogInformation("Seeded 10 repair types.");
    }

    private async Task SeedAdminUsersAsync(CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(ct)) return;

        var adminPwd   = _config["Seed:AdminPassword"]   ?? throw new InvalidOperationException("Seed:AdminPassword not configured");
        var managerPwd = _config["Seed:ManagerPassword"] ?? throw new InvalidOperationException("Seed:ManagerPassword not configured");
        var adminLogin   = _config["Seed:AdminLogin"]   ?? "admin";
        var managerLogin = _config["Seed:ManagerLogin"] ?? "uk_manager";

        _db.Users.AddRange(
            new User { Id = AdminId,   Login = adminLogin,   PasswordHash = _hasher.Hash(adminPwd),   Role = UserRole.ManagementCompany, IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Id = ManagerId, Login = managerLogin, PasswordHash = _hasher.Hash(managerPwd), Role = UserRole.ManagementCompany, IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        _logger.LogDebug("Seeded admin users.");
    }

    private async Task SeedCustomersAsync(CancellationToken ct)
    {
        if (await _db.Customers.AnyAsync(ct)) return;

        // Юр. лица: login = INN (10 цифр), password = INN
        var companies = new[]
        {
            Cust(C1, "7701342018", "ООО «Городской Транспорт»",        "Смирнов Андрей Викторович",    "+7 (495) 234-56-78", "a.smirnov@gorodtrans.ru"),
            Cust(C2, "7709854321", "АО «СпецТранс»",                   "Козлов Михаил Петрович",       "+7 (495) 345-67-89", "m.kozlov@spectrans.ru"),
            Cust(C3, "5001234569", "МУП «Городские Перевозки»",        "Новикова Елена Сергеевна",     "+7 (498) 765-43-21", "e.novikova@mup-perevozki.ru"),
            Cust(C4, "5009431087", "ООО «Логистик Груп»",              "Орлов Денис Александрович",    "+7 (496) 543-21-09", "d.orlov@logistic-grup.ru"),
            Cust(C5, "6612109876", "ЗАО «РусТранс»",                   "Фёдоров Виктор Николаевич",    "+7 (343) 123-45-67", "v.fedorov@rustrans.ru"),
        };

        // Физ. лица: INN 12 цифр, login = INN, password = INN
        var persons = new[]
        {
            Cust(C6, "772212345679", "Иванов Иван Иванович",           contactPerson: null,             "+7 (916) 111-22-33", "ivanov.ii@gmail.com"),
            Cust(C7, "771987654322", "Петрова Анна Сергеевна",         contactPerson: null,             "+7 (926) 222-33-44", "petrova.as@yandex.ru"),
            Cust(C8, "500187654321", "Сидоров Алексей Михайлович",     contactPerson: null,             "+7 (903) 333-44-55", "sidorov.am@mail.ru"),
            Cust(C9, "661212345678", "Козлова Мария Дмитриевна",       contactPerson: null,             "+7 (912) 444-55-66", "kozlova.md@gmail.com"),
        };

        await _db.Customers.AddRangeAsync(companies.Concat(persons), ct);

        foreach (var c in companies.Concat(persons))
        {
            _db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Login = c.INN,
                PasswordHash = _hasher.Hash(c.INN),
                Role = UserRole.Customer,
                CustomerId = c.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        _logger.LogInformation("Seeded 5 companies + 4 individual customers.");
    }

    private async Task SeedExecutorsAsync(CancellationToken ct)
    {
        if (await _db.Executors.AnyAsync(ct)) return;

        var executors = new[]
        {
            Exec(E1, "7712654321",   "ООО «АвтоСервис Профи»",       "г. Москва, ул. Автомобильная, д. 12, стр. 1",   "+7 (495) 111-22-33", "info@avtoservis-profi.ru"),
            Exec(E2, "5012876543",   "ООО «ТехМастер»",              "г. Химки, пр-т Мира, д. 45",                    "+7 (498) 222-33-44", "info@tekhmaster.ru"),
            Exec(E3, "772312345678", "ИП Петров Сергей Валерьевич",  "г. Москва, ул. Ленина, д. 78, оф. 3",           "+7 (916) 333-44-55", "petrov.auto@mail.ru"),
            Exec(E4, "6612345234",   "ООО «МоторТех»",               "г. Екатеринбург, ул. Промышленная, д. 23",      "+7 (343) 444-55-66", "info@motortech.ru"),
        };

        await _db.Executors.AddRangeAsync(executors, ct);

        foreach (var e in executors)
        {
            _db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Login = e.INN,
                PasswordHash = _hasher.Hash(e.INN),
                Role = UserRole.Executor,
                ExecutorId = e.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        _logger.LogInformation("Seeded 4 executors with users.");
    }

    private async Task SeedVehiclesAsync(CancellationToken ct)
    {
        if (await _db.Vehicles.AnyAsync(ct)) return;

        _db.Vehicles.AddRange(
            // ООО «Городской Транспорт» — городские автобусы
            Veh(V01, "А101АА77", "ПАЗ",          "3205-110",          2019, "XTM32050190001001", VehicleType.Bus,       C1),
            Veh(V02, "В202ВВ77", "ЛиАЗ",         "5292.67",           2020, "X1M52920L0A001002", VehicleType.Bus,       C1),
            Veh(V03, "К303КК77", "НефАЗ",         "5299-30-51",        2021, "X1M52993M0A001003", VehicleType.Bus,       C1),

            // АО «СпецТранс» — спецтехника
            Veh(V04, "С001СС77", "КамАЗ",         "65115-3960-48",     2018, "XTC651153J0000001", VehicleType.Special,   C2),
            Veh(V05, "Р002РР77", "МАЗ",           "5551A5-371",        2020, "Y3M5551A5L0000002", VehicleType.Special,   C2),
            Veh(V06, "Т003ТТ77", "Урал",          "4320-1951-60",      2017, "X1C432001H0000003", VehicleType.Special,   C2),

            // МУП «Городские Перевозки» — муниципальные автобусы
            Veh(V07, "М101МА50", "MAN",           "Lion's City G",     2022, "WMAN332ZXN5000101", VehicleType.Bus,       C3),
            Veh(V08, "М102МВ50", "МАЗ",           "203.C69",           2021, "Y3M203C69M0000102", VehicleType.Bus,       C3),
            Veh(V09, "М103МК50", "НефАЗ",         "5299-40-52",        2019, "X1M52994K0A000103", VehicleType.Bus,       C3),

            // ООО «Логистик Груп» — грузовые
            Veh(V10, "О201ОА50", "КамАЗ",         "5490-S5",           2022, "XTC54905N0B000201", VehicleType.Truck,     C4),
            Veh(V11, "О202ОВ50", "Volvo",         "FH 500",            2021, "YV2RT40A5MA000202", VehicleType.Truck,     C4),
            Veh(V12, "О203ОК50", "MAN",           "TGX 18.470",        2020, "WMAN324ZXL0000203", VehicleType.Truck,     C4),
            Veh(V13, "О204ОМ50", "Scania",        "R 500 LA4x2MNA",   2023, "YSMR4X20004000204", VehicleType.Truck,     C4),

            // ЗАО «РусТранс» — смешанный парк
            Veh(V14, "У301УА66", "Toyota",        "Camry 3.5",         2022, "XW7BF4FK60S300001", VehicleType.Passenger, C5),
            Veh(V15, "У302УВ66", "Mercedes-Benz", "Sprinter 519 CDI",  2021, "WDB9066321P300002", VehicleType.Truck,     C5),
            Veh(V16, "У303УК66", "Volkswagen",    "Crafter 50",        2020, "WV1ZZZ2EZLH300003", VehicleType.Special,   C5),

            // Физ. лица — легковые
            Veh(V17, "А777ОО77", "Lada",         "Vesta 1.8 MT",       2021, "XTA210900M0000017", VehicleType.Passenger, C6),
            Veh(V18, "В321РС77", "Hyundai",      "Solaris 1.6 AT",     2020, "Z94CT41CBLR000018", VehicleType.Passenger, C7),
            Veh(V19, "К456МН50", "Kia",          "Rio 1.6 AT",         2019, "XWEJC812AKA000019", VehicleType.Passenger, C8),
            Veh(V20, "Т789ОР66", "Volkswagen",   "Polo 1.6 MPI AT",   2022, "XW8ZZZ61ZNY000020", VehicleType.Passenger, C9)
        );

        _logger.LogInformation("Seeded 20 vehicles.");
    }

    private async Task SeedRepairsAsync(CancellationToken ct)
    {
        if (await _db.Repairs.AnyAsync(ct)) return;

        // Shorthand: R(vehicleId, executorId, repairTypeId, received, issued?, cost, mileage, status, comment?)
        _db.Repairs.AddRange(

            // ── ООО «Городской Транспорт» ─────────────────────────────────────

            // В01 — ПАЗ 3205
            R(V01, E2, RT1,  D(2024,  2, 15), D(2024,  2, 20),  12_500, 145_200, RepairStatus.Issued,      "Плановое ТО-1"),
            R(V01, E1, RT7,  D(2024,  7, 10), D(2024,  7, 16),  28_400, 162_800, RepairStatus.Issued,      "Замена тормозных колодок и дисков передней оси"),
            R(V01, E2, RT2,  D(2025,  1, 20), D(2025,  1, 28),  24_200, 182_500, RepairStatus.Issued,      "Плановое ТО-2 с заменой масла ДВС и трансмиссии"),

            // В02 — ЛиАЗ 5292
            R(V02, E1, RT6,  D(2024,  3,  5), D(2024,  3, 12),  45_800, 88_400,  RepairStatus.Issued,      "Замена передних амортизаторов и рычагов"),
            R(V02, E2, RT1,  D(2024,  9, 18), D(2024,  9, 25),  14_200, 108_300, RepairStatus.Issued,      "ТО-1 по регламенту"),
            R(V02, E1, RT8,  D(2025,  4,  5), null,             38_500,  121_000, RepairStatus.InProgress, "Замена генератора, диагностика бортовой сети"),

            // В03 — НефАЗ 5299
            R(V03, E4, RT4,  D(2024,  4, 22), D(2024,  5,  2), 185_000, 72_400,  RepairStatus.Issued,      "Ремонт ДВС: замена поршневой группы, вкладышей"),
            R(V03, E2, RT1,  D(2025,  3, 10), null,             13_500,  95_100,  RepairStatus.InProgress, "ТО-1 текущее обслуживание"),

            // ── АО «СпецТранс» ────────────────────────────────────────────────

            // В04 — КамАЗ 65115
            R(V04, E4, RT1,  D(2024,  1, 15), D(2024,  1, 22),  18_200, 123_500, RepairStatus.Issued,      "Плановое ТО-1"),
            R(V04, E4, RT5,  D(2024,  6,  8), D(2024,  6, 20),  95_600, 141_200, RepairStatus.Issued,      "Ремонт КПП ZF, замена синхронизаторов"),
            R(V04, E1, RT7,  D(2025,  2, 14), D(2025,  2, 21),  35_400, 159_800, RepairStatus.Issued,      "Замена тормозных барабанов задней оси"),

            // В05 — МАЗ 5551
            R(V05, E2, RT9,  D(2024,  5, 20), D(2024,  5, 28),  16_000, 67_300,  RepairStatus.Issued,      "Сезонная смена шин 315/80 R22.5"),
            R(V05, E1, RT6,  D(2024, 11,  3), D(2024, 11, 10),  52_300, 84_100,  RepairStatus.Issued,      "Замена реактивных штанг и сайлентблоков"),

            // В06 — Урал 4320
            R(V06, E1, RT3,  D(2024,  8, 15), D(2024,  8, 22),  78_200, 95_400,  RepairStatus.Issued,      "Ремонт кузова после ДТП, замена крыла и бампера"),
            R(V06, E4, RT2,  D(2025,  4,  1), null,             32_000, 108_200, RepairStatus.InProgress, "ТО-2: замена масла, фильтры, ремень ГРМ"),

            // ── МУП «Городские Перевозки» ─────────────────────────────────────

            // В07 — MAN Lion's City G
            R(V07, E2, RT1,  D(2024,  2, 28), D(2024,  3,  5),  22_000, 48_200,  RepairStatus.Issued,      "Плановое ТО-1"),
            R(V07, E1, RT8,  D(2024,  8, 10), D(2024,  8, 18),  38_400, 67_500,  RepairStatus.Issued,      "Замена АКБ, ремонт системы зарядки"),
            R(V07, E2, RT2,  D(2025,  1, 15), D(2025,  1, 23),  38_000, 85_300,  RepairStatus.Issued,      "ТО-2: замена масел, свечей накала, фильтров"),

            // В08 — МАЗ 203
            R(V08, E1, RT7,  D(2024,  4, 12), D(2024,  4, 20),  29_200, 55_800,  RepairStatus.Issued,      "Замена тормозных колодок всех осей"),
            R(V08, E2, RT1,  D(2024, 10, 25), D(2024, 11,  2),  21_000, 74_600,  RepairStatus.Issued,      "ТО-1 плановое"),

            // В09 — НефАЗ 5299 (C3)
            R(V09, E4, RT6,  D(2024,  6, 18), D(2024,  6, 28),  41_500, 62_400,  RepairStatus.Issued,      "Замена амортизаторов задней пневмоподвески"),
            R(V09, E2, RT9,  D(2025,  3, 20), null,             18_000, 81_200,  RepairStatus.Received,    "Смена сезонных шин"),

            // ── ООО «Логистик Груп» ───────────────────────────────────────────

            // В10 — КамАЗ 5490
            R(V10, E4, RT2,  D(2024,  1, 25), D(2024,  2,  5),  42_000, 187_400, RepairStatus.Issued,      "ТО-2 с заменой масла ДВС Cummins ISB"),
            R(V10, E4, RT5,  D(2024,  7, 22), D(2024,  8,  2), 112_400, 215_200, RepairStatus.Issued,      "Замена сцепления, ремонт коробки ZF AS-Tronic"),
            R(V10, E1, RT7,  D(2025,  2,  5), D(2025,  2, 15),  48_000, 242_800, RepairStatus.Issued,      "Замена тормозных дисков и суппортов"),

            // В11 — Volvo FH 500
            R(V11, E1, RT1,  D(2024,  3, 18), D(2024,  3, 25),  35_000, 145_600, RepairStatus.Issued,      "ТО-1 по регламенту Volvo"),
            R(V11, E3, RT8,  D(2024,  9,  5), D(2024,  9, 14),  45_800, 168_400, RepairStatus.Issued,      "Диагностика и ремонт электрики, замена датчиков"),

            // В12 — MAN TGX
            R(V12, E1, RT6,  D(2024,  5, 10), D(2024,  5, 20),  55_200, 122_500, RepairStatus.Issued,      "Замена рулевых тяг, регулировка развала-схождения"),
            R(V12, E4, RT2,  D(2024, 12,  8), D(2024, 12, 18),  44_000, 148_300, RepairStatus.Issued,      "ТО-2 с диагностикой системы EBS"),

            // В13 — Scania R 500
            R(V13, E1, RT1,  D(2024,  4,  5), D(2024,  4, 14),  38_000, 98_500,  RepairStatus.Issued,      "ТО-1 по регламенту Scania"),
            R(V13, E3, RT9,  D(2025,  1, 10), D(2025,  1, 20),  24_000, 122_300, RepairStatus.Issued,      "Замена комплекта шин 385/65 R22.5"),

            // ── ЗАО «РусТранс» ────────────────────────────────────────────────

            // В14 — Toyota Camry
            R(V14, E3, RT1,  D(2024,  4, 18), D(2024,  4, 22),   8_500, 45_200,  RepairStatus.Issued,      "ТО-1: масло, фильтры"),
            R(V14, E3, RT7,  D(2024, 10, 12), D(2024, 10, 16),  14_500, 58_400,  RepairStatus.Issued,      "Замена передних тормозных колодок и дисков"),

            // В15 — Mercedes Sprinter
            R(V15, E1, RT6,  D(2024,  2,  8), D(2024,  2, 15),  35_200, 88_600,  RepairStatus.Issued,      "Замена амортизаторов и пружин задней подвески"),
            R(V15, E1, RT3,  D(2024,  8, 22), D(2024,  8, 30),  65_000, 104_200, RepairStatus.Issued,      "Ремонт кузова: замена двери, покраска"),
            R(V15, E2, RT2,  D(2025,  3,  5), null,             28_000, 121_400, RepairStatus.Completed,   "ТО-2: выполнено, ожидает выдачи"),

            // В16 — VW Crafter
            R(V16, E3, RT10, D(2024,  5, 30), D(2024,  6,  6),  22_000, 73_200,  RepairStatus.Issued,      "Ремонт рефрижераторного оборудования"),
            R(V16, E3, RT8,  D(2024, 11, 18), D(2024, 11, 25),  18_500, 88_600,  RepairStatus.Issued,      "Замена форсунок, чистка впускного коллектора"),

            // ── Физ. лица ─────────────────────────────────────────────────────

            // В17 — Lada Vesta (Иванов)
            R(V17, E3, RT1,  D(2024,  3, 25), D(2024,  3, 28),   6_500, 32_000,  RepairStatus.Issued,      "ТО: замена масла и фильтров"),
            R(V17, E3, RT7,  D(2024,  9, 14), D(2024,  9, 17),   9_800, 48_500,  RepairStatus.Issued,      "Замена передних тормозных колодок"),
            R(V17, E2, RT9,  D(2025,  4, 10), null,               4_500, 55_200,  RepairStatus.Completed,   "Сезонная смена шин"),

            // В18 — Hyundai Solaris (Петрова)
            R(V18, E3, RT1,  D(2024,  6,  5), D(2024,  6,  9),   7_200, 28_400,  RepairStatus.Issued,      "ТО: масло 5W-30, фильтры"),
            R(V18, E3, RT6,  D(2024, 12, 20), D(2024, 12, 24),  21_500, 41_200,  RepairStatus.Issued,      "Замена передних стоек и опорных подшипников"),

            // В19 — Kia Rio (Сидоров)
            R(V19, E2, RT1,  D(2024,  7,  8), D(2024,  7, 12),   6_800, 55_400,  RepairStatus.Issued,      "ТО плановое"),
            R(V19, E1, RT3,  D(2025,  2, 25), D(2025,  3,  1),  38_200, 63_800,  RepairStatus.Issued,      "Ремонт кузова после парковочного инцидента"),

            // В20 — VW Polo (Козлова)
            R(V20, E3, RT9,  D(2024,  5, 15), D(2024,  5, 19),   5_500, 19_200,  RepairStatus.Issued,      "Смена летней резины"),
            R(V20, E4, RT4,  D(2024, 11,  8), D(2024, 11, 18),  78_000, 28_400,  RepairStatus.Issued,      "Замена ремня ГРМ, помпы, натяжителей"),
            R(V20, E3, RT1,  D(2025,  5, 20), null,               7_500, 38_100,  RepairStatus.Received,    "ТО плановое, ожидает приёмки"),

            // ══════════════════════════════════════════════════════════════════
            // 2026 — январь–июнь
            // ══════════════════════════════════════════════════════════════════

            // ── ООО «Городской Транспорт» ─────────────────────────────────────
            R(V01, E2, RT9,  D(2026,  1, 13), D(2026,  1, 15),  15_200, 193_400, RepairStatus.Issued,      "Сезонная смена шин на зимние"),
            R(V02, E1, RT7,  D(2026,  2, 10), D(2026,  2, 17),  31_500, 129_800, RepairStatus.Issued,      "Замена суппортов и тормозных шлангов"),
            R(V03, E2, RT2,  D(2026,  4,  7), D(2026,  4, 15),  26_000, 108_600, RepairStatus.Issued,      "ТО-2 по регламенту с дефектовкой"),
            R(V01, E4, RT6,  D(2026,  6,  3), null,             58_400, 202_100, RepairStatus.InProgress, "Замена задней рессорной подвески"),

            // ── АО «СпецТранс» ────────────────────────────────────────────────
            R(V04, E4, RT2,  D(2026,  1, 20), D(2026,  1, 29),  34_000, 170_500, RepairStatus.Issued,      "ТО-2: масла, фильтры, проверка тормозов"),
            R(V06, E1, RT8,  D(2026,  3, 12), D(2026,  3, 20),  44_800, 119_400, RepairStatus.Issued,      "Замена стартера и реле-регулятора"),
            R(V05, E2, RT1,  D(2026,  5, 19), D(2026,  5, 26),  17_500,  97_200, RepairStatus.Issued,      "ТО-1 плановое"),

            // ── МУП «Городские Перевозки» ─────────────────────────────────────
            R(V07, E1, RT6,  D(2026,  1,  8), D(2026,  1, 16),  48_200,  98_100, RepairStatus.Issued,      "Замена пневмобаллонов задней оси"),
            R(V09, E4, RT4,  D(2026,  2, 24), D(2026,  3,  7), 210_000,  91_800, RepairStatus.Issued,      "Капремонт ДВС: гильзы, поршни, вкладыши"),
            R(V08, E2, RT9,  D(2026,  5,  5), D(2026,  5,  9),  19_200,  86_300, RepairStatus.Issued,      "Сезонная смена шин на летние"),
            R(V07, E1, RT3,  D(2026,  6, 10), null,             72_000, 103_500, RepairStatus.InProgress, "Ремонт кузова: вмятины, замена стекла"),

            // ── ООО «Логистик Груп» ───────────────────────────────────────────
            R(V10, E4, RT1,  D(2026,  1, 15), D(2026,  1, 21),  40_500, 258_400, RepairStatus.Issued,      "ТО-1 по регламенту КамАЗ"),
            R(V13, E1, RT6,  D(2026,  2,  5), D(2026,  2, 14),  61_000, 135_800, RepairStatus.Issued,      "Замена рессор и стабилизатора поперечной устойчивости"),
            R(V11, E3, RT2,  D(2026,  3, 18), D(2026,  3, 26),  38_500, 182_400, RepairStatus.Issued,      "ТО-2 по регламенту Volvo"),
            R(V12, E4, RT5,  D(2026,  4, 22), D(2026,  5,  5), 128_000, 162_100, RepairStatus.Issued,      "Замена сцепления MAN TipMatic, регулировка"),
            R(V10, E1, RT3,  D(2026,  6,  2), null,             85_000, 265_200, RepairStatus.Completed,   "Ремонт кабины после аварии, покраска"),

            // ── ЗАО «РусТранс» ────────────────────────────────────────────────
            R(V15, E1, RT1,  D(2026,  1, 27), D(2026,  1, 31),  29_500, 132_600, RepairStatus.Issued,      "ТО-1 с заменой масла OM651"),
            R(V16, E3, RT7,  D(2026,  3, 24), D(2026,  3, 28),  22_000,  98_800, RepairStatus.Issued,      "Замена тормозных дисков и колодок"),
            R(V14, E3, RT9,  D(2026,  5, 13), D(2026,  5, 15),   6_800,  67_400, RepairStatus.Issued,      "Сезонная смена шин на летние"),

            // ── Физ. лица ─────────────────────────────────────────────────────
            R(V17, E3, RT1,  D(2026,  2, 18), D(2026,  2, 20),   7_200,  62_100, RepairStatus.Issued,      "ТО: масло, фильтры, тормозная жидкость"),
            R(V19, E2, RT9,  D(2026,  3, 10), D(2026,  3, 12),   5_800,  68_500, RepairStatus.Issued,      "Смена сезонных шин"),
            R(V18, E3, RT1,  D(2026,  4, 14), D(2026,  4, 16),   7_500,  48_200, RepairStatus.Issued,      "ТО плановое"),
            R(V20, E3, RT6,  D(2026,  5, 28), D(2026,  6,  3),  18_500,  44_700, RepairStatus.Issued,      "Замена передних стоек и опор"),
            R(V17, E2, RT7,  D(2026,  6, 16), null,             11_200,  67_300, RepairStatus.Received,    "Замена задних тормозных колодок")
        );

        _logger.LogInformation("Seeded 72 repairs (2024–2026).");
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static DateTime D(int y, int m, int d) => new(y, m, d, 9, 0, 0, DateTimeKind.Utc);

    private static RepairType RT(string name, string desc, Guid id) =>
        new() { Id = id, Name = name, Description = desc, IsActive = true };

    private static Customer Cust(Guid id, string inn, string name, string? contactPerson, string phone, string email) =>
        new() { Id = id, INN = inn, Name = name, ContactPerson = contactPerson, Phone = phone, Email = email, IsActive = true, CreatedAt = DateTime.UtcNow };

    private static Executor Exec(Guid id, string inn, string name, string address, string phone, string email) =>
        new() { Id = id, INN = inn, Name = name, Address = address, Phone = phone, Email = email, IsActive = true, CreatedAt = DateTime.UtcNow };

    private static Vehicle Veh(Guid id, string plate, string make, string model, int year, string vin, VehicleType type, Guid customerId) =>
        new() { Id = id, LicensePlate = plate, Make = make, Model = model, Year = year, VIN = vin, VehicleType = type, IsActive = true, CustomerId = customerId, CreatedAt = DateTime.UtcNow };

    private Repair R(Guid vehicleId, Guid executorId, Guid repairTypeId, DateTime received, DateTime? issued, decimal cost, int mileage, RepairStatus status, string? comment = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            ExecutorId = executorId,
            RepairTypeId = repairTypeId,
            ReceivedAt = received,
            IssuedAt = issued,
            Cost = cost,
            Mileage = mileage,
            Status = status,
            Comment = comment,
            CreatedByUserId = AdminId,
            CreatedAt = received,
            UpdatedAt = issued ?? received
        };
}
