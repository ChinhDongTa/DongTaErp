namespace DongTaErp.Application.Common.DTOs;

public class CategoryDto : BaseDto
{
    /// <summary>
    /// Loại danh mục
    /// </summary>
    public string CategoryType { get; set; } = null!;

    /// <summary>
    /// Mã danh mục
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Tên danh mục
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Tên danh mục cha
    /// </summary>
    public string? ParentName { get; set; }
}

public class CreateCategoryDto
{
    /// <summary>
    /// Id loại danh mục.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid CategoryTypeId { get; set; }

    /// <summary>
    /// Mã danh mục.
    /// Bắt buộc.
    /// Tối đa 50 ký tự.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tên danh mục.
    /// Bắt buộc.
    /// Tối đa 255 ký tự.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Id danh mục cha.
    /// Không bắt buộc.
    /// Nếu có thì phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? ParentId { get; set; }
}
public class UpdateCategoryDto
{
    /// <summary>
    /// Id danh mục.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id loại danh mục.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? CategoryTypeId { get; set; }

    /// <summary>
    /// Mã danh mục.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 50 ký tự.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên danh mục.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì tối đa 255 ký tự.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Id danh mục cha.
    /// Không bắt buộc khi cập nhật.
    /// Nếu có thì phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? ParentId { get; set; }
}