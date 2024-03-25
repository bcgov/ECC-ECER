namespace ECER.Infrastructure.Common;

public static class StreamExtensions
{
  /// <summary>
  /// Copies a stream into a memory byte array and return the original stream position to 0
  /// </summary>
  /// <param name="stream">The original stream</param>
  /// <returns>A copy of the original stream</returns>
  public static async Task<Memory<byte>> CloneAsync(this Stream stream)
  {
    var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    return new Memory<byte>(ms.ToArray());
  }
}
