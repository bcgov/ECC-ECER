﻿using AutoMapper;
using ECER.Resources.Documents.Applications;

namespace ECER.Managers.Registry;

internal class ApplicationMapper : Profile
{
  public ApplicationMapper()
  {
    CreateMap<Contract.Applications.Application, Application>()
        .ForCtorParam(nameof(Application.Id), opts => opts.MapFrom(s => s.Id))
        .ForCtorParam(nameof(Application.ApplicantId), opts => opts.MapFrom(s => s.RegistrantId))
        .ForCtorParam(nameof(Application.CertificationTypes), opts => opts.MapFrom(s => s.CertificationTypes))
        .ForMember(d => d.Status, opts => opts.Ignore())
        .ForMember(d => d.CreatedOn, opts => opts.Ignore())
        .ForMember(d => d.Stage, opts => opts.MapFrom(s => s.Stage))
        .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.Transcripts))
        .ForMember(d => d.WorkExperienceReferences, opts => opts.MapFrom(s => s.WorkExperienceReferences))
        .ForMember(d => d.CharacterReferences, opts => opts.MapFrom(s => s.CharacterReferences))
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        .ForCtorParam(nameof(Contract.Applications.Application.RegistrantId), opts => opts.MapFrom(s => s.ApplicantId))
        ;

    CreateMap<Contract.Applications.Transcript, Transcript>().ReverseMap();
    CreateMap<Contract.Applications.WorkExperienceReference, WorkExperienceReference>().ReverseMap();
    CreateMap<Contract.Applications.CharacterReference, CharacterReference>().ReverseMap();
  }
}
