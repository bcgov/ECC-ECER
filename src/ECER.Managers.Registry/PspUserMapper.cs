using AutoMapper;
using ECER.Resources.Accounts.PspReps;
using ECER.Resources.Accounts.PSPReps;

namespace ECER.Managers.Registry;

internal sealed class PspUserMapper : AutoMapper.Profile
{
  public PspUserMapper()
  {
    CreateMap<Contract.PspUsers.RegisterNewPspUserCommand, PspUser>()
      .ForMember(d => d.Identities, opts => opts.MapFrom(s => (new[] { s.Identity })))
      .ForMember(d => d.Profile, opts => opts.MapFrom(s => s.Profile))
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id));
    
    CreateMap<Contract.PspUsers.PspUser, PspUser>()
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.Identities, opts => opts.Ignore())
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(Contract.PspUsers.PspUser.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Contract.PspUsers.PspUser.Profile), opts => opts.MapFrom(s => s.Profile));

    CreateMap<Contract.PspUsers.PspUserProfile, PspUserProfile>()
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.Email))
      .ForMember(d => d.HasAcceptedTermsOfUse, opts => opts.MapFrom(s => s.HasAcceptedTermsOfUse))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
  }
}
