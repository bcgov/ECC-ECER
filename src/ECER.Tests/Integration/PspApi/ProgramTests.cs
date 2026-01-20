using Alba;
using ECER.Clients.PSPPortal.Server.Programs;
using Shouldly;
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
        ProgramTypes = updatedProgramTypes,
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
    updated.Program.ProgramTypes.ShouldBe(updatedProgramTypes);
    updated.Program.ProgramName.ShouldBe("Program Name");

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
        ProgramTypes = programTypes,
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

    var status = await response.ReadAsJsonAsync<IEnumerable<Program>>();
    status.ShouldNotBeNull();

    var firstProfile = status.FirstOrDefault().ShouldNotBeNull();
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
  
    var status = await response.ReadAsJsonAsync<IEnumerable<Program>>();
    status.ShouldNotBeNull();
  
    var firstProfile = status.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
    firstProfile.Courses.Count().ShouldBe(0);
  }
  
  [Fact]
  public async Task UpdateCourses_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(new []{ CreateCourse() })).ToUrl($"/api/program/{Fixture.programId}/courses");
      _.StatusCodeShouldBeOk();
    });
    
    var updateStatus = await response.ReadAsJsonAsync<string>();
    updateStatus.ShouldNotBeNull();
    updateStatus.ShouldBe(Fixture.programId);

    var programResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.programId}");
      _.StatusCodeShouldBeOk();
    });
    var status = await programResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    status.ShouldNotBeNull();

    var firstProfile = status.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
    firstProfile.Courses.ElementAt(0).CourseNumber.ShouldBe("101");
    firstProfile.Courses.ElementAt(0).CourseTitle.ShouldBe("Course 101");
    firstProfile.Courses.ElementAt(0).NewCourseNumber.ShouldBe("102");
    firstProfile.Courses.ElementAt(0).NewCourseTitle.ShouldBe("Course 102");
  }
  
  [Fact]
  public async Task UpdateCourses_With_AreaOfInstructions_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(new []{ CreateCourseWithCourseAreaOfInstructions() })).ToUrl($"/api/program/{Fixture.programId}/courses");
      _.StatusCodeShouldBeOk();
    });
    
    var updateStatus = await response.ReadAsJsonAsync<string>();
    updateStatus.ShouldNotBeNull();
    updateStatus.ShouldBe(Fixture.programId);

    var programResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programs/{this.Fixture.programId}");
      _.StatusCodeShouldBeOk();
    });
    var status = await programResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    status.ShouldNotBeNull();

    var firstProfile = status.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
    firstProfile.Courses.Count().ShouldNotBe(0);

    var firstCourse = firstProfile.Courses.SingleOrDefault();
    firstCourse.ShouldNotBeNull();
    firstCourse.CourseNumber.ShouldBe("101");
    firstCourse.CourseTitle.ShouldBe("Course 101");
    firstCourse.NewCourseNumber.ShouldBe("102");
    firstCourse.NewCourseTitle.ShouldBe("Course 102");
    
    firstCourse.CourseAreaOfInstruction.ShouldNotBeNull();
    
    var firstCourseAreaOfInstruction = firstCourse.CourseAreaOfInstruction.SingleOrDefault();
    firstCourseAreaOfInstruction.ShouldNotBeNull();
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
    var status = await programResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    var program = status.First();
    
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
    var status = await programResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    var program = status.First();
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
    var updated = await updatedProgramResponse.ReadAsJsonAsync<IEnumerable<Program>>();
    var updatedProgram = updated.First();
    updatedProgram.ShouldNotBeNull();
    updatedProgram.Status.ShouldBe(ProgramStatus.Withdrawn);
  }

  private Course CreateCourse()
  {
    return new Course
    {
      CourseId = Fixture.courseId,
      CourseNumber = "101",
      CourseTitle = "Course 101",
      NewCourseNumber = "102",
      NewCourseTitle = "Course 102",
      ProgramType = ProgramTypes.SNE.ToString()
    };
  }
  
  private Course CreateCourseWithCourseAreaOfInstructions()
  {
    return new Course
    {
      CourseId = Fixture.courseId,
      CourseNumber = "101",
      CourseTitle = "Course 101",
      NewCourseNumber = "102",
      NewCourseTitle = "Course 102",
      ProgramType = ProgramTypes.SNE.ToString(),
      CourseAreaOfInstruction = new []
      {
        new CourseAreaOfInstruction()
        {
          CourseAreaOfInstructionId = Guid.NewGuid().ToString(),
          AreaOfInstructionId = Fixture.AreaOfInstructionId,
          NewHours = "20.00"
        }
      }
    };
  }
}
