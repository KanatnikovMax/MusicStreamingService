using MusicStreamingService.Service.DI;
using MusicStreamingService.Service.Init;
using MusicStreamingService.Service.Settings;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var settings = MusicServiceSettingsReader.ReadSettings(configuration);

var builder = WebApplication.CreateBuilder(args);
ApplicationConfigurator.ConfigureServices(builder, settings);


var app = builder.Build();
ApplicationConfigurator.ConfigureApplication(app);

await PostgresInitializer.InitializeAsync(app, settings);
await app.Services.GetRequiredService<CassandraCluster>().InitializeAsync();

app.Use(async (context, next) => // TODO: remove
{
    context.Request.EnableBuffering();
    
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    context.Request.Body.Position = 0;

    Console.WriteLine(
        $"Request: {context.Request.Method} {context.Request.Path} | Body: {requestBody}"
    );

    await next(); 
});

app.Run();

public partial class Program;