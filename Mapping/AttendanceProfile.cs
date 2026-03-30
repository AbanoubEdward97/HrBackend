using AutoMapper;
using HrBackend.DTO_S.Attendance;
using HrBackend.Models;

namespace HrBackend.Mapping;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        CreateMap<AttendanceUpsertDTO, AttendanceRecord>();
    }
}
