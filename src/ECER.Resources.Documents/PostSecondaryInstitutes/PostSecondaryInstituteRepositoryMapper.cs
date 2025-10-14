using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.PostSecondaryInstitutes;

internal sealed class PostSecondaryInstituteRepositoryMapper : Profile
{
  public PostSecondaryInstituteRepositoryMapper()
  {
    CreateMap<ecer_PostSecondaryInstitute, PostSecondaryInstitute>()
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_PostSecondaryInstituteId))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.BceidBusinessId, opts => opts.MapFrom(s => s.ecer_BusinessBCeID))
      .ReverseMap()
      .ForMember(d => d.ecer_PostSecondaryInstituteId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.Name))
      .ForMember(d => d.ecer_BusinessBCeID, opts => opts.MapFrom(s => s.BceidBusinessId))
      .ValidateMemberList(MemberList.Source);
  }
}
