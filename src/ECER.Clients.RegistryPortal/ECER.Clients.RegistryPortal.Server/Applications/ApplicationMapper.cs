using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationMapper : Profile
{
  public ApplicationMapper()
  {
    CreateMap<DraftApplication, Managers.Registry.Contract.Applications.Application>()
      .ForMember(d => d.RegistrantId, opts => opts.Ignore())
      .ForMember(d => d.CreatedOn, opts => opts.Ignore())
      .ForMember(d => d.SubmittedOn, opts => opts.Ignore())
      .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.Transcripts))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.RegistrantId), opts => opts.MapFrom((_, ctx) => ctx.Items["registrantId"]))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.Status), opts => opts.MapFrom(_ => Managers.Registry.Contract.Applications.ApplicationStatus.Draft))
      ;

    CreateMap<Managers.Registry.Contract.Applications.Application, Application>()
      ;

    CreateMap<Managers.Registry.Contract.Applications.Transcript, Transcript>()
      ;
  }
}
