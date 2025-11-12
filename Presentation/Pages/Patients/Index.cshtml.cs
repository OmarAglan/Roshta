using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rosheta.Infrastructure.Data;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using Microsoft.AspNetCore.Mvc.Rendering; // Keep if needed elsewhere, otherwise remove

namespace Rosheta.Presentation.Pages_Patients
{
    public class IndexModel : PageModel
    {
        private readonly IPatientService _patientService;
        private const int PageSize = 10; // Define page size

        public IndexModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public List<Patient> Patient { get; set; } = new List<Patient>(); // Initialize as empty list

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1; // Default to page 1

        public int TotalPages { get; set; }
        public int Count { get; set; }

        // --- Sorting Properties ---
        [BindProperty(SupportsGet = true)]
        public string? CurrentSort { get; set; } // Holds the current sort order parameter
        public string? NameSort { get; set; }
        public string? DateSort { get; set; }
        public string? VisitDateSort { get; set; }
        // -------------------------

        public async Task OnGetAsync(string? sortOrder) // Add sortOrder parameter
        {
            CurrentSort = sortOrder; // Store the incoming sort order

            // --- Determine next sort order for links ---
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; // Default sort is Name Asc, so next is Desc
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            VisitDateSort = sortOrder == "VisitDate" ? "visitdate_desc" : "VisitDate";
            // -------------------------------------------

            // Ensure CurrentPage is at least 1
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            // Get total count for pagination calculation
            Count = await _patientService.GetPatientsCountAsync(SearchString);
            TotalPages = (int)Math.Ceiling(Count / (double)PageSize);

            // Ensure CurrentPage is not beyond the last page
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }

            // Get the paged data, passing the current sort order
            Patient = await _patientService.GetPatientsPagedAsync(CurrentPage, PageSize, SearchString, CurrentSort);

            // Note: The old logic using SearchPatientsAsync/GetAllPatientsAsync is replaced by the paged call.
        }

        public async Task<IActionResult> OnGetSearchAsync(string searchTerm)
        {
            try
            {
                // Handle empty/null search terms
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new JsonResult(new List<PatientSearchDto>());
                }

                // Get patients using existing service method, limit to 10 for autocomplete
                var patients = await _patientService.GetPatientsPagedAsync(1, 10, searchTerm, null);

                // Map to DTO
                var patientDtos = patients.Select(p => new PatientSearchDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ContactInfo = p.ContactInfo
                }).ToList();

                return new JsonResult(patientDtos);
            }
            catch (Exception)
            {
                // Return empty result on error
                return new JsonResult(new List<PatientSearchDto>());
            }
        }
    }
}
