using AutoMapper;
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
      .ReverseMap();

    CreateMap<PspUserProfile, ecer_ECEProgramRepresentative>(MemberList.Source)
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.Email))
      .ForMember(d => d.ecer_HasAcceptedTermsofUse, opts => opts.MapFrom(s => s.HasAcceptedTermsOfUse));

    
    CreateMap<ecer_ECEProgramRepresentative, PspUserProfile>(MemberList.Destination)
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.ecer_FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.ecer_LastName))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.ecer_EmailAddress))
      .ForMember(d => d.HasAcceptedTermsOfUse, opts => opts.MapFrom(s => s.ecer_HasAcceptedTermsofUse))
      .ValidateMemberList(MemberList.Destination);
  }
}
