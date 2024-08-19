namespace ECER.Utilities.FileScanner.Providers;

internal class NoopScanner : IFileScannerProvider
{
  public Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default) => Task.FromResult(new ScanResult(true, string.Empty));
}
