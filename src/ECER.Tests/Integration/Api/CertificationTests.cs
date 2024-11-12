using Alba;
using Xunit.Abstractions;
using ECER.Clients.Api.Certifications;
using Shouldly;

namespace ECER.Tests.Integration.Api;

public class CertificationTests : ApiWebAppScenarioBase
{
  public CertificationTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetCertifications_ReturnsCertifications()
  {
    var certificationsResponse = await Host.Scenario(_ =>
    {
      _.Get.Url("/api/certifications/files");
      _.StatusCodeShouldBeOk();
    });

    var certifications = await certificationsResponse.ReadAsJsonAsync<CertificationSummary[]>();
    certifications.ShouldNotBeNull();
  }
}
