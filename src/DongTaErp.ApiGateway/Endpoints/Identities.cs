using DongTaErp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Identities : EndpointGroupBase
{
    public override string? GroupName => "Identities";
    public override void Map(RouteGroupBuilder group)
    {
        group.MapGet(GetAllUserDtoAsync, "/");
        group.MapGet(SearchAsync, "/search");
    }
    public async Task<IResult> GetAllUserDtoAsync([FromServices] IIdentityService service, int pageNumber=1, int pageSize=20,  CancellationToken ct=default)
    {
        return (await service.GetAllUserDtoAsync(pageNumber,pageSize,ct)).ToHttpResult();
    }
    public async Task<IResult> SearchAsync([FromServices] IIdentityService service,[AsParameters] SimpleSearch request, CancellationToken ct = default)
    {
        return (await service.SearchAsync(request, ct)).ToHttpResult();
    }
}
