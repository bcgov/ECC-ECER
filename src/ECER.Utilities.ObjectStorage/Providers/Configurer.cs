using Amazon.S3;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers.Dataverse;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.ObjectStorage.Providers;

public class Configurer : IConfigureComponents, IPostConfigureChecker, IProvideInstrumentationSources
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var settings = GetSettings(configurationContext.Configuration);

    if (settings != null && settings.Url != null)
    {
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
    else
    {
      configurationContext.Services.AddSingleton<IObjecStorageProvider, DataverseProvider>();
    }
  }

  public async Task Check([NotNull] CheckContext context, CancellationToken ct)
  {
    var settings = GetSettings(context.Configuration);
    var client = context.Services.GetRequiredService<IAmazonS3>();

    if (settings != null && settings.Url != null)
    {
      await client.ListBucketsAsync(ct);
    }
  }

  private static S3StorageProviderSettings? GetSettings(IConfiguration configuration)
    => configuration.GetSection("objectStorage").Get<S3StorageProviderSettings>();

  public InstrumentationSources GetInstrumentationSources() => new InstrumentationSources { TraceSources = [S3Provider.TraceSourceName] };
}
