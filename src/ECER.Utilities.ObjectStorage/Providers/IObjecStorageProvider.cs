namespace ECER.Utilities.ObjectStorage.Providers;

public interface IObjecStorageProvider
{
  string BucketName { get; }

  Task StoreAsync(Descriptor destination, FileObject obj, CancellationToken ct);

  Task DeleteAsync(Descriptor source, CancellationToken ct);

  Task<FileObject?> GetAsync(Descriptor source, CancellationToken ct);

  Task CopyAsync(Descriptor source, Descriptor destination, CancellationToken ct);

  Task MoveAsync(Descriptor source, Descriptor destination, CancellationToken ct);
}

public record Descriptor();

public record FileObject(string FileName, string ContentType, Stream Content, IEnumerable<KeyValuePair<string, string>>? Tags);

public enum EcerWebApplicationType
{
  PSP,
  Registry
}
