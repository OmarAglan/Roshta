using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Rosheta.Pages.DoctorProfile;

public class EditModel : PageModel, IValidatableObject
{
    private readonly IDoctorService _doctorService;
    private readonly ILicenseService _licenseService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IDoctorService doctorService, ILicenseService licenseService, ISettingsService settingsService, ILogger<EditModel> logger)
    {
        _doctorService = doctorService;
        _licenseService = licenseService;
        _settingsService = settingsService;
        _logger = logger;
    }

    [BindProperty]
    public DoctorProfileInputModel DoctorProfile { get; set; } = new DoctorProfileInputModel();

    [BindProperty]
    public UserSettingsModel UserSettings { get; set; } = new UserSettingsModel();

    // Reusing the same input model structure as Setup
    public class DoctorProfileInputModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(50)]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Contact Phone")]
        public string? ContactPhone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Activation/Setup check should be handled by the filter, but defensive checks are good.
        if (!_licenseService.IsActivated() || !await _licenseService.IsProfileSetupAsync())
        {
            _logger.LogWarning("Attempted to access profile edit page without proper activation or setup.");
            // Redirect to appropriate page (Setup if not set up, Activate if not activated)
            return RedirectToPage(!_licenseService.IsActivated() ? "/Activate" : "/DoctorProfile/Setup");
        }

        int? currentDoctorId = await _licenseService.GetCurrentDoctorIdAsync();
        if (currentDoctorId == null)
        {
            _logger.LogError("Could not retrieve Doctor ID for editing. User might not be properly associated with a profile.");
            // This indicates a potentially inconsistent state
            TempData["ErrorMessage"] = "Could not load your profile. Please contact support.";
            return RedirectToPage("/Index");
        }

        var doctor = await _doctorService.GetDoctorProfileAsync(currentDoctorId.Value);
        if (doctor == null)
        {
            _logger.LogError("Doctor profile not found for ID: {DoctorId} during edit.", currentDoctorId.Value);
            TempData["ErrorMessage"] = "Your profile could not be found.";
            // Should ideally not happen if IsProfileSetup is true, but handle defensively
            return RedirectToPage("/Index");
        }

        // Populate the input model with existing data
        DoctorProfile.Name = doctor.Name;
        DoctorProfile.Specialization = doctor.Specialization;
        DoctorProfile.LicenseNumber = doctor.LicenseNumber;
        DoctorProfile.ContactPhone = doctor.ContactPhone;
        DoctorProfile.ContactEmail = doctor.ContactEmail;

        // Load user settings
        UserSettings = await _settingsService.GetUserSettingsAsync(currentDoctorId.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Re-check activation/setup status on post
        if (!_licenseService.IsActivated() || !await _licenseService.IsProfileSetupAsync())
        {
            return RedirectToPage(!_licenseService.IsActivated() ? "/Activate" : "/DoctorProfile/Setup");
        }

        int? currentDoctorId = await _licenseService.GetCurrentDoctorIdAsync();
        if (currentDoctorId == null)
        {
            _logger.LogError("Could not retrieve Doctor ID for profile update POST.");
            TempData["ErrorMessage"] = "Your session expired or profile is invalid. Please login again.";
            return RedirectToPage("/Index"); // Or login page
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed for Doctor ID: {DoctorId} during profile update.", currentDoctorId.Value);
            return Page(); // Re-display the form with validation errors
        }

        try
        {
            // Map the Input Model to DTO
            var updateDto = new UpdateDoctorProfileDto
            {
                Name = DoctorProfile.Name,
                Specialization = DoctorProfile.Specialization ?? string.Empty,
                LicenseNumber = DoctorProfile.LicenseNumber ?? string.Empty,
                Phone = DoctorProfile.ContactPhone ?? string.Empty,
                Email = DoctorProfile.ContactEmail ?? string.Empty
            };

            bool success = await _doctorService.UpdateDoctorProfileAsync(currentDoctorId.Value, updateDto);

            if (success)
            {
                _logger.LogInformation("Doctor profile updated successfully for Doctor ID: {DoctorId}", currentDoctorId.Value);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                // It's good practice to redirect after POST to prevent re-submission
                // Redirect back to the same page to show the success message and updated data
                return RedirectToPage();
            }
            else
            {
                _logger.LogWarning("UpdateDoctorProfileAsync returned false for Doctor ID: {DoctorId}", currentDoctorId.Value);
                ModelState.AddModelError(string.Empty, "Could not update the profile. The data might be invalid or unchanged.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor profile for Doctor ID: {DoctorId}", currentDoctorId.Value);
            ModelState.AddModelError(string.Empty, "An error occurred while saving the profile. Please try again.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostSaveSettingsAsync()
    {
        // Re-check activation/setup status on post
        if (!_licenseService.IsActivated() || !await _licenseService.IsProfileSetupAsync())
        {
            return RedirectToPage(!_licenseService.IsActivated() ? "/Activate" : "/DoctorProfile/Setup");
        }

        int? currentDoctorId = await _licenseService.GetCurrentDoctorIdAsync();
        if (currentDoctorId == null)
        {
            _logger.LogError("Could not retrieve Doctor ID for settings update POST.");
            TempData["ErrorMessage"] = "Your session expired or profile is invalid. Please login again.";
            return RedirectToPage("/Index");
        }

        // Validate only the settings model
        var settingsValidationResults = new List<ValidationResult>();
        var settingsValidationContext = new ValidationContext(UserSettings, null, null);
        bool isSettingsValid = Validator.TryValidateObject(UserSettings, settingsValidationContext, settingsValidationResults, true);

        if (!isSettingsValid)
        {
            foreach (var validationResult in settingsValidationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    ModelState.AddModelError($"UserSettings.{memberName}", validationResult.ErrorMessage ?? "Invalid value");
                }
            }

            // Reload profile data for display
            var doctor = await _doctorService.GetDoctorProfileAsync(currentDoctorId.Value);
            if (doctor != null)
            {
                DoctorProfile.Name = doctor.Name;
                DoctorProfile.Specialization = doctor.Specialization;
                DoctorProfile.LicenseNumber = doctor.LicenseNumber;
                DoctorProfile.ContactPhone = doctor.ContactPhone;
                DoctorProfile.ContactEmail = doctor.ContactEmail;
            }

            return Page();
        }

        try
        {
            bool success = await _settingsService.SaveUserSettingsAsync(currentDoctorId.Value, UserSettings);

            if (success)
            {
                _logger.LogInformation("Settings updated successfully for Doctor ID: {DoctorId}", currentDoctorId.Value);
                TempData["SuccessMessage"] = "Settings updated successfully!";
                return RedirectToPage();
            }
            else
            {
                _logger.LogWarning("SaveUserSettingsAsync returned false for Doctor ID: {DoctorId}", currentDoctorId.Value);
                ModelState.AddModelError(string.Empty, "Could not update the settings. Please try again.");

                // Reload profile data for display
                var doctor = await _doctorService.GetDoctorProfileAsync(currentDoctorId.Value);
                if (doctor != null)
                {
                    DoctorProfile.Name = doctor.Name;
                    DoctorProfile.Specialization = doctor.Specialization;
                    DoctorProfile.LicenseNumber = doctor.LicenseNumber;
                    DoctorProfile.ContactPhone = doctor.ContactPhone;
                    DoctorProfile.ContactEmail = doctor.ContactEmail;
                }

                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating settings for Doctor ID: {DoctorId}", currentDoctorId.Value);
            ModelState.AddModelError(string.Empty, "An error occurred while saving the settings. Please try again.");

            // Reload profile data for display
            var doctor = await _doctorService.GetDoctorProfileAsync(currentDoctorId.Value);
            if (doctor != null)
            {
                DoctorProfile.Name = doctor.Name;
                DoctorProfile.Specialization = doctor.Specialization;
                DoctorProfile.LicenseNumber = doctor.LicenseNumber;
                DoctorProfile.ContactPhone = doctor.ContactPhone;
                DoctorProfile.ContactEmail = doctor.ContactEmail;
            }

            return Page();
        }
    }

    // IValidatableObject implementation for EditModel
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Rule: Require at least one contact method
        if (string.IsNullOrWhiteSpace(DoctorProfile.ContactPhone) && string.IsNullOrWhiteSpace(DoctorProfile.ContactEmail))
        {
            yield return new ValidationResult(
                "Please provide at least one contact method (Phone or Email).",
                new[] { nameof(DoctorProfile.ContactPhone), nameof(DoctorProfile.ContactEmail) });
        }
    }
}
