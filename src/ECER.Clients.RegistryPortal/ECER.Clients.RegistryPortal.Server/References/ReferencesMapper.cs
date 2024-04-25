﻿using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.References;

public class ReferencesMapper : Profile
{
  public ReferencesMapper()
  {
    CreateMap<ReferenceContactInformation, Managers.Registry.Contract.Applications.ReferenceContactInformation>();
    CreateMap<CharacterReferenceEvaluation, Managers.Registry.Contract.Applications.CharacterReferenceEvaluation>();
    CreateMap<CharacterReferenceSubmissionRequest, Managers.Registry.Contract.Applications.CharacterReferenceSubmissionRequest>();
    CreateMap<OptOutReferenceRequest, Managers.Registry.Contract.Applications.OptOutReferenceRequest>();
  }
}
