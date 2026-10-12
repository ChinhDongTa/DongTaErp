namespace DongTaErp.Application.Common.DTOs;

public class GoodsIssueLineDto : BaseDto
{
    public Guid GoodsIssueId { get; set; }
    public Guid SalesOrderLineId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductSku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string? Note { get; set; }
}

public class CreateGoodsIssueLineDto
{
    public Guid SalesOrderLineId { get; set; }
    public decimal Quantity { get; set; }
    public string? Note { get; set; }
}
