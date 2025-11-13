using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using Rosheta.Core.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Rosheta.Core.Domain.Enums;

namespace Rosheta.Core.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IPatientRepository _patientRepository; // To validate PatientId
    // private readonly IDoctorRepository _doctorRepository; // Needed later when we have Doctor repo
    // Potentially IMedicationRepository if validation is needed here
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository,
                               IPatientRepository patientRepository,
                               ILogger<PrescriptionService> logger)
    {
        _prescriptionRepository = prescriptionRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    public async Task<Prescription?> CreatePrescriptionAsync(PrescriptionCreateModel model, int doctorId)
    {
        // Map ViewModel to Model
        var prescription = new Prescription
        {
            PatientId = model.PatientId,
            DoctorId = doctorId, // Use the ID passed from the license service
            DateIssued = DateTime.UtcNow, // Set issue date on creation
            ExpiryDate = model.ExpiryDate,
            NextAppointmentDate = model.NextAppointmentDate,
            Status = PrescriptionStatus.Active, // Default status
            PrescriptionItems = new List<PrescriptionItem>()
        };

        // Add PrescriptionItems
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
                    Prescription = prescription // Link back to parent
                });
            }
        }

        // Validate
        if (!prescription.PrescriptionItems.Any())
        {
            throw new ValidationException("Prescription must have at least one medication item.");
        }

        // Verify patient exists
        if (!await _patientRepository.ExistsAsync(prescription.PatientId))
        {
            throw new NotFoundException(nameof(Patient), prescription.PatientId);
        }

        try
        {
            return await _prescriptionRepository.AddAsync(prescription);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to create prescription.", ex);
        }
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
        _logger.LogInformation("Attempting to cancel prescription ID {PrescriptionId}", prescriptionId);

        // Verify prescription exists and get its status
        var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);

        if (prescription == null)
        {
            throw new NotFoundException(nameof(Prescription), prescriptionId);
        }

        // Business rule: can't cancel already cancelled prescriptions
        if (prescription.Status == PrescriptionStatus.Cancelled)
        {
            throw new BusinessRuleException("Cannot cancel a prescription that is already cancelled.");
        }

        // Business rule: can't cancel filled prescriptions
        if (prescription.Status == PrescriptionStatus.Filled)
        {
            throw new BusinessRuleException("Cannot cancel a prescription that has been filled.");
        }

        try
        {
            return await _prescriptionRepository.CancelAsync(prescriptionId);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to cancel prescription.", ex);
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
}
