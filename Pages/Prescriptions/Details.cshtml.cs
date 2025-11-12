using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Roshta.Models.Entities;
using Roshta.Services.Interfaces;

namespace Rosheta.Pages.Prescriptions;

public class DetailsModel : PageModel
{
    private readonly IPrescriptionService _prescriptionService;

    public DetailsModel(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    public Prescription? Prescription { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // GetPrescriptionByIdAsync in the service/repo should handle including related data
        Prescription = await _prescriptionService.GetPrescriptionByIdAsync(id.Value);

        if (Prescription == null)
        {
            return NotFound();
        }
        return Page();
    }
}
