using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;
using ComponentGroupMetadata = ECER.Resources.Documents.ProgramApplications.ComponentGroupMetadata;
using ProgramCampus = ECER.Resources.Documents.ProgramApplications.ProgramCampus;
using ResourcesProgramCertificationType = ECER.Resources.Documents.ProgramApplications.ProgramCertificationType;

namespace ECER.Managers.Registry;

internal class ProgramApplicationMapper: Profile
{
  public ProgramApplicationMapper()
  {
    CreateMap<ProgramApplication, Resources.Documents.ProgramApplications.ProgramApplication>()
      .ForCtorParam(nameof(ProgramApplication.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(ProgramApplication.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.PostSecondaryInstituteId))
      .ForMember(d => d.ProgramApplicationName, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ForMember(d => d.ProgramApplicationType, opts => opts.MapFrom(s => s.ProgramApplicationType))
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ProgramTypes))
      .ForMember(d => d.DeliveryType, opts => opts.MapFrom(s => s.DeliveryType))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
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
      .ForMember(d => d.OtherAdmissionOptions, opts => opts.MapFrom(s => s.OtherAdmissionOptions))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<Contract.ProgramApplications.ProgramCampus, ProgramCampus>()
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ApplicationStatus, ApplicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ApplicationType, ApplicationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.DeliveryType, DeliveryType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ProgramCertificationType, ResourcesProgramCertificationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<MethodofInstruction, MethodofInstruction>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<DeliveryMethodforInstructor, DeliveryMethodforInstructor>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<WorkHoursType, WorkHoursType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<AdmissionOptions, AdmissionOptions>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ComponentGroupMetadata, ComponentGroupMetadata>().ReverseMap();
    CreateMap<Contract.ProgramApplications.FileInfo, Resources.Documents.ProgramApplications.FileInfo>().ReverseMap();
    CreateMap<Contract.ProgramApplications.ProgramApplicationComponent, Resources.Documents.ProgramApplications.ProgramApplicationComponent>().ReverseMap();
    CreateMap<Contract.ProgramApplications.ComponentGroupWithComponents, Resources.Documents.ProgramApplications.ComponentGroupWithComponents>().ReverseMap();
  }
}
