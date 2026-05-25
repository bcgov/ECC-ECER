using Riok.Mapperly.Abstractions;
using ContractPspUsers = ECER.Managers.Registry.Contract.PspUsers;

namespace ECER.Clients.PSPPortal.Server.Users;

internal interface IPspUserMapper
{
  PspUserProfile MapUserProfile(ContractPspUsers.PspUserProfile source);
  ContractPspUsers.PspUserProfile MapUserProfile(PspUserProfile source);
  IEnumerable<PspUserListItem> MapUserListItems(IEnumerable<ContractPspUsers.PspUser> source);
}

[Mapper]
internal partial class PspUserMapper : IPspUserMapper
{
  public PspUserProfile MapUserProfile(ContractPspUsers.PspUserProfile source) => new()
  {
    Id = source.Id,
    FirstName = source.FirstName,
    LastName = source.LastName,
    PreferredName = source.PreferredName,
    Phone = source.Phone,
    PhoneExtension = source.PhoneExtension,
    JobTitle = source.JobTitle,
    Role = MapPspUserRole(source.Role),
    UnreadMessagesCount = 0,
    Email = source.Email,
    HasAcceptedTermsOfUse = source.HasAcceptedTermsOfUse,
  };

  public ContractPspUsers.PspUserProfile MapUserProfile(PspUserProfile source) => new()
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

  public IEnumerable<PspUserListItem> MapUserListItems(IEnumerable<ContractPspUsers.PspUser> source) => source.Select(MapUserListItem).ToList();

  private PspUserListItem MapUserListItem(ContractPspUsers.PspUser source) => new()
  {
    Id = source.Id,
    Profile = MapUserProfile(source.Profile),
    AccessToPortal = MapPortalAccessStatus(source.AccessToPortal),
    PostSecondaryInstituteId = source.PostSecondaryInstituteId,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PspUserRole MapPspUserRole(ContractPspUsers.PspUserRole source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractPspUsers.PspUserRole MapPspUserRole(PspUserRole source);

  private PspUserRole? MapPspUserRole(ContractPspUsers.PspUserRole? source) => source.HasValue ? MapPspUserRole(source.Value) : null;

  private ContractPspUsers.PspUserRole? MapPspUserRole(PspUserRole? source) => source.HasValue ? MapPspUserRole(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial PortalAccessStatus MapPortalAccessStatus(ContractPspUsers.PortalAccessStatus source);

  private PortalAccessStatus? MapPortalAccessStatus(ContractPspUsers.PortalAccessStatus? source) => source.HasValue ? MapPortalAccessStatus(source.Value) : null;
}
