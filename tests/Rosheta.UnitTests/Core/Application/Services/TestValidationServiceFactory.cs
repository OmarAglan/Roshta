using FluentValidation;
using FluentValidation.Results;
using Rosheta.Core.Application.Common.Results;
using Rosheta.Core.Application.Common.Validation;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Validators;
using Rosheta.Core.Domain.Entities;

namespace Rosheta.UnitTests.Core.Application.Services;

internal static class TestValidationServiceFactory
{
    public static IValidationService Create() => new TestValidationService();
}

internal sealed class TestValidationService : IValidationService
{
    public async Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
    {
        if (instance == null)
        {
            return Result.Failure("Validation", "The input payload cannot be null.");
        }

        var failures = instance switch
        {
            Doctor doctor => await ValidateWithAsync(new DoctorValidator(), doctor, cancellationToken),
            UpdateDoctorProfileDto dto => await ValidateWithAsync(new UpdateDoctorProfileDtoValidator(), dto, cancellationToken),
            Patient patient => await ValidateWithAsync(new PatientValidator(), patient, cancellationToken),
            Medication medication => await ValidateWithAsync(new MedicationValidator(), medication, cancellationToken),
            PrescriptionCreateModel model => await ValidateWithAsync(new PrescriptionCreateModelValidator(), model, cancellationToken),
            _ => new List<ValidationFailure>()
        };

        if (failures.Count == 0)
        {
            return Result.Success();
        }

        var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        return Result.Failure("Validation", errorMessage);
    }

    private static async Task<List<ValidationFailure>> ValidateWithAsync<T>(
        IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(instance, cancellationToken);
        return validationResult.Errors.Where(e => e != null).ToList();
    }
}
