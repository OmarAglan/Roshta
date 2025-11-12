namespace Rosheta.Core.Application.DTOs;

public class MedicationSearchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Dosage { get; set; }
}
