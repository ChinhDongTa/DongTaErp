using DongTaErp.Application.Services.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Departments : EndpointGroupBase
{
    override public string GroupName => "Departments";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetDepartmentById, "/{id}");
        group.MapGet(GetDepartments, "/");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateDepartmentAsync);
        group.MapPut(UpdateDepartmentAsync, "{id}");
        group.MapPut(SoftDeleteDepartmentAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteDepartmentAsync, "{id}");
    }
    public async Task<IResult> GetDepartmentById([FromServices] IDepartmentService DepartmentService, string id, CancellationToken ct = default)
    {
        var result = await DepartmentService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetDepartments([FromServices] IDepartmentService DepartmentService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await DepartmentService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateDepartmentAsync([FromServices] IDepartmentService DepartmentService, [FromBody] CreateDepartmentDto dto, CancellationToken ct = default)
    {
        var result = await DepartmentService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateDepartmentAsync([FromServices] IDepartmentService DepartmentService, string id, [FromBody] UpdateDepartmentDto dto, CancellationToken ct = default)
    {
        var result = await DepartmentService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteDepartmentAsync([FromServices] IDepartmentService DepartmentService, string id, CancellationToken ct = default)
    {
        var result = await DepartmentService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteDepartmentAsync([FromServices] IDepartmentService DepartmentService, string id, CancellationToken ct = default)
    {
        var result = await DepartmentService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}