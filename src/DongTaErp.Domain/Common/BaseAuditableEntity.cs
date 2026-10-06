namespace DongTaErp.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; } 

    public DateTimeOffset LastModified { get; set; } = DateTime.UtcNow;

    public string? LastModifiedBy { get; set; }
    
}