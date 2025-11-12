# Rosheta Project Organization & Architecture Optimization

## Executive Summary

This document outlines a comprehensive reorganization of the Rosheta project structure to align with Clean Architecture principles, prepare for MAUI migration (Phase 3), and establish sustainable patterns for long-term growth.

### Key Issues Identified

1. **Namespace Inconsistency**: Mixed use of `Roshta` and `Rosheta` namespaces
2. **Layer Violations**: Razor Pages directly reference repositories instead of only services
3. **Mixed Concerns**: ViewModels folder contains DTOs, search models, and validation models
4. **Missing Organization**: No clear place for validators, mappers, constants, or enums
5. **Future Scalability**: Current structure will be difficult to extract into class libraries for MAUI

## Current vs. Proposed Architecture

### Current Structure (Issues)
```
Rosheta/
├── Data/                          # ❌ Just DbContext, no configurations
├── Filters/                       # ❌ Mixed with business logic
├── Infrastructure/Storage/        # ✅ Good start, needs expansion
├── Models/                        # ❌ Mixed domain and base entities
├── Pages/                         # ✅ Well organized by feature
├── Repositories/                  # ❌ Not organized by domain
├── Services/                      # ❌ Not organized by domain
├── Settings/                      # ❌ Mixed with configuration models
├── ViewModels/                    # ❌ Catch-all for various DTOs
└── wwwroot/                       # ✅ Static assets
```

### Proposed Clean Architecture Structure
```
Rosheta/
├── Core/                                    # Domain + Application Layers
│   ├── Domain/                             # Pure business entities (no dependencies)
│   │   ├── Entities/
│   │   │   ├── Doctor.cs
│   │   │   ├── Patient.cs
│   │   │   ├── Prescription.cs
│   │   │   ├── PrescriptionItem.cs
│   │   │   └── Medication.cs
│   │   ├── Enums/
│   │   │   └── PrescriptionStatus.cs
│   │   ├── Base/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── AuditableEntity.cs
│   │   │   └── IAuditable.cs
│   │   └── Constants/
│   │       └── BusinessConstants.cs
│   │
│   ├── Application/                        # Business logic layer
│   │   ├── Contracts/                      # All interfaces
│   │   │   ├── Persistence/                # Repository contracts
│   │   │   │   ├── IDoctorRepository.cs
│   │   │   │   ├── IPatientRepository.cs
│   │   │   │   ├── IPrescriptionRepository.cs
│   │   │   │   └── IMedicationRepository.cs
│   │   │   ├── Services/                   # Service contracts
│   │   │   │   ├── IDoctorService.cs
│   │   │   │   ├── IPatientService.cs
│   │   │   │   ├── IPrescriptionService.cs
│   │   │   │   ├── IMedicationService.cs
│   │   │   │   ├── ILicenseService.cs
│   │   │   │   └── ISettingsService.cs
│   │   │   └── Infrastructure/             # Infrastructure contracts
│   │   │       └── IFileStorageProvider.cs
│   │   │
│   │   ├── Services/                       # Service implementations
│   │   │   ├── DoctorService.cs
│   │   │   ├── PatientService.cs
│   │   │   ├── PrescriptionService.cs
│   │   │   ├── MedicationService.cs
│   │   │   ├── LicenseService.cs
│   │   │   └── SettingsService.cs
│   │   │
│   │   ├── DTOs/                           # Data Transfer Objects
│   │   │   ├── Doctor/
│   │   │   │   ├── DoctorDto.cs
│   │   │   │   ├── UpdateDoctorProfileDto.cs
│   │   │   │   └── DoctorSetupDto.cs
│   │   │   ├── Patient/
│   │   │   │   ├── PatientDto.cs
│   │   │   │   ├── PatientSearchDto.cs
│   │   │   │   └── CreatePatientDto.cs
│   │   │   ├── Prescription/
│   │   │   │   ├── PrescriptionDto.cs
│   │   │   │   ├── PrescriptionSearchDto.cs
│   │   │   │   └── PrescriptionDetailDto.cs
│   │   │   └── Medication/
│   │   │       ├── MedicationDto.cs
│   │   │       └── MedicationSearchDto.cs
│   │   │
│   │   ├── Models/                         # Application-specific models
│   │   │   ├── PagedResult.cs
│   │   │   ├── Result.cs
│   │   │   └── UserSettingsModel.cs
│   │   │
│   │   ├── Validators/                     # Business validation (future)
│   │   │   ├── DoctorValidators/
│   │   │   ├── PatientValidators/
│   │   │   └── PrescriptionValidators/
│   │   │
│   │   └── Common/
│   │       ├── Exceptions/
│   │       │   ├── BusinessException.cs
│   │       │   └── ValidationException.cs
│   │       └── Mappings/                   # Future: AutoMapper profiles
│   │
│   └── README.md
│
├── Infrastructure/                          # External dependencies layer
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/                 # EF entity configurations
│   │   │   ├── DoctorConfiguration.cs
│   │   │   ├── PatientConfiguration.cs
│   │   │   └── PrescriptionConfiguration.cs
│   │   └── Repositories/
│   │       ├── DoctorRepository.cs
│   │       ├── PatientRepository.cs
│   │       ├── PrescriptionRepository.cs
│   │       └── MedicationRepository.cs
│   │
│   ├── Storage/
│   │   └── LocalFileStorageProvider.cs
│   │
│   ├── Settings/
│   │   └── LicenseSettings.cs
│   │
│   └── README.md
│
├── Presentation/                            # UI Layer
│   ├── Pages/
│   │   ├── DoctorProfile/
│   │   ├── Medications/
│   │   ├── Patients/
│   │   ├── Prescriptions/
│   │   ├── Shared/
│   │   ├── _ViewImports.cshtml
│   │   ├── _ViewStart.cshtml
│   │   ├── Activate.cshtml(.cs)
│   │   ├── Error.cshtml(.cs)
│   │   ├── Index.cshtml(.cs)
│   │   └── Privacy.cshtml(.cs)
│   │
│   ├── Filters/
│   │   └── ActivationCheckPageFilter.cs
│   │
│   ├── ViewModels/                         # Page-specific view models
│   │   ├── Prescription/
│   │   │   └── PrescriptionCreateModel.cs
│   │   └── Common/
│   │       └── SearchViewModel.cs
│   │
│   └── wwwroot/
│       ├── css/
│       ├── js/
│       └── lib/
│
├── Migrations/                              # Keep at root (EF requirement)
├── Properties/
├── Program.cs
├── appsettings.json
└── Rosheta.csproj
```

## Detailed Migration Plan

### Phase 1: Namespace Unification (Critical - Do First)

**Why**: Inconsistent namespaces cause confusion and compilation issues.

**Actions**:
1. Standardize all namespaces to `Rosheta` (not `Roshta`)
2. Update all `using` statements
3. Update project file if needed

**Files to Update** (All files):
- All `.cs` files currently using `Roshta` namespace
- Update `Roshta.csproj` to `Rosheta.csproj` if needed

**Script to Help**:
```powershell
# Find all files with Roshta namespace
Get-ChildItem -Recurse -Filter "*.cs" | Select-String "namespace Roshta" -List
Get-ChildItem -Recurse -Filter "*.cs" | Select-String "using Roshta" -List
```

### Phase 2: Create Core Layer Structure

**Why**: Establishes clean domain boundaries and prepares for class library extraction.

#### 2.1 Domain Layer Organization

**Create Structure**:
```
mkdir Core
mkdir Core\Domain
mkdir Core\Domain\Entities
mkdir Core\Domain\Base
mkdir Core\Domain\Enums
mkdir Core\Domain\Constants
```

**File Movements**:
| From | To | Namespace Change |
|------|-----|-----------------|
| `Models/Entities/*.cs` | `Core/Domain/Entities/` | `Rosheta.Models.Entities` → `Rosheta.Core.Domain.Entities` |
| `Models/Base/*.cs` | `Core/Domain/Base/` | `Rosheta.Models.Base` → `Rosheta.Core.Domain.Base` |
| Extract `PrescriptionStatus` enum | `Core/Domain/Enums/PrescriptionStatus.cs` | New: `Rosheta.Core.Domain.Enums` |

#### 2.2 Application Layer Organization

**Create Structure**:
```
mkdir Core\Application
mkdir Core\Application\Contracts
mkdir Core\Application\Contracts\Persistence
mkdir Core\Application\Contracts\Services
mkdir Core\Application\Contracts\Infrastructure
mkdir Core\Application\Services
mkdir Core\Application\DTOs
mkdir Core\Application\Models
mkdir Core\Application\Common
```

**File Movements**:
| From | To | Namespace Change |
|------|-----|-----------------|
| `Services/Interfaces/*.cs` | `Core/Application/Contracts/Services/` | `Rosheta.Services.Interfaces` → `Rosheta.Core.Application.Contracts.Services` |
| `Repositories/Interfaces/*.cs` | `Core/Application/Contracts/Persistence/` | `Rosheta.Repositories.Interfaces` → `Rosheta.Core.Application.Contracts.Persistence` |
| `Infrastructure/Storage/Interfaces/*.cs` | `Core/Application/Contracts/Infrastructure/` | `Rosheta.Infrastructure.Storage.Interfaces` → `Rosheta.Core.Application.Contracts.Infrastructure` |
| `Services/*.cs` | `Core/Application/Services/` | `Rosheta.Services` → `Rosheta.Core.Application.Services` |
| `ViewModels/*SearchDto.cs` | `Core/Application/DTOs/{Feature}/` | `Rosheta.ViewModels` → `Rosheta.Core.Application.DTOs.{Feature}` |
| `ViewModels/UpdateDoctorProfileDto.cs` | `Core/Application/DTOs/Doctor/` | `Rosheta.ViewModels` → `Rosheta.Core.Application.DTOs.Doctor` |
| `ViewModels/UserSettingsModel.cs` | `Core/Application/Models/` | `Rosheta.ViewModels` → `Rosheta.Core.Application.Models` |

### Phase 3: Create Infrastructure Layer

**Create Structure**:
```
mkdir Infrastructure\Data
mkdir Infrastructure\Data\Configurations
mkdir Infrastructure\Data\Repositories
```

**File Movements**:
| From | To | Namespace Change |
|------|-----|-----------------|
| `Data/ApplicationDbContext.cs` | `Infrastructure/Data/` | `Rosheta.Data` → `Rosheta.Infrastructure.Data` |
| `Repositories/*.cs` | `Infrastructure/Data/Repositories/` | `Rosheta.Repositories` → `Rosheta.Infrastructure.Data.Repositories` |
| `Settings/LicenseSettings.cs` | `Infrastructure/Settings/` | `Rosheta.Settings` → `Rosheta.Infrastructure.Settings` |

### Phase 4: Reorganize Presentation Layer

**Create Structure**:
```
mkdir Presentation
mkdir Presentation\Pages
mkdir Presentation\Filters
mkdir Presentation\ViewModels
mkdir Presentation\wwwroot
```

**File Movements**:
| From | To | Namespace Change |
|------|-----|-----------------|
| `Pages/` | `Presentation/Pages/` | `Rosheta.Pages` → `Rosheta.Presentation.Pages` |
| `Filters/` | `Presentation/Filters/` | `Rosheta.Filters` → `Rosheta.Presentation.Filters` |
| `ViewModels/PrescriptionCreateModel.cs` | `Presentation/ViewModels/Prescription/` | `Rosheta.ViewModels` → `Rosheta.Presentation.ViewModels.Prescription` |
| `wwwroot/` | `Presentation/wwwroot/` | No namespace change |

### Phase 5: Update Program.cs and DI Registration

**Update Paths**:
```csharp
// Program.cs updates
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Presentation/Pages";
    });

// Static files configuration
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Presentation/wwwroot")),
    RequestPath = ""
});
```

**Update Namespace Imports**:
```csharp
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Infrastructure;
using Rosheta.Core.Application.Services;
using Rosheta.Infrastructure.Data;
using Rosheta.Infrastructure.Data.Repositories;
using Rosheta.Infrastructure.Storage;
using Rosheta.Infrastructure.Settings;
using Rosheta.Presentation.Filters;
```

## Implementation Guide

### Safe Migration Order

1. **Backup Current State**
   ```bash
   git add .
   git commit -m "Before architecture reorganization"
   git checkout -b architecture-reorganization
   ```

2. **Fix Namespace Inconsistencies First** (15 minutes)
   - Find/Replace all `Roshta` with `Rosheta`
   - Test build

3. **Create All New Folders** (5 minutes)
   - Create entire new structure empty
   - No files moved yet

4. **Move Domain Layer** (20 minutes)
   - Move entities, base classes
   - Update namespaces
   - Build and fix errors

5. **Move Application Contracts** (15 minutes)
   - Move all interfaces first
   - Update namespaces
   - Build and fix errors

6. **Move Application Services** (20 minutes)
   - Move service implementations
   - Move DTOs
   - Update namespaces
   - Build and fix errors

7. **Move Infrastructure** (20 minutes)
   - Move DbContext
   - Move Repositories
   - Update namespaces
   - Build and fix errors

8. **Move Presentation** (30 minutes)
   - Move Pages
   - Move Filters
   - Move remaining ViewModels
   - Move wwwroot
   - Update Program.cs paths
   - Build and fix errors

9. **Final Testing** (15 minutes)
   - Run application
   - Test key features
   - Fix any runtime issues

**Total Estimated Time**: 2.5 hours

## Risk Assessment & Mitigation

### Risks

1. **Build Failures**
   - **Mitigation**: Fix namespaces incrementally, build after each phase
   
2. **Runtime Path Issues**
   - **Mitigation**: Carefully update Program.cs configurations
   
3. **EF Migrations Break**
   - **Mitigation**: Keep Migrations folder at root, update DbContext namespace references

4. **Lost Work**
   - **Mitigation**: Use Git branch, commit frequently

### Rollback Plan

If issues arise:
```bash
git stash  # Save any fixes
git checkout main  # Return to original
git branch -D architecture-reorganization  # Remove failed attempt
```

## Future Considerations

### Phase 1.5: Testing Structure
```
tests/
├── Rosheta.Core.Domain.Tests/
├── Rosheta.Core.Application.Tests/
├── Rosheta.Infrastructure.Tests/
└── Rosheta.Presentation.Tests/
```

### Phase 3: MAUI Class Library Extraction
```
src/
├── Rosheta.Core.Domain/        # Class library
├── Rosheta.Core.Application/   # Class library
├── Rosheta.Infrastructure/     # Class library
├── Rosheta.Web/                # Current Razor Pages
└── Rosheta.Maui/               # New MAUI app
```

## Benefits of New Structure

### 1. Clean Architecture Compliance
- **Domain Independence**: Domain entities have no external dependencies
- **Dependency Rule**: Dependencies point inward (Presentation → Application → Domain)
- **Testability**: Each layer can be tested independently

### 2. MAUI Migration Ready
- **Shared Core**: Domain and Application layers can be shared
- **Platform-Specific UI**: Web and MAUI have separate presentation layers
- **Minimal Refactoring**: Clean boundaries mean less code change

### 3. Developer Experience
- **Discoverability**: Clear organization by feature and layer
- **Consistency**: Predictable file locations
- **Onboarding**: New developers understand structure quickly

### 4. Scalability
- **Feature Addition**: Clear where new features go
- **Team Scaling**: Teams can work on different layers
- **Performance**: Future ability to split into microservices

### 5. Maintainability
- **Single Responsibility**: Each folder has clear purpose
- **Reduced Coupling**: Layers communicate through interfaces
- **Easier Refactoring**: Changes isolated to specific layers

## Architecture Decision Records (ADRs)

### ADR-001: Separate Core into Domain and Application
**Decision**: Split Core into Domain and Application subdirectories rather than separate projects initially.

**Rationale**:
- Simpler initial migration
- Can extract to class libraries later
- Maintains clean boundaries through folder structure

### ADR-002: Keep Migrations at Root
**Decision**: Leave Migrations folder at project root.

**Rationale**:
- EF Core convention
- Simplifies migration commands
- Avoid path issues

### ADR-003: Group DTOs by Feature
**Decision**: Organize DTOs in feature folders (Doctor/, Patient/, etc.).

**Rationale**:
- Better cohesion
- Easier to find related DTOs
- Scales better with growth

### ADR-004: Infrastructure Contains All External Dependencies
**Decision**: Put database, file storage, and future email/SMS in Infrastructure.

**Rationale**:
- Single place for external dependencies
- Easy to mock for testing
- Clear boundary for what needs configuration

## Implementation Checklist

### Pre-Migration
- [ ] Create Git branch for safety
- [ ] Document current working features for testing
- [ ] Ensure all tests pass (if any exist)
- [ ] Backup database

### During Migration
- [ ] Fix namespace inconsistencies (Roshta → Rosheta)
- [ ] Create new folder structure
- [ ] Move Domain layer files
- [ ] Move Application layer files
- [ ] Move Infrastructure layer files
- [ ] Move Presentation layer files
- [ ] Update Program.cs
- [ ] Update all namespace imports
- [ ] Build and fix compilation errors
- [ ] Test application runs

### Post-Migration
- [ ] Test all major features
- [ ] Update documentation
- [ ] Create README.md for each layer
- [ ] Commit changes
- [ ] Consider creating NuGet packages for Core layers

## Layer-Specific README Templates

### Core/README.md
```markdown
# Core Layer

This layer contains the heart of the application - business logic and domain models.

## Structure
- **Domain/**: Pure business entities and logic
- **Application/**: Business use cases and application logic

## Rules
- Domain has NO external dependencies
- Application depends only on Domain
- All external dependencies use interfaces defined here

## Future
Will be extracted to Rosheta.Core class library for sharing with MAUI.
```

### Infrastructure/README.md
```markdown
# Infrastructure Layer

This layer implements all external dependencies.

## Structure
- **Data/**: Entity Framework, database access
- **Storage/**: File system operations
- **Settings/**: Configuration models

## Rules
- Implements interfaces defined in Core/Application
- Can reference Core layer
- Cannot reference Presentation layer

## Technologies
- Entity Framework Core
- Local file storage (future: Azure Blob Storage)
```

### Presentation/README.md
```markdown
# Presentation Layer

This layer contains all UI-related code.

## Structure
- **Pages/**: Razor Pages organized by feature
- **Filters/**: ASP.NET Core filters
- **ViewModels/**: Page-specific models
- **wwwroot/**: Static assets (CSS, JS, images)

## Rules
- References Core and Infrastructure
- Depends on Application Services (not repositories directly)
- Contains no business logic

## Future
Will be one of multiple presentation layers (Web, MAUI, API).
```

## Conclusion

This reorganization positions Rosheta for sustainable growth while maintaining clean architecture principles. The phased approach minimizes risk while providing clear benefits at each stage.

### Immediate Benefits
- Fixed namespace inconsistencies
- Clear separation of concerns
- Better code organization

### Long-term Benefits
- Ready for MAUI migration
- Supports team growth
- Enables independent testing
- Facilitates future architectural changes

### Recommendation
**Proceed with this reorganization** in a dedicated branch. The 2.5-hour investment will pay dividends in:
- Reduced technical debt
- Easier feature development
- Smoother MAUI migration
- Better team collaboration

The structure follows industry best practices and positions Rosheta as a professional, maintainable healthcare application ready for expansion across platforms.