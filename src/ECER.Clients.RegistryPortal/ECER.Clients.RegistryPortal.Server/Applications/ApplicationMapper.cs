using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationMapper : Profile
{
  public ApplicationMapper()
  {
    CreateMap<WorkExperienceReference, Managers.Registry.Contract.Applications.WorkExperienceReference>()
      .ForMember(d => d.Status, opts => opts.Ignore())
      .ForMember(d => d.WillProvideReference, opts => opts.Ignore())
      .ForMember(d => d.TotalNumberofHoursApproved, opts => opts.Ignore())
      .ForMember(d => d.TotalNumberofHoursObserved, opts => opts.Ignore())
      .ReverseMap();

    CreateMap<ProfessionalDevelopment, Managers.Registry.Contract.Applications.ProfessionalDevelopment>()
     .ForMember(d => d.Status, opts => opts.Ignore())
     .ForCtorParam(nameof(Managers.Registry.Contract.Applications.ProfessionalDevelopment.Id),
              opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? null : src.Id))
     .ReverseMap();

    CreateMap<Transcript, Managers.Registry.Contract.Applications.Transcript>()
      .ForMember(d => d.Status, opts => opts.Ignore())
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.Id),
               opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? null : src.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.EducationalInstitutionName),
               opt => opt.MapFrom(src => src.EducationalInstitutionName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.ProgramName),
               opt => opt.MapFrom(src => src.ProgramName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StudentFirstName),
               opt => opt.MapFrom(src => src.StudentFirstName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StudentLastName),
               opt => opt.MapFrom(src => src.StudentLastName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StudentNumber),
               opt => opt.MapFrom(src => src.StudentNumber))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StartDate),
               opt => opt.MapFrom(src => src.StartDate))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.EndDate),
               opt => opt.MapFrom(src => src.EndDate))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.IsECEAssistant),
               opt => opt.MapFrom(src => src.IsECEAssistant))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.DoesECERegistryHaveTranscript),
               opt => opt.MapFrom(src => src.DoesECERegistryHaveTranscript))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.IsOfficialTranscriptRequested),
               opt => opt.MapFrom(src => src.IsOfficialTranscriptRequested))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.IsNameUnverified),
         opt => opt.MapFrom(src => src.IsNameUnverified))
      .ForMember(d => d.CampusLocation,
             opts => opts.MapFrom(src => src.CampusLocation))
      .ForMember(d => d.LanguageofInstruction,
             opts => opts.MapFrom(src => src.LanguageofInstruction))
      .ForMember(d => d.StudentMiddleName,
       opts => opts.MapFrom(src => src.StudentMiddleName))
      .ReverseMap();

    CreateMap<CharacterReference, Managers.Registry.Contract.Applications.CharacterReference>()
      .ForMember(d => d.Status, opts => opts.Ignore())
      .ForMember(d => d.WillProvideReference, opts => opts.Ignore())
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.CharacterReference.FirstName),
        opt => opt.MapFrom(src => src.FirstName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.CharacterReference.LastName),
        opt => opt.MapFrom(src => src.LastName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.CharacterReference.EmailAddress),
        opt => opt.MapFrom(src => src.EmailAddress))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.CharacterReference.PhoneNumber),
        opt => opt.MapFrom(src => src.PhoneNumber))
      .ForMember(d => d.Id,
        opts => opts.MapFrom(src => string.IsNullOrEmpty(src.Id) ? null : src.Id))
      .ReverseMap();

    CreateMap<DraftApplication, Managers.Registry.Contract.Applications.Application>()
      .ForMember(d => d.RegistrantId, opts => opts.Ignore())
      .ForMember(d => d.CreatedOn, opts => opts.Ignore())
      .ForMember(d => d.SubmittedOn, opts => opts.Ignore())
      .ForMember(d => d.SubStatus, opts => opts.Ignore())
      .ForMember(d => d.ReadyForAssessmentDate, opts => opts.Ignore())
      .ForMember(d => d.AddMoreCharacterReference, opts => opts.Ignore())
      .ForMember(d => d.AddMoreWorkExperienceReference, opts => opts.Ignore())
      .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.Transcripts))
      .ForMember(d => d.ExplanationLetter, opts => opts.MapFrom(s => s.ExplanationLetter))
      .ForMember(d => d.WorkExperienceReferences, opts => opts.MapFrom(s => s.WorkExperienceReferences))
      .ForMember(d => d.ProfessionalDevelopments, opts => opts.MapFrom(s => s.ProfessionalDevelopments))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.RegistrantId), opts => opts.MapFrom((_, ctx) => ctx.Items["registrantId"]))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.Status), opts => opts.MapFrom(_ => Managers.Registry.Contract.Applications.ApplicationStatus.Draft))
      .ForMember(d => d.Stage, opts => opts.MapFrom(s => s.Stage))
      .ForMember(d => d.OneYearRenewalexplanation, opts => opts.MapFrom(s => s.OneYearRenewalexplanation))
      .ForMember(d => d.SignedDate, opts => opts.MapFrom(s => s.SignedDate))
      .ForMember(d => d.CharacterReferences, opts => opts.MapFrom(s => s.CharacterReferences))
      ;

    CreateMap<Managers.Registry.Contract.Applications.Application, Application>();
    CreateMap<Managers.Registry.Contract.Applications.FileInfo, FileInfo>().ReverseMap();

    CreateMap<Managers.Registry.Contract.Applications.Application, SubmittedApplicationStatus>()
      .ForCtorParam(nameof(SubmittedApplicationStatus.Id),
        opt => opt.MapFrom(s => s.Id))
      .ForCtorParam(nameof(SubmittedApplicationStatus.SubmittedOn),
        opt => opt.MapFrom(s => s.SubmittedOn))
      .ForCtorParam(nameof(SubmittedApplicationStatus.Status),
        opt => opt.MapFrom(s => s.Status))
      .ForCtorParam(nameof(SubmittedApplicationStatus.SubStatus),
        opt => opt.MapFrom(s => s.SubStatus))
      .ForMember(d => d.CertificationTypes, opts => opts.MapFrom(s => s.CertificationTypes))
      .ForMember(d => d.ReadyForAssessmentDate, opts => opts.MapFrom(s => s.ReadyForAssessmentDate))
      .ForMember(d => d.TranscriptsStatus, opts => opts.MapFrom(s => s.Transcripts))
      .ForMember(d => d.WorkExperienceReferencesStatus, opts => opts.MapFrom(s => s.WorkExperienceReferences))
      .ForMember(d => d.CharacterReferencesStatus, opts => opts.MapFrom(s => s.CharacterReferences))
      .ForMember(d => d.AddMoreCharacterReference, opts => opts.MapFrom(s => s.AddMoreCharacterReference))
      .ForMember(d => d.AddMoreWorkExperienceReference, opts => opts.MapFrom(s => s.AddMoreWorkExperienceReference))
      .ForMember(d => d.ApplicationType, opts => opts.MapFrom(s => s.ApplicationType));

    CreateMap<Managers.Registry.Contract.Applications.CharacterReference, CharacterReferenceStatus>()
      .ForCtorParam(nameof(CharacterReferenceStatus.FirstName),
        opt => opt.MapFrom(src => src.FirstName))
      .ForCtorParam(nameof(CharacterReferenceStatus.LastName),
        opt => opt.MapFrom(src => src.LastName))
      .ForCtorParam(nameof(CharacterReferenceStatus.EmailAddress),
        opt => opt.MapFrom(src => src.EmailAddress))
      .ForCtorParam(nameof(CharacterReferenceStatus.Id),
        opt => opt.MapFrom(src => src.Id))
      .ForCtorParam(nameof(CharacterReferenceStatus.Status),
        opt => opt.MapFrom(src => src.Status))
      .ForMember(d => d.PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference));

    CreateMap<Managers.Registry.Contract.Applications.WorkExperienceReference, WorkExperienceReferenceStatus>()
      .ForCtorParam(nameof(WorkExperienceReferenceStatus.FirstName),
        opt => opt.MapFrom(src => src.FirstName))
      .ForCtorParam(nameof(WorkExperienceReferenceStatus.LastName),
        opt => opt.MapFrom(src => src.LastName))
      .ForCtorParam(nameof(WorkExperienceReferenceStatus.EmailAddress),
        opt => opt.MapFrom(src => src.EmailAddress))
      .ForCtorParam(nameof(WorkExperienceReferenceStatus.Id),
        opt => opt.MapFrom(src => src.Id))
      .ForCtorParam(nameof(WorkExperienceReferenceStatus.Status),
        opt => opt.MapFrom(src => src.Status))
      .ForMember(d => d.PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.TotalNumberofHoursAnticipated, opts => opts.MapFrom(s => s.Hours))
      .ForMember(d => d.TotalNumberofHoursApproved, opts => opts.MapFrom(s => s.TotalNumberofHoursApproved))
      .ForMember(d => d.TotalNumberofHoursObserved, opts => opts.MapFrom(s => s.TotalNumberofHoursObserved))
      .ForMember(d => d.WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference))
      .ForMember(d => d.Type, opts => opts.MapFrom(s => s.Type));

    CreateMap<Managers.Registry.Contract.Applications.Transcript, TranscriptStatus>()
      .ForCtorParam(nameof(TranscriptStatus.Id),
        opt => opt.MapFrom(src => src.Id))
      .ForCtorParam(nameof(TranscriptStatus.EducationalInstitutionName),
        opt => opt.MapFrom(src => src.EducationalInstitutionName))
      .ForCtorParam(nameof(TranscriptStatus.Status),
        opt => opt.MapFrom(src => src.Status));
  }
}
