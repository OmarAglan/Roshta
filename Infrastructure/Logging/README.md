# Error Handling & Logging - Rosheta

## Overview

Rosheta implements a comprehensive, multi-layer error handling system following ASP.NET Core best practices. This document describes the error handling strategy, exception hierarchy, and logging configuration.

---

## Architecture

### 1. Custom Exception Hierarchy

Located in `Core/Application/Common/Exceptions/`, our custom exceptions provide type-safe error handling:

```
ApplicationException (abstract base)
├── ValidationException
├── NotFoundException
├── BusinessRuleException
└── InfrastructureException
```

#### Exception Types

**`ApplicationException`** (Base Class)
- Abstract base for all application-specific exceptions
- Inherits from `System.Exception`
- Located: [`Core/Application/Common/Exceptions/ApplicationException.cs`](../../Core/Application/Common/Exceptions/ApplicationException.cs)

**`ValidationException`**
- **When to use**: User input validation failures
- **HTTP Status**: 400 (Bad Request)
- **Example**:
  ```csharp
  if (string.IsNullOrWhiteSpace(patient.Name))
  {
      throw new ValidationException("Patient name is required.");
  }
  ```

**`NotFoundException`**
- **When to use**: Requested entity doesn't exist
- **HTTP Status**: 404 (Not Found)
- **Example**:
  ```csharp
  var patient = await _patientRepository.GetByIdAsync(id);
  if (patient == null)
  {
      throw new NotFoundException(nameof(Patient), id);
  }
  ```

**`BusinessRuleException`**
- **When to use**: Business logic violations
- **HTTP Status**: 422 (Unprocessable Entity)
- **Example**:
  ```csharp
  if (prescription.Status == PrescriptionStatus.Filled)
  {
      throw new BusinessRuleException("Cannot cancel a prescription that has been filled.");
  }
  ```

**`InfrastructureException`**
- **When to use**: Database, file system, or external service failures
- **HTTP Status**: 503 (Service Unavailable)
- **Example**:
  ```csharp
  try
  {
      return await _repository.GetAllAsync();
  }
  catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
  {
      throw new InfrastructureException("Failed to retrieve patients.", ex);
  }
  ```

---

## 2. Global Exception Handler Middleware

Located in [`Presentation/Middleware/GlobalExceptionHandlerMiddleware.cs`](../../Presentation/Middleware/GlobalExceptionHandlerMiddleware.cs)

### Features

- ✅ Centralized exception handling
- ✅ Automatic logging of all exceptions
- ✅ Different behavior for Development vs Production
- ✅ Separate handling for API vs Razor Pages
- ✅ User-friendly error messages
- ✅ Request ID tracking for troubleshooting

### Exception to HTTP Status Code Mapping

| Exception Type | HTTP Status | Title | Production Message |
|----------------|-------------|-------|-------------------|
| `ValidationException` | 400 | Validation Error | Exception message |
| `NotFoundException` | 404 | Resource Not Found | Exception message |
| `BusinessRuleException` | 422 | Business Rule Violation | Exception message |
| `InfrastructureException` | 503 | Service Unavailable | "A service error occurred. Please try again later." |
| Other | 500 | Internal Server Error | "An unexpected error occurred. Please try again later." |

### API vs Razor Pages

**For API Endpoints** (`/api/*`):
- Returns JSON Problem Details (RFC 7807 compliant)
- Includes: type, title, status, detail, instance, timestamp

**For Razor Pages**:
- Redirects to `/Error?statusCode={code}`
- Shows user-friendly error page
- Includes Request ID for support

---

## 3. Error Page

Enhanced error page at [`Presentation/Pages/Error.cshtml`](../../Presentation/Pages/Error.cshtml)

### Features

- Dynamic error messages based on status code
- Request ID display for troubleshooting
- Quick action buttons (Dashboard, Go Back)
- Status code-specific help (e.g., 404 shows common pages)
- Bootstrap-styled, responsive design

---

## 4. Service Layer Exception Handling

All services in `Core/Application/Services/` implement consistent error handling:

### Pattern

```csharp
public async Task<Entity> GetByIdAsync(int id)
{
    try
    {
        var entity = await _repository.GetByIdAsync(id);
        
        if (entity == null)
        {
            throw new NotFoundException(nameof(Entity), id);
        }
        
        return entity;
    }
    catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
    {
        throw new InfrastructureException("Failed to retrieve entity.", ex);
    }
}
```

### Key Points

1. **Validate inputs** → throw `ValidationException`
2. **Check entity existence** → throw `NotFoundException`
3. **Enforce business rules** → throw `BusinessRuleException`
4. **Wrap infrastructure errors** → throw `InfrastructureException`
5. **Let custom exceptions bubble up** → use `when (ex is not ApplicationException)`

---

## 5. Logging Configuration

### Production ([`appsettings.json`](../../appsettings.json))

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

### Development ([`appsettings.Development.json`](../../appsettings.Development.json))

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Rosheta": "Debug",
      "Rosheta.Presentation.Middleware": "Debug"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "[yyyy-MM-dd HH:mm:ss] "
    }
  }
}
```

### Log Levels by Namespace

| Namespace | Production | Development |
|-----------|-----------|-------------|
| `Default` | Information | Debug |
| `Rosheta.*` | Information | Debug |
| `Microsoft.AspNetCore` | Warning | Information |
| `Microsoft.EntityFrameworkCore` | - | Information |

---

## Best Practices

### ✅ DO

- **Use specific exception types** for different error scenarios
- **Include meaningful error messages** that help users understand what went wrong
- **Log exceptions at appropriate levels** (Error for unexpected, Warning for validation)
- **Provide Request IDs** for troubleshooting production issues
- **Hide sensitive information** in production error messages
- **Validate early** in service methods before touching infrastructure
- **Wrap infrastructure exceptions** to provide context

### ❌ DON'T

- **Don't catch exceptions without rethrowing or logging**
  ```csharp
  // BAD
  try { /* code */ } catch { }
  
  // GOOD
  try { /* code */ } catch (Exception ex) { _logger.LogError(ex, "..."); throw; }
  ```

- **Don't use generic `Exception` in throw statements**
  ```csharp
  // BAD
  throw new Exception("Something failed");
  
  // GOOD
  throw new ValidationException("Patient name is required.");
  ```

- **Don't expose sensitive data in error messages**
  ```csharp
  // BAD
  throw new Exception($"Database connection failed: {connectionString}");
  
  // GOOD
  throw new InfrastructureException("Database connection failed.");
  ```

- **Don't log sensitive information**
  - Passwords, license keys, personal data, connection strings

---

## Testing Error Scenarios

### 1. Trigger a 404 Error
Navigate to a non-existent page:
```
https://localhost:5001/NonExistentPage
```

### 2. Trigger a Validation Error
Try to create a patient with missing required fields:
```csharp
var patient = new Patient { Name = "", ContactInfo = "" };
await _patientService.AddPatientAsync(patient);
```

### 3. Trigger a Not Found Error
Try to get a non-existent entity:
```csharp
await _patientService.GetPatientByIdAsync(99999);
```

### 4. Trigger a Business Rule Error
Try to cancel an already-filled prescription:
```csharp
await _prescriptionService.CancelPrescriptionAsync(alreadyFilledId);
```

---

## Troubleshooting

### Error Page Not Showing

1. Check that middleware is registered in [`Program.cs`](../../Program.cs):
   ```csharp
   app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
   ```

2. Verify error page exists at `Presentation/Pages/Error.cshtml`

### Exceptions Not Being Caught

1. Ensure you're throwing custom exceptions (not `System.Exception`)
2. Check middleware order in `Program.cs`
3. Verify exception type matches one in the middleware's switch statement

### Logs Not Appearing

1. Check `appsettings.json` log levels
2. Ensure logger is injected in service constructor
3. Verify log level is appropriate (Debug, Info, Warning, Error)

---

## Future Enhancements

- [ ] Add structured logging with Serilog
- [ ] Implement log file rotation
- [ ] Add Application Insights integration
- [ ] Create error analytics dashboard
- [ ] Add custom error pages for specific status codes (401, 403)
- [ ] Implement retry policies for transient failures
- [ ] Add correlation IDs for distributed tracing

---

## References

- [ASP.NET Core Error Handling](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [Logging in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)
- [RFC 7807 - Problem Details](https://tools.ietf.org/html/rfc7807)
- [Exception Best Practices](https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)

---

## Support

For questions or issues related to error handling:
1. Check this documentation
2. Review the exception hierarchy in `Core/Application/Common/Exceptions/`
3. Examine middleware implementation in `Presentation/Middleware/`
4. Test with development environment for detailed error messages

**Last Updated**: 2025-01-13