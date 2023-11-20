using System.Diagnostics.CodeAnalysis;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Resources.Applications;

public class Configurer : IConfigureComponents
{
    public void Configure([NotNull] ConfigurationContext configurationContext)
    {
        configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
    }
}
