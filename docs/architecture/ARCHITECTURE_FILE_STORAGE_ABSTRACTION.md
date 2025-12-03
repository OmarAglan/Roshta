# File Storage Abstraction Architecture

## Overview

This document describes the file storage abstraction layer implemented in the Rosheta application to enable testability and cross-platform compatibility for future MAUI migration.

## Problem Statement

Previously, [`LicenseService`](Services/LicenseService.cs) and [`SettingsService`](Services/SettingsService.cs) directly accessed the file system using:
- `File.Exists()`, `File.ReadAllText()`, `File.WriteAllText()`
- `Directory.CreateDirectory()`
- `Path.Combine()`
- `Environment.GetFolderPath(SpecialFolder.LocalApplicationData)`

This created several issues:
- ❌ Not testable without actual file system
- ❌ Not portable across platforms (desktop/mobile/web)
- ❌ Violated dependency inversion principle
- ❌ Tight coupling to .NET file system APIs

## Solution Architecture

### Infrastructure Layer Structure

```
Infrastructure/
├── Storage/
│   ├── Interfaces/
│   │   └── IFileStorageProvider.cs    # Abstraction interface
│   └── LocalFileStorageProvider.cs     # Desktop implementation
```

### Core Components

#### 1. IFileStorageProvider Interface

Location: [`Infrastructure/Storage/Interfaces/IFileStorageProvider.cs`](Infrastructure/Storage/Interfaces/IFileStorageProvider.cs)

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

**Purpose**: Provides platform-agnostic file storage operations.

**Key Methods**:
- `FileExists()` - Synchronous check for file existence
- `ReadAllTextAsync()` - Async file reading
- `WriteAllTextAsync()` - Async file writing with directory creation
- `GetApplicationDataPath()` - Platform-specific app data location
- `CombinePath()` - Platform-safe path combination
- `EnsureDirectoryExists()` - Safe directory creation

#### 2. LocalFileStorageProvider Implementation

Location: [`Infrastructure/Storage/LocalFileStorageProvider.cs`](Infrastructure/Storage/LocalFileStorageProvider.cs)

**Purpose**: Desktop/local file system implementation using standard .NET APIs.

**Characteristics**:
- Registered as `Singleton` (stateless, can be shared)
- Uses standard `System.IO` namespace
- Handles directory creation automatically
- Async operations for better performance

#### 3. Updated Services

**LicenseService Changes**:
- Added `IFileStorageProvider` dependency injection
- Converted methods to async: [`MarkAsActivatedAsync()`](Services/LicenseService.cs:46), [`GetCurrentDoctorIdAsync()`](Services/LicenseService.cs:89), [`IsProfileSetupAsync()`](Services/LicenseService.cs:65), [`MarkProfileAsSetupAsync()`](Services/LicenseService.cs:71)
- All file operations now use abstraction layer
- Application data stored in `{AppData}/Rosheta/` directory

**SettingsService Changes**:
- Added `IFileStorageProvider` dependency injection
- All file operations use abstraction layer
- Settings stored in `{AppData}/Rosheta/Settings/` directory
- Automatic directory creation before file writes

### Dependency Injection Configuration

Location: [`Program.cs`](Program.cs:28)

```csharp
// Infrastructure Services (registered first)
builder.Services.AddSingleton<IFileStorageProvider, LocalFileStorageProvider>();

// Application Services (depend on infrastructure)
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
```

**Registration Strategy**:
- `IFileStorageProvider` → `Singleton` (stateless, thread-safe)
- Services → `Scoped` (per request lifecycle)

## Async/Await Pattern

### Why Async?

1. **File I/O is inherently I/O-bound** - Benefits from async operations
2. **Better responsiveness** - UI doesn't freeze during file operations
3. **MAUI requirement** - Mobile platforms require async file access
4. **Modern .NET best practices** - Aligns with async patterns

### Updated Method Signatures

**Interface Changes** ([`ILicenseService.cs`](Services/Interfaces/ILicenseService.cs)):
```csharp
Task MarkAsActivatedAsync();
Task<bool> IsProfileSetupAsync();
Task MarkProfileAsSetupAsync(int doctorId);
Task<int?> GetCurrentDoctorIdAsync();
```

**Caller Updates**:
- [`Activate.cshtml.cs`](Pages/Activate.cshtml.cs:63) - `await _licenseService.MarkAsActivatedAsync()`
- [`Setup.cshtml.cs`](Pages/DoctorProfile/Setup.cshtml.cs:118) - `await _licenseService.MarkProfileAsSetupAsync()`
- [`Edit.cshtml.cs`](Pages/DoctorProfile/Edit.cshtml.cs:72) - `await _licenseService.GetCurrentDoctorIdAsync()`
- [`ActivationCheckPageFilter.cs`](Filters/ActivationCheckPageFilter.cs:45) - `await _licenseService.IsProfileSetupAsync()`
- [`_Layout.cshtml`](Pages/Shared/_Layout.cshtml:136) - `await LicenseService.IsProfileSetupAsync()`

## Benefits of This Architecture

### 1. Testability ✅
```csharp
// Example: Mock file storage for unit tests
var mockStorage = new Mock<IFileStorageProvider>();
mockStorage.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
var service = new LicenseService(options, mockStorage.Object);
```

### 2. Platform Portability ✅
Future implementations can target different platforms:
```csharp
// MAUI implementation
public class MauiFileStorageProvider : IFileStorageProvider
{
    public string GetApplicationDataPath() 
        => FileSystem.AppDataDirectory; // MAUI API
}

// Azure Blob Storage implementation
public class AzureBlobStorageProvider : IFileStorageProvider
{
    // Cloud storage implementation
}

// Secure Storage for sensitive data
public class SecureStorageProvider : IFileStorageProvider
{
    public async Task WriteAllTextAsync(string path, string content)
        => await SecureStorage.SetAsync(path, content); // MAUI SecureStorage
}
```

### 3. Separation of Concerns ✅
- Business logic (Services) separated from infrastructure (Storage)
- Clean Architecture principles
- Single Responsibility Principle

### 4. Flexibility ✅
- Easy to swap implementations
- Can add caching layer
- Can add encryption
- Can log all file operations

## Future Enhancements

### Planned Infrastructure Organization

```
Infrastructure/
├── Storage/          # File storage (DONE)
│   ├── Interfaces/
│   │   └── IFileStorageProvider.cs
│   ├── LocalFileStorageProvider.cs
│   ├── MauiFileStorageProvider.cs      # TODO: MAUI migration
│   └── SecureStorageProvider.cs         # TODO: Sensitive data
├── Logging/          # TODO: Logging implementations
│   ├── Interfaces/
│   └── FileLogger.cs
├── Email/            # TODO: Email services
│   ├── Interfaces/
│   └── SmtpEmailProvider.cs
└── Caching/          # TODO: Caching implementations
    ├── Interfaces/
    └── MemoryCacheProvider.cs
```

### Potential MAUI Implementations

1. **MauiFileStorageProvider** - Using MAUI `FileSystem` API
2. **SecureStorageProvider** - Using MAUI `SecureStorage` for licenses
3. **CloudStorageProvider** - For cloud sync scenarios
4. **SqliteStorageProvider** - For structured data storage

## Testing Strategy

### Unit Tests Example

```csharp
[Fact]
public async Task MarkAsActivated_CreatesActivationFile()
{
    // Arrange
    var mockStorage = new Mock<IFileStorageProvider>();
    var service = new LicenseService(options, mockStorage.Object);
    
    // Act
    await service.MarkAsActivatedAsync();
    
    // Assert
    mockStorage.Verify(x => x.WriteAllTextAsync(
        It.IsAny<string>(), 
        It.IsAny<string>()), Times.Once);
}
```

### Integration Tests

- Test with actual `LocalFileStorageProvider`
- Verify file creation in temp directories
- Test error handling for missing directories
- Test concurrent access scenarios

## Migration Guide for New Services

When creating new services that need file access:

1. **Inject IFileStorageProvider**:
```csharp
public class MyNewService : IMyNewService
{
    private readonly IFileStorageProvider _fileStorage;
    
    public MyNewService(IFileStorageProvider fileStorage)
    {
        _fileStorage = fileStorage;
    }
}
```

2. **Use async patterns**:
```csharp
public async Task SaveDataAsync(string data)
{
    var path = _fileStorage.CombinePath(
        _fileStorage.GetApplicationDataPath(), 
        "MyApp", 
        "data.json");
        
    _fileStorage.EnsureDirectoryExists(path);
    await _fileStorage.WriteAllTextAsync(path, data);
}
```

3. **Never use System.IO directly** in service classes

## Breaking Changes

### Before (Synchronous)
```csharp
_licenseService.MarkAsActivated();
var doctorId = _licenseService.GetCurrentDoctorId();
```

### After (Asynchronous)
```csharp
await _licenseService.MarkAsActivatedAsync();
var doctorId = await _licenseService.GetCurrentDoctorIdAsync();
```

**Note**: All callers have been updated. No breaking changes remain.

## Performance Considerations

1. **Singleton Pattern**: `IFileStorageProvider` is stateless - single instance across app
2. **Async I/O**: Non-blocking file operations improve responsiveness
3. **Caching**: `LicenseService` caches doctor ID to minimize file reads
4. **Directory Checks**: `EnsureDirectoryExists()` only creates if needed

## Security Considerations

Current implementation stores files in:
- Windows: `%LOCALAPPDATA%\Rosheta\`
- Future MAUI: Platform-specific secure locations

For sensitive data (licenses, credentials):
- Consider `SecureStorageProvider` for MAUI
- Encrypt file contents
- Use OS-level file encryption

## Compliance & Standards

- ✅ Follows SOLID principles
- ✅ Async/await best practices
- ✅ Clean Architecture layering
- ✅ Dependency Inversion Principle
- ✅ Single Responsibility Principle

## Summary

This refactoring successfully:
- ✅ Abstracted all file system dependencies
- ✅ Made codebase testable without real file system
- ✅ Prepared architecture for MAUI migration
- ✅ Improved code quality and maintainability
- ✅ Added async/await for better performance
- ✅ Maintained all existing functionality
- ✅ Zero breaking changes (all callers updated)
- ✅ Project builds successfully

The application is now ready for cross-platform deployment and has a solid foundation for future infrastructure enhancements.