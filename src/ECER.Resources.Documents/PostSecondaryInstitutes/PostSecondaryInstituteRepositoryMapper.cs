using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.PostSecondaryInstitutes;

internal sealed class PostSecondaryInstituteRepositoryMapper : SecureProfile
{
  public PostSecondaryInstituteRepositoryMapper()
  {
    CreateMap<ecer_PostSecondaryInstitute, PostSecondaryInstitute>()

      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_PostSecondaryInstituteId))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.InstitutionType, opts => opts.MapFrom(s => s.ecer_PSIInstitutionType))
      .ForMember(d => d.PrivateAuspiceType, opts => opts.MapFrom(s => s.ecer_PrivateAuspiceType))
      .ForMember(d => d.PtiruInstitutionId, opts => opts.MapFrom(s => s.ecer_PTIRUInstitutionID))
      .ForMember(d => d.WebsiteUrl, opts => opts.MapFrom(s => s.ecer_Website))
      .ForMember(d => d.Street1, opts => opts.MapFrom(s => s.ecer_Street1))
      .ForMember(d => d.Street2, opts => opts.MapFrom(s => s.ecer_Street2))
      .ForMember(d => d.Street3, opts => opts.MapFrom(s => s.ecer_Street3))
      .ForMember(d => d.City, opts => opts.MapFrom(s => s.ecer_City))
      .ForMember(d => d.Province, opts => opts.MapFrom(s => s.ecer_ProvinceIdName))
      .ForMember(d => d.Country, opts => opts.MapFrom(s => s.ecer_Country))
      .ForMember(d => d.PostalCode, opts => opts.MapFrom(s => s.ecer_ZipPostalCode))
      .ForMember(d => d.BceidBusinessId, opts => opts.MapFrom(s => s.ecer_BusinessBCeID))
      .ForMember(d => d.BceidBusinessName, opts => opts.MapFrom(s => s.ecer_BCeIDBusinessName))
      .ForMember(d => d.Campuses, opts => opts.MapFrom(s => s.ecer_postsecondaryinstitutecampus_postsecondaryinstitute_ecer_postsecondaryinstitute))
      .ReverseMap()
      .ForSourceMember(s => s.Campuses, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.InstitutionType, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PrivateAuspiceType, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PtiruInstitutionId, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_PostSecondaryInstituteId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.ecer_BusinessBCeID, opts => opts.MapFrom(s => s.BceidBusinessId))
      .ForMember(d => d.ecer_BCeIDBusinessName, opts => opts.MapFrom(s => s.BceidBusinessName))
      .ForMember(d => d.ecer_ProvinceIdName, opts => opts.MapFrom(s => s.Province))
      .ValidateMemberList(MemberList.Source);
    
    CreateMap<ecer_PostSecondaryInstituteCampus, Campus>()
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
      .ForMember(d => d.IsSatelliteOrTemporaryLocation, opts => opts.MapFrom(s => s.ecer_SatelliteorTemporaryLocation.HasValue ? s.ecer_SatelliteorTemporaryLocation.Equals(ecer_YesNoNull.Yes) : default(bool?)))
      .ForMember(d => d.Street1, opts => opts.MapFrom(s => s.ecer_street1))
      .ForMember(d => d.Street2, opts => opts.MapFrom(s => s.ecer_street2))
      .ForMember(d => d.Street3, opts => opts.MapFrom(s => s.ecer_street3))
      .ForMember(d => d.City, opts => opts.MapFrom(s => s.ecer_city))
      .ForMember(d => d.Province, opts => opts.MapFrom(s => s.ecer_provinceName))
      .ForMember(d => d.PostalCode, opts => opts.MapFrom(s => s.ecer_postalcode))
      .ForMember(d => d.KeyCampusContactName, opts => opts.MapFrom(s => s.ecer_KeyCampusContactName))
      .ValidateMemberList(MemberList.Destination);
  }
}
