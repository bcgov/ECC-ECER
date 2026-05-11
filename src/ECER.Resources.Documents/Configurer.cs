using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Certifications;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.PortalInvitations;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.ICRA;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using ECER.Resources.Documents.ProgramApplications;
using ECER.Resources.Documents.Programs;

namespace ECER.Resources.Documents;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ICertificationRepositoryMapper, CertificationRepositoryMapper>();
    configurationContext.Services.AddTransient<IApplicationRepositoryMapper, ApplicationRepositoryMapper>();
    configurationContext.Services.AddTransient<ICourseRepositoryMapper, CourseRepositoryMapper>();
    configurationContext.Services.AddTransient<IMetadataResourceRepositoryMapper, MetadataResourceRepositoryMapper>();
    configurationContext.Services.AddTransient<IPortalInvitationRepositoryMapper, PortalInvitationMapper>();
    configurationContext.Services.AddTransient<IICRARepositoryMapper, IcraRepositoryMapper>();
    configurationContext.Services.AddTransient<IPostSecondaryInstituteRepositoryMapper, PostSecondaryInstituteRepositoryMapper>();
    configurationContext.Services.AddTransient<IProgramApplicationRepositoryMapper, ProgramApplicationRepositoryMapper>();
    configurationContext.Services.AddTransient<IProgramRepositoryMapper, ProgramRepositoryMapper>();
    configurationContext.Services.AddTransient<ICertificationRepository, CertificationRepository>();
    configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
    configurationContext.Services.AddTransient<IPortalInvitationRepository, PortalInvitationRepository>();
    configurationContext.Services.AddTransient<IMetadataResourceRepository, MetadataResourceRepository>();
    configurationContext.Services.AddTransient<ICRA.IICRARepository, ICRA.ICRARepository>();
    configurationContext.Services.AddTransient<IPostSecondaryInstituteRepository, PostSecondaryInstituteRepository>();
    configurationContext.Services.AddTransient<IProgramRepository, ProgramRepository>();
    configurationContext.Services.AddTransient<IProgramApplicationRepository, ProgramApplicationRepository>();
    configurationContext.Services.AddTransient<ICourseRepository, CourseRepository>();
  }
}
