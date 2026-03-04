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
      .ForMember(d => d.StatusReasonDetail, opts => opts.MapFrom(s => s.StatusReasonDetail))
      .ForMember(d => d.ComponentsGenerationCompleted, opts => opts.MapFrom(s => s.ComponentsGenerationCompleted))
      .ForMember(d => d.ProgramRepresentativeId, opts => opts.MapFrom(s => s.ProgramRepresentativeId))
      .ForMember(d => d.ProgramLength, opts => opts.MapFrom(s => s.ProgramLength))
      .ForMember(d => d.OnlineMethodOfInstruction, opts => opts.MapFrom(s => s.OnlineMethodOfInstruction))
      .ForMember(d => d.DeliveryMethod, opts => opts.MapFrom(s => s.DeliveryMethod))
      .ForMember(d => d.EnrollmentOptions, opts => opts.MapFrom(s => s.EnrollmentOptions))
      .ForMember(d => d.AdmissionOptions, opts => opts.MapFrom(s => s.AdmissionOptions))
      .ForMember(d => d.MinimumEnrollment, opts => opts.MapFrom(s => s.MinimumEnrollment))
      .ForMember(d => d.MaximumEnrollment, opts => opts.MapFrom(s => s.MaximumEnrollment))
      .ForMember(d => d.ProgramCampuses, opts => opts.MapFrom(s => s.ProgramCampuses))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<ProgramCampus, Managers.Registry.Contract.ProgramApplications.ProgramCampus>()
      .ReverseMap();
    
    CreateMap<MethodofInstruction, Managers.Registry.Contract.ProgramApplications.MethodofInstruction>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<DeliveryMethodforInstructor, Managers.Registry.Contract.ProgramApplications.DeliveryMethodforInstructor>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<WorkHoursType, Managers.Registry.Contract.ProgramApplications.WorkHoursType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<AdmissionOptions, Managers.Registry.Contract.ProgramApplications.AdmissionOptions>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ApplicationStatus, Managers.Registry.Contract.ProgramApplications.ApplicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ApplicationStatusReasonDetail, Managers.Registry.Contract.ProgramApplications.ApplicationStatusReasonDetail>()
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
    CreateMap<FileInfo, Managers.Registry.Contract.ProgramApplications.FileInfo>().ReverseMap();
    CreateMap<ProgramApplicationComponent, Managers.Registry.Contract.ProgramApplications.ProgramApplicationComponent>()
      .ForMember(d => d.Name, opts => opts.Ignore())
      .ForMember(d => d.Question, opts => opts.Ignore())
      .ForMember(d => d.DisplayOrder, opts => opts.Ignore())
      .ForMember(d => d.Files, opts => opts.Ignore())
      .ReverseMap();
    CreateMap<ComponentGroupWithComponents, Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponents>()
      .ForMember(d => d.Name, opts => opts.Ignore())
      .ForMember(d => d.Instruction, opts => opts.Ignore())
      .ForMember(d => d.Status, opts => opts.Ignore())
      .ForMember(d => d.CategoryName, opts => opts.Ignore())
      .ForMember(d => d.DisplayOrder, opts => opts.Ignore())
      .ReverseMap();
  }
}
