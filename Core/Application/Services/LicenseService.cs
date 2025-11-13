using Microsoft.Extensions.Options;
using Rosheta.Core.Application.Contracts.Infrastructure;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Configuration.Settings;

namespace Rosheta.Core.Application.Services;

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
            _fileStorage.EnsureDirectoryExists(_activationFlagPath);
            await _fileStorage.WriteAllTextAsync(_activationFlagPath, string.Empty);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to mark application as activated.", ex);
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
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to mark profile as setup.", ex);
        }
    }

    public async Task<int?> GetCurrentDoctorIdAsync()
    {
        // Return cached value if already checked
        if (_doctorIdChecked)
        {
            return _cachedDoctorId;
        }

        _doctorIdChecked = true;

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
            catch (Exception ex)
            {
                throw new InfrastructureException("Failed to retrieve current doctor ID.", ex);
            }
        }

        _cachedDoctorId = null;
        return null;
    }
}
