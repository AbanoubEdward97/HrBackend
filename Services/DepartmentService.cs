using DocumentFormat.OpenXml.Office2010.Excel;
using HrBackend.DTO_S.Departments;
using HrBackend.Models;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Services;

public class DepartmentService : IDepartmentService
{
    private readonly HrContext _context;
    public DepartmentService(HrContext context)
    {
        _context = context;
    }

    public async Task AddDept(Department dept)
    {
        await _context.AddAsync(dept);
    }

    public async Task<bool> DeptExists(DeptDTO dto)
    {
        return await _context.Departments.AnyAsync(d => d.Name == dto.Name);
    }

    public async Task<List<Department>> getAll()
    {
       var depts = await _context.Departments.ToListAsync();
        return depts;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();

    }


    public async Task<Department> GetById(int id)
    {
        var department = await _context.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id);
        return department;
    }
    public Department Remove(Department dept)
    {

        _context.Remove(dept);
        return dept;
    }
}
