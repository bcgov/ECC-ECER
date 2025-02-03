using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.MetadataResources;

internal class MetadataResourceRepositoryMapper : Profile
{
  public MetadataResourceRepositoryMapper()
  {
    CreateMap<ecer_Province, Province>(MemberList.Source)
    .ForCtorParam(nameof(Province.ProvinceId), opt => opt.MapFrom(src => src.ecer_ProvinceId))
    .ForCtorParam(nameof(Province.ProvinceName), opt => opt.MapFrom(src => src.ecer_Name))
    .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_SystemMessage, SystemMessage>(MemberList.Source)
    .ForCtorParam(nameof(SystemMessage.Message), opt => opt.MapFrom(src => src.ecer_message))
    .ForCtorParam(nameof(SystemMessage.Subject), opt => opt.MapFrom(src => src.ecer_subject))
    .ForCtorParam(nameof(SystemMessage.Name), opt => opt.MapFrom(src => src.ecer_name))
    .ForCtorParam(nameof(SystemMessage.PortalTag), opt => opt.MapFrom(src => src.ecer_portaltag))
    .ForMember(d => d.StartDate, opts => opts.MapFrom(s => s.ecer_startdate))
    .ForMember(d => d.EndDate, opts => opts.MapFrom(s => s.ecer_enddate))
    .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_Country, Country>(MemberList.Source)
    .ForCtorParam(nameof(Country.CountryId), opt => opt.MapFrom(src => src.ecer_CountryId))
    .ForCtorParam(nameof(Country.CountryName), opt => opt.MapFrom(src => src.ecer_Name))
    .ForCtorParam(nameof(Country.CountryCode), opt => opt.MapFrom(src => src.ecer_ShortName))
    .ValidateMemberList(MemberList.Destination);
  }
}
