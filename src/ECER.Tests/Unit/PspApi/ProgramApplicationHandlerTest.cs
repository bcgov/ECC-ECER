using ECER.Engines.Validation.ProgramApplications;
using ECER.Managers.Registry;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.ProgramApplications;
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
  private readonly Mock<IProgramApplicationRepository> _repositoryMock;
  private readonly Mock<IMetadataResourceRepository> _metadataRepositoryMock;
  private readonly Mock<ICourseRepository> _courseRepositoryMock;
  private readonly Mock<IProgramApplicationValidationEngine> _validationEngineMock;
  private readonly IProgramApplicationMapper _programApplicationMapper;
  private readonly ICoursesMapper _coursesMapper;

  public ProgramApplicationHandlerTest()
  {
    _repositoryMock = new Mock<IProgramApplicationRepository>();
    _metadataRepositoryMock = new Mock<IMetadataResourceRepository>();
    _courseRepositoryMock = new Mock<ICourseRepository>();
    _validationEngineMock = new Mock<IProgramApplicationValidationEngine>();
    _programApplicationMapper = new ProgramApplicationMapper();
    _coursesMapper = new CoursesMapper();
  }

  [Fact]
  public async Task Handle_CreateProgramApplicationCommand_CreatesAndReturnsApplication()
  {
    var instituteId = Guid.NewGuid().ToString();
    var contractApplication = new ContractProgramApplication(null, instituteId)
    {
      ProgramApplicationName = "Test Application",
      ProgramApplicationType = ContractApplicationType.NewBasicECEPostBasicProgram,
      ProgramTypes = new[] { ContractProgramCertificationType.Basic },
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

    _repositoryMock
      .Setup(r => r.Create(
        It.Is<ResourcesProgramApplication>(application =>
          application.PostSecondaryInstituteId == instituteId
          && application.ProgramApplicationName == contractApplication.ProgramApplicationName
          && application.ProgramApplicationType == ApplicationType.NewBasicECEPostBasicProgram
          && application.Status == ResourcesApplicationStatus.Draft),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(createdId);
    _repositoryMock
      .Setup(r => r.Query(It.Is<ResourcesProgramApplicationQuery>(q => q.ById == createdId && q.ByPostSecondaryInstituteId == instituteId), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults(new[] { queriedApplication }, 1));

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _metadataRepositoryMock.Object, _courseRepositoryMock.Object, _validationEngineMock.Object, _programApplicationMapper, _coursesMapper);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    result!.Id.ShouldBe(createdId);
    result.ProgramApplicationName.ShouldBe(contractApplication.ProgramApplicationName);
    result.Status.ShouldBe(ContractApplicationStatus.Draft);
    _repositoryMock.Verify(r => r.Create(It.IsAny<ResourcesProgramApplication>(), It.IsAny<CancellationToken>()), Times.Once);
    _repositoryMock.Verify(r => r.Query(It.IsAny<ResourcesProgramApplicationQuery>(), It.IsAny<CancellationToken>()), Times.Once);
  }
  
  [Fact]
  public async Task Handle_CreateProgramApplicationCommand_Type_NEWCAMPUS_CreatesAndReturnsApplication()
  {
    var instituteId = Guid.NewGuid().ToString();
    var contractApplication = new ContractProgramApplication(null, instituteId)
    {
      ProgramApplicationName = "Test Application",
      ProgramApplicationType = ContractApplicationType.NewCampusatRecognizedPrivateInstitution,
      ProgramTypes = new[] { ContractProgramCertificationType.Basic },
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

    _repositoryMock
      .Setup(r => r.Create(
        It.Is<ResourcesProgramApplication>(application =>
          application.PostSecondaryInstituteId == instituteId
          && application.ProgramApplicationType == ApplicationType.NewCampusatRecognizedPrivateInstitution
          && application.Status == ResourcesApplicationStatus.Draft),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(createdId);
    _repositoryMock
      .Setup(r => r.Query(It.Is<ResourcesProgramApplicationQuery>(q => q.ById == createdId && q.ByPostSecondaryInstituteId == instituteId), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults(new[] { queriedApplication }, 1));

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _metadataRepositoryMock.Object, _courseRepositoryMock.Object, _validationEngineMock.Object, _programApplicationMapper, _coursesMapper);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    result!.Id.ShouldBe(createdId);
    result.ProgramApplicationName.ShouldBe(contractApplication.ProgramApplicationName);
    result.Status.ShouldBe(ContractApplicationStatus.Draft);
    result.ProgramApplicationType.ShouldBe(contractApplication.ProgramApplicationType);
    _repositoryMock.Verify(r => r.Create(It.IsAny<ResourcesProgramApplication>(), It.IsAny<CancellationToken>()), Times.Once);
    _repositoryMock.Verify(r => r.Query(It.IsAny<ResourcesProgramApplicationQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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
    var command = new CreateProgramApplicationCommand(contractApplication);

    _repositoryMock.Setup(r => r.Create(It.IsAny<ResourcesProgramApplication>(), It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid().ToString());
    _repositoryMock.Setup(r => r.Query(It.IsAny<ResourcesProgramApplicationQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults(Array.Empty<ResourcesProgramApplication>(), 0));

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _metadataRepositoryMock.Object, _courseRepositoryMock.Object, _validationEngineMock.Object, _programApplicationMapper, _coursesMapper);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldBeNull();
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_NullRequest_ThrowsArgumentNullException()
  {
    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _metadataRepositoryMock.Object, _courseRepositoryMock.Object, _validationEngineMock.Object, _programApplicationMapper, _coursesMapper);

    await Should.ThrowAsync<ArgumentNullException>(() => handler.Handle((UpdateComponentGroupCommand)null!, CancellationToken.None));
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_UpdatesAndReturnsUpdatedComponentGroup()
  {
    var appId = Guid.NewGuid().ToString();
    var groupId = Guid.NewGuid().ToString();
    var componentId = Guid.NewGuid().ToString();
    var postSecondaryInstituteId = Guid.NewGuid().ToString();

    var contractComponents = new[] { new ContractProgramApplicationComponent(componentId, "Q1", "What is X?", 1, "Answer A", null, null) };
    var contractComponentGroup = new ContractComponentGroupWithComponents(groupId, "Group 1", "Instruction text", "InProgress", "Category A", 1, contractComponents);
    var command = new UpdateComponentGroupCommand(contractComponentGroup, appId, postSecondaryInstituteId);

    _repositoryMock
      .Setup(r => r.UpdateComponentGroup(
        It.Is<ResourcesComponentGroupWithComponents>(group =>
          group.Id == groupId
          && group.Name == "Group 1"
          && group.Instruction == "Instruction text"
          && group.Components.Single().Id == componentId
          && group.Components.Single().Answer == "Answer A"),
        appId,
        postSecondaryInstituteId,
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(groupId);

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _metadataRepositoryMock.Object, _courseRepositoryMock.Object, _validationEngineMock.Object, _programApplicationMapper, _coursesMapper);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldBe(groupId);
    _repositoryMock.Verify(r => r.UpdateComponentGroup(It.IsAny<ResourcesComponentGroupWithComponents>(), appId, postSecondaryInstituteId, It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_UsesCommandProgramApplicationIdForPostUpdateQuery()
  {
    var appId = Guid.NewGuid().ToString();
    var groupId = Guid.NewGuid().ToString();
    var postSecondaryInstituteId = Guid.NewGuid().ToString();

    var contractComponentGroup = new ContractComponentGroupWithComponents(groupId, "Group", null, "Draft", "Cat", 1, Array.Empty<ContractProgramApplicationComponent>());
    var command = new UpdateComponentGroupCommand(contractComponentGroup, appId, postSecondaryInstituteId);

    _repositoryMock
      .Setup(r => r.UpdateComponentGroup(It.IsAny<ResourcesComponentGroupWithComponents>(), appId, postSecondaryInstituteId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(groupId);

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _metadataRepositoryMock.Object, _courseRepositoryMock.Object, _validationEngineMock.Object, _programApplicationMapper, _coursesMapper);
    await handler.Handle(command, CancellationToken.None);

    _repositoryMock.Verify(r => r.UpdateComponentGroup(It.IsAny<ResourcesComponentGroupWithComponents>(), appId, postSecondaryInstituteId, It.IsAny<CancellationToken>()), Times.Once);
  }
}
