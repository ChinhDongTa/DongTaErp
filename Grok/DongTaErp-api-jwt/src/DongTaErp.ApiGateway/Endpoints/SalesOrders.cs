using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.SalesOrders;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class SalesOrders : EndpointGroupBase
{
    public override string? GroupName => "SalesOrders";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.SalesArea.Split(',')));
        group.MapGet(Search, "/search");
        group.MapGet(GetNew, "/new");
        group.MapPost(Create);
        group.MapPut(Update, "/{id}");
        group.MapPost(Confirm, "/{id}/confirm");
        group.MapPost(Cancel, "/{id}/cancel");
    }

    public async Task<IResult> GetById([FromServices] ISalesOrderService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Search([FromServices] ISalesOrderService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, string? sortBy = null, bool isDescending = false, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize, sortBy, isDescending), ct)).ToHttpResult();

    public async Task<IResult> GetNew([FromServices] ISalesOrderService service, CancellationToken ct)
        => (await service.GetNewOrdersAsync(ct)).ToHttpResult();

    public async Task<IResult> Create([FromServices] ISalesOrderService service, [FromBody] CreateSalesOrderDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> Update([FromServices] ISalesOrderService service, string id, [FromBody] UpdateSalesOrderDto dto, CancellationToken ct)
        => (await service.UpdateAsync(id.ToGuid(), dto, ct)).ToHttpResult();

    public async Task<IResult> Confirm([FromServices] ISalesOrderService service, string id, CancellationToken ct)
        => (await service.ConfirmAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Cancel([FromServices] ISalesOrderService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
