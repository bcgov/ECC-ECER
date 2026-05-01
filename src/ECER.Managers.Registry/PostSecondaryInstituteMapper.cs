using Riok.Mapperly.Abstractions;
using ContractPostSecondaryInstitutes = ECER.Managers.Registry.Contract.PostSecondaryInstitutes;
using ResourcePostSecondaryInstitutes = ECER.Resources.Documents.PostSecondaryInstitutes;

namespace ECER.Managers.Registry;

public interface IPostSecondaryInstituteMapper
{
  IEnumerable<ContractPostSecondaryInstitutes.PostSecondaryInstitute> MapPostSecondaryInstitutions(IEnumerable<ResourcePostSecondaryInstitutes.PostSecondaryInstitute> source);
  ResourcePostSecondaryInstitutes.PostSecondaryInstitute MapPostSecondaryInstitute(ContractPostSecondaryInstitutes.PostSecondaryInstitute source);
  ResourcePostSecondaryInstitutes.Campus MapCampus(ContractPostSecondaryInstitutes.Campus source);
}

[Mapper]
internal partial class PostSecondaryInstituteMapper : IPostSecondaryInstituteMapper
{
  public IEnumerable<ContractPostSecondaryInstitutes.PostSecondaryInstitute> MapPostSecondaryInstitutions(IEnumerable<ResourcePostSecondaryInstitutes.PostSecondaryInstitute> source) => source.Select(MapPostSecondaryInstitute).ToList();

  public ResourcePostSecondaryInstitutes.PostSecondaryInstitute MapPostSecondaryInstitute(ContractPostSecondaryInstitutes.PostSecondaryInstitute source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    InstitutionType = MapInstitutionType(source.InstitutionType),
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

  public ResourcePostSecondaryInstitutes.Campus MapCampus(ContractPostSecondaryInstitutes.Campus source) => new()
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

  private ContractPostSecondaryInstitutes.PostSecondaryInstitute MapPostSecondaryInstitute(ResourcePostSecondaryInstitutes.PostSecondaryInstitute source) => new()
  {
    Id = source.Id,
    Name = source.Name,
    InstitutionType = MapInstitutionType(source.InstitutionType),
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

  private ContractPostSecondaryInstitutes.Campus MapCampus(ResourcePostSecondaryInstitutes.Campus source) => new()
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
  private partial ResourcePostSecondaryInstitutes.CampusStatus MapCampusStatus(ContractPostSecondaryInstitutes.CampusStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPostSecondaryInstitutes.CampusStatus MapCampusStatus(ResourcePostSecondaryInstitutes.CampusStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePostSecondaryInstitutes.PsiInstitutionType MapInstitutionType(ContractPostSecondaryInstitutes.PsiInstitutionType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPostSecondaryInstitutes.PsiInstitutionType MapInstitutionType(ResourcePostSecondaryInstitutes.PsiInstitutionType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePostSecondaryInstitutes.PrivateAuspiceType MapPrivateAuspiceType(ContractPostSecondaryInstitutes.PrivateAuspiceType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPostSecondaryInstitutes.PrivateAuspiceType MapPrivateAuspiceType(ResourcePostSecondaryInstitutes.PrivateAuspiceType source);

  private ResourcePostSecondaryInstitutes.CampusStatus? MapCampusStatus(ContractPostSecondaryInstitutes.CampusStatus? source) => source.HasValue ? MapCampusStatus(source.Value) : null;

  private ContractPostSecondaryInstitutes.CampusStatus? MapCampusStatus(ResourcePostSecondaryInstitutes.CampusStatus? source) => source.HasValue ? MapCampusStatus(source.Value) : null;

  private ResourcePostSecondaryInstitutes.PsiInstitutionType? MapInstitutionType(ContractPostSecondaryInstitutes.PsiInstitutionType? source) => source.HasValue ? MapInstitutionType(source.Value) : null;

  private ContractPostSecondaryInstitutes.PsiInstitutionType? MapInstitutionType(ResourcePostSecondaryInstitutes.PsiInstitutionType? source) => source.HasValue ? MapInstitutionType(source.Value) : null;

  private ResourcePostSecondaryInstitutes.PrivateAuspiceType? MapPrivateAuspiceType(ContractPostSecondaryInstitutes.PrivateAuspiceType? source) => source.HasValue ? MapPrivateAuspiceType(source.Value) : null;

  private ContractPostSecondaryInstitutes.PrivateAuspiceType? MapPrivateAuspiceType(ResourcePostSecondaryInstitutes.PrivateAuspiceType? source) => source.HasValue ? MapPrivateAuspiceType(source.Value) : null;
}
