using Rosheta.Core.Domain.Entities;

namespace Rosheta.Core.Application.DTOs;

public class PrescriptionSearchDto
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public PrescriptionStatus Status { get; set; }
}
