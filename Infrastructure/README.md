# Infrastructure Layer

This layer contains **concrete implementations** of the abstractions defined in the Core layer, handling all **external dependencies** and **technical concerns**.

## Structure

### Data (`Infrastructure/Data/`)
Contains **database-related** implementations.

- **`ApplicationDbContext.cs`** - Entity Framework Core DbContext
- **`Repositories/`** - Repository implementations (DoctorRepository, PatientRepository, etc.)
- **`Configurations/`** - Entity Framework entity configurations (for future use)

**Technologies:** Entity Framework Core, SQLite

### Storage (`Infrastructure/Storage/`)
Contains **file storage** implementations.

- **`LocalFileStorageProvider.cs`** - Local file system storage implementation
- **`Interfaces/`** - Storage-specific interfaces (if needed)

**Technologies:** System.IO, ASP.NET Core file handling

## Key Responsibilities

1. **Data Access** - Implement repository interfaces using Entity Framework Core
2. **File Storage** - Implement file storage providers (local, cloud, etc.)
3. **External Services** - Any third-party service integrations
4. **Technical Infrastructure** - Logging, caching, email, etc. (when added)

## Dependencies

- **Depends on:** Core layer (for interfaces and entities)
- **External packages:** Entity Framework Core, database providers, etc.

## Namespace Convention

All code in this layer uses the `Rosheta.Infrastructure.*` namespace pattern:
- Data: `Rosheta.Infrastructure.Data.*`
- Storage: `Rosheta.Infrastructure.Storage.*`

## Important Notes

- **Never reference the Presentation layer** from Infrastructure
- All database configurations should eventually move to `Data/Configurations/`
- Follow the Repository pattern for data access
- Use dependency injection for all infrastructure services