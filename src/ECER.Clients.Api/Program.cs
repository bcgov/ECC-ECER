using System.Reflection;
using System.Text.Json.Serialization;
using ECER.Infrastructure.Common;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Wolverine;

namespace ECER.Clients.Api;

#pragma warning disable RCS1102 // Make class static
#pragma warning disable S1118 // Utility classes should not have public constructors

public class Program
{
  private static async Task Main(string[] args)
  {
    string[] DisabledHttpVerbs = { "TRACE", "OPTIONS" };

    var builder = WebApplication.CreateBuilder(args);

    var logger = builder.ConfigureWebApplicationObservability();

    logger.Information("Starting up");

    try
    {
      var assemblies = ReflectionExtensions.DiscoverLocalAessemblies(prefix: "ECER.");

      builder.Host.UseWolverine(opts =>
      {
        foreach (var assembly in assemblies)
        {
          opts.Discovery.IncludeAssembly(assembly);
          opts.Discovery.CustomizeHandlerDiscovery(x =>
          {
            x.Includes.WithNameSuffix("Handlers");
          });
        }
      });
      builder.Services.AddAutoMapper(cfg =>
      {
        cfg.ShouldUseConstructor = constructor => constructor.IsPublic;
      }, assemblies);

      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(opts =>
      {
        opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        opts.UseOneOfForPolymorphism();
      });

      builder.Services.Configure<JsonOptions>(opts => opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
      builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

      builder.Services.AddProblemDetails();

      builder.Services.AddCors(options =>
      {
        options.AddDefaultPolicy(policy =>
        {
          var allowedOrigins = builder.Configuration.GetValue("cors:allowedOrigins", string.Empty)!.Split(";");
          policy.WithOrigins(allowedOrigins).SetIsOriginAllowedToAllowWildcardSubdomains();
        });
      });

      builder.Services
        .AddAuthentication()
        .AddJwtBearer("api", opts =>
        {
          opts.Events = new JwtBearerEvents
          {
            OnTokenValidated = async ctx =>
            {
              await Task.CompletedTask;
            }
          };
          opts.Validate();
        });

      builder.Services.AddAuthorizationBuilder()
        .AddDefaultPolicy("api_user", policy =>
        {
          policy
            .AddAuthenticationSchemes("api")
            .RequireAuthenticatedUser();
        });

      builder.Services.AddDistributedMemoryCache();
      builder.ConfigureDataProtection();
      builder.Services.AddHealthChecks();
      builder.Services.AddResponseCompression(opts => opts.EnableForHttps = true);
      builder.Services.AddResponseCaching();
      builder.Services.Configure<CspSettings>(builder.Configuration.GetSection("ContentSecurityPolicy"));

      HostConfigurer.ConfigureAll(builder.Services, builder.Configuration);

      var app = builder.Build();

      app.UseHealthChecks("/health");
      app.UseObservabilityMiddleware();
      app.UseDisableHttpVerbs(DisabledHttpVerbs);
      app.UseResponseCompression();
      app.UseCsp();
      app.UseSecurityHeaders();
      app.UseCors();
      app.UseResponseCaching();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseSwagger();
      if (app.Environment.IsDevelopment())
      {
        app.UseSwaggerUI();
      }

      EndpointsRegistrar.RegisterAll(app);

      await app.RunAsync();
      logger.Information("Stopped");
    }
    catch (Exception e)
    {
      logger.Fatal(e, "An unhandled exception occurred during bootstrapping");
      throw;
    }
  }
}
