using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository
{
  private async Task UpdateProfessionalDevelopments(ecer_Application application, string ApplicantId, List<ProfessionalDevelopment> updatedEntities, CancellationToken ct)
  {
    await Task.CompletedTask;

    var existingProfessionalDevelopments = context.ecer_ProfessionalDevelopmentSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();
    foreach (var professionalDevelopment in existingProfessionalDevelopments)
    {
      if (!updatedEntities.Any(t => t.Id == professionalDevelopment.Id.ToString()))
      {
        // This deletes all files and document urls before deleting the professional development object
        await DeleteFilesForProfessionalDevelopment(professionalDevelopment.ecer_ProfessionalDevelopmentId, Guid.Parse(ApplicantId), ct);
        context.DeleteObject(professionalDevelopment);
      }
    }

    foreach (var professionalDevelopment in updatedEntities.Where(d => !string.IsNullOrEmpty(d.Id)))
    {
      var oldProfessionalDevelopment = existingProfessionalDevelopments.SingleOrDefault(t => t.Id.ToString() == professionalDevelopment.Id);
      if (oldProfessionalDevelopment != null)
      {
        context.Detach(oldProfessionalDevelopment);
      }

      var ecerProfessionalDevelopment = mapper.Map<ecer_ProfessionalDevelopment>(professionalDevelopment)!;
      context.Attach(ecerProfessionalDevelopment);
      context.UpdateObject(ecerProfessionalDevelopment);
      await HandleProfessionalDevelopmentFiles(ecerProfessionalDevelopment, Guid.Parse(ApplicantId), professionalDevelopment.NewFiles, professionalDevelopment.DeletedFiles, ct);
    }

    foreach (var professionalDevelopment in updatedEntities.Where(d => string.IsNullOrEmpty(d.Id)))
    {
      var ecerProfessionalDevelopment = mapper.Map<ecer_ProfessionalDevelopment>(professionalDevelopment)!;
      var newId = Guid.NewGuid();
      ecerProfessionalDevelopment.ecer_ProfessionalDevelopmentId = newId;
      context.AddObject(ecerProfessionalDevelopment);
      context.AddLink(application, ecer_Application.Fields.ecer_ecer_professionaldevelopment_Applicationi, ecerProfessionalDevelopment);
      await HandleProfessionalDevelopmentFiles(ecerProfessionalDevelopment, Guid.Parse(ApplicantId), professionalDevelopment.NewFiles, professionalDevelopment.DeletedFiles, ct);
    }
  }

  private async Task HandleProfessionalDevelopmentFiles(ecer_ProfessionalDevelopment professionalDevelopment, Guid applicantId, IEnumerable<string> tobeAddedFileIds, IEnumerable<string> tobeDeletedFileIds, CancellationToken ct)
  {
    await Task.CompletedTask;
    await HandleDeletedFiles(professionalDevelopment, tobeDeletedFileIds.ToList(), ct);
    await HandleAddedFiles(professionalDevelopment, applicantId, tobeAddedFileIds.ToList(), ct);
  }

  private async Task HandleDeletedFiles(ecer_ProfessionalDevelopment professionalDevelopment, List<string> tobeDeletedFileIds, CancellationToken ct)
  {
    await Task.CompletedTask;
    foreach (var fileId in tobeDeletedFileIds)
    {
      if (professionalDevelopment == null)
      {
        throw new InvalidOperationException($"professionalDevelopment '{professionalDevelopment}' not found");
      }
      // delete file and related document url
      await DeleteFileById(professionalDevelopment.Id, fileId, ct);
    }
  }

  private async Task DeleteFileById(Guid professionalDevelopmentId, string fileId, CancellationToken ct)
  {
    await Task.CompletedTask;

    var file = context.bcgov_DocumentUrlSet.SingleOrDefault(d => d.bcgov_DocumentUrlId == Guid.Parse(fileId));
    if (fileId == null)
    {
      throw new InvalidOperationException($"File with ID '{fileId}' not found");
    }
    var folder = "ecer_professionaldevelopment/" + professionalDevelopmentId;
    await objectStorageProvider.DeleteAsync(new S3Descriptor(GetBucketName(configuration), fileId, folder), ct);
    context.DeleteObject(file);
  }

  private async Task HandleAddedFiles(ecer_ProfessionalDevelopment professionalDevelopment, Guid applicantId, List<string> tobeAddedFileIds, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var fileId in tobeAddedFileIds)
    {
      if (professionalDevelopment == null)
      {
        throw new InvalidOperationException($"professionalDevelopment '{professionalDevelopment}' not found");
      }
      // add file and create document url
      // link it to professional development
      await AddFilesForProfessionalDevelopment(professionalDevelopment, applicantId, new List<string>() { fileId }, ct);
    }
  }

  private async Task DeleteFilesForProfessionalDevelopment(Guid? professionalDevelopmentId, Guid? applicantId, CancellationToken ct)
  {
    await Task.CompletedTask;

    var files = context.bcgov_DocumentUrlSet.Where(d => d.ecer_bcgov_documenturl_ProfessionalDevelopmentId.ecer_ProfessionalDevelopmentId == professionalDevelopmentId && d.bcgov_contact_bcgov_documenturl.ContactId == applicantId).ToList();
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

  private async Task AddFilesForProfessionalDevelopment(ecer_ProfessionalDevelopment professionalDevelopment, Guid? applicantId, List<string> fileIds, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var fileId in fileIds)
    {
      var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == applicantId);
      if (applicant == null) throw new InvalidOperationException($"Applicant '{applicantId}' not found");

      if (professionalDevelopment == null) throw new InvalidOperationException($"professionalDevelopment '{professionalDevelopment}' not found");

      var sourceFolder = "tempfolder";
      var destinationFolder = "ecer_professionaldevelopment/" + professionalDevelopment.Id;
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
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_ProfessionalDevelopmentId, professionalDevelopment);
    }
  }



  private static string GetBucketName(IConfiguration configuration) =>
  configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}
