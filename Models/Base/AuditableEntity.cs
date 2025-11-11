namespace Roshta.Models.Base
{
    /// <summary>
    /// Base entity with primary key and audit fields
    /// </summary>
    public abstract class AuditableEntity : BaseEntity, IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}