using ECER.Managers.Admin.Contract.Metadatas;
using AutoMapper;
using ECER.Clients.PSPPortal.Server.EducationInstitutions;

namespace ECER.Clients.PSPPortal.Server;

public class ConfigurationMapper : Profile
{
  public ConfigurationMapper()
  {
    CreateMap<Managers.Admin.Contract.Metadatas.Province, Province>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.Country, Country>().ReverseMap();
    CreateMap<Managers.Admin.Contract.Metadatas.AreaOfInstruction, AreaOfInstruction>().ReverseMap();
  }
}
