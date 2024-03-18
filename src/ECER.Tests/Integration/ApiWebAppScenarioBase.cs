using System.Globalization;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk.Client;
using Xunit.Abstractions;

namespace ECER.Tests.Integration;

[CollectionDefinition("ApiWebAppScenario")]
public class ApiWebAppScenarioCollectionFixture : ICollectionFixture<ApiWebAppFixture>;

[Collection("RegistryPortalWebAppScenario")]
public abstract class ApiWebAppScenarioBase : WebAppScenarioBase
{
  protected new RegistryPortalWebAppFixture Fixture => (RegistryPortalWebAppFixture)base.Fixture;

  protected ApiWebAppScenarioBase(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class ApiWebAppFixture : WebAppFixtureBase
{

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.Api.Program>();
  }

}
