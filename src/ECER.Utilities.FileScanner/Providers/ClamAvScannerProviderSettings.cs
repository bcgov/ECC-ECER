namespace ECER.Utilities.FileScanner.Providers;

internal record ClamAvProviderSettings
{
  public string? Url { get; set; }
  public int Port { get; set; } = 3310;
}
