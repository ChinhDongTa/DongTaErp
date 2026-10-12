namespace DongTaErp.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<CompanyProfile> CompanyProfiles { get; }
    DbSet<Product> Products { get; }
    DbSet<Partner> Partners { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderLine> SalesOrderLines { get; }
    DbSet<InventoryTxn> InventoryTxns { get; }
    DbSet<Department> Departments { get; }
    DbSet<JobPosition> JobPositions { get; }
    DbSet<Employee> Employees { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<StockBalance> StockBalances { get; }
    DbSet<CategoryType> CategoryTypes { get; }
    DbSet<Category> Categories { get; }
    DbSet<GoodsIssue> GoodsIssues { get; }
    DbSet<GoodsIssueLine> GoodsIssueLines { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceLine> InvoiceLines { get; }
    DbSet<Payment> Payments { get; }
    DbSet<PaymentAllocation> PaymentAllocations { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderLine> PurchaseOrderLines { get; }
    DbSet<GoodsReceipt> GoodsReceipts { get; }
    DbSet<GoodsReceiptLine> GoodsReceiptLines { get; }
    DbSet<VendorBill> VendorBills { get; }
    DbSet<VendorBillLine> VendorBillLines { get; }
    DbSet<Disbursement> Disbursements { get; }
    DbSet<DisbursementAllocation> DisbursementAllocations { get; }
    DbSet<Payroll> Payrolls { get; }
}