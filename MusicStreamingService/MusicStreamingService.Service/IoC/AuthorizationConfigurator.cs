﻿using System.Security.Claims;
using System.Text;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.Service.Init;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.IoC;

public static class AuthorizationConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        IdentityModelEventSource.ShowPII = true;
        
        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<MusicServiceDbContext>()
            .AddDefaultTokenProviders()
            .AddRoles<Role>(); 

        services.AddIdentityServer(options =>
            {
                
            })
            .AddInMemoryIdentityResources(IdentityServerConfigSettings.IdentityResources)
            .AddInMemoryApiScopes(IdentityServerConfigSettings.ApiScopes)
            .AddInMemoryApiResources(IdentityServerConfigSettings.ApiResources)
            .AddInMemoryClients(IdentityServerConfigSettings.GetClients(settings))
            .AddAspNetIdentity<User>()
            .AddDeveloperSigningCredential();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = settings.IdentityServerUri;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.ClientSecret)),
                    ValidateIssuer = true, 
                    ValidateAudience = true, 
                    ValidIssuer = settings.IdentityServerUri,
                    ValidAudience = "api",
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RoleClaimType = ClaimTypes.Role
                };
            });
        services.AddAuthorization();

        services.AddTransient<IProfileService, IdentityProfileService>();
    }

    public static void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();
    }
}