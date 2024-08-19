using ECER.Infrastructure.Common;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.DataverseSdk;

public class Configurer : IConfigureComponents, IPostConfigureChecker
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddSingleton<IOrganizationServiceAsync>(sp =>
    {
      var logger = sp.GetRequiredService<ILogger<ServiceClient>>();
      var client = new ServiceClient(configurationContext.Configuration.GetSection("Dataverse").GetValue("ConnectionString", string.Empty), logger);
      if (!client.IsReady) throw new InvalidOperationException($"Failed to connect to Dataverse: {client.LastError}", client.LastException);
      return client;
    });
    configurationContext.Services.AddScoped(sp =>
    {
      var client = sp.GetRequiredService<IOrganizationServiceAsync>();
      return new EcerContext(client);
    });
  }

  public async Task Check([NotNull] CheckContext context, CancellationToken ct)
  {
    var ctx = context.Services.GetRequiredService<IOrganizationServiceAsync>();
    await ctx.ExecuteAsync(new WhoAmIRequest());
  }
}
