using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rosheta.Data;
using Rosheta.Models.Entities;
using Rosheta.Services.Interfaces;

namespace Rosheta.Pages_Patients
{
    public class DetailsModel : PageModel
    {
        private readonly IPatientService _patientService;

        public DetailsModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

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
    }
}
