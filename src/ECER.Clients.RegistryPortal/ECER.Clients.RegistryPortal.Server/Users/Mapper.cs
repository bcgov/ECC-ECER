using AutoMapper;
using ECER.Managers.Registry;

namespace ECER.Clients.RegistryPortal.Server.Users;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<UserProfileQueryResponse, UserInfoResponse>();

        CreateMap<ECER.Managers.Registry.UserProfile, UserProfile>();
    }
}