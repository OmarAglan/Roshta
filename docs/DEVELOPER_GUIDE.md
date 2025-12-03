# 👩‍💻 Rosheta Developer Guide

## 1. Prerequisites

- **.NET 9.0 SDK**
- **Visual Studio 2022** (v17.8+) or **VS Code** (with C# Dev Kit)
- **PowerShell** (recommended for maintenance scripts)

## 2. Solution Structure

The solution strictly follows **Clean Architecture**:

| Project | Path | Layer | Purpose |
|---------|------|-------|---------|
| **Rosheta.Core** | `Core/` | Domain & Application | Pure business logic, Entities, Interfaces. **No external dependencies.** |
| **Rosheta.Infrastructure** | `Infrastructure/` | Infrastructure | Database (EF Core), File System, External APIs. Implements Core interfaces. |
| **Rosheta.Web** | `Presentation/` | Presentation | ASP.NET Core Razor Pages, Controllers, UI. The entry point. |
| **Rosheta.UnitTests** | `tests/Rosheta.UnitTests/` | Testing | xUnit tests focusing on Core logic (Services/Entities). |

## 3. Getting Started

### Build

```bash
dotnet build
```

### Run

The entry point is `Rosheta.Web` (located in the `Presentation` folder).

```bash
dotnet run --project Presentation/Rosheta.Web.csproj
```

*Note: The app will automatically create the SQLite database (`roshta.db`) in the `Presentation` folder on first run.*

### Run Tests

```bash
dotnet test
```

## 4. Database & Migrations

Since `DbContext` is in **Infrastructure** but the startup project is **Web**, you must specify projects explicitly when running EF commands.

**Create a Migration:**

```bash
dotnet ef migrations add <MigrationName> --project Infrastructure/Rosheta.Infrastructure.csproj --startup-project Presentation/Rosheta.Web.csproj
```

**Update Database:**

```bash
dotnet ef database update --project Infrastructure/Rosheta.Infrastructure.csproj --startup-project Presentation/Rosheta.Web.csproj
```

## 5. Coding Standards

- **Dependencies:** Always inject interfaces (e.g., `IDoctorRepository`), never concrete classes.
- **Core:** Never reference `Microsoft.EntityFrameworkCore` (except for specific abstractions if absolutely necessary) or `Infrastructure` namespaces in the Core project.
- **Testing:** Write unit tests for all new Service logic. Use `Moq` to mock repositories.
