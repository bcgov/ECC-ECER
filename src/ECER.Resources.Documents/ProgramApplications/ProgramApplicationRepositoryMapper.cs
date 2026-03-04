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
      .ForMember(d => d.ecer_PostSecondaryInstituteProgramApplicaitonId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_statusreasondetail, opts => opts.MapFrom(s => s.StatusReasonDetail))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ForMember(d => d.ecer_ApplicationType, opts => opts.MapFrom(s => s.ProgramApplicationType))
      .ForMember(d => d.ecer_ProgramType, opts => opts.MapFrom(s => s.ProgramTypes))
      .ForMember(d => d.ecer_DeliveryType, opts => opts.MapFrom(s => s.DeliveryType))
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
      ;

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
    
    CreateMap<ecer_ProgramApplicationComponentGroup, ComponentGroupMetadata>(MemberList.Source)
      .ForCtorParam(nameof(ComponentGroupMetadata.Id), opt => opt.MapFrom(src => src.ecer_ProgramApplicationComponentGroupId))
      .ForCtorParam(nameof(ComponentGroupMetadata.Name), opt => opt.MapFrom(src => src.ecer_GroupName))
      .ForCtorParam(nameof(ComponentGroupMetadata.Status), opt => opt.MapFrom(src => src.ecer_EntryProgress))
      .ForCtorParam(nameof(ComponentGroupMetadata.CategoryName), opt => opt.MapFrom(src => src.ecer_categoryName))
      .ForCtorParam(nameof(ComponentGroupMetadata.DisplayOrder), opt => opt.MapFrom(src => src.ecer_DisplayOrder))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_ProgramApplicationComponentGroup, ComponentGroupResults>(MemberList.Source)
      .ForCtorParam(nameof(ComponentGroupResults.Id), opt => opt.MapFrom(src => src.ecer_ProgramApplicationComponentGroupId))
      .ForCtorParam(nameof(ComponentGroupResults.Name), opt => opt.MapFrom(src => src.ecer_GroupName))
      .ForCtorParam(nameof(ComponentGroupResults.Instruction), opt => opt.MapFrom(src => src.ecer_programapplicationcomponentgroup_ComponentGroup.ecer_Instructions))
      .ForCtorParam(nameof(ComponentGroupResults.CategoryName), opt => opt.MapFrom(src => src.ecer_categoryName))
      .ForCtorParam(nameof(ComponentGroupResults.Status), opt => opt.MapFrom(src => src.ecer_EntryProgress))
      .ForCtorParam(nameof(ComponentGroupResults.DisplayOrder), opt => opt.MapFrom(src => src.ecer_DisplayOrder))
      .ForCtorParam(nameof(ComponentGroupResults.Components), opt => opt.MapFrom(src => src.ecer_programapplicationcomponent_ComponentGroup))
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
      .ForMember(d => d.ecer_Componentanswer, opts => opts.MapFrom(s => s.Answer ?? string.Empty));
  }
}
