using ECER.Managers.Registry.Contract.Captcha;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECER.Managers.Registry
{
  public class CaptchaHandlers(IHttpClientFactory httpClientFactory, IOptions<CaptchaAppSettings> captchaAppSettings, ILogger<CaptchaHandlers> logger) : IRequestHandler<VerifyCaptchaCommand, CaptchaResponse>
  {
    public const string ServiceUnavailableErrorCode = "captcha-service-unavailable";
    public const string HostnameMismatchErrorCode = "hostname-mismatch";
    public const string ActionMismatchErrorCode = "action-mismatch";

    /// <summary>
    /// Handles verifying a captcha token. Always fails closed.
    /// </summary>
    public async ValueTask<CaptchaResponse> Handle(VerifyCaptchaCommand request, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(request);
      var settings = captchaAppSettings.Value;

      var parameters = new Dictionary<string, string>
      {
        { "secret", settings.Secret },
        { "response", request.captchaToken }
      };
      if (!string.IsNullOrWhiteSpace(request.remoteIp)) parameters["remoteip"] = request.remoteIp;

      CaptchaResponse? captchaResponse;
      try
      {
        using var content = new FormUrlEncodedContent(parameters);
        var httpClient = httpClientFactory.CreateClient("captcha");
        using var response = await httpClient.PostAsync(new Uri(settings.Url), content, cancellationToken);
        response.EnsureSuccessStatusCode();
        captchaResponse = await response.Content.ReadFromJsonAsync<CaptchaResponse>(cancellationToken);
      }
      catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or NotSupportedException && !cancellationToken.IsCancellationRequested)
      {
        logger.LogError(ex, "Captcha verification service failed; rejecting token");
        return Failure(ServiceUnavailableErrorCode);
      }

      if (captchaResponse == null)
      {
        logger.LogError("Captcha verification service returned an empty response; rejecting token");
        return Failure(ServiceUnavailableErrorCode);
      }
      if (!captchaResponse.Success) return captchaResponse;

      if (settings.ExpectedHostnames.Count > 0 &&
          !settings.ExpectedHostnames.Contains(captchaResponse.Hostname, StringComparer.OrdinalIgnoreCase))
      {
        logger.LogWarning("Captcha hostname {Hostname} is not an expected hostname", captchaResponse.Hostname);
        return Failure(HostnameMismatchErrorCode);
      }

      if (request.expectedAction != null && !string.Equals(request.expectedAction, captchaResponse.Action, StringComparison.Ordinal))
      {
        logger.LogWarning("Captcha action {Action} did not match expected action {ExpectedAction}", captchaResponse.Action, request.expectedAction);
        return Failure(ActionMismatchErrorCode);
      }

      return captchaResponse;
    }

    private static CaptchaResponse Failure(string errorCode) => new() { Success = false, ErrorCodes = [errorCode] };
  }
}
