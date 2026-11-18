using ECER.Clients.RegistryPortal.Server.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System.Net;

namespace ECER.Tests.Unit.RegistryPortal;

public class RateLimitingTests
{
  private static async Task<IHost> StartAsync()
  {
    var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
      ["RateLimiting:PublicCaptcha:PermitLimit"] = "2",
      ["RateLimiting:PublicCaptcha:WindowSeconds"] = "60"
    }).Build();

    return await new HostBuilder().ConfigureWebHost(web => web
      .UseTestServer()
      .ConfigureServices(services =>
      {
        services.AddRouting();
        services.AddForwardedHeadersSupport(config);
        services.AddPublicRateLimiting(config);
      })
      .Configure(app =>
      {
        app.UseForwardedHeaders();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseEndpoints(e => e.MapGet("/limited", () => "ok").RequireRateLimiting(RateLimitPolicies.PublicCaptcha));
      })).StartAsync();
  }

  private static async Task<HttpStatusCode> GetAsync(HttpClient client, string forwardedFor)
  {
    using var request = new HttpRequestMessage(HttpMethod.Get, "/limited");
    request.Headers.Add("X-Forwarded-For", forwardedFor);
    using var response = await client.SendAsync(request);
    return response.StatusCode;
  }

  [Fact]
  public async Task PublicCaptchaPolicy_LimitsPerForwardedClientIp()
  {
    using var host = await StartAsync();
    var client = host.GetTestClient();

    (await GetAsync(client, "203.0.113.1")).ShouldBe(HttpStatusCode.OK);
    (await GetAsync(client, "203.0.113.1")).ShouldBe(HttpStatusCode.OK);
    (await GetAsync(client, "203.0.113.1")).ShouldBe(HttpStatusCode.TooManyRequests);
    (await GetAsync(client, "203.0.113.2")).ShouldBe(HttpStatusCode.OK);
  }

  [Fact]
  public async Task PublicCaptchaPolicy_IgnoresSpoofedLeftmostForwardedEntries()
  {
    using var host = await StartAsync();
    var client = host.GetTestClient();

    (await GetAsync(client, "1.1.1.1, 203.0.113.9")).ShouldBe(HttpStatusCode.OK);
    (await GetAsync(client, "2.2.2.2, 203.0.113.9")).ShouldBe(HttpStatusCode.OK);
    (await GetAsync(client, "3.3.3.3, 203.0.113.9")).ShouldBe(HttpStatusCode.TooManyRequests);
  }
}
