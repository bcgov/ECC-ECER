using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Users;

internal sealed class UserMapper : AutoMapper.Profile
{
  public UserMapper()
  {
    CreateMap<UserInfo, Managers.Registry.Contract.Registrants.UserProfile>()
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.FirstName), opts => opts.MapFrom(s => s.FirstName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.LastName), opts => opts.MapFrom(s => s.LastName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.DateOfBirth), opts => opts.MapFrom(s => s.DateOfBirth))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.Email), opts => opts.MapFrom(s => s.Email))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.Phone), opts => opts.MapFrom(s => s.Phone))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.ResidentialAddress), opts => opts.MapFrom(s => (Address?)null))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.MailingAddress), opts => opts.MapFrom(s => (Address?)null))
      .ReverseMap()
      .ValidateMemberList(MemberList.Source)
      .ForSourceMember(s => s.ResidentialAddress, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.MailingAddress, opts => opts.DoNotValidate())
      ;

    CreateMap<Address, Managers.Registry.Contract.Registrants.Address>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Source)
      ;
  }
}
