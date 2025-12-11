using ECER.Managers.Admin.Contract.Files;
using ECER.Resources.Documents.MetadataResources;
using ECER.Utilities.FileScanner;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace ECER.Managers.Admin;

public class FileHandlers(IObjecStorageProvider objectStorageProvider, IConfiguration configuration, IFileScannerProvider fileScannerProvider, IMetadataResourceRepository metadataResourceRepository)
  : IRequestHandler<SaveFileCommand, SaveFileCommandResponse>, IRequestHandler<DeleteFileCommand>, IRequestHandler<FileQuery, FileQueryResults>

{
  public async Task<SaveFileCommandResponse> Handle(SaveFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var bucket = GetBucketName(configuration);
    var saveFileResults = new ConcurrentBag<SaveFileResult>();
    await Parallel.ForEachAsync(request.Items, cancellationToken, async (file, ct) =>
    {

      var tags = new Dictionary<string, string>(file.FileProperties.TagsList ?? Array.Empty<KeyValuePair<string, string>>());

      // Check if the classification property is not null or empty and the key doesn't already exist
      if (!string.IsNullOrEmpty(file.FileProperties.Classification) && !tags.ContainsKey("classification"))
      {
        tags.Add("classification", file.FileProperties.Classification);
      }

      var scanResult = await fileScannerProvider.ScanAsync(file.Content, ct);
      if (scanResult.IsClean)
      {
        await objectStorageProvider.StoreAsync(new S3Descriptor(bucket, file.FileLocation.Id, file.FileLocation.Folder), new FileObject(file.FileName, file.ContentType, file.Content, tags), ct);
        saveFileResults.Add(new SaveFileResult(file, true, "File Saved Successfully"));
      }
      else
      {
        saveFileResults.Add(new SaveFileResult(file, false, scanResult.Message));
      }
    });
    return new SaveFileCommandResponse(saveFileResults);
  }

  public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var bucket = GetBucketName(configuration);
    await objectStorageProvider.DeleteAsync(new S3Descriptor(bucket, request.Item.FileLocation.Id, request.Item.FileLocation.Folder), cancellationToken);
  }

  public async Task<FileQueryResults> Handle(FileQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(objectStorageProvider);
    ArgumentNullException.ThrowIfNull(configuration);

    var bucket = GetBucketName(configuration);
    var files = new ConcurrentBag<FileData>();
    await Parallel.ForEachAsync(request.FileLocations, cancellationToken, async (fileLocation, ct) =>
    {
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(bucket, fileLocation.Id, fileLocation.Folder), ct);
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

  private static string GetBucketName(IConfiguration configuration) =>
    configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}
