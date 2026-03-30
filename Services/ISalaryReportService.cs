using HrBackend.DTO_S.SalaryReport;
using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Services;

public interface ISalaryReportService
{
    Task<List<SalaryReportRowDTO>> Get(SalaryReportQueryDTO query);
}
