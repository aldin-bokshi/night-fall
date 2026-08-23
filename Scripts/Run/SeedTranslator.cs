using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace NightFall.Scripts.Run;

public static class SeedTranslator
{
    private const string EmptySeed = "EMPTY_SEED";

    public static ulong ToNumericSeed(string seedText)
    {
        string seed = string.IsNullOrEmpty(seedText)
            ? EmptySeed
            : seedText;

        byte[] hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(seed));

        return BinaryPrimitives.ReadUInt64LittleEndian(hash);
    }

    public static string ToDisplaySeed(ulong numericSeed)
    {
        return numericSeed.ToString(
            "X16",
            CultureInfo.InvariantCulture);
    }
}