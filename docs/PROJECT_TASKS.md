# 📋 Project Task Tracking

**Current Focus:** Phase 3 (MAUI Preparation)
**Status:** Phase 2 Complete

---

## 🏗️ Phase 1: Foundation & Architecture (COMPLETE) ✅

**Goal:** Establish a clean, testable, portable architecture.

- [x] **T001: Architectural Clean-up**
  - [x] Remove PageModel dependencies from Repositories (DoctorRepo violation fixed).
  - [x] Fix circular dependencies between Web and Data layers.
- [x] **T002: Domain Abstractions**
  - [x] Create `BaseEntity` and `AuditableEntity`.
  - [x] Update all Models (`Doctor`, `Patient`, etc.) to inherit from Base.
  - [x] Implement generic Audit logic in `ApplicationDbContext`.
- [x] **T003: Infrastructure Abstraction**
  - [x] Create `IFileStorageProvider` interface in Core.
  - [x] Implement `LocalFileStorageProvider` in Infrastructure.
  - [x] Refactor `LicenseService` and `SettingsService` to use abstraction (removed direct `System.IO` usage).
- [x] **T003.5: Physical Project Split (Clean Architecture)**
  - [x] Create `Rosheta.Core` (Class Library - Domain/Application).
  - [x] Create `Rosheta.Infrastructure` (Class Library - Data/Storage).
  - [x] Create `Rosheta.Web` (ASP.NET Core App - Presentation).
  - [x] Move files to respective layers and fix namespaces.
  - [x] Implement Dependency Injection Extensions (`AddApplicationServices`, `AddInfrastructureServices`).
- [x] **T004: Global Error Handling**
  - [x] Create Domain Exceptions (`ValidationException`, `NotFoundException`, etc.).
  - [x] Implement `GlobalExceptionHandlerMiddleware`.
  - [x] Standardize Service layer to throw Domain Exceptions.

---

## 🧪 Phase 1.5: Quality Assurance (COMPLETE) ✅

**Goal:** Ensure core business logic is correct and regression-free via automated tests.

- [x] **T004.5: Test Infrastructure Setup**
  - [x] Create `Rosheta.UnitTests` project (xUnit).
  - [x] Configure Moq and FluentAssertions.
- [x] **T004.6: Service Layer Unit Tests**
  - [x] `DoctorService` (Validation logic).
  - [x] `PatientService` (Uniqueness checks).
  - [x] `PrescriptionService` (Complex creation logic).
  - [x] `MedicationService` (CRUD & Validation).
  - [x] `LicenseService` (File I/O mocking).
  - [x] `SettingsService` (Serialization mocking).
- [x] **T004.7: Repository Layer Tests**
  - [x] Setup Sqlite in-memory fixture.
  - [x] Test `PrescriptionRepository` includes, filtering, paging, sorting, counting, and cancel flow.
- [x] **T004.8: Code Coverage**
  - [x] Achieve >70% line coverage on `Rosheta.Core`.
  - [x] Add automated coverage gate script: `scripts/check-core-coverage.ps1`.

---

## 🎨 Phase 2: Architectural Patterns (COMPLETE) ✅

**Goal:** Remove code duplication and enforce consistency across the application.

- [x] **T005: Generic Repository Pattern**
  - [x] Define `IRepository<T>` interface.
  - [x] Implement `RepositoryBase<T>` in Infrastructure.
  - [x] Refactor repositories to inherit from base (`Doctor`, `Patient`, `Medication`, `Prescription`).
- [x] **T006: Unit of Work Pattern**
  - [x] Define `IUnitOfWork`.
  - [x] Implement `UnitOfWork` in Infrastructure (wrapping `DbContext`).
  - [x] Update Services to use UoW for transactional integrity.
- [x] **T007: FluentValidation**
  - [x] Install FluentValidation package in Core.
  - [x] Move validation logic from Services/Entities to Validator classes.
  - [x] Implement automatic validation pipeline in DI.
- [x] **T008: Result Pattern**
  - [x] Create `Result<T>` class (Success/Failure wrapper).
  - [x] Refactor Services to return `Result` for mutation/control-flow outcomes.

---

## 📱 Phase 3: MAUI Preparation (FUTURE) 🔮

**Goal:** Enable multi-platform support (Desktop/Mobile) reusing the Core logic.

- [ ] **T009: API Layer**
  - [ ] Create `Rosheta.Api` project (ASP.NET Core Web API).
  - [ ] Expose Core Services via REST Controllers.
- [ ] **T010: UI Separation (RCL)**
  - [ ] Create `Rosheta.UI.Shared` (Razor Class Library).
  - [ ] Extract reusable components (Cards, Grids, Forms) from Web project.
- [ ] **T011: MAUI Integration**
  - [ ] Create MAUI Blazor Hybrid Project.
  - [ ] Integrate `Rosheta.Core` and `Rosheta.Infrastructure`.
  - [ ] Implement `MauiFileStorageProvider`.
