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
        .ReverseMap()
        .ValidateMemberList(MemberList.Source);
    
    CreateMap<PspUserProfile, ecer_ECEProgramRepresentative>()
        .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
        .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
        .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.Email))
        .ForMember(d => d.ecer_ECEProgramRepresentativeId, opts => opts.MapFrom(s => s.ProgramRepresentativeId))
        .ReverseMap()
        .ValidateMemberList(MemberList.Source);
        
    CreateMap<ecer_Authentication, UserIdentity>()
        .ForCtorParam(nameof(UserIdentity.UserId), opts => opts.MapFrom(s => s.ecer_ExternalID))
        .ForCtorParam(nameof(UserIdentity.IdentityProvider), opts => opts.MapFrom(s => s.ecer_IdentityProvider));
  }
}
