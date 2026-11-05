using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.GoodsReceipts;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class GoodsReceipts : EndpointGroupBase
{
    public override string GroupName => "GoodsReceipts";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.PurchaseArea.Split(',')));
        group.MapGet(GetGoodsReceiptById, "/{id}");
        group.MapGet(SearchGoodsReceipts, "/search");
        group.MapGet(GetGoodsReceiptLines, "/lines/{purchaseOrderId}");
        group.MapPost(CreateGoodsReceipt);
        group.MapPost(PostGoodsReceipt, "/{id}/post");
        group.MapPost(CancelGoodsReceipt, "/{id}/cancel");
    }

    public async Task<IResult> GetGoodsReceiptById([FromServices] IGoodsReceiptService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> SearchGoodsReceipts([FromServices] IGoodsReceiptService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), ct)).ToHttpResult();

    public async Task<IResult> GetGoodsReceiptLines([FromServices] IGoodsReceiptService service, string purchaseOrderId, CancellationToken ct)
        => (await service.GetGoodsReceiptLinesAsync(purchaseOrderId.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CreateGoodsReceipt([FromServices] IGoodsReceiptService service, [FromBody] CreateGoodsReceiptDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> PostGoodsReceipt([FromServices] IGoodsReceiptService service, string id, CancellationToken ct)
        => (await service.PostAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CancelGoodsReceipt([FromServices] IGoodsReceiptService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
