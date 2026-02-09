using ECER.Managers.Registry.Contract.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Engines.Validation.Applications;

public interface IApplicationValidationEngineResolver
{
  IApplicationValidationEngine Resolve(ApplicationTypes appType);
}

public class ApplicationValidationEngineResolver : IApplicationValidationEngineResolver
{
  private readonly IServiceProvider _serviceProvider;

  public ApplicationValidationEngineResolver(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public IApplicationValidationEngine Resolve(ApplicationTypes appType)
  {
    return appType switch
    {
      ApplicationTypes.New => _serviceProvider.GetRequiredService<NewApplicationSubmissionValidationEngine>(),
      ApplicationTypes.LabourMobility => _serviceProvider.GetRequiredService<LabourMobilityApplicationSubmissionValidationEngine>(),
      ApplicationTypes.Renewal => _serviceProvider.GetRequiredService<ApplicationRenewalValidationEngine>(),
      ApplicationTypes.ICRA => _serviceProvider.GetRequiredService<IcraApplicationSubmissionValidationEngine>(),
      _ => throw new ArgumentOutOfRangeException(nameof(appType), appType, null)
    };
  }
}
