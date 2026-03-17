using AutoMapper;
using ECER.Infrastructure.Common;
using ECER.Resources.Documents.PostSecondaryInstitutes;

namespace ECER.Managers.Registry;

internal sealed class PostSecondaryInstituteMapper : SecureProfile
{
  public PostSecondaryInstituteMapper()
  {
    CreateMap<Contract.PostSecondaryInstitutes.PostSecondaryInstitute, PostSecondaryInstitute>()
      .ForMember(d => d.BceidBusinessId, opts => opts.Ignore())
      .ForMember(d => d.BceidBusinessName, opts => opts.Ignore())
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
  }
}
