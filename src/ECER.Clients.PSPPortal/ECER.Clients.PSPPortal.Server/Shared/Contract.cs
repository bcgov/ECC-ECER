namespace ECER.Clients.PSPPortal.Server.Shared;

public record ClaimCacheSettings
{
  public double CacheTimeInSeconds { get; set; } = 500;
}

public record UploaderSettings
{
  public string TempFolderName { get; set; } = string.Empty;
  public IEnumerable<string> AllowedFileTypes { get; set; } = Array.Empty<string>();
  public int DefaultPageSize { get; set; }
  public int DefaultPageNumber { get; set; }
  public string PageProperty { get; set; } = string.Empty;
  public string PageSizeProperty { get; set; } = string.Empty;
}
