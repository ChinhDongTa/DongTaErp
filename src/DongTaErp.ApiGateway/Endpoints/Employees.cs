using DongTaErp.Application.Services.Employees;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Employees : EndpointGroupBase
{
    override public string GroupName => "Employees";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetEmployeeById, "/{id}");
        group.MapGet(GetEmployees, "/");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateEmployeeAsync);
        group.MapPut(UpdateEmployeeAsync, "{id}");
        group.MapDelete(DeleteEmployeeAsync, "{id}");
    }
    public async Task<IResult> GetEmployeeById([FromServices] IEmployeeService EmployeeService, string id, CancellationToken ct = default)
    {
        var result = await EmployeeService.GetEmployeeByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetEmployees([FromServices] IEmployeeService EmployeeService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await EmployeeService.GetAllEmployeesAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateEmployeeAsync([FromServices] IEmployeeService EmployeeService, [FromBody] CreateEmployeeDto dto, CancellationToken ct = default)
    {
        var result = await EmployeeService.CreateAsync(dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> UpdateEmployeeAsync([FromServices] IEmployeeService EmployeeService, string id, [FromBody] UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        var result = await EmployeeService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteEmployeeAsync([FromServices] IEmployeeService EmployeeService, string id, CancellationToken ct = default)
    {
        var result = await EmployeeService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}
