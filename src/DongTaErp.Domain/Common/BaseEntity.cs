namespace DongTaErp.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }=new Guid();
    public bool IsDeleted { get; set; } = false;
}