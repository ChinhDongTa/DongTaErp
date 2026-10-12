using DongTaErp.Application.Services.Categories;
using DongTaErp.Application.Services.CompanyProfiles;
using DongTaErp.Application.Services.Departments;
using DongTaErp.Application.Services.Employees;
using DongTaErp.Application.Services.InventoryTxns;
using DongTaErp.Application.Services.JobPositions;
using DongTaErp.Application.Services.LeaveRequests;
using DongTaErp.Application.Services.Partners;
using DongTaErp.Application.Services.Payrolls;
using DongTaErp.Application.Services.Products;
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

        builder.Services.AddValidatorsFromAssemblies([typeof(DependencyInjection).Assembly],includeInternalTypes: true);
    }
    
}