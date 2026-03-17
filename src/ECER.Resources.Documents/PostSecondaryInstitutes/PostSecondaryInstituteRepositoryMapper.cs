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
      .ForMember(d => d.ecer_postsecondaryinstitutecampus_postsecondaryinstitute_ecer_postsecondaryinstitute, opts => opts.Ignore())
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
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_campus))
      .ForMember(d => d.GeneratedName, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
      .ForMember(d => d.IsSatelliteOrTemporaryLocation, opts => opts.MapFrom(s => MapSatelliteOrTemporaryLocation(s.ecer_SatelliteorTemporaryLocation)))
      .ForMember(d => d.Street1, opts => opts.MapFrom(s => s.ecer_street1))
      .ForMember(d => d.Street2, opts => opts.MapFrom(s => s.ecer_street2))
      .ForMember(d => d.Street3, opts => opts.MapFrom(s => s.ecer_street3))
      .ForMember(d => d.City, opts => opts.MapFrom(s => s.ecer_city))
      .ForMember(d => d.Province, opts => opts.MapFrom(s => s.ecer_provinceName))
      .ForMember(d => d.PostalCode, opts => opts.MapFrom(s => s.ecer_postalcode))
      .ForMember(d => d.KeyCampusContactId, opts => opts.MapFrom(s => s.ecer_KeyCampusContact != null ? s.ecer_KeyCampusContact.Id.ToString() : null))
      .ForMember(d => d.KeyCampusContactName, opts => opts.MapFrom(s => s.ecer_KeyCampusContactName))
      .ForMember(d => d.OtherCampusContactName, opts => opts.MapFrom(s => s.ecer_OtherContactName))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<Campus, ecer_PostSecondaryInstituteCampus>()
      .ForMember(d => d.ecer_PostSecondaryInstituteCampusId,
        opts => opts.MapFrom(s => s.Id != null ? Guid.Parse(s.Id) : (Guid?)null))
      .ForMember(d => d.ecer_campus, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.ecer_street1, opts => opts.MapFrom(s => s.Street1))
      .ForMember(d => d.ecer_street2, opts => opts.MapFrom(s => s.Street2))
      .ForMember(d => d.ecer_street3, opts => opts.MapFrom(s => s.Street3))
      .ForMember(d => d.ecer_city, opts => opts.MapFrom(s => s.City))
      .ForMember(d => d.ecer_postalcode, opts => opts.MapFrom(s => s.PostalCode))
      .ForMember(d => d.ecer_SatelliteorTemporaryLocation, opts => opts.MapFrom(s => MapSatelliteOrTemporaryLocationEnum(s.IsSatelliteOrTemporaryLocation)))
      .ForMember(d => d.ecer_KeyCampusContact, opts => opts.MapFrom(s => MapKeyCampusContact(s.KeyCampusContactId)))
      .ForMember(d => d.ecer_OtherContactName, opts => opts.MapFrom(s => s.OtherCampusContactName))
      .ForSourceMember(s => s.GeneratedName, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Status, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.KeyCampusContactName, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Province, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.IsSatelliteOrTemporaryLocation, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.KeyCampusContactId, opts => opts.DoNotValidate())
      .ValidateMemberList(MemberList.Source);
  }

  private static bool? MapSatelliteOrTemporaryLocation(ecer_YesNoNull? value)
  {
    if (!value.HasValue) return null;
    return value.Equals(ecer_YesNoNull.Yes);
  }

  private static ecer_YesNoNull? MapSatelliteOrTemporaryLocationEnum(bool? value)
  {
    if (!value.HasValue) return null;
    return value.Value ? ecer_YesNoNull.Yes : ecer_YesNoNull.No;
  }

  private static Microsoft.Xrm.Sdk.EntityReference? MapKeyCampusContact(string? contactId)
  {
    if (contactId == null) return null;
    return new Microsoft.Xrm.Sdk.EntityReference(ecer_ECEProgramRepresentative.EntityLogicalName, Guid.Parse(contactId));
  }
}
