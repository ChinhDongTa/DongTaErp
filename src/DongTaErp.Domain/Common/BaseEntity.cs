namespace DongTaErp.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
}