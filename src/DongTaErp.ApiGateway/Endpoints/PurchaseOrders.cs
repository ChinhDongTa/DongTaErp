using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class PurchaseOrders : EndpointGroupBase
{
    public override string GroupName => "PurchaseOrders";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.PurchaseArea.Split(',')));
        group.MapGet(SearchPurchaseOrder, "/search");
        group.MapGet(GetPurchaseOrderById, "/{id}");
        group.MapPost(CreatePurchaseOrder, "/create");
        group.MapPut(UpdatePurchaseOrder, "/{id}");
        group.MapPost(ConfirmPurchaseOrder, "/{id}/confirm");
        group.MapPost(CancelPurchaseOrder, "/{id}/cancel");
    }

    public async Task<IResult> GetPurchaseOrderById([FromServices] IPurchaseOrderService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> SearchPurchaseOrder([FromServices] IPurchaseOrderService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, string? sortBy = null, bool isDescending = false, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize, sortBy, isDescending), ct)).ToHttpResult();

    public async Task<IResult> CreatePurchaseOrder([FromServices] IPurchaseOrderService service, [FromBody] CreatePurchaseOrderDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> UpdatePurchaseOrder([FromServices] IPurchaseOrderService service, string id, [FromBody] UpdatePurchaseOrderDto dto, CancellationToken ct)
        => (await service.UpdateAsync(id.ToGuid(), dto, ct)).ToHttpResult();

    public async Task<IResult> ConfirmPurchaseOrder([FromServices] IPurchaseOrderService service, string id, CancellationToken ct)
        => (await service.ConfirmAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CancelPurchaseOrder([FromServices] IPurchaseOrderService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
