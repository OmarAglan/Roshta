using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Common.Persistence;
using Rosheta.Core.Application.Common.Results;
using Rosheta.Core.Application.Common.Validation;
using Microsoft.Extensions.Logging;
using Rosheta.Core.Domain.Enums;

namespace Rosheta.Core.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PrescriptionService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidationService _validationService;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository,
                               IPatientRepository patientRepository,
                               ILogger<PrescriptionService> logger)
        : this(prescriptionRepository, patientRepository, logger, new NoOpUnitOfWork(), new NoOpValidationService())
    {
    }

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        IPatientRepository patientRepository,
        ILogger<PrescriptionService> logger,
        IUnitOfWork unitOfWork,
        IValidationService validationService)
    {
        _prescriptionRepository = prescriptionRepository;
        _patientRepository = patientRepository; 
        _logger = logger;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<Result<Prescription>> CreatePrescriptionResultAsync(PrescriptionCreateModel model, int doctorId)
    {
        var validation = await _validationService.ValidateAsync(model);
        if (validation.IsFailure)
        {
            return Result<Prescription>.Failure(validation.ErrorCode, validation.ErrorMessage);
        }

        var prescription = new Prescription
        {
            PatientId = model.PatientId,
            DoctorId = doctorId,
            DateIssued = DateTime.UtcNow,
            ExpiryDate = model.ExpiryDate,
            NextAppointmentDate = model.NextAppointmentDate,
            Status = PrescriptionStatus.Active,
            PrescriptionItems = new List<PrescriptionItem>()
        };

        foreach (var itemModel in model.Items)
        {
            if (itemModel.MedicationId > 0 && !string.IsNullOrWhiteSpace(itemModel.Instructions))
            {
                prescription.PrescriptionItems.Add(new PrescriptionItem
                {
                    MedicationId = itemModel.MedicationId,
                    Dosage = itemModel.Dosage,
                    Frequency = itemModel.Frequency,
                    Duration = itemModel.Duration,
                    Instructions = itemModel.Instructions,
                    Quantity = itemModel.Quantity,
                    Refills = itemModel.Refills,
                    Notes = itemModel.Notes,
                    Prescription = prescription
                });
            }
        }

        if (!await _patientRepository.ExistsAsync(prescription.PatientId))
        {
            return Result<Prescription>.Failure("NotFound", $"Entity \"Patient\" with key ({prescription.PatientId}) was not found.");
        }

        try
        {
            var created = await _prescriptionRepository.AddAsync(prescription);
            await _unitOfWork.SaveChangesAsync();
            return Result<Prescription>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<Prescription>.Failure("Infrastructure", $"Failed to create prescription. {ex.Message}");
        }
    }

    public async Task<Prescription?> CreatePrescriptionAsync(PrescriptionCreateModel model, int doctorId)
    {
        var result = await CreatePrescriptionResultAsync(model, doctorId);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw ToException(result);
    }

    public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
    {
        try
        {
            return await _prescriptionRepository.GetAllAsync();
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve prescriptions.", ex);
        }
    }

    public async Task<IEnumerable<Prescription>> SearchPrescriptionsAsync(string searchTerm)
    {
        try
        {
            return await _prescriptionRepository.SearchAsync(searchTerm);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to search prescriptions.", ex);
        }
    }

    public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
    {
        try
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
            {
                throw new NotFoundException(nameof(Prescription), id);
            }

            return prescription;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve prescription.", ex);
        }
    }

    public async Task<bool> CancelPrescriptionAsync(int prescriptionId)
    {
        var result = await CancelPrescriptionResultAsync(prescriptionId);
        if (result.IsSuccess)
        {
            return true;
        }

        throw ToException(result);
    }

    public async Task<Result> CancelPrescriptionResultAsync(int prescriptionId)
    {
        _logger.LogInformation("Attempting to cancel prescription ID {PrescriptionId}", prescriptionId);

        var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
        if (prescription == null)
        {
            return Result.Failure("NotFound", $"Entity \"Prescription\" with key ({prescriptionId}) was not found.");
        }
        if (prescription.Status == PrescriptionStatus.Cancelled)
        {
            return Result.Failure("BusinessRule", "Cannot cancel a prescription that is already cancelled.");
        }
        if (prescription.Status == PrescriptionStatus.Filled)
        {
            return Result.Failure("BusinessRule", "Cannot cancel a prescription that has been filled.");
        }

        try
        {
            var canceled = await _prescriptionRepository.CancelAsync(prescriptionId);
            if (!canceled)
            {
                return Result.Failure("NotFound", $"Entity \"Prescription\" with key ({prescriptionId}) was not found.");
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Infrastructure", $"Failed to cancel prescription. {ex.Message}");
        }
    }

    // Add Update/Delete service methods later if needed

    // --- Implementation for Pagination Methods ---

    public async Task<List<Prescription>> GetPrescriptionsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        try
        {
            return await _prescriptionRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, sortOrder);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve paged prescriptions.", ex);
        }
    }

    public async Task<int> GetPrescriptionsCountAsync(string? searchTerm = null)
    {
        try
        {
            return await _prescriptionRepository.GetCountAsync(searchTerm);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to count prescriptions.", ex);
        }
    }

    // ---------------------------------------------

    private static Exception ToException(Result result)
    {
        if (result.ErrorCode == "Validation")
        {
            return new ValidationException(result.ErrorMessage);
        }
        if (result.ErrorCode == "BusinessRule")
        {
            return new BusinessRuleException(result.ErrorMessage);
        }
        if (result.ErrorCode == "NotFound")
        {
            return new NotFoundException(result.ErrorMessage);
        }

        return new InfrastructureException(result.ErrorMessage);
    }
}
