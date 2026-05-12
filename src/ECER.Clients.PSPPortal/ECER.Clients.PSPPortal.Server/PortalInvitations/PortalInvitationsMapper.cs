using Riok.Mapperly.Abstractions;
using ContractPortalInvitations = ECER.Managers.Registry.Contract.PortalInvitations;

namespace ECER.Clients.PSPPortal.Server.PortalInvitations;

internal interface IPortalInvitationMapper
{
  PortalInvitation MapPortalInvitation(ContractPortalInvitations.PortalInvitation? source);
}

[Mapper]
internal partial class PortalInvitationMapper : IPortalInvitationMapper
{
  public PortalInvitation MapPortalInvitation(ContractPortalInvitations.PortalInvitation? source)
  {
    if (source == null)
    {
      return null!;
    }

    return new PortalInvitation(source.Id)
    {
      PspProgramRepresentativeId = source.PspProgramRepresentativeId,
      InviteType = MapInviteType(source.InviteType),
      BceidBusinessName = source.BceidBusinessName,
      PostSecondaryInstitutionName = source.PostSecondaryInstitutionName,
      IsLinked = source.IsLinked,
    };
  }

  private static InviteType? MapInviteType(ContractPortalInvitations.InviteType? source) => source switch
  {
    null => null,
    ContractPortalInvitations.InviteType.PSIProgramRepresentative => InviteType.PSIProgramRepresentative,
    _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Unsupported portal invitation type for PSP portal."),
  };
}
