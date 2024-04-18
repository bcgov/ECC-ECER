using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Applications;

internal class ApplicationRepositoryMapper : Profile
{
  public ApplicationRepositoryMapper()
  {
    CreateMap<Application, ecer_Application>(MemberList.Source)
       .ForSourceMember(s => s.CreatedOn, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.SubmittedOn, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.ApplicantId, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.CertificationTypes, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.Transcripts, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.WorkExperienceReferences, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.CharacterReferences, opts => opts.DoNotValidate())
       .ForMember(d => d.ecer_ApplicationId, opts => opts.MapFrom(s => s.Id))
       .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
       .ForMember(d => d.ecer_IsECEAssistant, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.EceAssistant)))
       .ForMember(d => d.ecer_isECE1YR, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.OneYear)))
       .ForMember(d => d.ecer_isECE5YR, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.FiveYears)))
       .ForMember(d => d.ecer_isITE, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.Ite)))
       .ForMember(d => d.ecer_isSNE, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.Sne)))
       .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.SignedDate))
       .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.Stage))
       .ReverseMap()
       .ValidateMemberList(MemberList.Destination)
       .ForCtorParam(nameof(Application.Id), opts => opts.MapFrom(s => s.ecer_ApplicationId!.ToString()))
       .ForCtorParam(nameof(Application.ApplicantId), opts => opts.MapFrom(s => s.ecer_Applicantid.Id.ToString()))
       .ForCtorParam(nameof(Application.CertificationTypes), opts => opts.MapFrom(s => s))
       .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
       .ForMember(d => d.SubmittedOn, opts => opts.MapFrom(s => s.ecer_DateSubmitted))
       .ForMember(d => d.SignedDate, opts => opts.MapFrom(s => s.ecer_DateSigned))
       .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.ecer_transcript_Applicationid))
       .ForMember(d => d.WorkExperienceReferences, opts => opts.MapFrom(s => s.ecer_workexperienceref_Applicationid_ecer))
       .ForMember(d => d.CharacterReferences, opts => opts.MapFrom(s => s.ecer_characterreference_Applicationid));

    CreateMap<ecer_Application, IEnumerable<CertificationType>>()
        .ConstructUsing((s, _) =>
        {
          var types = new List<CertificationType>();
          if (s.ecer_IsECEAssistant.GetValueOrDefault()) types.Add(CertificationType.EceAssistant);
          if (s.ecer_isECE1YR.GetValueOrDefault()) types.Add(CertificationType.OneYear);
          if (s.ecer_isECE5YR.GetValueOrDefault()) types.Add(CertificationType.FiveYears);
          if (s.ecer_isITE.GetValueOrDefault()) types.Add(CertificationType.Ite);
          if (s.ecer_isSNE.GetValueOrDefault()) types.Add(CertificationType.Sne);
          return types;
        });

    CreateMap<ApplicationStatus, ecer_Application_StatusCode>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();

    CreateMap<Transcript, ecer_Transcript>(MemberList.Source)
           .ForSourceMember(s => s.StartDate, opts => opts.DoNotValidate())
           .ForSourceMember(s => s.EndDate, opts => opts.DoNotValidate())
           .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
           .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
           .ForMember(d => d.ecer_ProgramCourseName, opts => opts.MapFrom(s => s.ProgramName))
           .ForMember(d => d.ecer_CampusLocation, opts => opts.MapFrom(s => s.CampusLocation))
           .ForMember(d => d.ecer_StudentName, opts => opts.MapFrom(s => s.StudentName))
           .ForMember(d => d.ecer_StudentNumber, opts => opts.MapFrom(s => s.StudentNumber))
           .ForMember(d => d.ecer_EducationInstitutionFullName, opts => opts.MapFrom(s => s.EducationalInstitutionName))
           .ForMember(d => d.ecer_LanguageofInstruction, opts => opts.MapFrom(s => s.LanguageofInstruction))
           .ForMember(d => d.ecer_TranscriptId, opts => opts.MapFrom(s => s.Id));

    CreateMap<ecer_Transcript, Transcript>(MemberList.Source)
          .ForCtorParam(nameof(Transcript.Id), opt => opt.MapFrom(src => src.ecer_TranscriptId))
          .ForCtorParam(nameof(Transcript.EducationalInstitutionName), opt => opt.MapFrom(src => src.ecer_EducationInstitutionFullName))
          .ForCtorParam(nameof(Transcript.ProgramName), opt => opt.MapFrom(src => src.ecer_ProgramCourseName))
          .ForCtorParam(nameof(Transcript.StudentName), opt => opt.MapFrom(src => src.ecer_StudentName))
          .ForCtorParam(nameof(Transcript.StudentNumber), opt => opt.MapFrom(src => src.ecer_StudentNumber))
          .ForCtorParam(nameof(Transcript.StartDate), opt => opt.MapFrom(src => src.ecer_StartDate))
          .ForCtorParam(nameof(Transcript.EndDate), opt => opt.MapFrom(src => src.ecer_EndDate))
          .ForMember(d => d.CampusLocation, opts => opts.MapFrom(s => s.ecer_CampusLocation))
          .ForMember(d => d.LanguageofInstruction, opts => opts.MapFrom(s => s.ecer_LanguageofInstruction))
    .ValidateMemberList(MemberList.Destination);

    CreateMap<WorkExperienceReference, ecer_WorkExperienceRef>(MemberList.Source)
          .ForMember(d => d.ecer_WorkExperienceRefId, opts => opts.MapFrom(s => s.Id))
          .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
          .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
          .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.EmailAddress))
          .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
          .ForMember(d => d.ecer_TotalNumberofHoursAnticipated, opts => opts.MapFrom(s => s.Hours));

    CreateMap<ecer_WorkExperienceRef, WorkExperienceReference>(MemberList.Source)
      .ForCtorParam(nameof(WorkExperienceReference.FirstName), opt => opt.MapFrom(src => src.ecer_FirstName))
      .ForCtorParam(nameof(WorkExperienceReference.LastName), opt => opt.MapFrom(src => src.ecer_LastName))
      .ForCtorParam(nameof(WorkExperienceReference.EmailAddress), opt => opt.MapFrom(src => src.ecer_EmailAddress))
      .ForCtorParam(nameof(WorkExperienceReference.Hours), opt => opt.MapFrom(src => src.ecer_TotalNumberofHoursAnticipated))
      .ForMember(d => d.PhoneNumber, opts => opts.MapFrom(s => s.ecer_PhoneNumber))
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_WorkExperienceRefId))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<CharacterReference, ecer_CharacterReference>(MemberList.Source)
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.EmailAddress))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.ecer_CharacterReferenceId, opts => opts.MapFrom(s => s.Id));

    CreateMap<ecer_CharacterReference, CharacterReference>(MemberList.Source)
          .ForCtorParam(nameof(CharacterReference.FirstName), opt => opt.MapFrom(src => src.ecer_FirstName))
          .ForCtorParam(nameof(CharacterReference.LastName), opt => opt.MapFrom(src => src.ecer_LastName))
          .ForCtorParam(nameof(CharacterReference.EmailAddress), opt => opt.MapFrom(src => src.ecer_EmailAddress))
          .ForCtorParam(nameof(CharacterReference.PhoneNumber), opt => opt.MapFrom(src => src.ecer_PhoneNumber))
          .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_CharacterReferenceId))
          .ValidateMemberList(MemberList.Destination);

    CreateMap<ecer_Province, Province>(MemberList.Source)
      .ForCtorParam(nameof(Province.ProvinceId), opt => opt.MapFrom(src => src.ecer_ProvinceId))
      .ForCtorParam(nameof(Province.ProvinceName), opt => opt.MapFrom(src => src.ecer_Name))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ApplicationStatus, ecer_Application_StatusCode>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<CharacterReferenceSubmissionRequest, ecer_CharacterReference>(MemberList.Source)
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.ReferenceContactInformation.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.ReferenceContactInformation.LastName))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.PhoneNumber))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.ReferenceContactInformation.Email))
      .ForMember(d => d.ecer_ReferenceCertifiedProvince, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateProvinceOther))
      .ForMember(d => d.ecer_ReferenceCertificationNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateNumber))
      .ForMember(d => d.ecer_HaveObservedApplicantwithChildren, opts => opts.MapFrom(s => s.ReferenceEvaluation.WorkedWithChildren ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_KnowApplicantsTime, opts => opts.MapFrom(s => s.ReferenceEvaluation.LengthOfAcquaintance))
      .ForMember(d => d.ecer_RelationshipwithApplicant, opts => opts.MapFrom(s => s.ReferenceEvaluation.Relationship))
      .ForMember(d => d.ecer_ApplicantSituationDescription, opts => opts.MapFrom(s => s.ReferenceEvaluation.ChildInteractionObservations))
      .ForMember(d => d.ecer_ApplicantSuitableReason, opts => opts.MapFrom(s => s.ReferenceEvaluation.ApplicantTemperamentAssessment))
      .ForMember(d => d.ecer_ApplicantShouldNOTbeECE, opts => opts.MapFrom(s => s.ReferenceEvaluation.ApplicantShouldNotBeECE ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_applicantnotqualifiedreason, opts => opts.MapFrom(s => s.ReferenceEvaluation.ApplicantNotQualifiedReason))

      .ValidateMemberList(MemberList.Destination);
  }
}
