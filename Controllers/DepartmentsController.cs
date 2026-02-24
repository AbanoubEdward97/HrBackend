using HrBackend.DTO_S.Departments;
using HrBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enums;
namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "SuperAdmin")]
public class DepartmentsController : ControllerBase
{
    private readonly HrContext _context;
    public DepartmentsController(HrContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize(Enums.Permissions.Departments.View)]
    public async Task<IActionResult> GetAllDepts()
    {
        var depts = await _context.Departments.ToListAsync();
        return Ok(depts);
    }

    [HttpPost]
    [Authorize(Enums.Permissions.Departments.Add)]
    public async Task<IActionResult> AddDept(DeptDTO dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt,
            IsActive = dto.IsActive,

        };
        await _context.AddAsync(department);
        _context.SaveChanges();

        return Ok(department);
    }

    [HttpPut("{id}")]
    [Authorize(Enums.Permissions.Departments.Edit)]
    public async Task<IActionResult> UpdateDept(int id , [FromBody]  DeptDTO dto )
    {
        var dept = await _context.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id);
        if (dept == null)
        {
            return NotFound("DEPARTMENT NOT FOUND");
        }
        dept.Name = dto.Name;
        dept.Description = dto.Description;
        dept.IsActive = dto.IsActive;
        _context.SaveChanges();
        return Ok(dept);
    }
    [HttpDelete("{id}")]
    [Authorize(Enums.Permissions.Departments.Delete)]
    public async Task<IActionResult> DeleteDept(int id)
    {
        var dept = await _context.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id);
        if (dept == null)
        {
            return NotFound("DEPARTMENT NOT FOUND");
        }
        _context.Remove(dept);
        _context.SaveChanges();
        return Ok(dept);
    }
}
