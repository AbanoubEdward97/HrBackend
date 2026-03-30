using FluentValidation;
using HrBackend.DTO_S.SalaryReport;

namespace HrBackend.Validators;

public class SalaryReportQueryValidator : AbstractValidator<SalaryReportQueryDTO>
{
    public SalaryReportQueryValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Invalid Month Value");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2008)
            .WithMessage("Year cannot be less than 2008");

    }
}
