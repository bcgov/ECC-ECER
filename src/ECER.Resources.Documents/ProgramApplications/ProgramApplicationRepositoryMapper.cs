using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.ProgramApplications;

internal class ProgramApplicationRepositoryMapper : Profile
{

  public ProgramApplicationRepositoryMapper()
  {
    CreateMap<ProgramApplication, ecer_PostSecondaryInstituteProgramApplicaiton>(MemberList.Source)
      .ForSourceMember(s => s.PostSecondaryInstituteId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ComponentsGenerationCompleted, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ProgramRepresentativeId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ProgramCampuses, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.DeliveryType, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ProgramTypes, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.InstituteInfoEntryProgress, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_PostSecondaryInstituteProgramApplicaitonId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_statusreasondetail, opts => opts.MapFrom(s => s.StatusReasonDetail))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ForMember(d => d.ecer_ApplicationType, opts => opts.MapFrom(s => s.ProgramApplicationType))
      .ForMember(d => d.ecer_ProjectedLength, opts => opts.MapFrom(s => s.ProgramLength))
      .ForMember(d => d.ecer_Onlinemethodsofinstruction, opts => opts.MapFrom(s => s.OnlineMethodOfInstruction))
      .ForMember(d => d.ecer_Deliverymethodforpracticuminstructor, opts => opts.MapFrom(s => s.DeliveryMethod))
      .ForMember(d => d.ecer_ProgramEnrollment, opts => opts.MapFrom(s => s.EnrollmentOptions))
      .ForMember(d => d.ecer_AdmissionOptions, opts => opts.MapFrom(s => s.AdmissionOptions))
      .ForMember(d => d.ecer_MinimumStudentEnrollmentperCourse, opts => opts.MapFrom(s => s.MinimumEnrollment))
      .ForMember(d => d.ecer_MaximumStudentEnrollmentperCourse, opts => opts.MapFrom(s => s.MaximumEnrollment))
      .ForMember(d => d.ecer_OtherAdmissionOptions, opts => opts.MapFrom(s => s.OtherAdmissionOptions))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(ProgramApplication.Id), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstituteProgramApplicaitonId.HasValue ? s.ecer_PostSecondaryInstituteProgramApplicaitonId.Value.ToString() : null))
      .ForCtorParam(nameof(ProgramApplication.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstitute.Id.ToString()))
      .ForMember(d => d.ProgramApplicationName, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.StatusReasonDetail, opts => opts.MapFrom(s => s.ecer_statusreasondetail))
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ecer_ProgramType))
      .ForMember(d => d.DeliveryType, opts => opts.MapFrom(s => s.ecer_DeliveryType))
      .ForMember(d => d.ProgramApplicationType, opts => opts.MapFrom(s => s.ecer_ApplicationType))
      .ForMember(d => d.ComponentsGenerationCompleted, opts => opts.MapFrom(s => s.ecer_ComponentsGenerationCompleted))
      .ForMember(d => d.ProgramRepresentativeId, opts => opts.MapFrom(s => s.ecer_PSIProgramRepresentative == null ? null : s.ecer_PSIProgramRepresentative.Id.ToString()))
      .ForMember(d => d.ProgramLength, opts => opts.MapFrom(s => s.ecer_ProjectedLength))
      .ForMember(d => d.OnlineMethodOfInstruction, opts => opts.MapFrom(s => s.ecer_Onlinemethodsofinstruction))
      .ForMember(d => d.DeliveryMethod, opts => opts.MapFrom(s => s.ecer_Deliverymethodforpracticuminstructor))
      .ForMember(d => d.EnrollmentOptions, opts => opts.MapFrom(s => s.ecer_ProgramEnrollment))
      .ForMember(d => d.AdmissionOptions, opts => opts.MapFrom(s => s.ecer_AdmissionOptions))
      .ForMember(d => d.MinimumEnrollment, opts => opts.MapFrom(s => s.ecer_MinimumStudentEnrollmentperCourse))
      .ForMember(d => d.MaximumEnrollment, opts => opts.MapFrom(s => s.ecer_MaximumStudentEnrollmentperCourse))
      .ForMember(d => d.ProgramCampuses, opts => opts.MapFrom(s => s.ecer_ProgramApplicationId_ecer_postsecondaryinstituteprogramapplicaiton))
      .ForMember(d => d.OtherAdmissionOptions, opts => opts.MapFrom(s => s.ecer_OtherAdmissionOptions))
      .ForMember(d => d.InstituteInfoEntryProgress, opts => opts.MapFrom(s => s.ecer_InstitutionProgramInformationEntryProgress))
      ;
    
    CreateMap<ecer_ProgramCampus, ProgramCampus>(MemberList.Destination)
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.CampusId, opts => opts.MapFrom(s => s.ecer_CampusId.Id))
      .ReverseMap();

    CreateMap<ApplicationStatus, ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<ApplicationStatusReasonDetail, ecer_Statusreasondetail>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<ApplicationType, ecer_PSIApplicationType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<DeliveryType, ecer_PSIDeliveryType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<ProgramCertificationType, ecer_PSIProgramType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<MethodofInstruction, ecer_PSPMethodofInstruction>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<DeliveryMethodforInstructor, ecer_PSPDeliveryMethodforInstructor>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<WorkHoursType, ecer_WorkHoursType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<AdmissionOptions, ecer_PSPAdmissionOptions>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    
    CreateMap<ecer_ProgramApplicationComponentGroup, NavigationMetadata>(MemberList.Source)
      .ForCtorParam(nameof(NavigationMetadata.Id), opt => opt.MapFrom(src => src.ecer_ProgramApplicationComponentGroupId))
      .ForCtorParam(nameof(NavigationMetadata.Name), opt => opt.MapFrom(src => src.ecer_GroupName))
      .ForCtorParam(nameof(NavigationMetadata.Status), opt => opt.MapFrom(src => src.ecer_EntryProgress))
      .ForCtorParam(nameof(NavigationMetadata.CategoryName), opt => opt.MapFrom(src => src.ecer_categoryName))
      .ForCtorParam(nameof(NavigationMetadata.DisplayOrder), opt => opt.MapFrom(src => src.ecer_DisplayOrder))
      .ForCtorParam(nameof(NavigationMetadata.NavigationType), opt => opt.MapFrom(_ => NavigationType.Component))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_ProgramApplicationComponentGroup, ComponentGroupWithComponents>(MemberList.Source)
      .ForCtorParam(nameof(ComponentGroupWithComponents.Id), opt => opt.MapFrom(src => src.ecer_ProgramApplicationComponentGroupId))
      .ForCtorParam(nameof(ComponentGroupWithComponents.Name), opt => opt.MapFrom(src => src.ecer_GroupName))
      .ForCtorParam(nameof(ComponentGroupWithComponents.Instruction), opt => opt.MapFrom(src => src.ecer_programapplicationcomponentgroup_ComponentGroup.ecer_Instructions))
      .ForCtorParam(nameof(ComponentGroupWithComponents.CategoryName), opt => opt.MapFrom(src => src.ecer_categoryName))
      .ForCtorParam(nameof(ComponentGroupWithComponents.Status), opt => opt.MapFrom(src => src.ecer_EntryProgress))
      .ForCtorParam(nameof(ComponentGroupWithComponents.DisplayOrder), opt => opt.MapFrom(src => src.ecer_DisplayOrder))
      .ForCtorParam(nameof(ComponentGroupWithComponents.Components), opt => opt.MapFrom(src => src.ecer_programapplicationcomponent_ComponentGroup))
      .ValidateMemberList(MemberList.Destination);
    
    CreateMap<ecer_ProgramApplicationComponent, ProgramApplicationComponent>(MemberList.Source)
      .ForCtorParam(nameof(ProgramApplicationComponent.Id), opt => opt.MapFrom(src => src.ecer_ProgramApplicationComponentId.HasValue ? src.ecer_ProgramApplicationComponentId.Value.ToString() : string.Empty))
      .ForCtorParam(nameof(ProgramApplicationComponent.Name), opt => opt.MapFrom(src => src.ecer_Component))
      .ForCtorParam(nameof(ProgramApplicationComponent.Question), opt => opt.MapFrom(src => src.ecer_Question))
      .ForCtorParam(nameof(ProgramApplicationComponent.DisplayOrder), opt => opt.MapFrom(src => src.ecer_DisplayOrder))
      .ForCtorParam(nameof(ProgramApplicationComponent.Answer), opt => opt.MapFrom(src => src.ecer_Componentanswer))
      .ForCtorParam(nameof(ProgramApplicationComponent.Files), opt => opt.MapFrom(src => src.ecer_documenturl_ProgramApplicationComponentId))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<bcgov_DocumentUrl, FileInfo>(MemberList.Destination)
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.bcgov_DocumentUrlId))
      .ForMember(d => d.Name, opts => opts.MapFrom(s => s.bcgov_FileName))
      .ForMember(d => d.Url, opts => opts.MapFrom(s => s.bcgov_Url))
      .ForMember(d => d.Size, opts => opts.MapFrom(s => s.bcgov_FileSize))
      .ForMember(d => d.Extension, opts => opts.MapFrom(s => s.bcgov_FileExtension));

    CreateMap<ProgramApplicationComponent, ecer_ProgramApplicationComponent>(MemberList.Source)
      .ForSourceMember(s => s.Name, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Question, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.DisplayOrder, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.Files, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_ProgramApplicationComponentId, opts => opts.MapFrom(s => Guid.Parse(s.Id)))
      .ForMember(d => d.ecer_Componentanswer, opts => opts.MapFrom(s => s.Answer));
  }
}
