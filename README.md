# Учёт ремонтов ТС (VehicleRepair)

Система учёта ремонтов транспортных средств для управляющей компании (УК), которая координирует ремонт автопарка своих заказчиков силами сторонних исполнителей (автосервисов).

## Что делает проект

Три роли, три разных сценария использования одной и той же базы данных:

- **ManagementCompany (УК/менеджер)** — видит всё: дашборд с общей статистикой, список всех ремонтов, заказчиков, исполнителей, транспорт, отчёты (по заказчику/исполнителю/ремонтам/ТС с экспортом в Excel), управление пользователями и справочником видов ремонта.
- **Executor (исполнитель, автосервис)** — принимает ТС в ремонт, ведёт его по стадиям (`Принят → В работе → Завершён → Выдан`) и видит свою статистику (выручка, количество выполненных работ).
- **Customer (заказчик)** — видит только свои автомобили, активные ремонты и историю по каждому ТС, без доступа к чужим данным.

Разграничение доступа двухуровневое: роль определяет, какие операции вообще доступны (`[Authorize(Roles=...)]` на эндпоинтах), а привязка пользователя к конкретному заказчику/исполнителю (`LinkedEntityId`) определяет, какие именно записи он может видеть и менять — например, исполнитель не может взять в работу чужой ремонт, а заказчик не видит чужие машины.

Проект существует в двух клиентских формах поверх одного и того же REST API:
- **Веб-приложение** (React) — основной интерфейс для всех трёх ролей.
- **Десктоп-приложение** (Windows Forms) — упрощённый клиент для исполнителя: приём ТС в ремонт и проведение его по стадиям вплоть до выдачи.

## Технологический стек

| Слой | Технологии |
|---|---|
| Backend API | ASP.NET Core 8 (Clean Architecture: Domain / Application / Infrastructure / API), EF Core + Npgsql, JWT-аутентификация, Serilog |
| База данных | PostgreSQL 17 |
| Веб-фронтенд | React 19, TypeScript, Vite, Ant Design, Zustand, Axios, dayjs |
| Десктоп | .NET 8 Windows Forms |
| Тесты | xUnit, NSubstitute (Application-слой), присутствует отдельный проект Domain-тестов |

### Архитектура backend (Clean Architecture)

```
src/
  VehicleRepair.Domain/          # сущности, enum'ы (RepairStatus, UserRole, VehicleType), доменные исключения
  VehicleRepair.Application/     # use case'ы (по одному на действие), DTO, интерфейсы репозиториев
  VehicleRepair.Infrastructure/  # EF Core, миграции, репозитории, JWT, экспорт в Excel, сидинг БД
  VehicleRepair.API/             # ASP.NET Core контроллеры, middleware, DI-композиция
  VehicleRepair.Desktop/         # WinForms-клиент для исполнителя
tests/
  VehicleRepair.Application.Tests/
  VehicleRepair.Domain.Tests/
web/                             # React SPA
```

## Как развернуть проект

Есть два способа: через Docker (рекомендуется, наименьшая настройка хоста) и нативно (нужны локально установленные .NET SDK + Node.js + PostgreSQL).

### Способ 1 — Docker (рекомендуется)

Требуется только Docker (Engine или Desktop) — ни .NET, ни Node.js, ни PostgreSQL на хосте ставить не нужно, всё собирается внутри контейнеров.

```bash
git clone <repo-url>
cd vehicle-repair
docker compose up -d --build
```

Поднимутся три сервиса:
- `db` — PostgreSQL 17 (порт наружу не пробрасывается, доступен только внутри docker-сети)
- `api` — ASP.NET Core API, порт `5000` (Swagger отключён — включён только в `Development`)
- `web` — nginx с собранным React-приложением, порт `80`, проксирует `/api/*` на `api`

Приложение будет доступно на **http://localhost**. При первом старте `api` сам накатывает EF-миграции и засеивает демо-данные (заказчики, исполнители, ТС, ~70 ремонтов за 2024–2026 гг.).

Если у вас Windows без Docker Desktop — Docker Engine можно поставить прямо внутри WSL2 (без установки чего-либо в саму Windows):

```bash
wsl -d <ваш-дистрибутив> -u root -- bash -c "curl -fsSL https://get.docker.com | sh"
```

и дальше `docker compose up -d --build` выполнять внутри той же WSL-сессии (либо через `wsl -u root -- bash -c "cd /путь/к/проекту && docker compose up -d --build"`).

> ⚠️ **Важно про WSL2:** по умолчанию WSL2 гасит дистрибутив (а вместе с ним и Docker) сразу после завершения любой команды, если нет других активных сессий. Чтобы контейнеры не останавливались, держите дистрибутив «живым» отдельным фоновым процессом, например: `wsl -d <дистрибутив> -- sleep infinity`.

### Способ 2 — нативный запуск (без Docker)

Что понадобится установить на машину:

| Компонент | Версия | Зачем |
|---|---|---|
| [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0.x | сборка/запуск API и десктоп-клиента, `dotnet test` |
| [Node.js](https://nodejs.org/) | 20.x (LTS) | сборка и dev-сервер веб-фронтенда |
| [PostgreSQL](https://www.postgresql.org/download/) | 16–17 | база данных |

Порядок действий:

1. **PostgreSQL** — создать БД `vehicle_repair` с пользователем `postgres` (или любым другим — поменяв строку подключения на следующем шаге).

2. **API** — задать строку подключения и секреты в `src/VehicleRepair.API/appsettings.Development.json` (пример уже есть в файле, поменять `Password` под свою БД), затем:
   ```bash
   cd src/VehicleRepair.API
   dotnet run
   ```
   При старте автоматически применятся миграции и сидинг. По умолчанию слушает `http://localhost:5000`, Swagger — `http://localhost:5000/swagger`.

3. **Веб-фронтенд**:
   ```bash
   cd web
   npm install
   npm run dev
   ```
   Откроется на `http://localhost:5173`, обращается к API по адресу из `Cors:Origins`/`.env`.

4. **Десктоп-клиент** (Windows-only, WinForms):
   ```bash
   dotnet run --project src/VehicleRepair.Desktop/VehicleRepair.Desktop.csproj
   ```
   Клиент жёстко обращается к `http://localhost:5000/api/v1` (см. `Services/ApiClient.cs`), поэтому требует локально запущенный API именно на этом порту.

### Учётные записи по умолчанию (сид-данные)

| Роль | Логин | Пароль |
|---|---|---|
| ManagementCompany (админ) | `admin` | `Admin123!` |
| Executor | `7712654321` (ООО «АвтоСервис Профи») | `7712654321` |
| Executor | `5012876543` (ООО «ТехМастер») | `5012876543` |
| Customer | `7701342018` (ООО «Городской Транспорт») | `7701342018` |

Для юридических и физических лиц (заказчики/исполнители) логин и пароль всегда совпадают с их ИНН — полный список см. в `src/VehicleRepair.Infrastructure/Seed/DatabaseSeeder.cs`.

### Тесты

```bash
dotnet test VehicleRepair.sln
```

## Что я устанавливал на машине с нуля

Для запуска проекта на ПК (Windows, чистая машина без .NET/Node/PostgreSQL/Docker) были установлены:

1. **.NET 8 SDK** — нативно на Windows, через winget:
   ```powershell
   winget install --id Microsoft.DotNet.SDK.8
   ```
   Нужен для сборки и запуска десктоп-клиента (`VehicleRepair.Desktop`, WinForms, `net8.0-windows`) и для `dotnet test`.

2. **Docker Engine** — не на Windows, а внутри уже существовавшего на машине WSL2-дистрибутива (Ubuntu 24.04), через официальный скрипт:
   ```bash
   curl -fsSL https://get.docker.com | sh
   ```
   PostgreSQL и Node.js на хост **не устанавливались вообще** — они собираются и работают исключительно внутри Docker-образов (`postgres:17-alpine`, `node:20-alpine` на этапе сборки веб-фронтенда, `mcr.microsoft.com/dotnet/sdk:8.0`/`aspnet:8.0` — для API).

Ничего из этого не обязательно для работы самого проекта — это лишь то, чего не хватало на конкретной машине. На машине, где уже есть Docker (Windows/macOS/Linux — не важно), потребуется только `docker compose up -d --build`.
