using FluentValidation;
using HrBackend.DTO_S.Attendance;
using Humanizer;

namespace HrBackend.Validators;

public class AttendanceUpsertValidator: AbstractValidator<AttendanceUpsertDTO>
{
    public AttendanceUpsertValidator()
    {
        //if (dto.CheckIn.HasValue && dto.CheckOut.HasValue && dto.CheckOut < dto.CheckIn)
        //{
        //    return BadRequest("checkIn date must be less than checkOut date");
        //}

        RuleFor(x => x.CheckOut)
          .GreaterThanOrEqualTo(x => x.CheckIn)
          .When(x => x.CheckIn.HasValue && x.CheckOut.HasValue)
          .WithMessage("Check In must be less than Check out time");
    }
}
