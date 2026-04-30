
using HrBackend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Services;

public class EmployeesService : IEmployeesService
{
    private readonly HrContext _context;

    public EmployeesService(HrContext context)
    {
        _context = context;
    }

    public async Task<Employee> AddEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public Employee DeleteEmployee(Employee employee)
    {
         _context.Employees.Remove(employee);
        return employee;
    }

    public async Task<Employee> GetById(long Id)
    {
        return await _context.Employees.Include(d => d.Department).SingleOrDefaultAsync(e => Id == e.Id);
    }

    public PagedList<Employee> GetEmployees(PagingParams pagingParams)
    {
        //return _context.Employees.Include(e => e.Department).AsNoTracking().AsQueryable();
        var query = _context.Employees.Include(e => e.Department).AsQueryable();
        if (!string.IsNullOrEmpty(pagingParams.Name))
        {
            query = query.Where(e => e.Name.Contains(pagingParams.Name));
        }
        if (pagingParams.Id.HasValue)
        {
            query = query.Where(e => e.Id.ToString().StartsWith(pagingParams.Id.Value.ToString()));
        }
        if (!string.IsNullOrEmpty(pagingParams.DepartmentId))
        {
            query = query.Where(e => e.DepartmentId.ToString() == pagingParams.DepartmentId);
        }
        return PagedList<Employee>.ToPagedList(query.OrderBy(emp => emp.Name),
        pagingParams.PageNumber,
        pagingParams.PageSize);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public Task<Employee> UpdateEmployee(long Id, Employee employee)
    {
        throw new NotImplementedException();
    }
}
