﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ECER.Utilities.Hosting;

public static class WebApplicationExtensions
{
  public static void UseCsp(this WebApplication webApplication)
  {
    var settings = webApplication.Services.GetRequiredService<IOptions<CspSettings>>();
    UseCsp(webApplication, settings.Value);
  }

  public static void UseCsp(this WebApplication webApplication, CspSettings settings)
  {
    ArgumentNullException.ThrowIfNull(settings);

    var cspString = new StringValues(settings.ToCsppString());
    webApplication.Use(async (context, next) =>
    {
      context.Response.Headers.Append("Content-Security-Policy", cspString);
      await next();
    });
  }

  public static void UseSecurityHeaders(this WebApplication webApplication)
  {
    webApplication.Use(async (context, next) =>
    {
      context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
      context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
      context.Response.Headers.Append("X-Frame-Protection", "DENY");
      await next();
    });
  }

  public static void UseDisableHttpVerbs(this WebApplication webApplication, IEnumerable<string> verbs)
  {
    var list = verbs.ToArray();
    webApplication.Use(async (context, next) =>
    {
      if (list.Contains(context.Request.Method.ToUpperInvariant()))
      {
        context.Response.StatusCode = 405;
        return;
      }

      await next.Invoke();
    });
  }
}

public record CspSettings : IOptions<CspSettings>
{
  public string BaseUri { get; set; } = string.Empty;
  public string DefaultSource { get; set; } = string.Empty;
  public string ScriptSource { get; set; } = string.Empty;
  public string ConnectSource { get; set; } = string.Empty;
  public string ImageSource { get; set; } = string.Empty;
  public string StyleSource { get; set; } = string.Empty;
  public string FontSource { get; set; } = string.Empty;
  public string FrameAncestors { get; set; } = string.Empty;
  public string FormAction { get; set; } = string.Empty;

  public CspSettings Value => this;

  public string ToCsppString() =>
    (string.IsNullOrWhiteSpace(BaseUri) ? "" : $"base-uri {BaseUri};") +
    (string.IsNullOrWhiteSpace(DefaultSource) ? "" : $"default-src {DefaultSource};") +
    (string.IsNullOrWhiteSpace(ScriptSource) ? "" : $"script-src {ScriptSource};") +
    (string.IsNullOrWhiteSpace(ConnectSource) ? "" : $"connect-src {ConnectSource};") +
    (string.IsNullOrWhiteSpace(ImageSource) ? "" : $"img-src {ImageSource};") +
    (string.IsNullOrWhiteSpace(StyleSource) ? "" : $"style-src {StyleSource};") +
    (string.IsNullOrWhiteSpace(FontSource) ? "" : $"font-src {this.FontSource};") +
    (string.IsNullOrWhiteSpace(FrameAncestors) ? "" : $"frame-ancestors {this.FrameAncestors};") +
    (string.IsNullOrWhiteSpace(FormAction) ? "" : $"form-action {this.FormAction};")
  ;
}
