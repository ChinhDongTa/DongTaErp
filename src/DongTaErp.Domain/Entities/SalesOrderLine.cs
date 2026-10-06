
namespace DongTaErp.Domain.Entities;

public class SalesOrderLine:BaseAuditableEntity
{
    public Guid SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
  
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public ICollection<GoodsIssueLine> GoodsIssueLines { get; set; }= [];
}
