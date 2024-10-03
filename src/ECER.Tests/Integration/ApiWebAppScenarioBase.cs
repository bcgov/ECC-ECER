using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("ApiWebAppScenario")]
public class ApiWebAppScenarioCollectionFixture : ICollectionFixture<ApiWebAppFixture>;

[Collection("ApiWebAppScenario")]
public abstract class ApiWebAppScenarioBase : WebAppScenarioBase
{
  protected new ApiWebAppFixture Fixture => (ApiWebAppFixture)base.Fixture;

  protected ApiWebAppScenarioBase(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class ApiWebAppFixture : WebAppFixtureBase
{
  private IServiceScope serviceScope = null!;

  public IServiceProvider Services => serviceScope.ServiceProvider;

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("api_user", new AuthorizationPolicyBuilder(opts.GetPolicy("api_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.DefaultPolicy = opts.GetPolicy("api_user")!;
  }

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.Api.Program>();
    serviceScope = Host.Services.CreateScope();
  }

  public override async Task DisposeAsync()
  {
    await Task.CompletedTask;
    serviceScope?.Dispose();
  }
}
