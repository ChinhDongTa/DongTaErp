using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.GoodsIssues;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class GoodsIssues : EndpointGroupBase
{
    public override string? GroupName => "GoodsIssues";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.SalesArea.Split(',')));
        group.MapGet(Search, "/search");
        group.MapGet(GetLines, "/lines/{salesOrderId}");
        group.MapPost(CreateAndPost, "/post");
        group.MapPost(Post, "/{id}/post");
        group.MapPost(Cancel, "/{id}/cancel");
    }

    public async Task<IResult> GetById([FromServices] IGoodsIssueService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Search([FromServices] IGoodsIssueService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), ct)).ToHttpResult();

    public async Task<IResult> GetLines([FromServices] IGoodsIssueService service, string salesOrderId, string? warehouseId, CancellationToken ct)
        => (await service.GetGoodsIssueLinesAsync(salesOrderId.ToGuid(), string.IsNullOrEmpty(warehouseId) ? null : warehouseId.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CreateAndPost([FromServices] IGoodsIssueService service, [FromBody] CreateGoodsIssueDto dto, CancellationToken ct)
        => (await service.CreateAndPostAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> Post([FromServices] IGoodsIssueService service, string id, CancellationToken ct)
        => (await service.PostAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Cancel([FromServices] IGoodsIssueService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
