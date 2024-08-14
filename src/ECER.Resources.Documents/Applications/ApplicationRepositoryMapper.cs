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
       .ForSourceMember(s => s.ProfessionalDevelopments, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.WorkExperienceReferences, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.CharacterReferences, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.SubStatus, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.ReadyForAssessmentDate, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.AddMoreCharacterReference, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.AddMoreWorkExperienceReference, opts => opts.DoNotValidate())
       .ForMember(d => d.ecer_ApplicationId, opts => opts.MapFrom(s => s.Id))
       .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
       .ForMember(d => d.ecer_IsECEAssistant, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.EceAssistant)))
       .ForMember(d => d.ecer_isECE1YR, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.OneYear)))
       .ForMember(d => d.ecer_isECE5YR, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.FiveYears)))
       .ForMember(d => d.ecer_isITE, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.Ite)))
       .ForMember(d => d.ecer_isSNE, opts => opts.MapFrom(s => s.CertificationTypes.Contains(CertificationType.Sne)))
       .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.SignedDate))
       .ForMember(d => d.ecer_PortalStage, opts => opts.MapFrom(s => s.Stage))
       .ForMember(d => d.ecer_Type, opts => opts.MapFrom(s => s.ApplicationType))
       .ForMember(d => d.ecer_EducationOrigin, opts => opts.MapFrom(s => s.EducationOrigin))
       .ForMember(d => d.ecer_EducationRecognition, opts => opts.MapFrom(s => s.EducationRecognition))
       .ForMember(d => d.ecer_ExplanationLetter, opts => opts.MapFrom(s => s.ExplanationLetter))
       .ForMember(d => d.ecer_1YRExplanationChoice, opts => opts.MapFrom(s => s.OneYearRenewalexplanation))
       .ReverseMap()
       .ValidateMemberList(MemberList.Destination)
       .ForCtorParam(nameof(Application.Id), opts => opts.MapFrom(s => s.ecer_ApplicationId!.ToString()))
       .ForCtorParam(nameof(Application.ApplicantId), opts => opts.MapFrom(s => s.ecer_Applicantid.Id.ToString()))
       .ForCtorParam(nameof(Application.CertificationTypes), opts => opts.MapFrom(s => s))
       .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
       .ForMember(d => d.SubStatus, opts => opts.MapFrom(s => s.ecer_StatusReasonDetail))
       .ForMember(d => d.SubmittedOn, opts => opts.MapFrom(s => s.ecer_DateSubmitted))
       .ForMember(d => d.SignedDate, opts => opts.MapFrom(s => s.ecer_DateSigned))
       .ForMember(d => d.Transcripts, opts => opts.MapFrom(s => s.ecer_transcript_Applicationid))
       .ForMember(d => d.ProfessionalDevelopments, opts => opts.MapFrom(s => s.ecer_ecer_professionaldevelopment_Applicationi))
       .ForMember(d => d.WorkExperienceReferences, opts => opts.MapFrom(s => s.ecer_workexperienceref_Applicationid_ecer))
       .ForMember(d => d.CharacterReferences, opts => opts.MapFrom(s => s.ecer_characterreference_Applicationid))
       .ForMember(d => d.ReadyForAssessmentDate, opts => opts.MapFrom(s => s.ecer_ReadyforAssessmentDate))
       .ForMember(d => d.AddMoreCharacterReference, opts => opts.MapFrom(s => s.ecer_AddMoreCharacterReference))
       .ForMember(d => d.AddMoreWorkExperienceReference, opts => opts.MapFrom(s => s.ecer_AddMoreWorkExperienceReference));

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

    CreateMap<ApplicationStatusReasonDetail, ecer_ApplicationStatusReasonDetail>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();

    CreateMap<OneYearRenewalexplanations, ecer_yrrenewalexplanations>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();

    CreateMap<ApplicationTypes, ecer_ApplicationTypes>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<EducationOrigin, ecer_EducationOrigin>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<EducationRecognition, ecer_EducationRecognition>()
    .ConvertUsingEnumMapping(opts => opts.MapByName(true))
    .ReverseMap();

    CreateMap<Transcript, ecer_Transcript>(MemberList.Source)
           .ForSourceMember(s => s.StartDate, opts => opts.DoNotValidate())
           .ForSourceMember(s => s.EndDate, opts => opts.DoNotValidate())
           .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
           .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
           .ForMember(d => d.ecer_ProgramCourseName, opts => opts.MapFrom(s => s.ProgramName))
           .ForMember(d => d.ecer_CampusLocation, opts => opts.MapFrom(s => s.CampusLocation))
           .ForMember(d => d.ecer_StudentFirstName, opts => opts.MapFrom(s => s.StudentFirstName))
           .ForMember(d => d.ecer_StudentMiddleName, opts => opts.MapFrom(s => s.StudentMiddleName))
           .ForMember(d => d.ecer_StudentLastName, opts => opts.MapFrom(s => s.StudentLastName))
           .ForMember(d => d.ecer_StudentNumber, opts => opts.MapFrom(s => s.StudentNumber))
           .ForMember(d => d.ecer_EducationInstitutionFullName, opts => opts.MapFrom(s => s.EducationalInstitutionName))
           .ForMember(d => d.ecer_LanguageofInstruction, opts => opts.MapFrom(s => s.LanguageofInstruction))
           .ForMember(d => d.ecer_TranscriptId, opts => opts.MapFrom(s => s.Id))
           .ForMember(d => d.ecer_IsECEAssistant, opts => opts.MapFrom(s => s.IsECEAssistant))
           .ForMember(d => d.ecer_DoesECERegistryHaveTranscript, opts => opts.MapFrom(s => s.DoesECERegistryHaveTranscript))
           .ForMember(d => d.ecer_IsOfficialTranscriptRequested, opts => opts.MapFrom(s => s.IsOfficialTranscriptRequested))
           .ForMember(d => d.ecer_IsNameUnverified, opts => opts.MapFrom(s => s.IsNameUnverified))
           .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status));

    CreateMap<ecer_Transcript, Transcript>(MemberList.Source)
          .ForCtorParam(nameof(Transcript.Id), opt => opt.MapFrom(src => src.ecer_TranscriptId))
          .ForCtorParam(nameof(Transcript.EducationalInstitutionName), opt => opt.MapFrom(src => src.ecer_EducationInstitutionFullName))
          .ForCtorParam(nameof(Transcript.ProgramName), opt => opt.MapFrom(src => src.ecer_ProgramCourseName))
          .ForCtorParam(nameof(Transcript.StudentNumber), opt => opt.MapFrom(src => src.ecer_StudentNumber))
          .ForCtorParam(nameof(Transcript.StartDate), opt => opt.MapFrom(src => src.ecer_StartDate))
          .ForCtorParam(nameof(Transcript.EndDate), opt => opt.MapFrom(src => src.ecer_EndDate))
          .ForCtorParam(nameof(Transcript.IsECEAssistant), opt => opt.MapFrom(src => src.ecer_IsECEAssistant))
          .ForCtorParam(nameof(Transcript.DoesECERegistryHaveTranscript), opt => opt.MapFrom(src => src.ecer_DoesECERegistryHaveTranscript))
          .ForCtorParam(nameof(Transcript.IsOfficialTranscriptRequested), opt => opt.MapFrom(src => src.ecer_IsOfficialTranscriptRequested))
          .ForCtorParam(nameof(Transcript.StudentFirstName), opt => opt.MapFrom(src => src.ecer_StudentFirstName))
          .ForCtorParam(nameof(Transcript.StudentLastName), opt => opt.MapFrom(src => src.ecer_StudentLastName))
          .ForCtorParam(nameof(Transcript.IsNameUnverified), opt => opt.MapFrom(src => src.ecer_IsNameUnverified))
          .ForMember(d => d.CampusLocation, opts => opts.MapFrom(s => s.ecer_CampusLocation))
          .ForMember(d => d.LanguageofInstruction, opts => opts.MapFrom(s => s.ecer_LanguageofInstruction))
          .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
          .ForMember(d => d.StudentMiddleName, opts => opts.MapFrom(s => s.ecer_StudentMiddleName))
    .ValidateMemberList(MemberList.Destination);

    CreateMap<ProfessionalDevelopment, ecer_ProfessionalDevelopment>(MemberList.Source)
       .ForSourceMember(s => s.StartDate, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.EndDate, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.NewFiles, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.DeletedFiles, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.Files, opts => opts.DoNotValidate())
       .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
       .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
       .ForMember(d => d.ecer_CertifcationExpiryDate, opts => opts.MapFrom(s => s.CertificationExpiryDate))
       .ForMember(d => d.ecer_CertificationNumber, opts => opts.MapFrom(s => s.CertificationNumber))
       .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.DateSigned))
       .ForMember(d => d.ecer_CourseName, opts => opts.MapFrom(s => s.CourseName))
       .ForMember(d => d.ecer_OrganizationName, opts => opts.MapFrom(s => s.OrganizationName))
       .ForMember(d => d.ecer_HostOrganizationContactInformation, opts => opts.MapFrom(s => s.OrganizationContactInformation))
       .ForMember(d => d.ecer_InstructorName, opts => opts.MapFrom(s => s.InstructorName))
       .ForMember(d => d.ecer_NumberofHours, opts => opts.MapFrom(s => s.NumberOfHours))
       .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status));

    CreateMap<ecer_ProfessionalDevelopment, ProfessionalDevelopment>(MemberList.Source)
          .ForCtorParam(nameof(ProfessionalDevelopment.Id), opt => opt.MapFrom(src => src.ecer_ProfessionalDevelopmentId))
          .ForCtorParam(nameof(ProfessionalDevelopment.CertificationNumber), opt => opt.MapFrom(src => src.ecer_CertificationNumber))
          .ForCtorParam(nameof(ProfessionalDevelopment.CertificationExpiryDate), opt => opt.MapFrom(src => src.ecer_CertifcationExpiryDate))
          .ForCtorParam(nameof(ProfessionalDevelopment.DateSigned), opt => opt.MapFrom(src => src.ecer_DateSigned))
          .ForCtorParam(nameof(ProfessionalDevelopment.CourseName), opt => opt.MapFrom(src => src.ecer_CourseName))
          .ForCtorParam(nameof(ProfessionalDevelopment.StartDate), opt => opt.MapFrom(src => src.ecer_StartDate))
          .ForCtorParam(nameof(ProfessionalDevelopment.EndDate), opt => opt.MapFrom(src => src.ecer_EndDate))
          .ForCtorParam(nameof(ProfessionalDevelopment.OrganizationName), opt => opt.MapFrom(src => src.ecer_OrganizationName))
          .ForMember(d => d.OrganizationContactInformation, opts => opts.MapFrom(s => s.ecer_HostOrganizationContactInformation))
          .ForMember(d => d.InstructorName, opts => opts.MapFrom(s => s.ecer_InstructorName))
          .ForMember(d => d.NumberOfHours, opts => opts.MapFrom(s => s.ecer_NumberofHours))
          .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
          .ForMember(d => d.NewFiles, opts => opts.Ignore())
          .ForMember(d => d.DeletedFiles, opts => opts.Ignore())
          .ForMember(d => d.Files, opts => opts.MapFrom(src => src.ecer_bcgov_documenturl_ProfessionalDevelopmentId.Select(doc => doc.bcgov_Url).ToList()))
    .ValidateMemberList(MemberList.Destination);

    CreateMap<WorkExperienceReference, ecer_WorkExperienceRef>(MemberList.Source)
          .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
          .ForSourceMember(s => s.TotalNumberofHoursApproved, opts => opts.DoNotValidate())
          .ForSourceMember(s => s.TotalNumberofHoursObserved, opts => opts.DoNotValidate())
          .ForMember(d => d.ecer_WorkExperienceRefId, opts => opts.MapFrom(s => s.Id))
          .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
          .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
          .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.EmailAddress))
          .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
          .ForMember(d => d.ecer_TotalNumberofHoursAnticipated, opts => opts.MapFrom(s => s.Hours))
          .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status));

    CreateMap<ecer_WorkExperienceRef, WorkExperienceReference>(MemberList.Source)
      .ForCtorParam(nameof(WorkExperienceReference.FirstName), opt => opt.MapFrom(src => src.ecer_FirstName))
      .ForCtorParam(nameof(WorkExperienceReference.LastName), opt => opt.MapFrom(src => src.ecer_LastName))
      .ForCtorParam(nameof(WorkExperienceReference.EmailAddress), opt => opt.MapFrom(src => src.ecer_EmailAddress))
      .ForCtorParam(nameof(WorkExperienceReference.Hours), opt => opt.MapFrom(src => src.ecer_TotalNumberofHoursAnticipated))
      .ForMember(d => d.TotalNumberofHoursApproved, opts => opts.MapFrom(s => s.ecer_TotalNumberofHoursApproved))
      .ForMember(d => d.TotalNumberofHoursObserved, opts => opts.MapFrom(s => s.ecer_TotalNumberofHoursObserved))
      .ForMember(d => d.WillProvideReference, opts => opts.MapFrom(s => s.ecer_WillProvideReference.HasValue ? s.ecer_WillProvideReference.Equals(ecer_YesNoNull.Yes) : default(bool?)))
      .ForMember(d => d.PhoneNumber, opts => opts.MapFrom(s => s.ecer_PhoneNumber))
      .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_WorkExperienceRefId))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<CharacterReference, ecer_CharacterReference>(MemberList.Source)
      .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.LastName))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.EmailAddress))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.PhoneNumber))
      .ForMember(d => d.ecer_CharacterReferenceId, opts => opts.MapFrom(s => s.Id))
      .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status));

    CreateMap<ecer_CharacterReference, CharacterReference>(MemberList.Source)
          .ForCtorParam(nameof(CharacterReference.FirstName), opt => opt.MapFrom(src => src.ecer_FirstName))
          .ForCtorParam(nameof(CharacterReference.LastName), opt => opt.MapFrom(src => src.ecer_LastName))
          .ForCtorParam(nameof(CharacterReference.EmailAddress), opt => opt.MapFrom(src => src.ecer_EmailAddress))
          .ForCtorParam(nameof(CharacterReference.PhoneNumber), opt => opt.MapFrom(src => src.ecer_PhoneNumber))
          .ForMember(d => d.WillProvideReference, opts => opts.MapFrom(s => s.ecer_WillProvideReference.HasValue ? s.ecer_WillProvideReference.Equals(ecer_YesNoNull.Yes) : default(bool?)))
          .ForMember(d => d.Id, opts => opts.MapFrom(s => s.ecer_CharacterReferenceId))
          .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
          .ValidateMemberList(MemberList.Destination);

    CreateMap<CharacterReferenceSubmissionRequest, ecer_CharacterReference>(MemberList.Source)
      .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ReferenceContactInformation, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ReferenceEvaluation, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ConfirmProvidedInformationIsRight, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PortalInvitation, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.ReferenceContactInformation.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.ReferenceContactInformation.LastName))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.PhoneNumber))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.ReferenceContactInformation.Email))
      .ForMember(d => d.ecer_ReferenceCertifiedProvince, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateProvinceOther))
      .ForMember(d => d.ecer_ReferenceCertificationNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateNumber))
      .ForMember(d => d.ecer_HaveObservedApplicantwithChildren, opts => opts.MapFrom(s => s.ReferenceEvaluation.WorkedWithChildren ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_KnownApplicantTimeChoice, opts => opts.MapFrom(s => s.ReferenceEvaluation.LengthOfAcquaintance))
      .ForMember(d => d.ecer_RelationshiptoApplicant, opts => opts.MapFrom(s => s.ReferenceEvaluation.ReferenceRelationship))
      .ForMember(d => d.ecer_RelationshipwithApplicantOther, opts => opts.MapFrom(s => s.ReferenceEvaluation.ReferenceRelationshipOther))
      .ForMember(d => d.ecer_ApplicantSituationDescription, opts => opts.MapFrom(s => s.ReferenceEvaluation.ChildInteractionObservations))
      .ForMember(d => d.ecer_ApplicantSuitableReason, opts => opts.MapFrom(s => s.ReferenceEvaluation.ApplicantTemperamentAssessment))
      .ForMember(d => d.ecer_WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.DateSigned));

    CreateMap<WorkExperienceReferenceSubmissionRequest, ecer_WorkExperienceRef>(MemberList.Source)
      .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ReferenceContactInformation, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.WorkExperienceReferenceDetails, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.WorkExperienceReferenceCompetenciesAssessment, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.PortalInvitation, opts => opts.DoNotValidate())
      .ForSourceMember(s => s.ConfirmProvidedInformationIsRight, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_FirstName, opts => opts.MapFrom(s => s.ReferenceContactInformation.FirstName))
      .ForMember(d => d.ecer_LastName, opts => opts.MapFrom(s => s.ReferenceContactInformation.LastName))
      .ForMember(d => d.ecer_PhoneNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.PhoneNumber))
      .ForMember(d => d.ecer_EmailAddress, opts => opts.MapFrom(s => s.ReferenceContactInformation.Email))
      .ForMember(d => d.ecer_ReferenceECECertifiedProvince, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateProvinceOther))
      .ForMember(d => d.ecer_ReferenceECECertificationNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateNumber))
      .ForMember(d => d.ecer_ReferenceBirthDate, opts => opts.MapFrom(s => s.ReferenceContactInformation.DateOfBirth))
      .ForMember(d => d.ecer_TotalNumberofHoursObserved, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.Hours))
      .ForMember(d => d.ecer_WorkHoursType, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.WorkHoursType))
      .ForMember(d => d.ecer_ChildCareProgramName, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildrenProgramName))
      .ForMember(d => d.ecer_TypeofChildrenProgram, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildrenProgramType))
      .ForMember(d => d.ecer_OtherChildProgramType, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildrenProgramTypeOther))
      .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.StartDate))
      .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.EndDate))
      .ForMember(d => d.ecer_RelationshiptoApplicant, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ReferenceRelationship))
      .ForMember(d => d.ecer_RelationshiptoApplicantOther, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ReferenceRelationshipOther))
      .ForMember(d => d.ecer_CompetenceChildDevelopment, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopment))
      .ForMember(d => d.ecer_CompetenceChildDevelopmentReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.ChildDevelopmentReason))
      .ForMember(d => d.ecer_CompetenceChildGuidance, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.ChildGuidance))
      .ForMember(d => d.ecer_CompetenceChildGuidanceReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.ChildGuidanceReason))
      .ForMember(d => d.ecer_CompetenceHealthSafetyandNutrition, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutrition))
      .ForMember(d => d.ecer_CompetenceHealthSafetyandNutritionReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.HealthSafetyAndNutritionReason))
      .ForMember(d => d.ecer_CompetenceDevelopanECECurriculum, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculum))
      .ForMember(d => d.ecer_CompetenceDevelopanECECurriculumReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.DevelopAnEceCurriculumReason))
      .ForMember(d => d.ecer_CompetenceImplementanECECurriculum, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculum))
      .ForMember(d => d.ecer_CompetenceImplementECECurriculumReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.ImplementAnEceCurriculumReason))
      .ForMember(d => d.ecer_CompetenceFosteringPositiveRelationChild, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChild))
      .ForMember(d => d.ecer_CompetenceFosteringRelationChildReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationChildReason))
      .ForMember(d => d.ecer_CompetenceFosteringPositiveRelationFamily, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamily))
      .ForMember(d => d.ecer_CompetenceFosteringRelationFamilyReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationFamilyReason))
      .ForMember(d => d.ecer_CompetenceFosteringPositiveRelationCoworker, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworker))
      .ForMember(d => d.ecer_CompetenceFosteringRelationCoworkerReason, opts => opts.MapFrom(s => s.WorkExperienceReferenceCompetenciesAssessment.FosteringPositiveRelationCoworkerReason))
      .ForMember(d => d.ecer_ChildcareAgeRange, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildcareAgeRanges))
      .ForMember(d => d.ecer_WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.DateSigned));

    CreateMap<ecer_Transcript_StatusCode, TranscriptStage>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ecer_CharacterReference_StatusCode, CharacterReferenceStage>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ecer_WorkExperienceRef_StatusCode, WorkExperienceRefStage>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<UnabletoProvideReferenceReasons, ecer_UnabletoProvideReferenceReasons>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<LikertScale, ecer_likertscales>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ReferenceRelationship, ecer_ReferenceRelationships>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ChildrenProgramType, ecer_ChildrenProgramTypes>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<WorkHoursType, ecer_WorkHoursType>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();
    CreateMap<ChildcareAgeRanges, ecer_ChildcareAgeRanges>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ReferenceKnownTime, ecer_ReferenceKnownTime>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<OptOutReferenceRequest, ecer_CharacterReference>(MemberList.Source)
      .ForSourceMember(s => s.PortalInvitation, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_UnabletoProvideReferenceReason, opts => opts.MapFrom(s => s.UnabletoProvideReferenceReasons));

    CreateMap<OptOutReferenceRequest, ecer_WorkExperienceRef>(MemberList.Source)
      .ForSourceMember(s => s.PortalInvitation, opts => opts.DoNotValidate())
      .ForMember(d => d.ecer_UnabletoProvideReferenceReason, opts => opts.MapFrom(s => s.UnabletoProvideReferenceReasons));
  }
}
