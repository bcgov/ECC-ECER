namespace ECER.Utilities.ObjectStorage.Providers.S3;
internal record S3StorageProviderSettings
{
  public string Url { get; set; } = null!;
  public string AccessKey { get; set; } = null!;
  public string SecretKey { get; set; } = null!;
  public string BucketName { get; set; } = null!;
}

internal record ObjectStorageSettings {
  public string Url { get; set; } = null!;
  public string AccessKey { get; set; } = null!;
  public string SecretKey { get; set; } = null!;
  public string BucketName { get; set; } = null!;
  //TODO GET RID OF THE ABOVE
  public S3StorageProviderSettings Psp { get; set; } = null!;
  public S3StorageProviderSettings Registry { get; set; } = null!;

}
