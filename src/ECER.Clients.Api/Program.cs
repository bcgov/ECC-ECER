using ECER.Infrastructure.Common;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace ECER.Clients.Api;

#pragma warning disable RCS1102 // Make class static
#pragma warning disable S1118 // Utility classes should not have public constructors

public class Program
{
  private static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    var logger = builder.ConfigureWebApplicationObservability();

    logger.Information("Starting up");

    try
    {
      var assemblies = ReflectionExtensions.DiscoverLocalAessemblies(prefix: "ECER.");

      builder.Services.AddMediatR(opts =>
      {
        opts.RegisterServicesFromAssemblies(assemblies);
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

      builder.Services.AddCorsPolicy(builder.Configuration.GetSection("cors").Get<CorsSettings>());

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

      builder.Services.ConfigureDistributedCache(builder.Configuration.GetSection("DistributedCache").Get<DistributedCacheSettings>());
      builder.Services.ConfigureDataProtection(builder.Configuration.GetSection("DataProtection").Get<DataProtectionSettings>());
      builder.Services.ConfigureHealthChecks();
      builder.Services.AddResponseCompression(opts => opts.EnableForHttps = true);
      builder.Services.AddResponseCaching();
      builder.Services.Configure<CspSettings>(builder.Configuration.GetSection("ContentSecurityPolicy"));

      builder.ConfigureComponents();

      var app = builder.Build();

      app.UseHealthChecks();
      app.UseObservabilityMiddleware();
      app.UseDisableHttpVerbsMiddleware(app.Configuration.GetValue("DisabledHttpVerbs", string.Empty));
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

      app.RegisterApiEndpoints();

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
