using FluentValidation;
using Rosheta.Core.Domain.Entities;

namespace Rosheta.Core.Application.Validators;

public class DoctorValidator : AbstractValidator<Doctor>
{
    public DoctorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Doctor name is required.");

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Doctor specialization is required.");
    }
}
