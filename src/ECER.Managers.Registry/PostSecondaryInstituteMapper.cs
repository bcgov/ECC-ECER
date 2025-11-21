using AutoMapper;
using ECER.Resources.Documents.PostSecondaryInstitutes;

namespace ECER.Managers.Registry;

internal sealed class PostSecondaryInstituteMapper : AutoMapper.Profile
{
  public PostSecondaryInstituteMapper()
  {
    CreateMap<Contract.PostSecondaryInstitutes.PostSecondaryInstitute, PostSecondaryInstitute>()
      .ForMember(d => d.BceidBusinessId, opts => opts.Ignore())
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
  }
}
