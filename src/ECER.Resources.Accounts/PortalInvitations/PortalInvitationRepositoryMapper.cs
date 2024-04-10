using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.PortalInvitations;

internal sealed class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<ecer_PortalInvitation, PortalInvitation>(MemberList.Destination)
      .ForCtorParam(nameof(PortalInvitation.Id), opts => opts.MapFrom(s => s.ecer_PortalInvitationId))
      .ForCtorParam(nameof(PortalInvitation.name), opts => opts.MapFrom(s => s.ecer_Name))
      .ForCtorParam(nameof(PortalInvitation.firstName), opts => opts.MapFrom(s => s.ecer_FirstName))
      .ForCtorParam(nameof(PortalInvitation.lastName), opts => opts.MapFrom(s => s.ecer_LastName))
      .ForCtorParam(nameof(PortalInvitation.emailAddress), opts => opts.MapFrom(s => s.ecer_EmailAddress))
      .ForCtorParam(nameof(PortalInvitation.inviteType), opts => opts.MapFrom(s => DetermineInviteType(s)))
      .ForMember(d => d.Registrant, opts => opts.MapFrom(s => s.ecer_portalinvitation_ApplicantId));
  }

  private static InviteType? DetermineInviteType(ecer_PortalInvitation source)
  {
    if (source.ecer_WorkExperienceReferenceId != null)
    {
      return InviteType.WorkExperienceReference;
    }
    else if (source.ecer_CharacterReferenceId != null)
    {
      return InviteType.CharacterReference;
    }
    return null;
  }
}
