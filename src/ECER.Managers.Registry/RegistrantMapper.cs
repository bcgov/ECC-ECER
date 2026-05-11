using Riok.Mapperly.Abstractions;
using ContractRegistrants = ECER.Managers.Registry.Contract.Registrants;
using ResourceRegistrants = ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

public interface IRegistrantMapper
{
  IEnumerable<ContractRegistrants.Registrant> MapRegistrants(IEnumerable<ResourceRegistrants.Registrant> source);
  ResourceRegistrants.UserProfile MapUserProfile(ContractRegistrants.UserProfile source);
  IEnumerable<ResourceRegistrants.IdentityDocument> MapIdentityDocuments(IEnumerable<ContractRegistrants.IdentityDocument> source);
  ResourceRegistrants.Registrant MapRegisteredRegistrant(ContractRegistrants.RegisterNewUserCommand source);
  ResourceRegistrants.Address MapAddress(ContractRegistrants.Address source);
}

[Mapper]
internal partial class RegistrantMapper : IRegistrantMapper
{
  public IEnumerable<ContractRegistrants.Registrant> MapRegistrants(IEnumerable<ResourceRegistrants.Registrant> source) => source.Select(MapRegistrant).ToList();

  public ResourceRegistrants.UserProfile MapUserProfile(ContractRegistrants.UserProfile source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    AlternateContactPhone = source.AlternateContactPhone,
    DateOfBirth = source.DateOfBirth,
    RegistrationNumber = source.RegistrationNumber,
    Email = source.Email,
    Phone = source.Phone,
    ResidentialAddress = source.ResidentialAddress == null ? null : MapAddress(source.ResidentialAddress),
    MailingAddress = source.MailingAddress == null ? null : MapAddress(source.MailingAddress),
    IsVerified = source.IsVerified,
    PreviousNames = source.PreviousNames.Select(MapPreviousName).ToList(),
    IsRegistrant = source.IsRegistrant,
    Status = MapStatusCode(source.Status),
  };

  public IEnumerable<ResourceRegistrants.IdentityDocument> MapIdentityDocuments(IEnumerable<ContractRegistrants.IdentityDocument> source) => source.Select(MapIdentityDocument).ToList();

  public ResourceRegistrants.Registrant MapRegisteredRegistrant(ContractRegistrants.RegisterNewUserCommand source) => new()
  {
    Identities = [source.Identity],
    Profile = MapUserProfile(source.Profile),
  };

  public ResourceRegistrants.Address MapAddress(ContractRegistrants.Address source) => new(
    source.Line1,
    source.Line2,
    source.City,
    source.PostalCode,
    source.Province,
    source.Country);

  private ContractRegistrants.Registrant MapRegistrant(ResourceRegistrants.Registrant source) => new(source.Id, MapUserProfile(source.Profile));

  private ContractRegistrants.UserProfile MapUserProfile(ResourceRegistrants.UserProfile source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    AlternateContactPhone = source.AlternateContactPhone,
    DateOfBirth = source.DateOfBirth,
    RegistrationNumber = source.RegistrationNumber,
    Email = source.Email,
    Phone = source.Phone,
    ResidentialAddress = source.ResidentialAddress == null ? null : MapAddress(source.ResidentialAddress),
    MailingAddress = source.MailingAddress == null ? null : MapAddress(source.MailingAddress),
    IsVerified = source.IsVerified,
    PreviousNames = source.PreviousNames.Select(MapPreviousName).ToList(),
    IsRegistrant = source.IsRegistrant,
    Status = MapStatusCode(source.Status),
  };

  private ResourceRegistrants.PreviousName MapPreviousName(ContractRegistrants.PreviousName source) => new(source.FirstName, source.LastName)
  {
    Id = source.Id,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    Status = MapPreviousNameStage(source.Status),
    Source = MapPreviousNameSource(source.Source),
    Documents = source.Documents.Select(MapIdentityDocument).ToList(),
  };

  private ContractRegistrants.PreviousName MapPreviousName(ResourceRegistrants.PreviousName source) => new(source.FirstName, source.LastName)
  {
    Id = source.Id,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    Status = MapPreviousNameStage(source.Status),
    Source = MapPreviousNameSource(source.Source),
    Documents = source.Documents.Select(MapIdentityDocument).ToList(),
  };

  private static ResourceRegistrants.IdentityDocument MapIdentityDocument(ContractRegistrants.IdentityDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static ContractRegistrants.IdentityDocument MapIdentityDocument(ResourceRegistrants.IdentityDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static ContractRegistrants.Address MapAddress(ResourceRegistrants.Address source) => new(
    source.Line1,
    source.Line2 ?? string.Empty,
    source.City,
    source.PostalCode,
    source.Province,
    source.Country);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceRegistrants.StatusCode MapStatusCode(ContractRegistrants.StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractRegistrants.StatusCode MapStatusCode(ResourceRegistrants.StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceRegistrants.PreviousNameStage MapPreviousNameStage(ContractRegistrants.PreviousNameStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractRegistrants.PreviousNameStage MapPreviousNameStage(ResourceRegistrants.PreviousNameStage source);

  private ResourceRegistrants.PreviousNameStage? MapPreviousNameStage(ContractRegistrants.PreviousNameStage? source) => source.HasValue ? MapPreviousNameStage(source.Value) : null;

  private ContractRegistrants.PreviousNameStage? MapPreviousNameStage(ResourceRegistrants.PreviousNameStage? source) => source.HasValue ? MapPreviousNameStage(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceRegistrants.PreviousNameSources MapPreviousNameSource(ContractRegistrants.PreviousNameSources source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractRegistrants.PreviousNameSources MapPreviousNameSource(ResourceRegistrants.PreviousNameSources source);

  private ResourceRegistrants.PreviousNameSources? MapPreviousNameSource(ContractRegistrants.PreviousNameSources? source) => source.HasValue ? MapPreviousNameSource(source.Value) : null;

  private ContractRegistrants.PreviousNameSources? MapPreviousNameSource(ResourceRegistrants.PreviousNameSources? source) => source.HasValue ? MapPreviousNameSource(source.Value) : null;
}
