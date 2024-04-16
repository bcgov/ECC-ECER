using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Resources.Documents.References;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Resources.Documents;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
    configurationContext.Services.AddTransient<IPortalInvitationRepository, PortalInvitationRepository>();
    configurationContext.Services.AddTransient<IReferenceRepository, ReferenceRepository>();
  }
}
