﻿using ECER.Resources.Documents.Applications;
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
    application.SignedDate.ShouldBeNull();
  }

  [Fact]
  public async Task SaveDraftApplication_Existing_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }));
    var existingApplicationId = await repository.SaveDraft(new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear, CertificationType.FiveYears }));

    existingApplicationId.ShouldBe(newApplicationId);

    var application = (await repository.Query(new ApplicationQuery { ById = existingApplicationId })).ShouldHaveSingleItem();
    application.Status.ShouldBe(ApplicationStatus.Draft);
    application.ApplicantId.ShouldBe(applicantId);
    application.CertificationTypes.ShouldBe(new[] { CertificationType.OneYear, CertificationType.FiveYears });
    application.SignedDate.ShouldBeNull();
  }
  
  [Fact]
  public async Task SaveDraftApplication_Existing_Signed_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }));
    var application = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    application.SignedDate = DateTime.Now;
    var existingApplicationId = await repository.SaveDraft(application);

    existingApplicationId.ShouldBe(newApplicationId);

    var existingApplication = (await repository.Query(new ApplicationQuery { ById = existingApplicationId })).ShouldHaveSingleItem();
    existingApplication.Status.ShouldBe(ApplicationStatus.Draft);
    existingApplication.ApplicantId.ShouldBe(applicantId);
    existingApplication.SignedDate.ShouldNotBeNull();
  }
  
    
  [Fact]
  public async Task SaveDraftApplication_Existing_AlreadySigned_Updated_ShouldThrowInvalidOperation()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplication = new Application(null, applicantId, new[] { CertificationType.OneYear });
    newApplication.SignedDate = DateTime.Now;
    var newApplicationId = await repository.SaveDraft(newApplication);
    var existingApplication = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    existingApplication.SignedDate = DateTime.Now;
    await Should.ThrowAsync<InvalidOperationException>(async () =>
    {
      await repository.SaveDraft(existingApplication);
    });
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
