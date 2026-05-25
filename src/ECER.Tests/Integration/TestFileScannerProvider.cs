using ECER.Utilities.FileScanner;

namespace ECER.Tests.Integration;

internal sealed class TestFileScannerProvider : IFileScannerProvider
{
  public Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default) => Task.FromResult(new ScanResult(true, string.Empty));
}
