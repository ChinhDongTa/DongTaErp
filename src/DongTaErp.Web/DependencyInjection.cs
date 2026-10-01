using DongTaErp.Application.Common.Interfaces;
using DongTaErp.Domain.Entities;
using DongTaErp.Infrastructure.Data;
using DongTaErp.Web.Components.Account;
using DongTaErp.Web.Security;
using DongTaErp.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using IdentityRevalidatingAuthenticationStateProvider = DongTaErp.Web.Components.Account.IdentityRevalidatingAuthenticationStateProvider;


namespace DongTaErp.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
            .AddIdentityCookies();

        builder.Services.AddIdentityCore<AppUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<AppUser>, IdentityNoOpEmailSender>();


        builder.Services.AddCors(o => o.AddPolicy("mobile", p =>
            p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
    }
}