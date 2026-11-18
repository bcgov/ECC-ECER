using Mediator;
using System.Text.Json.Serialization;

namespace ECER.Managers.Registry.Contract.Captcha;
public record VerifyCaptchaCommand(string captchaToken, string? remoteIp = null, string? expectedAction = null) : IRequest<CaptchaResponse>;

public record CaptchaResponse
{
  public bool Success { get; set; }
  [JsonPropertyName("hostname")]
  public string? Hostname { get; set; }
  [JsonPropertyName("action")]
  public string? Action { get; set; }
  [JsonPropertyName("error-codes")]
  public IEnumerable<string> ErrorCodes { get; set; } = Array.Empty<string>();
}
