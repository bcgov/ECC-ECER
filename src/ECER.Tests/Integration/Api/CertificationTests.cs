using Alba;
using ECER.Clients.Api.Certifications;
using ECER.Utilities.Security;
using Shouldly;
using System.IdentityModel.Claims;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Api;

public class CertificationTests : ApiWebAppScenarioBase
{
  public CertificationTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  [Category("Internal")]
  public async Task GetCertifications_ReturnsCertifications()
  {
    var certificationsResponse = await Host.Scenario(_ =>
    {
      _.WithClaim(ClaimTypes.Name, ApiClaims.EcerEwUser);
      _.Get.Url("/api/certifications/files");
      _.StatusCodeShouldBeOk();
    });

    var certifications = await certificationsResponse.ReadAsJsonAsync<CertificationSummary[]>();
    certifications.ShouldNotBeNull();

    var testFile = certifications.Where(c => c.FileId != null).FirstOrDefault();
    if (testFile != null)
    {
      var response = await Host.Scenario(_ =>
      {
        _.WithClaim(ClaimTypes.Name, ApiClaims.EcerEwUser);
        _.Get.Url($"/api/certifications/file/download/{testFile.Id}");
        _.StatusCodeShouldBeOk();
      });
    }
  }
}
