using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rosheta.Data;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Services;

namespace Rosheta.Pages_Patients
{
    public class DeleteModel : PageModel
    {
        private readonly IPatientService _patientService;

        public DeleteModel(IPatientService patientService)
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
            else
            {
                Patient = patient;
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
            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            var patientName = patient?.Name ?? $"ID {id.Value}"; // Fallback to ID if name not found

            var deleted = await _patientService.DeletePatientAsync(id.Value);

            if (deleted)
            {
                TempData["SuccessMessage"] = $"Patient '{patientName}' deleted successfully.";
            }
            else
            {
                // This might happen if the patient was deleted between OnGet and OnPost, or DB error
                TempData["ErrorMessage"] = $"Could not delete patient '{patientName}'. Please try again or contact support.";
            }

            return RedirectToPage("./Index");
        }
    }
}
