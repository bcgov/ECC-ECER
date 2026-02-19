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
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ForMember(d => d.ecer_ApplicationType, opts => opts.MapFrom(s => s.ProgramApplicationType))
      .ForMember(d => d.ecer_ProgramType, opts => opts.MapFrom(s => s.ProgramTypes))
      .ForMember(d => d.ecer_DeliveryType, opts => opts.MapFrom(s => s.DeliveryType))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(ProgramApplication.Id), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstituteProgramApplicaitonId.HasValue ? s.ecer_PostSecondaryInstituteProgramApplicaitonId.Value.ToString() : null))
      .ForCtorParam(nameof(ProgramApplication.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstitute.Id.ToString()))
      .ForMember(d => d.ProgramApplicationName, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.ProgramTypes, opts => opts.MapFrom(s => s.ecer_ProgramType))
      .ForMember(d => d.DeliveryType, opts => opts.MapFrom(s => s.ecer_DeliveryType))
      .ForMember(d => d.ProgramApplicationType, opts => opts.MapFrom(s => s.ecer_ApplicationType))
      .ForMember(d => d.ComponentsGenerationCompleted, opts => opts.MapFrom(s => s.ecer_ComponentsGenerationCompleted))
      ;

    CreateMap<ApplicationStatus, ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode>()
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
  }
}
