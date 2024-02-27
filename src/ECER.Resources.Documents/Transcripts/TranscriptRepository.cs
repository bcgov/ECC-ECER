using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.Transcripts;

internal sealed class TranscriptRepository : ITranscriptRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public TranscriptRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<bool> Delete(string transcriptId)
  {
    await Task.CompletedTask;
    var transcript = context.ecer_TranscriptSet.SingleOrDefault(c => c.Id == Guid.Parse(transcriptId));
    if (transcript == null) throw new InvalidOperationException($"Transcript '{transcriptId}' not found");

    var result = context.Detach(transcript);
    context.SaveChanges();

    return result;
  }

  public async Task<IEnumerable<Transcript>> Query(TranscriptQuery query)
  {
    await Task.CompletedTask;
    var transcripts = from t in context.ecer_TranscriptSet
                      join a in context.ecer_ApplicationSet on t.ecer_Applicationid.Id equals a.ecer_ApplicationId
                      join c in context.ContactSet on a.ecer_Applicantid.Id equals c.ContactId
                      select new { t, a, c };

    if (query.ByApplicationId != null) transcripts = transcripts.Where(r => r.a.Id == Guid.Parse(query.ByApplicationId));
    return mapper.Map<IEnumerable<Transcript>>(transcripts.Select(r => r.t).ToList())!;
  }

  public async Task<string> Save(Transcript transcript)
  {
    await Task.CompletedTask;

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(transcript.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{transcript.ApplicantId}' not found");

    var application = context.ecer_ApplicationSet.SingleOrDefault(c => c.Id == Guid.Parse(transcript.ApplicationId));
    if (application == null) throw new InvalidOperationException($"Application '{transcript.ApplicationId}' not found");

    var ecerTranscript = mapper.Map<ecer_Transcript>(transcript)!;

    ecerTranscript.ecer_TranscriptId = Guid.NewGuid();
    context.AddObject(ecerTranscript);
    context.AddLink(ecerTranscript, ecer_Transcript.Fields.ecer_transcript_Applicantid_Contact, applicant);
    context.AddLink(ecerTranscript, ecer_Transcript.Fields.ecer_transcript_Applicationid, application);
    context.SaveChanges();

    return ecerTranscript.ecer_TranscriptId.Value.ToString();
  }

  public async Task<string> Update(Transcript transcript)
  {
    await Task.CompletedTask;

    var currentTranscript = context.ecer_TranscriptSet.SingleOrDefault(c => transcript.Id == null || c.Id == Guid.Parse(transcript.Id));
    if (currentTranscript == null) throw new InvalidOperationException($"Transcript '{transcript.Id}' not found");

    var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(transcript.ApplicantId));
    if (applicant == null) throw new InvalidOperationException($"Applicant '{transcript.ApplicantId}' not found");

    var application = context.ecer_ApplicationSet.SingleOrDefault(c => c.Id == Guid.Parse(transcript.ApplicationId));
    if (application == null) throw new InvalidOperationException($"Application '{transcript.ApplicationId}' not found");

    var ecerTranscript = mapper.Map<ecer_Transcript>(currentTranscript)!;

    var existingTranscript = context.ecer_TranscriptSet.SingleOrDefault(c => c.ecer_TranscriptId == ecerTranscript.ecer_TranscriptId);
    if (existingTranscript == null) throw new InvalidOperationException($"ecer_Transcript '{ecerTranscript.ecer_TranscriptId}' not found");
    context.Detach(existingTranscript);

    context.Attach(ecerTranscript);
    context.UpdateObject(ecerTranscript);
    context.SaveChanges();

    return ecerTranscript.Id.ToString();
  }
}
