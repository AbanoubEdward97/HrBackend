using HrBackend.DTO_S.GeneralSettings;
using HrBackend.Models;

namespace HrBackend.Services;

public interface IGenerallSettingsService
{
    Task<GeneralSettings> GetSettings();
    Task<GeneralSettings> UpdateSettings(UpdateGeneralSettingsDTO dto);
}
