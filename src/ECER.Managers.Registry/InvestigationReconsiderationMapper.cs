using Riok.Mapperly.Abstractions;
using ContractInvestigationReconsiderations = ECER.Managers.Registry.Contract.InvestigationReconsiderations;
using ResourceInvestigationReconsiderations = ECER.Resources.Documents.InvestigationReconsiderations;

namespace ECER.Managers.Registry;

public interface IInvestigationReconsiderationMapper
{
  ResourceInvestigationReconsiderations.InvestigationReconsideration MapInvestigationReconsiderationRequest(ContractInvestigationReconsiderations.InvestigationReconsideration source);

  ContractInvestigationReconsiderations.InvestigationReconsideration MapInvestigationReconsiderationRequest(ResourceInvestigationReconsiderations.InvestigationReconsideration source);

  IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsideration> MapInvestigationReconsiderationRequests(IEnumerable<ResourceInvestigationReconsiderations.InvestigationReconsideration> source);

  IEnumerable<ResourceInvestigationReconsiderations.InvestigationReconsiderationStatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode> source);
}

[Mapper]
internal partial class InvestigationReconsiderationMapper : IInvestigationReconsiderationMapper
{
  public ResourceInvestigationReconsiderations.InvestigationReconsideration MapInvestigationReconsiderationRequest(ContractInvestigationReconsiderations.InvestigationReconsideration source) => new ResourceInvestigationReconsiderations.InvestigationReconsideration()
  {
    Id = source.Id,
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
  };

  public ContractInvestigationReconsiderations.InvestigationReconsideration MapInvestigationReconsiderationRequest(ResourceInvestigationReconsiderations.InvestigationReconsideration source) => new ContractInvestigationReconsiderations.InvestigationReconsideration()
  {
    Id = source.Id,
    Status = MapInvestigationReconsiderationStatus(source.Status),
    Type = MapInvestigationReconsiderationType(source.Type),
    ExplanationAndEvidence = source.ExplanationAndEvidence,
    Files = source.Files.Select(MapFileInfo).ToList(),
    ReconsiderationEndDate = source.ReconsiderationEndDate,
  };

  private static ContractInvestigationReconsiderations.FileInfo MapFileInfo(ResourceInvestigationReconsiderations.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType
  };

  private static ResourceInvestigationReconsiderations.FileInfo MapFileInfo(ContractInvestigationReconsiderations.FileInfo source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType
  };

  public IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsideration> MapInvestigationReconsiderationRequests(IEnumerable<ResourceInvestigationReconsiderations.InvestigationReconsideration> source) => source.Select(MapInvestigationReconsiderationRequest).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode MapInvestigationReconsiderationStatus(ResourceInvestigationReconsiderations.InvestigationReconsiderationStatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceInvestigationReconsiderations.InvestigationReconsiderationStatusCode MapInvestigationReconsiderationStatus(ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode source);

  public IEnumerable<ResourceInvestigationReconsiderations.InvestigationReconsiderationStatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<ContractInvestigationReconsiderations.InvestigationReconsiderationStatusCode> source) => source.Select(MapInvestigationReconsiderationStatus).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractInvestigationReconsiderations.InvestigationReconsiderationType MapInvestigationReconsiderationType(ResourceInvestigationReconsiderations.InvestigationReconsiderationType source);
}
