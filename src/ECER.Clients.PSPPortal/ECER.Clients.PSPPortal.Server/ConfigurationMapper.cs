using ECER.Clients.PSPPortal.Server.EducationInstitutions;
using ECER.Clients.PSPPortal.Server.Programs;
using Riok.Mapperly.Abstractions;
using ContractMetadatas = ECER.Managers.Admin.Contract.Metadatas;

namespace ECER.Clients.PSPPortal.Server;

internal interface IConfigurationMapper
{
  IEnumerable<Province> MapProvinces(IEnumerable<ContractMetadatas.Province> source);
  IEnumerable<Country> MapCountries(IEnumerable<ContractMetadatas.Country> source);
  IEnumerable<AreaOfInstruction> MapAreaOfInstructions(IEnumerable<ContractMetadatas.AreaOfInstruction> source);
  IEnumerable<SystemMessage> MapSystemMessages(IEnumerable<ContractMetadatas.SystemMessage> source);
}

[Mapper]
internal partial class ConfigurationMapper : IConfigurationMapper
{
  public IEnumerable<Province> MapProvinces(IEnumerable<ContractMetadatas.Province> source) => source.Select(MapProvince).ToList();

  public IEnumerable<Country> MapCountries(IEnumerable<ContractMetadatas.Country> source) => source.Select(MapCountry).ToList();

  public IEnumerable<AreaOfInstruction> MapAreaOfInstructions(IEnumerable<ContractMetadatas.AreaOfInstruction> source) => source.Select(MapAreaOfInstruction).ToList();

  public IEnumerable<SystemMessage> MapSystemMessages(IEnumerable<ContractMetadatas.SystemMessage> source) => source.Select(MapSystemMessage).ToList();

  private AreaOfInstruction MapAreaOfInstruction(ContractMetadatas.AreaOfInstruction source) => new(
    source.Id,
    source.Name,
    ParseProgramTypes(source.ProgramTypes),
    source.MinimumHours,
    source.DisplayOrder,
    source.ParentAreaOfInstructionId);

  private SystemMessage MapSystemMessage(ContractMetadatas.SystemMessage source) => new(source.Name, source.Subject, source.Message)
  {
    StartDate = source.StartDate,
    EndDate = source.EndDate,
    PortalTags = source.PortalTags.Select(MapPortalTag).ToArray(),
  };

  private partial Province MapProvince(ContractMetadatas.Province source);

  private partial Country MapCountry(ContractMetadatas.Country source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PortalTags MapPortalTag(ContractMetadatas.PortalTags source);

  private static ProgramTypes[] ParseProgramTypes(IEnumerable<string>? programTypes)
  {
    if (programTypes == null)
    {
      return [];
    }

    return programTypes
      .Select(type => Enum.TryParse<ProgramTypes>(type, true, out var parsed) ? (ProgramTypes?)parsed : null)
      .Where(parsed => parsed.HasValue)
      .Select(parsed => parsed!.Value)
      .ToArray();
  }
}
