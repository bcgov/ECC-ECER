using ECER.Engines.Transformation.InviteLinks;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Engines.Transformation;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IInviteLinkTransformationEngine, InviteLinkTransformationEngine>();
  }
}
