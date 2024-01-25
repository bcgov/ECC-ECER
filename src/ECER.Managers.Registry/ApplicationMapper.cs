using AutoMapper;

namespace ECER.Managers.Registry;

internal class ApplicationMapper : Profile
{
    public ApplicationMapper()
    {
        CreateMap<CertificationApplication, Resources.Applications.CertificationApplication>()
            .ForCtorParam(nameof(Resources.Applications.CertificationApplication.Id), opts => opts.MapFrom(s => s.Id))
            .ForCtorParam(nameof(Resources.Applications.CertificationApplication.ApplicantId), opts => opts.MapFrom(s => s.RegistrantId))
            .ForCtorParam(nameof(Resources.Applications.CertificationApplication.CertificationTypes), opts => opts.MapFrom(s => s.CertificationTypes))
            .ForMember(d => d.Status, opts => opts.Ignore())
            .ForMember(d => d.CreateddOn, opts => opts.Ignore())
            .ReverseMap()
            .ValidateMemberList(MemberList.Destination)
            ;
    }
}