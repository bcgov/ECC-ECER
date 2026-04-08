using Alba;
using ECER.Managers.Registry.Contract.PspUsers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class ProfileTests : PspPortalWebAppScenarioBase
{
  public ProfileTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetProfile_WithMatchingBceidBusinessName_HealsInstitutionGuid()
  {
    // User from a broken institution (has business name, no GUID) logs in with correct bceid info
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.HealingTestUserIdentity, Fixture.HealingTestProgramRepId, true);
      _.Get.Url($"/api/users/profile?bceidBusinessId={PspPortalWebAppFixture.HealingTestBceidBusinessId}&bceidBusinessName={PspPortalWebAppFixture.HealingTestBceidBusinessName}");
      _.StatusCodeShouldBeOk();
    });

    // Verify the institution now has the BCeID GUID by querying via MediatR
    var bus = Fixture.Services.GetRequiredService<IMediator>();
    var healResult = await bus.Send(new HealBceidBusinessIdCommand(
      Fixture.HealingTestPostSecondaryInstituteId,
      PspPortalWebAppFixture.HealingTestBceidBusinessId,
      PspPortalWebAppFixture.HealingTestBceidBusinessName), CancellationToken.None);

    // Should be NotNeeded now since it was already healed
    healResult.ShouldBe(HealBceidBusinessIdResult.NotNeeded);
  }

  [Fact]
  public async Task GetProfile_WithNonMatchingBceidBusinessName_DoesNotHealInstitutionGuid()
  {
    // User from a broken institution logs in with wrong business name — GUID should NOT be saved
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.HealingTestUserIdentity, Fixture.HealingTestProgramRepId, true);
      _.Get.Url($"/api/users/profile?bceidBusinessId={PspPortalWebAppFixture.HealingTestBceidBusinessId}&bceidBusinessName=Wrong Business Name");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task GetProfile_WithoutBceidParams_DoesNotAttemptHealing()
  {
    // User logs in without providing bceid params — should work normally without healing
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.HealingTestUserIdentity, Fixture.HealingTestProgramRepId, true);
      _.Get.Url("/api/users/profile");
      _.StatusCodeShouldBeOk();
    });
  }
}
