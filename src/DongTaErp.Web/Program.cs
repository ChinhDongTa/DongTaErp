using DongTaErp.Application;
using DongTaErp.Infrastructure;
using DongTaErp.Web;
using DongTaErp.Web.Components;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructureServices();
builder.AddApplicationServices();
builder.AddWebServices();

builder.Services.AddRazorComponents()
.AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

builder.AddServiceDefaults();


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //using var scope = app.Services.CreateScope();
    //await DbSeeder.SeedAsync(scope.ServiceProvider);
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
