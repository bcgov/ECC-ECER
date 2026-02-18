namespace ECER.Infrastructure.Common.Validators;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class RouterValidationExtensions
{
  public static RouteHandlerBuilder AddGuidValidation(this RouteHandlerBuilder builder, string parameterName)
  {
    return builder.AddEndpointFilter(async (context, next) =>
    {
      var value = context.HttpContext.Request.RouteValues[parameterName]?.ToString();

      if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out _))
      {
        return Results.BadRequest($"The parameter '{parameterName}' must be a valid GUID.");
      }

      return await next(context);
    });
  }
}
