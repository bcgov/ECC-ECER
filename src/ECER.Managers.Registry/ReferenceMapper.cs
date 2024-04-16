using AutoMapper;
using ECER.Managers.Registry.Contract.References;

namespace ECER.Managers.Registry;

internal class ReferenceMapper : Profile
{
  public ReferenceMapper()
  {
    CreateMap<Resources.Documents.References.ReferenceContactInformation, ReferenceContactInformation>();
    CreateMap<Resources.Documents.References.ReferenceEvaluation, ReferenceEvaluation>();
    CreateMap<Resources.Documents.References.ReferenceSubmissionRequest, ReferenceSubmissionRequest>();
  }
}
