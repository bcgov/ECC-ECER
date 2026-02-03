using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.ProgramApplications;

internal class ProgramApplicationRepositoryMapper : Profile
{
  public ProgramApplicationRepositoryMapper()
  {
    CreateMap<ProgramApplication, ecer_PostSecondaryInstituteProgramApplicaiton>(MemberList.Source)
      .ForSourceMember(s => s.PostSecondaryInstituteId, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.DeliveryType, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ProgramType, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ProgramApplicationType, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_PostSecondaryInstituteProgramApplicaitonId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
      .ForMember(d => d.ecer_Name, opts => opts.MapFrom(s => s.ProgramApplicationName))
      .ReverseMap()
      .ValidateMemberList(MemberList.Destination)
      .ForCtorParam(nameof(ProgramApplication.Id), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstituteProgramApplicaitonId.HasValue ? s.ecer_PostSecondaryInstituteProgramApplicaitonId.Value.ToString() : null))
      .ForCtorParam(nameof(ProgramApplication.PostSecondaryInstituteId), opts => opts.MapFrom(s => s.ecer_PostSecondaryInstitute.Id.ToString()))
      .ForMember(d => d.ProgramApplicationName, opts => opts.MapFrom(s => s.ecer_Name))
      .ForMember(d => d.ProgramType, opts => opts.MapFrom(s => s.ecer_ProvincialCertificationTypeOffered ))
      .ForMember(d => d.DeliveryType, opts => opts.MapFrom(s => s.ecer_DeliveryType ))
      .ForMember(d => d.ProgramApplicationType, opts => opts.MapFrom(s => s.ecer_ApplicationType ))
      ;
  }
}
