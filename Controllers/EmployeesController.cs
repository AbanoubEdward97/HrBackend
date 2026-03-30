using AutoMapper;
using HrApi.Models;
using HrBackend.DTO_S.Employees;
using HrBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HrBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //private readonly HrContext _context;
        private readonly IMapper _mapper;
        private readonly IEmployeesService _empService;


        public EmployeesController(HrContext context, IMapper mapper, IEmployeesService empService)
        {
            //_context = context;
            _mapper = mapper;
            _empService = empService;
        }

        // GET: api/Employees
        [Authorize(Enums.Permissions.Employees.View)]
        //[Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = _empService.GetEmployees();//_context.Employees.Include(e => e.Department).AsNoTracking().AsQueryable();
            //.Select(e => new EmpDetailsDTO
            //{
            //    Address = e.Address,
            //    AttendDate = e.AttendDate,
            //    BirthDate = e.BirthDate,
            //    CreatedAt = e.CreatedAt,
            //    DepartmentId = e.DepartmentId,
            //    HireDate = e.HireDate,
            //    GenderName = Enum.GetName(e.Gender),
            //    Id = e.Id,
            //    IsActive = e.IsActive,
            //    LeaveDate = e.LeaveDate,
            //    Name = e.Name,
            //    NationalId = e.NationalId,
            //    Nationality = e.Nationality,
            //    PhoneNumber = e.PhoneNumber,
            //    Salary = e.Salary,
            //    DepartmentName = e.Department.Name

            //}).ToListAsync();
            var data = _mapper.Map<IEnumerable<EmpDetailsDTO>>(employees);
            return Ok(data);
        }

        // GET: api/Employees/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetEmployee(long id)
        {
            var e =await _empService.GetById(id);//await _context.Employees.Include(e=>e.Department).FirstOrDefaultAsync(e=>e.Id == id);

            if (e == null)
            {
                return NotFound();
            }
            var employeeModel = _mapper.Map<EmpDetailsDTO>(e);
            return Ok(employeeModel);
        }

        // PUT: api/Employees/5
        [HttpPut("{Id}")]
        public async Task<IActionResult> EditEmployee(long Id,[FromBody] UpdateEmpDTO dto)
        {
            var emp = _empService.GetById(Id);//await _context.Employees.Include(d => d.Department).SingleOrDefaultAsync(e=> Id ==e.Id);
            if (emp == null)
            {
                return NotFound();
            };
            //if (Id != employee.Id)
            //{
            //    return BadRequest();
            //}
            //emp.AttendDate = employee.AttendDate;
            //emp.LeaveDate = employee.LeaveDate;
            //emp.HireDate = employee.HireDate;
            //emp.BirthDate = employee.BirthDate;
            //emp.Gender = employee.Gender;
            //emp.Address = employee.Address; 
            //emp.PhoneNumber = employee.PhoneNumber;
            //emp.CreatedAt = employee.CreatedAt;
            //emp.Salary = employee.Salary;
            //emp.DepartmentId = employee.DepartmentId;
            //emp.Department = employee.Department;
            //emp.IsActive = employee.IsActive;
            //emp.Name = employee.Name;
            //emp.NationalId = employee.NationalId;
            //emp.Id = employee.Id;
            //emp.Nationality = employee.Nationality;
            _mapper.Map<Employee>(dto);
            await _empService.SaveChanges();//await _context.SaveChangesAsync();
            return Ok(dto);
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmpDTO newEmp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var employee = _mapper.Map<Employee>(newEmp);
            employee = await _empService.AddEmployee(employee);
            return Ok(employee);
            // _context.Employees.Add(employee);
            //await _context.SaveChangesAsync();

            // return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
            //return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        } 

        // DELETE: api/Employees/5
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var employee = await _empService.GetById(id);// await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

             _empService.DeleteEmployee(employee);
            //await _context.SaveChangesAsync();
            await _empService.SaveChanges();
            return Ok(employee);
        }

        //private bool EmployeeExists(long id)
        //{
        //    return _context.Employees.Any(e => e.Id == id);
        //}
    }
}
