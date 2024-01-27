using AutoMapper;
using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationMapper : Profile
{
  public ApplicationMapper()
  {
    CreateMap<DraftApplication, CertificationApplication>()
        .ForMember(d => d.RegistrantId, opts => opts.Ignore())
        .ForMember(d => d.SubmittedOn, opts => opts.Ignore())
        ;
  }
}
