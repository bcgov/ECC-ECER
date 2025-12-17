using Alba;
using ECER.Clients.PSPPortal.Server.Programs;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class ProgramTests : PspPortalWebAppScenarioBase
{
  public ProgramTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task SaveDraftProgram_CreatesDraftProgram()
  {
    var stage = "InitialStage";
    var name = $"Draft Program {Guid.NewGuid():N}";

    var draftResponse = await CreateDraftProgram(stage, name);

    draftResponse.Program.Id.ShouldNotBeNull();
    draftResponse.Program.PortalStage.ShouldBe(stage);
    draftResponse.Program.Status.ShouldBe(ProgramStatus.Draft);

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url($"/api/programs/{draftResponse.Program.Id}");
      _.StatusCodeShouldBeOk();
    });

    var programs = await getResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    var fetched = programs!.Single();
    fetched.Id.ShouldBe(draftResponse.Program.Id);
    fetched.PortalStage.ShouldBe(stage);
    fetched.Name.ShouldNotBeNullOrWhiteSpace();
    fetched.Status.ShouldBe(ProgramStatus.Draft);
  }

  [Fact]
  public async Task SaveDraftProgram_UpdatesExistingDraft()
  {
    var initial = await CreateDraftProgram("InitialStage", "Initial Name");
    var updatedStage = "UpdatedStage";

    var updateResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(new SaveDraftProgramRequest(new Program
      {
        Id = initial.Program.Id,
        PortalStage = updatedStage
      })).ToUrl($"/api/draftprograms/{initial.Program.Id}");
      _.StatusCodeShouldBeOk();
    });

    var updated = await updateResponse.ReadAsJsonAsync<DraftProgramResponse>();
    updated.ShouldNotBeNull();
    updated.Program.Id.ShouldBe(initial.Program.Id);
    updated.Program.PortalStage.ShouldBe(updatedStage);
    updated.Program.Name.ShouldNotBeNullOrWhiteSpace();

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url($"/api/programs/{initial.Program.Id}");
      _.StatusCodeShouldBeOk();
    });

    var programs = await getResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    var fetched = programs!.Single();
    fetched.PortalStage.ShouldBe(updatedStage);
    fetched.Name.ShouldNotBeNullOrWhiteSpace();
  }

  [Fact]
  public async Task SaveDraftProgram_ReturnsNotFound_WhenProgramDoesNotExistForUpdate()
  {
    var randomId = Guid.NewGuid().ToString();

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(new SaveDraftProgramRequest(new Program
      {
        Id = randomId,
        PortalStage = "AnyStage"
      })).ToUrl($"/api/draftprograms/{randomId}");
      _.StatusCodeShouldBe(404);
    });
  }

  private async Task<DraftProgramResponse> CreateDraftProgram(string stage, string name)
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(new SaveDraftProgramRequest(new Program
      {
        PortalStage = stage,
        Name = name
      })).ToUrl("/api/draftprograms");
      _.StatusCodeShouldBeOk();
    });

    var draft = await response.ReadAsJsonAsync<DraftProgramResponse>();
    draft.ShouldNotBeNull();
    return draft!;
  }
}
