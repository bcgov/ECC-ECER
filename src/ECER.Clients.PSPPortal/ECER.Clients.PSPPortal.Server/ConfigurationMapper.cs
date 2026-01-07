using ECER.Managers.Admin.Contract.Metadatas;
using AutoMapper;
using ECER.Clients.PSPPortal.Server.EducationInstitutions;
using ECER.Clients.PSPPortal.Server.Programs;
using System;
using System.Linq;

namespace ECER.Clients.PSPPortal.Server;

public class ConfigurationMapper : Profile
{
  public ConfigurationMapper()
  {
    CreateMap<Managers.Admin.Contract.Metadatas.Province, Province>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.Country, Country>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.AreaOfInstruction, AreaOfInstruction>()
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => ParseProgramTypes(s.ProgramTypes)));
    CreateMap<AreaOfInstruction, Managers.Admin.Contract.Metadatas.AreaOfInstruction>()
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ProgramTypes != null ? s.ProgramTypes.Select(type => type.ToString()) : Array.Empty<string>()));
  }

  private static ProgramTypes[] ParseProgramTypes(IEnumerable<string>? programTypes)
  {
    if (programTypes == null)
    {
      return Array.Empty<ProgramTypes>();
    }

    return programTypes
      .Select(type => Enum.TryParse<ProgramTypes>(type, true, out var parsed) ? (ProgramTypes?)parsed : null)
      .Where(parsed => parsed.HasValue)
      .Select(parsed => parsed!.Value)
      .ToArray();
  }
}
