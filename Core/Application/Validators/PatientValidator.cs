using FluentValidation;
using Rosheta.Core.Domain.Entities;

namespace Rosheta.Core.Application.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Patient name is required.");

        RuleFor(x => x.ContactInfo)
            .NotEmpty()
            .WithMessage("Patient contact information is required.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today)
            .When(x => x.DateOfBirth != default)
            .WithMessage("Date of Birth must be in the past.");

        RuleFor(x => x.LastVisitDate)
            .LessThanOrEqualTo(DateTime.Today)
            .When(x => x.LastVisitDate.HasValue)
            .WithMessage("Last Visit Date cannot be in the future.");
    }
}
