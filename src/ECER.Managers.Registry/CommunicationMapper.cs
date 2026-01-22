using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Resources.Accounts.Communications;

namespace ECER.Managers.Registry;

internal class CommunicationMapper : Profile
{
  public CommunicationMapper()
  {
    CreateMap<Communication, Contract.Communications.Communication>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? string.Empty))
        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Body))
        .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.Documents))
        .ReverseMap();

    CreateMap<CommunicationCategory, Contract.Communications.CommunicationCategory>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<CommunicationsStatus, Contract.Communications.CommunicationsStatus>();

    CreateMap<CommunicationDocument, Contract.Communications.CommunicationDocument>()
    .ForCtorParam(nameof(CommunicationDocument.Id), opts => opts.MapFrom(s => s.Id))
    .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url ?? string.Empty))
    .ForMember(dest => dest.Extention, opt => opt.MapFrom(src => src.Extention ?? string.Empty))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
    .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size ?? string.Empty))
    .ReverseMap();
  }
}
