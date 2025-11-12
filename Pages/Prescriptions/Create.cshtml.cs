using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using Roshta.Models.Entities;
using Roshta.Repositories.Interfaces;
using Roshta.Services.Interfaces;
using Roshta.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace Roshta.Pages.Prescriptions
{
    public class CreateModel : PageModel
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicationRepository _medicationRepository;
        private readonly ILicenseService _licenseService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IPrescriptionService prescriptionService,
            IPatientRepository patientRepository,
            IMedicationRepository medicationRepository,
            ILicenseService licenseService,
            ILogger<CreateModel> logger)
        {
            _prescriptionService = prescriptionService;
            _patientRepository = patientRepository;
            _medicationRepository = medicationRepository;
            _licenseService = licenseService;
            _logger = logger;
        }

        // Properties to hold data for the form
        [BindProperty]
        public PrescriptionCreateModel PrescriptionCreate { get; set; } = new PrescriptionCreateModel();

        public SelectList? PatientSelectList { get; set; }
        public SelectList? MedicationSelectList { get; set; }

        public async Task OnGetAsync(int? copyFromId)
        {
            // Populate SelectLists for dropdowns (always needed)
            var patients = await _patientRepository.GetAllAsync();
            PatientSelectList = new SelectList(patients.OrderBy(p => p.Name), nameof(Patient.Id), nameof(Patient.Name));

            var medications = await _medicationRepository.GetAllAsync();
            MedicationSelectList = new SelectList(medications.OrderBy(m => m.Name), nameof(Medication.Id), nameof(Medication.Name));

            // If copying from an existing prescription
            if (copyFromId.HasValue)
            {
                _logger.LogInformation("Attempting to copy prescription ID {CopyFromId}", copyFromId.Value);
                // Fetch the original prescription - GetByIdAsync should include items
                var originalPrescription = await _prescriptionService.GetPrescriptionByIdAsync(copyFromId.Value);

                if (originalPrescription != null)
                {
                    // Pre-populate the ViewModel
                    PrescriptionCreate.PatientId = originalPrescription.PatientId;
                    // ExpiryDate and NextAppointmentDate could be copied or left blank - let's copy for now
                    PrescriptionCreate.ExpiryDate = originalPrescription.ExpiryDate;
                    PrescriptionCreate.NextAppointmentDate = originalPrescription.NextAppointmentDate;

                    // Map the original items to the ViewModel items
                    PrescriptionCreate.Items = originalPrescription.PrescriptionItems.Select(item => new PrescriptionCreateModel.PrescriptionItemCreateModel
                    {
                        MedicationId = item.MedicationId,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        Duration = item.Duration,
                        Quantity = item.Quantity,
                        Instructions = item.Instructions,
                        Refills = item.Refills,
                        Notes = item.Notes
                    }).ToList();
                    _logger.LogInformation("Successfully pre-populated form from prescription ID {CopyFromId}", copyFromId.Value);
                }
                else
                {
                    _logger.LogWarning("Could not find prescription ID {CopyFromId} to copy.", copyFromId.Value);
                    // Optional: Add a TempData message if needed
                    // TempData["ErrorMessage"] = "Could not find the prescription to copy.";
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // --- Pre-check selected Patient ID --- 
            if (!await _patientRepository.ExistsAsync(PrescriptionCreate.PatientId))
            {
                ModelState.AddModelError("PrescriptionCreate.PatientId", "Selected patient does not exist.");
            }

            // --- Pre-check selected Medication IDs --- 
            if (PrescriptionCreate.Items != null)
            {
                // Ensure we have the select list available for looking up names
                if (MedicationSelectList == null) { await OnGetAsync(null); } // Pass null to avoid re-copying if validation fails

                for (int i = 0; i < PrescriptionCreate.Items.Count; i++)
                {
                    var item = PrescriptionCreate.Items[i];
                    if (!await _medicationRepository.ExistsAsync(item.MedicationId))
                    {
                        var medName = MedicationSelectList?.FirstOrDefault(m => m.Value == item.MedicationId.ToString())?.Text;
                        var errorMsg = medName != null
                            ? $"Selected medication '{medName}' no longer exists or is invalid."
                            : $"Selected medication (ID: {item.MedicationId}) does not exist.";
                        ModelState.AddModelError($"PrescriptionCreate.Items[{i}].MedicationId", errorMsg);
                    }
                }
            }

            // --- Check Model State AFTER custom checks --- 
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed during prescription creation.");
                await OnGetAsync(null); // Re-populate lists, pass null to copyFromId
                return Page();
            }

            // --- Get Doctor ID from License Service ---
            int? currentDoctorId = await _licenseService.GetCurrentDoctorIdAsync();
            if (currentDoctorId == null)
            {
                _logger.LogError("Could not retrieve Doctor ID for prescription creation. Profile might not be set up.");
                ModelState.AddModelError(string.Empty, "Cannot create prescription. Doctor profile not found.");
                await OnGetAsync(null); // Re-populate lists
                return Page();
            }
            // ---------------------------------------------------

            try
            {
                var createdPrescription = await _prescriptionService.CreatePrescriptionAsync(PrescriptionCreate, currentDoctorId.Value);
                _logger.LogInformation("Prescription created successfully with ID {PrescriptionId}", createdPrescription.Id);
                TempData["SuccessMessage"] = "Prescription created successfully!";
                return RedirectToPage("./Index");
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument during prescription creation for Patient ID {PatientId}", PrescriptionCreate.PatientId);
                ModelState.AddModelError(string.Empty, argEx.Message);
                await OnGetAsync(null); // Re-populate lists
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription for Patient ID {PatientId}", PrescriptionCreate.PatientId);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the prescription. Please try again or contact support.");
                await OnGetAsync(null); // Re-populate lists
                return Page();
            }
        }

        // AJAX Handler to check if a medication ID exists
        public async Task<IActionResult> OnGetCheckMedicationExistsAsync(int id)
        {
            bool exists = await _medicationRepository.ExistsAsync(id);
            return new JsonResult(new { exists });
        }
    }
}