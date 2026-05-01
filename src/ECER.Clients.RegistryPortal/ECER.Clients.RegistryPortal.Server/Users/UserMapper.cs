using ECER.Utilities.ObjectStorage.Providers;
using Riok.Mapperly.Abstractions;
using ContractRegistrants = ECER.Managers.Registry.Contract.Registrants;

namespace ECER.Clients.RegistryPortal.Server.Users;

internal interface IUserMapper
{
  UserInfo MapUserInfo(ContractRegistrants.UserProfile source);
  ContractRegistrants.UserProfile MapRegistrationUserProfile(UserInfo source);
  UserProfile MapUserProfile(ContractRegistrants.UserProfile source);
  ContractRegistrants.UserProfile MapUserProfile(UserProfile source);
  ContractRegistrants.ProfileIdentification MapProfileIdentification(ProfileIdentification source);
}

[Mapper]
internal partial class UserMapper : IUserMapper
{
  public UserInfo MapUserInfo(ContractRegistrants.UserProfile source) => new(
    source.LastName!,
    source.DateOfBirth!.Value,
    source.Email,
    source.Phone)
  {
    FirstName = source.FirstName,
    GivenName = source.GivenName,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    RegistrationNumber = source.RegistrationNumber,
    IsVerified = source.IsVerified,
    Status = MapStatusCode(source.Status),
    ResidentialAddress = source.ResidentialAddress == null ? null : MapAddress(source.ResidentialAddress),
    MailingAddress = source.MailingAddress == null ? null : MapAddress(source.MailingAddress),
    IsRegistrant = source.IsRegistrant,
  };

  public ContractRegistrants.UserProfile MapRegistrationUserProfile(UserInfo source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    MiddleName = source.MiddleName,
    GivenName = source.GivenName,
    PreferredName = source.PreferredName,
    AlternateContactPhone = null,
    DateOfBirth = source.DateOfBirth,
    RegistrationNumber = source.RegistrationNumber,
    Email = source.Email,
    Phone = source.Phone,
    ResidentialAddress = source.ResidentialAddress == null ? null : MapAddress(source.ResidentialAddress),
    MailingAddress = source.MailingAddress == null ? null : MapAddress(source.MailingAddress),
    IsVerified = source.IsVerified,
    PreviousNames = Array.Empty<ContractRegistrants.PreviousName>(),
    IsRegistrant = source.IsRegistrant,
  };

  public UserProfile MapUserProfile(ContractRegistrants.UserProfile source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    AlternateContactPhone = source.AlternateContactPhone,
    DateOfBirth = source.DateOfBirth,
    Email = source.Email,
    Phone = source.Phone,
    ResidentialAddress = source.ResidentialAddress == null ? null : MapAddress(source.ResidentialAddress),
    MailingAddress = source.MailingAddress == null ? null : MapAddress(source.MailingAddress),
    PreviousNames = source.PreviousNames.Select(MapPreviousName).ToList(),
  };

  public ContractRegistrants.UserProfile MapUserProfile(UserProfile source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    AlternateContactPhone = source.AlternateContactPhone,
    DateOfBirth = source.DateOfBirth,
    Email = source.Email,
    Phone = source.Phone,
    ResidentialAddress = source.ResidentialAddress == null ? null : MapAddress(source.ResidentialAddress),
    MailingAddress = source.MailingAddress == null ? null : MapAddress(source.MailingAddress),
    PreviousNames = source.PreviousNames.Select(MapPreviousName).ToList(),
  };

  public ContractRegistrants.ProfileIdentification MapProfileIdentification(ProfileIdentification source) => new()
  {
    RegistrantId = source.RegistrantId,
    PrimaryIdTypeObjectId = source.PrimaryIdTypeObjectId,
    PrimaryIds = source.PrimaryIds.Select(MapIdentityDocument).ToList(),
    SecondaryIdTypeObjectId = source.SecondaryIdTypeObjectId,
    SecondaryIds = source.SecondaryIds.Select(MapIdentityDocument).ToList(),
  };

  private PreviousName MapPreviousName(ContractRegistrants.PreviousName source) => new(source.FirstName, source.LastName)
  {
    Id = source.Id,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    Status = MapPreviousNameStage(source.Status),
    Source = MapPreviousNameSource(source.Source),
    Documents = source.Documents.Select(MapIdentityDocument).ToList(),
  };

  private ContractRegistrants.PreviousName MapPreviousName(PreviousName source) => new(source.FirstName, source.LastName)
  {
    Id = source.Id,
    MiddleName = source.MiddleName,
    PreferredName = source.PreferredName,
    Status = MapPreviousNameStage(source.Status),
    Source = MapPreviousNameSource(source.Source),
    Documents = source.Documents.Select(MapIdentityDocument).ToList(),
  };

  private static IdentityDocument MapIdentityDocument(ContractRegistrants.IdentityDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static ContractRegistrants.IdentityDocument MapIdentityDocument(IdentityDocument source) => new(source.Id)
  {
    Url = source.Url,
    Extention = source.Extention,
    Name = source.Name,
    Size = source.Size,
    EcerWebApplicationType = EcerWebApplicationType.Registry,
  };

  private static Address MapAddress(ContractRegistrants.Address source) => new(
    source.Line1,
    source.Line2,
    source.City,
    source.PostalCode,
    source.Province,
    source.Country);

  private static ContractRegistrants.Address MapAddress(Address source) => new(
    source.Line1,
    source.Line2!,
    source.City,
    source.PostalCode,
    source.Province,
    source.Country);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial StatusCode MapStatusCode(ContractRegistrants.StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PreviousNameStage MapPreviousNameStage(ContractRegistrants.PreviousNameStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractRegistrants.PreviousNameStage MapPreviousNameStage(PreviousNameStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PreviousNameSources MapPreviousNameSource(ContractRegistrants.PreviousNameSources source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractRegistrants.PreviousNameSources MapPreviousNameSource(PreviousNameSources source);

  private PreviousNameStage? MapPreviousNameStage(ContractRegistrants.PreviousNameStage? source) => source.HasValue ? MapPreviousNameStage(source.Value) : null;

  private ContractRegistrants.PreviousNameStage? MapPreviousNameStage(PreviousNameStage? source) => source.HasValue ? MapPreviousNameStage(source.Value) : null;

  private PreviousNameSources? MapPreviousNameSource(ContractRegistrants.PreviousNameSources? source) => source.HasValue ? MapPreviousNameSource(source.Value) : null;

  private ContractRegistrants.PreviousNameSources? MapPreviousNameSource(PreviousNameSources? source) => source.HasValue ? MapPreviousNameSource(source.Value) : null;
}
