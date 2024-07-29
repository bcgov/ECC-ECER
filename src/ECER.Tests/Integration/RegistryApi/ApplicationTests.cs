using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using Shouldly;
using System.Net;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class ApplicationTests : RegistryPortalWebAppScenarioBase
{
  public ApplicationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetApplications_ReturnsApplications()
  {
    var applicationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url("/api/applications");
      _.StatusCodeShouldBeOk();
    });

    var applications = await applicationsResponse.ReadAsJsonAsync<Application[]>();
    applications.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetApplications_ById()
  {
    var application = CreateDraftApplication();
    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().ApplicationId;

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url($"/api/applications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });

    var applicationsById = await applicationByIdResponse.ReadAsJsonAsync<DraftApplication[]>();
    var applicationById = applicationsById.ShouldHaveSingleItem();
    applicationById.Transcripts.ShouldNotBeEmpty();
    applicationById.CharacterReferences.ShouldNotBeEmpty();
    applicationById.WorkExperienceReferences.ShouldNotBeEmpty();
    applicationById.Stage.ShouldBe(PortalStage.CertificationType);
  }

  [Fact]
  public async Task SaveDraftApplication_WithProfessionalDevelopment_Saved()
  {
    var application = CreateDraftApplication();

    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var savedApplication = (await response.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull();
    savedApplication.ApplicationId.ShouldBe(application.Id);
  }

  [Fact]
  public async Task GetApplicationStatus_ById()
  {
    var application = CreateDraftApplication();
    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().ApplicationId;

    var applicationStatusByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url($"/api/applications/{applicationId}/status");
      _.StatusCodeShouldBeOk();
    });

    var applicationStatusById = await applicationStatusByIdResponse.ReadAsJsonAsync<SubmittedApplicationStatus>();
    applicationStatusById!.Id.ShouldBe(application.Id);
    applicationStatusById.TranscriptsStatus.ShouldNotBeEmpty();
    applicationStatusById.CharacterReferencesStatus.ShouldNotBeEmpty();
    applicationStatusById.WorkExperienceReferencesStatus.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task SaveDraftApplication_ExistingDraft_Saved()
  {
    var application = CreateDraftApplication();

    var existingAppResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });
    var existingApplicationId = (await existingAppResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().ApplicationId;
    existingApplicationId.ShouldBe(application.Id);
  }

  [Fact]
  public async Task SaveDraftApplication_WithInvalidTranscript_ReturnsBadRequest()
  {
    var invalidDraftApplication = CreateDraftApplication();
    invalidDraftApplication.Transcripts = new List<Transcript> { CreateInvalidTranscript() };

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(invalidDraftApplication)).ToUrl($"/api/draftapplications/{invalidDraftApplication.Id}");
      _.StatusCodeShouldBe(400);
    });
  }

  private DraftApplication CreateDraftApplication()
  {
    var application = new Faker<DraftApplication>("en_CA")
        .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .RuleFor(f => f.Transcripts, f => f.Make(f.Random.Number(2, 5), () => CreateTranscript()))
        .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference()))
        .RuleFor(f => f.WorkExperienceReferences, f => f.Make(f.Random.Number(2, 5), () => CreateWorkExperienceReference()))
        .RuleFor(f => f.ProfessionalDevelopments, f => f.Make(f.Random.Number(1, 3), () => CreateProfessionalDevelopment()))
        .Generate();

    application.Id = this.Fixture.draftTestApplicationId;
    return application;
  }

  [Fact]
  public async Task SaveDraftApplication_ForUserWithExistingDraft_ReturnsBadRequest()
  {
    // Attempt to save a new draft application for the same user
    var application = CreateDraftApplication();
    application.Id = null;

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications");
      _.StatusCodeShouldBe(500);
    });
  }

  [Fact]
  public async Task SubmitApplication_WithoutEducation_ReturnsBadRequest()
  {
    var submissionRequest = new ApplicationSubmissionRequest(this.Fixture.draftTestApplicationId3);

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, Fixture.AuthenticatedBcscUserId);
      _.Post.Json(submissionRequest).ToUrl("/api/applications");
      _.StatusCodeShouldBe(400);
    });
  }

  [Fact]
  public async Task CancelApplication_ById_ShouldReturnId_QueryApplications_ShouldNotReturnCancelledApplications()
  {
    var application = CreateDraftApplication();
    application.Id = this.Fixture.draftTestApplicationId2;

    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity2, this.Fixture.AuthenticatedBcscUserId2);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().ApplicationId;

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity2, this.Fixture.AuthenticatedBcscUserId2);
      _.Delete.Url($"/api/draftApplications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });

    (await applicationByIdResponse.ReadAsJsonAsync<CancelDraftApplicationResponse>()).ShouldNotBeNull().ApplicationId.ShouldBe(applicationId);
  }

  [Fact]
  public async Task CancelApplication_ById_WithInvalidApplicationId_ShouldReturnBadRequest()
  {
    Guid randomGuid = Guid.NewGuid();

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Delete.Url($"/api/draftApplications/{randomGuid}");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }

  [Fact]
  public async Task UpdateWorkExReference_ForSubmittedApplication_ByReferenceId()
  {
    var applicationId = this.Fixture.submittedTestApplicationId;
    var referenceId = this.Fixture.submittedTestApplicationWorkExperienceRefId;
    WorkExperienceReference newWork = CreateWorkExperienceReference();
    var UpdateWorkExRefResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Json(newWork).ToUrl($"/api/applications/{applicationId}/workexperiencereference/{referenceId}");
      _.StatusCodeShouldBeOk();
    });
    var UpdateWorkExReferenceResponseId = await UpdateWorkExRefResponse.ReadAsJsonAsync<UpdateReferenceResponse>();
    UpdateWorkExReferenceResponseId!.ReferenceId.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task AddNewWorkExReference_ForSubmittedApplication()
  {
    var applicationId = this.Fixture.submittedTestApplicationId4;
    WorkExperienceReference newWork = CreateWorkExperienceReference();
    var UpdateWorkExRefResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Json(newWork).ToUrl($"/api/applications/{applicationId}/workexperiencereference");
      _.StatusCodeShouldBeOk();
    });
    var UpdateWorkExReferenceResponseId = await UpdateWorkExRefResponse.ReadAsJsonAsync<UpdateReferenceResponse>();
    UpdateWorkExReferenceResponseId!.ReferenceId.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task UpdateCharacterReference_ForSubmittedApplication_ByReferenceId()
  {
    var applicationId = this.Fixture.submittedTestApplicationId3;
    var referenceId = this.Fixture.submittedTestApplicationCharacterRefId;
    CharacterReference newCharacter = CreateCharacterReference();
    var UpdateCharacterRefResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Json(newCharacter).ToUrl($"/api/applications/{applicationId}/characterreference/{referenceId}");
      _.StatusCodeShouldBeOk();
    });
    var UpdateCharacterRefResponseId = await UpdateCharacterRefResponse.ReadAsJsonAsync<UpdateReferenceResponse>();
    UpdateCharacterRefResponseId!.ReferenceId.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task AddNewCharacterReference_ForSubmittedApplication()
  {
    var applicationId = this.Fixture.submittedTestApplicationId4;
    CharacterReference newCharacter = CreateCharacterReference();
    var UpdateCharacterRefResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Json(newCharacter).ToUrl($"/api/applications/{applicationId}/characterreference");
      _.StatusCodeShouldBeOk();
    });
    var UpdateCharacterRefResponseId = await UpdateCharacterRefResponse.ReadAsJsonAsync<UpdateReferenceResponse>();
    UpdateCharacterRefResponseId!.ReferenceId.ShouldNotBeEmpty();
  }

  [Fact]
  public async Task ResendWorkExperienceReferenceInvite_ShouldReturnOk()
  {
    var applicationId = this.Fixture.submittedTestApplicationId2;
    var referenceId = this.Fixture.submittedTestApplicationWorkExperienceRefId2;
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Url($"/api/applications/{applicationId}/workExperienceReference/{referenceId}/resendInvite");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task ResendWorkExperienceReferenceInvite_WithBadReferenceId_ShouldReturnBadRequest()
  {
    var applicationId = this.Fixture.submittedTestApplicationId3;
    var referenceId = Guid.NewGuid();
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Post.Url($"/api/applications/{applicationId}/workExperienceReference/{referenceId}/resendInvite");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }

  private Transcript CreateTranscript()
  {
    var languages = new List<string> { "English", "French", "Spanish", "German", "Mandarin", "Japanese", "Russian", "Arabic", "Portuguese", "Hindi" };

    return new Faker<Transcript>("en_CA")
      .RuleFor(f => f.EducationalInstitutionName, f => f.Company.CompanyName())
      .RuleFor(f => f.StudentName, f => f.Name.FullName())
      .RuleFor(f => f.StudentNumber, f => f.Random.Number(10000000, 99999999).ToString())
      .RuleFor(f => f.StartDate, f => f.Date.Past())
      .RuleFor(f => f.EndDate, f => f.Date.Past())
      .RuleFor(f => f.ProgramName, (f, u) => $"{f.Hacker.Adjective()} Program")
      .RuleFor(f => f.LanguageofInstruction, f => f.PickRandom(languages))
      .RuleFor(f => f.CampusLocation, f => f.Address.City())
      .Generate();
  }

  private CharacterReference CreateCharacterReference()
  {
    var faker = new Faker("en_CA");

    return new CharacterReference(
      faker.Name.FirstName(), faker.Name.LastName(), faker.Phone.PhoneNumber(), "Character_Reference@example.com"
    );
  }

  private ProfessionalDevelopment CreateProfessionalDevelopment()
  {
    var faker = new Faker("en_CA");

    return new ProfessionalDevelopment(
        CertificationNumber: faker.Random.AlphaNumeric(10),
        CertificationExpiryDate: faker.Date.Future(),
        DateSigned: faker.Date.Recent(),
        CourseName: faker.Company.CatchPhrase(),
        OrganizationName: faker.Company.CompanyName(),
        StartDate: faker.Date.Past(),
        EndDate: faker.Date.Recent())
    {
      Id = faker.Random.Guid().ToString(),
      OrganizationContactInformation = faker.Phone.PhoneNumber(),
      InstructorName = faker.Name.FullName(),
      NumberOfHours = faker.Random.Int(1, 100),
      Status = faker.PickRandom<ProfessionalDevelopmentStatusCode>()
    };
  }

  private WorkExperienceReference CreateWorkExperienceReference()
  {
    var faker = new Faker("en_CA");

    return new WorkExperienceReference(
       faker.Name.FirstName(), faker.Name.LastName(), "Work_Experience_Reference@example.com", faker.Random.Number(10, 150)
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber()
    };
  }

  private Transcript CreateInvalidTranscript()
  {
    var faker = new Faker("en_CA");
    var invalidTranscript = new Transcript
    {
      EducationalInstitutionName = null,
      ProgramName = null,
      StudentName = null,
      StudentNumber = faker.Random.Number(1000, 9999).ToString(),
      StartDate = faker.Date.Recent(),
      EndDate = faker.Date.Soon(),
      IsECEAssistant = faker.Random.Bool(),
      DoesECERegistryHaveTranscript = faker.Random.Bool(),
      IsOfficialTranscriptRequested = faker.Random.Bool()
    };

    return invalidTranscript;
  }
}
