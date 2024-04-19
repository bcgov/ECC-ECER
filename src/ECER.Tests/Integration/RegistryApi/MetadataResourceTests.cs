using Alba;
using ECER.Clients.RegistryPortal.Server.MetadataResources;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class MetadataResourceTests : RegistryPortalWebAppScenarioBase
{
  public MetadataResourceTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {

  }

  [Fact]
  public async Task GetProvinces_ReturnsProvinces()
  {
    var provincesResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/provincelist");
      _.StatusCodeShouldBeOk();
    });

    var provinces = await provincesResponse.ReadAsJsonAsync<Province[]>();
    provinces.ShouldNotBeNull();
  }
}
