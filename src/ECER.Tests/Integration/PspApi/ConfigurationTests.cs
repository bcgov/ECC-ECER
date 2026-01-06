using Alba;
using ECER.Clients.PSPPortal.Server;
using ECER.Clients.PSPPortal.Server.Programs;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class ConfigurationTests : PspPortalWebAppScenarioBase
{
  public ConfigurationTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task GetAreaOfInstructions_ReturnsAreaOfInstructions()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url("/api/areaofinstructionlist");
      _.StatusCodeShouldBeOk();
    });

    var payload = await response.ReadAsJsonAsync<AreaOfInstructionListResponse>();
    payload.ShouldNotBeNull();
    payload.AreaOfInstruction.ShouldNotBeNull();
    payload.AreaOfInstruction.ShouldContain(item => item.Id == Fixture.AreaOfInstructionId);

    var instruction = payload.AreaOfInstruction.Single(item => item.Id == Fixture.AreaOfInstructionId);
    instruction.Name.ShouldBe(Fixture.AreaOfInstructionName);
    instruction.MinimumHours.ShouldBe(Fixture.AreaOfInstructionMinimumHours);
    instruction.ProgramTypes.ShouldNotBeNull();
    instruction.ProgramTypes.ShouldContain(ProgramTypes.ITE);
    instruction.ProgramTypes.ShouldContain(ProgramTypes.SNE);
  }
}
