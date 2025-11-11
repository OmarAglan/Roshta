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

namespace Roshta.Pages_Medications
{
    public class EditModel : PageModel
    {
        private readonly IMedicationService _medicationService;

        public EditModel(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [BindProperty]
        public Medication Medication { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _medicationService.GetMedicationByIdAsync(id.Value);
            if (medication == null)
            {
                return NotFound();
            }
            Medication = medication;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Optional: Add server-side uniqueness check here too for robustness
            // Pass the current medication's ID to exclude it from the check
            bool isUnique = await _medicationService.IsNameUniqueAsync(Medication.Name, Medication.Id);
            if (!isUnique)
            {
                ModelState.AddModelError("Medication.Name", "Medication name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _medicationService.UpdateMedicationAsync(Medication);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MedicationExistsAsync(Medication.Id)) // Keep existing concurrency check
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Add success message before redirecting
            TempData["SuccessMessage"] = $"Medication '{Medication.Name}' updated successfully.";

            return RedirectToPage("./Index");
        }

        private async Task<bool> MedicationExistsAsync(int id)
        {
            // Should ideally use the service if it provides ExistsAsync
            return await _medicationService.MedicationExistsAsync(id);
        }

        // --- New Handler for AJAX Uniqueness Check ---
        public async Task<IActionResult> OnGetCheckMedicationNameUniqueAsync(string name, int currentId)
        {
            // currentId is passed from the client-side JS for the item being edited
            if (string.IsNullOrWhiteSpace(name))
            {
                return new JsonResult(new { isUnique = true });
            }

            // Pass the currentId to the service check
            bool isUnique = await _medicationService.IsNameUniqueAsync(name.Trim(), currentId);

            return new JsonResult(new { isUnique });
        }
        // --------------------------------------------
    }
}
