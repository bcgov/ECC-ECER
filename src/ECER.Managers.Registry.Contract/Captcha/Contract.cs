using MediatR;
using System.Text.Json.Serialization;

namespace ECER.Managers.Registry.Contract.Captcha;
public record VerifyCaptchaCommand(string captchaToken) : IRequest<CaptchaResponse>;

public record CaptchaResponse
{
  public bool Success { get; set; }
  [JsonPropertyName("error-codes")]
  public IEnumerable<string> ErrorCodes { get; set; } = Array.Empty<string>();
}
