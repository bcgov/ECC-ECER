using System.Collections.Concurrent;
using ECER.Managers.Admin.Contract.Files;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;

namespace ECER.Managers.Admin;

public static class FileHandlers
{
  public static async Task Handle(SaveFileCommand cmd, IObjecStorageProvider objectStorageProvider, IConfiguration configuration, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(cmd);
    ArgumentNullException.ThrowIfNull(objectStorageProvider);
    ArgumentNullException.ThrowIfNull(configuration);

    var bucket = GetBucketName(configuration);
    await Parallel.ForEachAsync(cmd.Items, ct, async (file, ct) =>
    {
      await objectStorageProvider.StoreAsync(new S3Descriptor(bucket, file.FileLocation.Id, file.FileLocation.Folder), new FileObject(file.FileName, file.ContentType, file.Content), ct);
    });
  }

  public static async Task<FIleQueryResults> Handle(FileQuery query, IObjecStorageProvider objectStorageProvider, IConfiguration configuration, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(query);
    ArgumentNullException.ThrowIfNull(objectStorageProvider);
    ArgumentNullException.ThrowIfNull(configuration);

    var bucket = GetBucketName(configuration);
    var files = new ConcurrentBag<FileData>();
    await Parallel.ForEachAsync(query.FileLocations, ct, async (fileLocation, ct) =>
    {
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(bucket, fileLocation.Id, fileLocation.Folder), ct);

      if (file != null) files.Add(new FileData(fileLocation, file.FileName, file.ContentType, file.Content));
    });

    return new FIleQueryResults(files.ToList());
  }

  private static string GetBucketName(IConfiguration configuration) =>
    configuration.GetValue<string>("objectStorage:bucketName") ?? throw new InvalidOperationException("objectStorage:bucketName is not set");
}
