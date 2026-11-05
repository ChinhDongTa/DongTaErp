using DongTaErp.Application.Services.CompanyProfiles;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class CompanyProfiles : EndpointGroupBase
{
    public override string GroupName => "CompanyProfiles";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetCompanyProfileById, "/{id}");
        group.MapGet(GetCompanyProfiles, "/");
        group.MapGet(SearchCompanyProfiles, "/search");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateCompanyProfileAsync);
        group.MapPut(UpdateCompanyProfileAsync, "{id}");
        group.MapPut(SoftDeleteCompanyProfileAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteCompanyProfileAsync, "{id}");
    }
    public async Task<IResult> GetCompanyProfileById([FromServices] ICompanyProfileService CompanyProfileService, string id, CancellationToken ct = default)
    {
        var result = await CompanyProfileService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetCompanyProfiles([FromServices] ICompanyProfileService CompanyProfileService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await CompanyProfileService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> SearchCompanyProfiles([FromServices] ICompanyProfileService CompanyProfileService, string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        var result = await CompanyProfileService.SearchAsync(searchTerm, pageNumber, pageSize, sortBy, isDescending, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateCompanyProfileAsync([FromServices] ICompanyProfileService CompanyProfileService, [FromBody] CreateCompanyProfileDto dto, CancellationToken ct = default)
    {
        var result = await CompanyProfileService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateCompanyProfileAsync([FromServices] ICompanyProfileService CompanyProfileService, string id, [FromBody] UpdateCompanyProfileDto dto, CancellationToken ct = default)
    {
        var result = await CompanyProfileService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteCompanyProfileAsync([FromServices] ICompanyProfileService CompanyProfileService, string id, CancellationToken ct = default)
    {
        var result = await CompanyProfileService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteCompanyProfileAsync([FromServices] ICompanyProfileService CompanyProfileService, string id, CancellationToken ct = default)
    {
        var result = await CompanyProfileService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}