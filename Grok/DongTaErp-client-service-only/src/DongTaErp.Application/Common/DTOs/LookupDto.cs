namespace DongTaErp.Application.Common.DTOs;

public class LookupItemDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = "";
    public decimal Price { get; set; }
    public Guid? ExtraId { get; set; }
}

public class DocumentLineLookupDto
{
    public Guid LineId { get; set; }
    public Guid ProductId { get; set; }
    public string Label { get; set; } = "";
    public decimal OrderedQty { get; set; }
    public decimal DoneQty { get; set; }
    public decimal RelatedQty { get; set; }
    public decimal RemainingQty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal StockQty { get; set; }
}

public class OpenDocumentDto
{
    public Guid Id { get; set; }
    public Guid PartnerId { get; set; }
    public string Number { get; set; } = "";
    public decimal Total { get; set; }
    public decimal Balance { get; set; }
}

public class DashboardDto
{
    public decimal SalesWeek { get; set; }
    public decimal SalesMonth { get; set; }
    public decimal ReceivableDue { get; set; }
    public decimal PayableDue { get; set; }
    public List<DashboardStockDto> HighStock { get; set; } = [];
    public List<DashboardReplenishDto> Replenish { get; set; } = [];
    public List<DashboardDueDto> Dues { get; set; } = [];
    public List<DashboardTopProductDto> TopProducts { get; set; } = [];
}

public class DashboardStockDto
{
    public string Warehouse { get; set; } = "";
    public string Product { get; set; } = "";
    public decimal Qty { get; set; }
    public decimal MinQty { get; set; }
    public decimal Ratio { get; set; }
}

public class DashboardReplenishDto
{
    public string Warehouse { get; set; } = "";
    public string Product { get; set; } = "";
    public decimal Qty { get; set; }
    public decimal Shortage { get; set; }
    public string Action { get; set; } = "";
    public string Detail { get; set; } = "";
    public Guid ProductId { get; set; }
    public Guid? FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public decimal TransferQty { get; set; }
    public decimal BuyQty { get; set; }
    public bool CanTransfer => FromWarehouseId is not null && TransferQty > 0;
    public bool CanBuy => BuyQty > 0;
}

public class DashboardDueDto
{
    public string Kind { get; set; } = "";
    public string Number { get; set; } = "";
    public string Partner { get; set; } = "";
    public DateTime DueDate { get; set; }
    public decimal Balance { get; set; }
    public string State { get; set; } = "";
}

public class DashboardTopProductDto
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Qty { get; set; }
    public decimal Amount { get; set; }
}
