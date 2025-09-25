using ECER.Managers.Registry.Contract.Captcha;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace ECER.Managers.Registry
{
  public class CaptchaHandlers(IHttpClientFactory httpClientFactory, IOptions<CaptchaAppSettings> captchaAppSettings) : IRequestHandler<VerifyCaptchaCommand, CaptchaResponse>
  {
    /// <summary>
    /// Handles verifying a captcha token
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    public async Task<CaptchaResponse> Handle(VerifyCaptchaCommand request, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(request);

      var parameters = new Dictionary<string, string>()
  {
            { "secret", captchaAppSettings.Value.Secret },
            { "response", request.captchaToken }
        };

      var content = new FormUrlEncodedContent(parameters);
      var httpClient = httpClientFactory.CreateClient("captcha");
      var response = await httpClient.PostAsync(new Uri(captchaAppSettings.Value.Url), content, cancellationToken);
      content.Dispose();
      response.EnsureSuccessStatusCode();
      var captchaResponse = await response.Content.ReadFromJsonAsync<CaptchaResponse>(cancellationToken);

      return captchaResponse!;
    }
  }
}
