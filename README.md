# 🏥 Rosheta - Prescription Management System

**Rosheta** is a Clean Architecture-based ASP.NET Core 9.0 application designed for healthcare professionals.

## 🏗️ Architecture

The solution follows strict **Clean Architecture** principles, split into three distinct projects:

| Project | Layer | Responsibility | Dependencies |
|---------|-------|----------------|--------------|
| **Rosheta.Core** | Domain & Application | Entities, Interfaces, DTOs, Business Logic | *None* |
| **Rosheta.Infrastructure** | Infrastructure | EF Core, Repositories, File Storage | `Rosheta.Core` |
| **Rosheta.Web** | Presentation | Razor Pages, Middleware, UI | `Rosheta.Infrastructure` |

## 🚀 Getting Started

1. **Prerequisites:** .NET 9.0 SDK.
2. **Database:** SQLite (Auto-created on first run).
3. **Run:**

   ```bash
   dotnet run --project Presentation/Rosheta.Web.csproj
   ```

## 📂 Project Structure

```text
/src
  ├── /Core            (Business Logic, Interfaces)
  ├── /Infrastructure  (Database, External Services)
  └── /Presentation    (Web UI, Razor Pages)
/docs                  (Architecture decisions & Roadmap)
/tests                 (Unit & Integration Tests)
```

## 📜 Documentation

- [Roadmap](ROADMAP.md)
- [Changelog](CHANGELOG.md)
- [Architecture Details](docs/README.md)

---
*Built with ❤️ using .NET 9 and Razor Pages.*
