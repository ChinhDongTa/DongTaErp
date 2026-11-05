using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Payments : EndpointGroupBase
{
    public override string GroupName => "Payments";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.Money.Split(',')));
        group.MapGet(GetPaymentById, "/{id}");
        group.MapGet(SearchPayment, "/search");
        group.MapPost(CreatePayment);
        group.MapPost(PostPayment, "/{id}/post");
        group.MapPost(CancelPayment, "/{id}/cancel");
    }

    public async Task<IResult> GetPaymentById([FromServices] IPaymentService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> SearchPayment([FromServices] IPaymentService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), ct)).ToHttpResult();

    public async Task<IResult> CreatePayment([FromServices] IPaymentService service, [FromBody] CreatePaymentDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> PostPayment([FromServices] IPaymentService service, string id, CancellationToken ct)
        => (await service.PostAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> CancelPayment([FromServices] IPaymentService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}