namespace ECER.Utilities.ObjectStorage.Providers.S3;
internal record S3StorageProviderSettings
{
  public string Url { get; set; } = null!;
  public string AccessKey { get; set; } = null!;
  public string SecretKey { get; set; } = null!;
  public string BucketName { get; set; } = null!;
}
