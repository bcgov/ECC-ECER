using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Clients.RegistryPortal.Server.Files;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.RegistryApi;

public class ApplicationTests : RegistryPortalWebAppScenarioBase
{
  public ApplicationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  private readonly Faker faker = new Faker("en_CA");

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
    application.Stage = "CertificationType";
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
    applicationById.ApplicationType.ShouldBe(ApplicationTypes.New);
    applicationById.Transcripts.ShouldNotBeEmpty();
    applicationById.CharacterReferences.ShouldNotBeEmpty();
    applicationById.WorkExperienceReferences.ShouldNotBeEmpty();
    applicationById.Stage.ShouldBe("CertificationType");
  }

  [Fact]
  public async Task GetRenewalApplications_ById_With400HoursTypeWorkExpRef()
  {
    var application = Create400HoursTypeRenewalDraftApplication();

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
    applicationById.WorkExperienceReferences.All(workExp => workExp.Type == WorkExperienceTypes.Is400Hours).ShouldBeTrue();
    applicationById.ApplicationType.ShouldBe(ApplicationTypes.Renewal);
    applicationById.EducationOrigin.ShouldBeNull();
    applicationById.EducationRecognition.ShouldBeNull();
  }

  [Fact]
  public async Task GetRenewalApplications_ById_ReturnsCorrectEducationData()
  {
    var application = CreateDraftApplication();
    application.ApplicationType = ApplicationTypes.Renewal;
    application.EducationOrigin = EducationOrigin.InsideBC;
    application.EducationRecognition = EducationRecognition.Recognized;
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
    applicationById.ApplicationType.ShouldBe(ApplicationTypes.Renewal);
    applicationById.EducationOrigin.ShouldBe(EducationOrigin.InsideBC);
    applicationById.EducationRecognition.ShouldBe(EducationRecognition.Recognized);
  }

  [Fact]
  public async Task SaveDraftApplication_WithProfessionalDevelopmentAndFiles_Saved()
  {
    var fileLength = 1041;
    var testFile = await faker.GenerateTestFile(fileLength);
    var testFileId = Guid.NewGuid().ToString();
    var testFolder = "tempfolder";
    var testTags = "tag1=1,tag2=2";
    var testClassification = "test-classification";
    using var content = new StreamContent(testFile.Content);
    content.Headers.ContentType = new MediaTypeHeaderValue(testFile.ContentType);

    using var formData = new MultipartFormDataContent
        {
          { content, "file", testFile.FileName }
        };

    var fileResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    var uploadedFileResponse = (await fileResponse.ReadAsJsonAsync<FileResponse>()).ShouldNotBeNull();

    var professionalDevelopment = CreateProfessionalDevelopment();
    professionalDevelopment.NewFiles = [testFolder + "/" + uploadedFileResponse.fileId];
    var application = CreateDraftApplication();
    application.ProfessionalDevelopments = [professionalDevelopment];
    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var savedApplication = (await response.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull();
    savedApplication.ApplicationId.ShouldBe(application.Id);

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Get.Url($"/api/applications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationsById = await applicationByIdResponse.ReadAsJsonAsync<Application[]>();
    applicationsById.ShouldHaveSingleItem();
    var applicationFromServer = applicationsById[0];
    applicationFromServer.ProfessionalDevelopments.ShouldHaveSingleItem();
    var professionalDev = applicationFromServer.ProfessionalDevelopments.First();
    professionalDev.Files.ShouldHaveSingleItem();
    professionalDev.Files.First().ShouldContain(uploadedFileResponse.fileId);
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

  private DraftApplication Create400HoursTypeRenewalDraftApplication()
  {
    var application = new Faker<DraftApplication>("en_CA")
        .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .RuleFor(f => f.Transcripts, f => f.Make(f.Random.Number(2, 5), () => CreateTranscript()))
        .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference()))
        .RuleFor(f => f.WorkExperienceReferences, f => f.Make(f.Random.Number(2, 5), () => Create400HoursTypeWorkExperienceReference()))
        .RuleFor(f => f.ProfessionalDevelopments, f => f.Make(f.Random.Number(1, 3), () => CreateProfessionalDevelopment()))
        .Generate();

    application.Id = this.Fixture.draftTestApplicationId;
    application.ApplicationType = ApplicationTypes.Renewal;
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
      .RuleFor(f => f.StudentFirstName, f => f.Name.FirstName())
      .RuleFor(f => f.StudentLastName, f => f.Name.LastName())
      .RuleFor(f => f.StudentNumber, f => f.Random.Number(10000000, 99999999).ToString())
      .RuleFor(f => f.StartDate, f => f.Date.Past())
      .RuleFor(f => f.EndDate, f => f.Date.Past())
      .RuleFor(f => f.ProgramName, (f, u) => $"{f.Hacker.Adjective()} Program")
      .RuleFor(f => f.LanguageofInstruction, f => f.PickRandom(languages))
      .RuleFor(f => f.CampusLocation, f => f.Address.City())
      .RuleFor(f => f.IsNameUnverified, f => f.Random.Bool())
      .Generate();
  }

  private CharacterReference CreateCharacterReference()
  {
    return new CharacterReference(
      faker.Name.FirstName(), $"{Fixture.TestRunId}{faker.Name.LastName()}", faker.Phone.PhoneNumber(), "Character_Reference@test.gov.bc.ca"
    );
  }

  private ProfessionalDevelopment CreateProfessionalDevelopment()
  {
    return new ProfessionalDevelopment(
        CertificationNumber: faker.Random.AlphaNumeric(10),
        CertificationExpiryDate: faker.Date.Future(),
        DateSigned: faker.Date.Recent(),
        CourseName: faker.Company.CatchPhrase(),
        OrganizationName: faker.Company.CompanyName(),
        StartDate: faker.Date.Past(),
        EndDate: faker.Date.Recent())
    {
      Id = null,
      OrganizationContactInformation = faker.Phone.PhoneNumber(),
      InstructorName = faker.Name.FullName(),
      NumberOfHours = faker.Random.Int(1, 100),
      Status = faker.PickRandom<ProfessionalDevelopmentStatusCode>()
    };
  }

  private WorkExperienceReference CreateWorkExperienceReference()
  {
    return new WorkExperienceReference(
       faker.Name.FirstName(), faker.Name.LastName(), "Work_Experience_Reference@test.gov.bc.ca", faker.Random.Number(10, 150)
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber()
    };
  }

  private WorkExperienceReference Create400HoursTypeWorkExperienceReference()
  {
    return new WorkExperienceReference(
       faker.Name.FirstName(), faker.Name.LastName(), "Work_Experience_Reference@test.gov.bc.ca", faker.Random.Number(10, 150)
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber(),
      Type = WorkExperienceTypes.Is400Hours
    };
  }

  private Transcript CreateInvalidTranscript()
  {
    var invalidTranscript = new Transcript
    {
      EducationalInstitutionName = null,
      ProgramName = null,
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
