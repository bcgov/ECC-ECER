using AutoMapper;
using ECER.Clients.RegistryPortal.Server.References;

namespace ECER.Clients.RegistryPortal.Server.PortalInvitations;

public class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<Managers.Registry.Contract.PortalInvitations.PortalInvitation, PortalInvitation>();
  }
}
