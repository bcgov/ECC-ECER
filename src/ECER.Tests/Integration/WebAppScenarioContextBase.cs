using Alba;
using Alba.Security;
using Bogus;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration;

[IntegrationTest]
public abstract class WebAppScenarioBase : IAsyncLifetime
{
  protected WebAppFixtureBase Fixture { get; }

  public IAlbaHost Host => Fixture.Host;

  protected Faker Faker { get; } = new Faker("en_CA");

  protected WebAppScenarioBase(ITestOutputHelper output, WebAppFixtureBase fixture)
  {
    this.Fixture = fixture;
    this.Fixture.OutputHelper = output;
  }

  public virtual async Task InitializeAsync()
  {
    await Task.CompletedTask;
  }

  public virtual async Task DisposeAsync()
  {
    await Task.CompletedTask;
  }
}

public abstract class WebAppFixtureBase : IAsyncLifetime, ITestOutputHelperAccessor
{
  public IServiceCollection Services { get; } = new ServiceCollection();

  public string TestRunId { get; } = $"autotest_{Guid.NewGuid().ToString().Substring(0, 4)}_";
  public IAlbaHost Host { get; protected set; } = null!;
  public ITestOutputHelper? OutputHelper { get; set; }

  protected virtual async Task<IAlbaHost> CreateHost<TProgram>(KeyValuePair<string, string?>[]? configurationSettings = null, IEnumerable<IAlbaExtension>? extensions = null)
      where TProgram : class
  {
    if (extensions == null) extensions = Array.Empty<IAlbaExtension>();

    // add jwt authentication stub
#pragma warning disable CA2000 // Dispose objects before losing scope
    extensions = extensions.Append(new JwtSecurityStub());
#pragma warning restore CA2000 // Dispose objects before losing scope

    return await AlbaHost.For<TProgram>(
        builder =>
        {
          builder.ConfigureServices(
                  (_, services) =>
                  {
                    services.AddLogging(loggingBuilder => loggingBuilder.AddXUnit(this));
                    // Configure test authentication and policy - Alba expects JwtBearerDefaults.AuthenticationScheme to automatically configure this
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
                        {
                          opts.Authority = "https://test_server";
                          opts.Audience = "test_client";
                        });

                    services.AddAuthorization(opts =>
                    {
                      opts.AddPolicy("registry_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
                      opts.AddPolicy("registry_new_user", new AuthorizationPolicyBuilder(opts.GetPolicy("registry_new_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
                      opts.DefaultPolicy = opts.GetPolicy("registry_user")!;
                    });
                  });
          var configOverrides = new Dictionary<string, string?>(configurationSettings ?? Enumerable.Empty<KeyValuePair<string, string?>>());
          builder.ConfigureAppConfiguration(
                  (_, configBuilder) =>
                  {
                    configBuilder.AddInMemoryCollection(configOverrides);
                  });
        },
        extensions.ToArray());
  }

  public abstract Task InitializeAsync();

  public async Task DisposeAsync()
  {
    await Task.CompletedTask;
  }
}
