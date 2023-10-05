using System.Text;
using JetBrains.Annotations;

namespace DKrOSS.Core.Extensions;

public static class ByteExtensions
{
    [Pure]
    public static string ToHexString(this IEnumerable<byte> bytes)
    {
        var sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}
