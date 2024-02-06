using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ECER.Utilities.Hosting;

public static class ObservabilityExtensions
{
  /// <summary>
  /// Configures observability instruments like logging to the web application and return an initial logger
  /// </summary>
  /// <returns>A logger that can be used during starting up the web application</returns>
  public static ILogger ConfigureWebApplicationObservability(this WebApplicationBuilder builder)
  {
    var logger = CreateBootstrapLogger(builder.Configuration);

    builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services));

    return logger;
  }

  /// <summary>
  /// Adds observability instruments like logging to the web application's middleware pipelines
  /// </summary>
  public static void UseObservabilityMiddleware(this WebApplication webApplication)
  {
    webApplication.UseSerilogRequestLogging();
  }

  private static ILogger CreateBootstrapLogger(IConfiguration configuration)
  {
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateBootstrapLogger();
    return Log.Logger;
  }
}
