using AutoMapper;
using ECER.Resources.Accounts.Registrants;

namespace ECER.Managers.Registry;

internal sealed class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<RegisterNewUserCommand, NewRegistrantRequest>()
            .ForCtorParam(nameof(NewRegistrantRequest.UserProfile), opts => opts.MapFrom(s => s.UserProfile))
            .ForCtorParam(nameof(NewRegistrantRequest.UserIdentity), opts => opts.MapFrom(s => s.Login))
            ;

        CreateMap<UserProfile, Resources.Accounts.Registrants.UserProfile>();

        CreateMap<Login, UserIdentity>()
            .ForMember(d => d.LastLogin, opts => opts.Ignore())
            ;
    }
}