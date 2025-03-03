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

    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId != null))
    {
      var oldTranscript = existingTranscripts
         .SingleOrDefault(t => t.ecer_TranscriptId == transcript.ecer_TranscriptId);

      if (oldTranscript != null)
      {
        context.DeleteObject(oldTranscript);
      }
    }

    foreach (var transcript in updatedEntities)
    {
      transcript.ecer_TranscriptId = Guid.NewGuid();
      transcript.StatusCode = ecer_Transcript_StatusCode.Draft;
      var country = transcript.ecer_transcript_InstituteCountryId == null ? null : context.ecer_CountrySet.SingleOrDefault(c => c.ecer_CountryId == transcript.ecer_transcript_InstituteCountryId!.ecer_CountryId);
      var province = transcript.ecer_transcript_ProvinceId == null ? null : context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_ProvinceId == transcript.ecer_transcript_ProvinceId!.ecer_ProvinceId);
      var institution = transcript.ecer_transcript_postsecondaryinstitutionid == null ? null : context.ecer_PostSecondaryInstituteSet.SingleOrDefault(p => p.ecer_PostSecondaryInstituteId == transcript.ecer_transcript_postsecondaryinstitutionid!.ecer_PostSecondaryInstituteId);
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
