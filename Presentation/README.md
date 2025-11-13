# Presentation Layer

This layer contains all **UI-related code** for the ASP.NET Core Razor Pages application.

## Structure

### Pages (`Presentation/Pages/`)
Contains **Razor Pages** (.cshtml) and their **PageModels** (.cshtml.cs).

**Organization:**
- Root pages (Index, Privacy, Activate, Error)
- **`DoctorProfile/`** - Doctor profile management pages
- **`Medications/`** - Medication CRUD pages
- **`Patients/`** - Patient CRUD pages
- **`Prescriptions/`** - Prescription management pages
- **`Shared/`** - Shared layouts and partial views

### Filters (`Presentation/Filters/`)
Contains **action filters** and **page filters**.

- **`ActivationCheckPageFilter.cs`** - Global filter for license activation check

### ViewModels (`Presentation/ViewModels/`)
Contains **view-specific models** (if needed for complex UI scenarios).

**Note:** Most DTOs are in `Core/Application/DTOs/` for reusability.

### wwwroot (`Presentation/wwwroot/`)
Contains **static files**.

- **`css/`** - Stylesheets
- **`js/`** - JavaScript files (live-search, validation, etc.)
- **`lib/`** - Client-side libraries (Bootstrap, jQuery, etc.)

## Key Responsibilities

1. **User Interface** - Render HTML pages using Razor syntax
2. **User Input** - Handle form submissions and validation
3. **Navigation** - Route handling and page flow
4. **Client-side Logic** - JavaScript for interactive features

## Dependencies

- **Depends on:** Core.Application layer (for services and DTOs)
- **Does NOT depend on:** Infrastructure layer (uses DI for services)

## Namespace Convention

Due to ASP.NET Core Razor Pages constraints with custom `RootDirectory`, namespaces use the following pattern:
- Root pages: `Rosheta.Pages`
- Feature pages: `Rosheta.Pages_{Feature}` (e.g., `Rosheta.Pages_Medications`)
- Filters: `Rosheta.Filters`

**Note:** The underscore naming is a Razor compiler requirement when using custom root directories.

## Important Notes

- **Razor Pages Configuration:** The `RootDirectory` is set to `/Presentation/Pages` in `Program.cs`
- **Static Files:** Served from `Presentation/wwwroot` via custom configuration
- **Filters:** Global filters are registered in `Program.cs`
- **View Imports:** Common namespaces are defined in `_ViewImports.cshtml`

## Future Considerations

When migrating to .NET MAUI:
- This layer will be replaced with MAUI views
- Business logic in PageModels should be minimal (delegate to services)
- Keep UI logic separate from application logic for easier migration