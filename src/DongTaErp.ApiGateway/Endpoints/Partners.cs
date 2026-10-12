using DongTaErp.Application.Services.Partners;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Partners : EndpointGroupBase
{
    public override string GroupName => "Partners";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetPartnerById, "/{id}");
        group.MapGet(GetPartners, "/");
        group.MapGet(SearchPartners, "/search");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreatePartnerAsync);
        group.MapPut(UpdatePartnerAsync, "{id}");
        group.MapPut(SoftDeletePartnerAsync, "/soft-delete/{id}");
        group.MapDelete(DeletePartnerAsync, "{id}");
    }
    public async Task<IResult> GetPartnerById([FromServices] IPartnerService PartnerService, string id, CancellationToken ct = default)
    {
        var result = await PartnerService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetPartners([FromServices] IPartnerService PartnerService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await PartnerService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> SearchPartners([FromServices] IPartnerService PartnerService, string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        var result = await PartnerService.SearchAsync(searchTerm, pageNumber, pageSize, sortBy, isDescending, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreatePartnerAsync([FromServices] IPartnerService PartnerService, [FromBody] CreatePartnerDto dto, CancellationToken ct = default)
    {
        var result = await PartnerService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdatePartnerAsync([FromServices] IPartnerService PartnerService, string id, [FromBody] UpdatePartnerDto dto, CancellationToken ct = default)
    {
        var result = await PartnerService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeletePartnerAsync([FromServices] IPartnerService PartnerService, string id, CancellationToken ct = default)
    {
        var result = await PartnerService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeletePartnerAsync([FromServices] IPartnerService PartnerService, string id, CancellationToken ct = default)
    {
        var result = await PartnerService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}
