using System.Net;
using Alba;
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
  public async Task UpdateCourses_ReturnsStatusInvalidOperation()
  {
    var course = new[] { CreateCourse(101, "201" )};
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(course, FunctionType.ProgramProfile))
        .ToUrl($"/api/courses/{Fixture.programId}");
      _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
    });
  }

  [Fact]
  public async Task UpdateCourses_ReturnsStatusOk()
  {
    var course = new[] { CreateCourse(101, "") };
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(course, FunctionType.ProgramProfile)).ToUrl($"/api/courses/{Fixture.programId}");
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
    firstProfile.Courses.ElementAt(0).CourseNumber.ShouldBe("101");
    firstProfile.Courses.ElementAt(0).CourseTitle.ShouldBe("Course 101");
    firstProfile.Courses.ElementAt(0).NewCourseNumber.ShouldBe(course[0].NewCourseNumber);
    firstProfile.Courses.ElementAt(0).NewCourseTitle.ShouldBe("Course 102");
  }
  
  [Fact]
  public async Task UpdateCourses_With_AreaOfInstructions_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(new UpdateCourseRequest(new []{ CreateCourseWithCourseAreaOfInstructions() }, FunctionType.ProgramProfile)).ToUrl($"/api/courses/{Fixture.programId}");
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

    var firstCourse = firstProfile.Courses.SingleOrDefault();
    firstCourse.ShouldNotBeNull();
    firstCourse.CourseNumber.ShouldBe("101");
    firstCourse.CourseTitle.ShouldBe("Course 101");
    
    firstCourse.CourseAreaOfInstruction.ShouldNotBeNull();
    
    var firstCourseAreaOfInstruction = firstCourse.CourseAreaOfInstruction.SingleOrDefault();
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

  
  private Course CreateCourseWithCourseAreaOfInstructionsAndHours(List<AreaOfInstruction> areaOfInstructions)
  {
    var courseAreaOfInstruction =  new List<CourseAreaOfInstruction>();

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
  
  private Course CreateCourseWithCourseAreaOfInstructions()
  {
    return new Course
    {
      CourseId = Fixture.courseId,
      CourseNumber = "101",
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
