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
}

[Mapper]
internal partial class ConfigurationMapper : IConfigurationMapper
{
  public IEnumerable<Province> MapProvinces(IEnumerable<ContractMetadatas.Province> source) => source.Select(MapProvince).ToList();

  public IEnumerable<Country> MapCountries(IEnumerable<ContractMetadatas.Country> source) => source.Select(MapCountry).ToList();

  public IEnumerable<AreaOfInstruction> MapAreaOfInstructions(IEnumerable<ContractMetadatas.AreaOfInstruction> source) => source.Select(MapAreaOfInstruction).ToList();

  private AreaOfInstruction MapAreaOfInstruction(ContractMetadatas.AreaOfInstruction source) => new(
    source.Id,
    source.Name,
    ParseProgramTypes(source.ProgramTypes),
    source.MinimumHours,
    source.DisplayOrder,
    source.ParentAreaOfInstructionId);

  private partial Province MapProvince(ContractMetadatas.Province source);

  private partial Country MapCountry(ContractMetadatas.Country source);

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
