using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Roshta.Models.Base;

namespace Roshta.Models.Entities;

// Join entity for the many-to-many relationship between Prescription and Medication
public class PrescriptionItem : AuditableEntity
{
    [Required]
    public int PrescriptionId { get; set; }
    [ForeignKey("PrescriptionId")]
    public virtual Prescription Prescription { get; set; } = null!;

    [Required]
    public int MedicationId { get; set; }
    [ForeignKey("MedicationId")]
    public virtual Medication Medication { get; set; } = null!;

    [StringLength(100)]
    public string? Dosage { get; set; }

    [StringLength(100)]
    public string? Frequency { get; set; }

    [StringLength(100)]
    public string? Duration { get; set; }

    [Required]
    [StringLength(50)] // e.g., "30 tablets", "1 bottle (100ml)"
    public string Quantity { get; set; } = string.Empty;

    [Required]
    [StringLength(300)] // e.g., "Take one tablet twice daily with food"
    public string Instructions { get; set; } = string.Empty;

    public int? Refills { get; set; } // Number of allowed refills

    [StringLength(500)]
    public string? Notes { get; set; } // Added: Notes on the item (nullable)
}