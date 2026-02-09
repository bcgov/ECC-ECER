using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Certifications;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.PortalInvitations;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using ECER.Resources.Documents.Programs;

namespace ECER.Resources.Documents;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ICertificationRepository, CertificationRepository>();
    configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
    configurationContext.Services.AddTransient<IPortalInvitationRepository, PortalInvitationRepository>();
    configurationContext.Services.AddTransient<IMetadataResourceRepository, MetadataResourceRepository>();
    configurationContext.Services.AddTransient<ICRA.IICRARepository, ICRA.ICRARepository>();
    configurationContext.Services.AddTransient<IPostSecondaryInstituteRepository, PostSecondaryInstituteRepository>();
    configurationContext.Services.AddTransient<IProgramRepository, ProgramRepository>();
  }
}
