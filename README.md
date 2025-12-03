# 🏥 Rosheta - Prescription Management System

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Platform](https://img.shields.io/badge/platform-.NET%209.0-blue)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Architecture](https://img.shields.io/badge/architecture-Clean-orange)](docs/architecture/ARCHITECTURE_PROJECT_ORGANIZATION.md)

**Rosheta** is a modern, cross-platform prescription management system designed for healthcare professionals. Built on **ASP.NET Core 9.0**, it adheres to strict **Clean Architecture** principles to ensure scalability, testability, and future support for Desktop/Mobile clients via .NET MAUI.

---

## 🚀 Quick Start

### Prerequisites

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
* Visual Studio 2022 or VS Code

### Running the Application

The solution is split into layers. The entry point is the **Web** project.

```bash
# Clone the repository
git clone https://github.com/YourUsername/rosheta.git
cd rosheta

# Restore dependencies
dotnet restore

# Run the Web Application
dotnet run --project Presentation/Rosheta.Web.csproj
```

*Note: The application uses SQLite. The database file (`roshta.db`) will be automatically created in the `Presentation` folder upon first run.*

---

## 🏗️ Architecture Structure

The solution follows the **Dependency Rule**, where dependencies flow inwards.

| Project | Path | Layer | Responsibility |
| :--- | :--- | :--- | :--- |
| **Rosheta.Core** | `src/Core` | **Domain** | Entities, Enums, Interfaces, Business Logic. **(No Dependencies)** |
| **Rosheta.Infrastructure** | `src/Infrastructure` | **Infrastructure** | Database (EF Core), File System, External Adapters. |
| **Rosheta.Web** | `src/Presentation` | **Presentation** | UI (Razor Pages), Middleware, Composition Root. |
| **Rosheta.UnitTests** | `tests/Rosheta.UnitTests` | **Testing** | xUnit tests for Core logic using Moq. |

For a deep dive, see **[Architecture Blueprint](docs/architecture/ARCHITECTURE_PROJECT_ORGANIZATION.md)**.

---

## 📚 Documentation

We maintain comprehensive documentation in the `docs/` folder:

* **👩‍💻 [Developer Guide](docs/DEVELOPER_GUIDE.md):** Best practices, how to run migrations, and coding standards.
* **🛣️ [Roadmap](ROADMAP.md):** Future plans (MAUI, API, etc.).
* **📋 [Task Board](docs/PROJECT_TASKS.md):** Detailed progress tracking.
* **📝 [Changelog](CHANGELOG.md):** Version history.

---

## ✨ Key Features (v0.9.9.11)

* **Clean Architecture:** Strict physical separation of concerns.
* **Patient Management:** CRUD operations with unique contact validation.
* **Prescription Engine:** Dynamic item addition, validation, and status tracking.
* **Search:** Live autocomplete for Patients and Medications.
* **Settings System:** JSON-based user preferences persistence.
* **Security:** Global Error Handling Middleware and Domain Exceptions.

---

## 🧪 Testing

We use **xUnit**, **Moq**, and **FluentAssertions**.

```bash
# Run all tests
dotnet test
```

---

## 🤝 Contributing

1. Read the **[Developer Guide](docs/DEVELOPER_GUIDE.md)**.
2. Ensure you adhere to the **Dependency Rule** (Core never references Infrastructure).
3. Write Unit Tests for new Services.

---

*Built with ❤️ using .NET 9*
