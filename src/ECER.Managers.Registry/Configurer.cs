using ECER.Infrastructure.Common;
using ECER.Managers.Registry.Contract.Registrants;
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
    configurationContext.Services.AddTransient<RegistrantHandlers>();
    configurationContext.Services.AddTransient<PortalInvitationHandlers>();
    configurationContext.Services.AddTransient<CaptchaHandlers>();
    configurationContext.Services.Configure<CaptchaAppSettings>(captchaAppSettings =>
    configurationContext.Configuration.GetSection("Captcha").Bind(captchaAppSettings));
    configurationContext.Services.AddTransient<BceidRegistrationIdentityService>();
    configurationContext.Services.AddTransient<BcscRegistrationIdentityService>();
    configurationContext.Services.AddTransient<IRegistrationIdentityService>(provider =>
    { return provider.GetRequiredService<BcscRegistrationIdentityService>(); });
  }
}

public record CaptchaAppSettings : IOptions<CaptchaAppSettings>
{
  public string Url { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;
  public CaptchaAppSettings Value => this;
}
