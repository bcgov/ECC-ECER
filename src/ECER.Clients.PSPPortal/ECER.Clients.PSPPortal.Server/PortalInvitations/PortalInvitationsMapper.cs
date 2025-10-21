using AutoMapper;
using ECER.Clients.PSPPortal.Server.PortalInvitations;

namespace ECER.Clients.PSPPortal.Server.PortalInvitations;

public class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<Managers.Registry.Contract.PortalInvitations.PortalInvitation, PortalInvitation>();
  }
}
