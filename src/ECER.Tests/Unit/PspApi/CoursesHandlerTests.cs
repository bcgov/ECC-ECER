using AutoMapper;
using ECER.Engines.Validation;
using ECER.Managers.Registry;
using ECER.Managers.Registry.Contract.Courses;
using ECER.Managers.Registry.Contract.Shared;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.ProgramApplications;
using ECER.Resources.Documents.Programs;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Moq;
using Shouldly;

namespace ECER.Tests.Unit.PspApi;

public class CoursesHandlerTests
{
  private readonly Mock<IProgramApplicationRepository> _programApplicationRepositoryMock;
  private readonly Mock<IProgramRepository> _programProfileRepositoryMock;
  private readonly Mock<ICourseRepository> _courseRepositoryMock;
  private readonly Mock<IMetadataResourceRepository> _metadataRepositoryMock;
  private readonly Mock<ICourseProgressEvaluator> _courseProgressEvaluatorMock;
  private readonly Mock<IMapper> _mapperMock;
  private readonly EcerContext _ecerContext;

  public CoursesHandlerTests()
  {
    _programApplicationRepositoryMock = new Mock<IProgramApplicationRepository>();
    _programProfileRepositoryMock = new Mock<IProgramRepository>();
    _courseRepositoryMock = new Mock<ICourseRepository>();
    _metadataRepositoryMock = new Mock<IMetadataResourceRepository>();
    _courseProgressEvaluatorMock = new Mock<ICourseProgressEvaluator>();
    _mapperMock = new Mock<IMapper>();
    _ecerContext = new EcerContext(new Mock<IOrganizationService>().Object);

    _courseRepositoryMock
      .Setup(r => r.GetCourses(It.IsAny<GetCoursesRequest>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new List<ECER.Resources.Documents.Shared.Course>());

    _metadataRepositoryMock
      .Setup(r => r.QueryAreaOfInstructions(It.IsAny<AreaOfInstructionsQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new List<AreaOfInstruction>());

    _programApplicationRepositoryMock
      .Setup(r => r.UpdateCourseProgress(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    _courseProgressEvaluatorMock
      .Setup(e => e.EvaluateProgress(It.IsAny<IEnumerable<Course>>(), It.IsAny<string>(), It.IsAny<IReadOnlyCollection<AreaOfInstruction>>(), It.IsAny<bool>()))
      .Returns("ToDo");

    _mapperMock
      .Setup(m => m.Map<IList<Course>>(It.IsAny<object>()))
      .Returns(new List<Course>());

    _mapperMock
      .Setup(m => m.Map<ECER.Resources.Documents.Shared.Course>(It.IsAny<object>()))
      .Returns(new ECER.Resources.Documents.Shared.Course { CourseId = "mapped-id", CourseNumber = "100", CourseTitle = "Test", ProgramType = "SNE" });
  }

  private CoursesHandler CreateHandler() =>
    new CoursesHandler(
      _programProfileRepositoryMock.Object,
      _courseRepositoryMock.Object,
      _programApplicationRepositoryMock.Object,
      _metadataRepositoryMock.Object,
      _courseProgressEvaluatorMock.Object,
      _mapperMock.Object,
      _ecerContext);

  [Fact]
  public async Task Handle_AddCourseCommand_GivenIncorrectProgramApplicationType_ShouldReturnError_GivenCorrectInput_ReturnsCourseId()
  {
    var instituteId = Guid.NewGuid().ToString();
    var incorrectProgramApplicationId = Guid.NewGuid().ToString();
    var correctProgramApplicationId = Guid.NewGuid().ToString();
    var notFoundProgramApplicationId = Guid.NewGuid().ToString();
    var courseId = Guid.NewGuid().ToString();

    var addCourse = new Course
    {
      CourseId = courseId,
      CourseAreaOfInstruction = [new CourseAreaOfInstruction { AreaOfInstructionId = Guid.NewGuid().ToString(), NewHours = "0", CourseAreaOfInstructionId = Guid.NewGuid().ToString() }],
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
        return query.ById switch
        {
          var id when id == incorrectProgramApplicationId => incorrectResourcesProgramApplicationQueryResults,
          var id when id == correctProgramApplicationId => correctResourcesProgramApplicationQueryResults,
          _ => emptyResourcesProgramApplicationQueryResults
        };
      });

    _courseRepositoryMock
      .Setup(r => r.AddCourse(
        It.IsAny<Resources.Documents.Shared.Course>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<CancellationToken>()
      ))
      .ReturnsAsync(courseId);

    var handler = CreateHandler();

    var incorrectProgramApplicationTypeResult = await handler.Handle(incorrectCommand, CancellationToken.None);
    var notFoundProgramApplicationTypeResult = await handler.Handle(notFoundCommand, CancellationToken.None);
    var correctProgramApplicationTypeResult = await handler.Handle(correctCommand, CancellationToken.None);

    incorrectProgramApplicationTypeResult.Error.ShouldBe(SaveCourseError.IncorrectProgramApplicationTypeToSaveCourse);
    notFoundProgramApplicationTypeResult.Error.ShouldBe(SaveCourseError.ProgramApplicationNotFound);
    correctProgramApplicationTypeResult.CourseId.ShouldBe(courseId);
  }

  [Fact]
  public async Task Handle_DeleteCourseCommand_WithoutApplicationId_OnlyDeletesCourse()
  {
    var courseId = Guid.NewGuid().ToString();
    var instituteId = Guid.NewGuid().ToString();

    _courseRepositoryMock
      .Setup(r => r.DeleteCourse(courseId, instituteId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(courseId);

    var command = new DeleteCourseCommand(courseId, instituteId);
    var handler = CreateHandler();

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldBe(courseId);
    _courseRepositoryMock.Verify(r => r.DeleteCourse(courseId, instituteId, It.IsAny<CancellationToken>()), Times.Once);
    _programApplicationRepositoryMock.Verify(
      r => r.UpdateCourseProgress(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public async Task Handle_DeleteCourseCommand_WithApplicationId_DeletesCourseAndUpdatesProgress()
  {
    var courseId = Guid.NewGuid().ToString();
    var applicationId = Guid.NewGuid().ToString();
    var instituteId = Guid.NewGuid().ToString();

    var programApplication = new ProgramApplication(applicationId, instituteId)
    {
      ProgramApplicationType = ApplicationType.NewBasicECEPostBasicProgram,
      ProgramTypes = new[] { ProgramCertificationType.Basic },
      Status = ApplicationStatus.Draft
    };

    _programApplicationRepositoryMock
      .Setup(r => r.Query(It.IsAny<ProgramApplicationQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ProgramApplicationQueryResults([programApplication], 1));

    _courseRepositoryMock
      .Setup(r => r.DeleteCourse(courseId, instituteId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(courseId);

    var command = new DeleteCourseCommand(courseId, instituteId, applicationId);
    var handler = CreateHandler();

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldBe(courseId);
    _courseRepositoryMock.Verify(r => r.DeleteCourse(courseId, instituteId, It.IsAny<CancellationToken>()), Times.Once);
    _programApplicationRepositoryMock.Verify(
      r => r.UpdateCourseProgress(applicationId, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
      Times.Once);
  }

  [Fact]
  public async Task Handle_UpdateCourseCommand_ForProgramApplication_UpdatesCourseAndProgress()
  {
    var courseId = Guid.NewGuid().ToString();
    var applicationId = Guid.NewGuid().ToString();
    var instituteId = Guid.NewGuid().ToString();

    var programApplication = new ProgramApplication(applicationId, instituteId)
    {
      ProgramApplicationType = ApplicationType.NewBasicECEPostBasicProgram,
      ProgramTypes = new[] { ProgramCertificationType.Basic },
      Status = ApplicationStatus.Draft
    };

    _programApplicationRepositoryMock
      .Setup(r => r.Query(It.IsAny<ProgramApplicationQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ProgramApplicationQueryResults([programApplication], 1));

    _courseRepositoryMock
      .Setup(r => r.UpdateCourse(It.IsAny<Resources.Documents.Shared.Course>(), applicationId, true, It.IsAny<CancellationToken>()))
      .ReturnsAsync(applicationId);

    var course = new Course { CourseId = courseId, CourseAreaOfInstruction = [] };
    var command = new UpdateCourseCommand(course, applicationId, nameof(Resources.Documents.Courses.FunctionType.ProgramApplication), instituteId);
    var handler = CreateHandler();

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldBe(applicationId);
    _courseRepositoryMock.Verify(r => r.UpdateCourse(It.IsAny<Resources.Documents.Shared.Course>(), applicationId, true, It.IsAny<CancellationToken>()), Times.Once);
    _programApplicationRepositoryMock.Verify(
      r => r.UpdateCourseProgress(applicationId, It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
      Times.Once);
  }
}
