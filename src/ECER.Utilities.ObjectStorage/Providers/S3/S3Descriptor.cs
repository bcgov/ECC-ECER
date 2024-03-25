namespace ECER.Utilities.ObjectStorage.Providers.S3;

/// <summary>
/// S3 descriptor
/// </summary>
/// <param name="BucketName">The bucket name</param>
/// <param name="Name">the file name, can be with extension</param>
/// <param name="Path">The file path</param>
public record S3Descriptor(string BucketName, string Name, string Path) : Descriptor
{
  public string Key => System.IO.Path.Combine(Path, Name).Replace('\\', '/');
}
