using AutoMapper;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

internal sealed class RegistrantMapper : AutoMapper.Profile
{
  public RegistrantMapper()
  {
    CreateMap<Contract.Registrants.RegisterNewUserCommand, Resources.Accounts.Registrants.Registrant>()
      .ForMember(d => d.Identities, opts => opts.MapFrom(s => (new[] { s.Identity })))
      .ForMember(d => d.Profile, opts => opts.MapFrom(s => s.Profile))
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
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.MiddleName, opts => opts.MapFrom(s => s.MiddleName))
      .ForMember(d => d.PreferredName, opts => opts.MapFrom(s => s.PreferredName))
      .ForMember(d => d.AlternateContactPhone, opts => opts.MapFrom(s => s.AlternateContactPhone))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.Email))
      .ForMember(d => d.DateOfBirth, opts => opts.MapFrom(s => s.DateOfBirth))
      .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.Phone))
      .ForMember(d => d.IsVerified, opts => opts.MapFrom(s => s.IsVerified))
      .ForMember(d => d.RegistrationNumber, opts => opts.MapFrom(s => s.RegistrationNumber))
      .ForMember(d => d.ResidentialAddress, opts => opts.MapFrom(s => s.ResidentialAddress))
      .ForMember(d => d.MailingAddress, opts => opts.MapFrom(s => s.MailingAddress))
      .ForMember(d => d.PreviousNames, opts => opts.MapFrom(s => s.PreviousNames))
      .ForMember(d => d.IsRegistrant, opts => opts.MapFrom(s => s.IsRegistrant))
      .ReverseMap()
      .ForMember(d => d.GivenName, opts => opts.Ignore())
      .ForMember(d => d.MiddleName, opts => opts.MapFrom(s => s.MiddleName))
        .ValidateMemberList(MemberList.Destination)
        ;

    CreateMap<Contract.Registrants.PreviousName, PreviousName>().ReverseMap();

    CreateMap<Contract.Registrants.IdentityDocument, IdentityDocument>().ReverseMap();

    CreateMap<Contract.Registrants.Address, Address>()
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        ;
  }
}
