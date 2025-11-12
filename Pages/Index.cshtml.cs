using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rosheta.Services.Interfaces; // Add this
using System.Threading.Tasks; // Add this

namespace Rosheta.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IPatientService _patientService;
    private readonly IMedicationService _medicationService;
    private readonly IPrescriptionService _prescriptionService;
    private readonly ILicenseService _licenseService; // Inject License Service

    public IndexModel(
        ILogger<IndexModel> logger,
        IPatientService patientService,
        IMedicationService medicationService,
        IPrescriptionService prescriptionService,
        ILicenseService licenseService) // Add to constructor
    {
        _logger = logger;
        _patientService = patientService;
        _medicationService = medicationService;
        _prescriptionService = prescriptionService;
        _licenseService = licenseService; // Assign injected service
    }

    // Properties to hold dashboard data
    public int PatientCount { get; set; }
    public int MedicationCount { get; set; }
    public int PrescriptionCount { get; set; }
    public bool ShowEditProfileButton { get; set; } // Property for button visibility
    // Could add counts for Active Prescriptions, etc. later

    public async Task OnGetAsync()
    {
        // Fetch counts using the injected services
        // Assuming GetCountAsync/GetMedicationsCountAsync/GetPrescriptionsCountAsync exist and return total counts
        // We might need to add filtering later (e.g., for active prescriptions)
        PatientCount = await _patientService.GetPatientsCountAsync();
        MedicationCount = await _medicationService.GetMedicationsCountAsync();
        PrescriptionCount = await _prescriptionService.GetPrescriptionsCountAsync();

        // Check license status for Edit Profile button
        ShowEditProfileButton = _licenseService.IsActivated() && await _licenseService.IsProfileSetupAsync();

        _logger.LogInformation("Dashboard loaded: Patients={PatientCount}, Medications={MedicationCount}, Prescriptions={PrescriptionCount}, ShowEditProfile={ShowEditProfile}",
            PatientCount, MedicationCount, PrescriptionCount, ShowEditProfileButton);
    }
}
