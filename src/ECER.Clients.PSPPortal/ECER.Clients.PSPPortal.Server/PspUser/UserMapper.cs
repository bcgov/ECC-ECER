using AutoMapper;

namespace ECER.Clients.PSPPortal.Server.Users;

internal sealed class PspUserMapper : AutoMapper.Profile
{
  public PspUserMapper()
  {
    CreateMap<PspUserProfile, ECER.Managers.Registry.Contract.PspUsers.PspUserProfile>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ECER.Managers.Registry.Contract.PspUsers.PortalAccessStatus, PortalAccessStatus>();
    CreateMap<ECER.Managers.Registry.Contract.PspUsers.PspUser, PspUserListItem>()
      .ForMember(d => d.Profile, opts => opts.MapFrom(s => s.Profile))
      .ForMember(d => d.PostSecondaryInstituteId, opts => opts.MapFrom(s => s.PostSecondaryInstituteId))
      .ForMember(d => d.AccessToPortal, opts => opts.MapFrom(s => s.AccessToPortal))
      .ValidateMemberList(MemberList.Destination);
  }
}
