using AutoMapper;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Infrastructure.Common;

namespace ECER.Clients.RegistryPortal.Server.PortalInvitations;

public class PortalInvitationMapper : SecureProfile
{
  public PortalInvitationMapper()
  {
    CreateMap<Managers.Registry.Contract.PortalInvitations.PortalInvitation, PortalInvitation>();
  }
}
