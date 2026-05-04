using ECER.Clients.PSPPortal.Server.Communications;
using ECER.Clients.PSPPortal.Server.Courses;
using ECER.Clients.PSPPortal.Server.EducationInstitutions;
using ECER.Clients.PSPPortal.Server.PortalInvitations;
using ECER.Clients.PSPPortal.Server.ProgramApplications;
using ECER.Clients.PSPPortal.Server.Programs;
using ECER.Clients.PSPPortal.Server.Users;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Clients.PSPPortal.Server;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ICommunicationMapper, CommunicationMapper>();
    configurationContext.Services.AddTransient<IConfigurationMapper, ConfigurationMapper>();
    configurationContext.Services.AddTransient<ICoursesMapper, CoursesMapper>();
    configurationContext.Services.AddTransient<IEducationInstitutionMapper, EducationInstitutionMapper>();
    configurationContext.Services.AddTransient<IPortalInvitationMapper, PortalInvitationMapper>();
    configurationContext.Services.AddTransient<IProgramApplicationsMapper, ProgramApplicationsMapper>();
    configurationContext.Services.AddTransient<IProgramMapper, ProgramMapper>();
    configurationContext.Services.AddTransient<IPspUserMapper, PspUserMapper>();
  }
}
