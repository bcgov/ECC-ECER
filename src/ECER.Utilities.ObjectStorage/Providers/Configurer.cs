using Amazon.S3;
using ECER.Infrastructure.Common;
using ECER.Utilities.ObjectStorage.Providers.Dataverse;
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

    if (settings != null && settings.Psp != null)
    {
      configurationContext.logger.LogInformation("setting up PSP S3 connection");
      configurationContext.Services.AddKeyedSingleton<IAmazonS3>(EcerWebApplicationType.PSP, (_, _) =>
        new AmazonS3Client(
          settings.Psp.AccessKey,
          settings.Psp.SecretKey,
          new AmazonS3Config
          {
            ServiceURL = settings.Psp.Url,
            ForcePathStyle = true,
          }));
      configurationContext.Services.AddKeyedSingleton<IObjecStorageProvider, S3Provider>(EcerWebApplicationType.PSP, (sp, key) =>
      {
        var client = sp.GetRequiredKeyedService<IAmazonS3>(key);
        return new S3Provider(client, settings.Psp.BucketName);
      });
    }

    if (settings != null && settings.Registry != null)
    {
      configurationContext.logger.LogInformation("setting up Registry S3 connection");
      configurationContext.Services.AddKeyedSingleton<IAmazonS3>(EcerWebApplicationType.Registry, (_, _) =>
        new AmazonS3Client(
          settings.Registry.AccessKey,
          settings.Registry.SecretKey,
          new AmazonS3Config
          {
            ServiceURL = settings.Registry.Url,
            ForcePathStyle = true,
          }));
      configurationContext.Services.AddKeyedSingleton<IObjecStorageProvider, S3Provider>(EcerWebApplicationType.Registry, (sp, key) =>
      {
        var client = sp.GetRequiredKeyedService<IAmazonS3>(key);
        return new S3Provider(client, settings.Registry.BucketName);
      });
    }

    //adding the resolver here
    configurationContext.Services.AddSingleton<IObjectStorageProviderResolver, ObjectStorageProviderResolver>();
  }

  public async Task Check([NotNull] CheckContext context, CancellationToken ct)
  {
    var settings = GetSettings(context.Configuration);
    
    if (settings?.Psp != null)
    {
      var pspClient = context.Services.GetRequiredKeyedService<IAmazonS3>(EcerWebApplicationType.PSP);
      await pspClient.ListBucketsAsync(ct);
    }

    if (settings?.Registry != null)
    {
      var registryClient = context.Services.GetRequiredKeyedService<IAmazonS3>(EcerWebApplicationType.Registry);
      await registryClient.ListBucketsAsync(ct);
    }
  }

  private static ObjectStorageSettings? GetSettings(IConfiguration configuration)
    => configuration.GetSection("objectStorage").Get<ObjectStorageSettings>();

  public InstrumentationSources GetInstrumentationSources() => new InstrumentationSources { TraceSources = [S3Provider.TraceSourceName] };
}
