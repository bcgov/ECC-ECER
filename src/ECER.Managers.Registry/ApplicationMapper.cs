using AutoMapper;
using ECER.Resources.Applications;

namespace ECER.Managers.Registry;

internal class ApplicationMapper : Profile
{
  public ApplicationMapper()
  {
    CreateMap<Contract.CertificationApplication, CertificationApplication>()
        .ForCtorParam(nameof(CertificationApplication.Id), opts => opts.MapFrom(s => s.Id))
        .ForCtorParam(nameof(CertificationApplication.ApplicantId), opts => opts.MapFrom(s => s.RegistrantId))
        .ForCtorParam(nameof(CertificationApplication.CertificationTypes), opts => opts.MapFrom(s => s.CertificationTypes))
        .ForMember(d => d.Status, opts => opts.Ignore())
        .ForMember(d => d.CreateddOn, opts => opts.Ignore())
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        ;
  }
}
