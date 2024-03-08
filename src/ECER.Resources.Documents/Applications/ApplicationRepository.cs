using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

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

    var applications = context.ecer_ApplicationSet;

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_Application_StatusCode>>(query.ByStatus)!.ToList();
      applications = applications.WhereIn(a => a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) applications = applications.Where(r => r.ecer_ApplicationId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) applications = applications.Where(r => r.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));

    context.LoadProperties(applications, ecer_Application.Fields.ecer_transcript_Applicationid);

    return mapper.Map<IEnumerable<Application>>(applications)!.ToList();
  }

  public async Task<string> SaveDraft(Application application)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(application.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{application.ApplicantId}' not found");

    var ecerApplication = mapper.Map<ecer_Application>(application)!;

    var ecerTranscripts = mapper.Map<IEnumerable<ecer_Transcript>>(application.Transcripts)!.ToList();

    var ecerCharacterReferences = mapper.Map<IEnumerable<ecer_CharacterReference>>(application.CharacterReference)!.ToList();

    if (!ecerApplication.ecer_ApplicationId.HasValue)
    {
      ecerApplication.ecer_ApplicationId = Guid.NewGuid();
      context.AddObject(ecerApplication);
      context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
    }
    else
    {
      var existingApplication = context.ecer_ApplicationSet.SingleOrDefault(c => c.ecer_ApplicationId == ecerApplication.ecer_ApplicationId);
      if (existingApplication == null) throw new InvalidOperationException($"ecer_Application '{ecerApplication.ecer_ApplicationId}' not found");

      if (ecerApplication.ecer_DateSigned.HasValue && existingApplication.ecer_DateSigned.HasValue) ecerApplication.ecer_DateSigned = existingApplication.ecer_DateSigned;

      context.Detach(existingApplication);
      context.Attach(ecerApplication);
      context.UpdateObject(ecerApplication);
    }
    _ = UpdateApplicationTranscripts(ecerApplication, ecerTranscripts);
    _ = UpdateCharacterReferences(ecerApplication, ecerCharacterReferences);
    context.SaveChanges();
    return ecerApplication.ecer_ApplicationId.Value.ToString();
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

//TODO investigate whether this pattern is okay or if we need to copy what we do in transcripts
  public async Task UpdateCharacterReferences(ecer_Application application, List<ecer_CharacterReference> updatedCharacterReferences)
  {
    await Task.CompletedTask;
    var existingCharacterReferences = context.ecer_CharacterReferenceSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    // 1. Remove character references
    foreach (var characterReference in existingCharacterReferences)
    {
      context.DeleteObject(characterReference);
    }

    // 2. Add New character references
    foreach (var characterReference in updatedCharacterReferences)
    {
      characterReference.ecer_CharacterReferenceId = Guid.NewGuid();
      context.AddObject(characterReference);
      context.AddLink(application, ecer_Application.Fields.ecer_characterreference_Applicationid, characterReference);
    }
  }

  public Task<string> Submit(string applicationId) => throw new NotImplementedException();
}
