using System.Net;
using Alba;
using Bogus;
using ECER.Clients.PSPPortal.Server;
using ECER.Clients.PSPPortal.Server.Courses;
using ECER.Clients.PSPPortal.Server.Programs;
using Shouldly;
using Xunit.Abstractions;
using Course = ECER.Clients.PSPPortal.Server.Shared.Course;
using CourseAreaOfInstruction = ECER.Clients.PSPPortal.Server.Shared.CourseAreaOfInstruction;

namespace ECER.Tests.Integration.PspApi;

public class CoursesTest : PspPortalWebAppScenarioBase
{
  public CoursesTest(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
  
  [Fact]
  public async Task AddCourse_WhenTypeProgramProfile_ReturnsStatusBadRequest()
  {
    var course =  CreateCourse(101, "201" );
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(new AddCourseRequest(course, FunctionType.ProgramProfile, Fixture.programApplicationId)).ToUrl($"/api/courses");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }
  
  [Fact]
  public async Task AddCourse_WhenCourseNumberAlreadyExists_ReturnsStatusInvalidOperation()
  {
    var course =  CreateCourse(101, "201" );
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(new AddCourseRequest(course, FunctionType.ProgramApplication, Fixture.programApplicationId)).ToUrl($"/api/courses");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }
  
  [Fact]
  public async Task DeleteCourse_CourseDoesNotExist_ReturnsStatusInvalidOperation()
  {
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Delete.Url($"/api/courses/{Guid.NewGuid()}");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }
  
  [Fact]
  public async Task AddCourseAndDeleteCourse_WhenNewCourse_ReturnsOK()
  {
    var course =  CreateCourseWithCourseAreaOfInstructions();
    course.CourseTitle = "Test_psp_add_course";
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(new AddCourseRequest(course, FunctionType.ProgramApplication, Fixture.programApplicationId)).ToUrl($"/api/courses");
      _.StatusCodeShouldBeOk();
    });
    
    var newCourseId = await response.ReadAsJsonAsync<string>();
    newCourseId.ShouldNotBeNull();
    
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Delete.Url($"/api/courses/{newCourseId}");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task UpdateCourses_WhenCourseNumberAlreadyExists_ReturnsStatusInvalidOperation()
  {
    var course = CreateCourse(101, "109" );
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(course, FunctionType.ProgramProfile, Fixture.programId))
        .ToUrl($"/api/courses/{course.CourseId}");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }

  [Fact]
  public async Task UpdateCourses_ReturnsStatusOk()
  {
    var course = CreateCourse(101, "");
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(course, FunctionType.ProgramProfile, Fixture.programId)).ToUrl($"/api/courses/{course.CourseId}");
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
    var status = await programResponse.ReadAsJsonAsync<GetProgramsResponse>();
    status.ShouldNotBeNull();

    var firstProfile = status.Programs!.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
    var matchingCourse = firstProfile.Courses.Where(c => c.CourseNumber == "101").ToList();
    matchingCourse.First().CourseNumber.ShouldBe("101");
    matchingCourse.First().CourseTitle.ShouldBe("Course 101");
    matchingCourse.First().NewCourseNumber.ShouldBe(course.NewCourseNumber);
    matchingCourse.First().NewCourseTitle.ShouldBe("Course 102");
  }
  
  [Fact]
  public async Task UpdateCourses_With_AreaOfInstructions_ReturnsStatusOk()
  {
    var course = CreateCourseWithCourseAreaOfInstructions();
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(course, FunctionType.ProgramProfile, Fixture.programId)).ToUrl($"/api/courses/{course.CourseId}");
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
    var status = await programResponse.ReadAsJsonAsync<GetProgramsResponse>();
    status.ShouldNotBeNull();

    var firstProfile = status.Programs!.FirstOrDefault().ShouldNotBeNull();
    firstProfile.Courses.ShouldNotBeNull();
    firstProfile.Courses.Count().ShouldNotBe(0);

    var matchingCourse = firstProfile.Courses.Where(c => c.CourseNumber == "101").ToList();
    matchingCourse.ShouldNotBeNull();
    matchingCourse.First().CourseNumber.ShouldBe("101");
    matchingCourse.First().CourseTitle.ShouldBe("Course 101");
    
    matchingCourse.First().CourseAreaOfInstruction.ShouldNotBeNull();
    matchingCourse.First().CourseAreaOfInstruction.ShouldNotBeEmpty();
    var firstCourseAreaOfInstruction = matchingCourse.First().CourseAreaOfInstruction!.SingleOrDefault();
    firstCourseAreaOfInstruction.ShouldNotBeNull();
  }
  
  private Course CreateCourse(int courseNumber, string newNumber)
  {
    return new Course
    {
      CourseId = Fixture.courseId,
      CourseNumber = courseNumber.ToString(),
      CourseTitle = "Course 101",
      NewCourseNumber = !string.IsNullOrWhiteSpace(newNumber) ? newNumber : (courseNumber + 10).ToString(),
      NewCourseTitle = "Course 102",
      ProgramType = ProgramTypes.SNE.ToString()
    };
  }
  
  private Course CreateCourseWithCourseAreaOfInstructions()
  {
    var faker = new Faker("en_CA");
    return new Course
    {
      CourseId = Fixture.courseId,
      CourseNumber = faker.Random.Number(0, 999).ToString(),
      CourseTitle = "Course 101",
      ProgramType = ProgramTypes.SNE.ToString(),
      CourseAreaOfInstruction = new []
      {
        new CourseAreaOfInstruction()
        {
          AreaOfInstructionId = Fixture.AreaOfInstructionId,
          NewHours = "20.00"
        }
      }
    };
  }

}
