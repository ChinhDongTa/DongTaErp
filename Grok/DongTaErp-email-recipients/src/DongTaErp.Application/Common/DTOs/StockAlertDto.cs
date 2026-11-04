namespace DongTaErp.Application.Common.DTOs;

public class StockAlertDto
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string Warehouse { get; set; } = "";
    public decimal Qty { get; set; }
    public decimal MinQty { get; set; }
    public decimal SafeQty { get; set; }
    public string Level { get; set; } = "";
}
