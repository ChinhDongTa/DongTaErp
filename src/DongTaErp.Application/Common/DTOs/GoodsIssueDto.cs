namespace DongTaErp.Application.Common.DTOs;

public class GoodsIssueDto : BaseDto
{
    /// <summary>
    /// Mã phiếu xuất kho
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Tên kho xuất
    /// </summary>
    public string WarehouseName { get; set; } = null!;

    /// <summary>
    /// Ngày xuất kho
    /// </summary>
    public DateTime IssueDate { get; set; }
}

public class CreateGoodsIssueDto
{
    /// <summary>
    /// Mã phiếu xuất kho.
    /// Bắt buộc.
    /// Phải duy nhất trong hệ thống.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Id kho xuất.
    /// Bắt buộc.
    /// Phải tồn tại trong hệ thống.
    /// </summary>
    public Guid WarehouseId { get; set; }

    /// <summary>
    /// Ngày xuất kho.
    /// Bắt buộc.
    /// Không được lớn hơn ngày hiện tại nếu nghiệp vụ không cho phép xuất tương lai.
    /// </summary>
    public DateTime IssueDate { get; set; }
}

public class UpdateGoodsIssueDto
{
    /// <summary>
    /// Id phiếu xuất kho.
    /// Bắt buộc.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Mã phiếu xuất kho.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải duy nhất trong hệ thống.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Id kho xuất.
    /// Không bắt buộc khi cập nhật.
    /// Nếu nhập phải tồn tại trong hệ thống.
    /// </summary>
    public Guid? WarehouseId { get; set; }

    /// <summary>
    /// Ngày xuất kho.
    /// Không bắt buộc khi cập nhật.
    /// </summary>
    public DateTime? IssueDate { get; set; }
}