using Amazon.Runtime;

namespace ECER.Utilities.ObjectStorage.Providers.S3;

internal static class Extentions
{
  public static void EnsureSuccess(this AmazonWebServiceResponse response)
  {
    var statusCode = ((int)response.HttpStatusCode);
    if (statusCode < 200 || statusCode >= 300) throw new InvalidOperationException($"Operation {response.ResponseMetadata.RequestId} failed with status {response.HttpStatusCode}");
  }
}
