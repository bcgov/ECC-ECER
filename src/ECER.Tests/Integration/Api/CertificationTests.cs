using Alba;
using ECER.Clients.Api.Certifications;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using ECER.Utilities.Security;
using AdminCertifications = ECER.Managers.Admin.Contract.Certifications;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.IdentityModel.Claims;
using System.Text;
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
      var messageBus = Fixture.Services.GetRequiredService<IMediator>();
      var certificationResult = await messageBus.Send<AdminCertifications.GetCertificationsCommandResponse>(new AdminCertifications.GetCertificationsCommand(testFile.Id), CancellationToken.None);
      var certification = certificationResult.Items.Single(c => c.Id == testFile.Id);
      var objectStorageProvider = Fixture.Services.GetRequiredService<IObjectStorageProviderResolver>().resolve(EcerWebApplicationType.Registry);
      await objectStorageProvider.StoreAsync(
        new S3Descriptor(objectStorageProvider.BucketName, certification.FileId!, certification.FilePath ?? string.Empty),
        new FileObject(certification.FileName ?? "certification.json", "application/json", new MemoryStream(Encoding.UTF8.GetBytes("{}")), null),
        CancellationToken.None);

      var response = await Host.Scenario(_ =>
      {
        _.WithClaim(ClaimTypes.Name, ApiClaims.EcerEwUser);
        _.Get.Url($"/api/certifications/file/download/{testFile.Id}");
        _.StatusCodeShouldBeOk();
      });
    }
  }
}
