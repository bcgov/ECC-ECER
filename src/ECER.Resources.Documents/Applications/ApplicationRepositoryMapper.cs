using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Applications;

internal class ApplicationRepositoryMapper : Profile
{
  public ApplicationRepositoryMapper()
  {
    CreateMap<Application, ecer_Application>(MemberList.Source)
        .ForSourceMember(s => s.CreatedOn, opts => opts.DoNotValidate())
        .ForSourceMember(s => s.SubmittedOn, opts => opts.DoNotValidate())
        .ForMember(d => d.ecer_ApplicationId, opts => opts.MapFrom(s => s.Id))
        .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
        .ForMember(d => d.ecer_IsECEAssistant, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.EceAssistant)))
        .ForMember(d => d.ecer_isECE1YR, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.OneYear)))
        .ForMember(d => d.ecer_isECE5YR, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.FiveYears)))
        .ForMember(d => d.ecer_isITE, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.Ite)))
        .ForMember(d => d.ecer_isSNE, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.Sne)))
        .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.Stage))
        .ForSourceMember(s => s.ApplicantId, opts => opts.DoNotValidate())
        .ForSourceMember(s => s.CertificationTypes, opts => opts.DoNotValidate())
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        .ForCtorParam(nameof(Application.Id), opts => opts.MapFrom(s => s.ecer_ApplicationId!.ToString()))
        .ForCtorParam(nameof(Application.ApplicantId), opts => opts.MapFrom(s => s.ecer_Applicantid.Id.ToString()))
        .ForCtorParam(nameof(Application.CertificationTypes), opts => opts.MapFrom(s => s))
        .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
        .ForMember(d => d.SubmittedOn, opts => opts.MapFrom(s => s.ecer_DateSubmitted))
    ;

    CreateMap<ecer_Application, IEnumerable<CertificationType>>()
        .ConstructUsing((s, _) =>
        {
          var types = new List<CertificationType>();
          if (s.ecer_IsECEAssistant.GetValueOrDefault()) types.Add(CertificationType.EceAssistant);
          if (s.ecer_isECE1YR.GetValueOrDefault()) types.Add(CertificationType.OneYear);
          if (s.ecer_isECE5YR.GetValueOrDefault()) types.Add(CertificationType.FiveYears);
          if (s.ecer_isITE.GetValueOrDefault()) types.Add(CertificationType.Ite);
          if (s.ecer_isSNE.GetValueOrDefault()) types.Add(CertificationType.Sne);
          return types;
        });

    CreateMap<ApplicationStatus, ecer_Application_StatusCode>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();
  }
}
