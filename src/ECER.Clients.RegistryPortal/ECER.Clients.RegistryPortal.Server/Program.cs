using System.Reflection;
using System.Security.Claims;
using ECER.Infrastructure.Common;
using ECER.Utilities.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wolverine;

namespace ECER.Clients.RegistryPortal.Server;

#pragma warning disable RCS1102 // Make class static
#pragma warning disable S1118 // Utility classes should not have public constructors

public class Program
{
  private static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

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
      .AddTransient<AuthenticationService>()
      .AddAuthentication()
      .AddJwtBearer("bceid", opts =>
      {
        opts.Events = new JwtBearerEvents
        {
          OnTokenValidated = async ctx =>
            {
              ctx.Principal!.AddIdentity(new ClaimsIdentity(new[] { new Claim("identity_id", ctx.Principal!.FindFirstValue("bceid_user_guid") ?? string.Empty) }));
              ctx.Principal = await ctx.HttpContext.RequestServices.GetRequiredService<AuthenticationService>().EnrichUserSecurityContext(ctx.Principal, ctx.HttpContext.RequestAborted);
            }
        };
        opts.Validate();
      })
      .AddJwtBearer("bcsc", opts =>
      {
        opts.Events = new JwtBearerEvents
        {
          OnTokenValidated = async ctx =>
            {
              await Task.CompletedTask;

              ctx.Principal!.AddIdentity(new ClaimsIdentity(new[]
              {
                new Claim("identity_provider", "bcsc"),
                new Claim("identity_id", ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty)
              }));
              ctx.Principal = await ctx.HttpContext.RequestServices.GetRequiredService<AuthenticationService>().EnrichUserSecurityContext(ctx.Principal, ctx.HttpContext.RequestAborted);
            }
        };
        opts.Validate();
      });

    builder.Services.AddAuthorizationBuilder()
      .AddDefaultPolicy("registry_user", policy =>
      {
        policy
          .AddAuthenticationSchemes("bcsc", "bceid")
          .RequireClaim("identity_provider")
          .RequireClaim("identity_id")
          .RequireClaim("user_id")
          .RequireAuthenticatedUser();
      })
      .AddPolicy("registry_new_user", policy =>
      {
        policy
          .AddAuthenticationSchemes("bcsc", "bceid")
          .RequireClaim("identity_provider")
          .RequireClaim("identity_id")
          .RequireAuthenticatedUser();
      });

    builder.Services.AddDistributedMemoryCache();

    HostConfigurer.ConfigureAll(builder.Services, builder.Configuration);

    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    EndpointsRegistrar.RegisterAll(app);

    await app.RunAsync();
  }
}
