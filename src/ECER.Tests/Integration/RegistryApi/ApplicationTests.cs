using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Clients.RegistryPortal.Server.Files;
using ECER.Clients.RegistryPortal.Server.ICRA;
using ECER.Resources.Documents.ICRA;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;
using Xunit.Categories;

namespace ECER.Tests.Integration.RegistryApi;

public class ApplicationTests : RegistryPortalWebAppScenarioBase
{
  private readonly IICRARepository icraRepository;
  private readonly ECER.Resources.Documents.Applications.IApplicationRepository applicationRepository;

  public ApplicationTests(ITestOutputHelper output, RegistryPortalWebAppFixture fixture) : base(output, fixture)
  {
    icraRepository = Fixture.Services.GetRequiredService<IICRARepository>();
    applicationRepository = Fixture.Services.GetRequiredService<ECER.Resources.Documents.Applications.IApplicationRepository>();
  }

  private readonly Faker faker = new Faker("en_CA");

  [Fact]
  public async Task GetApplications_ReturnsApplications()
  {
    var applicationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/applications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });

    var applicationsById = await applicationByIdResponse.ReadAsJsonAsync<DraftApplication[]>();
    var applicationById = applicationsById.ShouldHaveSingleItem();
    applicationById.ApplicationType.ShouldBe(ApplicationTypes.New);
    applicationById.Origin.ShouldBe(ApplicationOrigin.Portal);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/applications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });

    var applicationsById = await applicationByIdResponse.ReadAsJsonAsync<DraftApplication[]>();
    var applicationById = applicationsById.ShouldHaveSingleItem();
    applicationById.WorkExperienceReferences.All(workExp => workExp.Type == WorkExperienceTypes.Is400Hours).ShouldBeTrue();
    applicationById.ApplicationType.ShouldBe(ApplicationTypes.Renewal);
    applicationById.EducationOrigin.ShouldBeNull();
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    var uploadedFileResponse = (await fileResponse.ReadAsJsonAsync<FileResponse>()).ShouldNotBeNull();

    var professionalDevelopment = CreateProfessionalDevelopment();
    professionalDevelopment.NewFiles = [uploadedFileResponse.fileId];
    var application = CreateDraftApplication();
    application.ProfessionalDevelopments = [professionalDevelopment];
    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var savedApplication = (await response.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull();
    savedApplication.Application.Id.ShouldBe(application.Id);

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/applications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationsById = await applicationByIdResponse.ReadAsJsonAsync<Application[]>();
    applicationsById.ShouldHaveSingleItem();
    var applicationFromServer = applicationsById[0];
    applicationFromServer.ProfessionalDevelopments.ShouldHaveSingleItem();
    var professionalDev = applicationFromServer.ProfessionalDevelopments.First();
    professionalDev.Files.ShouldHaveSingleItem();
    professionalDev.Files.First().Id!.ShouldContain(uploadedFileResponse.fileId);
  }

  [Fact]
  public async Task GetApplicationStatus_ById()
  {
    var application = CreateDraftApplication();
    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var applicationStatusByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
  public async Task GetApplicationStatus_ById_ForRenewalApplications()
  {
    var application = Create400HoursTypeRenewalDraftApplication();
    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var applicationStatusByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/applications/{applicationId}/status");
      _.StatusCodeShouldBeOk();
    });

    var applicationStatusById = await applicationStatusByIdResponse.ReadAsJsonAsync<SubmittedApplicationStatus>();
    applicationStatusById!.Id.ShouldBe(application.Id);
    applicationStatusById.ApplicationType.ShouldBe(application.ApplicationType);
    applicationStatusById.TranscriptsStatus.ShouldNotBeEmpty();
    applicationStatusById.CharacterReferencesStatus.ShouldNotBeEmpty();
    applicationStatusById.WorkExperienceReferencesStatus.ShouldNotBeEmpty();
    applicationStatusById.ProfessionalDevelopmentsStatus.ShouldNotBeEmpty();
    applicationStatusById.WorkExperienceReferencesStatus.All(we => we.Type == WorkExperienceTypes.Is400Hours).ShouldBeTrue();
  }

  [Fact]
  public async Task SaveDraftApplication_ExistingDraft_Saved()
  {
    var application = CreateDraftApplication();

    var existingAppResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });
    var existingApplicationId = (await existingAppResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;
    existingApplicationId.ShouldBe(application.Id);
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
        .RuleFor(f => f.CertificationTypes, f => [CertificationType.FiveYears])
        .RuleFor(f => f.FiveYearRenewalExplanationChoice, f => FiveYearRenewalExplanations.Ileftthechildcarefieldforpersonalreasons)
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .RuleFor(f => f.Transcripts, f => f.Make(f.Random.Number(2, 5), () => CreateTranscript()))
        .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference()))
        .RuleFor(f => f.WorkExperienceReferences, f => f.Make(f.Random.Number(2, 5), () => Create400HoursTypeWorkExperienceReference()))
        .RuleFor(f => f.ProfessionalDevelopments, f => f.Make(f.Random.Number(1, 3), () => CreateProfessionalDevelopment()))
        .Generate();

    application.Id = this.Fixture.draftTestApplicationId4;
    application.ApplicationType = ApplicationTypes.Renewal;
    return application;
  }

  private DraftApplication CreateDraftIcraApplication()
  {
    var application = new Faker<DraftApplication>("en_CA")
        .RuleFor(f => f.ApplicationType, f => ApplicationTypes.ICRA)
        .RuleFor(f => f.CertificationTypes, f => [CertificationType.FiveYears])
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .RuleFor(f => f.Transcripts, f => f.Make(1, () => CreateTranscript()))
        .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference()))
        .Generate();

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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, Fixture.AuthenticatedBcscUser);
      _.Post.Json(submissionRequest).ToUrl("/api/applications");
      _.StatusCodeShouldBe(400);
    });
  }

  [Fact]
  public async Task SubmitApplication_TwiceBySameUser_ReturnsBadRequest()
  {
    // Assert user already has submitted application
    var applicationsResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url("/api/applications");
      _.StatusCodeShouldBeOk();
    });
    var applications = await applicationsResponse.ReadAsJsonAsync<Application[]>();
    applications.Where(application => application.Status == ApplicationStatus.Submitted).ShouldNotBeEmpty();

    // Create and submit application
    var firstApplication = CreateDraftApplication();
    firstApplication.Id = Fixture.draftTestApplicationId4;
    firstApplication.CertificationTypes = new[] { CertificationType.EceAssistant };

    // Save draft
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(firstApplication)).ToUrl($"/api/draftapplications/{firstApplication.Id}");
      _.StatusCodeShouldBeOk();
    });

    // Submit application
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, Fixture.AuthenticatedBcscUser);
      _.Post.Json(new ApplicationSubmissionRequest(firstApplication.Id!)).ToUrl("/api/applications");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task CancelApplication_ById_ShouldReturnId_QueryApplications_ShouldNotReturnCancelledApplications()
  {
    var application = CreateDraftApplication();
    application.Id = this.Fixture.draftTestApplicationId2;

    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity2, this.Fixture.AuthenticatedBcscUser2);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity2, this.Fixture.AuthenticatedBcscUser2);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Json(newCharacter).ToUrl($"/api/applications/{applicationId}/characterreference/{referenceId}");
      _.StatusCodeShouldBeOk();
    });
    var UpdateCharacterRefResponseId = await UpdateCharacterRefResponse.ReadAsJsonAsync<UpdateReferenceResponse>();
    UpdateCharacterRefResponseId!.ReferenceId.ShouldNotBeEmpty();
  }

  [Fact]
  [Category("Internal")]
  public async Task AddNewCharacterReference_ForSubmittedApplication()
  {
    var applicationId = this.Fixture.submittedTestApplicationId4;
    CharacterReference newCharacter = CreateCharacterReference();
    var UpdateCharacterRefResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Url($"/api/applications/{applicationId}/workExperienceReference/{referenceId}/resendInvite");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }

  [Fact]
  [Category("Internal")]
  public async Task AddProfessionalDevelopmentAndFiles_ToSubmittedApplication()
  {
    // Create Renewal Draft Application
    var application = Create400HoursTypeRenewalDraftApplication();

    // Save Renewal Draft Application
    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{application.Id}");
      _.StatusCodeShouldBeOk();
    });

    var draftApplicationId = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application.Id;

    // Submit Renewal Application
    var applicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Json(new ApplicationSubmissionRequest(draftApplicationId)).ToUrl($"/api/applications");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await applicationResponse.ReadAsJsonAsync<SubmitApplicationResponse>()).ShouldNotBeNull().Application.Id;

    var submittedApplicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/applications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });

    var submittedApplicationsById = await submittedApplicationByIdResponse.ReadAsJsonAsync<Application[]>();

    submittedApplicationsById.ShouldHaveSingleItem();
    var submittedApplicationFromServer = submittedApplicationsById[0];
    submittedApplicationFromServer.ProfessionalDevelopments.ShouldNotBeEmpty();
    var existingProfessionalDevIds = submittedApplicationFromServer.ProfessionalDevelopments
                     .Select(pd => pd.Id)
                     .ToList();

    // Add test File
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
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.WithRequestHeader("file-classification", testClassification);
      _.WithRequestHeader("file-tag", testTags);
      _.WithRequestHeader("file-folder", testFolder);
      _.Post.MultipartFormData(formData).ToUrl($"/api/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    var uploadedFileResponse = (await fileResponse.ReadAsJsonAsync<FileResponse>()).ShouldNotBeNull();

    // Create Professional development
    var professionalDevelopment = CreateProfessionalDevelopment();
    professionalDevelopment.NewFiles = [uploadedFileResponse.fileId];

    // Add professional development to submitted Renewal Application
    var response = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Post.Json(professionalDevelopment).ToUrl($"/api/applications/{applicationId}/professionaldevelopment/add");
      _.StatusCodeShouldBeOk();
    });

    var addedProfessionalDevelopment = (await response.ReadAsJsonAsync<AddProfessionalDevelopmentResponse>()).ShouldNotBeNull();
    addedProfessionalDevelopment.ApplicationId.ShouldNotBeEmpty();
    addedProfessionalDevelopment.ApplicationId.ShouldBe(applicationId);

    var applicationByIdResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUser);
      _.Get.Url($"/api/applications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });

    var applicationsById = await applicationByIdResponse.ReadAsJsonAsync<Application[]>();
    applicationsById.ShouldHaveSingleItem();
    var applicationFromServer = applicationsById[0];
    applicationFromServer.ProfessionalDevelopments.ShouldNotBeEmpty();
    var newProfessionalDev = applicationFromServer.ProfessionalDevelopments.FirstOrDefault(pd => !existingProfessionalDevIds.Contains(pd.Id));
    newProfessionalDev.ShouldNotBeNull();
    newProfessionalDev.Files.ShouldHaveSingleItem();
    newProfessionalDev.Files.First().Id!.ShouldContain(uploadedFileResponse.fileId);
    newProfessionalDev.Status.ShouldBe(ProfessionalDevelopmentStatusCode.Submitted); //for an already submitted application, the professional development should be submitted
  }

  [Fact]
  public async Task SubmitIcraApplication_WithExistingIcraEligibilityApproval_ShouldReturnStatusOk()
  {
    //create icraEligibility draft
    var eligibility = new Clients.RegistryPortal.Server.ICRA.ICRAEligibility
    {
      ApplicantId = this.Fixture.AuthenticatedBcscUser3.Id.ToString(),
      Status = Clients.RegistryPortal.Server.ICRA.ICRAStatus.Draft,
      EmploymentReferences = new[]
    {
       new Clients.RegistryPortal.Server.ICRA.EmploymentReference { FirstName = "John", LastName = "Doe", EmailAddress = "john.doe@example.com" }
     },
      InternationalCertifications = new List<Clients.RegistryPortal.Server.ICRA.InternationalCertification>
     {
       new Clients.RegistryPortal.Server.ICRA.InternationalCertification
       {
           CertificateStatus = Clients.RegistryPortal.Server.ICRA.CertificateStatus.Valid,
           CertificateTitle = faker.Company.CatchPhrase(),
           IssueDate = faker.Date.Past(),
           ExpiryDate = faker.Date.Soon(),
           CountryId = this.Fixture.Country.ecer_CountryId!.Value.ToString(),
       }
     }
    };

    var saveResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity3, this.Fixture.AuthenticatedBcscUser3);
      _.Put.Json(new SaveDraftICRAEligibilityRequest(eligibility)).ToUrl($"/api/icra/");
      _.StatusCodeShouldBeOk();
    });

    var saved = (await saveResponse.ReadAsJsonAsync<DraftICRAEligibilityResponse>()).ShouldNotBeNull().Eligibility;

    await icraRepository.SetIcraEligibilityForUnitTests(saved.Id!, true, CancellationToken.None);

    //create draft application to submit
    var draftApplication = CreateDraftIcraApplication();

    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity3, this.Fixture.AuthenticatedBcscUser3);
      _.Put.Json(new SaveDraftApplicationRequest(draftApplication)).ToUrl($"/api/draftapplications/{draftApplication.Id}");
      _.StatusCodeShouldBeOk();
    });

    var savedDraftApplication = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application;

    // Submit Renewal Application
    var applicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity3, this.Fixture.AuthenticatedBcscUser3);
      _.Post.Json(new ApplicationSubmissionRequest(savedDraftApplication.Id!)).ToUrl($"/api/applications");
      _.StatusCodeShouldBeOk();
    });

    var submittedApplication = (await applicationResponse.ReadAsJsonAsync<SubmitApplicationResponse>()).ShouldNotBeNull().Application;

    //test cleanup
    await icraRepository.SetIcraEligibilityForUnitTests(saved.Id!, false, CancellationToken.None);
    await applicationRepository.CancelApplicationForUnitTest(submittedApplication.Id!, CancellationToken.None);
  }

  [Fact]
  public async Task SubmitIcraApplication_WithOutExistingIcraEligibilityApproval_ShouldBeBadRequest()
  {
    //create draft application to submit
    var draftApplication = CreateDraftIcraApplication();

    var newDraftApplicationResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity3, this.Fixture.AuthenticatedBcscUser3);
      _.Put.Json(new SaveDraftApplicationRequest(draftApplication)).ToUrl($"/api/draftapplications/{draftApplication.Id}");
      _.StatusCodeShouldBeOk();
    });

    var savedDraftApplication = (await newDraftApplicationResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().Application;

    // Submit Renewal Application
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity3, this.Fixture.AuthenticatedBcscUser3);
      _.Post.Json(new ApplicationSubmissionRequest(savedDraftApplication.Id!)).ToUrl($"/api/applications");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    //test cleanup
    await applicationRepository.CancelApplicationForUnitTest(savedDraftApplication.Id!, CancellationToken.None);
  }

  private static Transcript CreateTranscript()
  {
    // Use Faker to generate values for the required parameters
    var faker = new Faker("en_CA");
    var educationalInstitutionName = faker.Company.CompanyName();
    var programName = $"{faker.Hacker.Adjective()} Program";
    var studentLastName = faker.Name.LastName();
    var startDate = faker.Date.Past();
    var endDate = faker.Date.Past();
    var isNameUnverified = faker.Random.Bool();
    var educationRecognition = new EducationRecognition(); // Initialize as needed
    var educationOrigin = new EducationOrigin(); // Initialize as needed

    // Instantiate the Transcript record with the required arguments
    var transcript = new Transcript(
        educationalInstitutionName,
        programName,
        studentLastName,
        startDate,
        endDate,
        isNameUnverified,
        educationRecognition,
        educationOrigin
    )
    {
      // Populate optional properties
      Id = null,
      CampusLocation = faker.Address.City(),
      StudentFirstName = faker.Name.FirstName(),
      StudentNumber = faker.Random.Number(10000000, 99999999).ToString(),
      IsECEAssistant = faker.Random.Bool(),
      TranscriptStatusOption = TranscriptStatusOptions.OfficialTranscriptRequested,
    };

    return transcript;
  }

  private CharacterReference CreateCharacterReference()
  {
    return new CharacterReference(
      $"{Fixture.TestRunId}{faker.Name.LastName()}", faker.Phone.PhoneNumber(), "Character_Reference@test.gov.bc.ca"
    )
    { FirstName = faker.Name.FirstName() };
  }

  private ProfessionalDevelopment CreateProfessionalDevelopment()
  {
    return new ProfessionalDevelopment(
        CourseName: faker.Company.CatchPhrase(),
        OrganizationName: faker.Company.CompanyName(),
        StartDate: faker.Date.Past(),
        EndDate: faker.Date.Recent(),
        NumberOfHours: faker.Random.Int(1, 100))
    {
      Id = null,
      OrganizationContactInformation = faker.Phone.PhoneNumber(),
      InstructorName = faker.Name.FullName(),
      Status = faker.PickRandom<ProfessionalDevelopmentStatusCode>()
    };
  }

  private WorkExperienceReference CreateWorkExperienceReference()
  {
    return new WorkExperienceReference(
       faker.Name.LastName(), "Work_Experience_Reference@test.gov.bc.ca", faker.Random.Number(10, 150)
    )
    {
      FirstName = faker.Name.FirstName(),
      PhoneNumber = faker.Phone.PhoneNumber()
    };
  }

  private WorkExperienceReference Create400HoursTypeWorkExperienceReference()
  {
    return new WorkExperienceReference(
        faker.Name.LastName(), "Work_Experience_Reference@test.gov.bc.ca", 400
    )
    {
      FirstName = faker.Name.FirstName(),
      PhoneNumber = faker.Phone.PhoneNumber(),
      Type = WorkExperienceTypes.Is400Hours
    };
  }
}
