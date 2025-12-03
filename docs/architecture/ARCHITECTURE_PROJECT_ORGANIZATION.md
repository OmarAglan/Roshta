# 🏗️ Rosheta Project Organization & Architecture Blueprint

**Status:** ✅ Phase 1 Implemented (v0.9.9.11)
**Scope:** Current Reference & Future Roadmap
**Last Updated:** December 2025

## 1. Executive Summary

This document serves as the **Master Blueprint** for the Rosheta solution. It details the implemented **Clean Architecture** (physical project separation), the strict rules governing dependencies, and the roadmap for future expansion into .NET MAUI (Desktop/Mobile).

The solution has moved from a Monolithic structure to a strict 3-Tier Clean Architecture to ensure testability, maintainability, and portability.

---

## 2. Reference Architecture Structure

The solution is physically divided into three projects (`Core`, `Infrastructure`, `Web`) plus a testing project.

### 2.1 The Big Picture

```text
Rosheta.sln
├── src/
│   ├── Rosheta.Core/            # (Class Library) The Inner Circle
│   │   ├── Domain/              # Pure business entities (Zero Dependencies)
│   │   ├── Application/         # Interfaces, Services, DTOs, Exceptions
│   │   └── DependencyInjection.cs
│   │
│   ├── Rosheta.Infrastructure/  # (Class Library) The Adapters
│   │   ├── Data/                # EF Core Context & Repositories
│   │   ├── Storage/             # File System Implementations
│   │   ├── Settings/            # Configuration Models
│   │   └── DependencyInjection.cs
│   │
│   └── Rosheta.Web/             # (ASP.NET Core App) The Entry Point
│       ├── Pages/               # Razor Pages (UI)
│       ├── Middleware/          # Global Exception Handling
│       └── Program.cs           # Composition Root
│
└── tests/
    └── Rosheta.UnitTests/       # xUnit Testing Project
        └── Core/                # Tests mirroring Core structure
```

---

## 3. Detailed Layer Breakdown

This section details the exact purpose and rules for every folder in the solution.

### 3.1 🟢 Rosheta.Core (The Domain Layer)

**Namespace:** `Rosheta.Core`
**Dependencies:** *None*. It depends on **zero** other projects.

#### `Domain/` (The Enterprise Business Rules)

Contains the heart of the business. These classes effectively ignore the existence of the database or the web.

* **`Entities/`**: Rich domain models (`Doctor`, `Patient`, `Prescription`). They inherit from `AuditableEntity`.
* **`Enums/`**: Domain-specific enumerations (`PrescriptionStatus`).
* **`Base/`**: Abstract base classes (`BaseEntity`, `AuditableEntity`, `IAuditable`).
* **`Constants/`**: System-wide business constants (e.g., maximum prescription duration).

#### `Application/` (The Application Business Rules)

Orchestrates the domain objects to perform specific tasks.

* **`Contracts/`**: The "Ports" of the application.
  * **`Persistence/`**: Interfaces for data access (`IDoctorRepository`, `IPrescriptionRepository`).
  * **`Services/`**: Interfaces for business logic (`IDoctorService`, `ILicenseService`).
  * **`Infrastructure/`**: Interfaces for external tools (`IFileStorageProvider`, `IEmailService`).
* **`Services/`**: Concrete implementations of the service interfaces (`DoctorService`). These contain the actual "Thinking" code.
* **`DTOs/`**: Data Transfer Objects grouped by feature.
  * `DTOs/Doctor/` (`UpdateDoctorProfileDto`)
  * `DTOs/Patient/` (`PatientSearchDto`)
* **`Common/Exceptions/`**: Domain-specific exceptions (`ValidationException`, `NotFoundException`).

**❌ FORBIDDEN in Core:**

* Referencing `Microsoft.EntityFrameworkCore` (except strictly necessary abstractions).
* Referencing `System.Web` or `Microsoft.AspNetCore`.
* Referencing the `Infrastructure` project.

---

### 3.2 🔵 Rosheta.Infrastructure (The Interface Adapters)

**Namespace:** `Rosheta.Infrastructure`
**Dependencies:** `Rosheta.Core`, `Microsoft.EntityFrameworkCore`, `Microsoft.Extensions.Configuration`.

#### `Data/` (Persistence)

* **`ApplicationDbContext.cs`**: The EF Core context. It maps Domain Entities to the Database.
* **`Repositories/`**: Implementation of `Contracts.Persistence`.
  * `DoctorRepository`: Implements `IDoctorRepository`. Uses `ApplicationDbContext` to fetch/save data.
* **`Migrations/`**: Database schema history (SQLite migrations).

#### `Storage/` (File System)

* **`LocalFileStorageProvider.cs`**: Implementation of `IFileStorageProvider`. Uses `System.IO` to read/write files to `%LOCALAPPDATA%`.

#### `DependencyInjection.cs`

An extension method `AddInfrastructureServices()` that encapsulates the registration of the DbContext and Repositories. This keeps `Program.cs` clean.

**✅ ALLOWED in Infrastructure:**

* Database-specific logic (SQL, EF Core).
* File system access.
* External API calls (HTTP Client).

---

### 3.3 🟠 Rosheta.Web (The Presentation Layer)

**Namespace:** `Rosheta.Web` (and `Rosheta.Pages` for Razor Pages)
**Dependencies:** `Rosheta.Core`, `Rosheta.Infrastructure`.

#### `Pages/` (UI)

Razor Pages organized by feature folders.

* `Pages/DoctorProfile/`: `Edit.cshtml`, `Setup.cshtml`.
* `Pages/Patients/`: CRUD pages for patients.

#### `Middleware/` (Pipeline)

* `GlobalExceptionHandlerMiddleware.cs`: Catches Domain Exceptions (like `ValidationException`) and converts them to user-friendly HTTP responses or Error Pages.

#### `Program.cs` (Composition Root)

The only place where all three layers meet. It wires up the Dependency Injection container:

```csharp
builder.Services.AddApplicationServices();       // From Core
builder.Services.AddInfrastructureServices(...); // From Infrastructure
```

---

## 4. Architecture Decision Records (ADRs)

### ADR-001: Physical Project Separation

**Context:** The application was a Monolith. Logic was leaking between layers.
**Decision:** Split into 3 physical `.csproj` projects.
**Consequences:**

* (+) Impossible to accidentally reference Infrastructure from Core.
* (+) Unit tests run faster and are purer.
* (-) Slightly more complex solution structure.

### ADR-002: Feature-Based Organization for DTOs

**Context:** DTOs were scattered or grouped by type.
**Decision:** Group DTOs by Feature (e.g., `DTOs/Doctor/`).
**Consequences:**

* (+) Better cohesion. All files related to "Doctor" are nearby.
* (+) Easier to navigate for new developers.

### ADR-003: Infrastructure-Centric DI Registration

**Context:** `Program.cs` was huge and knew too much about implementation details.
**Decision:** Use `DependencyInjection.cs` extension methods in each layer.
**Consequences:**

* (+) Web layer doesn't know about specific Repository implementations.
* (+) Easy to swap Infrastructure (e.g., SQLite -> SQL Server) by changing one line.

---

## 5. Future Roadmap & Scalability

This architecture is designed to support the following future phases defined in the Roadmap.

### Phase 2: Architectural Patterns (Refinement)

* **Generic Repository:** To reduce code duplication in `Infrastructure/Data/Repositories`.
  * *Plan:* Create `RepositoryBase<T>` implementing `IRepository<T>` in Infrastructure.
* **Unit of Work:** To ensure atomic transactions across multiple repositories.
  * *Plan:* Implement `IUnitOfWork` wrapping the `DbContext.SaveChanges()`.
* **FluentValidation:** To move validation logic out of Controllers/Services.
  * *Plan:* Add `FluentValidation` package to `Core` and scan for validators in `DependencyInjection.cs`.

### Phase 3: MAUI Migration (Multi-UI Support)

The critical benefit of this architecture is **Portability**.

**Target Structure for Phase 3:**

```text
src/
├── Rosheta.Core            (Shared - No changes needed)
├── Rosheta.Infrastructure  (Shared - No changes needed)
├── Rosheta.Web             (Existing Web UI)
└── Rosheta.Maui            (NEW: Desktop/Mobile UI)
```

**Strategy:**

1. Create `Rosheta.Maui` project.
2. Add reference to `Rosheta.Core` and `Rosheta.Infrastructure`.
3. Call `AddApplicationServices()` and `AddInfrastructureServices()` in `MauiProgram.cs`.
4. **Storage:** The `LocalFileStorageProvider` in Infrastructure is already compatible with Windows Desktop. For Android/iOS, we may implement `MauiFileStorageProvider` if sandboxing rules differ.

### Phase 4: API Layer (Optional/Hybrid)

If we move to a Client-Server model (where the DB is on a server, not local):

1. Create `Rosheta.Api` (ASP.NET Core Web API).
2. Reference Core/Infrastructure.
3. Expose Controllers that call `IDoctorService`, etc.
4. `Rosheta.Maui` then calls this API instead of using `Rosheta.Infrastructure` directly.

---

## 6. Developer Guidelines

### Adding a New Feature (e.g., Appointments)

1. **Core (Domain):** Create `Appointment` entity in `Domain/Entities`.
2. **Core (Contracts):** Define `IAppointmentRepository` in `Application/Contracts/Persistence` and `IAppointmentService` in `Application/Contracts/Services`.
3. **Core (DTOs):** Create `CreateAppointmentDto` in `Application/DTOs/Appointments`.
4. **Core (Service):** Implement `AppointmentService` in `Application/Services`. **Write Unit Tests.**
5. **Infrastructure:** Implement `AppointmentRepository` in `Infrastructure/Data/Repositories`. Add `DbSet<Appointment>` to `ApplicationDbContext`.
6. **Infrastructure (Migrations):** Run `dotnet ef migrations add AddAppointments`.
7. **Web:** Create Razor Pages in `Pages/Appointments`. Inject `IAppointmentService`.

### Testing

* **Unit Tests:** Must target `Rosheta.Core`. Mock all interfaces.
* **Integration Tests:** (Future) Will target `Rosheta.Web` or `Rosheta.Api` using `WebApplicationFactory`.

---

## 7. Migration History (Archived)

* **Original State:** Monolithic `Rosheta.csproj`.
* **Migration Date:** June 2025.
* **Actions Taken:**
    1. Standardized Namespaces.
    2. Created Class Libraries (`Core`, `Infrastructure`).
    3. Moved files to respect Dependency Rule.
    4. Refactored `Program.cs` to use clean DI registration.
    5. Abstracted File System.

---

**Document Version:** 2.1 (Comprehensive Reference)
**Last Updated:** December 3, 2025
