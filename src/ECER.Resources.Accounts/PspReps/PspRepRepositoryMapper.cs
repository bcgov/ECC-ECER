using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;

namespace ECER.Resources.Accounts.PSPReps;

internal sealed class PspRepRepositoryMapper : Profile
{
  public PspRepRepositoryMapper()
  {
    CreateMap<ecer_ECEProgramRepresentative, PspRep>()
        .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.ecer_FirstName))
        .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.ecer_LastName))
        .ForMember(d => d.Email, opts => opts.MapFrom(s => s.ecer_EmailAddress))
        .ReverseMap()
        .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
        .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
        .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.Email))
        .ValidateMemberList(MemberList.Source);

    CreateMap<ecer_Authentication, UserIdentity>()
        .ForCtorParam(nameof(UserIdentity.UserId), opts => opts.MapFrom(s => s.ecer_ExternalID))
        .ForCtorParam(nameof(UserIdentity.IdentityProvider), opts => opts.MapFrom(s => s.ecer_IdentityProvider));
  }
}
