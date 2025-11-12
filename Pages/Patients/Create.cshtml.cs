using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Roshta.Data;
using Roshta.Models.Entities;
using Roshta.Services.Interfaces;

namespace Rosheta.Pages_Patients
{
    public class CreateModel : PageModel
    {
        private readonly IPatientService _patientService;

        public CreateModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Robustness check: Server-side uniqueness validation
            if (Patient != null)
            {
                if (!string.IsNullOrWhiteSpace(Patient.ContactInfo))
                {
                    bool isContactUnique = await _patientService.IsContactInfoUniqueAsync(Patient.ContactInfo);
                    if (!isContactUnique)
                    {
                        ModelState.AddModelError("Patient.ContactInfo", "Contact Info already exists.");
                    }
                }
                if (!string.IsNullOrWhiteSpace(Patient.Name))
                {
                    bool isNameUnique = await _patientService.IsNameUniqueAsync(Patient.Name);
                    if (!isNameUnique)
                    {
                        ModelState.AddModelError("Patient.Name", "Patient Name already exists.");
                    }
                }
            }

            if (!ModelState.IsValid || Patient == null)
            {
                return Page();
            }

            var createdPatient = await _patientService.AddPatientAsync(Patient);

            // Add success message
            TempData["SuccessMessage"] = $"Patient '{createdPatient.Name}' created successfully.";

            return RedirectToPage("./Index");
        }

        // --- Handler for ContactInfo AJAX Uniqueness Check ---
        public async Task<IActionResult> OnGetCheckContactInfoUniqueAsync(string contactInfo)
        {
            if (string.IsNullOrWhiteSpace(contactInfo))
            {
                return new JsonResult(new { isUnique = true });
            }
            bool isUnique = await _patientService.IsContactInfoUniqueAsync(contactInfo.Trim());
            return new JsonResult(new { isUnique });
        }
        // --------------------------------------------------

        // --- Handler for Name AJAX Uniqueness Check ---
        public async Task<IActionResult> OnGetCheckNameUniqueAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new JsonResult(new { isUnique = true });
            }
            bool isUnique = await _patientService.IsNameUniqueAsync(name.Trim());
            return new JsonResult(new { isUnique });
        }
        // --------------------------------------------
    }
}
