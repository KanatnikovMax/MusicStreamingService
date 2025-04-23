using Microsoft.AspNetCore.Identity;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.Init;

public static class PostgresInitializer
{
    public static async Task InitializeAsync(IApplicationBuilder app, MusicServiceSettings settings)
    {
        using var scope = app.ApplicationServices.CreateScope();

        await AddRolesAsync(app, scope);
        await CreateMasterAdminAsync(app, scope, settings);
    }

    private static async Task AddRolesAsync(IApplicationBuilder app, IServiceScope scope)
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

    private static async Task CreateMasterAdminAsync(IApplicationBuilder app, IServiceScope scope, 
        MusicServiceSettings settings)
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