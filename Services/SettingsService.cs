using Microsoft.Extensions.Logging;
using Roshta.Services.Interfaces;
using Roshta.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roshta.Services;

public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsDirectory;

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;
        _settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rosheta", "Settings");
        
        // Ensure settings directory exists
        if (!Directory.Exists(_settingsDirectory))
        {
            Directory.CreateDirectory(_settingsDirectory);
        }
    }

    public async Task<UserSettingsModel> GetUserSettingsAsync(int doctorId)
    {
        try
        {
            var settingsFilePath = GetSettingsFilePath(doctorId);
            
            if (!File.Exists(settingsFilePath))
            {
                _logger.LogInformation("Settings file not found for doctor {DoctorId}, returning default settings", doctorId);
                return GetDefaultSettings();
            }

            var json = await File.ReadAllTextAsync(settingsFilePath);
            var settings = JsonSerializer.Deserialize<UserSettingsModel>(json);
            
            if (settings == null)
            {
                _logger.LogWarning("Failed to deserialize settings for doctor {DoctorId}, returning default settings", doctorId);
                return GetDefaultSettings();
            }

            _logger.LogDebug("Successfully loaded settings for doctor {DoctorId}", doctorId);
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading settings for doctor {DoctorId}, returning default settings", doctorId);
            return GetDefaultSettings();
        }
    }

    public async Task<bool> SaveUserSettingsAsync(int doctorId, UserSettingsModel settings)
    {
        try
        {
            var settingsFilePath = GetSettingsFilePath(doctorId);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(settingsFilePath, json);
            _logger.LogInformation("Successfully saved settings for doctor {DoctorId}", doctorId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving settings for doctor {DoctorId}", doctorId);
            return false;
        }
    }

    public UserSettingsModel GetDefaultSettings()
    {
        return new UserSettingsModel();
    }

    private string GetSettingsFilePath(int doctorId)
    {
        return Path.Combine(_settingsDirectory, $"doctor_{doctorId}_settings.json");
    }
}