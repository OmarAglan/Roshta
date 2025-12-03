# 🔵 Rosheta.Infrastructure

**Layer:** Infrastructure (Interface Adapters)
**Type:** Class Library (.NET 9.0)
**Dependencies:** `Rosheta.Core`, `Microsoft.EntityFrameworkCore.Sqlite`

## 📖 Overview

This project contains the **concrete implementations** of the interfaces defined in `Rosheta.Core`. It handles all "messy" external concerns like database connections, file I/O, and third-party integrations.

## 📂 Structure

### 1. Data (`/Data`)

Contains the persistence logic.

* **`ApplicationDbContext.cs`**: The EF Core Context. It maps the Domain Entities to SQLite tables.
* **`Repositories/`**: Implementation of Core interfaces (`DoctorRepository`, `PatientRepository`). These classes use the `DbContext` to fetch/save data.
* **`Migrations/`**: The history of database schema changes.

### 2. Storage (`/Storage`)

Contains file system logic.

* **`LocalFileStorageProvider.cs`**: Implements `IFileStorageProvider`. Uses `System.IO` to read/write files to the user's local application data folder.

### 3. DependencyInjection.cs

The `AddInfrastructureServices()` extension method. This is where the `DbContext` is configured and where Repositories are registered as Scoped services.

## 🛠️ Usage Guides

### Running Migrations

Because the `DbContext` lives here but the *Startup Project* is `Rosheta.Web`, you must specify paths explicitly:

**Create Migration:**

```bash
dotnet ef migrations add <Name> -p Infrastructure/Rosheta.Infrastructure.csproj -s Presentation/Rosheta.Web.csproj
```

**Update Database:**

```bash
dotnet ef database update -p Infrastructure/Rosheta.Infrastructure.csproj -s Presentation/Rosheta.Web.csproj
```

### Adding a New Repository

1. Define the Interface in `Rosheta.Core`.
2. Implement the class here in `Infrastructure/Data/Repositories`.
3. Register it in `DependencyInjection.cs`:

    ```csharp
    services.AddScoped<IMyRepository, MyRepository>();
    ```
