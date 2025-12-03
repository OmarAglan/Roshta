# 📉 Error Handling & Logging Strategy

**Scope:** Cross-Cutting Concern
**Components:** Core (Exceptions), Web (Middleware), Infrastructure (Logger)

## 1. Overview

Rosheta implements a centralized, type-safe error handling system compliant with Clean Architecture. It avoids "try-catch" noise in business logic by using **Domain Exceptions** and **Global Middleware**.

---

## 2. Architecture Components

### A. Custom Exception Hierarchy (`Rosheta.Core`)

Located in `Core/Application/Common/Exceptions/`. These are "Business Logic" errors, not "System" errors.

| Exception Type | HTTP Status | Usage Scenario |
|----------------|-------------|----------------|
| **`ValidationException`** | 400 Bad Request | User input failed business rules (e.g., "Name required"). |
| **`NotFoundException`** | 404 Not Found | ID does not exist in DB (e.g., "Patient 99 not found"). |
| **`BusinessRuleException`** | 422 Unprocessable | Logic violation (e.g., "Cannot cancel filled prescription"). |
| **`InfrastructureException`** | 503 Unavailable | DB/File System failure (wrapped by Infrastructure layer). |

**Base Class:** `ApplicationException` (Inherits from `System.Exception`).

### B. Global Middleware (`Rosheta.Web`)

Located in `Presentation/Middleware/GlobalExceptionHandlerMiddleware.cs`.

* **Role:** Intercepts ALL exceptions thrown during a request.
* **Behavior:**
  * **API Requests:** Returns JSON `ProblemDetails` (RFC 7807).
  * **Page Requests:** Redirects to `/Error` page with a friendly message.
  * **Logging:** Logs the full stack trace and Request ID automatically.

---

## 3. Implementation Pattern

### Service Layer (`Rosheta.Core`)

Services **do not** catch exceptions unless they can fix them. They simply throw specific Domain Exceptions.

```csharp
public async Task<Patient> GetByIdAsync(int id)
{
    var patient = await _repo.GetByIdAsync(id);
    if (patient == null)
    {
        // 1. Throw Domain Exception
        throw new NotFoundException(nameof(Patient), id); 
    }
    return patient;
}
```

### Infrastructure Layer (`Rosheta.Infrastructure`)

Repositories catch low-level System Exceptions and wrap them if necessary, or let them bubble up.

```csharp
try 
{
    await File.WriteAllTextAsync(path, content);
}
catch (IOException ex)
{
    // 2. Wrap System Exception in Domain Exception
    throw new InfrastructureException("Disk write failed", ex);
}
```

---

## 4. Logging Configuration

Logging is configured in the **Presentation Layer** (`Rosheta.Web`).

### `appsettings.json` (Production)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Rosheta": "Information"
    }
  }
}
```

### `appsettings.Development.json` (Debug)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Rosheta.Core": "Debug",
      "Rosheta.Infrastructure": "Debug"
    }
  }
}
```

---

## 5. Troubleshooting Guide

### Issue: "Error Page shows generic message"

* **Cause:** The exception thrown was not one of the custom types (`ApplicationException`), so the middleware treated it as a generic 500 Server Error to prevent leaking sensitive info.
* **Fix:** Ensure your Service throws `ValidationException` or `BusinessRuleException` for known error states.

### Issue: "Logs are empty"

* **Cause:** Incorrect LogLevel in `appsettings.json`.
* **Fix:** Ensure `Rosheta` namespace is set to `Information` or `Debug`.

---

**Note:** In Phase 2, we plan to implement **Structured Logging** (Serilog) to export logs to files or external systems.
