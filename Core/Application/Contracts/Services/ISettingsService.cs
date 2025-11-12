using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Services;

public interface ISettingsService
{
    Task<UserSettingsModel> GetUserSettingsAsync(int doctorId);
    Task<bool> SaveUserSettingsAsync(int doctorId, UserSettingsModel settings);
    UserSettingsModel GetDefaultSettings();
}
