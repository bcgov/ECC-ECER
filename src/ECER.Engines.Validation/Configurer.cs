using System.Diagnostics.CodeAnalysis;
using ECER.Engines.Validation.Applications;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Engines.Validation;

public class Configurer : IConfigureComponents
{
    public void Configure([NotNull] ConfigurationContext configurationContext)
    {
        configurationContext.Services.AddTransient<IApplicationValidationEngine, ApplicationValidationEngine>();
    }
}
