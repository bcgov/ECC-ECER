using ECER.Infrastructure.Common;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace ECER.Clients.RegistryPortal.Server;

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
        .AddTransient<AuthenticationService>()
        .AddAuthentication()
        .AddJwtBearer("kc", opts =>
         {
           opts.Events = new JwtBearerEvents
           {
             OnTokenValidated = async ctx =>
             {
               ctx.Principal!.AddIdentity(new ClaimsIdentity(new[]
               {
                  new Claim(RegistryPortalClaims.IdentityId, ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty)
                }));
               ctx.Principal = await ctx.HttpContext.RequestServices.GetRequiredService<AuthenticationService>().EnrichUserSecurityContext(ctx.Principal!, ctx.HttpContext.RequestAborted);
             }
           };
           opts.Validate();
         });

      builder.Services.AddAuthorizationBuilder()
        .AddDefaultPolicy("registry_user", policy =>
        {
          policy
            .AddAuthenticationSchemes("kc")
            .RequireClaim(RegistryPortalClaims.IdenityProvider)
            .RequireClaim(RegistryPortalClaims.IdentityId)
            .RequireClaim(RegistryPortalClaims.UserId)
            .RequireAuthenticatedUser();
        })
        .AddPolicy("registry_new_user", policy =>
        {
          policy
            .AddAuthenticationSchemes("kc")
            .RequireClaim(RegistryPortalClaims.IdentityId)
            .RequireClaim(ClaimTypes.NameIdentifier)
            .RequireAuthenticatedUser();
        });

      builder.Services.ConfigureDistributedCache(builder.Configuration.GetSection("DistributedCache").Get<DistributedCacheSettings>());
      builder.Services.ConfigureDataProtection(builder.Configuration.GetSection("DataProtection").Get<DataProtectionSettings>());
      builder.Services.AddHealthChecks();
      builder.Services.AddResponseCompression(opts => opts.EnableForHttps = true);
      builder.Services.AddResponseCaching();
      builder.Services.Configure<CspSettings>(builder.Configuration.GetSection("ContentSecurityPolicy"));
      builder.Services.ConfigureHealthChecks();

      builder.ConfigureComponents();

      var app = builder.Build();

      app.UseHealthChecks();
      app.UseObservabilityMiddleware();
      app.UseDisableHttpVerbsMiddleware(app.Configuration.GetValue("DisabledHttpVerbs", string.Empty));
      app.UseResponseCompression();
      app.UseCsp();
      app.UseSecurityHeaders();
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.MapFallbackToFile("index.html");
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
