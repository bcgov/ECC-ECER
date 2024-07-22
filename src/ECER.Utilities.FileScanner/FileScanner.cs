using nClam;

namespace ECER.Utilities.Security;

public class FileScanner : IFileScanner
{
  private readonly ClamClient _clamClient;

  public FileScanner(ClamClient clamClient)
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
        ClamScanResults.Clean => new ScanResult { IsClean = true, Message = "File is clean." },
        ClamScanResults.VirusDetected => new ScanResult { IsClean = false, Message = $"Virus detected: {scanResult.InfectedFiles?.First().VirusName}" },
        ClamScanResults.Error => new ScanResult { IsClean = false, Message = $"Error during scan: {scanResult.RawResult}" },
        _ => new ScanResult { IsClean = false, Message = "Unknown result from ClamAV." }
      };
    }
    catch (Exception ex)
    {
      return new ScanResult { IsClean = false, Message = $"Exception occurred: {ex.Message}" };
    }
  }
}

public interface IFileScanner
{
  Task<ScanResult> ScanAsync(Stream stream, CancellationToken ct = default);
}

public class ScanResult
{
  public bool IsClean { get; set; }
  public string Message { get; set; } = string.Empty;
}
