using ECER.Utilities.ObjectStorage.Providers;
using Riok.Mapperly.Abstractions;
using ContractInvestigationReconsiderations = ECER.Managers.Registry.Contract.InvestigationReconsiderations;

namespace ECER.Clients.RegistryPortal.Server.InvestigationReconsiderations;

internal interface IInvestigationReconsiderationsMapper
{
  ContractInvestigationReconsiderations.InvestigationReconsideration MapInvestigationReconsiderationRequest(InvestigationReconsideration source);

  InvestigationReconsideration MapInvestigationReconsiderationRequest(ContractInvestigationReconsiderations.InvestigationReconsideration source);

  IEnumerable<InvestigationReconsideration> MapInvestigationReconsiderationRequests(IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsideration> source);

  IEnumerable<InvestigationReconsiderationStatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode> source);

  IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<InvestigationReconsiderationStatusCode> source);
}

[Mapper]
internal partial class InvestigationReconsiderationsMapper : IInvestigationReconsiderationsMapper
{
  public ContractInvestigationReconsiderations.InvestigationReconsideration MapInvestigationReconsiderationRequest(InvestigationReconsideration source) => new ContractInvestigationReconsiderations.InvestigationReconsideration()
  {
    Id = source.Id,
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  public InvestigationReconsideration MapInvestigationReconsiderationRequest(ContractInvestigationReconsiderations.InvestigationReconsideration source) => new InvestigationReconsideration()
  {
    Id = source.Id,
    Status = MapInvestigationReconsiderationStatus(source.Status),
    Type = MapInvestigationReconsiderationType(source.Type),
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
    ReconsiderationEndDate = source.ReconsiderationEndDate,
  };

  public IEnumerable<InvestigationReconsideration> MapInvestigationReconsiderationRequests(IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsideration> source) => source.Select(MapInvestigationReconsiderationRequest).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InvestigationReconsiderationStatusCode MapInvestigationReconsiderationStatus(ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode MapInvestigationReconsiderationStatus(InvestigationReconsiderationStatusCode source);

  public IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<InvestigationReconsiderationStatusCode> source) => source.Select(MapInvestigationReconsiderationStatus).ToList();

  public IEnumerable<InvestigationReconsiderationStatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode> source) => source.Select(MapInvestigationReconsiderationStatus).ToList();

  private static Applications.FileInfo MapFileInfo(ContractInvestigationReconsiderations.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
  };

  private static ContractInvestigationReconsiderations.FileInfo MapFileInfo(Applications.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = EcerWebApplicationType.Registry
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InvestigationReconsiderationType MapInvestigationReconsiderationType(ContractInvestigationReconsiderations.InvestigationReconsiderationType source);
}
