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

    CreateMap<CreateCampusRequest, Managers.Registry.Contract.PostSecondaryInstitutes.Campus>()
      .ForMember(d => d.Id, opts => opts.Ignore())
      .ForMember(d => d.GeneratedName, opts => opts.Ignore())
      .ForMember(d => d.Status, opts => opts.Ignore())
      .ForMember(d => d.KeyCampusContactName, opts => opts.Ignore());

    CreateMap<UpdateCampusRequest, Managers.Registry.Contract.PostSecondaryInstitutes.Campus>()
      .ForMember(d => d.Id, opts => opts.Ignore())
      .ForMember(d => d.GeneratedName, opts => opts.Ignore())
      .ForMember(d => d.Status, opts => opts.Ignore())
      .ForMember(d => d.KeyCampusContactName, opts => opts.Ignore())
      .ForMember(d => d.IsSatelliteOrTemporaryLocation, opts => opts.Ignore());
  }
}

