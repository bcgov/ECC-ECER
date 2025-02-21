using ECER.Utilities.DataverseSdk.Model;
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

    // Update Existing Transcripts
    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId != null))
    {
      var oldTranscript = existingTranscripts.SingleOrDefault(t => t.ecer_TranscriptId == transcript.ecer_TranscriptId);
      if (oldTranscript != null)
      {
        context.Detach(oldTranscript);
        transcript.StatusCode = oldTranscript.StatusCode;
      }
      context.Attach(transcript);
      context.UpdateObject(transcript);
    }

    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId == null))
    {
      transcript.ecer_TranscriptId = Guid.NewGuid();
      transcript.StatusCode = ecer_Transcript_StatusCode.Draft;
      var country = context.ecer_CountrySet.FirstOrDefault(c => c.ecer_CountryId == transcript.ecer_transcript_InstituteCountryId!.ecer_CountryId);
      var province = context.ecer_ProvinceSet.FirstOrDefault(p => p.ecer_ProvinceId == transcript.ecer_transcript_ProvinceId!.ecer_ProvinceId);
      var institution = context.ecer_PostSecondaryInstituteSet.FirstOrDefault(p => p.ecer_PostSecondaryInstituteId == transcript.ecer_transcript_postsecondaryinstitutionid!.ecer_PostSecondaryInstituteId);
      transcript.ecer_transcript_InstituteCountryId = null;
      transcript.ecer_transcript_ProvinceId = null;
      transcript.ecer_transcript_postsecondaryinstitutionid = null;
      context.AddObject(transcript);
      context.AddLink(application, ecer_Application.Fields.ecer_transcript_Applicationid, transcript);

      if (country != null)
      {
        context.AddLink(country, ecer_Country.Fields.ecer_transcript_InstituteCountryId, transcript);
      }
      if (province != null)
      {
        context.AddLink(province, ecer_Province.Fields.ecer_transcript_ProvinceId, transcript);
      }
      if (institution != null)
      {
        context.AddLink(institution, ecer_PostSecondaryInstitute.Fields.ecer_transcript_postsecondaryinstitutionid, transcript);
      }
    }
  }
}
