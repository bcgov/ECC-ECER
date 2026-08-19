using ECER.Utilities.ObjectStorage.Providers;
using Riok.Mapperly.Abstractions;
using ContractReconsiderations = ECER.Managers.Registry.Contract.Reconsiderations;

namespace ECER.Clients.RegistryPortal.Server.Reconsiderations;

internal interface IReconsiderationsMapper
{
  ContractReconsiderations.Reconsideration MapReconsiderationRequest(Reconsideration source);

  Reconsideration MapReconsiderationRequest(ContractReconsiderations.Reconsideration source);

  IEnumerable<Reconsideration> MapReconsiderationRequests(IEnumerable<ContractReconsiderations.Reconsideration> source);

  IEnumerable<ReconsiderationStatusCode> MapReconsiderationStatusCodes(IEnumerable<ContractReconsiderations.ReconsiderationStatusCode> source);

  IEnumerable<ContractReconsiderations.ReconsiderationStatusCode> MapReconsiderationStatusCodes(IEnumerable<ReconsiderationStatusCode> source);
}

[Mapper]
internal partial class ReconsiderationsMapper : IReconsiderationsMapper
{
  public ContractReconsiderations.Reconsideration MapReconsiderationRequest(Reconsideration source) => new ContractReconsiderations.Reconsideration()
  {
    Id = source.Id,
    ReconsiderationDetails = source.ReconsiderationDetails,
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  public Reconsideration MapReconsiderationRequest(ContractReconsiderations.Reconsideration source) => new Reconsideration()
  {
    Id = source.Id,
    Status = MapReconsiderationStatus(source.Status),
    ReconsiderationDetails = source.ReconsiderationDetails,
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
    ReconsiderationEndDate = source.ReconsiderationEndDate,
    ApplicationId = source.ApplicationId
  };

  public IEnumerable<Reconsideration> MapReconsiderationRequests(IEnumerable<ContractReconsiderations.Reconsideration> source) => source.Select(MapReconsiderationRequest).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ReconsiderationStatusCode MapReconsiderationStatus(ContractReconsiderations.ReconsiderationStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractReconsiderations.ReconsiderationStatusCode MapReconsiderationStatus(ReconsiderationStatusCode source);

  public IEnumerable<ContractReconsiderations.ReconsiderationStatusCode> MapReconsiderationStatusCodes(IEnumerable<ReconsiderationStatusCode> source) => source.Select(MapReconsiderationStatus).ToList();

  public IEnumerable<ReconsiderationStatusCode> MapReconsiderationStatusCodes(IEnumerable<ContractReconsiderations.ReconsiderationStatusCode> source) => source.Select(MapReconsiderationStatus).ToList();

  private static FileInfo MapFileInfo(ContractReconsiderations.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = EcerWebApplicationType.Registry
  };

  private static ContractReconsiderations.FileInfo MapFileInfo(FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = EcerWebApplicationType.Registry
  };
}
