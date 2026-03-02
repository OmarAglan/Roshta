using FluentValidation;
using Rosheta.Core.Application.DTOs;

namespace Rosheta.Core.Application.Validators;

public class PrescriptionCreateModelValidator : AbstractValidator<PrescriptionCreateModel>
{
    public PrescriptionCreateModelValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0)
            .WithMessage("Please select a patient.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("The prescription must contain at least one medication item.");

        RuleForEach(x => x.Items)
            .SetValidator(new PrescriptionItemCreateModelValidator());

        RuleFor(x => x.ExpiryDate)
            .Must(x => !x.HasValue || x.Value.Date > DateTime.Today)
            .WithMessage("Expiry Date must be in the future.");

        RuleFor(x => x.NextAppointmentDate)
            .Must(x => !x.HasValue || x.Value.Date > DateTime.Today)
            .WithMessage("Next Appointment Date must be in the future.");
    }
}

public class PrescriptionItemCreateModelValidator : AbstractValidator<PrescriptionCreateModel.PrescriptionItemCreateModel>
{
    public PrescriptionItemCreateModelValidator()
    {
        RuleFor(x => x.MedicationId)
            .GreaterThan(0)
            .WithMessage("Please select a medication.");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required.");

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .WithMessage("Instructions are required.");
    }
}
