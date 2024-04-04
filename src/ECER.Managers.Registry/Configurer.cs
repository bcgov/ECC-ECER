using System.Diagnostics.CodeAnalysis;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Managers.Registry;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ApplicationHandlers>();
    configurationContext.Services.AddTransient<CommunicationHandler>();
    configurationContext.Services.AddTransient<RegistrantHandlers>();
  }
}
