using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Rosheta.ViewModels;

public class PrescriptionCreateModel : IValidatableObject
{
    [Required(ErrorMessage = "Please select a patient.")]
    [Display(Name = "Patient")]
    public int PatientId { get; set; }

    // We will need to get the DoctorId from the logged-in user later.
    // For now, we might omit it or hardcode it in the service for testing.
    // public int DoctorId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Expiry Date")]
    public DateTime? ExpiryDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Next Appointment")]
    public DateTime? NextAppointmentDate { get; set; }

    // List to hold the items added via the UI
    public List<PrescriptionItemCreateModel> Items { get; set; } = new List<PrescriptionItemCreateModel>();

    // Nested class for individual items
    public class PrescriptionItemCreateModel
    {
        [Required(ErrorMessage = "Please select a medication.")]
        public int MedicationId { get; set; }

        [StringLength(100)]
        public string? Dosage { get; set; }

        [StringLength(100)]
        public string? Frequency { get; set; }

        [StringLength(100)]
        public string? Duration { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [StringLength(50)]
        public string Quantity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Instructions are required.")]
        [StringLength(300)]
        public string Instructions { get; set; } = string.Empty;

        public int? Refills { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    // Implementation of IValidatableObject
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Rule 1: Expiry Date must be in the future (if provided)
        if (ExpiryDate.HasValue && ExpiryDate.Value.Date <= DateTime.Today)
        {
            yield return new ValidationResult(
                "Expiry Date must be in the future.", 
                new[] { nameof(ExpiryDate) });
        }

        // Rule 2: Next Appointment Date must be in the future (if provided)
        if (NextAppointmentDate.HasValue && NextAppointmentDate.Value.Date <= DateTime.Today)
        {
             yield return new ValidationResult(
                "Next Appointment Date must be in the future.", 
                new[] { nameof(NextAppointmentDate) });
        }

        // Rule 3: If both dates provided, Expiry Date must be after Next Appointment Date?
        // (This rule depends on business logic - adjust if needed)
        // if (ExpiryDate.HasValue && NextAppointmentDate.HasValue && ExpiryDate.Value.Date <= NextAppointmentDate.Value.Date)
        // {
        //     yield return new ValidationResult(
        //         "Expiry Date must be after the Next Appointment Date.", 
        //         new[] { nameof(ExpiryDate), nameof(NextAppointmentDate) });
        // }
        
        // Rule 4: Must have at least one item 
        // (This check is also done in the Service, but can be added here for earlier feedback)
        if (Items == null || !Items.Any())
        {
             yield return new ValidationResult(
                "The prescription must contain at least one medication item.", 
                new[] { nameof(Items) }); // Associate error with the Items collection
        }
    }
} 
