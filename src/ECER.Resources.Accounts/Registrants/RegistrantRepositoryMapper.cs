using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.Security;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Accounts.Registrants;

internal interface IRegistrantRepositoryMapper
{
  List<Registrant> MapRegistrants(IEnumerable<Contact> source);
  Contact MapUserProfile(UserProfile source);
  List<ecer_PreviousName> MapPreviousNames(IEnumerable<PreviousName> source);
  List<bcgov_DocumentUrl> MapIdentityDocuments(IEnumerable<IdentityDocument> source);
}

[Mapper]
internal partial class RegistrantRepositoryMapper : IRegistrantRepositoryMapper
{
  public List<Registrant> MapRegistrants(IEnumerable<Contact> source) => source.Select(MapRegistrant).ToList();

  public Contact MapUserProfile(UserProfile source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    BirthDate = source.DateOfBirth == null ? null : source.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue),
    Address1_Telephone1 = source.Phone,
    Telephone1 = source.Phone,
    ecer_IsVerified = source.IsVerified,
    ecer_TempClientID = source.RegistrationNumber,
    ecer_IsBCECE = !string.IsNullOrEmpty(source.RegistrationNumber),
    ecer_PreferredName = source.PreferredName,
    Address1_Telephone2 = source.AlternateContactPhone,
    MiddleName = source.MiddleName,
    EMailAddress1 = source.Email,
    Address1_Line1 = source.ResidentialAddress?.Line1,
    Address1_Line2 = source.ResidentialAddress?.Line2,
    Address1_City = source.ResidentialAddress?.City,
    Address1_PostalCode = source.ResidentialAddress?.PostalCode,
    Address1_StateOrProvince = source.ResidentialAddress?.Province,
    Address1_Country = source.ResidentialAddress?.Country,
    Address2_Line1 = source.MailingAddress?.Line1,
    Address2_Line2 = source.MailingAddress?.Line2,
    Address2_City = source.MailingAddress?.City,
    Address2_PostalCode = source.MailingAddress?.PostalCode,
    Address2_StateOrProvince = source.MailingAddress?.Province,
    Address2_Country = source.MailingAddress?.Country,
    StatusCode = MapStatusCode(source.Status),
    ecer_idverificationdecision = MapIdVerificationDecision(source.IDVerificationDecision),
  };

  public List<ecer_PreviousName> MapPreviousNames(IEnumerable<PreviousName> source) => source.Select(MapPreviousName).ToList();

  public List<bcgov_DocumentUrl> MapIdentityDocuments(IEnumerable<IdentityDocument> source) => source.Select(MapIdentityDocument).ToList();

  private Registrant MapRegistrant(Contact source) => new()
  {
    Id = source.Id.ToString(),
    Identities = MapUserIdentities(source.ecer_contact_ecer_authentication_455),
    Profile = MapUserProfile(source),
  };

  private UserProfile MapUserProfile(Contact source) => new()
  {
    FirstName = source.FirstName,
    LastName = source.LastName,
    MiddleName = source.MiddleName,
    PreferredName = source.ecer_PreferredName,
    AlternateContactPhone = source.Address1_Telephone2,
    DateOfBirth = source.BirthDate.HasValue ? DateOnly.FromDateTime(source.BirthDate.Value) : null,
    Email = source.EMailAddress1 ?? string.Empty,
    Phone = source.Address1_Telephone1 ?? string.Empty,
    IsVerified = source.ecer_IsVerified ?? false,
    ResidentialAddress = source.Address1_Line1 == null ? null : new Address(source.Address1_Line1, source.Address1_Line2, source.Address1_City, source.Address1_PostalCode, source.Address1_StateOrProvince, source.Address1_Country),
    MailingAddress = source.Address2_Line1 == null ? null : new Address(source.Address2_Line1, source.Address2_Line2, source.Address2_City, source.Address2_PostalCode, source.Address2_StateOrProvince, source.Address2_Country),
    PreviousNames = MapRegistrantPreviousNames(source.ecer_previousname_Contactid),
    RegistrationNumber = source.ecer_ClientID,
    Status = MapStatusCode(source.StatusCode) ?? default,
    IDVerificationDecision = MapIdVerificationDecision(source.ecer_idverificationdecision),
  };

  private ecer_PreviousName MapPreviousName(PreviousName source)
  {
    var previousName = new ecer_PreviousName
    {
      ecer_FirstName = source.FirstName,
      ecer_LastName = source.LastName,
      ecer_PreferredName = source.PreferredName,
      ecer_MiddleName = source.MiddleName,
      StatusCode = MapPreviousNameStage(source.Status),
      ecer_Source = MapPreviousNameSource(source.Source),
      ecer_documenturl_PreviousNameId = source.Documents.Any() ? MapIdentityDocuments(source.Documents) : null,
    };

    if (Guid.TryParse(source.Id, out var previousNameId))
    {
      previousName.Id = previousNameId;
    }

    return previousName;
  }

  private PreviousName MapPreviousName(ecer_PreviousName source) => new(source.ecer_FirstName ?? string.Empty, source.ecer_LastName ?? string.Empty)
  {
    PreferredName = source.ecer_PreferredName,
    MiddleName = source.ecer_MiddleName,
    Status = MapPreviousNameStage(source.StatusCode),
    Id = source.ecer_PreviousNameId?.ToString(),
    Source = MapPreviousNameSource(source.ecer_Source),
  };

  private List<PreviousName> MapRegistrantPreviousNames(IEnumerable<ecer_PreviousName>? source) =>
    source?.Select(MapPreviousName).ToList() ?? new List<PreviousName>();

  private bcgov_DocumentUrl MapIdentityDocument(IdentityDocument source)
  {
    var document = new bcgov_DocumentUrl
    {
      bcgov_FileName = source.Name,
      bcgov_FileSize = source.Size,
      bcgov_Url = source.Url,
      bcgov_FileExtension = source.Extention,
      ecer_ApplicationName = source.EcerWebApplicationType.ToString(),
    };

    if (Guid.TryParse(source.Id, out var documentId))
    {
      document.bcgov_DocumentUrlId = documentId;
    }

    return document;
  }

  private static List<UserIdentity> MapUserIdentities(IEnumerable<ecer_Authentication>? source) =>
    source?.Select(MapUserIdentity).ToList() ?? new List<UserIdentity>();

  private static UserIdentity MapUserIdentity(ecer_Authentication source) =>
    new(source.ecer_ExternalID ?? string.Empty, source.ecer_IdentityProvider ?? string.Empty);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PreviousName_StatusCode MapPreviousNameStage(PreviousNameStage source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PreviousNameStage MapPreviousNameStage(ecer_PreviousName_StatusCode source);

  private ecer_PreviousName_StatusCode? MapPreviousNameStage(PreviousNameStage? source) => source.HasValue ? MapPreviousNameStage(source.Value) : null;

  private PreviousNameStage? MapPreviousNameStage(ecer_PreviousName_StatusCode? source) => source.HasValue ? MapPreviousNameStage(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PreviousNameSources MapPreviousNameSource(PreviousNameSources source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PreviousNameSources MapPreviousNameSource(ecer_PreviousNameSources source);

  private ecer_PreviousNameSources? MapPreviousNameSource(PreviousNameSources? source) => source.HasValue ? MapPreviousNameSource(source.Value) : null;

  private PreviousNameSources? MapPreviousNameSource(ecer_PreviousNameSources? source) => source.HasValue ? MapPreviousNameSource(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial Contact_StatusCode MapStatusCode(StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial StatusCode MapStatusCode(Contact_StatusCode source);

  private Contact_StatusCode? MapStatusCode(StatusCode? source) => source.HasValue ? MapStatusCode(source.Value) : null;

  private StatusCode? MapStatusCode(Contact_StatusCode? source) => source.HasValue ? MapStatusCode(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_IDVerificationDecision MapIdVerificationDecision(IDVerificationDecision source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial IDVerificationDecision MapIdVerificationDecision(ecer_IDVerificationDecision source);

  private ecer_IDVerificationDecision? MapIdVerificationDecision(IDVerificationDecision? source) => source.HasValue ? MapIdVerificationDecision(source.Value) : null;

  private IDVerificationDecision? MapIdVerificationDecision(ecer_IDVerificationDecision? source) => source.HasValue ? MapIdVerificationDecision(source.Value) : null;
}
