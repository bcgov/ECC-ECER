using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Accounts.PSPReps;

internal interface IPspRepRepositoryMapper
{
  List<PspUser> MapPspUsers(IEnumerable<ecer_ECEProgramRepresentative> source);
  ecer_ECEProgramRepresentative MapPspUserProfile(PspUserProfile source);
}

[Mapper]
internal partial class PspRepRepositoryMapper : IPspRepRepositoryMapper
{
  public List<PspUser> MapPspUsers(IEnumerable<ecer_ECEProgramRepresentative> source) => source.Select(MapPspUser).ToList();

  public ecer_ECEProgramRepresentative MapPspUserProfile(PspUserProfile source)
  {
    var representative = new ecer_ECEProgramRepresentative
    {
      ecer_FirstName = source.FirstName,
      ecer_LastName = source.LastName,
      ecer_PreferredFirstName = source.PreferredName,
      ecer_PhoneNumber = source.Phone,
      ecer_PhoneExtension = source.PhoneExtension,
      ecer_Role = source.JobTitle,
      ecer_RepresentativeRole = MapPspUserRole(source.Role),
      ecer_EmailAddress = source.Email,
      ecer_HasAcceptedTermsofUse = source.HasAcceptedTermsOfUse,
    };

    if (Guid.TryParse(source.Id, out var representativeId))
    {
      representative.Id = representativeId;
    }

    return representative;
  }

  private PspUser MapPspUser(ecer_ECEProgramRepresentative source) => new()
  {
    Id = source.Id.ToString(),
    Profile = MapPspUserProfile(source),
    Identities = MapUserIdentities(source.ecer_authentication_eceprogramrepresentative),
    AccessToPortal = MapPortalAccessStatus(source.ecer_AccessToPortal),
    PostSecondaryInstituteId = source.ecer_PostSecondaryInstitute != null ? source.ecer_PostSecondaryInstitute.Id.ToString() : null,
  };

  private PspUserProfile MapPspUserProfile(ecer_ECEProgramRepresentative source) => new()
  {
    Id = source.Id.ToString(),
    FirstName = source.ecer_FirstName,
    LastName = source.ecer_LastName,
    PreferredName = source.ecer_PreferredFirstName,
    Phone = source.ecer_PhoneNumber,
    PhoneExtension = source.ecer_PhoneExtension,
    JobTitle = source.ecer_Role,
    Role = MapPspUserRole(source.ecer_RepresentativeRole),
    Email = source.ecer_EmailAddress,
    HasAcceptedTermsOfUse = source.ecer_HasAcceptedTermsofUse,
  };

  private List<UserIdentity> MapUserIdentities(IEnumerable<ecer_Authentication>? source) =>
    source?.Select(MapUserIdentity).ToList() ?? new List<UserIdentity>();

  private UserIdentity MapUserIdentity(ecer_Authentication source) =>
    new(source.ecer_ExternalID ?? string.Empty, source.ecer_IdentityProvider ?? string.Empty);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_AccessToPortal MapPortalAccessStatus(PortalAccessStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PortalAccessStatus MapPortalAccessStatus(ecer_AccessToPortal source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_RepresentativeRole MapPspUserRole(PspUserRole source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PspUserRole MapPspUserRole(ecer_RepresentativeRole source);

  private ecer_RepresentativeRole? MapPspUserRole(PspUserRole? source) => source.HasValue ? MapPspUserRole(source.Value) : null;

  private PspUserRole? MapPspUserRole(ecer_RepresentativeRole? source) => source.HasValue ? MapPspUserRole(source.Value) : null;

  private PortalAccessStatus? MapPortalAccessStatus(ecer_AccessToPortal? source) => source.HasValue ? MapPortalAccessStatus(source.Value) : null;
}
