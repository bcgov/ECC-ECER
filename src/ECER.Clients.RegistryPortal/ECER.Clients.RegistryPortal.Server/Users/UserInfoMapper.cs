using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Users;

internal sealed class UserInfoMapper : Profile
{
    public UserInfoMapper()
    {
        CreateMap<Managers.Registry.UserProfile, UserProfile>().ReverseMap();
        CreateMap<Address, Managers.Registry.Address>().ReverseMap();
    }
}