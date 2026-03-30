using AutoMapper;
using HrBackend.DTO_S.GeneralSettings;
using HrBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Services;

public class GenerallSettingsService : IGenerallSettingsService
{
    private readonly HrContext _context;
    private readonly IMapper _mapper;

    public GenerallSettingsService(HrContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GeneralSettings> GetSettings()
    {
        return await _context.GeneralSettings.FindAsync(1)
        ?? throw new InvalidOperationException("General Settings Are not Found");
    }

    public async Task<GeneralSettings> UpdateSettings(UpdateGeneralSettingsDTO dto)
    {
        var setting = await GetSettings();
        _mapper.Map(dto, setting);
        setting.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return setting;
    }
}
