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
    //if (settings != null && settings.Url != null)
    //{
    //  configurationContext.Services.AddSingleton<IAmazonS3>(_ =>
    //    new AmazonS3Client(
    //      settings.AccessKey,
    //      settings.SecretKey,
    //      new AmazonS3Config
    //      {
    //        ServiceURL = settings.Url,
    //        ForcePathStyle = true,
    //      }));
    //  configurationContext.Services.AddSingleton<IObjecStorageProvider, S3Provider>();
    //}

    if (settings != null && settings.Psp != null)
    {
      configurationContext.logger.LogInformation("setting up PSP");
      configurationContext.Services.AddKeyedSingleton<IAmazonS3>(EcerWebApplicationType.Psp, (_, _) =>
        new AmazonS3Client(
          settings.Psp.AccessKey,
          settings.Psp.SecretKey,
          new AmazonS3Config
          {
            ServiceURL = settings.Psp.Url,
            ForcePathStyle = true,
          }));
      configurationContext.Services.AddKeyedSingleton<IObjecStorageProvider, S3Provider>(EcerWebApplicationType.Psp, (sp, key) =>
      {
        var client = sp.GetRequiredKeyedService<IAmazonS3>(key);
        return new S3Provider(client, settings.Psp.BucketName);
      });
    }

    if (settings != null && settings.Registry != null)
    {
      configurationContext.logger.LogInformation("setting up Registry");
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
    //TODO Determine if we need this?
    //else
    //{
    //  configurationContext.Services.AddSingleton<IObjecStorageProvider, DataverseProvider>();
    //}

    //adding the resolver here
    configurationContext.Services.AddSingleton<IObjectStorageProviderResolver, ObjectStorageProviderResolver>();
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

  private static ObjectStorageSettings? GetSettings(IConfiguration configuration)
    => configuration.GetSection("objectStorage").Get<ObjectStorageSettings>();

  public InstrumentationSources GetInstrumentationSources() => new InstrumentationSources { TraceSources = [S3Provider.TraceSourceName] };
}
