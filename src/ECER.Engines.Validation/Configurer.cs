using System.Diagnostics.CodeAnalysis;
using ECER.Engines.Validation.Applications;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Engines.Validation;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<NewApplicationSubmissionValidationEngine>();
    configurationContext.Services.AddTransient<LabourMobilityApplicationSubmissionValidationEngine>();
    configurationContext.Services.AddTransient<ApplicationRenewalValidationEngine>();
    configurationContext.Services.AddTransient<IApplicationValidationEngineResolver, ApplicationValidationEngineResolver>();
    configurationContext.Services.AddTransient<IApplicationValidationEngine>(provider =>
    {
      return provider.GetRequiredService<NewApplicationSubmissionValidationEngine>();
    });
  }
}
