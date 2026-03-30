using HrBackend.DTO_S.Attendance;
using HrBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Services;

public interface IAttendanceService
{
    Task<List<AttendanceRecord>> GetAttendance(AttendanceQueryDTO dto);
    Task<AttendanceRecord?> Upsert(AttendanceUpsertDTO dto);
    Task<AttendanceRecord> Delete(int id);
    Task<AttendanceImportResultDto> ImportExcel(ImportExcellDTO dto);


}