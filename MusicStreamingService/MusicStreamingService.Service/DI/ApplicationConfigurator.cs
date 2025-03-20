using MusicStreamingService.Service.IoC;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.DI;

public static class ApplicationConfigurator
{
    public static void ConfigureServices(WebApplicationBuilder builder, MusicServiceSettings settings)
    {
        AuthorizationConfigurator.ConfigureServices(builder.Services, settings);
        DbContextConfigurator.ConfigureServices(builder.Services, settings);
        SerilogConfigurator.ConfigureServices(builder);
        SwaggerConfigurator.ConfigureServices(builder.Services);
    }

    public static void ConfigureApplication(WebApplication app)
    {
        AuthorizationConfigurator.ConfigureApplication(app);
        SerilogConfigurator.ConfigureApplication(app);
        SwaggerConfigurator.ConfigureApplication(app);
        DbContextConfigurator.ConfigureApplication(app);
    }
}