using Microsoft.Extensions.Options; // Required for IOptions
using Rosheta.Infrastructure.Storage.Interfaces;
using Roshta.Services.Interfaces;
using Roshta.Settings;

namespace Roshta.Services;

public class LicenseService : ILicenseService
{
    private readonly LicenseSettings _licenseSettings;
    private readonly IFileStorageProvider _fileStorage;
    private readonly string _activationFlagPath; // Path to the activation marker file
    private readonly string _doctorIdFlagPath;   // Path to store the configured doctor ID

    // Cache the Doctor ID to avoid reading the file repeatedly
    private int? _cachedDoctorId = null;
    private bool _doctorIdChecked = false;

    public LicenseService(IOptions<LicenseSettings> licenseSettingsOptions, IFileStorageProvider fileStorage)
    {
        _licenseSettings = licenseSettingsOptions.Value;
        _fileStorage = fileStorage;
        
        var baseDirectory = _fileStorage.CombinePath(_fileStorage.GetApplicationDataPath(), "Rosheta");
        _activationFlagPath = _fileStorage.CombinePath(baseDirectory, ".activated");
        _doctorIdFlagPath = _fileStorage.CombinePath(baseDirectory, ".doctorid");
    }

    public bool ValidateLicense(string enteredRegistrationNumber, string enteredSerialNumber)
    {
        // Simple string comparison (case-insensitive for robustness)
        bool isValid = string.Equals(enteredRegistrationNumber, _licenseSettings.ExpectedRegistrationNumber, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(enteredSerialNumber, _licenseSettings.ExpectedSerialNumber, StringComparison.OrdinalIgnoreCase);
        
        // TODO: Implement more robust validation if needed (e.g., checking format, checksums, or calling an external server)

        return isValid;
    }

    public bool IsActivated()
    {
        // Check if the activation marker file exists.
        return _fileStorage.FileExists(_activationFlagPath);
    }

    public async Task MarkAsActivatedAsync()
    {
        try
        {
            // Create the empty marker file to indicate activation.
            // The content doesn't matter, only its existence.
            _fileStorage.EnsureDirectoryExists(_activationFlagPath);
            await _fileStorage.WriteAllTextAsync(_activationFlagPath, string.Empty);
        }
        catch (Exception)
        {
            // TODO: Log the exception (ex)
            // Handle potential file system errors (permissions, etc.)
            // For now, activation might fail silently if the file can't be created.
        }
    }

    // --- New Methods ---

    public async Task<bool> IsProfileSetupAsync()
    {
        // Profile is setup if the Doctor ID file exists and contains a valid integer
        return await GetCurrentDoctorIdAsync() != null;
    }

    public async Task MarkProfileAsSetupAsync(int doctorId)
    {
        try
        {
            // Store the Doctor ID in the flag file.
            _fileStorage.EnsureDirectoryExists(_doctorIdFlagPath);
            await _fileStorage.WriteAllTextAsync(_doctorIdFlagPath, doctorId.ToString());
            // Reset cache
            _cachedDoctorId = doctorId;
            _doctorIdChecked = true;
        }
        catch (Exception)
        {
            // TODO: Log the exception (ex)
            // Handle potential file system errors
        }
    }

    public async Task<int?> GetCurrentDoctorIdAsync()
    {
        // Return cached value if already checked
        if (_doctorIdChecked)
        {
            return _cachedDoctorId;
        }

        _doctorIdChecked = true; // Mark as checked even if file doesn't exist or is invalid
        if (_fileStorage.FileExists(_doctorIdFlagPath))
        {
            try
            {
                string content = await _fileStorage.ReadAllTextAsync(_doctorIdFlagPath);
                if (int.TryParse(content, out int id))
                {
                    _cachedDoctorId = id;
                    return id;
                }
            }
            catch (Exception)
            {
                 // TODO: Log the exception (ex)
            }
        }
        
        _cachedDoctorId = null;
        return null;
    }
} 