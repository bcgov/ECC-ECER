namespace ECER.Infrastructure.Common.Validators;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class RouterValidationExtensions
{
  public static RouteHandlerBuilder AddGuidValidation(this RouteHandlerBuilder builder, string parameterName, bool isRequired = true)
  {
    return builder.AddEndpointFilter(async (context, next) =>
    {
      var value = context.HttpContext.Request.RouteValues[parameterName]?.ToString();

      if (string.IsNullOrWhiteSpace(value) && isRequired)
      {
        return Results.BadRequest($"{parameterName} is required cannot be null or whitespace");
      }

      if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out _))
      {
        return Results.BadRequest($"{parameterName} must be a valid GUID");
      }

      return await next(context);
    });
  }

  public static RouteHandlerBuilder AddGuidValidationQueryParams(this RouteHandlerBuilder builder, string[] parameterNames, bool isRequired = true)
  {
    return builder.AddEndpointFilter(async (context, next) =>
    {
      var errors = new List<string>();

      foreach (var queryParameter in parameterNames)
      {
        var value = context.HttpContext.Request.Query[queryParameter].ToString();

        if (string.IsNullOrWhiteSpace(value) && isRequired)
        {
          errors.Add($"{queryParameter} is required cannot be null or whitespace");
        }

        if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out _))
        {
          errors.Add($"{queryParameter} must be a valid GUID");
        }
      }

      if (errors.Count > 0)
      {
        return Results.BadRequest(errors);
      }

      return await next(context);
    });
  }
}
