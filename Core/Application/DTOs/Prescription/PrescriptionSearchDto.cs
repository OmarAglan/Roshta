using Rosheta.Models.Entities;

namespace Rosheta.ViewModels;

public class PrescriptionSearchDto
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public PrescriptionStatus Status { get; set; }
}
