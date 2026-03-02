using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Services;
using System.Threading.Tasks;

namespace Rosheta.Pages_Medications
{
    public class DeleteModel : PageModel
    {
        private readonly IMedicationService _medicationService;

        public DeleteModel(IMedicationService medicationService)
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
            else
            {
                Medication = medication;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Store the name before deleting for the message
            var medication = await _medicationService.GetMedicationByIdAsync(id.Value);
            var medicationName = medication?.Name ?? $"ID {id.Value}"; // Fallback to ID

            var deleteResult = await _medicationService.DeleteMedicationResultAsync(id.Value);

            if (deleteResult.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Medication '{medicationName}' deleted successfully.";
            }
            else
            {
                // This might happen if the medication was deleted between OnGet and OnPost, or DB error
                TempData["ErrorMessage"] = deleteResult.ErrorMessage;
            }

            return RedirectToPage("./Index");
        }
    }
}
