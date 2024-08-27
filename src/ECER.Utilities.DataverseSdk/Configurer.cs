using ECER.Infrastructure.Common;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Model;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.DataverseSdk;

public class Configurer : IConfigureComponents, IPostConfigureChecker
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddOptions<DataverseSettings>().Bind(configurationContext.Configuration.GetSection("Dataverse"));
    configurationContext.Services.AddSingleton<IOrganizationServiceAsync>(sp =>
    {
      var settings = sp.GetRequiredService<IOptions<DataverseSettings>>().Value;
      ServiceClient client;
      if (!string.IsNullOrWhiteSpace(settings.ConnectionString))

      {
        client = new ServiceClient(configurationContext.Configuration.GetSection("Dataverse").GetValue("ConnectionString", string.Empty));
      }
      else if (settings.ConnectionOptions != null)
      {
        settings.ConnectionOptions.Password = sp.GetRequiredService<IConfiguration>().GetSection("Dataverse:ConnectionOptions").GetValue<string>("Password").ToSecureString();
        client = new ServiceClient(settings.ConnectionOptions, true, settings.ConfigurationOptions);
        client.Connect();
      }
      else
      {
        throw new InvalidOperationException($"Dataverse is not configured properly");
      }

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

public record DataverseSettings
{
  public string? ConnectionString { get; set; }
  public ConnectionOptions? ConnectionOptions { get; set; }
  public ConfigurationOptions? ConfigurationOptions { get; set; }
}
