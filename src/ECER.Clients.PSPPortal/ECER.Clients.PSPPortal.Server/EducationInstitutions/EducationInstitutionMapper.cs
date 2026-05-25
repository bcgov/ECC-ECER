using Riok.Mapperly.Abstractions;
using ContractInstitutions = ECER.Managers.Registry.Contract.PostSecondaryInstitutes;

namespace ECER.Clients.PSPPortal.Server.EducationInstitutions;

internal interface IEducationInstitutionMapper
{
  EducationInstitution MapEducationInstitution(ContractInstitutions.PostSecondaryInstitute source);
  ContractInstitutions.PostSecondaryInstitute MapEducationInstitution(EducationInstitution source);
  ContractInstitutions.Campus MapCreateCampusRequest(CreateCampusRequest source);
  ContractInstitutions.Campus MapUpdateCampusRequest(UpdateCampusRequest source);
}

[Mapper]
internal partial class EducationInstitutionMapper : IEducationInstitutionMapper
{
  public EducationInstitution MapEducationInstitution(ContractInstitutions.PostSecondaryInstitute source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    InstitutionType = MapPsiInstitutionType(source.InstitutionType),
    PrivateAuspiceType = MapPrivateAuspiceType(source.PrivateAuspiceType),
    PtiruInstitutionId = source.PtiruInstitutionId,
    WebsiteUrl = source.WebsiteUrl,
    Street1 = source.Street1,
    Street2 = source.Street2,
    Street3 = source.Street3,
    City = source.City,
    Province = source.Province,
    Country = source.Country,
    PostalCode = source.PostalCode,
    Campuses = source.Campuses?.Select(MapCampus).ToList(),
  };

  public ContractInstitutions.PostSecondaryInstitute MapEducationInstitution(EducationInstitution source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    InstitutionType = MapPsiInstitutionType(source.InstitutionType),
    PrivateAuspiceType = MapPrivateAuspiceType(source.PrivateAuspiceType),
    PtiruInstitutionId = source.PtiruInstitutionId,
    WebsiteUrl = source.WebsiteUrl,
    Street1 = source.Street1,
    Street2 = source.Street2,
    Street3 = source.Street3,
    City = source.City,
    Province = source.Province,
    Country = source.Country,
    PostalCode = source.PostalCode,
    Campuses = source.Campuses?.Select(MapCampus).ToList(),
  };

  public ContractInstitutions.Campus MapCreateCampusRequest(CreateCampusRequest source) => new()
  {
    Name = source.Name,
    IsSatelliteOrTemporaryLocation = source.IsSatelliteOrTemporaryLocation,
    Street1 = source.Street1,
    Street2 = source.Street2,
    Street3 = source.Street3,
    City = source.City,
    Province = source.Province,
    PostalCode = source.PostalCode,
    KeyCampusContactId = source.KeyCampusContactId,
    OtherCampusContactName = source.OtherCampusContactName,
  };

  public ContractInstitutions.Campus MapUpdateCampusRequest(UpdateCampusRequest source) => new()
  {
    Name = source.Name,
    Street1 = source.Street1,
    Street2 = source.Street2,
    Street3 = source.Street3,
    City = source.City,
    Province = source.Province,
    PostalCode = source.PostalCode,
    KeyCampusContactId = source.KeyCampusContactId,
    OtherCampusContactName = source.OtherCampusContactName,
  };

  private Campus MapCampus(ContractInstitutions.Campus source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    GeneratedName = source.GeneratedName,
    Status = MapCampusStatus(source.Status),
    IsSatelliteOrTemporaryLocation = source.IsSatelliteOrTemporaryLocation,
    Street1 = source.Street1,
    Street2 = source.Street2,
    Street3 = source.Street3,
    City = source.City,
    Province = source.Province,
    PostalCode = source.PostalCode,
    KeyCampusContactId = source.KeyCampusContactId,
    KeyCampusContactName = source.KeyCampusContactName,
    OtherCampusContactName = source.OtherCampusContactName,
  };

  private ContractInstitutions.Campus MapCampus(Campus source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    GeneratedName = source.GeneratedName,
    Status = MapCampusStatus(source.Status),
    IsSatelliteOrTemporaryLocation = source.IsSatelliteOrTemporaryLocation,
    Street1 = source.Street1,
    Street2 = source.Street2,
    Street3 = source.Street3,
    City = source.City,
    Province = source.Province,
    PostalCode = source.PostalCode,
    KeyCampusContactId = source.KeyCampusContactId,
    KeyCampusContactName = source.KeyCampusContactName,
    OtherCampusContactName = source.OtherCampusContactName,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PsiInstitutionType MapPsiInstitutionType(ContractInstitutions.PsiInstitutionType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractInstitutions.PsiInstitutionType MapPsiInstitutionType(PsiInstitutionType source);

  private PsiInstitutionType? MapPsiInstitutionType(ContractInstitutions.PsiInstitutionType? source) => source.HasValue ? MapPsiInstitutionType(source.Value) : null;

  private ContractInstitutions.PsiInstitutionType? MapPsiInstitutionType(PsiInstitutionType? source) => source.HasValue ? MapPsiInstitutionType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PrivateAuspiceType MapPrivateAuspiceType(ContractInstitutions.PrivateAuspiceType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractInstitutions.PrivateAuspiceType MapPrivateAuspiceType(PrivateAuspiceType source);

  private PrivateAuspiceType? MapPrivateAuspiceType(ContractInstitutions.PrivateAuspiceType? source) => source.HasValue ? MapPrivateAuspiceType(source.Value) : null;

  private ContractInstitutions.PrivateAuspiceType? MapPrivateAuspiceType(PrivateAuspiceType? source) => source.HasValue ? MapPrivateAuspiceType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CampusStatus MapCampusStatus(ContractInstitutions.CampusStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractInstitutions.CampusStatus MapCampusStatus(CampusStatus source);

  private CampusStatus? MapCampusStatus(ContractInstitutions.CampusStatus? source) => source.HasValue ? MapCampusStatus(source.Value) : null;

  private ContractInstitutions.CampusStatus? MapCampusStatus(CampusStatus? source) => source.HasValue ? MapCampusStatus(source.Value) : null;
}
