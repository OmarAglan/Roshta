using Roshta.ViewModels;
using System.Threading.Tasks;

namespace Roshta.Services.Interfaces;

public interface ISettingsService
{
    Task<UserSettingsModel> GetUserSettingsAsync(int doctorId);
    Task<bool> SaveUserSettingsAsync(int doctorId, UserSettingsModel settings);
    UserSettingsModel GetDefaultSettings();
}