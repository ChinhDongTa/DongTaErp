using DongTaErp.Application.Services.LeaveRequests;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class LeaveRequests : EndpointGroupBase
{
    override public string GroupName => "LeaveRequests";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetLeaveRequestById, "/{id}");
        group.MapGet(GetLeaveRequests, "/");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateLeaveRequestAsync);
        group.MapPut(UpdateLeaveRequestAsync, "{id}");
        group.MapPut(SoftDeleteLeaveRequestAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteLeaveRequestAsync, "{id}");
    }
    public async Task<IResult> GetLeaveRequestById([FromServices] ILeaveRequestService LeaveRequestService, string id, CancellationToken ct = default)
    {
        var result = await LeaveRequestService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetLeaveRequests([FromServices] ILeaveRequestService LeaveRequestService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await LeaveRequestService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateLeaveRequestAsync([FromServices] ILeaveRequestService LeaveRequestService, [FromBody] CreateLeaveRequestDto dto, CancellationToken ct = default)
    {
        var result = await LeaveRequestService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateLeaveRequestAsync([FromServices] ILeaveRequestService LeaveRequestService, string id, [FromBody] UpdateLeaveRequestDto dto, CancellationToken ct = default)
    {
        var result = await LeaveRequestService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteLeaveRequestAsync([FromServices] ILeaveRequestService LeaveRequestService, string id, CancellationToken ct = default)
    {
        var result = await LeaveRequestService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteLeaveRequestAsync([FromServices] ILeaveRequestService LeaveRequestService, string id, CancellationToken ct = default)
    {
        var result = await LeaveRequestService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}