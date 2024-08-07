using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.Applications;

internal sealed partial class ApplicationRepository
{
  private async Task UpdateProfessionalDevelopments(ecer_Application application, List<ecer_ProfessionalDevelopment> updatedEntities, List<ApplicationFile> files, CancellationToken ct)
  {
    await Task.CompletedTask;
    var existingProfessionalDevelopments = context.ecer_ProfessionalDevelopmentSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    await HandleDeleteFiles(files, ct); // handles delete files before we delete professional developments
    foreach (var professionalDevelopment in existingProfessionalDevelopments)
    {
      if (!updatedEntities.Any(t => t.Id == professionalDevelopment.Id))
      {
        context.DeleteObject(professionalDevelopment);
      }
    }

    foreach (var professionalDevelopment in updatedEntities.Where(d => d.ecer_ProfessionalDevelopmentId != null))
    {
      var oldProfessionalDevelopment = existingProfessionalDevelopments.SingleOrDefault(t => t.Id == professionalDevelopment.Id);
      if (oldProfessionalDevelopment != null)
      {
        context.Detach(oldProfessionalDevelopment);
      }

      context.Attach(professionalDevelopment);
      context.UpdateObject(professionalDevelopment);
    }

    foreach (var professionalDevelopment in updatedEntities.Where(d => d.ecer_ProfessionalDevelopmentId == null))
    {
      professionalDevelopment.ecer_ProfessionalDevelopmentId = Guid.NewGuid();
      context.AddObject(professionalDevelopment);
      context.AddLink(application, ecer_Application.Fields.ecer_ecer_professionaldevelopment_Applicationi, professionalDevelopment);
    }
    await HandleAddFiles(files, ct); // handles add files after adding professional developments
  }

  private async Task HandleDeleteFiles(List<ApplicationFile> files, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var file in files)
    {
      if (file.FileOperation == FileOperation.Delete)
      {
        var professionalDevelopment = context.ecer_ProfessionalDevelopmentSet.SingleOrDefault(p => p.ecer_ProfessionalDevelopmentId.ToString() == file.RelatedEntityId);
        if (professionalDevelopment == null)
        {
          throw new InvalidOperationException($"professionalDevelopment '{file.RelatedEntityId}' not found");
        }
        // delete file and related document url
        await DeleteFilesForProfessionalDevelopment(professionalDevelopment.ecer_ProfessionalDevelopmentId, professionalDevelopment.ecer_ecer_professionaldevelopment_Applicantid_.ContactId, ct);
      }
    }
  }

  private async Task HandleAddFiles(List<ApplicationFile> files, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var file in files)
    {
      if (file.FileOperation == FileOperation.Add)
      {
        var professionalDevelopment = context.ecer_ProfessionalDevelopmentSet.SingleOrDefault(p => p.ecer_ProfessionalDevelopmentId.ToString() == file.RelatedEntityId);
        if (professionalDevelopment == null)
        {
          throw new InvalidOperationException($"professionalDevelopment '{file.RelatedEntityId}' not found");
        }
        // add file and create document url
        // link it to professional development
        await AddFilesForProfessionalDevelopment(professionalDevelopment.ecer_ProfessionalDevelopmentId, professionalDevelopment.ecer_ecer_professionaldevelopment_Applicantid_.ContactId, new List<string>() { file.UploadedFileId! }, ct);
      }
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

  private async Task AddFilesForProfessionalDevelopment(Guid? professionalDevelopmentId, Guid? applicantId, List<string> fileUrls, CancellationToken ct)
  {
    await Task.CompletedTask;

    foreach (var fileUrl in fileUrls)
    {
      var items = fileUrl.Split('/');
      if (items.Length != 2)
      {
        throw new ArgumentException($"Invalid file URL format: {fileUrl}");
      }

      var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == applicantId);
      if (applicant == null) throw new InvalidOperationException($"Applicant '{applicantId}' not found");

      var professionalDevelopment = context.ecer_ProfessionalDevelopmentSet.SingleOrDefault(c => c.ecer_ProfessionalDevelopmentId == professionalDevelopmentId);
      if (professionalDevelopment == null) throw new InvalidOperationException($"professionalDevelopment '{professionalDevelopmentId}' not found");

      var sourceFolder = items[0];
      var destinationFolder = "ecer_professionaldevelopment";
      var fileId = items[1];
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), ct);
      await objectStorageProvider.MoveAsync(new S3Descriptor(GetBucketName(configuration), fileId, sourceFolder), new S3Descriptor(GetBucketName(configuration), fileId, destinationFolder), ct);

      var documenturl = new bcgov_DocumentUrl()
      {
        bcgov_Url = string.Format("{0}/{1}", destinationFolder, fileId),
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
