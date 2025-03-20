using Microsoft.EntityFrameworkCore;
using MusicStreamingService.Service.Settings;
using MusicStreamingService.DataAccess.Context;

namespace MusicStreamingService.Service.IoC;

public static class DbContextConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        services.AddDbContextFactory<MusicServiceDbContext>(options =>
        {
            options.UseNpgsql(settings.MusicServiceDbConnectionString);
        }, ServiceLifetime.Scoped);
    }

    public static void ConfigureApplication(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>();
        using var context = contextFactory.CreateDbContext();
        context.Database.Migrate();
    }
}