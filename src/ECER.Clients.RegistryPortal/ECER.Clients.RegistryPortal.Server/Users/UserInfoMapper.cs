using AutoMapper;
using ECER.Managers.Registry;

namespace ECER.Clients.RegistryPortal.Server.Users;

internal sealed class UserInfoMapper : Profile
{
    public UserInfoMapper()
    {
        CreateMap<UserProfileQueryResponse, UserInfoResponse>()
            .ForCtorParam(nameof(UserInfoResponse.UserInfo), opts => opts.MapFrom(s => s.UserProfile))
            ;

        CreateMap<Managers.Registry.UserProfile, UserProfile>();
    }
}