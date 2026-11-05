using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.GoodsReceipts;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class GoodsReceipts : EndpointGroupBase
{
    public override string? GroupName => "GoodsReceipts";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.PurchaseArea.Split(',')));
        group.MapGet(Search, "/search");
        group.MapGet(GetLines, "/lines/{purchaseOrderId}");
        group.MapPost(Create);
        group.MapPost(Post, "/{id}/post");
        group.MapPost(Cancel, "/{id}/cancel");
    }

    public async Task<IResult> GetById([FromServices] IGoodsReceiptService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Search([FromServices] IGoodsReceiptService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), ct)).ToHttpResult();

    public async Task<IResult> GetLines([FromServices] IGoodsReceiptService service, string purchaseOrderId, CancellationToken ct)
        => (await service.GetGoodsReceiptLinesAsync(purchaseOrderId.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Create([FromServices] IGoodsReceiptService service, [FromBody] CreateGoodsReceiptDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> Post([FromServices] IGoodsReceiptService service, string id, CancellationToken ct)
        => (await service.PostAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Cancel([FromServices] IGoodsReceiptService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
