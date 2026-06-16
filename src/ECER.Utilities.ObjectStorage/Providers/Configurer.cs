using Amazon.Runtime;
using Amazon.S3;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.ObjectStorage.Providers;

public class Configurer : IConfigureComponents, IPostConfigureChecker, IProvideInstrumentationSources
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var settings = GetSettings(configurationContext.Configuration);
    var pspSettings = settings?.Psp;
    var registrySettings = settings?.Registry;

    if (pspSettings != null)
    {
      configurationContext.logger.LogInformation("setting up PSP S3 connection");
      configurationContext.Services.AddKeyedSingleton<IAmazonS3>(EcerWebApplicationType.PSP, (_, _) =>
        new AmazonS3Client(
          pspSettings.AccessKey,
          pspSettings.SecretKey,
          new AmazonS3Config
          {
            ServiceURL = pspSettings.Url,
            ForcePathStyle = true,
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
          }));
      configurationContext.Services.AddKeyedSingleton<IObjecStorageProvider, S3Provider>(EcerWebApplicationType.PSP, (sp, key) =>
      {
        var client = sp.GetRequiredKeyedService<IAmazonS3>(key);
        return new S3Provider(client, pspSettings.BucketName);
      });
    }

    if (registrySettings != null)
    {
      configurationContext.logger.LogInformation("setting up Registry S3 connection");
      configurationContext.Services.AddKeyedSingleton<IAmazonS3>(EcerWebApplicationType.Registry, (_, _) =>
        new AmazonS3Client(
          registrySettings.AccessKey,
          registrySettings.SecretKey,
          new AmazonS3Config
          {
            ServiceURL = registrySettings.Url,
            ForcePathStyle = true,
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
          }));
      configurationContext.Services.AddKeyedSingleton<IObjecStorageProvider, S3Provider>(EcerWebApplicationType.Registry, (sp, key) =>
      {
        var client = sp.GetRequiredKeyedService<IAmazonS3>(key);
        return new S3Provider(client, registrySettings.BucketName);
      });
    }

    //adding the resolver here
    configurationContext.Services.AddSingleton<IObjectStorageProviderResolver, ObjectStorageProviderResolver>();
  }

  public async Task Check([NotNull] CheckContext context, CancellationToken ct)
  {
    var settings = GetSettings(context.Configuration);
    var pspSettings = settings?.Psp;
    var registrySettings = settings?.Registry;

    if (pspSettings != null)
    {
      var pspClient = context.Services.GetRequiredKeyedService<IAmazonS3>(EcerWebApplicationType.PSP);
      await pspClient.ListBucketsAsync(ct);
    }

    if (registrySettings != null)
    {
      var registryClient = context.Services.GetRequiredKeyedService<IAmazonS3>(EcerWebApplicationType.Registry);
      await registryClient.ListBucketsAsync(ct);
    }
  }

  private static ObjectStorageSettings? GetSettings(IConfiguration configuration)
    => configuration.GetSection("objectStorage").Get<ObjectStorageSettings>();

  public InstrumentationSources GetInstrumentationSources() => new InstrumentationSources { TraceSources = [S3Provider.TraceSourceName] };
}
