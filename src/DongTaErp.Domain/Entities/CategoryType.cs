namespace DongTaErp.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class CategoryType : BaseAuditableEntity
{
    [MaxLength(50)]
    public required string Code { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }

    public ICollection<Category> Categories { get; set; } = [];
}