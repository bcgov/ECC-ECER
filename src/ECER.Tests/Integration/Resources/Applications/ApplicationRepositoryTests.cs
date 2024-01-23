using ECER.Resources.Applications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Resources.Applications;

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
        var applicationId = await repository.SaveDraft(new CertificationApplication(null, applicantId, new[] { CertificationType.OneYear }));

        applicationId.ShouldNotBeNull();

        var application = (await repository.Query(new CertificationApplicationQuery { ById = applicationId })).ShouldHaveSingleItem().ShouldBeOfType<CertificationApplication>();
        application.Status.ShouldBe(ApplicationStatus.Draft);
        application.ApplicantId.ShouldBe(applicantId);
        application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear });
    }

    [Fact]
    public async Task SaveDraftApplication_Existing_Updated()
    {
        var applicantId = Fixture.AuthenticatedBcscUserId;
        var newApplicationId = await repository.SaveDraft(new CertificationApplication(null, applicantId, new[] { CertificationType.OneYear }));
        var existintApplicationId = await repository.SaveDraft(new CertificationApplication(newApplicationId, applicantId, new[] { CertificationType.OneYear, CertificationType.FiveYears }));

        existintApplicationId.ShouldBe(newApplicationId);

        var application = (await repository.Query(new CertificationApplicationQuery { ById = existintApplicationId })).ShouldHaveSingleItem().ShouldBeOfType<CertificationApplication>();
        application.Status.ShouldBe(ApplicationStatus.Draft);
        application.ApplicantId.ShouldBe(applicantId);
        application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear, CertificationType.FiveYears });
    }

    [Fact]
    public async Task QueryApplications_ByApplicantId_Found()
    {
        var applicantId = Fixture.AuthenticatedBcscUserId;
        await repository.SaveDraft(new CertificationApplication(null, applicantId, new[] { CertificationType.OneYear }));

        var applications = await repository.Query(new CertificationApplicationQuery { ByApplicantId = applicantId });
        applications.ShouldNotBeEmpty();
        applications.ShouldBeAssignableTo<IEnumerable<CertificationApplication>>()!.ShouldAllBe(ca => ca.ApplicantId == applicantId);
    }

    [Fact]
    public async Task QueryApplications_ByApplicantIdAndStatus_Found()
    {
        var applicantId = Fixture.AuthenticatedBcscUserId;
        var statuses = new[] { ApplicationStatus.Draft, ApplicationStatus.Complete };
        await repository.SaveDraft(new CertificationApplication(null, applicantId, new[] { CertificationType.OneYear }));

        var applications = await repository.Query(new CertificationApplicationQuery { ByApplicantId = applicantId, ByStatus = statuses });
        applications.ShouldNotBeEmpty();
        applications.ShouldBeAssignableTo<IEnumerable<CertificationApplication>>()!.ShouldAllBe(ca => ca.ApplicantId == applicantId && statuses.Contains(ca.Status));
    }
}