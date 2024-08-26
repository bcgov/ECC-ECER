using nClam;

namespace ECER.Utilities.FileScanner.Providers;

public class ClamAvScanner(IClamClient clamClient) : IFileScannerProvider
{
  public async Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default)
  {
    var scanResult = await clamClient.SendAndScanFileAsync(stream, ct);

    return scanResult.Result switch
    {
      ClamScanResults.Clean => new ScanResult(true, "File is clean."),
      ClamScanResults.VirusDetected => new ScanResult(false, $"Virus detected: {scanResult.InfectedFiles?[0].VirusName}"),
      ClamScanResults.Error => new ScanResult(false, $"Error during scan: {scanResult.RawResult}"),
      _ => throw new NotImplementedException($"Result {scanResult.Result}")
    };
  }
}
