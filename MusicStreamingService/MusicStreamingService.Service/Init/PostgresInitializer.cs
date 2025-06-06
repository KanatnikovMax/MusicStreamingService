﻿using Microsoft.AspNetCore.Identity;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.Init;

public static class PostgresInitializer
{
    public static async Task InitializeAsync(IApplicationBuilder app, MusicServiceSettings settings)
    {
        using var scope = app.ApplicationServices.CreateScope();

        await AddRolesAsync(scope);
        await CreateMasterAdminAsync(scope, settings);
    }

    private static async Task AddRolesAsync(IServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        
        var roles = new[]
        {
            "admin", 
            "user" 
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Role { Name = role });
        }
    }

    private static async Task CreateMasterAdminAsync(IServiceScope scope, MusicServiceSettings settings)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var user = await userManager.FindByEmailAsync(settings.MasterAdminEmail);
        if (user is null)
        {
            user = new User
            {
                Email = settings.MasterAdminEmail,
                UserName = settings.MasterAdminEmail,
            };
            await userManager.CreateAsync(user, settings.MasterAdminPassword);
            await userManager.AddToRoleAsync(user, "admin");
        }
    }
}