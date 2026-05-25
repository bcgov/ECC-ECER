namespace ECER.Utilities.ObjectStorage.Providers.S3;
internal record S3StorageProviderSettings
{
  public string Url { get; set; } = null!;
  public string AccessKey { get; set; } = null!;
  public string SecretKey { get; set; } = null!;
  public string BucketName { get; set; } = null!;
}

internal record ObjectStorageSettings
{
  public string? Url { get; set; }
  public string? AccessKey { get; set; }
  public string? SecretKey { get; set; }
  public string? BucketName { get; set; }
  public S3StorageProviderSettings? Psp { get; set; }
  public S3StorageProviderSettings? Registry { get; set; }

  public S3StorageProviderSettings? ResolveRegistrySettings()
  {
    if (Registry != null) return Registry;
    if (string.IsNullOrWhiteSpace(Url) ||
        string.IsNullOrWhiteSpace(AccessKey) ||
        string.IsNullOrWhiteSpace(SecretKey) ||
        string.IsNullOrWhiteSpace(BucketName))
    {
      return null;
    }

    return new S3StorageProviderSettings
    {
      Url = Url,
      AccessKey = AccessKey,
      SecretKey = SecretKey,
      BucketName = BucketName,
    };
  }
}
