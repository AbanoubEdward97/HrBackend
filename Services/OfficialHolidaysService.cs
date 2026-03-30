using HrBackend.DTO_S.OfficialHolidays;
using HrBackend.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HrBackend.Services;

public class OfficialHolidaysService : IOfficialHolidaysService
{
    private readonly HrContext _context;

    public OfficialHolidaysService(HrContext context)
    {
        _context = context;
    }

    public async Task<OfficialHoliday> Add(CreateOfficialHolidayDTO dto)
    {
        var date = dto.Date;
        var exists = await checkExists(date);
        if (exists)
        {
            throw new InvalidOperationException("Holiday With That date already exists");
        }
        var holiday = new OfficialHoliday
        {
            Date = date,
            Name = dto.Name.Trim()
        };

        await _context.AddAsync(holiday);
        await _context.SaveChangesAsync();
        return holiday;
    }

    public async Task<bool> checkExists(DateOnly date)
    {
        return await _context.OfficialHolidays.AnyAsync(x => x.Date == date);
    }

    public async Task<OfficialHoliday> DeleteHoliday(int id)
    {
        var holiday = await _context.OfficialHolidays.FindAsync(id);
        if (holiday == null)
        {
            throw new InvalidOperationException("holiday not found");
        }
        _context.Remove(holiday);
        await _context.SaveChangesAsync();

        return holiday;
    }

    public async Task<List<OfficialHoliday>> GetAll()
    {
        return await _context.OfficialHolidays.ToListAsync();
    }

    public async Task<OfficialHoliday> UpdateHoliday(int id, UpdateOfficialHolidayDTO dto)
    {
        var holiday = await _context.OfficialHolidays.FindAsync(id);
        if (holiday == null)
        {
            throw new InvalidOperationException ("Holiday Not Found");
        }

        var date = dto.Date;
        var exists = await _context.OfficialHolidays.AnyAsync(h => h.Id != id && h.Date == date);
        if (exists)
        {
            throw new InvalidOperationException("A holiday already exists on that date !!");
        }

        holiday.Name = dto.Name.Trim();
        holiday.Date = date;

        await _context.SaveChangesAsync();
        return holiday;
    }
}
