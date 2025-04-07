using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using ECER.Clients.E2ETestData.Authentication;
using ECER.Infrastructure.Common;

namespace ECER.Clients.E2ETestData;


internal class Program
{
  private static async Task<int> Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    var assemblies = ReflectionExtensions.DiscoverLocalAessemblies(prefix: "ECER.");

    var logger = builder.ConfigureWebApplicationObservability(discoveryAssemblies: assemblies);
    logger.Information("Starting up");

    try
    {
      builder.Services.AddMediatR(opts => opts.RegisterServicesFromAssemblies(assemblies));
      builder.Services.AddAutoMapper(cfg =>
      {
        cfg.ShouldUseConstructor = constructor => constructor.IsPublic;
      }, assemblies);

      // Configure Swagger/OpenAPI with API Key authentication definition
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(opts =>
      {
        opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        opts.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = "X-API-KEY",
          Type = SecuritySchemeType.ApiKey,
          Description = "API Key needed to access the endpoints."
        });
        opts.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                        },
                        Array.Empty<string>()
                    }
                });
        opts.UseOneOfForPolymorphism();
      });

      builder.Services.Configure<JsonOptions>(opts =>
          opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
      builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(opts =>
          opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

      // Register API key authentication
      builder.Services
          .AddAuthentication("ApiKeyScheme")
          .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", options => { });

      // Define an authorization policy (requires claim "APIKeyUser")
      builder.Services.AddAuthorization(options =>
      {
        options.AddPolicy("api_user", policy =>
        {
          policy.AddAuthenticationSchemes("ApiKeyScheme")
                .RequireClaim(System.Security.Claims.ClaimTypes.Name, "APIKeyUser")
                .RequireAuthenticatedUser();
        });
      });

      builder.Services
          .ConfigureHealthChecks()
          .AddProblemDetails()
          .AddHttpClient();

      builder.ConfigureComponents(logger);

      var app = builder.Build();

      app.UseAuthentication();
      app.UseAuthorization();
      app.UseHealthChecks();

      app.UseSwagger();
      if (app.Environment.IsDevelopment())
        app.UseSwaggerUI();

      app.RegisterApiEndpoints();

      await app.RunAsync();
      logger.Information("Stopped");
      return 0;
    }
    catch (Exception e)
    {
      logger.Fatal(e, "An unhandled exception occurred during bootstrapping");
      return -1;
    }
  }
}
