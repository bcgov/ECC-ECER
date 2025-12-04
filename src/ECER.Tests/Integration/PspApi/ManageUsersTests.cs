using Alba;
using Bogus;
using ECER.Clients.PSPPortal.Server.Users;
using ECER.Resources.Accounts.PspReps;
using ECER.Resources.E2ETests.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using PspUserProfile = ECER.Clients.PSPPortal.Server.Users.PspUserProfile;
using RepoPortalAccessStatus = ECER.Resources.Accounts.PspReps.PortalAccessStatus;
using RepoPspUserRole = ECER.Resources.Accounts.PspReps.PspUserRole;

namespace ECER.Tests.Integration.PspApi;

public class ManageUsersTests : PspPortalWebAppScenarioBase
{
  
  private readonly IUnitTestRepository unitTestRepository;

  public ManageUsersTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
    unitTestRepository = Fixture.Services.GetRequiredService<IUnitTestRepository>();

  }
  private PspUserProfile CreatePspProfile()
  {
    var profile = new Faker<PspUserProfile>("en_CA")
      .RuleFor(f => f.FirstName, f => f.Name.FirstName())
      .RuleFor(f => f.LastName, f => f.Name.LastName())
      .RuleFor(f => f.Email, f => $"test_{f.Internet.Email()}")
      .RuleFor(f => f.JobTitle, f => f.Commerce.Department())
      .Generate();
    
    return profile;
  }

  [Fact]
  public async Task GetManageUsers_ReturnsRepsForInstitution()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url("/api/users/manage");
      _.StatusCodeShouldBeOk();
    });

    var reps = await response.ReadAsJsonAsync<IEnumerable<PspUserListItem>>();
    reps.ShouldNotBeNull();
    reps.ShouldContain(r => r.Id == Fixture.AuthenticatedPspUserId);
    reps.ShouldContain(r => r.Id == Fixture.SecondaryPspUserId);
    reps.ShouldAllBe(r => r.PostSecondaryInstituteId == Fixture.PostSecondaryInstituteId);
  }

  [Fact]
  public async Task DeactivateUser_InSameInstitution_SetsAccessToDisabled()
  {
    var deactivateResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Url($"/api/users/manage/{Fixture.SecondaryPspUserId}/deactivate");
      _.StatusCodeShouldBeOk();
    });

    var repo = Fixture.Services.GetRequiredService<IPspRepRepository>();
    var rep = (await repo.Query(new PspRepQuery { ById = Fixture.SecondaryPspUserId }, CancellationToken.None)).Single();
    rep.AccessToPortal.ShouldBe(RepoPortalAccessStatus.Disabled);
  }

  [Fact]
  public async Task DeactivateUser_DifferentInstitution_ReturnsNotFound()
  {
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Url($"/api/users/manage/{Fixture.OtherInstitutePspUserId}/deactivate");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
    });
  }

  [Fact]
  public async Task SetPrimaryUser_CannotSetDisabledAsPrimary_ReturnsBadRequest()
  {
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Url($"/api/users/manage/{Fixture.AuthenticatedPspUserId}/set-primary");
      _.StatusCodeShouldBeOk();
    }); 
    
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Url($"/api/users/manage/{Fixture.SecondaryPspUserId}/set-primary");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
    });

    var repo = Fixture.Services.GetRequiredService<IPspRepRepository>();
    var current = (await repo.Query(new PspRepQuery { ById = Fixture.AuthenticatedPspUserId }, CancellationToken.None)).Single();
    var target = (await repo.Query(new PspRepQuery { ById = Fixture.SecondaryPspUserId }, CancellationToken.None)).Single();

    target.Profile.Role.ShouldBe(RepoPspUserRole.Secondary);
    current.Profile.Role.ShouldBe(RepoPspUserRole.Primary);
  }
  
  [Fact]
  public async Task SetPrimaryUser_SwitchesPrimaryRepresentative()
  {
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Url($"/api/users/manage/{Fixture.TertiaryPspUserId}/set-primary");
      _.StatusCodeShouldBeOk();
    });

    var repo = Fixture.Services.GetRequiredService<IPspRepRepository>();
    var current = (await repo.Query(new PspRepQuery { ById = Fixture.AuthenticatedPspUserId }, CancellationToken.None)).Single();
    var target = (await repo.Query(new PspRepQuery { ById = Fixture.TertiaryPspUserId }, CancellationToken.None)).Single();

    target.Profile.Role.ShouldBe(RepoPspUserRole.Primary);
    current.Profile.Role.ShouldBe(RepoPspUserRole.Secondary);
  }
  
  [Fact]
  public async Task DeactivateUser_CannotDeactivateSelf_ReturnsBadRequest()
  {
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Url($"/api/users/manage/{Fixture.AuthenticatedPspUserId}/deactivate");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
    });

    var repo = Fixture.Services.GetRequiredService<IPspRepRepository>();
    var self = (await repo.Query(new PspRepQuery { ById = Fixture.AuthenticatedPspUserId }, CancellationToken.None)).Single();
    self.AccessToPortal.ShouldBe(RepoPortalAccessStatus.Active);
  }
  
  [Fact]
  public async Task InvitePspRep_SuccessfullyInvitesNewUser()
  {
    var profile = CreatePspProfile();

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Json(profile).ToUrl("/api/users/manage/add");
      _.StatusCodeShouldBeOk();
    });

    var result = await response.ReadAsJsonAsync<NewPspUserResponse>();
    result.Id.ShouldNotBeNullOrWhiteSpace();
    
    var repo = Fixture.Services.GetRequiredService<IPspRepRepository>();
    var newUser = (await repo.Query(new PspRepQuery { ById = result.Id }, CancellationToken.None)).Single();
    newUser.AccessToPortal.ShouldBe(RepoPortalAccessStatus.Invited);
    newUser.Profile.Role.ShouldBe(RepoPspUserRole.Secondary);
    
    await unitTestRepository.DeletePspRep(result.Id, CancellationToken.None);
  }
}
