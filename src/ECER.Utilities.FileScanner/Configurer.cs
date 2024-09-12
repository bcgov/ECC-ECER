using ECER.Infrastructure.Common;
using ECER.Utilities.FileScanner.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nClam;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.FileScanner;

public class Configurer : IConfigureComponents, IPostConfigureChecker
{
  private const int maxFileSize = 100_000_000;

  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var clamAvSettings = GetSettings(configurationContext.Configuration);
    if (clamAvSettings != null && !string.IsNullOrWhiteSpace(clamAvSettings.Url))
    {
      configurationContext.logger.LogInformation("Configuring file scanning using {Url}:{Port}", clamAvSettings.Url, clamAvSettings.Port);
      configurationContext.Services.AddSingleton<IClamClient>(_ =>
        new ClamClient(
          clamAvSettings.Url,
          clamAvSettings.Port
          )
        { MaxStreamSize = maxFileSize });
      configurationContext.Services.AddTransient<IFileScannerProvider, ClamAvScanner>();
    }
    else
    {
      configurationContext.logger.LogWarning("FileScanner:Url is empty - files will not be scanned");
      configurationContext.Services.AddTransient<IFileScannerProvider, NoopScanner>();
    }
  }

  public async Task Check([NotNull] CheckContext context, CancellationToken ct)
  {
    var clamAvSettings = GetSettings(context.Configuration);
    if (clamAvSettings != null && !string.IsNullOrWhiteSpace(clamAvSettings.Url))
    {
      var client = context.Services.GetRequiredService<IClamClient>();
      await client.PingAsync(ct);
    }
  }

  private static ClamAvProviderSettings? GetSettings(IConfiguration configuration)
    => configuration.GetSection("fileScanner").Get<ClamAvProviderSettings>();
}
