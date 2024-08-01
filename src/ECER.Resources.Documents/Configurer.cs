using ECER.Infrastructure.Common;
using ECER.Resources.Documents.Applications;
using ECER.Resources.Documents.Applications.ChildrenServices;
using ECER.Resources.Documents.Certifications;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Resources.Documents;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ICertificationRepository, CertificationRepository>();
    configurationContext.Services.AddTransient<IApplicationRepository, ApplicationRepository>();
    configurationContext.Services.AddTransient<IPortalInvitationRepository, PortalInvitationRepository>();
    configurationContext.Services.AddTransient<IMetadataResourceRepository, MetadataResourceRepository>();
    configurationContext.Services.AddTransient<IApplicationChildService<ecer_ProfessionalDevelopment>, ProfessionalDevelopmentsService>();
    configurationContext.Services.AddTransient<IApplicationChildService<ecer_WorkExperienceRef>, WorkExperienceReferencesService>();
    configurationContext.Services.AddTransient<IApplicationChildService<ecer_CharacterReference>, CharacterReferencesService>();
    configurationContext.Services.AddTransient<IApplicationChildService<ecer_Transcript>, TranscriptsService>();
  }
}
