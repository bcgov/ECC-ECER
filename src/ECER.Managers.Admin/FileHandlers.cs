using ECER.Managers.Admin.Contract.Files;
using ECER.Resources.Documents.MetadataResources;
using ECER.Utilities.FileScanner;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Mediator;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace ECER.Managers.Admin;

public class FileHandlers(IObjectStorageProviderResolver objectStorageProviderResolver, IConfiguration configuration, IFileScannerProvider fileScannerProvider, IMetadataResourceRepository metadataResourceRepository)
  : IRequestHandler<SaveFileCommand, SaveFileCommandResponse>, IRequestHandler<DeleteFileCommand>, IRequestHandler<FileQuery, FileQueryResults>

{
  public async ValueTask<SaveFileCommandResponse> Handle(SaveFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var saveFileResults = new ConcurrentBag<SaveFileResult>();
    await Parallel.ForEachAsync(request.Items, cancellationToken, async (file, ct) =>
    {
      var objectStorageProvider = objectStorageProviderResolver.resolve(file.FileLocation.ecerWebApplicationType);
      var bucket = objectStorageProvider.BucketName;
      var tags = new Dictionary<string, string>(file.FileProperties.TagsList ?? Array.Empty<KeyValuePair<string, string>>());

      // Check if the classification property is not null or empty and the key doesn't already exist
      if (!string.IsNullOrEmpty(file.FileProperties.Classification) && !tags.ContainsKey("classification"))
      {
        tags.Add("classification", file.FileProperties.Classification);
      }

      var scanResult = await fileScannerProvider.ScanAsync(file.Content, ct);
      if (scanResult.IsClean)
      {
        await objectStorageProvider.StoreAsync(new S3Descriptor(bucket!, file.FileLocation.Id, file.FileLocation.Folder), new FileObject(file.FileName, file.ContentType, file.Content, tags), ct);
        saveFileResults.Add(new SaveFileResult(file, true, "File Saved Successfully"));
      }
      else
      {
        saveFileResults.Add(new SaveFileResult(file, false, scanResult.Message));
      }
    });
    return new SaveFileCommandResponse(saveFileResults);
  }

  public async ValueTask<Unit> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var objectStorageProvider = objectStorageProviderResolver.resolve(request.Item.FileLocation.ecerWebApplicationType);
    var bucket = objectStorageProvider.BucketName;
    await objectStorageProvider.DeleteAsync(new S3Descriptor(bucket!, request.Item.FileLocation.Id, request.Item.FileLocation.Folder), cancellationToken);
    return Unit.Value;
  }

  public async ValueTask<FileQueryResults> Handle(FileQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(configuration);

    var files = new ConcurrentBag<FileData>();
    await Parallel.ForEachAsync(request.FileLocations, cancellationToken, async (fileLocation, ct) =>
    {
      var objectStorageProvider = objectStorageProviderResolver.resolve(fileLocation.ecerWebApplicationType);
      var bucket = objectStorageProvider.BucketName;

      var file = await objectStorageProvider.GetAsync(new S3Descriptor(bucket!, fileLocation.Id, fileLocation.Folder), ct);
      var classification = file?.Tags?.SingleOrDefault(t => t.Key == "classification");
      var fileProperties = new FileProperties
      {
        Classification = classification?.Value ?? string.Empty,
        TagsList = file?.Tags?.Where(t => t.Key != "classification")
      };

      if (file != null) files.Add(new FileData(fileLocation, fileProperties, file.FileName, file.ContentType, file.Content));

      if (request.TrackDownload)
      {
        await metadataResourceRepository.SetDownloadDate(fileLocation.Id, cancellationToken);
      }
    });

    return new FileQueryResults(files.ToList());
  }
}
