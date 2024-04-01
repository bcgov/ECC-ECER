using Bogus;
using ECER.Resources.Documents.Applications;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Categories;
using Application = ECER.Resources.Documents.Applications.Application;
using ApplicationStatus = ECER.Resources.Documents.Applications.ApplicationStatus;
using CertificationType = ECER.Resources.Documents.Applications.CertificationType;
using CharacterReference = ECER.Resources.Documents.Applications.CharacterReference;
using Transcript = ECER.Resources.Documents.Applications.Transcript;
using WorkExperienceReference = ECER.Resources.Documents.Applications.WorkExperienceReference;

namespace ECER.Tests.Integration.Resources.Applications;

[IntegrationTest]
public class ApplicationRepositoryTests : RegistryPortalWebAppScenarioBase
{
  private readonly IApplicationRepository repository;

  public ApplicationRepositoryTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    repository = Fixture.Services.GetRequiredService<IApplicationRepository>();
  }

  [Fact]
  public async Task SaveDraftApplication_New_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var applicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);

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
    var newApplicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);
    var existingApplicationId = await repository.SaveDraft(new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear, CertificationType.FiveYears }), CancellationToken.None);

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
    var newApplicationId = await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);
    var application = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    application.SignedDate = DateTime.Now;
    var existingApplicationId = await repository.SaveDraft(application, CancellationToken.None);

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
    var newApplicationId = await repository.SaveDraft(newApplication, CancellationToken.None);
    var existingApplication = new Application(newApplicationId, applicantId, new[] { CertificationType.OneYear });
    existingApplication.SignedDate = today;
    var existingApplicationId = await repository.SaveDraft(existingApplication, CancellationToken.None);

    existingApplicationId.ShouldBe(newApplicationId);

    var application = (await repository.Query(new ApplicationQuery { ById = existingApplicationId })).ShouldHaveSingleItem();
    application.SignedDate.ShouldBe(oneWeekAgo);
  }

  [Fact]
  public async Task QueryApplications_ByApplicantId_Found()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);

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
      Transcripts = new List<Transcript> { CreateTranscript() }
    }, CancellationToken.None);

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
    await repository.SaveDraft(new Application(null, applicantId, new[] { CertificationType.OneYear }), CancellationToken.None);

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
        CreateTranscript()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      Transcripts = transcripts
    };
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);
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
        CreateTranscript()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var transcript = query.First().Transcripts.First();
    transcript.CampusLocation = "Updated Campus";

    var updatedTranscripts = new List<Transcript> { transcript };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = updatedTranscripts;
    await repository.SaveDraft(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.Transcripts.First().CampusLocation.ShouldBe("Updated Campus");
  }

  [Fact]
  public async Task UpdateApplication_RemoveTranscripts_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalTranscripts = new List<Transcript> {
        CreateTranscript()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = originalTranscripts;
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);

    // Update application with empty transcripts list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.Transcripts = new List<Transcript>();
    await repository.SaveDraft(application, CancellationToken.None);

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
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);
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
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var characterReference = query.First().CharacterReferences.First();

    var newCharacterReference = new CharacterReference("Roberto", "Firmino", characterReference.PhoneNumber, characterReference.EmailAddress) { Id = characterReference.Id };

    var updatedCharacterReferences = new List<CharacterReference> { newCharacterReference };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = updatedCharacterReferences;
    await repository.SaveDraft(application, CancellationToken.None);

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
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);

    // Update application with empty character reference list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.CharacterReferences = new List<CharacterReference>();
    await repository.SaveDraft(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.CharacterReferences.ShouldBeEmpty();
  }

  [Fact]
  public async Task SaveDraftApplication_WithWorkExperienceReferences_Created()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var workExperienceReferences = new List<WorkExperienceReference>
    {
        CreateWorkExperienceReference()
    };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      WorkExperienceReferences = workExperienceReferences
    };
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);
    applicationId.ShouldNotBeNull();
    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var savedApplication = query.ShouldHaveSingleItem();
    savedApplication.WorkExperienceReferences.Count().ShouldBe(workExperienceReferences.Count);
  }

  [Fact]
  public async Task UpdateApplication_WithModifiedWorkExperienceReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalWorkExperienceReferences = new List<WorkExperienceReference> {
        CreateWorkExperienceReference()
  };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = originalWorkExperienceReferences;
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);

    var query = await repository.Query(new ApplicationQuery { ById = applicationId });
    var reference = query.First().WorkExperienceReferences.First();

    var newWorkExperienceReference = new WorkExperienceReference(reference.FirstName, reference.LastName, reference.EmailAddress, reference.Hours) { Id = reference.Id, PhoneNumber = "987-654-3210" };

    var updatedReferences = new List<WorkExperienceReference> { newWorkExperienceReference };
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = updatedReferences;
    await repository.SaveDraft(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.WorkExperienceReferences.First().PhoneNumber.ShouldBe("987-654-3210");
  }

  [Fact]
  public async Task UpdateApplication_RemoveWorkExperienceReferences_Updated()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var originalWorkExperienceReferences = new List<WorkExperienceReference> {
        CreateWorkExperienceReference()
  };
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = originalWorkExperienceReferences;
    var applicationId = await repository.SaveDraft(application, CancellationToken.None);

    // Update application with empty work experience references list
    application = new Application(applicationId, applicantId, new[] { CertificationType.OneYear });
    application.WorkExperienceReferences = new List<WorkExperienceReference>();
    await repository.SaveDraft(application, CancellationToken.None);

    var updatedApplication = (await repository.Query(new ApplicationQuery { ById = applicationId })).ShouldHaveSingleItem();
    updatedApplication.WorkExperienceReferences.ShouldBeEmpty();
  }

  [Fact]
  public async Task SubmitApplication_QueryShouldReturnStatusSubmitted()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear })
    {
      WorkExperienceReferences = new List<WorkExperienceReference>
      {
        CreateWorkExperienceReference()
      },
      CharacterReferences = new List<CharacterReference>
      {
        CreateCharacterReference()
      },
      Transcripts = new List<Transcript>
      {
        CreateTranscript()
      }
    };
    var savedApplicationId = await repository.SaveDraft(application, CancellationToken.None);

    await repository.Submit(savedApplicationId, CancellationToken.None);

    var submittedApplication = (await repository.Query(new ApplicationQuery { ById = savedApplicationId })).ShouldHaveSingleItem();
    submittedApplication.Status.ShouldBe(ApplicationStatus.Submitted);
  }

  [Fact]
  public async Task SubmitApplicationNotInDraft_ShouldThrowInvalidOperationException()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId;
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear }) { };
    application.Status = ApplicationStatus.Submitted;
    var savedApplicationId = await repository.SaveDraft(application, CancellationToken.None);

    await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.Submit(savedApplicationId, CancellationToken.None));
  }

  [Fact]
  public async Task CancelApplication_QueryShouldnotReturnResults_ThenTryToCancelAgain_ShouldThrowInvalidOperationException()
  {
    var applicantId = Fixture.AuthenticatedBcscUserId2;
    var application = new Application(null, applicantId, new[] { CertificationType.OneYear });
    var savedApplicationId = await repository.SaveDraft(application, CancellationToken.None);

    await repository.Cancel(savedApplicationId, CancellationToken.None);

    (await repository.Query(new ApplicationQuery { ById = savedApplicationId })).ShouldBeEmpty();

    await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.Cancel(savedApplicationId, CancellationToken.None));
  }

  private CharacterReference CreateCharacterReference()
  {
    var faker = new Faker("en_CA");

    return new CharacterReference(
      faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), faker.Phone.PhoneNumber()
    );
  }

  private WorkExperienceReference CreateWorkExperienceReference()
  {
    var faker = new Faker("en_CA");

    return new WorkExperienceReference(
      faker.Name.FirstName(), faker.Name.FirstName(), faker.Internet.Email(), faker.Random.Number(10, 150)
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber()
    };
  }

  private Transcript CreateTranscript()
  {
    var faker = new Faker("en_CA");

    var languages = new List<string> { "English", "French", "Spanish", "German", "Mandarin", "Japanese", "Russian", "Arabic", "Portuguese", "Hindi" };

    return new Transcript(null, faker.Company.CompanyName(), $"{faker.Hacker.Adjective()} Program",
      faker.Name.FullName(), faker.Random.Number(10000000, 99999999).ToString(), faker.Date.Past(), faker.Date.Recent()
    )
    {
      LanguageofInstruction = faker.PickRandom(languages),
      CampusLocation = faker.Address.City()
    };
  }
}
