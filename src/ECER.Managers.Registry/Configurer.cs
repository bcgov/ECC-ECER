using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Managers.Registry.PortalInvitations;
using ECER.Managers.Registry.UserRegistrationIdentityService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Managers.Registry;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<ApplicationHandlers>();
    configurationContext.Services.AddTransient<CommunicationHandlers>();
    configurationContext.Services.AddTransient<IApplicationMapper, ApplicationMapper>();
    configurationContext.Services.AddTransient<ICommunicationMapper, CommunicationMapper>();
    configurationContext.Services.AddTransient<ICertificationMapper, CertificationMapper>();
    configurationContext.Services.AddTransient<ICoursesMapper, CoursesMapper>();
    configurationContext.Services.AddTransient<ICRAEligibilityHandlers>();
    configurationContext.Services.AddTransient<IICRAEligibilityMapper, ICRAEligibilityMapper>();
    configurationContext.Services.AddTransient<IPostSecondaryInstituteMapper, PostSecondaryInstituteMapper>();
    configurationContext.Services.AddTransient<IProgramApplicationMapper, ProgramApplicationMapper>();
    configurationContext.Services.AddTransient<IProgramMapper, ProgramMapper>();
    configurationContext.Services.AddTransient<IPspUserMapper, PspUserMapper>();
    configurationContext.Services.AddTransient<IRegistrantMapper, RegistrantMapper>();
    configurationContext.Services.AddTransient<RegistrantHandlers>();
    configurationContext.Services.AddTransient<ProgramHandlers>();
    configurationContext.Services.AddTransient<CoursesHandler>();
    configurationContext.Services.AddTransient<PostSecondaryInstituteHandlers>();
    configurationContext.Services.AddTransient<ProgramApplicationHandler>();
    configurationContext.Services.AddTransient<PspUserHandlers>();
    configurationContext.Services.AddTransient<PortalInvitationHandlers>();
    configurationContext.Services.AddTransient<IPortalInvitationMapper, PortalInvitationMapper>();
    configurationContext.Services.AddTransient<IPortalInvitationVerificationHandler, ReferencePortalInvitationVerificationHandler>();
    configurationContext.Services.AddTransient<IPortalInvitationVerificationHandler, IcraReferencePortalInvitationVerificationHandler>();
    configurationContext.Services.AddTransient<IPortalInvitationVerificationHandler, PspPortalInvitationVerificationHandler>();
    configurationContext.Services.AddTransient<CaptchaHandlers>();
    configurationContext.Services.Configure<CaptchaAppSettings>(captchaAppSettings =>
      configurationContext.Configuration.GetSection("Captcha").Bind(captchaAppSettings));
    configurationContext.Services.AddTransient<BceidRegistrationIdentityService>();
    configurationContext.Services.AddTransient<BcscRegistrationIdentityService>();
    configurationContext.Services.AddTransient<IRegistrationIdentityService>(provider => provider.GetRequiredService<BcscRegistrationIdentityService>());
  }
}

public record CaptchaAppSettings : IOptions<CaptchaAppSettings>
{
  public string Url { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;
  public CaptchaAppSettings Value => this;
}
