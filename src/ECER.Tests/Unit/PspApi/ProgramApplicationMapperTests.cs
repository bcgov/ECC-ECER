using ECER.Managers.Registry;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Utilities.ObjectStorage.Providers;
using Shouldly;
using ContractFileInfo = ECER.Managers.Registry.Contract.ProgramApplications.FileInfo;

namespace ECER.Tests.Unit.PspApi;

public class ProgramApplicationMapperTests
{
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
