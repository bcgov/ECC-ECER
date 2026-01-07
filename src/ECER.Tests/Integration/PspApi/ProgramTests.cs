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
  private static readonly ProgramTypes[] BasicProgramTypes = { ProgramTypes.Basic };
  private static readonly ProgramTypes[] BasicAndIteProgramTypes = { ProgramTypes.Basic, ProgramTypes.ITE };
  private static readonly ProgramTypes[] SneProgramTypes = { ProgramTypes.SNE };

  public ProgramTests(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  [Fact]
  public async Task SaveDraftProgram_CreatesDraftProgram()
  {
    var stage = "InitialStage";
    var name = $"Draft Program {Guid.NewGuid():N}";
    var startDate = DateTime.UtcNow.Date;
    var endDate = DateTime.UtcNow.Date.AddYears(2);
    var programTypes = BasicAndIteProgramTypes;

    var draftResponse = await CreateDraftProgram(stage, name, startDate, endDate, programTypes);

    draftResponse.Program.Id.ShouldNotBeNull();
    draftResponse.Program.PortalStage.ShouldBe(stage);
    draftResponse.Program.Status.ShouldBe(ProgramStatus.Draft);
    draftResponse.Program.StartDate.ShouldBe(startDate);
    draftResponse.Program.EndDate.ShouldBe(endDate);
    draftResponse.Program.ProgramTypes.ShouldBe(programTypes);

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
    fetched.PostSecondaryInstituteName.ShouldNotBeNull();
    fetched.StartDate.ShouldBe(startDate);
    fetched.EndDate.ShouldBe(endDate);
    fetched.ProgramTypes.ShouldBe(programTypes);
  }

  [Fact]
  public async Task SaveDraftProgram_UpdatesExistingDraft()
  {
    var initial = await CreateDraftProgram("InitialStage", "Initial Name", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddYears(1), BasicProgramTypes);
    var updatedStage = "UpdatedStage";
    var updatedStartDate = DateTime.UtcNow.Date.AddMonths(1);
    var updatedEndDate = DateTime.UtcNow.Date.AddYears(3);
    var updatedProgramTypes = SneProgramTypes;

    var updateResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(new SaveDraftProgramRequest(new Program
      {
        Id = initial.Program.Id,
        PortalStage = updatedStage,
        Name = initial.Program.Name,
        StartDate = updatedStartDate,
        EndDate = updatedEndDate,
        ProgramTypes = updatedProgramTypes
      })).ToUrl($"/api/draftprograms/{initial.Program.Id}");
      _.StatusCodeShouldBeOk();
    });

    var updated = await updateResponse.ReadAsJsonAsync<DraftProgramResponse>();
    updated.ShouldNotBeNull();
    updated.Program.Id.ShouldBe(initial.Program.Id);
    updated.Program.PortalStage.ShouldBe(updatedStage);
    updated.Program.Name.ShouldNotBeNullOrWhiteSpace();
    updated.Program.StartDate.ShouldBe(updatedStartDate);
    updated.Program.EndDate.ShouldBe(updatedEndDate);
    updated.Program.ProgramTypes.ShouldBe(updatedProgramTypes);

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
    fetched.StartDate.ShouldBe(updatedStartDate);
    fetched.EndDate.ShouldBe(updatedEndDate);
    fetched.ProgramTypes.ShouldBe(updatedProgramTypes);
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

  private async Task<DraftProgramResponse> CreateDraftProgram(string stage, string name, DateTime? startDate = null, DateTime? endDate = null, IEnumerable<ProgramTypes>? programTypes = null)
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Put.Json(new SaveDraftProgramRequest(new Program
      {
        PortalStage = stage,
        Name = name,
        StartDate = startDate,
        EndDate = endDate,
        ProgramTypes = programTypes
      })).ToUrl("/api/draftprograms");
      _.StatusCodeShouldBeOk();
    });

    var draft = await response.ReadAsJsonAsync<DraftProgramResponse>();
    draft.ShouldNotBeNull();
    return draft!;
  }
  
  [Fact]
  public async Task GetProgramProfile_ReturnsStatus()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.programId}");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<IEnumerable<Program>>();
    status.ShouldNotBeNull();

    var firstProfile = status.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
  }
}
