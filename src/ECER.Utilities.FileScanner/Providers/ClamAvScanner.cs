using nClam;

namespace ECER.Utilities.FileScanner.Providers;

public class ClamAvScanner : IFileScannerProvider
{
  private readonly ClamClient _clamClient;

  public ClamAvScanner(ClamClient clamClient)
  {
    _clamClient = clamClient ?? throw new ArgumentNullException(nameof(clamClient));
  }

  public async Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default)
  {
    try
    {
      var scanResult = await _clamClient.SendAndScanFileAsync(stream, ct);

      return scanResult.Result switch
      {
        ClamScanResults.Clean => new ScanResult(true, "File is clean."),
        ClamScanResults.VirusDetected => new ScanResult(false, $"Virus detected: {scanResult.InfectedFiles?.First().VirusName}"),
        ClamScanResults.Error => new ScanResult(false, $"Error during scan: {scanResult.RawResult}"),
        _ => new ScanResult(false, "Unknown result from ClamAV.")
      };
    }
    catch (Exception ex)
    {
      return new ScanResult(false, $"Exception occurred: {ex.Message}");
    }
  }
}
