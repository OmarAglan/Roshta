using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rosheta.Models.Entities;
using Rosheta.Services.Interfaces;
using Rosheta.ViewModels;
using Microsoft.Extensions.Logging;
using System; // Added for Math.Ceiling
using System.Collections.Generic; // Added for List<>
using System.Linq; // Added for ToList()
using System.Threading.Tasks; // Added for Task

namespace Rosheta.Pages.Prescriptions;

public class IndexModel : PageModel
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly ILogger<IndexModel> _logger;
    private const int PageSize = 10; // Define page size

    public IndexModel(IPrescriptionService prescriptionService, ILogger<IndexModel> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }

    public List<Prescription> PrescriptionList { get; set; } = new List<Prescription>(); // Initialize

    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1; // Default to page 1

    public int TotalPages { get; set; }
    public int Count { get; set; }

    // --- Sorting Properties ---
    [BindProperty(SupportsGet = true)]
    public string? CurrentSort { get; set; }
    public string? NameSort { get; set; } // Patient Name
    public string? DateSort { get; set; } // Date Issued
    // Add properties for other sortable columns (Doctor, Status, Expiry) if needed later
    // -------------------------

    public async Task OnGetAsync(string? sortOrder) // Add sortOrder parameter
    {
        CurrentSort = sortOrder;

        // --- Determine next sort order for links ---
        // Default sort is DateIssued Descending
        NameSort = sortOrder == "Name" ? "name_desc" : "Name";
        DateSort = sortOrder == "Date" ? "date_desc" : "Date";
        // -------------------------------------------

        // Ensure CurrentPage is at least 1
        if (CurrentPage < 1)
        {
            CurrentPage = 1;
        }

        // Get total count for pagination calculation
        Count = await _prescriptionService.GetPrescriptionsCountAsync(SearchString);
        TotalPages = (int)Math.Ceiling(Count / (double)PageSize);

        // Ensure CurrentPage is not beyond the last page
        if (CurrentPage > TotalPages && TotalPages > 0)
        {
            CurrentPage = TotalPages;
        }

        // Get the paged data, passing the current sort order
        PrescriptionList = await _prescriptionService.GetPrescriptionsPagedAsync(CurrentPage, PageSize, SearchString, CurrentSort);

        // Note: The old logic using SearchPrescriptionsAsync/GetAllPrescriptionsAsync is replaced.
    }

    public async Task<IActionResult> OnGetSearchAsync(string searchTerm)
    {
        try
        {
            // Handle empty/null search terms
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new JsonResult(new List<PrescriptionSearchDto>());
            }

            // Get prescriptions using existing service method, limit to 10 for autocomplete
            var prescriptions = await _prescriptionService.GetPrescriptionsPagedAsync(1, 10, searchTerm, null);

            // Map to DTO
            var prescriptionDtos = prescriptions.Select(p => new PrescriptionSearchDto
            {
                Id = p.Id,
                PatientName = p.Patient?.Name ?? "Unknown Patient",
                Date = p.DateIssued,
                Status = p.Status
            }).ToList();

            return new JsonResult(prescriptionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during prescription search for term: {SearchTerm}", searchTerm);
            // Return empty result on error
            return new JsonResult(new List<PrescriptionSearchDto>());
        }
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        _logger.LogInformation("Received request to cancel prescription ID {PrescriptionId}", id);
        bool success = await _prescriptionService.CancelPrescriptionAsync(id);

        if (success)
        {
            TempData["SuccessMessage"] = $"Prescription ID {id} cancelled successfully.";
        }
        else
        {
            // Log error? Service/Repo should log specific DB errors.
            _logger.LogWarning("Failed to cancel prescription ID {PrescriptionId}. It might not exist or was already cancelled.", id);
            TempData["ErrorMessage"] = $"Could not cancel prescription ID {id}. It might have already been cancelled or an error occurred.";
        }

        // Redirect back to the index page (will refresh the list)
        return RedirectToPage();
    }
}
