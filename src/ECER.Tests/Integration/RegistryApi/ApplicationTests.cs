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
    var invalidApplication = CreateInvalidDraftApplication();
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
        .Generate();
  }

  private DraftApplication CreateInvalidDraftApplication()
  {
    var faker = new Faker("en_CA");
    var invalidTranscript = new Transcript
    {
      EducationalInstitutionName = null,
      ProgramName = faker.Lorem.Sentence(),
      StudentName = faker.Person.FullName,
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
}
