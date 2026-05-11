using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.PostSecondaryInstitutes;

internal interface IPostSecondaryInstituteRepositoryMapper
{
  List<PostSecondaryInstitute> MapPostSecondaryInstitutes(IEnumerable<ecer_PostSecondaryInstitute> source);
  ecer_PostSecondaryInstitute MapPostSecondaryInstitute(PostSecondaryInstitute source);
  ecer_PostSecondaryInstituteCampus MapCampus(Campus source);
}

[Mapper]
internal partial class PostSecondaryInstituteRepositoryMapper : IPostSecondaryInstituteRepositoryMapper
{
  public List<PostSecondaryInstitute> MapPostSecondaryInstitutes(IEnumerable<ecer_PostSecondaryInstitute> source) => source.Select(MapPostSecondaryInstitute).ToList();

  public ecer_PostSecondaryInstitute MapPostSecondaryInstitute(PostSecondaryInstitute source) => new()
  {
    ecer_PostSecondaryInstituteId = Guid.Parse(source.Id),
    ecer_Name = source.Name,
    ecer_PSIInstitutionType = MapInstitutionType(source.InstitutionType),
    ecer_PrivateAuspiceType = MapPrivateAuspiceType(source.PrivateAuspiceType),
    ecer_PTIRUInstitutionID = source.PtiruInstitutionId,
    ecer_Website = source.WebsiteUrl,
    ecer_Street1 = source.Street1,
    ecer_Street2 = source.Street2,
    ecer_Street3 = source.Street3,
    ecer_City = source.City,
    ecer_Country = source.Country,
    ecer_ZipPostalCode = source.PostalCode,
    ecer_BusinessBCeID = source.BceidBusinessId,
    ecer_BCeIDBusinessName = source.BceidBusinessName,
  };

  public ecer_PostSecondaryInstituteCampus MapCampus(Campus source) => new()
  {
    ecer_PostSecondaryInstituteCampusId = source.Id != null ? Guid.Parse(source.Id) : null,
    ecer_campus = source.Name,
    ecer_street1 = source.Street1,
    ecer_street2 = source.Street2,
    ecer_street3 = source.Street3,
    ecer_city = source.City,
    ecer_postalcode = source.PostalCode,
    ecer_SatelliteorTemporaryLocation = MapSatelliteOrTemporaryLocationEnum(source.IsSatelliteOrTemporaryLocation),
    ecer_KeyCampusContact = MapKeyCampusContact(source.KeyCampusContactId),
    ecer_OtherContactName = source.OtherCampusContactName,
  };

  private PostSecondaryInstitute MapPostSecondaryInstitute(ecer_PostSecondaryInstitute source) => new()
  {
    Id = source.ecer_PostSecondaryInstituteId?.ToString() ?? string.Empty,
    Name = source.ecer_Name,
    InstitutionType = MapInstitutionType(source.ecer_PSIInstitutionType),
    PrivateAuspiceType = MapPrivateAuspiceType(source.ecer_PrivateAuspiceType),
    PtiruInstitutionId = source.ecer_PTIRUInstitutionID,
    WebsiteUrl = source.ecer_Website,
    Street1 = source.ecer_Street1,
    Street2 = source.ecer_Street2,
    Street3 = source.ecer_Street3,
    City = source.ecer_City,
    Province = source.ecer_ProvinceIdName,
    Country = source.ecer_Country,
    PostalCode = source.ecer_ZipPostalCode,
    BceidBusinessId = source.ecer_BusinessBCeID,
    BceidBusinessName = source.ecer_BCeIDBusinessName,
    Campuses = (source.ecer_postsecondaryinstitutecampus_postsecondaryinstitute_ecer_postsecondaryinstitute ?? Array.Empty<ecer_PostSecondaryInstituteCampus>())
      .Select(MapCampus)
      .ToList(),
  };

  private Campus MapCampus(ecer_PostSecondaryInstituteCampus source) => new()
  {
    Id = source.Id.ToString(),
    Name = source.ecer_campus,
    GeneratedName = source.ecer_Name,
    Status = MapCampusStatus(source.StatusCode),
    IsSatelliteOrTemporaryLocation = MapSatelliteOrTemporaryLocation(source.ecer_SatelliteorTemporaryLocation),
    Street1 = source.ecer_street1,
    Street2 = source.ecer_street2,
    Street3 = source.ecer_street3,
    City = source.ecer_city,
    Province = source.ecer_provinceName,
    PostalCode = source.ecer_postalcode,
    KeyCampusContactId = source.ecer_KeyCampusContact?.Id.ToString(),
    KeyCampusContactName = source.ecer_KeyCampusContactName,
    OtherCampusContactName = source.ecer_OtherContactName,
  };

  private static ecer_psiinstitutiontype? MapInstitutionType(PsiInstitutionType? source) => source switch
  {
    null => null,
    PsiInstitutionType.Private => ecer_psiinstitutiontype.Private,
    PsiInstitutionType.Public => ecer_psiinstitutiontype.Public,
    PsiInstitutionType.ContinuingEducation => ecer_psiinstitutiontype.ContinuingEducation,
    PsiInstitutionType.PublicOOP => ecer_psiinstitutiontype.Publicoutofprovince,
    _ => null,
  };

  private static PsiInstitutionType? MapInstitutionType(ecer_psiinstitutiontype? source) => source switch
  {
    null => null,
    ecer_psiinstitutiontype.Private => PsiInstitutionType.Private,
    ecer_psiinstitutiontype.Public => PsiInstitutionType.Public,
    ecer_psiinstitutiontype.ContinuingEducation => PsiInstitutionType.ContinuingEducation,
    ecer_psiinstitutiontype.Publicoutofprovince => PsiInstitutionType.PublicOOP,
    _ => null,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PrivateAuspiceType MapPrivateAuspiceType(PrivateAuspiceType source);

  private ecer_PrivateAuspiceType? MapPrivateAuspiceType(PrivateAuspiceType? source) => source.HasValue ? MapPrivateAuspiceType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PrivateAuspiceType MapPrivateAuspiceType(ecer_PrivateAuspiceType source);

  private PrivateAuspiceType? MapPrivateAuspiceType(ecer_PrivateAuspiceType? source) => source.HasValue ? MapPrivateAuspiceType(source.Value) : null;

  private static CampusStatus? MapCampusStatus(ecer_PostSecondaryInstituteCampus_StatusCode? source) => source switch
  {
    null => null,
    ecer_PostSecondaryInstituteCampus_StatusCode.Active => CampusStatus.Active,
    ecer_PostSecondaryInstituteCampus_StatusCode.Inactive => CampusStatus.Inactive,
    _ => null,
  };

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

  private static EntityReference? MapKeyCampusContact(string? contactId)
  {
    if (contactId == null) return null;
    return new EntityReference(ecer_ECEProgramRepresentative.EntityLogicalName, Guid.Parse(contactId));
  }

}
