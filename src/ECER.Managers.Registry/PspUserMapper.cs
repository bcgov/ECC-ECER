using Riok.Mapperly.Abstractions;
using ContractPspUsers = ECER.Managers.Registry.Contract.PspUsers;
using ResourcePspReps = ECER.Resources.Accounts.PspReps;

namespace ECER.Managers.Registry;

public interface IPspUserMapper
{
  IEnumerable<ContractPspUsers.PspUser> MapPspUsers(IEnumerable<ResourcePspReps.PspUser> source);
  ResourcePspReps.PspUserProfile MapPspUserProfile(ContractPspUsers.PspUserProfile source);
  ResourcePspReps.PspUser MapRegisteredPspUser(ContractPspUsers.RegisterNewPspUserCommand source);
}

[Mapper]
internal partial class PspUserMapper : IPspUserMapper
{
  public IEnumerable<ContractPspUsers.PspUser> MapPspUsers(IEnumerable<ResourcePspReps.PspUser> source) => source.Select(MapPspUser).ToList();

  public ResourcePspReps.PspUserProfile MapPspUserProfile(ContractPspUsers.PspUserProfile source) => new()
  {
    Id = source.Id,
    FirstName = source.FirstName,
    LastName = source.LastName,
    PreferredName = source.PreferredName,
    Phone = source.Phone,
    PhoneExtension = source.PhoneExtension,
    JobTitle = source.JobTitle,
    Role = MapPspUserRole(source.Role),
    Email = source.Email,
    HasAcceptedTermsOfUse = source.HasAcceptedTermsOfUse,
  };

  public ResourcePspReps.PspUser MapRegisteredPspUser(ContractPspUsers.RegisterNewPspUserCommand source) => new()
  {
    Id = source.Id,
    Identities = [source.Identity],
    Profile = MapPspUserProfile(source.Profile),
  };

  private ContractPspUsers.PspUser MapPspUser(ResourcePspReps.PspUser source) => new(source.Id, MapPspUserProfile(source.Profile))
  {
    AccessToPortal = MapPortalAccessStatus(source.AccessToPortal),
    PostSecondaryInstituteId = source.PostSecondaryInstituteId,
  };

  private ContractPspUsers.PspUserProfile MapPspUserProfile(ResourcePspReps.PspUserProfile source) => new()
  {
    Id = source.Id,
    FirstName = source.FirstName,
    LastName = source.LastName,
    PreferredName = source.PreferredName,
    Phone = source.Phone,
    PhoneExtension = source.PhoneExtension,
    JobTitle = source.JobTitle,
    Role = MapPspUserRole(source.Role),
    Email = source.Email,
    HasAcceptedTermsOfUse = source.HasAcceptedTermsOfUse,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPspUsers.PortalAccessStatus MapPortalAccessStatus(ResourcePspReps.PortalAccessStatus source);

  private ContractPspUsers.PortalAccessStatus? MapPortalAccessStatus(ResourcePspReps.PortalAccessStatus? source) => source.HasValue ? MapPortalAccessStatus(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePspReps.PortalAccessStatus MapPortalAccessStatus(ContractPspUsers.PortalAccessStatus source);

  private ResourcePspReps.PortalAccessStatus? MapPortalAccessStatus(ContractPspUsers.PortalAccessStatus? source) => source.HasValue ? MapPortalAccessStatus(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPspUsers.PspUserRole MapPspUserRole(ResourcePspReps.PspUserRole source);

  private ContractPspUsers.PspUserRole? MapPspUserRole(ResourcePspReps.PspUserRole? source) => source.HasValue ? MapPspUserRole(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourcePspReps.PspUserRole MapPspUserRole(ContractPspUsers.PspUserRole source);

  private ResourcePspReps.PspUserRole? MapPspUserRole(ContractPspUsers.PspUserRole? source) => source.HasValue ? MapPspUserRole(source.Value) : null;
}
