using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository
{
  private async Task UpdateTranscripts(ecer_Application application, List<ecer_Transcript> updatedEntities)
  {
    await Task.CompletedTask;

    var results = context.From(context.ecer_TranscriptSet.Where(t => t.ecer_Applicationid.Id == application.ecer_ApplicationId))
    .Join()
      .Include(a => a.ecer_transcript_InstituteCountryId)
      .Include(a => a.ecer_transcript_ProvinceId)
      .Include(a => a.ecer_transcript_postsecondaryinstitutionid)
    .Execute();

    var existingTranscripts = results.ToList();

    foreach (var transcript in existingTranscripts)
    {
      if (!updatedEntities.Any(t => t.ecer_TranscriptId == transcript.ecer_TranscriptId))
      {
        context.DeleteObject(transcript);
      }
    }

    //// Update Existing Transcripts
    foreach (var transcript in updatedEntities.Where(d => d.ecer_TranscriptId != null))
    {
      var oldTranscript = existingTranscripts.SingleOrDefault(t => t.ecer_TranscriptId == transcript.ecer_TranscriptId);

      if (oldTranscript != null)
      {
        if (oldTranscript.ecer_transcript_InstituteCountryId != null)
        {
          //context.DeleteLink(oldTranscript.ecer_transcript_InstituteCountryId, ecer_Country.Fields.ecer_transcript_InstituteCountryId, oldTranscript);
          oldTranscript.ecer_transcript_InstituteCountryId = null;
        }
        if (oldTranscript.ecer_transcript_ProvinceId != null)
        {
          //context.DeleteLink(oldTranscript.ecer_transcript_ProvinceId, ecer_Province.Fields.ecer_transcript_ProvinceId, oldTranscript);
          oldTranscript.ecer_transcript_ProvinceId = null;
        }
        if (oldTranscript.ecer_transcript_postsecondaryinstitutionid != null)
        {
          //context.DeleteLink(oldTranscript.ecer_transcript_postsecondaryinstitutionid, ecer_PostSecondaryInstitute.Fields.ecer_transcript_postsecondaryinstitutionid, oldTranscript);
          oldTranscript.ecer_transcript_postsecondaryinstitutionid = null;
        }
        context.Attach(oldTranscript);
        context.UpdateObject(oldTranscript);
        transcript.StatusCode = oldTranscript.StatusCode;
        context.Detach(oldTranscript);
      }
      var country = transcript.ecer_transcript_InstituteCountryId == null ? null : context.ecer_CountrySet.SingleOrDefault(c => c.ecer_CountryId == transcript.ecer_transcript_InstituteCountryId!.ecer_CountryId);
      var province = transcript.ecer_transcript_ProvinceId == null ? null : context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_ProvinceId == transcript.ecer_transcript_ProvinceId!.ecer_ProvinceId);
      var institution = transcript.ecer_transcript_postsecondaryinstitutionid == null ? null : context.ecer_PostSecondaryInstituteSet.SingleOrDefault(p => p.ecer_PostSecondaryInstituteId == transcript.ecer_transcript_postsecondaryinstitutionid!.ecer_PostSecondaryInstituteId);
      transcript.ecer_transcript_InstituteCountryId = null;
      transcript.ecer_transcript_ProvinceId = null;
      transcript.ecer_transcript_postsecondaryinstitutionid = null;
      context.Attach(transcript);
      context.UpdateObject(transcript);
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
