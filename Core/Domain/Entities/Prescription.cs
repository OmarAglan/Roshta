using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Rosheta.Core.Domain.Base;
using Rosheta.Core.Domain.Enums;

namespace Rosheta.Core.Domain.Entities;

public class Prescription : AuditableEntity
{
    [Required]
    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [Required]
    public int DoctorId { get; set; }
    [ForeignKey("DoctorId")]
    public virtual Doctor Doctor { get; set; } = null!;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime DateIssued { get; set; } = DateTime.UtcNow;

    [DataType(DataType.Date)]
    public DateTime? ExpiryDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? NextAppointmentDate { get; set; }

    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;

    // Navigation property for related PrescriptionItems
    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}
