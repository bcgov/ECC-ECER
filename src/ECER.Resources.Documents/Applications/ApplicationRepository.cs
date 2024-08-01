using AutoMapper;
using ECER.Resources.Documents.Applications.ChildrenServices;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed class ApplicationRepository : IApplicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;
  private readonly IApplicationChildService<ecer_ProfessionalDevelopment> professionalDevelopmentService;
  private readonly IApplicationChildService<ecer_WorkExperienceRef> workExperienceService;
  private readonly IApplicationChildService<ecer_CharacterReference> characterReferenceService;
  private readonly IApplicationChildService<ecer_Transcript> transcriptService;

  public ApplicationRepository(
       EcerContext context,
       IMapper mapper,
       IApplicationChildService<ecer_ProfessionalDevelopment> professionalDevelopmentService,
       IApplicationChildService<ecer_WorkExperienceRef> workExperienceService,
       IApplicationChildService<ecer_CharacterReference> characterReferenceService,
       IApplicationChildService<ecer_Transcript> transcriptService)
  {
    this.context = context;
    this.mapper = mapper;
    this.professionalDevelopmentService = professionalDevelopmentService;
    this.workExperienceService = workExperienceService;
    this.characterReferenceService = characterReferenceService;
    this.transcriptService = transcriptService;
  }

  public async Task<IEnumerable<Application>> Query(ApplicationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applications = context.ecer_ApplicationSet.Where(a => a.StatusCode!.Value != ecer_Application_StatusCode.Cancelled);

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_Application_StatusCode>>(query.ByStatus)!.ToList();
      applications = applications.WhereIn(a => a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) applications = applications.Where(r => r.ecer_ApplicationId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) applications = applications.Where(r => r.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));

    context.LoadProperties(applications, ecer_Application.Fields.ecer_transcript_Applicationid);
    context.LoadProperties(applications, ecer_Application.Fields.ecer_workexperienceref_Applicationid_ecer);
    context.LoadProperties(applications, ecer_Application.Fields.ecer_characterreference_Applicationid);
    context.LoadProperties(applications, ecer_Application.Fields.ecer_ecer_professionaldevelopment_Applicationi);

    return mapper.Map<IEnumerable<Application>>(applications)!.ToList();
  }

  public async Task<string> SaveDraft(Application application, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(application.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{application.ApplicantId}' not found");

    var ecerApplication = mapper.Map<ecer_Application>(application)!;

    var ecerTranscripts = mapper.Map<IEnumerable<ecer_Transcript>>(application.Transcripts)!.ToList();
    var ecerWorkExperienceReferences = mapper.Map<IEnumerable<ecer_WorkExperienceRef>>(application.WorkExperienceReferences)!.ToList();
    var ecerCharacterReferences = mapper.Map<IEnumerable<ecer_CharacterReference>>(application.CharacterReferences)!.ToList();
    var ecerProfessionalDevelopments = mapper.Map<IEnumerable<ecer_ProfessionalDevelopment>>(application.ProfessionalDevelopments)!.ToList();

    if (!ecerApplication.ecer_ApplicationId.HasValue)
    {
      ecerApplication.ecer_ApplicationId = Guid.NewGuid();
      context.AddObject(ecerApplication);
      context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
    }
    else
    {
      var existingApplication = context.ecer_ApplicationSet.SingleOrDefault(c => c.ecer_ApplicationId == ecerApplication.ecer_ApplicationId && c.StatusCode == ecer_Application_StatusCode.Draft);
      if (existingApplication == null) throw new InvalidOperationException($"ecer_Application '{ecerApplication.ecer_ApplicationId}' not found");

      if (ecerApplication.ecer_DateSigned.HasValue && existingApplication.ecer_DateSigned.HasValue) ecerApplication.ecer_DateSigned = existingApplication.ecer_DateSigned;

      context.Detach(existingApplication);
      context.Attach(ecerApplication);
      context.UpdateObject(ecerApplication);
    }
    await professionalDevelopmentService.Update(ecerApplication, ecerProfessionalDevelopments);
    await workExperienceService.Update(ecerApplication, ecerWorkExperienceReferences);
    await characterReferenceService.Update(ecerApplication, ecerCharacterReferences);
    await transcriptService.Update(ecerApplication, ecerTranscripts);

    context.SaveChanges();
    return ecerApplication.ecer_ApplicationId.Value.ToString();
  }

  public async Task<string> Submit(string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.StatusCode == ecer_Application_StatusCode.Draft);
    if (application == null) throw new InvalidOperationException($"Application '{applicationId}' not found");

    application.StatusCode = ecer_Application_StatusCode.Submitted;
    context.UpdateObject(application);

    context.SaveChanges();
    return applicationId;
  }

  public async Task<string> Cancel(string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.StatusCode == ecer_Application_StatusCode.Draft
      );
    if (application == null) throw new InvalidOperationException($"Application '{applicationId}' not found");
    application.StatusCode = ecer_Application_StatusCode.Cancelled;
    application.StateCode = ecer_application_statecode.Inactive;
    context.UpdateObject(application);
    context.SaveChanges();
    return applicationId;
  }

  public async Task<string> SubmitReference(SubmitReferenceRequest request, CancellationToken cancellationToken)
  {
    return request switch
    {
      CharacterReferenceSubmissionRequest req => await SubmitCharacterReference(request.PortalInvitation!.CharacterReferenceId!, req),
      WorkExperienceReferenceSubmissionRequest req => await SubmitWorkexperienceReference(request.PortalInvitation!.WorkexperienceReferenceId!, req),
      _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
    };
  }

  public async Task<string> OptOutReference(OptOutReferenceRequest request, CancellationToken cancellationToken)
  {
    return request.PortalInvitation!.InviteType switch
    {
      InviteType.CharacterReference => await OptOutCharacterReference(request),
      InviteType.WorkExperienceReference => await OptOutWorkExperienceReference(request),
      _ => throw new NotSupportedException($"{request.GetType().Name} is not supported")
    };
  }

  #region implementationDetails

  private async Task<string> SubmitCharacterReference(string characterReferenceId, CharacterReferenceSubmissionRequest request)
  {
    await Task.CompletedTask;
    var characterReference = context.ecer_CharacterReferenceSet.Single(c => c.ecer_CharacterReferenceId == Guid.Parse(characterReferenceId));

    mapper.Map(request, characterReference);
    bool certificateProvinceIdIsGuid = Guid.TryParse(request.ReferenceContactInformation.CertificateProvinceId, out Guid certificateProvinceId);

    if (certificateProvinceIdIsGuid)
    {
      var province = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_ProvinceId == certificateProvinceId);
      if (province != null)
      {
        context.AddLink(characterReference, ecer_CharacterReference.Fields.ecer_characterreference_RefCertifiedProvinceId, province);
      }
    }
    characterReference.StatusCode = ecer_CharacterReference_StatusCode.Submitted;
    context.UpdateObject(characterReference);
    context.SaveChanges();
    return characterReference.ecer_CharacterReferenceId.ToString()!;
  }

  private async Task<string> SubmitWorkexperienceReference(string workExperienceReferenceId, WorkExperienceReferenceSubmissionRequest request)
  {
    await Task.CompletedTask;
    var workExperienceReference = context.ecer_WorkExperienceRefSet.Single(c => c.ecer_WorkExperienceRefId == Guid.Parse(workExperienceReferenceId));

    mapper.Map(request, workExperienceReference);
    bool certificateProvinceIdIsGuid = Guid.TryParse(request.ReferenceContactInformation.CertificateProvinceId, out Guid certificateProvinceId);
    if (certificateProvinceIdIsGuid)
    {
      var province = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_ProvinceId == certificateProvinceId);
      if (province != null)
      {
        context.AddLink(workExperienceReference, ecer_WorkExperienceRef.Fields.ecer_workexperienceref_RefCertifiedProvinceId, province);
      }
    }
    workExperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.Submitted;
    context.UpdateObject(workExperienceReference);
    context.SaveChanges();
    return workExperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }

  private async Task<string> OptOutCharacterReference(OptOutReferenceRequest request)
  {
    await Task.CompletedTask;
    var characterReference = context.ecer_CharacterReferenceSet.Single(c => c.ecer_CharacterReferenceId == Guid.Parse(request.PortalInvitation!.CharacterReferenceId!));

    mapper.Map(request, characterReference);
    characterReference.ecer_WillProvideReference = ecer_YesNoNull.No;
    characterReference.StatusCode = ecer_CharacterReference_StatusCode.Rejected;
    characterReference.StateCode = ecer_characterreference_statecode.Inactive;
    context.UpdateObject(characterReference);
    context.SaveChanges();
    return characterReference.ecer_CharacterReferenceId.ToString()!;
  }

  private async Task<string> OptOutWorkExperienceReference(OptOutReferenceRequest request)
  {
    await Task.CompletedTask;
    var workexperienceReference = context.ecer_WorkExperienceRefSet.Single(c => c.ecer_WorkExperienceRefId == Guid.Parse(request.PortalInvitation!.WorkexperienceReferenceId!));

    mapper.Map(request, workexperienceReference);
    workexperienceReference.ecer_WillProvideReference = ecer_YesNoNull.No;
    workexperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.Rejected;
    workexperienceReference.StateCode = ecer_workexperienceref_statecode.Inactive;
    context.UpdateObject(workexperienceReference);
    context.SaveChanges();
    return workexperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }

  #endregion implementationDetails

  public async Task<string> UpdateWorkExReferenceForSubmittedApplication(WorkExperienceReference updatedReference, string applicationId, string referenceId, string userId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.ecer_Applicantid.Id == Guid.Parse(userId)
      );
    if (application == null)
    {
      throw new InvalidOperationException($"Application '{applicationId}' not found");
    }

    var ecerWorkExperienceReference = mapper.Map<ecer_WorkExperienceRef>(updatedReference);

    var existingWorkExperiences = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_Applicationid.Id == Guid.Parse(applicationId)).ToList();

    bool RefIdIsGuid = Guid.TryParse(referenceId, out Guid referenceIdGuid);
    if (RefIdIsGuid)
    {
      var oldReference = existingWorkExperiences.SingleOrDefault(t => t.Id == referenceIdGuid);
      // 1. Remove existing WorkExperienceReference

      if (oldReference != null)
      {
        context.DeleteObject(oldReference);
      }
      else
      {
        throw new InvalidOperationException($"Reference '{referenceIdGuid}' not found for application '{applicationId}'");
      }
    }

    // 2. Add New WorkExperienceReferences

    ecerWorkExperienceReference.ecer_WorkExperienceRefId = Guid.NewGuid();
    ecerWorkExperienceReference.StatusCode = ecer_WorkExperienceRef_StatusCode.ApplicationSubmitted;
    ecerWorkExperienceReference.ecer_IsAdditional = true;
    context.AddObject(ecerWorkExperienceReference);
    context.AddLink(application, ecer_Application.Fields.ecer_workexperienceref_Applicationid_ecer, ecerWorkExperienceReference);
    context.SaveChanges();

    return ecerWorkExperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }

  public async Task<string> UpdateCharacterReferenceForSubmittedApplication(CharacterReference updatedReference, string applicationId, string referenceId, string userId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var application = context.ecer_ApplicationSet.FirstOrDefault(
      d => d.ecer_ApplicationId == Guid.Parse(applicationId) && d.ecer_Applicantid.Id == Guid.Parse(userId)
      );
    if (application == null)
    {
      throw new InvalidOperationException($"Application '{applicationId}' not found");
    }

    var ecerCharacterReference = mapper.Map<ecer_CharacterReference>(updatedReference);

    var existingCharacterReferences = context.ecer_CharacterReferenceSet.Where(t => t.ecer_Applicationid.Id == Guid.Parse(applicationId)).ToList();

    bool RefIdIsGuid = Guid.TryParse(referenceId, out Guid referenceIdGuid);
    if (RefIdIsGuid)
    {
      var oldReference = existingCharacterReferences.SingleOrDefault(t => t.Id == referenceIdGuid);

      // 1. Remove existing Character Reference
      if (oldReference != null)
      {
        context.DeleteObject(oldReference);
      }
      else
      {
        throw new InvalidOperationException($"Reference '{referenceIdGuid}' not found for application '{applicationId}'");
      }
    }

    // 2. Add New Character Reference

    ecerCharacterReference.ecer_CharacterReferenceId = Guid.NewGuid();
    ecerCharacterReference.StatusCode = ecer_CharacterReference_StatusCode.ApplicationSubmitted;
    ecerCharacterReference.ecer_IsAdditional = true;
    context.AddObject(ecerCharacterReference);
    context.AddLink(application, ecer_Application.Fields.ecer_characterreference_Applicationid, ecerCharacterReference);
    context.SaveChanges();

    return ecerCharacterReference.ecer_CharacterReferenceId.ToString()!;
  }

  public async Task<string> ResendCharacterReferenceInvite(ResendReferenceInviteRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var characterReference = context.ecer_CharacterReferenceSet.SingleOrDefault(c => c.ecer_CharacterReferenceId == Guid.Parse(request.ReferenceId!));

    if (characterReference == null)
    {
      throw new InvalidOperationException($"Character reference '{request.ReferenceId}' not found");
    }

    if (characterReference.StatusCode == ecer_CharacterReference_StatusCode.Rejected || characterReference.StatusCode == ecer_CharacterReference_StatusCode.Submitted)
    {
      throw new InvalidOperationException($"Character reference '{request.ReferenceId}' already responded");
    }

    mapper.Map(request, characterReference);
    characterReference.ecer_InviteAgain = true;
    context.UpdateObject(characterReference);
    context.SaveChanges();

    return characterReference.ecer_CharacterReferenceId.ToString()!;
  }

  public async Task<string> ResendWorkExperienceReferenceInvite(ResendReferenceInviteRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var workexperienceReference = context.ecer_WorkExperienceRefSet.SingleOrDefault(c => c.ecer_WorkExperienceRefId == Guid.Parse(request.ReferenceId!));

    if (workexperienceReference == null)
    {
      throw new InvalidOperationException($"Work experience reference '{request.ReferenceId}' not found");
    }

    if (workexperienceReference.StatusCode == ecer_WorkExperienceRef_StatusCode.Rejected || workexperienceReference.StatusCode == ecer_WorkExperienceRef_StatusCode.Submitted)
    {
      throw new InvalidOperationException($"Work experience reference '{request.ReferenceId}' already responded");
    }

    workexperienceReference.ecer_InviteAgain = true;
    context.UpdateObject(workexperienceReference);
    context.SaveChanges();
    return workexperienceReference.ecer_WorkExperienceRefId.ToString()!;
  }
}
