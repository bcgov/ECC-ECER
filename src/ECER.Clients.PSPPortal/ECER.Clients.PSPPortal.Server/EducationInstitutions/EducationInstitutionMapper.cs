using AutoMapper;
using ECER.Infrastructure.Common;

namespace ECER.Clients.PSPPortal.Server.EducationInstitutions;

internal sealed class EducationInstitutionMapper : SecureProfile
{
  public EducationInstitutionMapper()
  {
    CreateMap<EducationInstitution, Managers.Registry.Contract.PostSecondaryInstitutes.PostSecondaryInstitute>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<Campus, Managers.Registry.Contract.PostSecondaryInstitutes.Campus>()
      .ReverseMap();
  }
}

