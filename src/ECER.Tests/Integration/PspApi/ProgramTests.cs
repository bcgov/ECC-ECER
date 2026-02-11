using Alba;
using ECER.Clients.PSPPortal.Server;
using ECER.Clients.PSPPortal.Server.Courses;
using ECER.Clients.PSPPortal.Server.Programs;
using Shouldly;
using System.Net;
using Xunit.Abstractions;
using Course = ECER.Clients.PSPPortal.Server.Shared.Course;
using CourseAreaOfInstruction = ECER.Clients.PSPPortal.Server.Shared.CourseAreaOfInstruction;
using Program = ECER.Clients.PSPPortal.Server.Programs.Program;

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
    var offeredProgramTypes = BasicAndIteProgramTypes;

    var draftResponse = await CreateDraftProgram(stage, name, startDate, endDate, offeredProgramTypes);

    draftResponse.Program.Id.ShouldNotBeNull();
    draftResponse.Program.PortalStage.ShouldBe(stage);
    draftResponse.Program.Status.ShouldBe(ProgramStatus.Draft);
    draftResponse.Program.StartDate.ShouldBe(startDate);
    draftResponse.Program.EndDate.ShouldBe(endDate);
    draftResponse.Program.OfferedProgramTypes.ShouldBe(offeredProgramTypes);

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url($"/api/programs/{draftResponse.Program.Id}");
      _.StatusCodeShouldBeOk();
    });

    var response = await getResponse.ReadAsJsonAsync<GetProgramsResponse>();
    var fetched = response.Programs!.Single();
    fetched.Id.ShouldBe(draftResponse.Program.Id);
    fetched.PortalStage.ShouldBe(stage);
    fetched.Name.ShouldNotBeNullOrWhiteSpace();
    fetched.Status.ShouldBe(ProgramStatus.Draft);
    fetched.PostSecondaryInstituteName.ShouldNotBeNull();
    fetched.StartDate.ShouldBe(startDate);
    fetched.EndDate.ShouldBe(endDate);
    fetched.OfferedProgramTypes.ShouldBe(offeredProgramTypes);
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
        OfferedProgramTypes = updatedProgramTypes,
        ProgramName = "Program Name"
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
    updated.Program.OfferedProgramTypes.ShouldBe(updatedProgramTypes);
    updated.Program.ProgramName.ShouldBe("Program Name");

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url($"/api/programs/{initial.Program.Id}");
      _.StatusCodeShouldBeOk();
    });

    var response = await getResponse.ReadAsJsonAsync<GetProgramsResponse>();
    var fetched = response.Programs!.Single();
    fetched.PortalStage.ShouldBe(updatedStage);
    fetched.Name.ShouldNotBeNullOrWhiteSpace();
    fetched.StartDate.ShouldBe(updatedStartDate);
    fetched.EndDate.ShouldBe(updatedEndDate);
    fetched.OfferedProgramTypes.ShouldBe(updatedProgramTypes);
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

  private async Task<DraftProgramResponse> CreateDraftProgram(string stage, string name, DateTime? startDate = null, DateTime? endDate = null, IEnumerable<ProgramTypes>? offeredProgramTypes = null)
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
        OfferedProgramTypes = offeredProgramTypes,
      })).ToUrl("/api/draftprograms");
      _.StatusCodeShouldBeOk();
    });

    var draft = await response.ReadAsJsonAsync<DraftProgramResponse>();
    draft.ShouldNotBeNull();
    return draft!;
  }

  [Fact]
  public async Task GetProgramProfile_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.programIdWithTotals}");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<GetProgramsResponse>();
    status.ShouldNotBeNull();

    var firstProfile = status.Programs!.FirstOrDefault().ShouldNotBeNull();
    firstProfile.NewBasicTotalHours.ShouldBe("20.75");
    firstProfile.NewSneTotalHours.ShouldBe("10");
    firstProfile.NewIteTotalHours.ShouldBe("15.25");
  }

  [Fact]
  public async Task GetAllProgramProfiles_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/null");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<GetProgramsResponse>();
    status.ShouldNotBeNull();

    var firstProfile = status.Programs!.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
    firstProfile.Courses.Count().ShouldBe(0);
  }

  [Fact]
  public async Task GetProgramProfiles_ByFromProgramId_ReturnsMatchingProgram()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs?fromProgramId={this.Fixture.changeRequestFromProfileId}");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<GetProgramsResponse>();
    status.ShouldNotBeNull();
    status.Programs.ShouldNotBeNull();
    status.Programs.Count().ShouldBe(1);

    var program = status.Programs.First();
    program.Id.ShouldBe(this.Fixture.changeRequestProgramId);
    program.FromProgramProfileId.ShouldBe(this.Fixture.changeRequestFromProfileId);
    program.ProgramProfileType.ShouldBe(ProgramProfileType.ChangeRequest);
  }

  [Fact]
  public async Task UpdateProgram_Type_Draft_ReturnsBadRequest()
  {
    var programResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.programId}");
      _.StatusCodeShouldBeOk();
    });
    var status = await programResponse.ReadAsJsonAsync<GetProgramsResponse>();
    var program = status.Programs!.First();

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(program).ToUrl($"/api/program/{Fixture.programId}");
      _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task UpdateProgram_Type_ChangeRequest_ReturnsOk()
  {
    var programResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.changeRequestProgramId}");
      _.StatusCodeShouldBeOk();
    });
    var status = await programResponse.ReadAsJsonAsync<GetProgramsResponse>();
    var program = status.Programs!.First();
    program.Status = ProgramStatus.Withdrawn;

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(program).ToUrl($"/api/program/{Fixture.changeRequestProgramId}");
      _.StatusCodeShouldBeOk();
    });

    var updatedProgramResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.changeRequestProgramId}");
      _.StatusCodeShouldBeOk();
    });
    var updated = await updatedProgramResponse.ReadAsJsonAsync<GetProgramsResponse>();
    var updatedProgram = updated.Programs!.First();
    updatedProgram.ShouldNotBeNull();
    updatedProgram.Status.ShouldBe(ProgramStatus.Withdrawn);

    // Verify the from profile status was updated back to Approved (RegistryReviewComplete)
    var fromProfileResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.changeRequestFromProfileId}");
      _.StatusCodeShouldBeOk();
    });
    var fromProfileStatus = await fromProfileResponse.ReadAsJsonAsync<GetProgramsResponse>();
    var fromProfile = fromProfileStatus.Programs!.First();
    fromProfile.ShouldNotBeNull();
    fromProfile.Status.ShouldBe(ProgramStatus.Approved);
  }

  [Fact]
  public async Task SubmitDraftProgram_BadGuid_ReturnsBadRequest()
  {
    var request = new SubmitProgramRequest("Bad Guid");
    var postResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Json(request).ToUrl($"/api/programs");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task SubmitDraftProgram__ReturnsOk()
  {
    var areaOfInstructions = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Get.Url("/api/areaofinstructionlist");
      _.StatusCodeShouldBeOk();
    });

    var areaOfInstructionsPayload = await areaOfInstructions.ReadAsJsonAsync<AreaOfInstructionListResponse>();

    var allSneInstructions = areaOfInstructionsPayload.AreaOfInstruction
      .Where(ai => ai.ProgramTypes.Contains(ProgramTypes.SNE))
      .ToList();

    var course = CreateCourseWithCourseAreaOfInstructionsAndHours(allSneInstructions);
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(course, FunctionType.ProgramProfile, Fixture.submitProgramId)).ToUrl($"/api/courses/{course.CourseId}");
      _.StatusCodeShouldBeOk();
    });

    var request = new SubmitProgramRequest(this.Fixture.submitProgramId!);
    var postResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId, true);
      _.Post.Json(request).ToUrl($"/api/programs");
      _.StatusCodeShouldBeOk();
    });
  }

  private Course CreateCourseWithCourseAreaOfInstructionsAndHours(List<AreaOfInstruction> areaOfInstructions)
  {
    var courseAreaOfInstruction = new List<CourseAreaOfInstruction>();

    foreach (var ai in areaOfInstructions)
    {
      var ca = new CourseAreaOfInstruction()
      {
        AreaOfInstructionId = ai.Id,
        NewHours = ai.MinimumHours == 0 ? "200" : Convert.ToString(ai.MinimumHours)
      };
      courseAreaOfInstruction.Add(ca);
    }

    var course = new Course
    {
      CourseId = Fixture.courseId2,
      CourseNumber = "201",
      CourseTitle = "Course 201",
      NewCourseNumber = "202",
      NewCourseTitle = "Course 202",
      ProgramType = ProgramTypes.SNE.ToString(),
    };

    course.CourseAreaOfInstruction = courseAreaOfInstruction;
    return course;
  }
}
