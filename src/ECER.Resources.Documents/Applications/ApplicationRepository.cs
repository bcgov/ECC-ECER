﻿using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed class ApplicationRepository : IApplicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public ApplicationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<IEnumerable<Application>> Query(ApplicationQuery query)
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
    _ = UpdateApplicationTranscripts(ecerApplication, ecerTranscripts);
    _ = UpdateApplicationWorkExperienceReferences(ecerApplication, ecerWorkExperienceReferences);
    _ = UpdateCharacterReferences(ecerApplication, ecerCharacterReferences);

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

  public async Task UpdateApplicationWorkExperienceReferences(ecer_Application application, List<ecer_WorkExperienceRef> updatedReferences)
  {
    await Task.CompletedTask;
    var existingWorkExpericnes = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    // 1. Remove WorkExperienceReferences that they exist in the dataverse but not in the application
    foreach (var reference in existingWorkExpericnes)
    {
      if (!updatedReferences.Any(t => t.Id == reference.Id))
      {
        context.DeleteObject(reference);
      }
    }

    // 2. Add New WorkExperienceReferences that they exist in the application but not in the dataverse
    foreach (var reference in updatedReferences.Where(d => d.ecer_WorkExperienceRefId == null))
    {
      reference.ecer_WorkExperienceRefId = Guid.NewGuid();
      context.AddObject(reference);
      context.AddLink(application, ecer_Application.Fields.ecer_workexperienceref_Applicationid_ecer, reference);
    }

    // 3. Update Existing WorkExperienceReferences
    foreach (var reference in updatedReferences.Where(d => d.ecer_WorkExperienceRefId != null))
    {
      var oldReference = existingWorkExpericnes.SingleOrDefault(t => t.Id == reference.Id);
      if (oldReference != null)
      {
        context.Detach(oldReference);
      }
      context.Attach(reference);
      context.UpdateObject(reference);
    }
  }

  public async Task UpdateApplicationTranscripts(ecer_Application application, List<ecer_Transcript> updatedTranscripts)
  {
    await Task.CompletedTask;
    var existingTranscripts = context.ecer_TranscriptSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    // 1. Remove Transcripts that they exist in the dataverse but not in the application
    foreach (var transcript in existingTranscripts)
    {
      if (!updatedTranscripts.Any(t => t.Id == transcript.Id))
      {
        context.DeleteObject(transcript);
      }
    }

    // 2. Add New Transcripts that they exist in the application but not in the dataverse
    foreach (var transcript in updatedTranscripts.Where(d => d.ecer_TranscriptId == null))
    {
      transcript.ecer_TranscriptId = Guid.NewGuid();
      context.AddObject(transcript);
      context.AddLink(application, ecer_Application.Fields.ecer_transcript_Applicationid, transcript);
    }

    // 3. Update Existing Transcripts
    foreach (var transcript in updatedTranscripts.Where(d => d.ecer_TranscriptId != null))
    {
      var oldTranscript = existingTranscripts.SingleOrDefault(t => t.Id == transcript.Id);
      if (oldTranscript != null)
      {
        context.Detach(oldTranscript);
      }
      context.Attach(transcript);
      context.UpdateObject(transcript);
    }
  }

  public async Task UpdateCharacterReferences(ecer_Application application, List<ecer_CharacterReference> updatedCharacterReferences)
  {
    await Task.CompletedTask;
    var existingCharacterReferences = context.ecer_CharacterReferenceSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    // 1. Remove Character References that exist in the dataverse but not in the application
    foreach (var reference in existingCharacterReferences)
    {
      if (!updatedCharacterReferences.Any(t => t.Id == reference.Id))
      {
        context.DeleteObject(reference);
      }
    }

    // 2. Add New Character References that exist in the application but not in the dataverse
    foreach (var reference in updatedCharacterReferences.Where(d => d.ecer_CharacterReferenceId == null))
    {
      reference.ecer_CharacterReferenceId = Guid.NewGuid();
      context.AddObject(reference);
      context.AddLink(application, ecer_Application.Fields.ecer_characterreference_Applicationid, reference);
    }

    // 3. Update Existing Character References
    foreach (var reference in updatedCharacterReferences.Where(d => d.ecer_CharacterReferenceId != null))
    {
      var oldReference = existingCharacterReferences.SingleOrDefault(t => t.Id == reference.Id);
      if (oldReference != null)
      {
        context.Detach(oldReference);
      }
      context.Attach(reference);
      context.UpdateObject(reference);
    }
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
}
