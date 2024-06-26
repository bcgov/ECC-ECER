using MediatR;
using System.Text.Json.Serialization;

namespace ECER.Managers.Registry.Contract.Recaptcha;

public record VerifyRecaptchaCommand(string RecaptchaToken) : IRequest<RecaptchaResponse>;

public record RecaptchaResponse
{
  public bool Success { get; set; }
  [JsonPropertyName("error-codes")]
  public IEnumerable<string> ErrorCodes { get; set; } = Array.Empty<string>();
}
