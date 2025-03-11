using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers.S3;
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

  public async Task<string> SaveApplicationTranscripts(TranscriptDocuments transcriptDocuments, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var transcript = context.ecer_TranscriptSet.SingleOrDefault(
      d => d.ecer_TranscriptId == Guid.Parse(transcriptDocuments.TranscriptId) && d.ecer_Applicationid.Id == Guid.Parse(transcriptDocuments.ApplicationId));
    if (transcript == null) throw new InvalidOperationException($"Application '{transcriptDocuments.ApplicationId}' not found or Transcript '{transcriptDocuments.TranscriptId}' not found");
    // TODO: Add flags to transcript object

    if (transcriptDocuments.CourseOutlineOptions != null)
    {
      if (transcriptDocuments.CourseOutlineOptions.Value == CourseOutlineOptions.UploadNow)
      {
        transcript.ecer_isECEregistryalreadyhasmycourseoutline = false;
        transcript.ecer_iseducationalinstitutionsendcourseoutline = false;
      }
      else if (transcriptDocuments.CourseOutlineOptions.Value == CourseOutlineOptions.RegistryAlreadyHas)
      {
        transcript.ecer_isECEregistryalreadyhasmycourseoutline = true;
        transcript.ecer_iseducationalinstitutionsendcourseoutline = false;
      }
    }
    //if (transcriptDocuments.ComprehensiveReportOptions != null)
    //{
    //  if (transcriptDocuments.ComprehensiveReportOptions.Value == ComprehensiveReportOptions.FeeWaiver)
    //  {
    //    transcript
    //  }
    //  else if (transcriptDocuments.ComprehensiveReportOptions.Value == ComprehensiveReportOptions.InternationalCredentialEvaluationService)
    //  {
    //  }
    //  else if (transcriptDocuments.ComprehensiveReportOptions.Value == ComprehensiveReportOptions.RegistryAlreadyHas)
    //  {
    //  }
    //}
    //if (transcriptDocuments.ProgramConfirmationOptions != null)
    //{
    //  if (transcriptDocuments.ProgramConfirmationOptions.Value == ProgramConfirmationOptions.UploadNow)
    //  {
    //  }
    //  else if (transcriptDocuments.ProgramConfirmationOptions.Value == ProgramConfirmationOptions.RegistryAlreadyHas)
    //  {
    //  }
    //}

    await AddFilesToTranscript(transcript, transcriptDocuments.NewCourseOutlineFiles, "CourseOutlineFiles", cancellationToken);
    if (transcriptDocuments.NewCourseOutlineFiles.Any()) transcript.ecer_CourseOutlineReceived = true;
    await AddFilesToTranscript(transcript, transcriptDocuments.NewProgramConfirmationFiles, "ProgramConfirmationFiles", cancellationToken);
    if (transcriptDocuments.NewProgramConfirmationFiles.Any()) transcript.ecer_ProgramConfirmationFormReceived = true;
    context.UpdateObject(transcript);
    context.SaveChanges();
    return transcript.ecer_Applicationid.Id.ToString();
  }

  private async Task AddFilesToTranscript(ecer_Transcript transcript, IEnumerable<string> fileIds, string tagName, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var fileId in fileIds)
    {
      var sourceFolder = "tempfolder";
      var destinationFolder = "ecer_transcript/" + transcript.Id;
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), ct);
      await objectStorageProvider.MoveAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), new S3Descriptor(GetBucketName(configuration), fileId, destinationFolder), ct);

      var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == transcript.ecer_Applicantid.Id);
      if (applicant == null) throw new InvalidOperationException($"Applicant '{transcript.ecer_Applicantid.Id}' not found");

      var documenturl = new bcgov_DocumentUrl()
      {
        bcgov_FileName = file!.FileName,
        bcgov_FileSize = Infrastructure.Common.UtilityFunctions.HumanFileSize(file!.Content.Length),
        bcgov_DocumentUrlId = Guid.Parse(fileId),
        bcgov_Url = destinationFolder,
        StatusCode = bcgov_DocumentUrl_StatusCode.Active,
        StateCode = bcgov_documenturl_statecode.Active,
      };

      context.AddObject(documenturl);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.bcgov_contact_bcgov_documenturl, applicant);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_TranscriptId, transcript);

      var tag = context.bcgov_tagSet.SingleOrDefault(t => t.bcgov_name.Contains(tagName));
      if (tag == null) throw new InvalidOperationException($"Tag {tagName} not found");
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.bcgov_tag1_bcgov_documenturl, tag);
    }
  }
}
