using ECER.Managers.Registry.Contract.Recaptcha;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace ECER.Managers.Registry
{
  public class RecaptchaHandlers(IHttpClientFactory httpClientFactory, IOptions<RecaptchaAppSettings> recaptchaAppSettings) : IRequestHandler<VerifyRecaptchaCommand, RecaptchaResponse>
  {
    /// <summary>
    /// Handles verifying a recaptcha token
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    public async Task<RecaptchaResponse> Handle(VerifyRecaptchaCommand request, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(request);

      var parameters = new Dictionary<string, string>()
      {
                { "secret", recaptchaAppSettings.Value.Secret },
                { "response", request.RecaptchaToken }
            };

      var content = new FormUrlEncodedContent(parameters);
      var httpClient = httpClientFactory.CreateClient("recaptcha");
      var response = await httpClient.PostAsync(new Uri(recaptchaAppSettings.Value.Url), content, cancellationToken);
      content.Dispose();
      response.EnsureSuccessStatusCode();
      var recaptchaResponse = await response.Content.ReadFromJsonAsync<RecaptchaResponse>(cancellationToken);

      return recaptchaResponse!;
    }
  }
}
