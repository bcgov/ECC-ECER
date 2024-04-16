using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Managers.Registry;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ApplicationHandlers>();
    configurationContext.Services.AddTransient<CommunicationHandlers>();
    configurationContext.Services.AddTransient<RegistrantHandlers>();
    configurationContext.Services.AddTransient<PortalInvitationHandlers>();
    configurationContext.Services.AddTransient<ReferenceHandlers>();
  }
}
