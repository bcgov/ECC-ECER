using ECER.Infrastructure.Common;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ECER.Clients.Api;

#pragma warning disable RCS1102 // Make class static
#pragma warning disable S1118 // Utility classes should not have public constructors

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
        opts.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
        {
          Type = SecuritySchemeType.Http,
          Scheme = "bearer",
          BearerFormat = "JWT",
          Description = "JWT Authorization header using the Bearer scheme."
        });
        opts.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                },
                []
            }
        }); opts.UseOneOfForPolymorphism();
      });

      builder.Services.Configure<JsonOptions>(opts => opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
      builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
      builder.Services
        .AddAuthentication()
        .AddJwtBearer("api", opts =>
        {
          opts.Events = new JwtBearerEvents
          {
            OnTokenValidated = async ctx =>
            {
              await Task.CompletedTask;

              var aud = ctx.Principal?.FindFirst("aud")?.Value;
              if (aud!.Contains("ecer-api"))
              {
                ctx.Principal!.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "ecer-api") }));
              }
              if (aud!.Contains("ecer-ew"))
              {
                ctx.Principal!.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "ecer-ew") }));
              }
            }
          };
          opts.Validate();
        });

      builder.Services.AddAuthorizationBuilder()
        .AddDefaultPolicy("api_user", policy =>
        {
          policy
            .AddAuthenticationSchemes("api")
            .RequireClaim(ClaimTypes.Name, "ecer-api")
            .RequireAuthenticatedUser();
        }).AddPolicy("ew_user", policy =>
        {
          policy
            .AddAuthenticationSchemes("api")
            .RequireClaim(ClaimTypes.Name, "ecer-ew")
            .RequireAuthenticatedUser();
        });

      builder.Services
        .AddProblemDetails()
        .AddCorsPolicy(builder.Configuration.GetSection("cors").Get<CorsSettings>())
        .ConfigureDistributedCache(builder.Configuration.GetSection("DistributedCache").Get<DistributedCacheSettings>())
        .ConfigureDataProtection(builder.Configuration.GetSection("DataProtection").Get<DataProtectionSettings>())
        .ConfigureHealthChecks()
        .AddResponseCompression(opts => opts.EnableForHttps = true)
        .AddRequestDecompression()
        .AddResponseCaching()
        .Configure<CspSettings>(builder.Configuration.GetSection("ContentSecurityPolicy"))
        .AddHttpClient();

      builder.ConfigureComponents(logger);

      var app = builder.Build();

      app.UseHealthChecks();
      app.UseObservabilityMiddleware();
      app.UseDisableHttpVerbsMiddleware(app.Configuration.GetValue("DisabledHttpVerbs", string.Empty));
      app.UseRequestDecompression();
      app.UseResponseCompression();
      app.UseCsp();
      app.UseSecurityHeaders();
      app.UseCors();
      app.UseOutputCache();
      app.UseResponseCaching();
      app.UseAuthentication();
      app.UseAuthorization();

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
