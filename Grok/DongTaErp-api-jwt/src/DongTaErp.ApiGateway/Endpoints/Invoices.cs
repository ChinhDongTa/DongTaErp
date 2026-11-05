using DongTaErp.ApiGateway.Security;
using DongTaErp.Application.Services.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Invoices : EndpointGroupBase
{
    public override string? GroupName => "Invoices";
    public override void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(p => p.RequireRole(ApiRoles.Money.Split(',')));
        group.MapGet(Search, "/search");
        group.MapGet(GetOpen, "/open/{partnerId}");
        group.MapPost(Create);
        group.MapPost(Issue, "/{id}/issue");
        group.MapPost(Cancel, "/{id}/cancel");
    }

    public async Task<IResult> GetById([FromServices] IInvoiceService service, string id, CancellationToken ct)
        => (await service.GetByIdAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Search([FromServices] IInvoiceService service, string? searchTerm, int pageNumber = 1, int pageSize = 20, string? range = null, bool dueOnly = false, CancellationToken ct = default)
        => (await service.SearchAsync(new SimpleSearch(searchTerm, pageNumber, pageSize), range, dueOnly, ct)).ToHttpResult();

    public async Task<IResult> GetOpen([FromServices] IInvoiceService service, string partnerId, CancellationToken ct)
        => (await service.GetOpenInvoicesAsync(partnerId.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Create([FromServices] IInvoiceService service, [FromBody] CreateInvoiceDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, ct)).ToCreatedHttpResult(GroupName);

    public async Task<IResult> Issue([FromServices] IInvoiceService service, string id, CancellationToken ct)
        => (await service.IssueAsync(id.ToGuid(), ct)).ToHttpResult();

    public async Task<IResult> Cancel([FromServices] IInvoiceService service, string id, CancellationToken ct)
        => (await service.CancelAsync(id.ToGuid(), ct)).ToHttpResult();
}
