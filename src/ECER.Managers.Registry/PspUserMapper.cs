using AutoMapper;
using ECER.Resources.Accounts.PSPReps;

namespace ECER.Managers.Registry;

internal sealed class PspUserMapper : AutoMapper.Profile
{
  public PspUserMapper()
  {
    CreateMap<Contract.PspUsers.RegisterNewPSPRepCommand, PSPRep>()
      .ForMember(d => d.Identities, opts => opts.MapFrom(s => (new[] { s.Identity })))
      .ForMember(d => d.Profile, opts => opts.MapFrom(s => s.Profile))
      .ForMember(d => d.Id, opts => opts.Ignore());

    CreateMap<Contract.PSPReps.PSPRep, PSPRep>()
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.UserId))
      .ForMember(d => d.Identities, opts => opts.Ignore())
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(Contract.Registrants.Registrant.UserId), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Contract.Registrants.Registrant.Profile), opts => opts.MapFrom(s => s.Profile));

    CreateMap<Contract.PSPReps.PSPRepProfile, PSPRepProfile>()
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.Email))
      .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.Phone))
      .ForMember(d => d.PhoneExtension, opts => opts.MapFrom(s => s.PhoneExtension))
      .ForMember(d => d.Role, opts => opts.MapFrom(s => s.Role))
      .ForMember(d => d.JobTitle, opts => opts.MapFrom(s => s.JobTitle))
      .ForMember(d => d.BceidBusinessId, opts => opts.MapFrom(s => s.BceidBusinessId))
      .ForMember(d => d.PreferredName, opts => opts.MapFrom(s => s.PreferredName))
      .ReverseMap()
      .ForMember(d => d.PreferredName, opts => opts.Ignore())
      .ValidateMemberList(MemberList.Destination);
  }
}
