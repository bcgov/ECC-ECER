using Alba;
using ECER.Clients.RegistryPortal.Server;
using ECER.Managers.Admin;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class ConfigurationTests : RegistryPortalWebAppScenarioBase
{
  public ConfigurationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetProvinces_ReturnsProvinces()
  {
    var provincesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/provincelist");
      _.StatusCodeShouldBeOk();
    });

    var provinces = await provincesResponse.ReadAsJsonAsync<Province[]>();
    provinces.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetRecaptchaSiteKey_ReturnsSiteKey()
  {
    var siteKeyResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/recaptchaSiteKey");
      _.StatusCodeShouldBeOk();
    });

    var siteKey = await siteKeyResponse.ReadAsJsonAsync<string>();
    siteKey.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetProvinces_Returned()
  {
    var manager = Fixture.Services.GetRequiredService<MetadataHandlers>();
    var provinces = await manager.Handle(new ECER.Managers.Admin.Contract.Metadatas.ProvincesQuery(), default);
    provinces.Items.ShouldNotBeEmpty();
  }
}
