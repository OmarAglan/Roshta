using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System; // For DateTime
using Rosheta.Core.Domain.Base;

namespace Rosheta.Core.Domain.Entities;

public class Patient : AuditableEntity, IValidatableObject // Implement interface
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")] // Added Display attribute
    public DateTime DateOfBirth { get; set; }

    [StringLength(200)]
    [Display(Name = "Contact Info")] // Added Display attribute
    public string? ContactInfo { get; set; } // e.g., phone, email

    [Display(Name = "Visit Count")] // Added Display attribute
    public int VisitCount { get; set; } = 0; // Added: Number of visits

    [DataType(DataType.Date)]
    [Display(Name = "Last Visit Date")] // Added Display attribute
    public DateTime? LastVisitDate { get; set; } // Added: Last time visited (nullable)

    [Display(Name = "Has Outstanding Balance?")] // Added Display attribute
    public bool HasOutstandingBalance { get; set; } = false; // Added: Payed all or not

    [Display(Name = "Is Active?")] // Added Display attribute
    public bool IsActive { get; set; } = true; // Suggested: Active status

    // Navigation property for related Prescriptions
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    
    // IValidatableObject implementation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Rule 1: Date of Birth must be in the past.
        if (DateOfBirth.Date >= DateTime.Today)
        {
            yield return new ValidationResult(
                "Date of Birth must be in the past.",
                new[] { nameof(DateOfBirth) });
        }

        // Rule 2: Last Visit Date cannot be in the future (if provided).
        if (LastVisitDate.HasValue && LastVisitDate.Value.Date > DateTime.Today)
        {
             yield return new ValidationResult(
                "Last Visit Date cannot be in the future.",
                new[] { nameof(LastVisitDate) });
        }
        
        // Add other complex rules here if needed
    }
}
