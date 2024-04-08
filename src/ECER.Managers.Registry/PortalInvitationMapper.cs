using AutoMapper;
using ECER.Resources.Accounts.PortalInvitations;

namespace ECER.Managers.Registry;

public class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<PortalInvitation, Contract.PortalInvitations.PortalInvitation>();
  }
}
