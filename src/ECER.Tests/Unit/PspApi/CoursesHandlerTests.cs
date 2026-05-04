using ECER.Managers.Registry;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.ProgramApplications;
using ECER.Resources.Documents.Programs;
using Moq;
using Shouldly;

namespace ECER.Tests.Unit.PspApi;

public class CoursesHandlerTests
{
  private readonly Mock<IProgramApplicationRepository> _programApplicationRepositoryMock;
  private readonly Mock<IProgramRepository> _programProfileRepositoryMock;
  private readonly Mock<ICourseRepository> _courseRepositoryMock;
  private readonly ICoursesMapper _coursesMapper;

  public CoursesHandlerTests()
  {
    _programApplicationRepositoryMock = new Mock<IProgramApplicationRepository>();
    _programProfileRepositoryMock = new Mock<IProgramRepository>();
    _courseRepositoryMock = new Mock<ICourseRepository>();
    _coursesMapper = new CoursesMapper();
  }

  [Fact]
  public async Task Handle_AddCourseCommand_GivenIncorrectProgramApplicationType_ShouldReturnError_GivenCorrectInput_ReturnsCourseId()
  {
    var instituteId = Guid.NewGuid().ToString();
    var incorrectProgramApplicationId = Guid.NewGuid().ToString();
    var correctProgramApplicationId = Guid.NewGuid().ToString();
    var notFoundProgramApplicationId = Guid.NewGuid().ToString();
    var courseId = Guid.NewGuid().ToString();
    var courseAreaOfInstructionId = Guid.NewGuid().ToString();
    var areaOfInstructionId = Guid.NewGuid().ToString();

    var addCourse = new Course
    {
      CourseId = courseId,
      CourseAreaOfInstruction = [new CourseAreaOfInstruction { AreaOfInstructionId = areaOfInstructionId, NewHours = "0", CourseAreaOfInstructionId = courseAreaOfInstructionId }],
    };

    var incorrectCommand = new SaveCourseCommand(addCourse, incorrectProgramApplicationId, instituteId);
    var correctCommand = new SaveCourseCommand(addCourse, correctProgramApplicationId, instituteId);
    var notFoundCommand = new SaveCourseCommand(addCourse, notFoundProgramApplicationId, instituteId);

    var incorrectResourcesProgramApplication = new Resources.Documents.ProgramApplications.ProgramApplication(Guid.NewGuid().ToString(), instituteId)
    {
      ProgramApplicationName = "application incorrect type",
      ProgramApplicationType = Resources.Documents.ProgramApplications.ApplicationType.SatelliteProgram,
      ProgramTypes = new[] { Resources.Documents.ProgramApplications.ProgramCertificationType.Basic },
      DeliveryType = Resources.Documents.ProgramApplications.DeliveryType.Hybrid,
      Status = Resources.Documents.ProgramApplications.ApplicationStatus.Draft
    };

    var correctResourcesProgramApplication = new Resources.Documents.ProgramApplications.ProgramApplication(Guid.NewGuid().ToString(), instituteId)
    {
      ProgramApplicationName = "application correct type",
      ProgramApplicationType = Resources.Documents.ProgramApplications.ApplicationType.NewBasicECEPostBasicProgram,
      ProgramTypes = new[] { Resources.Documents.ProgramApplications.ProgramCertificationType.Basic },
      DeliveryType = Resources.Documents.ProgramApplications.DeliveryType.Hybrid,
      Status = Resources.Documents.ProgramApplications.ApplicationStatus.Draft
    };

    var incorrectResourcesProgramApplicationQueryResults = new Resources.Documents.ProgramApplications.ProgramApplicationQueryResults([incorrectResourcesProgramApplication], 1);

    var correctResourcesProgramApplicationQueryResults = new Resources.Documents.ProgramApplications.ProgramApplicationQueryResults([correctResourcesProgramApplication], 1);

    var emptyResourcesProgramApplicationQueryResults = new Resources.Documents.ProgramApplications.ProgramApplicationQueryResults([], 0);

    _programApplicationRepositoryMock
    .Setup(r => r.Query(It.IsAny<ProgramApplicationQuery>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync((ProgramApplicationQuery query, CancellationToken ct) =>
    {
      switch (query.ById)
      {
        case var id when id == incorrectProgramApplicationId:
          return incorrectResourcesProgramApplicationQueryResults;

        case var id when id == correctProgramApplicationId:
          return correctResourcesProgramApplicationQueryResults;

        case var id when id == notFoundProgramApplicationId:
          return emptyResourcesProgramApplicationQueryResults;

        default:
          return emptyResourcesProgramApplicationQueryResults;
      }
    });

    _courseRepositoryMock
    .Setup(r => r.AddCourse(
      It.Is<Resources.Documents.Shared.Course>(course =>
        course.CourseId == courseId
        && course.CourseAreaOfInstruction != null
        && course.CourseAreaOfInstruction.Count() == 1
        && course.CourseAreaOfInstruction.Single().CourseAreaOfInstructionId == courseAreaOfInstructionId
        && course.CourseAreaOfInstruction.Single().AreaOfInstructionId == areaOfInstructionId
        && course.CourseAreaOfInstruction.Single().NewHours == 0),
      It.IsAny<string>(),
      It.IsAny<string>(),
      It.IsAny<CancellationToken>()))
    .ReturnsAsync(courseId);

    var handler = new CoursesHandler(_programProfileRepositoryMock.Object, _courseRepositoryMock.Object, _programApplicationRepositoryMock.Object, _coursesMapper);

    var incorrectProgramApplicationTypeResult = await handler.Handle(incorrectCommand, CancellationToken.None);
    var notFoundProgramApplicationTypeResult = await handler.Handle(notFoundCommand, CancellationToken.None);
    var correctProgramApplicationTypeResult = await handler.Handle(correctCommand, CancellationToken.None);

    incorrectProgramApplicationTypeResult.Error.ShouldBe(SaveCourseError.IncorrectProgramApplicationTypeToSaveCourse);
    notFoundProgramApplicationTypeResult.Error.ShouldBe(SaveCourseError.ProgramApplicationNotFound);
    correctProgramApplicationTypeResult.CourseId.ShouldBe(courseId);
  }
}
