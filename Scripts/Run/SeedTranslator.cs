using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

namespace NightFall.Scripts.Run;

public static class SeedTranslator
{
    private const string EmptySeed = "EMPTY_SEED";

    public static ulong ToNumericSeed(string seedText)
    {
        string normalizedSeed = string.IsNullOrEmpty(seedText)
            ? EmptySeed
            : seedText;

        byte[] hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(normalizedSeed));

        return BinaryPrimitives.ReadUInt64LittleEndian(hash);
    }

    public static string ToDisplaySeed(ulong numericSeed)
    {
        return numericSeed.ToString("X16", CultureInfo.InvariantCulture);
    }
}