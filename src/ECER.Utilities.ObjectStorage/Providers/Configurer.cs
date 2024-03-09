using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers.Dataverse;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Utilities.ObjectStorage.Providers;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var s3settings = configurationContext.Configuration.GetSection("objectStorage").Get<S3StorageProviderSettings>();
    if (s3settings != null)
    {
      configurationContext.Services.AddSingleton<IAmazonS3>(_ =>
        new AmazonS3Client(
          s3settings.AccessKey,
          s3settings.SecretKey,
          new AmazonS3Config
          {
            ServiceURL = s3settings.Url,
            ForcePathStyle = true,
          }));
      configurationContext.Services.AddSingleton<IObjecStorageProvider, S3Provider>();
    }
    else
    {
      configurationContext.Services.AddSingleton<IObjecStorageProvider, DataverseProvider>();
    }
  }
}
