using DongTaErp.Application.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Categories : EndpointGroupBase
{
    public override string GroupName => "Categories";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetCategoryById, "/{id}");
        group.MapGet(GetCategorys, "/");
        group.MapGet(SearchCategorys, "/search");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateCategoryAsync);
        group.MapPut(UpdateCategoryAsync, "{id}");
        group.MapPut(SoftDeleteCategoryAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteCategoryAsync, "{id}");
    }
    public async Task<IResult> GetCategoryById([FromServices] ICategoryService CategoryService, string id, CancellationToken ct = default)
    {
        var result = await CategoryService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetCategorys([FromServices] ICategoryService CategoryService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await CategoryService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> SearchCategorys([FromServices] ICategoryService CategoryService, string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool isDescending = false,
        CancellationToken ct = default)
    {
        var result = await CategoryService.SearchAsync(searchTerm, pageNumber, pageSize, sortBy, isDescending, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateCategoryAsync([FromServices] ICategoryService CategoryService, [FromBody] CreateCategoryDto dto, CancellationToken ct = default)
    {
        var result = await CategoryService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult(GroupName);
    }
    public async Task<IResult> UpdateCategoryAsync([FromServices] ICategoryService CategoryService, string id, [FromBody] UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var result = await CategoryService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteCategoryAsync([FromServices] ICategoryService CategoryService, string id, CancellationToken ct = default)
    {
        var result = await CategoryService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteCategoryAsync([FromServices] ICategoryService CategoryService, string id, CancellationToken ct = default)
    {
        var result = await CategoryService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}