using AutoMapper;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.PortalInvitations;

namespace ECER.Managers.Registry;

public class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<PortalInvitation, Contract.PortalInvitations.PortalInvitation>()
         .ForMember(dest => dest.ApplicantFirstName, opt => opt.Ignore())
         .ForMember(dest => dest.ApplicantLastName, opt => opt.Ignore())
         .ForMember(dest => dest.CertificationTypes, opt => opt.Ignore())
         .ForMember(dest => dest.WorkExperienceReferenceHours, opt => opt.Ignore())
         .ForMember(dest => dest.WorkExperienceType, opt => opt.Ignore())
         .ForMember(dest => dest.LatestCertification, opt => opt.Ignore());

    CreateMap<WorkExperienceTypes, Contract.Applications.WorkExperienceTypes>();
  }
}
