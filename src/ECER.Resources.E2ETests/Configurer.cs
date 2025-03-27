using ECER.Infrastructure.Common;
using ECER.Resources.E2ETests.E2ETestsContacts;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Resources.E2ETests;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IE2ETestsContactRepository, E2ETestsContactRepository>();
  }
}

