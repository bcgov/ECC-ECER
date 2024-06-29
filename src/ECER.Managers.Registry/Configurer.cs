﻿using ECER.Infrastructure.Common;
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
    configurationContext.Services.AddTransient<RecaptchaHandlers>();
    configurationContext.Services.Configure<RecaptchaAppSettings>(recaptchaAppSettings =>
    configurationContext.Configuration.GetSection("Recaptcha").Bind(recaptchaAppSettings));
  }
}

public record RecaptchaAppSettings : IOptions<RecaptchaAppSettings>
{
  public string Url { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;
  public RecaptchaAppSettings Value => this;
}
