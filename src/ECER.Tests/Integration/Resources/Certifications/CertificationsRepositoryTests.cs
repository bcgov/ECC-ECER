using ECER.Resources.Documents.Certifications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;
using UserCertificationQuery = ECER.Resources.Documents.Certifications.UserCertificationQuery;

namespace ECER.Tests.Integration.Resources.Documents.Certifications;

[IntegrationTest]
public class CertificationsRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly ICertificationRepository repository;

  public CertificationsRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<ICertificationRepository>();
  }

  [Fact]
  public async Task QueryCertifications_ById_Found()
  {
    // Arrange
    var CertificationId = Fixture.activeCertificationOneId;

    // Act
    var Certifications = await repository.Query(new UserCertificationQuery { ById = CertificationId });

    // Assert
    Certifications.ShouldHaveSingleItem();
  }

  [Fact]
  public async Task QueryCertifications_ByApplicantId_Found()
  {
    var Certifications = await repository.Query(new UserCertificationQuery { ByApplicantId = Fixture.AuthenticatedBcscUserId });
    Certifications.ShouldHaveSingleItem();
    Certifications.First().StatusCode.ShouldBe(CertificateStatusCode.Active);
  }

  [Fact]
  public async Task QueryCertificateSummary_Found()
  {
    var Certifications = await repository.QueryCertificateSummary(new UserCertificationSummaryQuery { ById = null });
    Certifications.ShouldNotBeEmpty();
  }
}
