using ECER.Clients.RegistryPortal.Server.Applications;
using ECER.Clients.RegistryPortal.Server.Certifications;
using ECER.Clients.RegistryPortal.Server.Communications;
using ECER.Clients.RegistryPortal.Server.ICRA;
using ECER.Clients.RegistryPortal.Server.References;
using ECER.Clients.RegistryPortal.Server.Users;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Clients.RegistryPortal.Server;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IApplicationMapper, ApplicationMapper>();
    configurationContext.Services.AddTransient<ICertificationMapper, CertificationMapper>();
    configurationContext.Services.AddTransient<ICommunicationMapper, CommunicationMapper>();
    configurationContext.Services.AddTransient<IConfigurationMapper, ConfigurationMapper>();
    configurationContext.Services.AddTransient<IICRAEligibilityMapper, ICRAEligibilityMapper>();
    configurationContext.Services.AddTransient<IPortalInvitationMapper, PortalInvitationMapper>();
    configurationContext.Services.AddTransient<IReferencesMapper, ReferencesMapper>();
    configurationContext.Services.AddTransient<IUserMapper, UserMapper>();
  }
}
