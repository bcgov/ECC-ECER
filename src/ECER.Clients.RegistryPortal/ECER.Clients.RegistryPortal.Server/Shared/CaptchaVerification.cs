using ECER.Managers.Registry.Contract.Captcha;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace ECER.Clients.RegistryPortal.Server.Shared;

public static class CaptchaActions
{
  public const string ReferenceSubmission = "reference-submission";
  public const string CertificationLookup = "certification-lookup";
}

public static class CaptchaVerification
{
  public const string InvalidCaptchaTokenDetail = "Invalid captcha token";

  /// <summary>
  /// Verifies the captcha token. Returns null when valid, otherwise the 400 problem details.
  /// </summary>
  public static async Task<ProblemDetails?> VerifyCaptchaAsync(this IMediator messageBus, HttpContext httpContext, string captchaToken, string expectedAction, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(httpContext);
    var result = await messageBus.Send(new VerifyCaptchaCommand(captchaToken, httpContext.Connection.RemoteIpAddress?.ToString(), expectedAction), ct);
    if (result.Success) return null;

    return new ProblemDetails
    {
      Status = StatusCodes.Status400BadRequest,
      Detail = InvalidCaptchaTokenDetail,
      Extensions = { ["errors"] = result.ErrorCodes }
    };
  }
}
