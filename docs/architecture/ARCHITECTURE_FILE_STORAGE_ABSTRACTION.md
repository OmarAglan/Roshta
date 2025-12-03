# File Storage Abstraction Architecture

**Status:** ✅ Implemented
**Location:** `Rosheta.Infrastructure` Project

## Overview

This document describes the file storage abstraction layer implemented in the Rosheta application. This abstraction is critical for decoupling the Core business logic from the physical file system, enabling unit testing, and ensuring cross-platform compatibility (Web vs. MAUI/Mobile).

## Problem Statement

Direct file system access (using `System.IO.File` or `System.IO.Directory`) in the Business Logic Layer creates several issues:

1. **Testing:** It is impossible to write pure unit tests without mocking the file system.
2. **Portability:** Mobile platforms (MAUI/Android/iOS) handle file paths and permissions differently than Windows/Linux servers.
3. **Coupling:** The Domain/Application layers should not depend on specific infrastructure concerns.

## Solution Architecture

### Project Structure

```text
src/
├── Rosheta.Core/
│   └── Application/
│       └── Contracts/
│           └── Infrastructure/
│               └── IFileStorageProvider.cs    # The Abstraction (Interface)
│
└── Rosheta.Infrastructure/
    └── Storage/
        └── LocalFileStorageProvider.cs        # The Implementation (Concrete)
```

### Core Components

#### 1. IFileStorageProvider Interface

**Location:** `src/Rosheta.Core/Application/Contracts/Infrastructure/IFileStorageProvider.cs`

```csharp
public interface IFileStorageProvider
{
    bool FileExists(string path);
    Task<string> ReadAllTextAsync(string path);
    Task WriteAllTextAsync(string path, string content);
    string GetApplicationDataPath();
    string CombinePath(params string[] paths);
    void EnsureDirectoryExists(string path);
}
```

**Key Responsibilities:**

* Provides platform-agnostic file operations.
* Uses `Task` for I/O bound operations (Async/Await).
* Abstracts path combination logic.

#### 2. LocalFileStorageProvider Implementation

**Location:** `src/Rosheta.Infrastructure/Storage/LocalFileStorageProvider.cs`

**Purpose:** Standard implementation for Web and Desktop (Windows) environments using `System.IO`.

**Characteristics:**

* Registered as **Singleton** (Stateless).
* Uses `Environment.GetFolderPath(SpecialFolder.LocalApplicationData)` to find safe write locations.
* Handles directory creation automatically before writing files.

### Dependency Injection

Registration is handled within the Infrastructure layer's extension method, ensuring the Web layer doesn't need to know the specific implementation class.

**File:** `src/Rosheta.Infrastructure/DependencyInjection.cs`

```csharp
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
{
    // ... Database setup ...

    // Register File Storage
    services.AddSingleton<IFileStorageProvider, LocalFileStorageProvider>();

    return services;
}
```

## Usage Example

Services in `Rosheta.Core` inject the interface, remaining ignorant of the underlying implementation.

```csharp
public class LicenseService : ILicenseService
{
    private readonly IFileStorageProvider _fileStorage;

    public LicenseService(IFileStorageProvider fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task SaveLicenseAsync(string key)
    {
        var path = _fileStorage.CombinePath(
            _fileStorage.GetApplicationDataPath(), 
            "Rosheta", 
            "license.key"
        );
        
        await _fileStorage.WriteAllTextAsync(path, key);
    }
}
```

## Future Extensibility (MAUI Migration)

When porting to .NET MAUI (Phase 3), we do not need to change `Rosheta.Core`. We simply create a new implementation in the MAUI project or a specific Mobile Infrastructure project.

**Hypothetical MAUI Implementation:**

```csharp
public class MauiFileStorageProvider : IFileStorageProvider
{
    public string GetApplicationDataPath() 
    {
        // Maps to native sandbox locations on iOS/Android
        return Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
    }
    // ... other methods using MAUI specific APIs ...
}
```

## Testing Strategy

Because `Rosheta.Core` only references the interface, we can easily mock file operations in our Unit Tests using Moq.

**Example Test (`Rosheta.UnitTests`):**

```csharp
[Fact]
public async Task SaveLicense_ShouldWriteToFile()
{
    // Arrange
    var mockStorage = new Mock<IFileStorageProvider>();
    var service = new LicenseService(mockStorage.Object);

    // Act
    await service.SaveLicenseAsync("123");

    // Assert
    mockStorage.Verify(x => x.WriteAllTextAsync(It.IsAny<string>(), "123"), Times.Once);
}
```

## Summary

This abstraction successfully separates the **"What"** (saving data) from the **"How"** (writing to disk), adhering to the Dependency Inversion Principle of Clean Architecture.
