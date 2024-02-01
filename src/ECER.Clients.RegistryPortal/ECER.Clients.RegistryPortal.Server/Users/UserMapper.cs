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
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.PreferredName), opts => opts.MapFrom(s => (string?)null))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.MiddleName), opts => opts.MapFrom(s => (string?)null))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.UserProfile.AlternateContactPhone), opts => opts.MapFrom(s => (string?)null))
      .ReverseMap()
      .ValidateMemberList(MemberList.Source)
      .ForSourceMember(s => s.ResidentialAddress, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.MailingAddress, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PreferredName, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.MiddleName, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.AlternateContactPhone, opts => opts.DoNotValidate())

      ;

    CreateMap<UserProfile, Managers.Registry.Contract.Registrants.UserProfile>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      ;

    CreateMap<Address, Managers.Registry.Contract.Registrants.Address>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Source)
      ;
  }
}
