using ECER.Engines.Validation.ProgramApplications;
using ECER.Managers.Registry;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.ProgramApplications;
using MediatR;
using Moq;
using Shouldly;
using ContractApplicationStatus = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationStatus;
using ContractApplicationType = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationType;
using ContractComponentGroupWithComponents = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponents;
using ContractDeliveryType = ECER.Managers.Registry.Contract.ProgramApplications.DeliveryType;
using ContractProgramApplication = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplication;
using ContractProgramApplicationComponent = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationComponent;
using ContractProgramCertificationType = ECER.Managers.Registry.Contract.ProgramApplications.ProgramCertificationType;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using ResourcesApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;
using ResourcesComponentGroupWithComponents = ECER.Resources.Documents.ProgramApplications.ComponentGroupWithComponents;
using ResourcesProgramApplication = ECER.Resources.Documents.ProgramApplications.ProgramApplication;
using ResourcesProgramApplicationQuery = ECER.Resources.Documents.ProgramApplications.ProgramApplicationQuery;
using ResourcesProgramApplicationQueryResults = ECER.Resources.Documents.ProgramApplications.ProgramApplicationQueryResults;
using UpdateComponentGroupCommand = ECER.Managers.Registry.Contract.ProgramApplications.UpdateComponentGroupCommand;

namespace ECER.Tests.Unit.PspApi;

public class ProgramApplicationHandlerTest
{
  private readonly Mock<IProgramApplicationRepository> repositoryMock;
  private readonly Mock<IMetadataResourceRepository> metadataRepositoryMock;
  private readonly Mock<ICourseRepository> courseRepositoryMock;
  private readonly Mock<IProgramApplicationValidationEngine> validationEngineMock;
  private readonly Mock<IMediator> mediatorMock;
  private readonly IProgramApplicationMapper programApplicationMapper;
  private readonly ICoursesMapper coursesMapper;

  public ProgramApplicationHandlerTest()
  {
    repositoryMock = new Mock<IProgramApplicationRepository>();
    metadataRepositoryMock = new Mock<IMetadataResourceRepository>();
    courseRepositoryMock = new Mock<ICourseRepository>();
    validationEngineMock = new Mock<IProgramApplicationValidationEngine>();
    mediatorMock = new Mock<IMediator>();
    programApplicationMapper = new ProgramApplicationMapper();
    coursesMapper = new CoursesMapper();
  }

  private ProgramApplicationHandler CreateHandler() =>
    new(
      repositoryMock.Object,
      metadataRepositoryMock.Object,
      courseRepositoryMock.Object,
      validationEngineMock.Object,
      programApplicationMapper,
      coursesMapper,
      mediatorMock.Object);

  [Fact]
  public async Task Handle_CreateProgramApplicationCommand_CreatesAndReturnsApplication()
  {
    var instituteId = Guid.NewGuid().ToString();
    var contractApplication = new ContractProgramApplication(null, instituteId)
    {
      ProgramApplicationName = "Test Application",
      ProgramApplicationType = ContractApplicationType.NewBasicECEPostBasicProgram,
      ProgramTypes = [ContractProgramCertificationType.Basic],
      DeliveryType = ContractDeliveryType.Hybrid,
      Status = ContractApplicationStatus.Draft
    };
    var command = new CreateProgramApplicationCommand(contractApplication);

    var createdId = Guid.NewGuid().ToString();
    var queriedApplication = new ResourcesProgramApplication(createdId, instituteId)
    {
      ProgramApplicationName = contractApplication.ProgramApplicationName,
      Status = ResourcesApplicationStatus.Draft
    };

    repositoryMock
      .Setup(repository => repository.Create(
        It.Is<ResourcesProgramApplication>(application =>
          application.PostSecondaryInstituteId == instituteId
          && application.ProgramApplicationName == contractApplication.ProgramApplicationName
          && application.ProgramApplicationType == ApplicationType.NewBasicECEPostBasicProgram
          && application.Status == ResourcesApplicationStatus.Draft),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(createdId);
    repositoryMock
      .Setup(repository => repository.Query(
        It.Is<ResourcesProgramApplicationQuery>(query => query.ById == createdId && query.ByPostSecondaryInstituteId == instituteId),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults([queriedApplication], 1));

    var handler = CreateHandler();
    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    result!.Id.ShouldBe(createdId);
    result.ProgramApplicationName.ShouldBe(contractApplication.ProgramApplicationName);
    result.Status.ShouldBe(ContractApplicationStatus.Draft);
  }

  [Fact]
  public async Task Handle_CreateProgramApplicationCommand_ForNewCampus_ReturnsApplication()
  {
    var instituteId = Guid.NewGuid().ToString();
    var contractApplication = new ContractProgramApplication(null, instituteId)
    {
      ProgramApplicationName = "Test Application",
      ProgramApplicationType = ContractApplicationType.NewCampusatRecognizedPrivateInstitution,
      ProgramTypes = [ContractProgramCertificationType.Basic],
      DeliveryType = ContractDeliveryType.Hybrid,
      Status = ContractApplicationStatus.Draft
    };
    var command = new CreateProgramApplicationCommand(contractApplication);

    var createdId = Guid.NewGuid().ToString();
    var queriedApplication = new ResourcesProgramApplication(createdId, instituteId)
    {
      ProgramApplicationName = contractApplication.ProgramApplicationName,
      Status = ResourcesApplicationStatus.Draft,
      ProgramApplicationType = ApplicationType.NewCampusatRecognizedPrivateInstitution
    };

    repositoryMock
      .Setup(repository => repository.Create(
        It.Is<ResourcesProgramApplication>(application =>
          application.PostSecondaryInstituteId == instituteId
          && application.ProgramApplicationType == ApplicationType.NewCampusatRecognizedPrivateInstitution
          && application.Status == ResourcesApplicationStatus.Draft),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(createdId);
    repositoryMock
      .Setup(repository => repository.Query(
        It.Is<ResourcesProgramApplicationQuery>(query => query.ById == createdId && query.ByPostSecondaryInstituteId == instituteId),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults([queriedApplication], 1));

    var handler = CreateHandler();
    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    result!.Id.ShouldBe(createdId);
    result.ProgramApplicationType.ShouldBe(contractApplication.ProgramApplicationType);
  }

  [Fact]
  public async Task Handle_CreateProgramApplicationCommand_WhenQueryReturnsEmpty_ReturnsNull()
  {
    var instituteId = Guid.NewGuid().ToString();
    var contractApplication = new ContractProgramApplication(null, instituteId)
    {
      ProgramApplicationName = "Test",
      Status = ContractApplicationStatus.Draft
    };

    repositoryMock
      .Setup(repository => repository.Create(It.IsAny<ResourcesProgramApplication>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(Guid.NewGuid().ToString());
    repositoryMock
      .Setup(repository => repository.Query(It.IsAny<ResourcesProgramApplicationQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults(Array.Empty<ResourcesProgramApplication>(), 0));

    var handler = CreateHandler();
    var result = await handler.Handle(new CreateProgramApplicationCommand(contractApplication), CancellationToken.None);

    result.ShouldBeNull();
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_NullRequest_ThrowsArgumentNullException()
  {
    var handler = CreateHandler();
    await Should.ThrowAsync<ArgumentNullException>(() => handler.Handle((UpdateComponentGroupCommand)null!, CancellationToken.None));
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_MapsAndUpdatesGroup()
  {
    var applicationId = Guid.NewGuid().ToString();
    var componentGroupId = Guid.NewGuid().ToString();
    var componentId = Guid.NewGuid().ToString();

    var source = new ContractComponentGroupWithComponents(
      componentGroupId,
      "Group 1",
      "Instruction text",
      "InProgress",
      "Category A",
      1,
      [new ContractProgramApplicationComponent(componentId, "Q1", "What is X?", 1, "Answer A", null, false)]);

    repositoryMock
      .Setup(repository => repository.UpdateComponentGroup(
        It.Is<ResourcesComponentGroupWithComponents>(group =>
          group.Id == componentGroupId
          && group.Name == "Group 1"
          && group.Instruction == "Instruction text"
          && group.Components.Single().Id == componentId
          && group.Components.Single().Answer == "Answer A"),
        applicationId,
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(componentGroupId);

    var handler = CreateHandler();
    var result = await handler.Handle(new UpdateComponentGroupCommand(source, applicationId), CancellationToken.None);

    result.ShouldBe(componentGroupId);
    repositoryMock.Verify(repository => repository.UpdateComponentGroup(It.IsAny<ResourcesComponentGroupWithComponents>(), applicationId, It.IsAny<CancellationToken>()), Times.Once);
  }
}
