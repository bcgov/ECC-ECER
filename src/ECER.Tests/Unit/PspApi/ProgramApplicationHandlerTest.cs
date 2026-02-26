using AutoMapper;
using ECER.Managers.Registry;
using ECER.Resources.Documents.ProgramApplications;
using Moq;
using Shouldly;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using ContractProgramApplication = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplication;
using ContractApplicationStatus = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationStatus;
using ContractApplicationType = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationType;
using ContractProgramCertificationType = ECER.Managers.Registry.Contract.ProgramApplications.ProgramCertificationType;
using ContractDeliveryType = ECER.Managers.Registry.Contract.ProgramApplications.DeliveryType;
using ResourcesProgramApplication = ECER.Resources.Documents.ProgramApplications.ProgramApplication;
using ResourcesProgramApplicationQuery = ECER.Resources.Documents.ProgramApplications.ProgramApplicationQuery;
using ResourcesProgramApplicationQueryResults = ECER.Resources.Documents.ProgramApplications.ProgramApplicationQueryResults;
using ResourcesApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;

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
}
