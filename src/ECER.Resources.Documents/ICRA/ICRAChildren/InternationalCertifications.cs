using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ICRA;

internal sealed partial class ICRARepository
{
  private async Task UpdateInternationalCertifications(ecer_ICRAEligibilityAssessment icraEligibility, Contact contact, string ApplicantId, List<InternationalCertification> updatedEntities, CancellationToken ct)
  {
    await Task.CompletedTask;

    var existingInternationalCertifications = context.ecer_InternationalCertificationSet.Where(t => t.ecer_EligibilityAssessment.Id == icraEligibility.Id).ToList();
    foreach (var InternationalCertification in existingInternationalCertifications)
    {
      if (!updatedEntities.Any(t => t.Id == InternationalCertification.Id.ToString()))
      {
        // This deletes all files and document urls before deleting the international certifications object
        await DeleteFilesForInternationalCertification(InternationalCertification.ecer_InternationalCertificationId, Guid.Parse(ApplicantId), ct);
        context.DeleteObject(InternationalCertification);
      }
    }

    // Update existing International Certifications
    foreach (var InternationalCertification in updatedEntities.Where(d => !string.IsNullOrEmpty(d.Id)))
    {
      var oldInternationalCertification = existingInternationalCertifications.SingleOrDefault(t => t.Id.ToString() == InternationalCertification.Id);
      var ecerInternationalCertification = mapper.Map<ecer_InternationalCertification>(InternationalCertification)!;

      if (oldInternationalCertification != null)
      {
        context.Detach(oldInternationalCertification);
        ecerInternationalCertification.StatusCode = oldInternationalCertification.StatusCode;
      }

      context.Attach(ecerInternationalCertification);
      context.UpdateObject(ecerInternationalCertification);
      await HandleInternationalCertificationFiles(ecerInternationalCertification, Guid.Parse(ApplicantId), InternationalCertification.NewFiles, InternationalCertification.DeletedFiles, ct);
    }

    foreach (var InternationalCertification in updatedEntities.Where(d => string.IsNullOrEmpty(d.Id)))
    {
      var ecerInternationalCertification = mapper.Map<ecer_InternationalCertification>(InternationalCertification)!;
      // no draft status handling required here beyond default mapping
      var newId = Guid.NewGuid();
      ecerInternationalCertification.ecer_InternationalCertificationId = newId;
      context.AddObject(ecerInternationalCertification);
      context.AddLink(icraEligibility, ecer_ICRAEligibilityAssessment.Fields.ecer_internationalcertification_EligibilityAssessment_ecer_icraeligibilityassessment, ecerInternationalCertification);
      await HandleInternationalCertificationFiles(ecerInternationalCertification, Guid.Parse(ApplicantId), InternationalCertification.NewFiles, InternationalCertification.DeletedFiles, ct);
    }
  }

  private async Task HandleInternationalCertificationFiles(ecer_InternationalCertification InternationalCertification, Guid applicantId, IEnumerable<string> tobeAddedFileIds, IEnumerable<string> tobeDeletedFileIds, CancellationToken ct)
  {
    await Task.CompletedTask;
    await HandleDeletedFiles(InternationalCertification, tobeDeletedFileIds.ToList(), ct);
    await HandleAddedFiles(InternationalCertification, applicantId, tobeAddedFileIds.ToList(), ct);
  }

  private async Task HandleDeletedFiles(ecer_InternationalCertification InternationalCertification, List<string> tobeDeletedFileIds, CancellationToken ct)
  {
    await Task.CompletedTask;
    foreach (var fileId in tobeDeletedFileIds)
    {
      if (InternationalCertification == null)
      {
        throw new InvalidOperationException($"InternationalCertification '{InternationalCertification}' not found");
      }
      // delete file and related document url
      await DeleteFileById(InternationalCertification.Id, fileId, ct);
    }
  }

  private async Task DeleteFileById(Guid InternationalCertificationId, string fileId, CancellationToken ct)
  {
    await Task.CompletedTask;

    var file = context.bcgov_DocumentUrlSet.SingleOrDefault(d => d.bcgov_DocumentUrlId == Guid.Parse(fileId));
    if (fileId == null)
    {
      throw new InvalidOperationException($"File with ID '{fileId}' not found");
    }
    var folder = "ecer_InternationalCertification/" + InternationalCertificationId;
    await objectStorageProvider.DeleteAsync(new S3Descriptor(GetBucketName(configuration), fileId, folder), ct);
    context.DeleteObject(file);
  }

  private async Task HandleAddedFiles(ecer_InternationalCertification InternationalCertification, Guid applicantId, List<string> tobeAddedFileIds, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var fileId in tobeAddedFileIds)
    {
      if (InternationalCertification == null)
      {
        throw new InvalidOperationException($"InternationalCertification '{InternationalCertification}' not found");
      }
      // add file and create document url
      // link it to international certification
      await AddFilesForInternationalCertification(InternationalCertification, applicantId, new List<string>() { fileId }, ct);
    }
  }

  private async Task DeleteFilesForInternationalCertification(Guid? InternationalCertificationId, Guid? applicantId, CancellationToken ct)
  {
    await Task.CompletedTask;

    var files = context.bcgov_DocumentUrlSet.Where(d => d.ecer_bcgov_documenturl_internationalcertificationid.ecer_InternationalCertificationId == InternationalCertificationId && d.bcgov_contact_bcgov_documenturl.ContactId == applicantId).ToList();
    for (int i = 0; i < files.Count; i++)
    {
      var items = files[i].bcgov_Url.Split('/');
      if (items.Length != 2)
      {
        throw new FormatException($"Invalid file URL format: {files[i].bcgov_Url}");
      }
      var fileId = items[1];
      var folder = items[0];
      await objectStorageProvider.DeleteAsync(new S3Descriptor(GetBucketName(configuration), fileId, folder), ct);
      context.DeleteObject(files[i]);
    }
  }

  private async Task AddFilesForInternationalCertification(ecer_InternationalCertification InternationalCertification, Guid? applicantId, List<string> fileIds, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var fileId in fileIds)
    {
      var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == applicantId);
      if (applicant == null) throw new InvalidOperationException($"Applicant '{applicantId}' not found");

      if (InternationalCertification == null) throw new InvalidOperationException($"InternationalCertification '{InternationalCertification}' not found");

      var sourceFolder = "tempfolder";
      var destinationFolder = "ecer_InternationalCertification/" + InternationalCertification.Id;
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), ct);
      await objectStorageProvider.MoveAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), new S3Descriptor(GetBucketName(configuration), fileId, destinationFolder), ct);

      var documenturl = new bcgov_DocumentUrl()
      {
        bcgov_FileName = file!.FileName,
        bcgov_FileSize = Infrastructure.Common.UtilityFunctions.HumanFileSize(file!.Content.Length),
        bcgov_DocumentUrlId = Guid.Parse(fileId),
        bcgov_Url = destinationFolder,
        StatusCode = bcgov_DocumentUrl_StatusCode.Active,
        StateCode = bcgov_documenturl_statecode.Active
      };

      context.AddObject(documenturl);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.bcgov_contact_bcgov_documenturl, applicant);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_internationalcertificationid, InternationalCertification);
    }
  }

  private static string GetBucketName(IConfiguration configuration) =>
  configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}

