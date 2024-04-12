using Alba;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi
{
  public class ConfigurationTests : RegistryPortalWebAppScenarioBase
  {
    public ConfigurationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
    {
    }

    [Fact]
    public async Task GetConfigurations_ReturnsConfigurations()
    {
      var configurationsResponse = await Host.Scenario(_ =>
      {
        _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
        _.Get.Url("/api/configuration");
        _.StatusCodeShouldBeOk();
      });

      var configurations = await configurationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.ApplicationConfiguration>();
      configurations.ShouldNotBeNull();
    }
  }
}
