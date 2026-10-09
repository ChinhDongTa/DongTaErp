using DongTaErp.ApiGateway;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddApiGatewayServices();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DongTaErp.ApiGateway")
            .WithTheme(ScalarTheme.Kepler)
            ; 
    });
}

app.UseHttpsRedirection();


app.MapEndpoints();


app.Run();
