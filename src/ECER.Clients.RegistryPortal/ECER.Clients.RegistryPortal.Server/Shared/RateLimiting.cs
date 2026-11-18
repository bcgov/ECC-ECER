using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Threading.RateLimiting;

namespace ECER.Clients.RegistryPortal.Server.Shared;

public static class RateLimitPolicies
{
  public const string PublicCaptcha = "public-captcha";
}

public record RateLimitSettings
{
  public int PermitLimit { get; set; } = 10;
  public int WindowSeconds { get; set; } = 60;
}

public record ForwardedHeadersSettings
{
  public IList<string> KnownProxies { get; } = new List<string>();
  public IList<string> KnownNetworks { get; } = new List<string>();
}

public static class RateLimitingExtensions
{
  public static IServiceCollection AddPublicRateLimiting(this IServiceCollection services, IConfiguration configuration)
  {
    ArgumentNullException.ThrowIfNull(configuration);
    var settings = configuration.GetSection("RateLimiting:PublicCaptcha").Get<RateLimitSettings>() ?? new RateLimitSettings();

    services.AddRateLimiter(options =>
    {
      options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
      options.AddPolicy(RateLimitPolicies.PublicCaptcha, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
          httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
          _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = Math.Max(1, settings.PermitLimit),
            Window = TimeSpan.FromSeconds(Math.Max(1, settings.WindowSeconds)),
            QueueLimit = 0
          }));
    });

    return services;
  }

  /// <summary>
  /// Resolves the client IP from X-Forwarded-For so per-IP limits apply to the real client. Only the last hop is
  /// honored (ForwardLimit = 1) so client supplied header values cannot be used to evade limits.
  /// </summary>
  public static IServiceCollection AddForwardedHeadersSupport(this IServiceCollection services, IConfiguration configuration)
  {
    ArgumentNullException.ThrowIfNull(configuration);
    var settings = configuration.GetSection("ForwardedHeaders").Get<ForwardedHeadersSettings>() ?? new ForwardedHeadersSettings();

    services.Configure<ForwardedHeadersOptions>(options =>
    {
      options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
      options.ForwardLimit = 1;
      // The app sits behind the platform router and is not directly reachable; trust it unless explicit lists are configured.
      options.KnownIPNetworks.Clear();
      options.KnownProxies.Clear();
      foreach (var proxy in settings.KnownProxies) options.KnownProxies.Add(IPAddress.Parse(proxy));
      foreach (var network in settings.KnownNetworks)
      {
        var parts = network.Split('/');
        options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
      }
    });

    return services;
  }
}
