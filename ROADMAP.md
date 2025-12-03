# 🏥 Rosheta Development Roadmap

**Project:** Rosheta - Prescription Management System
**Current Version:** 0.9.9.11
**Focus:** Phase 1.5 (Unit Testing & Stability)

---

## 🎯 Strategic Milestones

| Phase | Goal | Status |
| :--- | :--- | :--- |
| **Phase 1** | **Foundation & Architecture** | ✅ **COMPLETED** |
| **Phase 1.5** | **Quality Assurance (Testing)** | 🚧 **IN PROGRESS** |
| **Phase 2** | **Architectural Patterns** | 📅 **PLANNED** |
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

## 🚧 Phase 1.5: Quality Assurance (Current Focus)
**Goal:** Establish a safety net of automated tests to ensure reliability before adding complex features.

*   ✅ **Test Infrastructure:** Setup xUnit, Moq, and FluentAssertions.
*   ✅ **Critical Service Tests:** Covered `DoctorService`, `PatientService`, and `PrescriptionService`.
*   [ ] **Remaining Service Tests:** Cover `MedicationService`, `LicenseService`, and `SettingsService`.
*   [ ] **Repository Tests:** Setup InMemory/SQLite tests for Data Layer.
*   [ ] **Code Coverage:** Target >70% coverage for Core logic.

---

## 📅 Phase 2: Architectural Patterns (Next Up)
**Goal:** Eliminate boilerplate code and enforce consistency.

*   **Generic Repository Pattern:** Implement `IRepository<T>` to reduce repetitive data access code.
*   **Unit of Work:** Implement `IUnitOfWork` to handle transactions across multiple repositories.
*   **FluentValidation:** Separate validation rules from Entities/Services into dedicated Validators.
*   **Result Pattern:** Replace Exception-driven flow control with a functional `Result<T>` pattern.

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
*Last Updated: December 2025*