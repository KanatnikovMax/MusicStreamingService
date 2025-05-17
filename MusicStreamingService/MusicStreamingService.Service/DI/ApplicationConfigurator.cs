using MusicStreamingService.Service.IoC;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.DI;

public static class ApplicationConfigurator
{
    public static void ConfigureServices(WebApplicationBuilder builder, MusicServiceSettings settings)
    {
        SerilogConfigurator.ConfigureServices(builder);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AdminFrontend", policy =>
            {
                policy.WithOrigins(settings.FrontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        AuthorizationConfigurator.ConfigureServices(builder.Services, settings);
        DbContextConfigurator.ConfigureServices(builder.Services, settings);
        SwaggerConfigurator.ConfigureServices(builder.Services);
        MapperConfigurator.ConfigureServices(builder.Services);
        ServicesConfigurator.ConfigureServices(builder.Services, settings);
        
        builder.Services.AddControllers();
    }

    public static void ConfigureApplication(WebApplication app)
    {
        SerilogConfigurator.ConfigureApplication(app);
        app.UseCors("AdminFrontend"); 
        ExceptionHandlerConfigurator.ConfigureApplication(app);
        AuthorizationConfigurator.ConfigureApplication(app);
        SwaggerConfigurator.ConfigureApplication(app);
        DbContextConfigurator.ConfigureApplication(app);
        
        app.MapControllers();
    }
}