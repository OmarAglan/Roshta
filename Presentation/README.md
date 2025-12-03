# 🟠 Rosheta.Web

**Layer:** Presentation (User Interface)
**Type:** ASP.NET Core 9.0 Web Application
**Dependencies:** `Rosheta.Core`, `Rosheta.Infrastructure`

## 📖 Overview

This project is the **Entry Point** and **Composition Root** of the application. It contains the UI logic (Razor Pages), static assets (CSS/JS), and the application startup configuration. It is responsible for interacting with the user and delegating business logic to the Core layer.

## 📂 Structure

### 1. Root Components

* **`Program.cs`**: The application entry point. It wires together the `Core` and `Infrastructure` layers using their dependency injection extensions.
* **`appsettings.json`**: Configuration file (Connection strings, Logging, License settings).

### 2. Pages (`/Pages`)

Contains the Razor Pages (`.cshtml` and `.cshtml.cs`). Organized by feature:

* **`DoctorProfile/`**: Setup and Edit profile logic.
* **`Patients/`**: CRUD operations for patients.
* **`Prescriptions/`**: Prescription management.
* **`Shared/`**: Layouts (`_Layout.cshtml`) and partial views.

### 3. Middleware (`/Middleware`)

* **`GlobalExceptionHandlerMiddleware.cs`**: Catches specific Domain Exceptions thrown by the Core layer (e.g., `ValidationException`) and converts them into user-friendly responses or error pages.

### 4. Filters (`/Filters`)

* **`ActivationCheckPageFilter.cs`**: Enforces license activation rules across the application.

### 5. Static Assets (`/wwwroot`)

Standard ASP.NET Core static files:

* `css/`: Stylesheets (Bootstrap, Custom site.css).
* `js/`: JavaScript logic (Validation helpers, Live Search, Advanced Tables).
* `lib/`: Third-party libraries.

## 🛡️ Architectural Rules

1. **Passive View:** The PageModels should contain minimal business logic. They should validate input, call a Service in `Rosheta.Core`, and render the result.
2. **No Direct Data Access:** Razor Pages must **never** inject `DbContext` or Repositories directly. They must use **Services** (e.g., `IDoctorService`).
3. **Composition Root:** `Program.cs` is the only place allowed to know about all layers to wire them together.

## 🚀 How to Run

This is the startup project.

```bash
dotnet run --project Presentation/Rosheta.Web.csproj
```
