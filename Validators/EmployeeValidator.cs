using System;
using FluentValidation;
using NuGet.Common;
public class EmployeeValidator : AbstractValidator<AddEmpDTO>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.Name)
        .Must(IsTitleCase)
        .WithMessage("Name must be in title case (e.g., 'John Doe').");

        RuleFor(x => x.BirthDate)
        .NotEmpty()
        .WithMessage("BirthDate is Required")
        .Must(olderThan18)
        .WithMessage("Employee Must be At Least 18 Years Old.")
        .Must(YoungerThan60)
        .WithMessage("Employee Must be Younger than 18 Years Old.");
    }

    private bool IsTitleCase(string Name)
    {
        if (String.IsNullOrWhiteSpace(Name))
        {
            return false;
        }
        var Words = Name.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in Words)
        {
            if (!char.IsUpper(word[0]))
            {
                return false;
            }
            for (int i = 1; i < word.Length; i++)
            {
                if (!char.IsLetter(word[i]) || !char.IsLower(word[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool olderThan18(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthDate.Year;

        if (today.AddYears(-age) < birthDate ) --age;
        return age >=18;
    }

    private bool YoungerThan60(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthDate.Year;
        if (today.AddYears(-age) < birthDate ) --age;
        return age <= 60;
    }
}