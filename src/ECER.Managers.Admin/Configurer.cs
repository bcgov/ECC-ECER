using ECER.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Managers.Admin;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.Configure<PortalAppSettings>(portalAppSettings =>
    configurationContext.Configuration.GetSection("PortalApp").Bind(portalAppSettings));
    configurationContext.Services.AddTransient<FileHandlers>();
    configurationContext.Services.AddTransient<InviteLinkHandlers>();
    configurationContext.Services.AddTransient<MetadataHandlers>();
  }
}

public record PortalAppSettings : IOptions<PortalAppSettings>
{
  public string BaseUrl { get; set; } = string.Empty;
  public string PspBaseUrl { get; set; } = string.Empty;
  public string ReferenceVerificationRoute { get; set; } = string.Empty;
  public PortalAppSettings Value => this;
}
