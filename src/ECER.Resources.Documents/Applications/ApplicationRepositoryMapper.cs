using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Resources.Documents.MetadataResources;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;

namespace ECER.Resources.Documents.Applications;

internal class ApplicationRepositoryMapper : Profile
{
  public ApplicationRepositoryMapper()
  {
    CreateMap<Application, ecer_Application>(MemberList.Source)
       .ForSourceMember(s => s.LabourMobilityCertificateInformation, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.CreatedOn, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.FromCertificate, opts => opts.DoNotValidate())
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
       .ForSourceMember(s => s.AddMoreProfessionalDevelopment, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.Origin, opts => opts.DoNotValidate())
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
       .ForMember(d => d.ecer_1YRExplanationChoice, opts => opts.MapFrom(s => s.OneYearRenewalExplanationChoice))
       .ForMember(d => d.ecer_5YRExplanationChoice, opts => opts.MapFrom(s => s.FiveYearRenewalExplanationChoice))
       .ForMember(d => d.ecer_RenewalExplanationOther, opts => opts.MapFrom(s => s.RenewalExplanationOther))
       .ForMember(d => d.ecer_CurrentCertificationNumber, opts => opts.MapFrom(s => (s.LabourMobilityCertificateInformation != null) ? s.LabourMobilityCertificateInformation.CurrentCertificationNumber : string.Empty))
       .ForMember(d => d.ecer_lmcerthasothername, opts => opts.MapFrom(s => (s.LabourMobilityCertificateInformation != null && s.LabourMobilityCertificateInformation.HasOtherName == true) ? true : false))
       .ForMember(d => d.ecer_LegalFirstName, opts => opts.MapFrom(s => (s.LabourMobilityCertificateInformation != null) ? s.LabourMobilityCertificateInformation.LegalFirstName : string.Empty))
       .ForMember(d => d.ecer_LegalMiddleName, opts => opts.MapFrom(s => (s.LabourMobilityCertificateInformation != null) ? s.LabourMobilityCertificateInformation.LegalMiddleName : string.Empty))
       .ForMember(d => d.ecer_LegalLastName, opts => opts.MapFrom(s => (s.LabourMobilityCertificateInformation != null) ? s.LabourMobilityCertificateInformation.LegalLastName : string.Empty))
       .ReverseMap()
       .ValidateMemberList(MemberList.Destination)
       .ForCtorParam(nameof(Application.Id), opts => opts.MapFrom(s => s.ecer_ApplicationId!.ToString()))
       .ForCtorParam(nameof(Application.ApplicantId), opts => opts.MapFrom(s => s.ecer_Applicantid.Id.ToString()))
       .ForCtorParam(nameof(Application.CertificationTypes), opts => opts.MapFrom(s => s))
       .ForMember(d => d.FromCertificate, opts => opts.MapFrom(s => IdOrEmpty(s.ecer_FromCertificateId)))
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
       .ForMember(d => d.AddMoreWorkExperienceReference, opts => opts.MapFrom(s => s.ecer_AddMoreWorkExperienceReference))
       .ForMember(d => d.AddMoreProfessionalDevelopment, opts => opts.MapFrom(s => s.ecer_AddMoreProfessionalDevelopment))
       .ForMember(d => d.Origin, opts => opts.MapFrom(s => s.ecer_Origin))
       .ForMember(d => d.LabourMobilityCertificateInformation, o => o.MapFrom(s => s))
       .ForMember(d => d.Stage, opts => opts.MapFrom(s => string.IsNullOrEmpty(s.ecer_PortalStage) ? "ContactInformation" : s.ecer_PortalStage));

    CreateMap<ecer_Application, CertificateInformation>()
    .ForMember(d => d.CertificateComparisonId,
        o => o.MapFrom(s =>
            s.ecer_application_certificationcomparisonid.ecer_certificationcomparisonId.ToString()
            ?? string.Empty))
    .ForMember(d => d.ExistingCertificationType,
        o => o.MapFrom(s =>
            s.ecer_application_certificationcomparisonid.ecer_transferringcertificateName
            ?? string.Empty))
    .ForMember(d => d.CurrentCertificationNumber, o => o.MapFrom(s => s.ecer_CurrentCertificationNumber))
    .ForMember(d => d.HasOtherName, o => o.MapFrom(s => s.ecer_lmcerthasothername))
    .ForMember(d => d.LegalFirstName, o => o.MapFrom(s => s.ecer_LegalFirstName))
    .ForMember(d => d.LegalMiddleName, o => o.MapFrom(s => s.ecer_LegalMiddleName))
    .ForMember(d => d.LegalLastName, o => o.MapFrom(s => s.ecer_LegalLastName))
    .ForMember(d => d.LabourMobilityProvince, o => o.MapFrom(s => s.ecer_application_lmprovinceid));

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

    CreateMap<ApplicationOrigin, ecer_Origin>()
        .ConvertUsingEnumMapping(opts => opts.MapByName(true))
        .ReverseMap();

    CreateMap<ApplicationStatus, ecer_Application_StatusCode>()
        .ConvertUsing(status =>
            status == ApplicationStatus.Draft ? ecer_Application_StatusCode.Draft :
            status == ApplicationStatus.Submitted ? ecer_Application_StatusCode.Submitted :
            status == ApplicationStatus.Complete ? ecer_Application_StatusCode.Complete :
            status == ApplicationStatus.Closed ? ecer_Application_StatusCode.Closed :
            status == ApplicationStatus.Reconsideration ? ecer_Application_StatusCode.Reconsideration :
            status == ApplicationStatus.Escalated ? ecer_Application_StatusCode.Escalated :
            status == ApplicationStatus.Decision ? ecer_Application_StatusCode.Decision :
            status == ApplicationStatus.Cancelled ? ecer_Application_StatusCode.Withdrawn :
            status == ApplicationStatus.Pending ? ecer_Application_StatusCode.Pending :
            status == ApplicationStatus.Ready ? ecer_Application_StatusCode.Ready :
            status == ApplicationStatus.InProgress ? ecer_Application_StatusCode.InProgress :
            status == ApplicationStatus.PendingQueue ? ecer_Application_StatusCode.PendingQueue :
            status == ApplicationStatus.PendingPSPConsultationNeeded ? ecer_Application_StatusCode.PendingPSPConsultationNeeded :
            status == ApplicationStatus.ReconsiderationDecision ? ecer_Application_StatusCode.ReconsiderationDecision :
            status == ApplicationStatus.AppealDecision ? ecer_Application_StatusCode.AppealDecision :
            ecer_Application_StatusCode.Draft);


    CreateMap<ecer_Application_StatusCode, ApplicationStatus>()
        .ConvertUsing(status =>
            status == ecer_Application_StatusCode.Draft ? ApplicationStatus.Draft :
            status == ecer_Application_StatusCode.Submitted ? ApplicationStatus.Submitted :
            status == ecer_Application_StatusCode.Complete ? ApplicationStatus.Complete :
            status == ecer_Application_StatusCode.Closed ? ApplicationStatus.Closed :
            status == ecer_Application_StatusCode.Reconsideration ? ApplicationStatus.Reconsideration :
            status == ecer_Application_StatusCode.Escalated ? ApplicationStatus.Escalated :
            status == ecer_Application_StatusCode.Decision ? ApplicationStatus.Decision :
            status == ecer_Application_StatusCode.Withdrawn ? ApplicationStatus.Cancelled :
            status == ecer_Application_StatusCode.Pending ? ApplicationStatus.Pending :
            status == ecer_Application_StatusCode.Ready ? ApplicationStatus.Ready :
            status == ecer_Application_StatusCode.InProgress ? ApplicationStatus.InProgress :
            status == ecer_Application_StatusCode.PendingQueue ? ApplicationStatus.PendingQueue :
            status == ecer_Application_StatusCode.PendingPSPConsultationNeeded ? ApplicationStatus.PendingPSPConsultationNeeded :
            status == ecer_Application_StatusCode.ReconsiderationDecision ? ApplicationStatus.ReconsiderationDecision :
            status == ecer_Application_StatusCode.AppealDecision ? ApplicationStatus.AppealDecision :
            status == ecer_Application_StatusCode.NotSubmitted ? ApplicationStatus.Draft :
            ApplicationStatus.Draft);


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
           .ForMember(d => d.ecer_TranscriptId, opts => opts.MapFrom(s => s.Id))
           .ForMember(d => d.ecer_IsECEAssistant, opts => opts.MapFrom(s => s.IsECEAssistant))
           //next 3 lines converts enum TranscriptStatusOption to the 3 boolean fields within dynamics
           .ForMember(d => d.ecer_DoesECERegistryHaveTranscript, opts => opts.MapFrom(s => s.TranscriptStatusOption == TranscriptStatusOptions.RegistryHasTranscript))
           .ForMember(d => d.ecer_IsOfficialTranscriptRequested, opts => opts.MapFrom(s => s.TranscriptStatusOption == TranscriptStatusOptions.OfficialTranscriptRequested))
           .ForMember(d => d.ecer_mytranscriptwillrequireenglishtranslation, opts => opts.MapFrom(s => s.TranscriptStatusOption == TranscriptStatusOptions.TranscriptWillRequireEnglishTranslation))
           .ForMember(d => d.ecer_IsNameUnverified, opts => opts.MapFrom(s => s.IsNameUnverified))
           .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
           .ForMember(d => d.ecer_EducationRecognition, opts => opts.MapFrom(s => s.EducationRecognition))
           .ForMember(d => d.ecer_EducationOrigin, opts => opts.MapFrom(s => s.EducationOrigin))
           .ForMember(d => d.ecer_transcript_InstituteCountryId, opts => opts.MapFrom(s => s.Country))
           .ForMember(d => d.ecer_transcript_ProvinceId, opts => opts.MapFrom(s => s.Province))
           .ForMember(d => d.ecer_transcript_postsecondaryinstitutionid, opts => opts.MapFrom(s => s.PostSecondaryInstitution))
           .ForSourceMember(d => d.TranscriptStatusOption, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.CourseOutlineReceivedByRegistry, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.ProgramConfirmationReceivedByRegistry, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.TranscriptReceivedByRegistry, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.ComprehensiveReportReceivedByRegistry, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.CourseOutlineFiles, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.ProgramConfirmationFiles, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.CourseOutlineOptions, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.ComprehensiveReportOptions, opts => opts.DoNotValidate())
           .ForSourceMember(d => d.ProgramConfirmationOptions, opts => opts.DoNotValidate());

    CreateMap<ecer_Transcript, Transcript>(MemberList.Source)
      .ForCtorParam(nameof(Transcript.Id), opt => opt.MapFrom(src => src.ecer_TranscriptId))
      .ForCtorParam(nameof(Transcript.EducationalInstitutionName), opt => opt.MapFrom(src => src.ecer_EducationInstitutionFullName))
      .ForCtorParam(nameof(Transcript.ProgramName), opt => opt.MapFrom(src => src.ecer_ProgramCourseName))
      .ForCtorParam(nameof(Transcript.StudentNumber), opt => opt.MapFrom(src => src.ecer_StudentNumber))
      .ForCtorParam(nameof(Transcript.StartDate), opt => opt.MapFrom(src => src.ecer_StartDate))
      .ForCtorParam(nameof(Transcript.EndDate), opt => opt.MapFrom(src => src.ecer_EndDate))
      .ForCtorParam(nameof(Transcript.IsECEAssistant), opt => opt.MapFrom(src => src.ecer_IsECEAssistant))
      .ForCtorParam(nameof(Transcript.StudentFirstName), opt => opt.MapFrom(src => src.ecer_StudentFirstName))
      .ForCtorParam(nameof(Transcript.StudentLastName), opt => opt.MapFrom(src => src.ecer_StudentLastName))
      .ForCtorParam(nameof(Transcript.IsNameUnverified), opt => opt.MapFrom(src => src.ecer_IsNameUnverified))
      .ForCtorParam(nameof(Transcript.EducationRecognition), opt => opt.MapFrom(src => src.ecer_EducationRecognition))
      .ForCtorParam(nameof(Transcript.EducationOrigin), opt => opt.MapFrom(src => src.ecer_EducationOrigin))
      .ForMember(d => d.TranscriptStatusOption, opt => opt.MapFrom(src => GetTranscriptStatusOption(src)))
      .ForMember(d => d.CampusLocation, opts => opts.MapFrom(s => s.ecer_CampusLocation))
      .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
      .ForMember(d => d.StudentMiddleName, opts => opts.MapFrom(s => s.ecer_StudentMiddleName))
      .ForMember(d => d.Country, opts => opts.MapFrom(src =>
        src.ecer_InstituteCountryId != null
        ? new Country(src.ecer_InstituteCountryId.Id.ToString(), src.ecer_InstituteCountryIdName, string.Empty, false)
        : null))
      .ForMember(d => d.Province, opts => opts.MapFrom(src =>
        src.ecer_ProvinceId != null
        ? new Province(src.ecer_ProvinceId.Id.ToString(), src.ecer_ProvinceIdName, string.Empty)
        : null))
      .ForMember(d => d.PostSecondaryInstitution, opts => opts.MapFrom(src =>
        src.ecer_postsecondaryinstitutionid != null
        ? new PostSecondaryInstitution(src.ecer_postsecondaryinstitutionid.Id.ToString(), src.ecer_postsecondaryinstitutionidName, string.Empty)
        : null))
      .ForMember(d => d.TranscriptReceivedByRegistry, opts => opts.MapFrom(s => s.ecer_TranscriptReceived))
      .ForMember(d => d.ComprehensiveReportReceivedByRegistry, opts => opts.MapFrom(s => s.ecer_ComprehensiveEvaluationReportReceived))
      .ForMember(d => d.CourseOutlineReceivedByRegistry, opts => opts.MapFrom(s => s.ecer_CourseOutlineReceived))
      .ForMember(d => d.ProgramConfirmationReceivedByRegistry, opts => opts.MapFrom(s => s.ecer_ProgramConfirmationFormReceived))
      .ForMember(d => d.ProgramConfirmationFiles, opts => opts.MapFrom(src => src.ecer_bcgov_documenturl_TranscriptId.Where(i => i.ecer_Tag1 == "Program Confirmation Form").ToList()))
      .ForMember(d => d.CourseOutlineFiles, opts => opts.MapFrom(src => src.ecer_bcgov_documenturl_TranscriptId.Where(i => i.ecer_Tag1 == "Course Outline").ToList()))
      .ForMember(d => d.CourseOutlineOptions, opts => opts.MapFrom(src =>
        src.ecer_Ihavemycourseoutlinessyllabiandwillupload == true ? CourseOutlineOptions.UploadNow :
        src.ecer_isECEregistryalreadyhasmycourseoutline == true ? CourseOutlineOptions.RegistryAlreadyHas : (CourseOutlineOptions?)null))
      .ForMember(d => d.ProgramConfirmationOptions, opts => opts.MapFrom(src =>
        src.ecer_IhavemyProgramConfirmationandwillupload == true ? ProgramConfirmationOptions.UploadNow :
        src.ecer_isECEregistryhasprogramconfirmation == true ? ProgramConfirmationOptions.RegistryAlreadyHas : (ProgramConfirmationOptions?)null))
      .ForMember(d => d.ComprehensiveReportOptions, opts => opts.MapFrom(src =>
        src.ecer_ECERegistryalreadyhasmyComprehensiveReport == true ? ComprehensiveReportOptions.RegistryAlreadyHas :
        src.ecer_iwishtoapplyforafeewaiver == true ? ComprehensiveReportOptions.FeeWaiver :
        src.ecer_ihavesubmittedanapplicationtobcits == true ? ComprehensiveReportOptions.InternationalCredentialEvaluationService : (ComprehensiveReportOptions?)null))
      .ValidateMemberList(MemberList.Destination);

    CreateMap<ProfessionalDevelopment, ecer_ProfessionalDevelopment>(MemberList.Source)
       .ForSourceMember(s => s.StartDate, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.EndDate, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.NewFiles, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.DeletedFiles, opts => opts.DoNotValidate())
       .ForSourceMember(s => s.Files, opts => opts.DoNotValidate())
       .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.StartDate))
       .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.EndDate))
       .ForMember(d => d.ecer_CourseName, opts => opts.MapFrom(s => s.CourseName))
       .ForMember(d => d.ecer_CourseorWorkshopLink, opts => opts.MapFrom(s => s.CourseorWorkshopLink))
       .ForMember(d => d.ecer_OrganizationName, opts => opts.MapFrom(s => s.OrganizationName))
       .ForMember(d => d.ecer_HostOrganizationContactInformation, opts => opts.MapFrom(s => s.OrganizationContactInformation))
       .ForMember(d => d.ecer_OrganizationEmailAddress, opts => opts.MapFrom(s => s.OrganizationEmailAddress))
       .ForMember(d => d.ecer_InstructorName, opts => opts.MapFrom(s => s.InstructorName))
       .ForMember(d => d.ecer_TotalAnticipatedHours, opts => opts.MapFrom(s => s.NumberOfHours))
       .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status));

    CreateMap<ecer_ProfessionalDevelopment, ProfessionalDevelopment>(MemberList.Source)
       .ForCtorParam(nameof(ProfessionalDevelopment.Id), opt => opt.MapFrom(src => src.ecer_ProfessionalDevelopmentId))
       .ForCtorParam(nameof(ProfessionalDevelopment.CourseName), opt => opt.MapFrom(src => src.ecer_CourseName))
       .ForCtorParam(nameof(ProfessionalDevelopment.StartDate), opt => opt.MapFrom(src => src.ecer_StartDate))
       .ForCtorParam(nameof(ProfessionalDevelopment.EndDate), opt => opt.MapFrom(src => src.ecer_EndDate))
       .ForCtorParam(nameof(ProfessionalDevelopment.OrganizationName), opt => opt.MapFrom(src => src.ecer_OrganizationName))
       .ForMember(d => d.OrganizationContactInformation, opts => opts.MapFrom(s => s.ecer_HostOrganizationContactInformation))
       .ForMember(d => d.OrganizationEmailAddress, opts => opts.MapFrom(s => s.ecer_OrganizationEmailAddress))
       .ForMember(d => d.InstructorName, opts => opts.MapFrom(s => s.ecer_InstructorName))
       .ForMember(d => d.NumberOfHours, opts => opts.MapFrom(s => s.ecer_TotalAnticipatedHours))
       .ForMember(d => d.Status, opts => opts.MapFrom(s => s.StatusCode))
       .ForMember(d => d.CourseorWorkshopLink, opts => opts.MapFrom(s => s.ecer_CourseorWorkshopLink))
       .ForMember(d => d.NewFiles, opts => opts.Ignore())
       .ForMember(d => d.DeletedFiles, opts => opts.Ignore())
       .ForMember(d => d.Files, opts => opts.MapFrom(src => src.ecer_bcgov_documenturl_ProfessionalDevelopmentId.ToList()))
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
       .ForMember(d => d.StatusCode, opts => opts.MapFrom(s => s.Status))
       .ForMember(d => d.ecer_Type, opts => opts.MapFrom(s => s.Type));

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
       .ForMember(d => d.Type, opts => opts.MapFrom(s => s.ecer_Type))
       .ValidateMemberList(MemberList.Destination);

    CreateMap<WorkExperienceTypes, ecer_WorkExperienceTypes>()
        .ConvertUsing(src =>
            src == WorkExperienceTypes.Is500Hours ? ecer_WorkExperienceTypes._500Hours :
            src == WorkExperienceTypes.ICRA ? ecer_WorkExperienceTypes.ICRA :
            ecer_WorkExperienceTypes._400Hours
        );

    CreateMap<ecer_WorkExperienceTypes, WorkExperienceTypes>()
        .ConvertUsing(src =>
            src == ecer_WorkExperienceTypes._500Hours ? WorkExperienceTypes.Is500Hours :
            src == ecer_WorkExperienceTypes.ICRA ? WorkExperienceTypes.ICRA :
            WorkExperienceTypes.Is400Hours
        );

    CreateMap<bcgov_DocumentUrl, FileInfo>(MemberList.Destination)
          .ForMember(d => d.Id, opts => opts.MapFrom(s => s.bcgov_DocumentUrlId))
          .ForMember(d => d.Name, opts => opts.MapFrom(s => s.bcgov_FileName))
          .ForMember(d => d.Size, opts => opts.MapFrom(s => s.bcgov_FileSize))
          .ForMember(d => d.Url, opts => opts.MapFrom(s => s.bcgov_Url))
          .ForMember(d => d.Extention, opts => opts.MapFrom(s => s.bcgov_FileExtension));

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
      .ForMember(d => d.ecer_referencefirstname, opts => opts.MapFrom(s => s.ReferenceContactInformation.FirstName))
      .ForMember(d => d.ecer_referencelastname, opts => opts.MapFrom(s => s.ReferenceContactInformation.LastName))
      .ForMember(d => d.ecer_referencephonenumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.PhoneNumber))
      .ForMember(d => d.ecer_referenceemailaddress, opts => opts.MapFrom(s => s.ReferenceContactInformation.Email))
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
      .ForMember(d => d.ecer_referencefirstname, opts => opts.MapFrom(s => s.ReferenceContactInformation.FirstName))
      .ForMember(d => d.ecer_referencelastname, opts => opts.MapFrom(s => s.ReferenceContactInformation.LastName))
      .ForMember(d => d.ecer_ReferencePhoneNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.PhoneNumber))
      .ForMember(d => d.ecer_referenceemailaddress, opts => opts.MapFrom(s => s.ReferenceContactInformation.Email))
      .ForMember(d => d.ecer_ReferenceECECertifiedProvince, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateProvinceOther))
      .ForMember(d => d.ecer_ReferenceECECertificationNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.CertificateNumber))
      .ForMember(d => d.ecer_ReferenceBirthDate, opts => opts.MapFrom(s => s.ReferenceContactInformation.DateOfBirth))
      .ForMember(d => d.ecer_TotalNumberofHoursObserved, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.Hours))
      .ForMember(d => d.ecer_WorkHoursType, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.WorkHoursType))
      .ForMember(d => d.ecer_ChildCareProgramName, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildrenProgramName))
      .ForMember(d => d.ecer_TypeofChildrenProgram, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildrenProgramType))
      .ForMember(d => d.ecer_OtherChildProgramType, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildrenProgramTypeOther))
      .ForMember(d => d.ecer_Role, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.Role))
      .ForMember(d => d.ecer_AgeofChildrenCaredFor, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.AgeofChildrenCaredFor))
      .ForMember(d => d.ecer_StartDate, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.StartDate))
      .ForMember(d => d.ecer_EndDate, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.EndDate))
      .ForMember(d => d.ecer_RelationshiptoApplicant, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ReferenceRelationship))
      .ForMember(d => d.ecer_RelationshiptoApplicantOther, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.ReferenceRelationshipOther))
      .ForMember(d => d.ecer_AdditionalComments, opts => opts.MapFrom(s => s.WorkExperienceReferenceDetails.AdditionalComments))
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
      .ForMember(d => d.ecer_ChildcareAgeRangeNew, opts => { opts.MapFrom(s => s.WorkExperienceReferenceDetails.ChildcareAgeRanges); opts.Condition((src, dest, srcMember) => srcMember != null && srcMember.Any()); })
      .ForMember(d => d.ecer_WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
      .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.DateSigned))
      .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember, ctx) => srcMember != null));

    CreateMap<IcraWorkExperienceReferenceSubmissionRequest, ecer_WorkExperienceRef>(MemberList.Source)
   .ForSourceMember(s => s.WillProvideReference, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.ReferenceContactInformation, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.WorkExperienceReferenceCompetenciesAssessment, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.PortalInvitation, opts => opts.DoNotValidate())
   .ForSourceMember(s => s.ConfirmProvidedInformationIsRight, opts => opts.DoNotValidate())
   .ForMember(d => d.ecer_referencefirstname, opts => opts.MapFrom(s => s.ReferenceContactInformation.FirstName))
   .ForMember(d => d.ecer_referencelastname, opts => opts.MapFrom(s => s.ReferenceContactInformation.LastName))
   .ForMember(d => d.ecer_ReferencePhoneNumber, opts => opts.MapFrom(s => s.ReferenceContactInformation.PhoneNumber))
   .ForMember(d => d.ecer_referenceemailaddress, opts => opts.MapFrom(s => s.ReferenceContactInformation.Email))
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
   .ForMember(d => d.ecer_WillProvideReference, opts => opts.MapFrom(s => s.WillProvideReference ? ecer_YesNoNull.Yes : ecer_YesNoNull.No))
   .ForMember(d => d.ecer_DateSigned, opts => opts.MapFrom(s => s.DateSigned));

    CreateMap<ecer_Transcript_StatusCode, TranscriptStage>()
      .ConvertUsingEnumMapping(opts => opts.MapByName(true))
      .ReverseMap();

    CreateMap<ecer_ProfessionalDevelopment_StatusCode, ProfessionalDevelopmentStatusCode>()
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

    CreateMap<ChildcareAgeRanges, ecer_ChildcareAgeRange>()
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

  private static TranscriptStatusOptions? GetTranscriptStatusOption(ecer_Transcript src)
  {
    if (src.ecer_DoesECERegistryHaveTranscript == true)
    {
      return TranscriptStatusOptions.RegistryHasTranscript;
    }
    if (src.ecer_IsOfficialTranscriptRequested == true)
    {
      return TranscriptStatusOptions.OfficialTranscriptRequested;
    }
    if (src.ecer_mytranscriptwillrequireenglishtranslation == true)
    {
      return TranscriptStatusOptions.TranscriptWillRequireEnglishTranslation;
    }
    return null;
  }

  public static string IdOrEmpty(EntityReference? reference) =>
      reference != null && reference.Id != Guid.Empty
          ? reference.Id.ToString()
          : string.Empty;
}
