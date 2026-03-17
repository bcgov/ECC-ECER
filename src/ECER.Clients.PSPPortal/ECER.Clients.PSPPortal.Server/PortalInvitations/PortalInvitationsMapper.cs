using AutoMapper;
using ECER.Infrastructure.Common;

namespace ECER.Clients.PSPPortal.Server.PortalInvitations;

public class PortalInvitationMapper : SecureProfile
{
  public PortalInvitationMapper()
  {
    CreateMap<Managers.Registry.Contract.PortalInvitations.PortalInvitation, PortalInvitation>();
  }
}
