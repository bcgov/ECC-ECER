using ECER.Managers.Admin;
using Shouldly;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;
using ResourceMetadata = ECER.Resources.Documents.MetadataResources;

namespace ECER.Tests.Unit.Admin;

public class MetadataMapperTests
{
  [Fact]
  public void MapPostSecondaryInstitutionsQuery_MapsFilterFields()
  {
    var mapper = new MetadataMapper();
    var source = new ContractMetadatas.PostSecondaryInstitutionsQuery
    {
      ById = Guid.NewGuid().ToString(),
      ByProvinceId = Guid.NewGuid().ToString(),
      ByName = "Institute",
      ByStatus = ContractMetadatas.PostSecondaryInstitutionStatus.Active,
    };

    var result = mapper.MapPostSecondaryInstitutionsQuery(source);

    result.ById.ShouldBe(source.ById);
    result.ByProvinceId.ShouldBe(source.ByProvinceId);
    result.ByName.ShouldBe(source.ByName);
    result.ByStatus.ShouldBe(ResourceMetadata.PostSecondaryInstitutionStatus.Active);
  }

  [Fact]
  public void MapSystemMessages_MapsDatesAndPortalTags()
  {
    var mapper = new MetadataMapper();
    var source = new ResourceMetadata.SystemMessage("Name", "Subject", "Message")
    {
      StartDate = new DateTime(2025, 1, 1),
      EndDate = new DateTime(2025, 2, 1),
      PortalTags = [ResourceMetadata.PortalTags.LOGIN, ResourceMetadata.PortalTags.REFERENCES],
    };

    var result = mapper.MapSystemMessages([source]).Single();

    result.Name.ShouldBe(source.Name);
    result.Subject.ShouldBe(source.Subject);
    result.Message.ShouldBe(source.Message);
    result.StartDate.ShouldBe(source.StartDate);
    result.EndDate.ShouldBe(source.EndDate);
    result.PortalTags.ShouldBe([ContractMetadatas.PortalTags.LOGIN, ContractMetadatas.PortalTags.REFERENCES]);
  }

  [Fact]
  public void MapDynamicsConfig_MapsIcraFeatureFlagFromNamedSetting()
  {
    var mapper = new MetadataMapper();
    var source = new[]
    {
      new ResourceMetadata.DynamicsConfig("Unrelated", "OFF"),
      new ResourceMetadata.DynamicsConfig("ICRA Feature", "ON"),
    };

    var result = mapper.MapDynamicsConfig(source);

    result.ICRAFeatureEnabled.ShouldBeTrue();
  }
}
