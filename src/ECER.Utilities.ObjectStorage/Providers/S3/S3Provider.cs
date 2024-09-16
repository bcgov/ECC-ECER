using Amazon.S3;
using Amazon.S3.Model;
using System.Diagnostics;

namespace ECER.Utilities.ObjectStorage.Providers.S3;

internal class S3Provider(IAmazonS3 s3Client) : IObjecStorageProvider
{
  internal static readonly string TraceSourceName = typeof(S3Provider).FullName!;
  private static ActivitySource activitySource = new(TraceSourceName);

  public async Task StoreAsync(Descriptor destination, FileObject obj, CancellationToken ct)
  {
    if (!(destination is S3Descriptor s3destination)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(obj));

    using var activity = activitySource.StartActivity(nameof(StoreAsync));
    activity?.SetTag(nameof(s3destination.Key), s3destination.Key);
    activity?.SetTag(nameof(s3destination.BucketName), s3destination.BucketName);
    activity?.SetTag(nameof(s3destination.Path), s3destination.Path);
    activity?.SetTag(nameof(s3destination.Name), s3destination.Name);
    activity?.SetTag(nameof(obj.Content.Length), obj.Content.Length);

    var request = new PutObjectRequest
    {
      Key = s3destination.Key,
      BucketName = s3destination.BucketName,
      ContentType = obj.ContentType,
      InputStream = obj.Content,
      TagSet = obj.Tags?.Select(t => new Tag { Key = t.Key, Value = t.Value }).ToList()
    };
    request.Metadata.Add("filename", obj.FileName);

    var response = await s3Client.PutObjectAsync(request, ct);
    response.EnsureSuccess();
  }

  public async Task<FileObject?> GetAsync(Descriptor source, CancellationToken ct)
  {
    if (!(source is S3Descriptor s3source)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(source));

    using var activity = activitySource.StartActivity(nameof(GetAsync));
    activity?.SetTag(nameof(s3source.Key), s3source.Key);
    activity?.SetTag(nameof(s3source.BucketName), s3source.BucketName);
    activity?.SetTag(nameof(s3source.Path), s3source.Path);
    activity?.SetTag(nameof(s3source.Name), s3source.Name);

    var request = new GetObjectRequest
    {
      BucketName = s3source.BucketName,
      Key = s3source.Key,
    };
    try
    {
      var objectResponse = await s3Client.GetObjectAsync(request, ct);
      objectResponse.EnsureSuccess();

      var tagResponse = await s3Client.GetObjectTaggingAsync(new GetObjectTaggingRequest { BucketName = s3source.BucketName, Key = s3source.Key }, ct);
      tagResponse.EnsureSuccess();

      activity?.SetTag(nameof(objectResponse.ContentLength), objectResponse.ContentLength);

      return new FileObject(objectResponse.Metadata["filename"], objectResponse.Headers.ContentType, objectResponse.ResponseStream, tagResponse.Tagging.ToDictionary(t => t.Key, t => t.Value));
    }
    catch (AmazonS3Exception e) when (e.Message.Equals("The specified key does not exist."))
    {
      return null;
    }
  }

  public async Task DeleteAsync(Descriptor source, CancellationToken ct)
  {
    if (!(source is S3Descriptor s3source)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(source));

    using var activity = activitySource.StartActivity(nameof(DeleteAsync));
    activity?.SetTag(nameof(s3source.Key), s3source.Key);
    activity?.SetTag(nameof(s3source.BucketName), s3source.BucketName);
    activity?.SetTag(nameof(s3source.Path), s3source.Path);
    activity?.SetTag(nameof(s3source.Name), s3source.Name);

    var request = new DeleteObjectRequest
    {
      BucketName = s3source.BucketName,
      Key = s3source.Key
    };

    var response = await s3Client.DeleteObjectAsync(request, ct);
    response.EnsureSuccess();
  }

  public async Task CopyAsync(Descriptor source, Descriptor destination, CancellationToken ct)
  {
    if (!(source is S3Descriptor s3source)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(source));
    if (!(destination is S3Descriptor s3destination)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(destination));

    using var activity = activitySource.StartActivity(nameof(CopyAsync));
    activity?.SetTag(nameof(s3source.Key), s3source.Key);
    activity?.SetTag(nameof(s3source.BucketName), s3source.BucketName);
    activity?.SetTag(nameof(s3source.Path), s3source.Path);
    activity?.SetTag(nameof(s3source.Name), s3source.Name);
    activity?.SetTag(nameof(s3destination.Key), s3destination.Key);
    activity?.SetTag(nameof(s3destination.BucketName), s3destination.BucketName);
    activity?.SetTag(nameof(s3destination.Path), s3destination.Path);
    activity?.SetTag(nameof(s3destination.Name), s3destination.Name);

    var request = new CopyObjectRequest
    {
      SourceBucket = s3source.BucketName,
      SourceKey = s3source.Key,
      DestinationBucket = s3destination.BucketName,
      DestinationKey = s3destination.Key,
    };

    var response = await s3Client.CopyObjectAsync(request, ct);
    response.EnsureSuccess();
  }

  public async Task MoveAsync(Descriptor source, Descriptor destination, CancellationToken ct)
  {
    using var activity = activitySource.StartActivity(nameof(MoveAsync));

    await CopyAsync(source, destination, ct);
    await DeleteAsync(source, ct);
  }
}
