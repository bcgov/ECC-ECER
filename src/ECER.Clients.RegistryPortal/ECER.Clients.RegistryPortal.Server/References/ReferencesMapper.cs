using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesMapper : Profile
{
  public ReferencesMapper()
  {
    CreateMap<Managers.Registry.Contract.Applications.ReferenceContactInformation, ReferenceContactInformation>();
    CreateMap<Managers.Registry.Contract.Applications.ReferenceEvaluation, ReferenceEvaluation>();
    CreateMap<Managers.Registry.Contract.Applications.ReferenceSubmissionRequest, ReferenceSubmissionRequest>();
  }
}
