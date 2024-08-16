using ECER.Infrastructure.Common;
using ECER.Utilities.FileScanner.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nClam;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.FileScanner;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var clamAvSettings = configurationContext.Configuration.GetSection("fileScanner").Get<ClamAvProviderSettings>();
    if (clamAvSettings != null && !string.IsNullOrWhiteSpace(clamAvSettings.Url))
    {
      configurationContext.logger.LogInformation("Configuring file scanning using {Url}:{Port}", clamAvSettings.Url, clamAvSettings.Port);
      configurationContext.Services.AddSingleton(_ =>
        new ClamClient(
          clamAvSettings.Url,
          clamAvSettings.Port
          ));
      configurationContext.Services.AddSingleton<IFileScannerProvider, ClamAvScanner>();
    }
    else
    {
      configurationContext.logger.LogWarning("FileScanner:Url is empty - files will not be scanned");
      configurationContext.Services.AddSingleton<IFileScannerProvider, NoopScanner>();
    }
  }
}
