using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Transcripts;

internal class TranscriptRepositoryMapper : Profile
{
  public TranscriptRepositoryMapper()
  {
    CreateMap<Transcript, ecer_Transcript>(MemberList.Source)
   .ForSourceMember(s => s.StartDate, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.EndDate, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.ApplicantId, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.ApplicationId, opts => opts.DoNotValidate())
   .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
   .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
   .ForMember(d => d.ecer_ProgramName, opts => opts.MapFrom(s => s.ProgramName))
   .ForMember(d => d.ecer_CampusLocation, opts => opts.MapFrom(s => s.CampusLocation))
   .ForMember(d => d.ecer_StudentName, opts => opts.MapFrom(s => s.StudentNumber))
   .ForMember(d => d.ecer_StudentNumber, opts => opts.MapFrom(s => s.StudentNumber))
   .ForMember(d => d.ecer_EducationInstitutionFullName, opts => opts.MapFrom(s => s.EducationalInstitutionName))
   .ForMember(d => d.ecer_LanguageofInstruction, opts => opts.MapFrom(s => s.LanguageofInstruction))
   .ForCtorParam(nameof(Transcript.Id), opts => opts.MapFrom(s => s.Id!.ToString()))
   .ForCtorParam(nameof(Transcript.ApplicantId), opts => opts.MapFrom(s => s.ApplicantId!.ToString()))
   .ForCtorParam(nameof(Transcript.ApplicationId), opts => opts.MapFrom(s => s.ApplicationId!.ToString()))

   .ReverseMap()
   .ValidateMemberList(MemberList.Destination);
  }
}
