using System.Diagnostics.Contracts;
using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Users;

internal sealed class UserMapper : AutoMapper.Profile
{
  public UserMapper()
  {
    CreateMap<UserInfo, Managers.Registry.Contract.Registrants.UserProfile>()
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.GivenName, opts => opts.MapFrom(s => s.GivenName))
      .ForMember(d => d.DateOfBirth, opts => opts.MapFrom(s => s.DateOfBirth))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.Email))
      .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.Phone))
      .ForMember(d => d.IsVerified, opts => opts.MapFrom(s => s.IsVerified))
      .ForMember(d => d.RegistrationNumber, opts => opts.MapFrom(s => s.RegistrationNumber))
      .ForMember(d => d.ResidentialAddress, opts => opts.MapFrom(s => (Address?)null))
      .ForMember(d => d.MailingAddress, opts => opts.MapFrom(s => (Address?)null))
      .ForMember(d => d.PreferredName, opts => opts.MapFrom(s => (string?)null))
      .ForMember(d => d.AlternateContactPhone, opts => opts.MapFrom(s => (string?)null))
      .ForMember(d => d.PreviousNames, opts => opts.MapFrom(s => (PreviousName[]?)null))
      .ReverseMap()
      .ValidateMemberList(MemberList.Source)
      .ForSourceMember(s => s.ResidentialAddress, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.MailingAddress, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PreferredName, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.AlternateContactPhone, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PreviousNames, opts => opts.DoNotValidate());

    CreateMap<PreviousName, Managers.Registry.Contract.Registrants.PreviousName>()
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.PreviousName.FirstName),
        opts => opts.MapFrom(s => s.FirstName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Registrants.PreviousName.LastName),
        opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.MiddleName, opts => opts.MapFrom(s => s.MiddleName))
      .ForMember(d => d.PreferredName, opts => opts.MapFrom(s => s.PreferredName))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.Id,
        opts => opts.MapFrom(src => string.IsNullOrEmpty(src.Id) ? null : src.Id))
      .ReverseMap();

    CreateMap<Managers.Registry.Contract.Registrants.IdentityDocument, IdentityDocument>().ReverseMap();

    CreateMap<UserProfile, Managers.Registry.Contract.Registrants.UserProfile>()
      .ForMember(d => d.PreviousNames, opts => opts.MapFrom(s => s.PreviousNames))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      ;

    CreateMap<Address, Managers.Registry.Contract.Registrants.Address>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Source)
      ;
  }
}
