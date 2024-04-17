using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesMapper : Profile
{
  public ReferencesMapper()
  {
    CreateMap<ReferenceContactInformation, Managers.Registry.Contract.Applications.ReferenceContactInformation>();
    CreateMap<ReferenceEvaluation, Managers.Registry.Contract.Applications.ReferenceEvaluation>();
    CreateMap<ReferenceSubmissionRequest, Managers.Registry.Contract.Applications.ReferenceSubmissionRequest>();
  }
}
