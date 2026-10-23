namespace DongTaErp.Application.Common.DTOs;

public class CreateStockTransferDto
{
    public Guid ProductId { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public decimal Qty { get; set; }
    public string? Notes { get; set; }
}

public class StockTransferOptionDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = null!;
}

public class StockTransferResultDto
{
    public string Code { get; set; } = "";
    public string Product { get; set; } = "";
    public string FromWarehouse { get; set; } = "";
    public string ToWarehouse { get; set; } = "";
    public decimal Qty { get; set; }
}

public class BuySuggestionDto
{
    public Guid ProductId { get; set; }
    public string Product { get; set; } = "";
    public decimal Qty { get; set; }
}
