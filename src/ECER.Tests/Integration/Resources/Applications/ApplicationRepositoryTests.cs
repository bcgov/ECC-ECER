using ECER.Resources.Documents.Applications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Applications;

[IntegrationTest]
public class ApplicationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IApplicationRepository repository;

  public ApplicationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Host.Services.GetRequiredService<IApplicationRepository>();
  }

  [Fact]
  public async Task SaveDraftApplication_New_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var applicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }));

    applicationId.ShouldNotBeNull();

    var application = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    application.Status.ShouldBe(ApplicationStatus.Draft);
    application.ApplicantId.ShouldBe(applicantId);
    application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear });
  }

  [Fact]
  public async Task SaveDraftApplication_Existing_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }));
    var existintApplicationId = await repository.SaveDraft(new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear, CertificationType.FiveYears }));

    existintApplicationId.ShouldBe(newApplicationId);

    var application = (await repository.Query(new ApplicationQuery { ById = existintApplicationId })).ShouldHaveSingleItem();
    application.Status.ShouldBe(ApplicationStatus.Draft);
    application.ApplicantId.ShouldBe(applicantId);
    application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear, CertificationType.FiveYears });
  }

  [Fact]
  public async Task QueryApplications_ByApplicantId_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }));

    var applications = await repository.Query(new ApplicationQuery { ByApplicantId = applicantId });
    applications.ShouldNotBeEmpty();
    applications.ShouldBeAssignableTo<IEnumerable<Application>>()!.ShouldAllBe(ca => ca.ApplicantId == applicantId);
  }

  [Fact]
  public async Task QueryApplications_ByApplicantIdAndStatus_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var statuses = new[] { ApplicationStatus.Draft, ApplicationStatus.Complete };
    await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }));

    var applications = await repository.Query(new ApplicationQuery { ByApplicantId = applicantId, ByStatus = statuses });
    applications.ShouldNotBeEmpty();
    applications.ShouldBeAssignableTo<IEnumerable<Application>>()!.ShouldAllBe(ca => ca.ApplicantId == applicantId && statuses.Contains(ca.Status));
  }
}
