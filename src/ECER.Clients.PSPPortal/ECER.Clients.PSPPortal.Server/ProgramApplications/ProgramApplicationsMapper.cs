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
      .ForMember(d => d.ProgramApplicationType, opts => opts.MapFrom(s => s.ProgramApplicationType))
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ProgramTypes))
      .ForMember(d => d.DeliveryType, opts => opts.MapFrom(s => s.DeliveryType))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ComponentsGenerationCompleted, opts => opts.MapFrom(s => s.ComponentsGenerationCompleted))
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
    
    CreateMap<ProgramCertificationType, Managers.Registry.Contract.ProgramApplications.ProgramCertificationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ComponentGroupMetadata, Managers.Registry.Contract.ProgramApplications.ComponentGroupMetadata>().ReverseMap();
    CreateMap<ProgramApplicationComponent, Managers.Registry.Contract.ProgramApplications.ProgramApplicationComponent>().ReverseMap();
    CreateMap<ComponentGroupWithComponents, Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponents>().ReverseMap();
  }
}
