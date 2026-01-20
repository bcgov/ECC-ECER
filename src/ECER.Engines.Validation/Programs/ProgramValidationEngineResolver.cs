using ECER.Managers.Registry.Contract.Programs;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Engines.Validation.Programs;

public interface IProgramValidationEngineResolver
{
  IProgramValidationEngine resolve(ProgramProfileType programType);
}

public class ProgramValidationEngineResolver : IProgramValidationEngineResolver
{
  private readonly IServiceProvider _serviceProvider;

  public ProgramValidationEngineResolver(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public IProgramValidationEngine resolve(ProgramProfileType programType)
  {
    return programType switch
    {
      ProgramProfileType.AnnualReview => _serviceProvider.GetRequiredService<NewProgramSubmissionValidationEngine>(),
      _ => throw new ArgumentOutOfRangeException(nameof(programType), programType, null)
    };
  }
}
