using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Resources.Documents.Applications;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;
using Application = ECER.Resources.Documents.Applications.Application;
using ApplicationStatus = ECER.Resources.Documents.Applications.ApplicationStatus;
using CertificationType = ECER.Resources.Documents.Applications.CertificationType;
using CharacterReference = ECER.Resources.Documents.Applications.CharacterReference;
using Transcript = ECER.Resources.Documents.Applications.Transcript;

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
      Transcripts = [new Transcript(null, null, null, null, null, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-5)) { CampusLocation = "test" }]
    });

    var applications = await repository.Query(new ApplicationQuery { ById = applicationId });
    var application = applications.ShouldHaveSingleItem();
    application.Transcripts.ShouldHaveSingleItem();
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
        new Transcript(null,"Test Institution","Test Program","Test Student","123456",DateTime.Now.AddYears(-2),DateTime.Now.AddYears(-1)) {
           LanguageofInstruction = "English",
           CampusLocation = "Test Campus",
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
      new Transcript(null,"Test Institution","Test Program","Test Student","123456",DateTime.Now.AddYears(-2),DateTime.Now.AddYears(-1)) {
      LanguageofInstruction = "English",
      CampusLocation = "Test Campus",
      },
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveDraft(application);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var transcript = query.First().Transcripts.First();
    transcript.CampusLocation = "Updated Campus";

    var updatedTranscripts = new List<Transcript> { transcript };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = updatedTranscripts;
    await repository.SaveDraft(application);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.Transcripts.First().CampusLocation.ShouldBe("Updated Campus");
  }

  [Fact]
  public async Task UpdateApplication_RemoveTranscripts_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalTranscripts = new List<Transcript> {
     new Transcript(null,"Test Institution","Test Program","Test Student","123456",DateTime.Now.AddYears(-2),DateTime.Now.AddYears(-1)) {
     LanguageofInstruction = "English",
     CampusLocation = "Test Campus",
     },
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

  [Fact]
  public async Task SaveDraftApplication_WithCharacterReferences_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var characterReferences = new List<CharacterReference>
    {
        CreateCharacterReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      CharacterReferences = characterReferences
    };
    var applicationId = await repository.SaveDraft(application);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.CharacterReferences.Count().ShouldBe(characterReferences.Count);
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedCharacterReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalCharacterReferences = new List<CharacterReference> {
      CreateCharacterReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = originalCharacterReferences;
    var applicationId = await repository.SaveDraft(application);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var characterReference = query.First().CharacterReferences.First();
    characterReference.FirstName = "Roberto";
    characterReference.LastName = "Firmino";

    var updatedCharacterReferences = new List<CharacterReference> { characterReference };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = updatedCharacterReferences;
    await repository.SaveDraft(application);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.CharacterReferences.First().FirstName.ShouldBe("Roberto");
    updatedApplication.CharacterReferences.First().LastName.ShouldBe("Firmino");
  }

  [Fact]
  public async Task UpdateApplication_RemoveCharacterReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalCharacterReferences = new List<CharacterReference> {
      CreateCharacterReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = originalCharacterReferences;
    var applicationId = await repository.SaveDraft(application);

    // Update application with empty character reference list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = new List<CharacterReference>();
    await repository.SaveDraft(application);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.CharacterReferences.ShouldBeEmpty();
  }
  
  private CharacterReference CreateCharacterReference()
  {
    return new Faker<CharacterReference>("en_CA")
      .RuleFor(f => f.FirstName, f => f.Name.FirstName())
      .RuleFor(f => f.LastName, f => f.Name.LastName())
      .RuleFor(f => f.EmailAddress, f => f.Internet.Email())
      .RuleFor(f => f.PhoneNumber, f => f.Phone.PhoneNumber())
      .Generate();
  }
}
