using AutoMapper;

namespace ECER.Clients.RegistryPortal.Server.Applications;

public class ApplicationMapper : Profile
{
  public ApplicationMapper()
  {
    CreateMap<Transcript, Managers.Registry.Contract.Applications.Transcript>()
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.Id),
               opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? null : src.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.EducationalInstitutionName),
               opt => opt.MapFrom(src => src.EducationalInstitutionName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.ProgramName),
               opt => opt.MapFrom(src => src.ProgramName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StudentName),
               opt => opt.MapFrom(src => src.StudentName))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StudentNumber),
               opt => opt.MapFrom(src => src.StudentNumber))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.StartDate),
               opt => opt.MapFrom(src => src.StartDate))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Transcript.EndDate),
               opt => opt.MapFrom(src => src.EndDate))
      .ForMember(d => d.CampusLocation,
             opts => opts.MapFrom(src => src.CampusLocation))
      .ForMember(d => d.LanguageofInstruction,
             opts => opts.MapFrom(src => src.LanguageofInstruction))
      .ReverseMap();

    CreateMap<DraftApplication, Managers.Registry.Contract.Applications.Application>()
      .ForMember(d => d.RegistrantId, opts => opts.Ignore())
      .ForMember(d => d.CreatedOn, opts => opts.Ignore())
      .ForMember(d => d.SubmittedOn, opts => opts.Ignore())
      .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.Transcripts))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.Id), opts => opts.MapFrom(s => s.Id))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.RegistrantId), opts => opts.MapFrom((_, ctx) => ctx.Items["registrantId"]))
      .ForCtorParam(nameof(Managers.Registry.Contract.Applications.Application.Status), opts => opts.MapFrom(_ => Managers.Registry.Contract.Applications.ApplicationStatus.Draft))
      .ForMember(d => d.Stage, opts => opts.MapFrom(s => s.Stage))
      .ForMember(d => d.SignedDate, opts => opts.MapFrom(s => s.SignedDate))
      .ForMember(d => d.CharacterReference, opts => opts.MapFrom(s => s.CharacterReference))
      ;

    CreateMap<Managers.Registry.Contract.Applications.Application, Application>()
      ;
    CreateMap<Managers.Registry.Contract.Applications.CharacterReference, CharacterReference>();
  }
}
