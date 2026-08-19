using Riok.Mapperly.Abstractions;
using ContractReconsiderations = ECER.Managers.Registry.Contract.Reconsiderations;
using ResourceReconsiderations = ECER.Resources.Documents.Reconsiderations;

namespace ECER.Managers.Registry;

public interface IReconsiderationMapper
{
  ResourceReconsiderations.Reconsideration MapReconsiderationRequest(ContractReconsiderations.Reconsideration source);

  ContractReconsiderations.Reconsideration MapReconsiderationRequest(ResourceReconsiderations.Reconsideration source);

  IEnumerable<ContractReconsiderations.Reconsideration> MapReconsiderationRequests(IEnumerable<ResourceReconsiderations.Reconsideration> source);

  IEnumerable<ResourceReconsiderations.ReconsiderationStatusCode> MapReconsiderationStatusCodes(IEnumerable<ContractReconsiderations.ReconsiderationStatusCode> source);
}

[Mapper]
internal partial class ReconsiderationMapper : IReconsiderationMapper
{
  public ResourceReconsiderations.Reconsideration MapReconsiderationRequest(ContractReconsiderations.Reconsideration source) => new ResourceReconsiderations.Reconsideration()
  {
    Id = source.Id,
    ReconsiderationDetails = source.ReconsiderationDetails,
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  public ContractReconsiderations.Reconsideration MapReconsiderationRequest(ResourceReconsiderations.Reconsideration source) => new ContractReconsiderations.Reconsideration()
  {
    Id = source.Id,
    Status = MapReconsiderationStatus(source.Status),
    ReconsiderationDetails = source.ReconsiderationDetails,
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
    ReconsiderationEndDate = source.ReconsiderationEndDate,
    ApplicationId = source.ApplicationId,
  };

  private static ContractReconsiderations.FileInfo MapFileInfo(ResourceReconsiderations.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType
  };

  private static ResourceReconsiderations.FileInfo MapFileInfo(ContractReconsiderations.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType
  };

  public IEnumerable<ContractReconsiderations.Reconsideration> MapReconsiderationRequests(IEnumerable<ResourceReconsiderations.Reconsideration> source) => source.Select(MapReconsiderationRequest).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractReconsiderations.ReconsiderationStatusCode MapReconsiderationStatus(ResourceReconsiderations.ReconsiderationStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceReconsiderations.ReconsiderationStatusCode MapReconsiderationStatus(ContractReconsiderations.ReconsiderationStatusCode source);

  public IEnumerable<ResourceReconsiderations.ReconsiderationStatusCode> MapReconsiderationStatusCodes(IEnumerable<ContractReconsiderations.ReconsiderationStatusCode> source) => source.Select(MapReconsiderationStatus).ToList();
}
