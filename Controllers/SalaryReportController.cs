using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using HrApi.Models;
using HrBackend.DTO_S.Employees;
using HrBackend.DTO_S.SalaryReport;
using HrBackend.Models;
using HrBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SalaryReportController : ControllerBase
{
    private readonly ISalaryReportService _SalaryReportService;
    public SalaryReportController(ISalaryReportService salaryReportService)
    {
        _SalaryReportService = salaryReportService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SalaryReportQueryDTO query)
    {
      var rows = await _SalaryReportService.Get(query);
      return Ok(rows);
    }
}
