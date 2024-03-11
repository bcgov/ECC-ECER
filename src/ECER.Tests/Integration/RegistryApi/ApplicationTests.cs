using Alba;
using Bogus;
using ECER.Clients.RegistryPortal.Server.Applications;
using Shouldly;
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
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl("/api/draftapplications");
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
    applicationById.Stage.ShouldBe(PortalStage.CertificationType);
  }

  [Fact]
  public async Task SaveDraftApplication_NewDraft_Saved()
  {
    var application = CreateDraftApplication();
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl("/api/draftapplications");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task SaveDraftApplication_ExistingDraft_Saved()
  {
    var application = CreateDraftApplication();
    var newAppResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl("/api/draftapplications");
      _.StatusCodeShouldBeOk();
    });

    var applicationId = (await newAppResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().ApplicationId;

    application.Id = applicationId;
    var existingAppResponse = await Host.Scenario(_ =>
    {
      _.WithExistingUser(Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(application)).ToUrl($"/api/draftapplications/{applicationId}");
      _.StatusCodeShouldBeOk();
    });
    var existingApplicationId = (await existingAppResponse.ReadAsJsonAsync<DraftApplicationResponse>()).ShouldNotBeNull().ApplicationId;
    existingApplicationId.ShouldBe(applicationId);
  }

  [Fact]
  public async Task SaveDraftApplication_WithInvalidTranscript_ReturnsBadRequest()
  {
    var invalidApplication = CreateDraftApplicationWithInvalidTranscript();
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(invalidApplication)).ToUrl("/api/draftapplications");
      _.StatusCodeShouldBe(400);
    });
  }
  
  [Fact]
  public async Task SaveDraftApplication_WithInvalidCharacterReference_ReturnsBadRequest()
  {
    var invalidApplication = CreateDraftApplicationWithInvalidCharacterReference();
    await Host.Scenario(_ =>
    {
      _.WithExistingUser(this.Fixture.AuthenticatedBcscUserIdentity, this.Fixture.AuthenticatedBcscUserId);
      _.Put.Json(new SaveDraftApplicationRequest(invalidApplication)).ToUrl("/api/draftapplications");
      _.StatusCodeShouldBe(400);
    });
  }

  private DraftApplication CreateDraftApplication()
  {
    return new Faker<DraftApplication>("en_CA")
        .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .RuleFor(f => f.Transcripts, f => f.Make(f.Random.Number(2, 5), () => CreateTranscript())) 
        .RuleFor(f => f.CharacterReferences, f => f.Make(1, () => CreateCharacterReference())) 

        .Generate();
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
      faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), faker.Phone.PhoneNumber()
    );
  }
  
  private DraftApplication CreateDraftApplicationWithInvalidTranscript()
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

    return new Faker<DraftApplication>("en_CA")
        .RuleFor(f => f.Transcripts, _ => new List<Transcript> { invalidTranscript })
        .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
        .RuleFor(f => f.SignedDate, f => f.Date.Recent())
        .Generate();
  }
  private DraftApplication CreateDraftApplicationWithInvalidCharacterReference()
  {
    var faker = new Faker("en_CA");
    var invalidCharacterReference = new CharacterReference(
      FirstName: faker.Name.FirstName(),
      LastName: faker.Name.LastName(),
      PhoneNumber: faker.Phone.PhoneNumber(),
      EmailAddress: null);

    return new Faker<DraftApplication>("en_CA")
      .RuleFor(f => f.CharacterReferences, _ => new List<CharacterReference> { invalidCharacterReference })
      .RuleFor(f => f.CertificationTypes, f => f.Make(f.Random.Number(2), () => f.PickRandom<CertificationType>()))
      .RuleFor(f => f.SignedDate, f => f.Date.Recent())
      .Generate();
  }
}
