using Amazon.S3;
using Amazon.S3.Model;

namespace ECER.Utilities.ObjectStorage.Providers.S3;

internal class S3Provider(IAmazonS3 s3Client) : IObjecStorageProvider
{
  public async Task StoreAsync(Descriptor destination, FileObject obj, CancellationToken ct)
  {
    if (!(destination is S3Descriptor s3destination)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(obj));

    var request = new PutObjectRequest
    {
      Key = s3destination.Key,
      BucketName = s3destination.BucketName,
      ContentType = obj.ContentType,
      InputStream = obj.Content,
    };
    request.Metadata.Add("filename", obj.FileName);

    var response = await s3Client.PutObjectAsync(request, ct);
    response.EnsureSuccess();
  }

  public async Task<FileObject?> GetAsync(Descriptor source, CancellationToken ct)
  {
    if (!(source is S3Descriptor s3source)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(source));

    var request = new GetObjectRequest
    {
      BucketName = s3source.BucketName,
      Key = s3source.Key,
    };
    try
    {
      var response = await s3Client.GetObjectAsync(request, ct);
      response.EnsureSuccess();

      return new FileObject(response.Metadata["filename"], response.Headers.ContentType, response.ResponseStream);
    }
    catch (AmazonS3Exception e) when (e.Message.Equals("The specified key does not exist."))
    {
      return null;
    }
  }

  public async Task DeleteAsync(Descriptor source, CancellationToken ct)
  {
    if (!(source is S3Descriptor s3source)) throw new ArgumentException($"Must be of {typeof(S3Descriptor).Name}", nameof(source));
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
    await CopyAsync(source, destination, ct);
    await DeleteAsync(source, ct);
  }
}
