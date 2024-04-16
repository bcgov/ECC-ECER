using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesMapper : Profile
{
  public ReferencesMapper()
  {
    CreateMap<Managers.Registry.Contract.References.ReferenceContactInformation, ReferenceContactInformation>();
    CreateMap<Managers.Registry.Contract.References.ReferenceEvaluation, ReferenceEvaluation>();
    CreateMap<Managers.Registry.Contract.References.ReferenceSubmissionRequest, ReferenceSubmissionRequest>();
  }
}
