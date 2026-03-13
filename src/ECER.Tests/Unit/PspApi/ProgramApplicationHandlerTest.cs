using AutoMapper;
using ECER.Managers.Registry;
using ECER.Resources.Documents.ProgramApplications;
using Moq;
using Shouldly;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using UpdateComponentGroupCommand = ECER.Managers.Registry.Contract.ProgramApplications.UpdateComponentGroupCommand;
using ContractProgramApplication = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplication;
using ContractApplicationStatus = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationStatus;
using ContractApplicationType = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationType;
using ContractProgramCertificationType = ECER.Managers.Registry.Contract.ProgramApplications.ProgramCertificationType;
using ContractDeliveryType = ECER.Managers.Registry.Contract.ProgramApplications.DeliveryType;
using ContractComponentGroupWithComponents = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponents;
using ContractProgramApplicationComponent = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationComponent;
using ResourcesProgramApplication = ECER.Resources.Documents.ProgramApplications.ProgramApplication;
using ResourcesProgramApplicationQuery = ECER.Resources.Documents.ProgramApplications.ProgramApplicationQuery;
using ResourcesProgramApplicationQueryResults = ECER.Resources.Documents.ProgramApplications.ProgramApplicationQueryResults;
using ResourcesApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;
using ResourcesComponentGroupWithComponents = ECER.Resources.Documents.ProgramApplications.ComponentGroupWithComponents;
using ResourcesComponentGroupWithComponentsQuery = ECER.Resources.Documents.ProgramApplications.ComponentGroupWithComponentsQuery;
using ResourcesProgramApplicationComponent = ECER.Resources.Documents.ProgramApplications.ProgramApplicationComponent;

namespace ECER.Tests.Unit.PspApi;

public class ProgramApplicationHandlerTest
{
  private readonly Mock<IProgramApplicationRepository> _repositoryMock;
  private readonly Mock<IMapper> _mapperMock;

  public ProgramApplicationHandlerTest()
  {
    _repositoryMock = new Mock<IProgramApplicationRepository>();
    _mapperMock = new Mock<IMapper>();
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

    var resourcesApplication = new ResourcesProgramApplication(null!, instituteId)
    {
      ProgramApplicationName = contractApplication.ProgramApplicationName,
      Status = ResourcesApplicationStatus.Draft
    };
    var createdId = Guid.NewGuid().ToString();
    var queriedApplication = new ResourcesProgramApplication(createdId, instituteId)
    {
      ProgramApplicationName = contractApplication.ProgramApplicationName,
      Status = ResourcesApplicationStatus.Draft
    };
    var expectedContract = new ContractProgramApplication(createdId, instituteId)
    {
      ProgramApplicationName = contractApplication.ProgramApplicationName,
      Status = ContractApplicationStatus.Draft
    };

    _mapperMock
      .Setup(m => m.Map<ResourcesProgramApplication>(contractApplication))
      .Returns(resourcesApplication);
    _repositoryMock
      .Setup(r => r.Create(It.IsAny<ResourcesProgramApplication>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(createdId);
    _repositoryMock
      .Setup(r => r.Query(It.Is<ResourcesProgramApplicationQuery>(q => q.ById == createdId && q.ByPostSecondaryInstituteId == instituteId), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults(new[] { queriedApplication }, 1));
    _mapperMock
      .Setup(m => m.Map<ContractProgramApplication>(queriedApplication))
      .Returns(expectedContract);

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _mapperMock.Object);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    result!.Id.ShouldBe(createdId);
    result.ProgramApplicationName.ShouldBe(contractApplication.ProgramApplicationName);
    result.Status.ShouldBe(ContractApplicationStatus.Draft);
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
    var resourcesApplication = new ResourcesProgramApplication(null!, instituteId);

    _mapperMock.Setup(m => m.Map<ResourcesProgramApplication>(contractApplication)).Returns(resourcesApplication);
    _repositoryMock.Setup(r => r.Create(It.IsAny<ResourcesProgramApplication>(), It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid().ToString());
    _repositoryMock.Setup(r => r.Query(It.IsAny<ResourcesProgramApplicationQuery>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ResourcesProgramApplicationQueryResults(Array.Empty<ResourcesProgramApplication>(), 0));

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _mapperMock.Object);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldBeNull();
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_NullRequest_ThrowsArgumentNullException()
  {
    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _mapperMock.Object);

    await Should.ThrowAsync<ArgumentNullException>(() => handler.Handle((UpdateComponentGroupCommand)null!, CancellationToken.None));
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_UpdatesAndReturnsUpdatedComponentGroup()
  {
    var appId = Guid.NewGuid().ToString();
    var groupId = Guid.NewGuid().ToString();
    var componentId = Guid.NewGuid().ToString();

    var contractComponents = new[] { new ContractProgramApplicationComponent(componentId, "Q1", "What is X?", 1, "Answer A", null, null) };
    var contractComponentGroup = new ContractComponentGroupWithComponents(groupId, "Group 1", "Instruction text", "InProgress", "Category A", 1, contractComponents);
    var command = new UpdateComponentGroupCommand(contractComponentGroup, appId);

    var resourcesComponentGroup = new ResourcesComponentGroupWithComponents(groupId, "Group 1", "Instruction text", "InProgress", "Category A", 1, Array.Empty<ResourcesProgramApplicationComponent>());
    var resourcesResult = new [] {new ResourcesComponentGroupWithComponents(groupId, "Group 1", "Instruction text", "InProgress", "Category A", 1, Array.Empty<ResourcesProgramApplicationComponent>())};

    _mapperMock
      .Setup(m => m.Map<ResourcesComponentGroupWithComponents>(contractComponentGroup))
      .Returns(resourcesComponentGroup);
    _repositoryMock
      .Setup(r => r.UpdateComponentGroup(resourcesComponentGroup, appId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(groupId);
    _repositoryMock
      .Setup(r => r.QueryComponentGroupWithComponents(
        It.Is<ResourcesComponentGroupWithComponentsQuery>(q => q.ByComponentGroupId == groupId && q.ByProgramApplicationId == appId),
        It.IsAny<CancellationToken>()))
      .ReturnsAsync(resourcesResult);
    _mapperMock
      .Setup(m => m.Map<IEnumerable<ContractProgramApplicationComponent>>(resourcesResult.SingleOrDefault()!.Components))
      .Returns(contractComponents);

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _mapperMock.Object);

    var result = await handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    _repositoryMock.Verify(r => r.UpdateComponentGroup(It.IsAny<ResourcesComponentGroupWithComponents>(), appId, It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task Handle_UpdateComponentGroupCommand_UsesCommandProgramApplicationIdForPostUpdateQuery()
  {
    var appId = Guid.NewGuid().ToString();
    var groupId = Guid.NewGuid().ToString();

    var contractComponentGroup = new ContractComponentGroupWithComponents(groupId, "Group", null, "Draft", "Cat", 1, Array.Empty<ContractProgramApplicationComponent>());
    var command = new UpdateComponentGroupCommand(contractComponentGroup, appId);

    var resourcesComponentGroup = new ResourcesComponentGroupWithComponents(groupId, "Group", null, "Draft", "Cat", 1, Array.Empty<ResourcesProgramApplicationComponent>());
    
    _mapperMock.Setup(m => m.Map<ResourcesComponentGroupWithComponents>(contractComponentGroup)).Returns(resourcesComponentGroup);
    _repositoryMock.Setup(r => r.UpdateComponentGroup(It.IsAny<ResourcesComponentGroupWithComponents>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(groupId);
   _mapperMock.Setup(m => m.Map<IEnumerable<ContractProgramApplicationComponent>>(It.IsAny<IEnumerable<ResourcesProgramApplicationComponent>>())).Returns(Array.Empty<ContractProgramApplicationComponent>());

    var handler = new ProgramApplicationHandler(_repositoryMock.Object, _mapperMock.Object);
    await handler.Handle(command, CancellationToken.None);
  }
}
