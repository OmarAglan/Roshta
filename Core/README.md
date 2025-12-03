# 🟢 Rosheta.Core

**Layer:** Domain & Application (The "Inner Circle")
**Type:** Class Library (.NET 9.0)
**Dependencies:** *None* (Strictly Enforced)

## 📖 Overview

This project contains the **Enterprise Business Rules** and **Application Business Rules**. It is the heart of the system and has no knowledge of the database, the file system, or the web user interface.

## 📂 Structure

### 1. Domain (`/Domain`)

Contains the pure entities and logic that represent the business concepts. These classes are "POCOs" (Plain Old CLR Objects).

* **`Entities/`**: Database-agnostic classes (`Doctor`, `Patient`, `Prescription`). They inherit from `AuditableEntity` or `BaseEntity`.
* **`Enums/`**: Strongly typed options (`PrescriptionStatus`).
* **`Base/`**: Shared abstractions (`BaseEntity`, `IAuditable`).

### 2. Application (`/Application`)

Contains the logic that orchestrates the domain entities to perform specific tasks.

* **`Contracts/`**: Interfaces defining *what* the system needs (e.g., `IDoctorRepository`, `IFileStorageProvider`). **The implementations live in Infrastructure.**
* **`Services/`**: The concrete implementation of business use cases (`DoctorService`, `PrescriptionService`). This is where the "thinking" happens.
* **`DTOs/`**: Data Transfer Objects, grouped by feature (e.g., `DTOs/Doctor/`), used to decouple the internal domain from the UI.
* **`Common/Exceptions/`**: Typed exceptions (`ValidationException`, `NotFoundException`) used to handle errors gracefully.

## 🛡️ Architectural Rules

1. **Dependency Rule:** This project must **never** reference `Rosheta.Infrastructure` or `Rosheta.Web`.
2. **No Infrastructure Leakage:** Do not reference `Microsoft.EntityFrameworkCore` (except for basic abstractions if absolutely necessary) or `System.Web`.
3. **Composition:** All services are registered via the `AddApplicationServices()` extension method in `DependencyInjection.cs`.
