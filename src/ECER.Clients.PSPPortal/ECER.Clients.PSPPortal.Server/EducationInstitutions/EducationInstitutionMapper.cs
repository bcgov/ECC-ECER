using AutoMapper;

namespace ECER.Clients.PSPPortal.Server.EducationInstitutions;

internal sealed class EducationInstitutionMapper : AutoMapper.Profile
{
  public EducationInstitutionMapper()
  {
    CreateMap<EducationInstitution, Managers.Registry.Contract.PostSecondaryInstitutes.PostSecondaryInstitute>()
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<Campus, Managers.Registry.Contract.PostSecondaryInstitutes.Campus>()
      .ReverseMap();

    CreateMap<CreateCampusRequest, Managers.Registry.Contract.PostSecondaryInstitutes.Campus>();

    CreateMap<UpdateCampusRequest, Managers.Registry.Contract.PostSecondaryInstitutes.Campus>()
      .ForMember(d => d.IsSatelliteOrTemporaryLocation, opts => opts.Ignore());
  }
}

