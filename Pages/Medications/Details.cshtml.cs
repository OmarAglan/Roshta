using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roshta.Models.Entities;
using Roshta.Services.Interfaces;
using System.Threading.Tasks;

namespace Roshta.Pages_Medications
{
    public class DetailsModel : PageModel
    {
        private readonly IMedicationService _medicationService;

        public DetailsModel(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

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
    }
}
