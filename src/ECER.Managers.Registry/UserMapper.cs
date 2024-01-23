using AutoMapper;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

internal sealed class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<RegisterNewUserCommand, NewRegistrantRequest>()
            .ForCtorParam(nameof(NewRegistrantRequest.UserProfile), opts => opts.MapFrom(s => s.UserProfile))
            .ForCtorParam(nameof(NewRegistrantRequest.UserIdentity), opts => opts.MapFrom(s => s.Identity))
            ;

        CreateMap<UserProfile, Resources.Accounts.Registrants.UserProfile>()
            .ReverseMap()
            .ValidateMemberList(MemberList.Destination)
            ;

        CreateMap<Address, Resources.Accounts.Registrants.Address>()
            .ReverseMap()
            .ValidateMemberList(MemberList.Destination)
            ;
    }
}