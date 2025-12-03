# 📚 Rosheta Documentation Hub

Welcome to the technical documentation repository for the Rosheta project. This folder contains detailed architectural decisions, guides, and project tracking documents.

## 🚀 Essentials

Start here to get up and running.

* **[Developer Guide](DEVELOPER_GUIDE.md)**: ⭐️ **Must Read**. Setup instructions, solution structure, migration commands, and coding standards.
* **[Roadmap](../ROADMAP.md)**: High-level strategic goals and phases (located in root).
* **[Changelog](../CHANGELOG.md)**: Detailed history of changes and versions (located in root).
* **[Project Tasks](PROJECT_TASKS.md)**: Granular task tracking (Kanban style).

---

## 🏗️ Architecture & Design

Deep dives into why the system is built the way it is.

* **[Architecture Blueprint](architecture/ARCHITECTURE_PROJECT_ORGANIZATION.md)**: The reference document for the Clean Architecture implementation, Layer rules, and DTO organization.
* **[State Analysis](architecture/ARCHITECTURE_ANALYSIS.md)**: A report on the current health of the architecture, historical refactoring, and future recommendations.
* **[File Storage Design](architecture/ARCHITECTURE_FILE_STORAGE_ABSTRACTION.md)**: Details on the `IFileStorageProvider` abstraction for cross-platform support.

---

## 📂 Layer Documentation

Specific details for each project in the solution.

* **[Rosheta.Core](../src/Core/README.md)**: Domain rules, Services, and Interfaces.
* **[Rosheta.Infrastructure](../src/Infrastructure/README.md)**: Database, Repositories, and Adapters.
* **[Rosheta.Web](../src/Presentation/README.md)**: UI, Middleware, and Startup logic.
* **[Logging & Error Handling](../src/Infrastructure/Logging/README.md)**: Strategy for exception management.

---

*Documentation maintained by the Engineering Team.*
