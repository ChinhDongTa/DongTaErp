using DongTaErp.Application.Services.InventoryTxns;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class InventoryTxns : EndpointGroupBase
{
    public override string GroupName => "InventoryTxns";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetInventoryTxnById, "/{id}");
        group.MapGet(GetInventoryTxns, "/");
        group.MapGet(SearchInventoryTxns, "/search");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateInventoryTxnAsync);
        group.MapPut(UpdateInventoryTxnAsync, "{id}");
        group.MapPut(SoftDeleteInventoryTxnAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteInventoryTxnAsync, "{id}");
    }
    public async Task<IResult> GetInventoryTxnById([FromServices] IInventoryTxnService InventoryTxnService, string id, CancellationToken ct = default)
    {
        var result = await InventoryTxnService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetInventoryTxns([FromServices] IInventoryTxnService InventoryTxnService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await InventoryTxnService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> SearchInventoryTxns([FromServices] IInventoryTxnService InventoryTxnService, string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        var result = await InventoryTxnService.SearchAsync(searchTerm, pageNumber, pageSize, sortBy, isDescending, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateInventoryTxnAsync([FromServices] IInventoryTxnService InventoryTxnService, [FromBody] CreateInventoryTxnDto dto, CancellationToken ct = default)
    {
        var result = await InventoryTxnService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateInventoryTxnAsync([FromServices] IInventoryTxnService InventoryTxnService, string id, [FromBody] UpdateInventoryTxnDto dto, CancellationToken ct = default)
    {
        var result = await InventoryTxnService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteInventoryTxnAsync([FromServices] IInventoryTxnService InventoryTxnService, string id, CancellationToken ct = default)
    {
        var result = await InventoryTxnService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteInventoryTxnAsync([FromServices] IInventoryTxnService InventoryTxnService, string id, CancellationToken ct = default)
    {
        var result = await InventoryTxnService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}