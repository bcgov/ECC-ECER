using ECER.Infrastructure.Common;
using ECER.Resources.E2ETests.Applications;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Resources.E2ETests;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
  }
}

