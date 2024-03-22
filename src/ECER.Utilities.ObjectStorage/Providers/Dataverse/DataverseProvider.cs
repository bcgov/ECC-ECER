using ECER.Infrastructure.Common;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Utilities.ObjectStorage.Providers.Dataverse;

internal class DataverseProvider(IOrganizationServiceAsync organizationService) : IObjecStorageProvider
{
  public Task CopyAsync(Descriptor source, Descriptor destination, CancellationToken ct) => throw new NotImplementedException();

  public Task DeleteAsync(Descriptor source, CancellationToken ct) => throw new NotImplementedException();

  public async Task<FileObject?> GetAsync(Descriptor source, CancellationToken ct)
  {
    var src = source.DownCastOrThrow<Descriptor, DataverseDescriptor>();
    var file = await organizationService.DownloadFileAsync(src.Entity, src.PropertyName, ct);

    return new FileObject(file.FileName, file.MimeType, new MemoryStream(file.Content.ToArray()), null);
  }

  public Task MoveAsync(Descriptor source, Descriptor destination, CancellationToken ct) => throw new NotImplementedException();

  public async Task StoreAsync(Descriptor destination, FileObject obj, CancellationToken ct)
  {
    var dst = destination.DownCastOrThrow<Descriptor, DataverseDescriptor>();
    await organizationService.UploadFileAsync(dst.Entity, dst.PropertyName, new FileContainer(obj.FileName, obj.ContentType, await obj.Content.CloneAsync()), ct);
  }
}
