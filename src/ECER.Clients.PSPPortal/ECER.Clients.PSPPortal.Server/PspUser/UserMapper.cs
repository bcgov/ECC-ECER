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
    
    CreateMap<ECER.Managers.Registry.Contract.PspUsers.PspUserProfile, PspUserProfile>()
      .ForMember(d => d.UnreadMessagesCount, opts => opts.Ignore())
      .ForMember(d => d.FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.PreferredName, opts => opts.MapFrom(s => s.PreferredName))
      .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.Phone))
      .ForMember(d => d.PhoneExtension, opts => opts.MapFrom(s => s.PhoneExtension))
      .ForMember(d => d.Phone, opts => opts.MapFrom(s => s.Phone))
      .ForMember(d => d.Email, opts => opts.MapFrom(s => s.Email))
      .ValidateMemberList(MemberList.Destination);
  }
}
