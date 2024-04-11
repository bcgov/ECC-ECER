using AutoMapper;
using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Managers.Registry;

public class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<PortalInvitation, Registry.Contract.PortalInvitations.PortalInvitation>();
  }
}
