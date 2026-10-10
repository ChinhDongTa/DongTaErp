using DongTaErp.Application.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace DongTaErp.ApiGateway.Endpoints;

public class Products : EndpointGroupBase
{
    public override string? GroupName => "Products";
    public override void Map(RouteGroupBuilder group)
    {
        //=============Data Retrieval Endpoints================
        group.MapGet(GetProductById, "/{id}");
        group.MapGet(GetProducts, "/");

        //=============Data Manipulation Endpoints================
        group.MapPost(CreateProductAsync);
        group.MapPut(UpdateProductAsync, "{id}");
        group.MapPut(SoftDeleteProductAsync, "/soft-delete/{id}");
        group.MapDelete(DeleteProductAsync, "{id}");
    }
    public async Task<IResult> GetProductById([FromServices] IProductService productService, string id, CancellationToken ct=default)
    {
        var result = await productService.GetByIdAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> GetProducts([FromServices] IProductService productService, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var result = await productService.GetAllAsync(pageNumber, pageSize, ct);
        return result.ToHttpResult();
    }

    public async Task<IResult> CreateProductAsync([FromServices] IProductService productService, [FromBody] CreateProductDto dto, CancellationToken ct = default)
    {
        var result = await productService.CreateAsync(dto, ct);
        return result.ToCreatedHttpResult("products");
    }
    public async Task<IResult> UpdateProductAsync([FromServices] IProductService productService, string id, [FromBody] UpdateProductDto dto, CancellationToken ct = default)
    {
        var result = await productService.UpdateAsync(id.ToGuid(), dto, ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> SoftDeleteProductAsync([FromServices] IProductService productService, string id, CancellationToken ct = default)
    {
        var result = await productService.SoftDeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
    public async Task<IResult> DeleteProductAsync ([FromServices] IProductService productService, string id, CancellationToken ct = default)
    {
        var result = await productService.DeleteAsync(id.ToGuid(), ct);
        return result.ToHttpResult();
    }
}
