namespace Roshta.Models.Base
{
    /// <summary>
    /// Interface for entities that require audit tracking
    /// </summary>
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}