using DongTaErp.Application.Services.Categories;
using DongTaErp.Application.Services.CompanyProfiles;
using DongTaErp.Application.Services.Departments;
using DongTaErp.Application.Services.Disbursements;
using DongTaErp.Application.Services.Employees;
using DongTaErp.Application.Services.GoodsIssues;
using DongTaErp.Application.Services.GoodsReceipts;
using DongTaErp.Application.Services.InventoryTxns;
using DongTaErp.Application.Services.Invoices;
using DongTaErp.Application.Services.JobPositions;
using DongTaErp.Application.Services.LeaveRequests;
using DongTaErp.Application.Services.Partners;
using DongTaErp.Application.Services.Payments;
using DongTaErp.Application.Services.Payrolls;
using DongTaErp.Application.Services.Products;
using DongTaErp.Application.Services.PurchaseOrders;
using DongTaErp.Application.Services.SalesOrders;
using DongTaErp.Application.Services.VendorBills;
using DongTaErp.Application.Services.Warehouses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongTaErp.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {

        builder.Services.AddScoped<IPartnerService, PartnerService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        
        builder.Services.AddScoped<ICompanyProfileService, CompanyProfileService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IWarehouseService, WarehouseService>();
        builder.Services.AddScoped<IDepartmentService, DepartmentService>();
        builder.Services.AddScoped<IInventoryTxnService, InventoryTxnService>();
        builder.Services.AddScoped<IJobPositionService, JobPositionService>();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<IPayrollService, PayrollService>();
        builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        builder.Services.AddScoped<IInventoryTxnService, InventoryTxnService>();
        builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        builder.Services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
        builder.Services.AddScoped<IVendorBillService, VendorBillService>();
        builder.Services.AddScoped<IDisbursementService, DisbursementService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IGoodsIssueService, GoodsIssueService>();
        builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();

        builder.Services.AddValidatorsFromAssemblies([typeof(DependencyInjection).Assembly],includeInternalTypes: true);
    }
    
}