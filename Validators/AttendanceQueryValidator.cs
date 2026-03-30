using FluentValidation;
using HrBackend.DTO_S.Attendance;

namespace HrBackend.Validators;

public class AttendanceQueryValidator : AbstractValidator<AttendanceQueryDTO>
{
    public AttendanceQueryValidator()
    {
        //if (query.From.HasValue && query.To.HasValue && query.From > query.To)
        //{
        //    return BadRequest("From Date cannot be greater than To date");
        //}

        RuleFor(q => q)
            .Must(q => !q.From.HasValue || !q.To.HasValue || q.From.Value < q.To.Value)
            .WithMessage("From Date must be smaller than To Date");

    }
}
