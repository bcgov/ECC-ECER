using System.Diagnostics.CodeAnalysis;
using ECER.Engines.Validation.Applications;
using ECER.Engines.Validation.ICRA;
using ECER.Engines.Validation.ProgramApplications;
using ECER.Engines.Validation.Programs;
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
    configurationContext.Services.AddTransient<IcraApplicationSubmissionValidationEngine>();
    configurationContext.Services.AddTransient<IApplicationValidationEngineResolver, ApplicationValidationEngineResolver>();
    configurationContext.Services.AddTransient<IApplicationValidationEngine>(provider =>
    {
      return provider.GetRequiredService<NewApplicationSubmissionValidationEngine>();
    });

    // ICRA
    configurationContext.Services.AddTransient<IICRAValidationEngine, IcraEligibilitySubmissionValidationEngine>();
    
    //Program
    configurationContext.Services.AddTransient<NewProgramSubmissionValidationEngine>();
    configurationContext.Services.AddTransient<IProgramValidationEngineResolver, ProgramValidationEngineResolver>();
    configurationContext.Services.AddTransient<IProgramValidationEngine>(provider =>
    {
      return provider.GetRequiredService<NewProgramSubmissionValidationEngine>();
    });

    // Program Application
    configurationContext.Services.AddTransient<IProgramApplicationValidationEngine, ProgramApplicationSubmissionValidationEngine>();

    // Course progress
    configurationContext.Services.AddTransient<ICourseProgressEvaluator, CourseProgressEvaluator>();
  }
}
