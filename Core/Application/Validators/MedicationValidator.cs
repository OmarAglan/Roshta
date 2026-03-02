using FluentValidation;
using Rosheta.Core.Domain.Entities;

namespace Rosheta.Core.Application.Validators;

public class MedicationValidator : AbstractValidator<Medication>
{
    public MedicationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Medication name is required.");
    }
}
