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
    var applications = from a in context.ecer_ApplicationSet
                       join c in context.ContactSet on a.ecer_Applicantid.Id equals c.ContactId
                       select new { a, c };

    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_Application_StatusCode>>(query.ByStatus)!.ToList();
      applications = applications.WhereIn(a => a.a.StatusCode!.Value, statuses);
    }

    if (query.ById != null) applications = applications.Where(r => r.a.ecer_ApplicationId == Guid.Parse(query.ById));
    if (query.ByApplicantId != null) applications = applications.Where(r => r.a.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));

    return mapper.Map<IEnumerable<Application>>(applications.Select(r => r.a).ToList())!;
  }

  public async Task<string> SaveDraft(Application application)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(application.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{application.ApplicantId}' not found");

    var ecerApplication = mapper.Map<ecer_Application>(application)!;

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

      _ = UpdateApplicationTranscripts(ecerApplication);

      context.Attach(ecerApplication);
      context.UpdateObject(ecerApplication);
    }

    context.SaveChanges();
    return ecerApplication.ecer_ApplicationId.Value.ToString();
  }

  public async Task UpdateApplicationTranscripts(ecer_Application application)
  {
    await Task.CompletedTask;
    var existingTranscripts = context.ecer_TranscriptSet.Where(t => t.ecer_Applicationid.Id == application.Id);

    // 1. Remove Transcripts that they exist in the dataverse but not in the application
    foreach (var transcript in existingTranscripts.ToList())
    {
      if (!application.ecer_transcript_Applicationid.Any(t => t.Id == transcript.Id))
      {
        context.Detach(transcript);
      }
    }

    // 2. Add New Transcripts that they exist in the application but not in the dataverse
    foreach (var transcript in application.ecer_transcript_Applicationid)
    {
      if (!existingTranscripts.Any(t => t.Id == transcript.Id))
      {
        transcript.Id = Guid.NewGuid();
        var ecerTranscript = mapper.Map<ecer_Transcript>(transcript)!;
        context.Attach(ecerTranscript);
        context.AddLink(application, ecer_Application.Fields.ecer_transcript_Applicationid, ecerTranscript);
      }
    }

    // 3. Update Existing Transcripts
    foreach (var oldTranscript in existingTranscripts)
    {
      var newTranscript = application.ecer_transcript_Applicationid.FirstOrDefault(t => t.Id == oldTranscript.Id);
      if (newTranscript != null)
      {
        oldTranscript.ecer_StudentName = newTranscript.ecer_StudentName;
        oldTranscript.ecer_StudentNumber = newTranscript.ecer_StudentNumber;
        oldTranscript.ecer_StartDate = newTranscript.ecer_StartDate;
        oldTranscript.ecer_EndDate = newTranscript.ecer_EndDate;
        oldTranscript.ecer_CampusLocation = newTranscript.ecer_CampusLocation;
        oldTranscript.ecer_ProgramName = newTranscript.ecer_ProgramName;
        oldTranscript.ecer_LanguageofInstruction = newTranscript.ecer_LanguageofInstruction;
        oldTranscript.ecer_EducationInstitutionFullName = newTranscript.ecer_EducationInstitutionFullName;
      }
    }
    context.SaveChanges();
  }

  public Task<string> Submit(string applicationId) => throw new NotImplementedException();
}
