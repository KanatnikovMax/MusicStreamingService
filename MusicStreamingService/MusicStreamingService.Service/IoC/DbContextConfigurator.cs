using Microsoft.EntityFrameworkCore;
using MusicStreamingService.Service.Settings;
using MusicStreamingService.DataAccess.Context;

namespace MusicStreamingService.Service.IoC;

public static class DbContextConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        services.AddDbContextPool<MusicServiceDbContext>(options =>
            options.UseNpgsql(settings.MusicServiceDbConnectionString),
            poolSize: 100
        );
    }

    public static void ConfigureApplication(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MusicServiceDbContext>();
        context.Database.Migrate();
    }
}