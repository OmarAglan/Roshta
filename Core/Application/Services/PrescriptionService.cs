using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
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

        // Validate (basic example - add more robust validation)
        if (!prescription.PrescriptionItems.Any())
        {
            throw new ArgumentException("Prescription must have at least one item.");
        }

        // Save using the repository

        return await _prescriptionRepository.AddAsync(prescription); // Use renamed method
    }

    // Add implementation for GetAllPrescriptionsAsync
    public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
    {
        // Enhance later to include Patient/Doctor info if needed for display
        return await _prescriptionRepository.GetAllAsync(); // Use renamed method
    }

    public async Task<IEnumerable<Prescription>> SearchPrescriptionsAsync(string searchTerm)
    {
        // Implement search logic, potentially involving related entities
        return await _prescriptionRepository.SearchAsync(searchTerm);
    }

    // Add implementation for GetPrescriptionByIdAsync
    public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
    {
        // Enhance later to include related items/patient/doctor
        return await _prescriptionRepository.GetByIdAsync(id); // Use renamed method
    }

    public async Task<bool> CancelPrescriptionAsync(int prescriptionId)
    {
        // Could add business logic here, e.g., check user permissions,
        // or prevent cancellation based on status or time.
        // For now, directly call the repository.
        _logger.LogInformation("Attempting to cancel prescription ID {PrescriptionId}", prescriptionId);
        return await _prescriptionRepository.CancelAsync(prescriptionId);
    }

    // Add Update/Delete service methods later if needed

    // --- Implementation for Pagination Methods ---

    public async Task<List<Prescription>> GetPrescriptionsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        return await _prescriptionRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, sortOrder);
    }

    public async Task<int> GetPrescriptionsCountAsync(string? searchTerm = null)
    {
        return await _prescriptionRepository.GetCountAsync(searchTerm);
    }

    // ---------------------------------------------
}
