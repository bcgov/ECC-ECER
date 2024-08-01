using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository
{
  public async Task UpdateTranscripts(ecer_Application application, List<ecer_Transcript> updatedEntities)
  {
    await Task.CompletedTask;
    var existingTranscripts = context.ecer_TranscriptSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    foreach (var transcript in existingTranscripts)
    {
      if (!updatedEntities.Any(t => t.Id == transcript.Id))
      {
        context.DeleteObject(transcript);
      }
    }

    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId != null))
    {
      var oldTranscript = existingTranscripts.SingleOrDefault(t => t.Id == transcript.Id);
      if (oldTranscript != null)
      {
        context.Detach(oldTranscript);
      }
      context.Attach(transcript);
      context.UpdateObject(transcript);
    }

    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId == null))
    {
      transcript.ecer_TranscriptId = Guid.NewGuid();
      context.AddObject(transcript);
      context.AddLink(application, ecer_Application.Fields.ecer_transcript_Applicationid, transcript);
    }
  }
}
