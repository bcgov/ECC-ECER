using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.PostSecondaryInstitutes;

internal sealed class PostSecondaryInstituteRepositoryMapper : Profile
{
  public PostSecondaryInstituteRepositoryMapper()
  {
    CreateMap<ecer_PostSecondaryInstitute, PostSecondaryInstitute>()
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_PostSecondaryInstituteId))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.Auspice, opts => opts.MapFrom(s => s.ecer_PSIInstitutionType))
      .ForMember(d => d.WebsiteUrl, opts => opts.MapFrom(s => s.ecer_Website))
      .ForMember(d => d.Street1, opts => opts.MapFrom(s => s.ecer_Street1))
      .ForMember(d => d.Street2, opts => opts.MapFrom(s => s.ecer_Street2))
      .ForMember(d => d.Street3, opts => opts.MapFrom(s => s.ecer_Street3))
      .ForMember(d => d.City, opts => opts.MapFrom(s => s.ecer_City))
      .ForMember(d => d.Province, opts => opts.MapFrom(s => s.ecer_ProvinceIdName))
      .ForMember(d => d.Country, opts => opts.MapFrom(s => s.ecer_Country))
      .ForMember(d => d.PostalCode, opts => opts.MapFrom(s => s.ecer_ZipPostalCode))
      .ForMember(d => d.BceidBusinessId, opts => opts.MapFrom(s => s.ecer_BusinessBCeID))
      .ReverseMap()
      .ForMember(d => d.ecer_PostSecondaryInstituteId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.ecer_BusinessBCeID, opts => opts.MapFrom(s => s.BceidBusinessId))
      .ForMember(d => d.ecer_ProvinceIdName, opts => opts.MapFrom(s => s.Province))
      .ValidateMemberList(MemberList.Source);
  }
}
