using AutoMapper;
using HrBackend.DTO_S.Departments;
using HrBackend.Models;

namespace HrBackend.Mapping;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        // Map DepartmentDto -> Employee
        CreateMap<DeptDTO, Department>().ReverseMap();


        CreateMap<UpdateDeptDTO, Department>().ReverseMap();
            
    }
    
}
