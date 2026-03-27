using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ProgramApplications;

internal sealed partial class ProgramApplicationRepository
{
  private async Task HandleComponentFiles(ecer_ProgramApplicationComponent component, IEnumerable<FileInfo> toAddFiles, IEnumerable<FileInfo> ToDeleteFiles, string postSecondaryInstituteId, string programApplicationId, string componentGroupId, CancellationToken ct)
  {
    await Task.CompletedTask;
    if (component == null)
    {
      throw new InvalidOperationException($"Component '{component}' not found");
    }

    if (ToDeleteFiles.Any())
    {
      await HandleDeletedFiles(component, ToDeleteFiles, ct);
    }

    //There are files to add. Run lookups for documentURL linkage
    if (toAddFiles.Any())
    {
      var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(d => d.ecer_PostSecondaryInstituteId == Guid.Parse(postSecondaryInstituteId));
      if (institute == null)
      {
        throw new InvalidOperationException($"Institute id '{postSecondaryInstituteId}' not found");
      }

      var componentGroup = context.ecer_ProgramApplicationComponentGroupSet.SingleOrDefault(cg => cg.ecer_ProgramApplicationComponentGroupId == Guid.Parse(componentGroupId));
      if (componentGroup == null)
      {
        throw new InvalidOperationException($"component group id '{componentGroupId}' not found");
      }

      var programApplication = context.ecer_PostSecondaryInstituteProgramApplicaitonSet.SingleOrDefault(a => a.ecer_PostSecondaryInstituteProgramApplicaitonId == Guid.Parse(programApplicationId));
      if (programApplication == null)
      {
        throw new InvalidOperationException($"program application id '{programApplicationId}' not found");
      }
      await HandleAddedFiles(component, toAddFiles, institute, componentGroup, programApplication, ct);
    }
  }

  private async Task HandleDeletedFiles(ecer_ProgramApplicationComponent component, IEnumerable<FileInfo> tobeDeletedFiles, CancellationToken ct)
  {
    await Task.CompletedTask;
    foreach (var deleteFile in tobeDeletedFiles)
    {
      // delete file and related document url
      var file = context.bcgov_DocumentUrlSet.SingleOrDefault(d => d.bcgov_DocumentUrlId == Guid.Parse(deleteFile.Id));
      if (file == null)
      {
        throw new InvalidOperationException($"File with ID '{deleteFile.Id}' not found");
      }
      var folder = "ecer_programapplicationcomponent/" + component.Id;
      var objectStorageProvider = objectStorageProviderResolver.resolve(deleteFile.EcerWebApplicationType);
      await objectStorageProvider.DeleteAsync(new S3Descriptor(objectStorageProvider.BucketName, deleteFile.Id, folder), ct);
      context.DeleteObject(file);
    }
  }

  private async Task HandleAddedFiles(ecer_ProgramApplicationComponent component, IEnumerable<FileInfo> toAddFiles, ecer_PostSecondaryInstitute institute, ecer_ProgramApplicationComponentGroup componentGroup, ecer_PostSecondaryInstituteProgramApplicaiton programApplication, CancellationToken ct)
  {
    foreach (var addFile in toAddFiles)
    {
      var sourceFolder = "tempfolder";
      var destinationFolder = "ecer_programapplicationcomponent/" + component.Id;
      var objectStorageProvider = objectStorageProviderResolver.resolve(addFile.EcerWebApplicationType);
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(objectStorageProvider.BucketName, addFile.Id, sourceFolder), ct);
      await objectStorageProvider.MoveAsync(new S3Descriptor(objectStorageProvider.BucketName, addFile.Id, sourceFolder), new S3Descriptor(objectStorageProvider.BucketName, addFile.Id, destinationFolder), ct);

      var documenturl = new bcgov_DocumentUrl()
      {
        bcgov_FileName = file!.FileName,
        bcgov_FileSize = Infrastructure.Common.UtilityFunctions.HumanFileSize(file!.Content.Length),
        bcgov_DocumentUrlId = Guid.Parse(addFile.Id),
        bcgov_Url = destinationFolder,
        StatusCode = bcgov_DocumentUrl_StatusCode.Active,
        StateCode = bcgov_documenturl_statecode.Active,
        ecer_ApplicationName = addFile.EcerWebApplicationType.ToString()
      };

      context.AddObject(documenturl);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_psiid_ecer_postsecondaryinstitute, institute);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_documenturl_ProgramApplicationComponentGroupId, componentGroup);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_programapplicationid_ecer_postsecondaryinstituteprogramapplicaiton, programApplication);
      context.AddLink(documenturl, bcgov_DocumentUrl.Fields.ecer_documenturl_ProgramApplicationComponentId, component);
    }
  }
}
