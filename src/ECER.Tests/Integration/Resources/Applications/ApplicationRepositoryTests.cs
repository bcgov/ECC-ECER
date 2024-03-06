using ECER.Resources.Documents.Applications;
using JasperFx.Core;
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
  public async Task SaveDraftApplication_ExistingSigned_Updated()
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
  public async Task SaveDraftApplication_ExistingShouldNotUpdateSignedDate_Updated()
  {
    var today = DateTime.Today;
    var oneWeekAgo = today - TimeSpan.FromDays(7);

    var applicantId = Fixture.AuthenticatedBcscUserId;
    var newApplication = new Application(null, applicantId, new[] { CertificationType.OneYear });
    newApplication.SignedDate = oneWeekAgo;
    var newApplicationId = await repository.SaveDraft(newApplication);
    var existingApplication = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    existingApplication.SignedDate = today;
    var existingApplicationId = await repository.SaveDraft(existingApplication);

    existingApplicationId.ShouldBe(newApplicationId);

    var application = (await repository.Query(new ApplicationQuery { ById = existingApplicationId })).ShouldHaveSingleItem();
    application.SignedDate.ShouldBe(oneWeekAgo);
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
  public async Task QueryApplications_ByApplictionId_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var applicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = [new Transcript { CampusLocation = "test", StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-5) }]
    });

    var applications = await repository.Query(new ApplicationQuery { ById = applicationId });
    var application = applications.ShouldHaveSingleItem();
    application.ApplicantId.ShouldBe(applicantId);
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

  [Fact]
  public async Task SaveDraftApplication_WithTranscripts_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var transcripts = new List<Transcript>
    {
        new Transcript {
           Id= null,
           EducationalInstitutionName = "Test Institution",
           ProgramName = "Test Program",
           LanguageofInstruction = "English",
           CampusLocation = "Test Campus",
           StudentName = "Test Student",
           StudentNumber = "123456",
           StartDate = DateTime.Now.AddYears(-2),
           EndDate = DateTime.Now.AddYears(-1),
        },
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = transcripts
    };
    var applicationId = await repository.SaveDraft(application);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.Transcripts.Count().ShouldBe(transcripts.Count);
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedTranscripts_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalTranscripts = new List<Transcript> {
      new Transcript {
         Id= null,
         EducationalInstitutionName = "Test Institution",
         ProgramName = "Test Program",
         LanguageofInstruction = "English",
         CampusLocation = "Test Campus",
         StudentName = "Test Student",
         StudentNumber = "123456",
         StartDate = DateTime.Now.AddYears(-2),
         EndDate = DateTime.Now.AddYears(-1),
      }
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveDraft(application);

    // TODO load transcripts from dataverse and update them
    var updatedTranscripts = new List<Transcript> { /* Initialize with updated set of transcripts */ };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = updatedTranscripts;
    await repository.SaveDraft(application);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.Transcripts.Count().ShouldBe(updatedTranscripts.Count);
  }

  [Fact]
  public async Task UpdateApplication_RemoveTranscripts_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalTranscripts = new List<Transcript> {
      new Transcript {
          Id= null,
          EducationalInstitutionName = "Test Institution",
          ProgramName = "Test Program",
          LanguageofInstruction = "English",
          CampusLocation = "Test Campus",
          StudentName = "Test Student",
          StudentNumber = "123456",
          StartDate = DateTime.Now.AddYears(-2),
          EndDate = DateTime.Now.AddYears(-1),
          }
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveDraft(application);

    // Update application with empty transcripts list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = new List<Transcript>();
    await repository.SaveDraft(application);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.Transcripts.ShouldBeEmpty();
  }
}
