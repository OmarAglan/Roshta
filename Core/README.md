# Core Layer

This layer contains the **Domain** and **Application** logic, representing the heart of the application.

## Structure

### Domain (`Core/Domain/`)
Contains the **business entities** and **domain logic** that are independent of any external concerns.

- **`Entities/`** - Domain entities (Doctor, Patient, Prescription, Medication, etc.)
- **`Enums/`** - Domain enumerations (PrescriptionStatus, etc.)
- **`Base/`** - Base classes for entities (BaseEntity, AuditableEntity)
- **`Constants/`** - Domain constants

**Key Principle:** Domain entities should have no dependencies on infrastructure, UI, or external libraries.

### Application (`Core/Application/`)
Contains **application logic**, **use cases**, and **abstractions** for external dependencies.

- **`Contracts/`** - Interfaces that define contracts for services and repositories
  - **`Services/`** - Service interfaces (IDoctorService, IPatientService, etc.)
  - **`Persistence/`** - Repository interfaces (IDoctorRepository, IPatientRepository, etc.)
  - **`Infrastructure/`** - Infrastructure service interfaces (IFileStorageProvider, etc.)
- **`Services/`** - Application service implementations
- **`DTOs/`** - Data Transfer Objects organized by feature
- **`Models/`** - Application-specific models (UserSettingsModel, etc.)

**Key Principle:** The Application layer defines **what** the system does, but not **how** it does it. It depends on abstractions (interfaces) rather than concrete implementations.

## Dependencies

- **No external dependencies** - This layer should be completely independent
- **Direction:** All other layers depend on Core, but Core doesn't depend on anything

## Namespace Convention

All code in this layer uses the `Rosheta.Core.*` namespace pattern:
- Domain: `Rosheta.Core.Domain.*`
- Application: `Rosheta.Core.Application.*`