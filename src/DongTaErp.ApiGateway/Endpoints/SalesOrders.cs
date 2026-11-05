using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.SalesOrders;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class SalesOrders : EndpointGroupBase
{
    public override string GroupName => "SalesOrders";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.SalesArea.Split(',')));
        group.MapGet(SearchSalesOrder, "/search");
        group.MapGet(GetNewSalesOrder, "/new");
        group.MapPost(CreateSalesOrder, "/create");
        group.MapPut(UpdateSalesOrder, "/{id}");
        group.MapPost(ConfirmSalesOrder, "/{id}/confirm");
        group.MapPost(CancelSalesOrder, "/{id}/cancel");
    }

    public async Task<IResult> GetSalesOrderById([FromServices] ISalesOrderService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> SearchSalesOrder([FromServices] ISalesOrderService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, string? sortBy = null, bool isDescending = false, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize, sortBy, isDescending), ct)).ToHttpResult();

    public async Task<IResult> GetNewSalesOrder([FromServices] ISalesOrderService service, CancellationToken ct)
        => (await service.GetNewOrdersAsync(ct)).ToHttpResult();

    public async Task<IResult> CreateSalesOrder([FromServices] ISalesOrderService service, [FromBody] CreateSalesOrderDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> UpdateSalesOrder([FromServices] ISalesOrderService service, string id, [FromBody] UpdateSalesOrderDto dto, CancellationToken ct)
        => (await service.UpdateAsync(id.ToGuid(), dto, ct)).ToHttpResult();

    public async Task<IResult> ConfirmSalesOrder([FromServices] ISalesOrderService service, string id, CancellationToken ct)
        => (await service.ConfirmAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CancelSalesOrder([FromServices] ISalesOrderService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
