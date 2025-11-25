using AutoMapper;
using HrApi.Models;
public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        // Map EmployeeDto -> Employee
        CreateMap<AddEmpDTO,Employee>();

        // Optional: Employee -> EmployeeDto if needed
        CreateMap<Employee,AddEmpDTO>();
    }

}