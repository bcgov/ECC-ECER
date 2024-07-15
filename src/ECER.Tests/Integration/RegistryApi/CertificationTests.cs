using Alba;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class CertificationsTests : RegistryPortalWebAppScenarioBase
{
  public CertificationsTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetCertifications_ReturnsCertifications()
  {
    var CertificationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/certifications");
      _.StatusCodeShouldBeOk();
    });

    var Certifications = await CertificationsResponse.ReadAsJsonAsync<Clients.RegistryPortal.Server.Certifications.Certification[]>();
    Certifications.ShouldNotBeNull();
  }
}
