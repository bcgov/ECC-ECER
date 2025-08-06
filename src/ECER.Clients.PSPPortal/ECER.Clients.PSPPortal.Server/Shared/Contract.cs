namespace ECER.Clients.PSPPortal.Server.Shared;

public record ClaimCacheSettings
{
  public double CacheTimeInSeconds { get; set; } = 500;
}
