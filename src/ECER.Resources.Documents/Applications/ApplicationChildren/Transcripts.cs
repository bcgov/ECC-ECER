﻿using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository
{
  private async Task UpdateTranscripts(ecer_Application application, List<ecer_Transcript> updatedEntities)
  {
    await Task.CompletedTask;
    var existingTranscripts = context.ecer_TranscriptSet.Where(t => t.ecer_Applicationid.Id == application.ecer_ApplicationId).ToList();

    foreach (var transcript in existingTranscripts)
    {
      if (!updatedEntities.Any(t => t.ecer_TranscriptId == transcript.ecer_TranscriptId))
      {
        context.DeleteObject(transcript);
      }
    }

    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId != null))
    {
      var oldTranscript = existingTranscripts.SingleOrDefault(t => t.ecer_TranscriptId == transcript.ecer_TranscriptId);
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
