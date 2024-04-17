using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Managers.Admin.Contract.PortalInvitations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using Xunit.Abstractions;
using InviteType = ECER.Managers.Admin.Contract.PortalInvitations.InviteType;

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
    applicationById.CertificationTypes.ShouldBeEquivalentTo(application.CertificationTypes);
    applicationById.Transcripts.ShouldNotBeEmpty();
    applicationById.CharacterReferences.ShouldNotBeEmpty();
    applicationById.WorkExperienceReferences.ShouldNotBeEmpty();
    applicationById.Stage.ShouldBe(PortalStage.CertificationType);
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

  [Fact]
  public async Task SaveDraftApplication_WithInvalidCharacterReference_ReturnsBadRequest()
  {
    var invalidDraftApplication = CreateDraftApplication();
    invalidDraftApplication.CharacterReferences = new List<CharacterReference> { CreateInvalidCharacterReference() };

    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(invalidDraftApplication)).ToUrl($"/api/draftapplications/{invalidDraftApplication.Id}");
      _.StatusCodeShouldBe(400);
    });
  }

  private ReferenceSubmissionRequest CreateReferenceSubmissionRequest(string token)
  {
    var faker = new Faker("en_CA");

    // Generating random data for ReferenceContactInformation
    var referenceContactInfo = new ReferenceContactInformation(
        faker.Person.LastName,
        faker.Person.FirstName,
        faker.Person.Email,
        faker.Phone.PhoneNumber(),
        faker.Random.AlphaNumeric(8), // Random certificate number
        faker.Address.StateAbbr() // Random Canadian province abbreviation
    );

    // Generating random data for ReferenceEvaluation
    var referenceEvaluation = new ReferenceEvaluation(
        ReferenceRelationships.CoWorker, // Relationship
        faker.Random.Word(), // LengthOfAcquaintance
        faker.Random.Bool(), // WorkedWithChildren
        faker.Lorem.Paragraph(), // ChildInteractionObservations
        faker.Lorem.Paragraph(), // ApplicantTemperamentAssessment
        faker.Random.Bool(), // Confirmed
        faker.Lorem.Paragraph()
    );

    // Creating the ReferenceSubmissionRequest record
    var referenceSubmissionRequest = new ReferenceSubmissionRequest(
        token,
        referenceContactInfo,
        referenceEvaluation,
        faker.Random.Bool() // ResponseAccuracyConfirmation
    );

    return referenceSubmissionRequest;
  }

  private DraftApplication CreateDraftApplication()
  {
    var application = new Faker<DraftApplication>("en_CA")
        .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .RuleFor(f => f.Transcripts, f => f.Make(f.Random.Number(2, 5), () => CreateTranscript()))
        .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference()))
        .RuleFor(f => f.WorkExperienceReferences, f => f.Make(f.Random.Number(2, 5), () => CreateWorkExperienceReference()))
        .Generate();

    application.Id = this.Fixture.applicationId;
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
    var submissionRequest = new ApplicationSubmissionRequest(this.Fixture.applicationId);

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
    application.Id = this.Fixture.applicationId2;

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
  public async Task SubmitWorkExperienceReference_ShouldReturnOk()
  {
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var portalInvitation = Fixture.portalInvitationId;
    var packingResponse = await bus.Send(new GenerateInviteLinkCommand(portalInvitation, InviteType.WorkExperienceReference, 7), CancellationToken.None);
    packingResponse.ShouldNotBeNull();

    var token = packingResponse.VerificationLink.Split('/')[2];
    var referenceSubmissionRequest = CreateReferenceSubmissionRequest(token);
    await Host.Scenario(_ =>
    {
      _.Post.Json(referenceSubmissionRequest).ToUrl($"/api/References");
      _.StatusCodeShouldBeOk();
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
      faker.Name.FirstName(), faker.Name.LastName(), faker.Phone.PhoneNumber(), faker.Internet.Email()
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
      EndDate = faker.Date.Soon()
    };

    return invalidTranscript;
  }

  private CharacterReference CreateInvalidCharacterReference()
  {
    var faker = new Faker("en_CA");
    var invalidCharacterReference = new CharacterReference(
      FirstName: faker.Name.FirstName(),
      LastName: faker.Name.LastName(),
      PhoneNumber: faker.Phone.PhoneNumber(),
      EmailAddress: null);

    return invalidCharacterReference;
  }
}
