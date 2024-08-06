using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using ECER.Infrastructure.Common;
using ECER.Utilities.FileScanner.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nClam;

namespace ECER.Utilities.FileScanner;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    var clamAvSettings = configurationContext.Configuration.GetSection("fileScanner").Get<ClamAvProviderSettings>();
    if (clamAvSettings != null)
    {
      configurationContext.Services.AddSingleton<ClamClient>(_ =>
        new ClamClient(
          clamAvSettings.Url,
          clamAvSettings.Port
          ));
      configurationContext.Services.AddSingleton<IFileScannerProvider, ClamAvScanner>();
    }
    else
    {
      throw new ConfigurationErrorsException("fileScanner configuration not found");
    }
  }
}
