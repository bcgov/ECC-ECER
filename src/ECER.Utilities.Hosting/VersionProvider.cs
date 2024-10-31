using System;

namespace ECER.Utilities.Hosting;

public static class VersionProvider
{
  public static VersionMetadata? GetVersion()
  {
    var version = Environment.GetEnvironmentVariable("VERSION");
    var timestamp = Environment.GetEnvironmentVariable("TIMESTAMP");
    var commit = Environment.GetEnvironmentVariable("COMMIT");

    return new VersionMetadata
    {
      Version = version,
      Timestamp = timestamp,
      Commit = commit,
    };
  }
}

public record VersionMetadata
{
  public string? Version { get; set; }
  public string? Timestamp { get; set; }
  public string? Commit { get; set; }
}
