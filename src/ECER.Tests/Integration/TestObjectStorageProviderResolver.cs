using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using System.Collections.Concurrent;

namespace ECER.Tests.Integration;

internal sealed class TestObjectStorageProviderResolver : IObjectStorageProviderResolver
{
  private readonly Dictionary<EcerWebApplicationType, IObjecStorageProvider> providers;

  public TestObjectStorageProviderResolver()
  {
    var store = new ConcurrentDictionary<string, StoredFile>();
    providers = new Dictionary<EcerWebApplicationType, IObjecStorageProvider>
    {
      [EcerWebApplicationType.Registry] = new InMemoryObjectStorageProvider("registry-test-bucket", store),
      [EcerWebApplicationType.PSP] = new InMemoryObjectStorageProvider("psp-test-bucket", store),
    };
  }

  public IObjecStorageProvider resolve(EcerWebApplicationType ecerWebApplicationType)
  {
    if (!providers.TryGetValue(ecerWebApplicationType, out var provider))
    {
      throw new ArgumentOutOfRangeException(nameof(ecerWebApplicationType), ecerWebApplicationType, null);
    }

    return provider;
  }

  private sealed record StoredFile(string FileName, string ContentType, byte[] Content, IReadOnlyDictionary<string, string>? Tags);

  private sealed class InMemoryObjectStorageProvider(string bucketName, ConcurrentDictionary<string, StoredFile> store) : IObjecStorageProvider
  {
    public string BucketName => bucketName;

    public async Task StoreAsync(Descriptor destination, FileObject obj, CancellationToken ct)
    {
      var descriptor = destination.DownCastOrThrow<Descriptor, S3Descriptor>();
      using var buffer = new MemoryStream();
      await obj.Content.CopyToAsync(buffer, ct);
      var tags = obj.Tags?.ToDictionary(t => t.Key, t => t.Value);
      store[BuildKey(descriptor)] = new StoredFile(obj.FileName, obj.ContentType, buffer.ToArray(), tags);
    }

    public Task DeleteAsync(Descriptor source, CancellationToken ct)
    {
      var descriptor = source.DownCastOrThrow<Descriptor, S3Descriptor>();
      store.TryRemove(BuildKey(descriptor), out _);
      return Task.CompletedTask;
    }

    public Task<FileObject?> GetAsync(Descriptor source, CancellationToken ct)
    {
      var descriptor = source.DownCastOrThrow<Descriptor, S3Descriptor>();
      if (!store.TryGetValue(BuildKey(descriptor), out var file))
      {
        return Task.FromResult<FileObject?>(null);
      }

      var stream = new MemoryStream(file.Content, writable: false);
      return Task.FromResult<FileObject?>(new FileObject(file.FileName, file.ContentType, stream, file.Tags));
    }

    public Task CopyAsync(Descriptor source, Descriptor destination, CancellationToken ct)
    {
      var sourceDescriptor = source.DownCastOrThrow<Descriptor, S3Descriptor>();
      var destinationDescriptor = destination.DownCastOrThrow<Descriptor, S3Descriptor>();
      if (store.TryGetValue(BuildKey(sourceDescriptor), out var file))
      {
        store[BuildKey(destinationDescriptor)] = file with { Content = file.Content.ToArray() };
      }

      return Task.CompletedTask;
    }

    public async Task MoveAsync(Descriptor source, Descriptor destination, CancellationToken ct)
    {
      await CopyAsync(source, destination, ct);
      await DeleteAsync(source, ct);
    }

    private static string BuildKey(S3Descriptor descriptor) => $"{descriptor.BucketName}/{descriptor.Key}";
  }
}
