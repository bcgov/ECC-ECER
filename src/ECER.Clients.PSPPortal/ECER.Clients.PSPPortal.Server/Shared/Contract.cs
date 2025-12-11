namespace ECER.Clients.PSPPortal.Server.Shared;

public record ClaimCacheSettings
{
  public double CacheTimeInSeconds { get; set; } = 500;
}

public record PaginationSettings
{
  public int DefaultPageSize { get; set; }
  public int DefaultPageNumber { get; set; }
  public string PageProperty { get; set; } = string.Empty;
  public string PageSizeProperty { get; set; } = string.Empty;
}
