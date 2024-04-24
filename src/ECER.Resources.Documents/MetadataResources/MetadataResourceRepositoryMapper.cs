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
  }
}
