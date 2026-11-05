using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class PurchaseOrders : EndpointGroupBase
{
    public override string? GroupName => "PurchaseOrders";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.PurchaseArea.Split(',')));
        group.MapGet(Search, "/search");
        group.MapPost(Create);
        group.MapPut(Update, "/{id}");
        group.MapPost(Confirm, "/{id}/confirm");
        group.MapPost(Cancel, "/{id}/cancel");
    }

    public async Task<IResult> GetById([FromServices] IPurchaseOrderService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Search([FromServices] IPurchaseOrderService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, string? sortBy = null, bool isDescending = false, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize, sortBy, isDescending), ct)).ToHttpResult();

    public async Task<IResult> Create([FromServices] IPurchaseOrderService service, [FromBody] CreatePurchaseOrderDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> Update([FromServices] IPurchaseOrderService service, string id, [FromBody] UpdatePurchaseOrderDto dto, CancellationToken ct)
        => (await service.UpdateAsync(id.ToGuid(), dto, ct)).ToHttpResult();

    public async Task<IResult> Confirm([FromServices] IPurchaseOrderService service, string id, CancellationToken ct)
        => (await service.ConfirmAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Cancel([FromServices] IPurchaseOrderService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
