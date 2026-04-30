using AutoMapper;
using HrBackend.DTO_S.Employees;
public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        // Map EmployeeDto -> Employee
        CreateMap<AddEmpDTO,Employee>()
             .ForMember(dest => dest.AttendDate,
                opt => opt.MapFrom(src =>
                    new DateTime(2000, 1, 1).Add(src.AttendDate.ToTimeSpan())))
            .ForMember(dest => dest.LeaveDate,
                opt => opt.MapFrom(src =>
                    new DateTime(2000, 1, 1).Add(src.LeaveDate.ToTimeSpan())));

        // Optional: Employee -> EmployeeDto if needed
        CreateMap<Employee,AddEmpDTO>();

        //Map Employee -> EmpDetailsDTO
        CreateMap<Employee, EmpDetailsDTO>()
            .ForMember(empDtl => empDtl.GenderName, opt => opt.MapFrom(emp => emp.Gender.ToString()))
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department.Name));
       
        
        CreateMap<Employee, UpdateEmpDTO>()
            .ForMember(d => d.Gender,
                opt => opt.MapFrom(s => s.Gender.ToString()));
            

        CreateMap<UpdateEmpDTO, Employee>()
            .ForMember(d => d.Gender,
                opt => opt.MapFrom(s => Enum.Parse<Gender>(s.Gender)));
    }

}