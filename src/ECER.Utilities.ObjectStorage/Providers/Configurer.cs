using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Utilities.ObjectStorage.Providers;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var settings = configurationContext.Configuration.GetSection("objectStorage").Get<S3StorageProviderSettings>();
    if (settings == null) return;

    configurationContext.Services.AddSingleton<IAmazonS3>(_ =>
      new AmazonS3Client(
        settings.AccessKey,
        settings.SecretKey,
        new AmazonS3Config
        {
          ServiceURL = settings.Url,
          ForcePathStyle = true,
        }));
    configurationContext.Services.AddSingleton<IObjecStorageProvider, S3Provider>();
  }
}
