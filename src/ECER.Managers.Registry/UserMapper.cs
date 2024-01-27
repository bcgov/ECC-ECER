using AutoMapper;

namespace ECER.Managers.Registry;

internal sealed class UserMapper : AutoMapper.Profile
{
  public UserMapper()
  {
    CreateMap<Contract.Registrants.RegisterNewRegistrantCommand, Resources.Accounts.Registrants.Registrant>()
      .ForMember(d => d.Identities, opts => opts.MapFrom(s => (new[] { s.Identity })))
      .ForMember(d => d.Id, opts => opts.Ignore())
      ;

    CreateMap<Contract.Registrants.Registrant, Resources.Accounts.Registrants.Registrant>()
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.UserId))
      .ForMember(d => d.Identities, opts => opts.Ignore())
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(Contract.Registrants.Registrant.UserId), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Contract.Registrants.Registrant.Profile), opts => opts.MapFrom(s => s.Profile))
      ;

    CreateMap<Contract.Registrants.UserProfile, Resources.Accounts.Registrants.UserProfile>()
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        ;

    CreateMap<Contract.Registrants.Address, Resources.Accounts.Registrants.Address>()
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        ;
  }
}
