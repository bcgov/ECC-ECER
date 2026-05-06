using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ApplicationStatus = ECER.Resources.Documents.ProgramApplications.ApplicationStatus;
using NavigationMetadata = ECER.Resources.Documents.ProgramApplications.NavigationMetadata;
using ProgramCampus = ECER.Resources.Documents.ProgramApplications.ProgramCampus;
using ResourcesProgramCertificationType = ECER.Resources.Documents.ProgramApplications.ProgramCertificationType;

namespace ECER.Managers.Registry;

internal class ProgramApplicationMapper: SecureProfile
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
      .ForMember(d => d.InPersonHoursPercentage, opts => opts.MapFrom(s => s.InPersonHoursPercentage))
      .ForMember(d => d.OnlineDeliveryHoursPercentage, opts => opts.MapFrom(s => s.OnlineDeliveryHoursPercentage))
      .ForMember(d => d.ProgramCampuses, opts => opts.MapFrom(s => s.ProgramCampuses))
      .ForMember(d => d.OtherAdmissionOptions, opts => opts.MapFrom(s => s.OtherAdmissionOptions))
      .ForMember(d => d.InstituteInfoEntryProgress, opts => opts.MapFrom(s => s.InstituteInfoEntryProgress))
      .ForMember(d => d.DeclarantName, opts => opts.MapFrom(s => s.DeclarantName))
      .ForMember(d => d.DeclarantId, opts => opts.MapFrom(s => s.DeclarantId))
      .ForMember(d => d.DeclarationAccepted, opts => opts.MapFrom(s => s.DeclarationAccepted))
      .ForMember(d => d.DeclarationDate, opts => opts.MapFrom(s => s.DeclarationDate))
      .ForMember(d => d.ProgramProfileId, opts => opts.MapFrom(s => s.ProgramProfileId))
      .ForMember(d => d.ProgramProfileName, opts => opts.MapFrom(s => s.ProgramProfileName))
      .ForMember(d => d.DeclarationText, opts => opts.MapFrom(s => s.DeclarationText))
      .ForMember(d => d.BasicProgress, opts => opts.MapFrom(s => s.BasicProgress))
      .ForMember(d => d.IteProgress, opts => opts.MapFrom(s => s.IteProgress))
      .ForMember(d => d.SneProgress, opts => opts.MapFrom(s => s.SneProgress))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<Contract.ProgramApplications.ProgramCampus, ProgramCampus>()
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.ApplicationStatus, ApplicationStatus>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ApplicationType, Resources.Documents.ProgramApplications.ApplicationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<DeliveryType, Resources.Documents.ProgramApplications.DeliveryType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ProgramCertificationType, ResourcesProgramCertificationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<NavigationType, NavigationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<MethodofInstruction, Resources.Documents.ProgramApplications.MethodofInstruction>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<DeliveryMethodforInstructor, Resources.Documents.ProgramApplications.DeliveryMethodforInstructor>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<WorkHoursType, Resources.Documents.ProgramApplications.WorkHoursType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<AdmissionOptions, Resources.Documents.ProgramApplications.AdmissionOptions>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<Contract.ProgramApplications.NavigationMetadata, NavigationMetadata>().ReverseMap();
    CreateMap<Contract.ProgramApplications.FileInfo, Resources.Documents.ProgramApplications.FileInfo>().ReverseMap();
    CreateMap<ProgramApplicationComponent, Resources.Documents.ProgramApplications.ProgramApplicationComponent>().ReverseMap();
    CreateMap<ComponentGroupWithComponents, Resources.Documents.ProgramApplications.ComponentGroupWithComponents>().ReverseMap();
  }
}
