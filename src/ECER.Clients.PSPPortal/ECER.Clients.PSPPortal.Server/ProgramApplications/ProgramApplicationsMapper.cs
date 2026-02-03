using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace ECER.Clients.PSPPortal.Server.ProgramApplications;

internal sealed class ProgramApplicationsMapper: Profile
{
  public ProgramApplicationsMapper()
  {
    CreateMap<ProgramApplication, Managers.Registry.Contract.ProgramApplications.ProgramApplication>()
      .ForCtorParam(nameof(Managers.Registry.Contract.ProgramApplications.ProgramApplication.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.ProgramApplications.ProgramApplication.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.PostSecondaryInstituteId))
      .ForMember(d => d.ProgramApplicationName, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<ApplicationStatus, Managers.Registry.Contract.ProgramApplications.ApplicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ApplicationType, Managers.Registry.Contract.ProgramApplications.ApplicationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<DeliveryType, Managers.Registry.Contract.ProgramApplications.DeliveryType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ProvincialCertificationTypeOffered, Managers.Registry.Contract.ProgramApplications.ProvincialCertificationTypeOffered>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
  }
}
