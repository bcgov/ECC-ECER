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

    foreach (var professionalDevelopment in existingProfessionalDevelopments)
    {
      if (!updatedEntities.Any(t => t.Id == professionalDevelopment.Id))
      {
        // delete files before object deletion
        await DeleteFilesForDeletedProfessionalDevelopments(professionalDevelopment, ct);
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
  }

  private async Task DeleteFilesForDeletedProfessionalDevelopments(ecer_ProfessionalDevelopment professionalDevelopment, CancellationToken ct)
  {
    await Task.CompletedTask;

    var files = professionalDevelopment.ecer_bcgov_documenturl_ProfessionalDevelopmentId.ToList();
    for (int i = 0; i < files.Count; i++)
    {
      var items = files[i].bcgov_Url.Split('/');
      var fileId = items[1];
      var folder = items[0];
      await objectStorageProvider.DeleteAsync(new S3Descriptor(GetBucketName(configuration), fileId, folder), ct);
      context.DeleteObject(files[i]);
    }
  }

  private static string GetBucketName(IConfiguration configuration) =>
  configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}
