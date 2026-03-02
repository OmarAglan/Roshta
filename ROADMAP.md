# 🏥 Rosheta Development Roadmap

**Project:** Rosheta - Prescription Management System
**Current Version:** 0.9.9.11
**Focus:** Phase 3 (MAUI Preparation)

---

## 🎯 Strategic Milestones

| Phase | Goal | Status |
| :--- | :--- | :--- |
| **Phase 1** | **Foundation & Architecture** | ✅ **COMPLETED** |
| **Phase 1.5** | **Quality Assurance (Testing)** | ✅ **COMPLETED** |
| **Phase 2** | **Architectural Patterns** | ✅ **COMPLETED** |
| **Phase 3** | **Multi-Platform (MAUI)** | 🔮 **FUTURE** |

---

## ✅ Phase 1: Foundation & Architecture (COMPLETED)
**Goal:** Transform the prototype into a professional, scalable Clean Architecture solution.

*   ✅ **Physical Project Split:** Separated into `Rosheta.Core`, `Rosheta.Infrastructure`, and `Rosheta.Web`.
*   ✅ **Dependency Injection:** Implemented Extension Method pattern for clean composition.
*   ✅ **Domain Abstraction:** Created `BaseEntity`, `IAuditable`, and pure Domain Models.
*   ✅ **Infrastructure Isolation:** Abstracted File System (`IFileStorageProvider`) and Database access.
*   ✅ **Global Error Handling:** Implemented Middleware and custom Domain Exceptions.
*   ✅ **Namespace Standardization:** Unified namespaces across the solution.

---

## ✅ Phase 1.5: Quality Assurance (Completed)
**Goal:** Establish a safety net of automated tests to ensure reliability before adding complex features.

*   ✅ **Test Infrastructure:** Setup xUnit, Moq, and FluentAssertions.
*   ✅ **Critical Service Tests:** Covered `DoctorService`, `PatientService`, and `PrescriptionService`.
*   ✅ **Service Tests:** Covered `DoctorService`, `PatientService`, `PrescriptionService`, `MedicationService`, `LicenseService`, and `SettingsService`.
*   ✅ **Repository Tests:** Added SQLite in-memory integration tests for `PrescriptionRepository` (includes, filtering, paging/sorting, count, cancel flow).
*   ✅ **Code Coverage:** `Rosheta.Core` line coverage exceeds 70% (enforced via `scripts/check-core-coverage.ps1`).

---

## ✅ Phase 2: Architectural Patterns (Completed)
**Goal:** Eliminate boilerplate code and enforce consistency.

*   ✅ **Generic Repository Pattern:** Completed (`IRepository<T>` + `RepositoryBase<T>` adopted by `Doctor`, `Patient`, `Medication`, and `Prescription` repositories).
*   ✅ **Unit of Work:** Implemented `IUnitOfWork` and `UnitOfWork` to centralize transactional `SaveChangesAsync`.
*   ✅ **FluentValidation:** Added dedicated validators and DI registration for automatic validation.
*   ✅ **Result Pattern:** Added `Result`/`Result<T>` and refactored service mutation flows to return structured outcomes.

---

## 🔮 Phase 3: Multi-Platform Expansion (MAUI)
**Goal:** enable the application to run as a Native Desktop app (Windows/macOS) using .NET MAUI Blazor Hybrid.

*   **Razor Class Library (RCL):** Extract UI components (`Rosheta.UI.Shared`) to share between Web and Desktop.
*   **MAUI Project:** Create `Rosheta.Maui` targeting Windows and Android/iOS.
*   **Platform Services:** Implement `MauiFileStorageProvider` and native printing support.
*   **Hybrid Bootstrapping:** Configure `MauiProgram.cs` to reuse `Rosheta.Core` and `Rosheta.Infrastructure`.

---

## 🔮 Phase 4: Advanced Features & Security
**Goal:** Enterprise-grade features and security hardening.

*   **API Layer:** Create `Rosheta.Api` for Client/Server deployment scenarios.
*   **Authentication/Identity:** Implement proper User/Role management (Identity Core).
*   **Reporting:** Advanced PDF generation for prescriptions and reports.
*   **Localization:** Support for Arabic/English switching.

---

## 📊 Version History Summary

*   **v0.9.9.11:** Clean Architecture Refactoring (Core/Infra/Web Split).
*   **v0.9.9.10:** UI Polish (Header/Footer/Themes).
*   **v0.9.9.9:** Advanced UI Features (Tables/Export).
*   **v0.9.9.8:** Settings & Configuration System.
*   **v0.9.9.7:** Live Search Implementation.

---
*Last Updated: March 2, 2026*
