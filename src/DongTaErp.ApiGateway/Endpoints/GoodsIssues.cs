using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.GoodsIssues;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class GoodsIssues : EndpointGroupBase
{
    public override string GroupName => "GoodsIssues";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.SalesArea.Split(',')));
        group.MapGet(GetGoodsIssueById, "/{id}");
        group.MapGet(SearchGoodsIssues, "/search");
        group.MapGet(GetGoodsIssueLines, "/lines/{salesOrderId}");
        group.MapPost(CreateAndPostGoodsIssue, "/post");
        group.MapPost(PostGoodsIssue, "/{id}/post");
        group.MapPost(CancelGoodsIssue, "/{id}/cancel");
    }

    public async Task<IResult> GetGoodsIssueById([FromServices] IGoodsIssueService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> SearchGoodsIssues([FromServices] IGoodsIssueService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), ct)).ToHttpResult();

    public async Task<IResult> GetGoodsIssueLines([FromServices] IGoodsIssueService service, string salesOrderId, string? warehouseId, CancellationToken ct)
        => (await service.GetGoodsIssueLinesAsync(salesOrderId.ToGuid(), string.IsNullOrEmpty(warehouseId) ? null : warehouseId.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CreateAndPostGoodsIssue([FromServices] IGoodsIssueService service, [FromBody] CreateGoodsIssueDto dto, CancellationToken ct)
        => (await service.CreateAndPostAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> PostGoodsIssue([FromServices] IGoodsIssueService service, string id, CancellationToken ct)
        => (await service.PostAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CancelGoodsIssue([FromServices] IGoodsIssueService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
