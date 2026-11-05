using DongTaErp.ApiGateway;
using DongTaErp.ApiGateway.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApiGatewayServices();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecurityRequirementTransformer>();
});

var app = builder.Build();




app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DongTaErp.ApiGateway")
               .WithTheme(ScalarTheme.Laserwave)
               .AddPreferredSecuritySchemes("Bearer")
               .AddHttpAuthentication("Bearer", scheme => scheme.Token = "")
               .EnablePersistentAuthentication();
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();


app.Run();
