# Configuration Layer

This layer contains **application configuration** and **settings models**.

## Structure

### Settings (`Configuration/Settings/`)
Contains **strongly-typed configuration models** for application settings.

- **`LicenseSettings.cs`** - License validation configuration

## Key Responsibilities

1. **Configuration Models** - Define strongly-typed classes for appsettings.json sections
2. **Application Settings** - Centralize all configuration concerns
3. **Environment-specific Config** - Support different settings per environment

## Usage

Settings are registered in `Program.cs` using the Options pattern:

```csharp
builder.Services.Configure<LicenseSettings>(
    builder.Configuration.GetSection(LicenseSettings.SectionName));
```

And injected into services/pages using `IOptions<T>`:

```csharp
public MyService(IOptions<LicenseSettings> licenseSettings)
{
    _settings = licenseSettings.Value;
}
```

## Dependencies

- **No dependencies** - Configuration models should be POCOs (Plain Old CLR Objects)
- **Used by:** Infrastructure and Presentation layers

## Namespace Convention

All code in this layer uses the `Rosheta.Configuration.*` namespace pattern:
- Settings: `Rosheta.Configuration.Settings.*`

## Best Practices

- Keep configuration models simple and focused
- Use meaningful section names in `appsettings.json`
- Validate configuration at startup when possible
- Never hardcode sensitive information (use User Secrets or environment variables)