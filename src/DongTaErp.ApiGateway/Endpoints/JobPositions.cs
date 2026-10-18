using DongTaErp.Application.Services.JobPositions;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class JobPositions : EndpointGroupBase
{
    override public string GroupName => "JobPositions";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetJobPositionById, "/{id}");
        group.MapGet(GetJobPositions, "/");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateJobPositionAsync);
        group.MapPut(UpdateJobPositionAsync, "{id}");
        group.MapPut(SoftDeleteJobPositionAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteJobPositionAsync, "{id}");
    }
    public async Task<IResult> GetJobPositionById([FromServices] IJobPositionService JobPositionService, string id, CancellationToken ct = default)
    {
        var result = await JobPositionService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetJobPositions([FromServices] IJobPositionService JobPositionService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await JobPositionService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateJobPositionAsync([FromServices] IJobPositionService JobPositionService, [FromBody] CreateJobPositionDto dto, CancellationToken ct = default)
    {
        var result = await JobPositionService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateJobPositionAsync([FromServices] IJobPositionService JobPositionService, string id, [FromBody] UpdateJobPositionDto dto, CancellationToken ct = default)
    {
        var result = await JobPositionService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteJobPositionAsync([FromServices] IJobPositionService JobPositionService, string id, CancellationToken ct = default)
    {
        var result = await JobPositionService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteJobPositionAsync([FromServices] IJobPositionService JobPositionService, string id, CancellationToken ct = default)
    {
        var result = await JobPositionService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}