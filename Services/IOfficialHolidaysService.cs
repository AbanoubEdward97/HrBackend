using HrBackend.DTO_S.OfficialHolidays;
using HrBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Services;

public interface IOfficialHolidaysService
{
    Task<List<OfficialHoliday>> GetAll();
    Task<bool> checkExists(DateOnly date);
    Task<OfficialHoliday> Add(CreateOfficialHolidayDTO dto);

    Task<OfficialHoliday> UpdateHoliday(int id, UpdateOfficialHolidayDTO dto);

    Task<OfficialHoliday> DeleteHoliday(int id);
}
