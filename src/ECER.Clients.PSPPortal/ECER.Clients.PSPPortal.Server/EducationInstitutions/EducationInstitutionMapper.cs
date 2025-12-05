using AutoMapper;

namespace ECER.Clients.PSPPortal.Server.EducationInstitutions;

internal sealed class EducationInstitutionMapper : AutoMapper.Profile
{
  public EducationInstitutionMapper()
  {
    CreateMap<Managers.Admin.Contract.Metadatas.Province, Province>().ReverseMap();
    CreateMap<EducationInstitution, Managers.Registry.Contract.PostSecondaryInstitutes.PostSecondaryInstitute>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
  }
}

