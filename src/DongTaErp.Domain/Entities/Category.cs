namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Category : BaseAuditableEntity
{
    public Guid CategoryTypeId { get; set; }
    public CategoryType CategoryType { get; set; } = null!;
    [MaxLength(50)]
    public required string Code { get; set; } 
    [MaxLength(255)]
    public required string Name { get; set; }

    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }

    public ICollection<Category> Children { get; set; }=    [];
}