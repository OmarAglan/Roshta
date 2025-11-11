using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roshta.Models.Entities;
using Roshta.Services.Interfaces;
using System.Threading.Tasks;
using System; // Added for StringComparison

namespace Roshta.Pages_Medications
{
    public class CreateModel : PageModel
    {
        private readonly IMedicationService _medicationService;

        public CreateModel(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Medication Medication { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Optional: Add server-side uniqueness check here too for robustness
            bool isUnique = await _medicationService.IsNameUniqueAsync(Medication.Name);
            if (!isUnique)
            {
                ModelState.AddModelError("Medication.Name", "Medication name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var createdMedication = await _medicationService.AddMedicationAsync(Medication);

            // Add success message
            TempData["SuccessMessage"] = $"Medication '{createdMedication.Name}' created successfully.";

            return RedirectToPage("./Index");
        }

        // --- New Handler for AJAX Uniqueness Check ---
        public async Task<IActionResult> OnGetCheckMedicationNameUniqueAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                // Avoid checking empty strings, assume unique or let Required handle it
                return new JsonResult(new { isUnique = true });
            }

            // In the Create scenario, we don't have a current ID to exclude.
            // The service method should handle the case where currentId is null.
            bool isUnique = await _medicationService.IsNameUniqueAsync(name.Trim());

            return new JsonResult(new { isUnique });
        }
        // --------------------------------------------
    }
}
