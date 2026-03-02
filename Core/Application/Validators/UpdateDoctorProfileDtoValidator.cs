using FluentValidation;
using Rosheta.Core.Application.DTOs.Doctor;

namespace Rosheta.Core.Application.Validators;

public class UpdateDoctorProfileDtoValidator : AbstractValidator<UpdateDoctorProfileDto>
{
    public UpdateDoctorProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Doctor name is required.");

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Doctor specialization is required.");
    }
}
