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
        .ForMember(d => d.OneYearRenewalExplanationChoice, opts => opts.MapFrom(s => s.OneYearRenewalExplanationChoice))
        .ForMember(d => d.FiveYearRenewalExplanationChoice, opts => opts.MapFrom(s => s.FiveYearRenewalExplanationChoice))
        .ForMember(d => d.RenewalExplanationOther, opts => opts.MapFrom(s => s.RenewalExplanationOther))
        .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.Transcripts))
        .ForMember(d => d.ProfessionalDevelopments, opts => opts.MapFrom(s => s.ProfessionalDevelopments))
        .ForMember(d => d.WorkExperienceReferences, opts => opts.MapFrom(s => s.WorkExperienceReferences))
        .ForMember(d => d.CharacterReferences, opts => opts.MapFrom(s => s.CharacterReferences))
        .ReverseMap()
        .ValidateMemberList(MemberList.Destination)
        .ForCtorParam(nameof(Contract.Applications.Application.RegistrantId), opts => opts.MapFrom(s => s.ApplicantId))
        ;
    CreateMap<Contract.Applications.CertificateInformation, CertificateInformation>().ReverseMap();
    CreateMap<Contract.Applications.Transcript, Transcript>().ReverseMap();
    CreateMap<Contract.Applications.TranscriptDocuments, TranscriptDocuments>().ReverseMap();
    CreateMap<Contract.Applications.ProfessionalDevelopment, ProfessionalDevelopment>().ReverseMap();
    CreateMap<Contract.Applications.WorkExperienceReference, WorkExperienceReference>().ReverseMap();
    CreateMap<Contract.Applications.CharacterReference, CharacterReference>().ReverseMap();
    
    CreateMap<Contract.Applications.ReferenceContactInformation, ReferenceContactInformation>();
    CreateMap<Contract.Applications.CharacterReferenceEvaluation, CharacterReferenceEvaluation>();
    CreateMap<Contract.Applications.CharacterReferenceSubmissionRequest, CharacterReferenceSubmissionRequest>()
      .ForMember(d => d.PortalInvitation, opts => opts.Ignore())
      .ForMember(d => d.DateSigned, opts => opts.Ignore());

    CreateMap<Contract.Applications.WorkExperienceReferenceDetails, WorkExperienceReferenceDetails>();
    CreateMap<Contract.Applications.WorkExperienceReferenceCompetenciesAssessment, WorkExperienceReferenceCompetenciesAssessment>();
    CreateMap<Contract.Applications.WorkExperienceReferenceSubmissionRequest, WorkExperienceReferenceSubmissionRequest>()
      .ForMember(d => d.PortalInvitation, opts => opts.Ignore())
      .ForMember(d => d.DateSigned, opts => opts.Ignore());

    CreateMap<ECER.Resources.Documents.Applications.FileInfo, ECER.Managers.Registry.Contract.Applications.FileInfo>().ReverseMap();

    CreateMap<Contract.Applications.OptOutReferenceRequest, OptOutReferenceRequest>()
      .ForMember(d => d.PortalInvitation, opts => opts.Ignore());
  }
}
