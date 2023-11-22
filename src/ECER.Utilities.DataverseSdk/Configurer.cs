using System.Diagnostics.CodeAnalysis;
using ECER.Infrastructure.Common;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace ECER.Utilities.DataverseSdk;

public class Configurer : IConfigureComponents
{
    public void Configure([NotNull] ConfigurationContext configurationContext)
    {
        configurationContext.Services.AddSingleton<IOrganizationService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ServiceClient>>();
            var client = new ServiceClient(configurationContext.Configuration.GetSection("Dataverse").GetValue("ConnectionString", string.Empty), logger);
            if (!client.IsReady) throw new InvalidOperationException($"Failed to connect to Dataverse: {client.LastError}", client.LastException);
            return client;
        });
        configurationContext.Services.AddScoped<EcerContext>();
    }
}
