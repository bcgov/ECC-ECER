using Bogus.DataSets;

namespace ECER.Tests;

public static class BogusExtensions
{
    public static async Task<Memory<byte>> ByteArray(this Lorem dataset, int size)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms);
        while (ms.Length < size)
        {
            await writer.WriteAsync(dataset.Text());
        }

        return new Memory<byte>(ms.ToArray(), 0, size);
    }
}