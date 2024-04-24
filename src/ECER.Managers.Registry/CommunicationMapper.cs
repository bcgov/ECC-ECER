using AutoMapper;
using ECER.Resources.Accounts.Communications;

namespace ECER.Managers.Registry;

internal class CommunicationMapper : Profile
{
  public CommunicationMapper()
  {
    CreateMap<Communication, Contract.Communications.Communication>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? string.Empty))
        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Body));

    CreateMap<CommunicationsStatus, Contract.Communications.CommunicationsStatus>();
  }
}
