namespace DongTaErp.Application.Common.DTOs;

/// <summary>
/// Base DTO class cho tất cả DTOs
/// </summary>
public abstract class BaseDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
public abstract class AuditableDto : BaseDto
{
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}