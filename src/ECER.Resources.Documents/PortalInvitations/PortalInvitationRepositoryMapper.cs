using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.PortalInvitations;

internal sealed class PortalInvitationMapper : Profile
{
  public PortalInvitationMapper()
  {
    CreateMap<ecer_PortalInvitation, PortalInvitation>(MemberList.Destination)
      .ForCtorParam(nameof(PortalInvitation.Id), opts => opts.MapFrom(s => s.ecer_PortalInvitationId))
      .ForCtorParam(nameof(PortalInvitation.Name), opts => opts.MapFrom(s => s.ecer_Name))
      .ForCtorParam(nameof(PortalInvitation.ReferenceFirstName), opts => opts.MapFrom(s => s.ecer_FirstName))
      .ForCtorParam(nameof(PortalInvitation.ReferenceLastName), opts => opts.MapFrom(s => s.ecer_LastName))
      .ForCtorParam(nameof(PortalInvitation.ReferenceEmailAddress), opts => opts.MapFrom(s => s.ecer_EmailAddress))
      .ForMember(d => d.WorkexperienceReferenceId, opts => opts.MapFrom(s => s.ecer_WorkExperienceReferenceId == null ? null : s.ecer_WorkExperienceReferenceId.Id.ToString()))
      .ForMember(d => d.CharacterReferenceId, opts => opts.MapFrom(s => s.ecer_CharacterReferenceId == null ? null : s.ecer_CharacterReferenceId.Id.ToString()))
      .ForMember(d => d.ApplicantId, opts => opts.MapFrom(s => s.ecer_ApplicantId == null ? null : s.ecer_ApplicantId.Id.ToString()))
      .ForMember(d => d.ApplicationId, opts => opts.MapFrom(s => s.ecer_ApplicationId == null ? null : s.ecer_ApplicationId.Id.ToString()))
      .ForMember(d => d.PspProgramRepresentativeId, opts => opts.MapFrom(s => s.ecer_psiprogramrepresentativeid == null ? null : s.ecer_psiprogramrepresentativeid.Id.ToString()))
      .ForMember(d => d.InviteType, opts => opts.MapFrom(s => s.ecer_Type))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.StatusCode));

    CreateMap<PortalInvitationStatusCode, ecer_PortalInvitation_StatusCode>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<InviteType, ecer_PortalInvitationTypes>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
  }
}
