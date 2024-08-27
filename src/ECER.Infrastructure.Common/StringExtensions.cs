using System.Security;

namespace ECER.Infrastructure.Common;

public static class StringExtensions
{
  /// <summary>
  /// Converts an unsecure string to a <see cref="SecureString"/>.
  /// </summary>
  /// <param name="unsecureString">The unsecure string to operate on</param>
  /// <returns><see cref="SecureString"/></returns>
  public static SecureString? ToSecureString(this string? unsecureString)
  {
    if (unsecureString == null) return null;

#pragma warning disable CA2000 // Dispose objects before losing scope
    return unsecureString.Aggregate(
        new SecureString(),
        (s, c) =>
        {
          s.AppendChar(c);
          return s;
        },
        (s) =>
        {
          s.MakeReadOnly();
          return s;
        });
#pragma warning restore CA2000 // Dispose objects before losing scope
  }
}
