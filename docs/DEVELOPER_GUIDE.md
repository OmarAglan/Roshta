# 👩‍💻 Rosheta Developer Guide

Welcome to the Rosheta development team! This guide covers everything you need to know to build, run, and extend the application following our **Clean Architecture** standards.

---

## 1. 🛠️ Development Environment

### Prerequisites

* **SDK:** [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (Required)
* **IDE:** Visual Studio 2022 (v17.8+) or VS Code (with C# Dev Kit).
* **Database:** SQLite (No installation required, runs in-process).

### Solution Setup

1. Clone the repository.
2. Open `Rosheta.sln`.
3. Restore dependencies:

    ```bash
    dotnet restore
    ```

---

## 2. 🏗️ Solution Anatomy

The solution is split into physical projects to enforce architectural boundaries.

| Project | Namespace | Layer | Responsibilities |
| :--- | :--- | :--- | :--- |
| **Rosheta.Core** | `Rosheta.Core` | Domain | **Pure Business Logic.** Entities, Interfaces, DTOs. <br/>*⛔ No external dependencies allowed.* |
| **Rosheta.Infrastructure** | `Rosheta.Infrastructure` | Infrastructure | **Implementation.** EF Core, File System, Email. <br/>*✅ Depends on Core.* |
| **Rosheta.Web** | `Rosheta.Web` | Presentation | **UI & Startup.** Razor Pages, Middleware. <br/>*✅ Depends on Core & Infrastructure.* |
| **Rosheta.UnitTests** | `Rosheta.UnitTests` | Testing | **Quality Assurance.** xUnit tests for Core. |

---

## 3. 🏃‍♂️ Running the Application

The entry point is **Rosheta.Web**.

**Via Visual Studio:**
Set `Rosheta.Web` as the **Startup Project** and press `F5`.

**Via CLI:**

```bash
dotnet run --project Presentation/Rosheta.Web.csproj
```

*First Run Note:* The application uses EF Core to automatically create the SQLite database (`roshta.db`) in the `Presentation` folder upon startup.

---

## 4. 💾 Database & Migrations

Because the `DbContext` lives in **Infrastructure** but the Configuration (`appsettings.json`) lives in **Web**, you must specify the paths explicitly when running EF commands.

### A. Create a New Migration

Run this after modifying an Entity in `Rosheta.Core`.

```bash
dotnet ef migrations add <MigrationName> \
    --project Infrastructure/Rosheta.Infrastructure.csproj \
    --startup-project Presentation/Rosheta.Web.csproj
```

### B. Update Database

Run this to apply pending migrations.

```bash
dotnet ef database update \
    --project Infrastructure/Rosheta.Infrastructure.csproj \
    --startup-project Presentation/Rosheta.Web.csproj
```

### C. Remove Last Migration

```bash
dotnet ef migrations remove \
    --project Infrastructure/Rosheta.Infrastructure.csproj \
    --startup-project Presentation/Rosheta.Web.csproj
```

---

## 5. 🧪 Testing Strategy

We follow the **Test Pyramid**. Currently, we focus on Unit Tests for the Core layer.

### Writing a Service Test

1. Go to `tests/Rosheta.UnitTests`.
2. Mirror the folder structure of Core (e.g., `Core/Application/Services`).
3. Use **Moq** to mock Repositories (`IDoctorRepository`).
4. Use **FluentAssertions** for checks.

**Example:**

```csharp
[Fact]
public async Task Create_ShouldFail_WhenNameIsEmpty() {
    // Arrange
    var mockRepo = new Mock<IDoctorRepository>();
    var service = new DoctorService(mockRepo.Object);
    
    // Act & Assert
    await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new Doctor()));
}
```

**Run Tests:**

```bash
dotnet test
```

---

## 6. 📝 Coding Standards

### Dependency Injection

* **Always** inject Interfaces (`IDoctorService`), never concrete classes.
* **Register** services in the layer they belong to:
  * Core services $\rightarrow$ `Core/DependencyInjection.cs`
  * Repositories $\rightarrow$ `Infrastructure/DependencyInjection.cs`

### Error Handling

* **Do not** return `null` to indicate failure.
* **Do not** return HTTP Status Codes from Services.
* **Throw** Domain Exceptions:
  * `ValidationException` (Invalid Input)
  * `NotFoundException` (ID not found)
  * `BusinessRuleException` (Logic violation)

### File Access

* **Never** use `System.IO` in `Rosheta.Core`.
* Inject `IFileStorageProvider` instead.

---

## 7. 🚀 Adding a New Feature Checklist

1. **Domain:** Add Entity to `Rosheta.Core/Domain/Entities`.
2. **Contract:** Add Interface `IRepository` to `Rosheta.Core/Application/Contracts/Persistence`.
3. **DTO:** Add Request/Response DTOs to `Rosheta.Core/Application/DTOs`.
4. **Service:** Implement logic in `Rosheta.Core/Application/Services`. **Write Tests.**
5. **Infrastructure:** Implement Repository in `Rosheta.Infrastructure/Data/Repositories`.
6. **Registration:** Register in `DependencyInjection.cs`.
7. **Migration:** Run `dotnet ef migrations add`.
8. **UI:** Create Razor Page in `Rosheta.Web/Pages`.
