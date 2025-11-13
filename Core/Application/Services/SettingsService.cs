using Microsoft.Extensions.Logging;
using Rosheta.Core.Application.Contracts.Infrastructure;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using Rosheta.Core.Application.Common.Exceptions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly IFileStorageProvider _fileStorage;
    private readonly string _settingsDirectory;

    public SettingsService(ILogger<SettingsService> logger, IFileStorageProvider fileStorage)
    {
        _logger = logger;
        _fileStorage = fileStorage;
        _settingsDirectory = _fileStorage.CombinePath(_fileStorage.GetApplicationDataPath(), "Rosheta", "Settings");

        // Ensure settings directory exists
        var testFilePath = _fileStorage.CombinePath(_settingsDirectory, "test.tmp");
        _fileStorage.EnsureDirectoryExists(testFilePath);
    }

    public async Task<UserSettingsModel> GetUserSettingsAsync(int doctorId)
    {
        try
        {
            var settingsFilePath = GetSettingsFilePath(doctorId);

            if (!_fileStorage.FileExists(settingsFilePath))
            {
                _logger.LogInformation("Settings file not found for doctor {DoctorId}, returning default settings", doctorId);
                return GetDefaultSettings();
            }

            var json = await _fileStorage.ReadAllTextAsync(settingsFilePath);
            var settings = JsonSerializer.Deserialize<UserSettingsModel>(json);

            if (settings == null)
            {
                _logger.LogWarning("Failed to deserialize settings for doctor {DoctorId}, returning default settings", doctorId);
                return GetDefaultSettings();
            }

            _logger.LogDebug("Successfully loaded settings for doctor {DoctorId}", doctorId);
            return settings;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            _logger.LogError(ex, "Error loading settings for doctor {DoctorId}", doctorId);
            throw new InfrastructureException($"Failed to load settings for doctor {doctorId}.", ex);
        }
    }

    public async Task<bool> SaveUserSettingsAsync(int doctorId, UserSettingsModel settings)
    {
        if (settings == null)
        {
            throw new ValidationException("Settings cannot be null.");
        }

        try
        {
            var settingsFilePath = GetSettingsFilePath(doctorId);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            _fileStorage.EnsureDirectoryExists(settingsFilePath);
            await _fileStorage.WriteAllTextAsync(settingsFilePath, json);
            _logger.LogInformation("Successfully saved settings for doctor {DoctorId}", doctorId);
            return true;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            _logger.LogError(ex, "Error saving settings for doctor {DoctorId}", doctorId);
            throw new InfrastructureException($"Failed to save settings for doctor {doctorId}.", ex);
        }
    }

    public UserSettingsModel GetDefaultSettings()
    {
        return new UserSettingsModel();
    }

    private string GetSettingsFilePath(int doctorId)
    {
        return _fileStorage.CombinePath(_settingsDirectory, $"doctor_{doctorId}_settings.json");
    }
}
