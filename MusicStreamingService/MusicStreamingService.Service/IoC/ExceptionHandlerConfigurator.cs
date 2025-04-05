using Microsoft.AspNetCore.Diagnostics;
using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.Service.IoC;

public static class ExceptionHandlerConfigurator
{
    public static void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseExceptionHandler(config =>
            config.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception is null)
                    return;

                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                
                logger.LogError(exception, 
                    "Unhandled exception: {Message}\nPath: {Path}\nMethod: {Method}",
                    exception.Message,
                    context.Request.Path,
                    context.Request.Method);
                
                string errorCode, message;
                if (exception is BusinessLogicException ex)
                {
                    context.Response.StatusCode = ex.StatusCode;
                    errorCode = ex.ErrorCode;
                    message = ex.Message;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    errorCode = "INTERNAL_ERROR";
                    message = "Internal Server Error";
                }
                
                var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
                if (environment.IsDevelopment())
                {
                    var response = new { Code = errorCode, Message = message, Detail = exception.ToString() };
                    await context.Response.WriteAsJsonAsync(response);
                }
                else
                {
                    var response = new { Code = errorCode, Message = message };
                    await context.Response.WriteAsJsonAsync(response);
                }
            }));
    }
}