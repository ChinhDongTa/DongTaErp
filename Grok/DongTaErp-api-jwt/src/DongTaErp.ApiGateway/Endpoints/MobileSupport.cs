using DongTaErp.Application.Services.Lookups;
using DongTaErp.Application.Services.Dashboard;
using DongTaErp.Application.Services.StockTransfers;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Lookups : EndpointGroupBase
{
    public override string? GroupName => "Lookups";
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet(GetCatalog, "/catalog");
        group.MapGet(GetCustomers, "/customers");
        group.MapGet(GetSuppliers, "/suppliers");
        group.MapGet(GetWarehouses, "/warehouses");
        group.MapGet(GetProducts, "/products");
    }

    public async Task<IResult> GetCatalog([FromServices] ILookupService service, CancellationToken ct)
        => (await service.GetCatalogAsync(ct)).ToHttpResult();

    public async Task<IResult> GetCustomers([FromServices] ILookupService service, CancellationToken ct)
        => (await service.GetCustomersAsync(ct)).ToHttpResult();

    public async Task<IResult> GetSuppliers([FromServices] ILookupService service, CancellationToken ct)
        => (await service.GetSuppliersAsync(ct)).ToHttpResult();

    public async Task<IResult> GetWarehouses([FromServices] ILookupService service, CancellationToken ct)
        => (await service.GetWarehousesAsync(ct)).ToHttpResult();

    public async Task<IResult> GetProducts([FromServices] ILookupService service, CancellationToken ct)
        => (await service.GetProductsAsync(true, ct)).ToHttpResult();
}

public class Dashboard : EndpointGroupBase
{
    public override string? GroupName => "Dashboard";
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet(Get, "/");
    }

    public async Task<IResult> Get([FromServices] IDashboardService service, CancellationToken ct)
        => (await service.GetAsync(ct)).ToHttpResult();
}

public class StockTransfers : EndpointGroupBase
{
    public override string? GroupName => "StockTransfers";
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(Transfer, "/");
    }

    public async Task<IResult> Transfer([FromServices] IStockTransferService service, [FromBody] CreateStockTransferDto dto, CancellationToken ct)
        => (await service.TransferAsync(dto, ct)).ToHttpResult();
}
