
using HrBackend.DTO_S.Departments;
using HrBackend.Models;

namespace HrBackend.Services;

public interface IDepartmentService
{
    Task<List<Department>> getAll();
    Task AddDept(Department dept);

    Task SaveChangesAsync();

    Task<bool> DeptExists(DeptDTO dto);
    Task<Department> GetById(int id);
    Department Remove(Department dept);

}
