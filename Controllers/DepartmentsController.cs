using HrBackend.DTO_S.Departments;
using HrBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enums;
using AutoMapper;
using HrBackend.Services;
namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "SuperAdmin")]
public class DepartmentsController : ControllerBase
{
    //private readonly HrContext _context;
    public readonly IMapper _mapper;
    private readonly IDepartmentService _departmentService;
    public DepartmentsController(/**HrContext context ,**/ IMapper mapper, IDepartmentService departmentService)
    {
       // _context = context;
        _mapper = mapper;
        _departmentService = departmentService;
    }
    [HttpGet]
    //[Authorize(Enums.Permissions.Departments.View)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDepts()
    {
        //var depts = await _context.Departments.ToListAsync();
        var depts = await _departmentService.getAll();
        return Ok(depts);
    }

    [HttpPost]
    [Authorize(Enums.Permissions.Departments.Add)]
    public async Task<IActionResult> AddDept(DeptDTO dto)
    {
        var exists = await _departmentService.DeptExists(dto);
        if (exists)
        {
            return BadRequest("Department With this Name Already added");
        }
        var department = _mapper.Map<Department>(dto);
        //var department = new Department
        //{
        //    Name = dto.Name,
        //    Description = dto.Description,
        //    CreatedAt = dto.CreatedAt,
        //    IsActive = dto.IsActive,

        //};

        //await _context.AddAsync(department);

        await _departmentService.AddDept(department);
        // _context.SaveChanges();
        await _departmentService.SaveChangesAsync();
        return Ok(department);
    }
    [HttpPut("{id}")]
    [Authorize(Enums.Permissions.Departments.Edit)]

    public async Task<IActionResult> UpdateDept(int id , [FromBody]  UpdateDeptDTO dto )
    {
        //var dept = await _context.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id);
        var dept = await _departmentService.GetById(id);
        if (dept == null)
        {
            return NotFound("Department not found");
        }
        _mapper.Map(dto, dept);
        //dept.Name = dto.Name;
        //dept.Description = dto.Description;
        //dept.IsActive = dto.IsActive;
        //_context.SaveChanges();
        await _departmentService.SaveChangesAsync();
        return NoContent();
    }
    [AllowAnonymous]
    [HttpDelete("{id}")]
    //[Authorize(Enums.Permissions.Departments.Delete)]
    public async Task<IActionResult> DeleteDept(int id)
    {
        //var dept = await _context.Departments.SingleOrDefaultAsync(d => d.DepartmentId == id);
        var dept = await _departmentService.GetById(id);
        if (dept == null)
        {
            return NotFound("DEPARTMENT NOT FOUND");
        }

        // _context.SaveChanges();
        _departmentService.Remove(dept);
        await _departmentService.SaveChangesAsync();
        return Ok(dept);
    }
}
