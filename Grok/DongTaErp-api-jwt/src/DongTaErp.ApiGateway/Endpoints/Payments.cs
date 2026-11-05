using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Payments : EndpointGroupBase
{
    public override string? GroupName => "Payments";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.Money.Split(',')));
        group.MapGet(Search, "/search");
        group.MapPost(Create);
        group.MapPost(Post, "/{id}/post");
        group.MapPost(Cancel, "/{id}/cancel");
    }

    public async Task<IResult> GetById([FromServices] IPaymentService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Search([FromServices] IPaymentService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), ct)).ToHttpResult();

    public async Task<IResult> Create([FromServices] IPaymentService service, [FromBody] CreatePaymentDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> Post([FromServices] IPaymentService service, string id, CancellationToken ct)
        => (await service.PostAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Cancel([FromServices] IPaymentService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
