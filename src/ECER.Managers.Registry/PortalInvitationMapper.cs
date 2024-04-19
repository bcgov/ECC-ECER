using AutoMapper;
using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Managers.Registry;

public class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<PortalInvitation, Registry.Contract.PortalInvitations.PortalInvitation>()
         .ForMember(dest => dest.ApplicantFirstName, opt => opt.Ignore())
         .ForMember(dest => dest.ApplicantLastName, opt => opt.Ignore());
  }
}
