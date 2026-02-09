using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;

namespace ECER.Resources.Accounts.PSPReps;

internal sealed class PspRepRepositoryMapper : Profile
{
  public PspRepRepositoryMapper()
  {
    CreateMap<ecer_ECEProgramRepresentative, PspUser>()
      .ForMember(d => d.Profile, opts => opts.MapFrom(s => s))
      .ForMember(d => d.Identities, opts => opts.MapFrom(s => s.ecer_authentication_eceprogramrepresentative))
      .ForMember(d => d.AccessToPortal, opts => opts.MapFrom(s => s.ecer_AccessToPortal))
      .ForMember(
        d => d.PostSecondaryInstituteId,
        opts => opts.MapFrom(s => s.ecer_PostSecondaryInstitute != null ? s.ecer_PostSecondaryInstitute.Id.ToString() : null))
      .ReverseMap()
      .ForMember(d => d.ecer_AccessToPortal, opts => opts.Ignore())
      .ForMember(d => d.ecer_PostSecondaryInstitute, opts => opts.Ignore());

    CreateMap<PspUserProfile, ecer_ECEProgramRepresentative>(MemberList.Source)
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.ecer_PreferredFirstName, opts => opts.MapFrom(s => s.PreferredName))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.Phone))
      .ForMember(d => d.ecer_PhoneExtension, opts => opts.MapFrom(s => s.PhoneExtension))
      .ForMember(d => d.ecer_Role, opts => opts.MapFrom(s => s.JobTitle))
      .ForMember(d => d.ecer_RepresentativeRole, opts => opts.MapFrom(s => s.Role))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.Email))
      .ForMember(d => d.ecer_HasAcceptedTermsofUse, opts => opts.MapFrom(s => s.HasAcceptedTermsOfUse));

    
    CreateMap<ecer_ECEProgramRepresentative, PspUserProfile>(MemberList.Destination)
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.ecer_FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.ecer_LastName))
      .ForMember(d => d.PreferredName, opts => opts.MapFrom(s => s.ecer_PreferredFirstName))
      .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.ecer_PhoneNumber))
      .ForMember(d => d.PhoneExtension, opts => opts.MapFrom(s => s.ecer_PhoneExtension))
      .ForMember(d => d.JobTitle, opts => opts.MapFrom(s => s.ecer_Role))
      .ForMember(d => d.Role, opts => opts.MapFrom(s => s.ecer_RepresentativeRole))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.ecer_EmailAddress))
      .ForMember(d => d.HasAcceptedTermsOfUse, opts => opts.MapFrom(s => s.ecer_HasAcceptedTermsofUse))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<PortalAccessStatus, ecer_AccessToPortal>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
  }
}
