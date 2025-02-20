namespace ECER.Clients.RegistryPortal.Server.Shared;

public record PaginationSettings
{
  public int DefaultPageSize { get; set; }
  public int DefaultPageNumber { get; set; }
  public string PageProperty { get; set; } = string.Empty;
  public string PageSizeProperty { get; set; } = string.Empty;
}

public record UploaderSettings
{
  public string TempFolderName { get; set; } = string.Empty;
  public IEnumerable<string> AllowedFileTypes { get; set; } = Array.Empty<string>();
}

public record RecaptchaSettings
{
  public string SiteKey { get; set; } = string.Empty;
}
public record ClaimCacheSettings
{
  public double CacheTimeInSeconds { get; set; } = 500;
}
