using nClam;
using System.Diagnostics;

namespace ECER.Utilities.FileScanner.Providers;

public class ClamAvScanner(IClamClient clamClient) : IFileScannerProvider
{
  internal static readonly string TraceSourceName = typeof(ClamAvScanner).FullName!;
  private static ActivitySource source = new(TraceSourceName);

  public async Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default)
  {
    ArgumentNullException.ThrowIfNull(stream);

    using var activity = source.StartActivity(nameof(ScanAsync));
    activity?.AddTag(nameof(stream.Length), stream.Length);

    var scanResult = await clamClient.SendAndScanFileAsync(stream, ct);

    activity?.SetTag(nameof(scanResult.Result), scanResult.Result);

    var result = scanResult.Result switch
    {
      ClamScanResults.Clean => new ScanResult(true, "File is clean."),
      ClamScanResults.VirusDetected => new ScanResult(false, $"Virus detected: {scanResult.InfectedFiles?[0].VirusName}"),
      ClamScanResults.Error => new ScanResult(false, $"Error during scan: {scanResult.RawResult}"),
      _ => throw new NotImplementedException($"Result {scanResult.Result}")
    };

    return result;
  }
}
