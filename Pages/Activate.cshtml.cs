using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rosheta.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Rosheta.Pages;

public class ActivateModel : PageModel
{
    private readonly ILicenseService _licenseService;
    private readonly ILogger<ActivateModel> _logger; // Optional: For logging errors

    public ActivateModel(ILicenseService licenseService, ILogger<ActivateModel> logger)
    {
        _licenseService = licenseService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public class InputModel
    {
        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; } = string.Empty;
    }

    public IActionResult OnGet()
    {
        // If already activated, redirect away from the activation page
        if (_licenseService.IsActivated())
        {
            _logger.LogInformation("Application already activated. Redirecting from Activate page.");
            // Redirect to the main page or dashboard (e.g., home page for now)
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (_licenseService.IsActivated())
        {
            _logger.LogInformation("Application already activated. Redirecting from Activate page post.");
            return RedirectToPage("/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool isValid = _licenseService.ValidateLicense(Input.RegistrationNumber, Input.SerialNumber);

        if (isValid)
        {
            _logger.LogInformation("License validation successful. Marking as activated.");
            await _licenseService.MarkAsActivatedAsync();
            TempData["SuccessMessage"] = "Application activated successfully! Please complete your profile.";
            // Redirect to the profile setup page now
            return RedirectToPage("/DoctorProfile/Setup");
        }
        else
        {
            _logger.LogWarning("License validation failed for Reg: {RegistrationNumber}", Input.RegistrationNumber);
            ModelState.AddModelError(string.Empty, "Invalid Registration Number or Serial Number. Please check your license details.");
            return Page();
        }
    }
}
