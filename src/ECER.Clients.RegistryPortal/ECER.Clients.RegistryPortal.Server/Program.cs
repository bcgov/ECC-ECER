using System.Reflection;
using System.Security.Claims;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry;
using ECER.Utilities.Hosting;
using ECER.Utilities.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
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
        });
        builder.Services.AddProblemDetails();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = builder.Configuration.GetValue("cors:allowedOrigins", string.Empty)!.Split(";");
                policy
                    .WithOrigins(allowedOrigins)
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
            });
        });

        builder.Services.AddAuthentication("bcsc")
            .AddJwtBearer("bceid", opts =>
            {
                opts.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        ctx.Principal = await GetAdditionalClaimsForUser(
                            ctx.Principal,
                            ctx.HttpContext.RequestServices.GetRequiredService<IMessageBus>(),
                            ctx.HttpContext.RequestServices.GetRequiredService<IDistributedCache>(),
                            ctx.HttpContext.RequestAborted);
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

                        ctx.Principal!.AddIdentity(new ClaimsIdentity(new[] { new Claim("identity_provider", "bcsc") }));
                        ctx.Principal = await GetAdditionalClaimsForUser(
                            ctx.Principal,
                            ctx.HttpContext.RequestServices.GetRequiredService<IMessageBus>(),
                            ctx.HttpContext.RequestServices.GetRequiredService<IDistributedCache>(),
                            ctx.HttpContext.RequestAborted);
                    }
                };
                opts.Validate();
            });

        builder.Services.AddAuthorizationBuilder().AddDefaultPolicy("jwt", policy =>
        {
            policy.AddAuthenticationSchemes("bcsc", "bceid").RequireAuthenticatedUser();
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

    private static async Task<ClaimsPrincipal?> GetAdditionalClaimsForUser(ClaimsPrincipal? principal, IMessageBus messageBus, IDistributedCache distributedCache, CancellationToken ct)
    {
        var identity = principal?.GetUserContext()?.Identity;
        if (identity == null) return principal;

        var userProfileResponse = await distributedCache.GetAsync(
            $"{identity.UserId}@{identity.IdentityProvider}",
            async ct => await messageBus.InvokeAsync<UserProfileQueryResponse>(new UserProfileQuery(identity), ct),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
            ct);
        if (userProfileResponse.UserId == null) return principal;

        principal!.AddIdentity(new ClaimsIdentity(new[]
        {
            new Claim("userId", userProfileResponse.UserId)
        }));

        return principal;
    }
}