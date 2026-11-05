using DongTaErp.Application.Services.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Warehouses : EndpointGroupBase
{
    public override string GroupName => "Warehouses";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetWarehouseById, "/{id}");
        group.MapGet(GetWarehouses, "/");
        group.MapGet(SearchWarehouses, "/search");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateWarehouseAsync);
        group.MapPut(UpdateWarehouseAsync, "{id}");
        group.MapPut(SoftDeleteWarehouseAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteWarehouseAsync, "{id}");
    }
    public async Task<IResult> GetWarehouseById([FromServices] IWarehouseService WarehouseService, string id, CancellationToken ct = default)
    {
        var result = await WarehouseService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetWarehouses([FromServices] IWarehouseService WarehouseService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await WarehouseService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> SearchWarehouses([FromServices] IWarehouseService WarehouseService, string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        var result = await WarehouseService.SearchAsync(searchTerm, pageNumber, pageSize, sortBy, isDescending, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateWarehouseAsync([FromServices] IWarehouseService WarehouseService, [FromBody] CreateWarehouseDto dto, CancellationToken ct = default)
    {
        var result = await WarehouseService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateWarehouseAsync([FromServices] IWarehouseService WarehouseService, string id, [FromBody] UpdateWarehouseDto dto, CancellationToken ct = default)
    {
        var result = await WarehouseService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteWarehouseAsync([FromServices] IWarehouseService WarehouseService, string id, CancellationToken ct = default)
    {
        var result = await WarehouseService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteWarehouseAsync([FromServices] IWarehouseService WarehouseService, string id, CancellationToken ct = default)
    {
        var result = await WarehouseService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}
