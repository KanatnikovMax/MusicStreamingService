using MusicStreamingService.Service.DI;
using MusicStreamingService.Service.Init;
using MusicStreamingService.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

var settings = MusicServiceSettingsReader.ReadSettings(builder.Configuration);
ApplicationConfigurator.ConfigureServices(builder, settings);


var app = builder.Build();
ApplicationConfigurator.ConfigureApplication(app);

await PostgresInitializer.InitializeAsync(app, settings);
app.Run();

public partial class Program;
