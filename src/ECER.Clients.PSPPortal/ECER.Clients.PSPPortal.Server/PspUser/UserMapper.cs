using AutoMapper;

namespace ECER.Clients.PSPPortal.Server.Users;

internal sealed class PspUserMapper : AutoMapper.Profile
{
  public PspUserMapper()
  {
    CreateMap<PspUserProfile, Managers.Registry.Contract.PspUsers.PspUserProfile>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
  }
}

