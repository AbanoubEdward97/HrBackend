using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Services;

public interface IEmployeesService
{
    //IQueryable<Employee> GetEmployees();
    PagedList<Employee> GetEmployees(PagingParams pagingParams);
    Task<Employee> AddEmployee(Employee employee);
    Task<Employee> UpdateEmployee(long Id, Employee employee);
    Employee DeleteEmployee(Employee employee);
    Task<Employee> GetById(long Id);
    Task SaveChanges();
}
