using ECER.Managers.Registry;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Utilities.ObjectStorage.Providers;
using Shouldly;
using ContractFileInfo = ECER.Managers.Registry.Contract.ProgramApplications.FileInfo;

namespace ECER.Tests.Unit.PspApi;

public class ProgramApplicationMapperTests
{
  [Fact]
  public void MapProgramApplication_PreservesDeclarantAndProgressFields()
  {
    var mapper = new ProgramApplicationMapper();
    var applicationId = Guid.NewGuid().ToString();
    var instituteId = Guid.NewGuid().ToString();

    var source = new ProgramApplication(applicationId, instituteId)
    {
      DeclarantId = Guid.NewGuid().ToString(),
      DeclarantName = "Program Rep",
      BasicProgress = "Completed",
      IteProgress = "InProgress",
      SneProgress = "ToDo",
    };

    var resource = mapper.MapProgramApplication(source);
    var mappedBack = mapper.MapProgramApplication(resource);

    resource.DeclarantId.ShouldBe(source.DeclarantId);
    resource.DeclarantName.ShouldBe(source.DeclarantName);
    resource.BasicProgress.ShouldBe(source.BasicProgress);
    resource.IteProgress.ShouldBe(source.IteProgress);
    resource.SneProgress.ShouldBe(source.SneProgress);

    mappedBack.ShouldNotBeNull();
    mappedBack.DeclarantId.ShouldBe(source.DeclarantId);
    mappedBack.DeclarantName.ShouldBe(source.DeclarantName);
    mappedBack.BasicProgress.ShouldBe(source.BasicProgress);
    mappedBack.IteProgress.ShouldBe(source.IteProgress);
    mappedBack.SneProgress.ShouldBe(source.SneProgress);
  }

  [Fact]
  public void MapComponentGroupWithComponents_PreservesFileOperations()
  {
    var mapper = new ProgramApplicationMapper();
    var componentId = Guid.NewGuid().ToString();
    var addedFileId = Guid.NewGuid().ToString();
    var deletedFileId = Guid.NewGuid().ToString();

    var component = new ProgramApplicationComponent(componentId, "Component", "Question", 1, "Answer", null, false)
    {
      NewFiles =
      [
        new ContractFileInfo(addedFileId)
        {
          EcerWebApplicationType = EcerWebApplicationType.PSP,
          Name = "added.pdf",
        }
      ],
      DeletedFiles =
      [
        new ContractFileInfo(deletedFileId)
        {
          EcerWebApplicationType = EcerWebApplicationType.PSP,
          Name = "deleted.pdf",
        }
      ]
    };

    var source = new ComponentGroupWithComponents(Guid.NewGuid().ToString(), "Group", null, "Draft", "Category", 1, [component]);

    var result = mapper.MapComponentGroupWithComponents(source);
    var mappedComponent = result.Components.Single();

    mappedComponent.Id.ShouldBe(componentId);
    mappedComponent.Answer.ShouldBe("Answer");
    mappedComponent.NewFiles.Single().Id.ShouldBe(addedFileId);
    mappedComponent.NewFiles.Single().EcerWebApplicationType.ShouldBe(EcerWebApplicationType.PSP);
    mappedComponent.NewFiles.Single().Name.ShouldBe("added.pdf");
    mappedComponent.DeletedFiles.Single().Id.ShouldBe(deletedFileId);
    mappedComponent.DeletedFiles.Single().EcerWebApplicationType.ShouldBe(EcerWebApplicationType.PSP);
    mappedComponent.DeletedFiles.Single().Name.ShouldBe("deleted.pdf");
  }
}
