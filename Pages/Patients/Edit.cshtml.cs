using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Roshta.Data;
using Roshta.Models.Entities;
using Roshta.Services.Interfaces;

namespace Roshta.Pages_Patients
{
    public class EditModel : PageModel
    {
        private readonly IPatientService _patientService;

        public EditModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null)
            {
                return NotFound();
            }
            Patient = patient;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Robustness check: Server-side uniqueness validation
            if (Patient != null)
            {
                if (!string.IsNullOrWhiteSpace(Patient.ContactInfo))
                {
                    // Pass the current patient's ID to exclude it from the check
                    bool isContactUnique = await _patientService.IsContactInfoUniqueAsync(Patient.ContactInfo, Patient.Id);
                    if (!isContactUnique)
                    {
                        ModelState.AddModelError("Patient.ContactInfo", "Contact Info already exists.");
                    }
                }
                if (!string.IsNullOrWhiteSpace(Patient.Name))
                {
                    bool isNameUnique = await _patientService.IsNameUniqueAsync(Patient.Name, Patient.Id);
                    if (!isNameUnique)
                    {
                        ModelState.AddModelError("Patient.Name", "Patient Name already exists.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _patientService.UpdatePatientAsync(Patient);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PatientExistsAsync(Patient.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Add success message before redirecting
            TempData["SuccessMessage"] = $"Patient '{Patient.Name}' updated successfully.";

            return RedirectToPage("./Index");
        }

        private async Task<bool> PatientExistsAsync(int id)
        {
            return await _patientService.PatientExistsAsync(id);
        }

        // --- Handler for ContactInfo AJAX Uniqueness Check ---
        public async Task<IActionResult> OnGetCheckContactInfoUniqueAsync(string contactInfo, int currentId)
        {
            // currentId is passed from the client-side JS for the item being edited
            if (string.IsNullOrWhiteSpace(contactInfo))
            {
                return new JsonResult(new { isUnique = true });
            }
            // Pass the currentId to the service check
            bool isUnique = await _patientService.IsContactInfoUniqueAsync(contactInfo.Trim(), currentId);
            return new JsonResult(new { isUnique });
        }
        // --------------------------------------------------

        // --- Handler for Name AJAX Uniqueness Check ---
        public async Task<IActionResult> OnGetCheckNameUniqueAsync(string name, int currentId)
        {
            // currentId is passed from the client-side JS for the item being edited
            if (string.IsNullOrWhiteSpace(name))
            {
                return new JsonResult(new { isUnique = true });
            }
            bool isUnique = await _patientService.IsNameUniqueAsync(name.Trim(), currentId);
            return new JsonResult(new { isUnique });
        }
        // --------------------------------------------
    }
}
