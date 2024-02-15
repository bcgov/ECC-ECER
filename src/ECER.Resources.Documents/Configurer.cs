using System.Diagnostics.CodeAnalysis;
using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Resources.Documents;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
  }
}
