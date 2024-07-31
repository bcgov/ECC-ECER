namespace ECER.Utilities.FileScanner;

public interface IFileScannerProvider
{
  Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default);
}

public record ScanResult(bool IsClean, string Message);
